using Caradhras.Tasks.Domain.Entities;
using Caradhras.Tasks.Repository;
using Caradhras.Tasks.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Service.Services
{
    public class ScheduledService : IScheduledService
    {
        private readonly ISFTPService _sFTPService;
        private readonly ITaskRepository _taskRepository;
        private readonly ICashBackRepository _cashBackRepository;
        private readonly ITasksServices _tasksServices;
        private AppSettings _appSettings;
        private ILogger _logger;

        public ScheduledService(ISFTPService sFTPService
            , ITaskRepository taskRepository
            , ICashBackRepository cashBackRepository
            , ITasksServices tasksServices
            , ILogger<TaskRepository> logger, IOptions<AppSettings> appSettings)
        {
            _cashBackRepository = cashBackRepository;
            _sFTPService = sFTPService;
            _taskRepository = taskRepository;
            _tasksServices = tasksServices;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task CheckReceiptsRequest()
        {
            try
            {
                var tasks = await _taskRepository.CashbackExist();

                if (!tasks.Any())
                {
                    _logger.LogDebug("Não há comprovantes a solicitar;");
                    return;
                }
                var yesterday = DateTime.Now.AddDays(-1);
                var limitdate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day).AddHours(15);

                var itemsonlybefore15 = tasks.Where(x => x.CreatedAt <= limitdate);
                var groupedtItems = itemsonlybefore15.GroupBy(x => x.CreatedAt.Date, x => x.OrderRequestId)
                                      .ToDictionary(x => x.Key, x => x.ToList()).OrderBy(x => x.Key);


                foreach (KeyValuePair<DateTime, List<int>> item in groupedtItems)
                {

                    var filter = new Filter
                    {
                        Servico = ServiceEnum.BANKTRANSFEROUT,
                        Date = item.Key
                    };

                    var requestresult = await _sFTPService.RequestFile(filter);

                    if (!requestresult.Sucess)
                    {
                        _logger.LogDebug("Não foi possivel solicitar arquivo;");
                        return;
                    }

                    var control = new ScheduledControl
                    {
                        Ticket = requestresult.Data.ticket,
                        StartExecution = item.Key,
                        Status = "Incluido",
                        Active = true,
                        CreatedBy = "Sistema",
                        CreatedAt = DateTime.Now
                    };

                    var idcontrol = await _tasksServices.InsertControl(control);
                    control.Items = _tasksServices.SetItems(item.Value, idcontrol);

                    await _tasksServices.InsertTaskControlItems(control.Items);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao solicitar comprovantes");
            }
        }

        public async Task GetReceipts()
        {
            try
            {
                var requests = await _taskRepository.GetAvailableTasks();

                if (requests == null || !requests.Any())
                {
                    return;
                }

                foreach (var item in requests)
                {
                    var receipt = await _sFTPService.DownloadFile(item.Ticket);

                    if (!receipt.Sucess)
                        continue;


                    /*
                     lição de casa: 

                    trocar o update individual por update em lote
                    exemplo:

                    select * into #tempcashbacknew  from CashBackReceipt where idadjustment in (<sua lista aqui>) 

                    update A
                    set codStatusTransfer = T.codStatusTransfer
                    FROM CashBackReceipt A
                    INNER JOIN #tempcashbacknew T on A.Id = T.Id
                     
                     */

                    await _tasksServices.UpdateTedReceipt(receipt.Data.Items);

                    await _tasksServices.UpdateControl(item.Id, "Concluido", false);

                }


            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Falha ao baixar comprovantes");
            }
        }



    }
}

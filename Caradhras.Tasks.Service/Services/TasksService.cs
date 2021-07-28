using Caradhras.Tasks.Domain.Entities;
using Caradhras.Tasks.Repository;
using Caradhras.Tasks.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Service.Services
{
    public class TasksService : ITasksServices
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICashBackRepository _cashBackRepository;
        private AppSettings _appSettings;
        private ILogger _logger;

        public TasksService(ITaskRepository taskRepository, ICashBackRepository cashBackRepository, IOptions<AppSettings> appSettings, ILogger<TasksService> logger)
        {
            _taskRepository = taskRepository;
            _cashBackRepository = cashBackRepository;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task InsertTaskControlItems(List<ScheduledControlItem> items)
        {
            try
            {

                foreach (var item in items)
                {
                    await _taskRepository.InsertTaskControlItem(item);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "falha ao inserir o item de taskcontrol");

            }
        }
        public List<ScheduledControlItem> SetItems(List<int> items, int id)
        {
            var result = new List<ScheduledControlItem>();

            foreach (var item in items)
            {
                var sch = new ScheduledControlItem();
                sch.TaskControlId = id;
                sch.OrderRequestId = item;
                sch.CreatedBy = "Sistema";
                sch.CreatedAt = DateTime.Now;
                result.Add(sch);
            }
            return result; ;
        }

        public async Task UpdateTedReceipt(List<TedResultItem> tedsresult)
        {

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



            foreach (var item in tedsresult)
            {
                try
                {

                    var localTed = await _cashBackRepository.GetByIdAdjusment(item.idAdjustment);

                    if (localTed == null)
                    {
                        continue;
                    }
                    localTed.statusTransfer = item.statusTransfer;
                    localTed.transferenceDate = item.transferenceDate.GetValueOrDefault();
                    localTed.transferSuccess = item.transferSuccess.GetValueOrDefault();
                    localTed.codStatusTransfer = item.codStatusTransfer;
                    localTed.ChangedAt = DateTime.Now;
                    localTed.ChangedBy = "Sistema";
                    localTed.Success = localTed.transferSuccess;


                    await _cashBackRepository.UpdateCashBackReceipt(localTed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao atualizar resultado da Ted");
                }

            }
        }

        public List<TedResultItem> FilterAndSetTedResultItem(DownloadResult file)
        {
            var result = new List<TedResultItem>();

            foreach (var item in file.Items)
            {
                var description = item.description.ToUpper();
                if (description.Contains("CASHBACK"))
                    result.Add(item);
            }

            return result;
        }

        public async Task UpdateControl(int id, string status, bool active)
        {
            await _taskRepository.UpdateStepTaskControl(id, status, active);
        }

        public async Task<int> InsertControl(ScheduledControl scheduled)
        {
            return await _taskRepository.InsertTaskControl(scheduled);
        }
    }
}

using Caradhras.Tasks.Domain.Entities;
using Caradhras.Tasks.Repository;
using Caradhras.Tasks.Service.Interfaces;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Service.Services
{
    public class SFTPService : ISFTPService
    {
        private readonly ITasksServices _tasksServices;
        private readonly ITaskRepository _taskRepository;
        private readonly ICaradhrasRepository _caradhrasRepository;
        string datastraction = "/v1/transactions";
        AppSettings _appSettings;
        ILogger _logger;



        public SFTPService(ICaradhrasRepository caradhrasRepository
            , ITasksServices tasksServices
            , IOptions<AppSettings> appSettings
            , ILogger<SFTPService> logger
            , ITaskRepository taskRepository)
        {
            _caradhrasRepository = caradhrasRepository;
            _taskRepository = taskRepository;
            _tasksServices = tasksServices;
            _appSettings = appSettings.Value;
            _logger = logger;


        }

        public async Task<ResultSimple<Ticket>> RequestFile(Filter filter)
        {
            var result = new ResultSimple<Ticket>();
            string querystring = string.Empty;
            try
            {
                querystring = QueryStringCompose(filter);
                var token = _caradhrasRepository.AutenticacaoCaradhras();
                var service = $"{_appSettings.DataApi}?{querystring}";
                result = await _caradhrasRepository.ConsumeServicesGET<Ticket>(service, token.access_token);

                if (!result.Sucess)
                {

                    result.Messages.Add("Falha na solicitação do arquivo");
                    if (result.Data != null)
                        result.Messages.Add($"campo:{result.Data.field} erro:{result.Data.message}");
                }




            }
            catch (Exception ex)
            {
                var msg = $"Falha ao solicitar o arquivo conforme a query {querystring}";
                _logger.LogError(ex, msg);
                result.Messages.Add(msg);
            }

            return result;
        }


        public async Task<ResultSimple<Ticket>> CheckRequest(string request)
        {
            var result = new ResultSimple<Ticket>();
            try
            {
                var token = _caradhrasRepository.AutenticacaoCaradhras();
                var service = $"{_appSettings.DataApi}/{request}";
                result = await _caradhrasRepository.ConsumeServicesGET<Ticket>(service, token.access_token);
            }
            catch (Exception ex)
            {
                var msg = $"Falha ao solicitar o arquivo conforme o ticket {request}";
                _logger.LogError(ex, msg);
                result.Messages.Add(msg);
            }

            return result;
        }

        public async Task<ResultSimple<DownloadResult>> ManualUpdate(string request)
        {
            var result = new ResultSimple<DownloadResult>();
            try
            {
                result = await DownloadFile(request);

                if (!result.Sucess)
                    return result;

                await _tasksServices.UpdateTedReceipt(result.Data.Items);

            }
            catch (Exception ex)
            {

                var msg = $"Falha update manual conforme o ticket {request}";
                _logger.LogError(ex, msg);
                result.Messages.Add(msg);

            }

            return result;
        }



        /// <summary>
        /// Recebe o ticket de solicitação de arquivo
        /// Verifica se o serviço já foi concluido
        /// Baixa o arquivo e converte para []byte para tratamento pelo programa que o chamou
        /// referencia para download do arquivo https://lluisfranco.com/2017/11/29/how-to-connect-via-sftp-using-ssh-net/
        /// </summary>
        /// <param name="request">Ticket de solicitação do arquivo em formato guid</param>
        /// <returns>[]byte</returns>
        public async Task<ResultSimple<DownloadResult>> DownloadFile(string request)
        {
            var result = new ResultSimple<DownloadResult>();
            result.Data = new DownloadResult();

            string pathzipfile = string.Empty;
            string pathunzipfile = string.Empty;
            try
            {
                var ticket = await CheckRequest(request);


                if (!ticket.Sucess)
                {
                    result.Messages.AddRange(ticket.Messages);
                    return result;
                }

                result.Data.Ticket = ticket.Data;
                if (ticket.Data != null && !ticket.Data.status.Contains("finished"))
                {
                    result.Messages.Add("Processo não concluido");
                    return result;
                }

                var key = File.ReadAllText(_appSettings.SSHKey);
                var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
                var privateKeyFile = new PrivateKeyFile(buf, "onlypay");
                pathzipfile = $"{_appSettings.DownloadPath}/{ticket.Data.file}";
                using (SftpClient sftpclient = new SftpClient(_appSettings.SFTPHost, _appSettings.SFTPUser, privateKeyFile))
                {
                    sftpclient.Connect();
                    using (var targetFile = new FileStream(pathzipfile, FileMode.Create))
                    {
                        //Os arquivos estão organizados por pasta do download, caso isto mude os metodos
                        // abaixo podem ajudar a encontrar o arquivo
                        //var files = sftpclient.ListDirectory(sftpclient.WorkingDirectory);
                        //var folderexist = sftpclient.Exists(ticket.Data.finishDate.ToString("yyyy-MM-dd"));
                        var remotefilepath = $"{ticket.Data.finishDate.ToString("yyyy-MM-dd")}/{ticket.Data.file}";
                        var fileexist = sftpclient.Exists(remotefilepath);
                        if (!fileexist)
                        {
                            result.Messages.Add($"arquivo do ticket {request}, não encontrado.");
                            return result;
                        }
                        sftpclient.DownloadFile(remotefilepath, targetFile);
                        targetFile.Close();
                    }
                    _logger.LogInformation($"Download do arquivo concluido. Caminho: {pathzipfile}");
                }

                var unzipfilename = ticket.Data.file.Split('.');
                pathunzipfile = ExtractZip(pathzipfile, $"{unzipfilename[0]}.{unzipfilename[1]}");
                var items = Readcsv(pathunzipfile);


                result.Data.Ticket = ticket.Data;
                result.Data.Items = items;
                result.Sucess = true;
            }
            catch (Exception ex)
            {
                var msg = $"Falha ao solicitar o arquivo conforme o ticket {request}";
                _logger.LogError(ex, msg);
                result.Messages.Add(msg);
            }
            finally
            {
                if (!string.IsNullOrEmpty(pathzipfile))
                    File.Delete(pathzipfile);

                if (!string.IsNullOrEmpty(pathunzipfile))
                    File.Delete(pathunzipfile);
            }


            return result;
        }

        private string ExtractZip(string zipfilepath, string filename)
        {
            string path = $"{_appSettings.DownloadPath}/{filename}";

            using (var zip = ZipFile.OpenRead(zipfilepath))
            {
                zip.Entries[0].ExtractToFile(path);
            }

            return path;
        }

        /// <summary>
        /// https://joshclose.github.io/CsvHelper/getting-started/
        /// </summary>
        /// <typeparam name="Tresult"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<TedResultItem> Readcsv(string path)
        {
            var result = new List<TedResultItem>();
            using (var reader = new StreamReader(path))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var IenumerableResult = csv.GetRecords<TedResultItem>();
                    foreach (var item in IenumerableResult)
                    {
                        var description = item.description.ToUpper();
                        if (description.Contains("CASHBACK"))
                            result.Add(item);

                        //result.Add(item);
                    }

                }
            }

            return result;

        }


        private string QueryStringCompose(Filter filter)
        {
            var service = Enum.GetName(typeof(ServiceEnum), filter.Servico);
            var result = $"compress=zip&service={service}";


            if (filter.Date != null)
            {
                result = $"{result}&date={filter.Date.GetValueOrDefault().ToString("yyyy-MM-dd")}";
            }
            else
            {
                var start = filter.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                var end = filter.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                result = $@"{result}&from={start}&to={end}";
            }
            return result;
        }


    }
}

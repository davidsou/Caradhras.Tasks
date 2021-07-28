using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class AppSettings
    {
        public string NomeServico { get; set; }
        public string Porta { get; set; }
        public string UrlServico { get; set; }
        public string UrlSafeBox { get; set; }
        public string SafeBoxUser { get; set; }
        public string SafeBoxInfo { get; set; }
        public string SSHKey { get; set; }
        public string SFTPUser { get; set; }
        public string SFTPHost { get; set; }
        public string DownloadPath { get; set; }

        public string TokenAuth { get; set; }
        public string DataApi { get; set; }

        public string CronScheduleRequestFile { get; set; }
        public string CronScheduleDownloadFile { get; set; }
    }
}

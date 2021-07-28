using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class TedResultItem
    {
        public int id { get; set; }
        public int idAdjustment { get; set; }
        public string transactionCode { get; set; }
        public int idIssuer { get; set; }
        public string description { get; set; }
        public int idOriginAccount { get; set; }
        public float value { get; set; }
        public string typeAccountFavored { get; set; }
        public string nameFavored { get; set; }
        public int bankFavored { get; set; }
        public int agencyFavored { get; set; }
        public string digitAgencyFavored { get; set; }
        public long accountFavored { get; set; }
        public string digitAccountFavored { get; set; }
        public string cnabFileName { get; set; }
        public string statusTransfer { get; set; }
        public string tariffCode { get; set; }
        public DateTime? transferenceDate { get; set; }
        public bool? transferSuccess { get; set; }
        public string codStatusTransfer { get; set; }
        public DateTime? processDate { get; set; }
        public string uid { get; set; }
        public string cpfCnpjFavored { get; set; }

    }    {
    }
}

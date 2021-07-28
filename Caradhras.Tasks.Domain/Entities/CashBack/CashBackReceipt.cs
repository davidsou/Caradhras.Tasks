using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class CashBackReceipt
    {
        public int Id { get; set; }
        public int CashBackItemId { get; set; }
        public int OrderRequestId { get; set; }
        public string Document { get; set; }
        public string Type { get; set; }
        public string ProcessingStatus { get; set; }
        public string typeAccountFavored { get; set; }
        public int idOriginAccount { get; set; }
        public string subIssuerCode { get; set; }
        public string description { get; set; }
        public int identificator { get; set; }
        public float value { get; set; }
        public int idIssuer { get; set; }
        public string tariffCode { get; set; }
        public string UID { get; set; }
        public DateTime date { get; set; }
        public string transactionCode { get; set; }
        public int idAdjustment { get; set; }
        public string cpfCnpjFavored { get; set; }
        public string type { get; set; }
        public string docIdCpfCnpjEinSSN { get; set; }
        public string name { get; set; }
        public int bankId { get; set; }
        public int agency { get; set; }
        public string agencyDigit { get; set; }
        public long account { get; set; }
        public string accountDigit { get; set; }
        public string accountType { get; set; }
        public string cnabFileName { get; set; }
        public string statusTransfer { get; set; }
        public DateTime transferenceDate { get; set; }
        public bool transferSuccess { get; set; }
        public string codStatusTransfer { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedAt { get; set; }
        public bool Success { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{

    public class CashBackItem
    {
        public int Id { get; set; }
        public int OrderRequestId { get; set; }
        public string Documento { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string DigitoConta { get; set; }
        public string TipoConta { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string StatusProcessamento { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedAt { get; set; }

    }
}

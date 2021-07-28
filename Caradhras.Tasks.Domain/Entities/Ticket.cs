using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{


    public class Ticket
    {
        public string ticket { get; set; }
        public string service { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public DateTime createdDate { get; set; }
        public string status { get; set; }
        public DateTime finishDate { get; set; }
        public string file { get; set; }
        public string field { get; set; }
        public string message { get; set; }
    }

}

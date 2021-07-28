using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class ScheduledControl
    {
        public int Id { get; set; }
        public int OrderRequestId { get; set; }
        public string Ticket { get; set; }
        public string Status { get; set; }
        public DateTime StartExecution { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedAt { get; set; }
        public List<ScheduledControlItem> Items { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class ScheduledControlItem
    {
        public int Id { get; set; }
        public int TaskControlId { get; set; }
        public int OrderRequestId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}

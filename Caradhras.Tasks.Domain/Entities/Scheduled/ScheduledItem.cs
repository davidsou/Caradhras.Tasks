using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class ScheduledItem
    {
        public int OrderRequestId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

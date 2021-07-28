using System;
using System.Collections.Generic;
using System.Text;

namespace Caradhras.Tasks.Domain.Entities
{
    public class DownloadResult
    {
        public Ticket Ticket { get; set; }
        public List<TedResultItem> Items { get; set; }
    }
}

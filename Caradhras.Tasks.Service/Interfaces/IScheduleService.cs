using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Service.Interfaces
{
    public interface IScheduledService
    {
        Task CheckReceiptsRequest();
        Task GetReceipts();
    }
}

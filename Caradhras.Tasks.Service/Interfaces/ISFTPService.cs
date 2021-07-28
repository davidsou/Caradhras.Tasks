using Caradhras.Tasks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Service.Interfaces
{
    public interface ISFTPService
    {
        Task<ResultSimple<Ticket>> RequestFile(Filter filter);
        Task<ResultSimple<Ticket>> CheckRequest(string request);
        Task<ResultSimple<DownloadResult>> DownloadFile(string request);
        Task<ResultSimple<DownloadResult>> ManualUpdate(string request);
    }
}

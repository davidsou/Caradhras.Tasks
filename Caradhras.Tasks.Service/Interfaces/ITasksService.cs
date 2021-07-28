using Caradhras.Tasks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Service.Interfaces
{
    public interface ITasksServices
    {
        Task InsertTaskControlItems(List<ScheduledControlItem> items);
        List<TedResultItem> FilterAndSetTedResultItem(DownloadResult file);
        Task UpdateTedReceipt(List<TedResultItem> tedsresult);
        List<ScheduledControlItem> SetItems(List<int> items, int id);
        Task UpdateControl(int id, string status, bool active);
        Task<int> InsertControl(ScheduledControl scheduled);
    }
}

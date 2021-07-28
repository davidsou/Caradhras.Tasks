using Caradhras.Tasks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Repository
{
    public interface ITaskRepository
    {

        Task<List<ScheduledControl>> GetAvailableTasks();
        Task<bool> DeleteTaskControlt(int id, string status);
        Task<bool> UpdateStepTaskControl(int id, string status, bool active);
        Task<bool> UpdateTicketTaskControl(int id, string status, string ticket);
        Task<int> InsertTaskControl(ScheduledControl taskitem);
        Task<List<ScheduledItem>> CashbackExist();
        Task<int> InsertTaskControlItem(ScheduledControlItem taskitem);
    }
}

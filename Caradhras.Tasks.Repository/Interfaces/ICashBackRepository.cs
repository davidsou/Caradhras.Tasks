using Caradhras.Tasks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Repository
{
    public interface ICashBackRepository
    {
        Task<CashBackReceipt> GetByIdAdjusment(int id);
        Task<bool> UpdateCashBackReceipt(CashBackReceipt receipt);
    }
}

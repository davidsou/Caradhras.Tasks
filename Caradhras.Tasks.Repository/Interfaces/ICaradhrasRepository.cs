using Caradhras.Tasks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Repository
{
    public interface ICaradhrasRepository
    {
        AutenticacaoCaradhras AutenticacaoCaradhras();
        Task<ResultSimple<Tresult>> ConsumeServicesGET<Tresult>(string service, string token);
        Task<ResultSimple<Tresult>> ConsumeServicesGETBureau<Tresult>(string service);
        Task<ResultSimple<Tresult>> ConsumeServicesPOST<Tresult, Trequest>(Trequest payload, string service, string token);
        Task<ResultSimple<Tresult>> ConsumeServicesPOST<Tresult>(string service, string token);
        Task<ResultSimple<Tresult>> ConsumeServicesPUT<Tresult, Trequest>(Trequest payload, string service, string token);
        Task<bool> ConsumeServicesPOST(string service, string token);
    }
}

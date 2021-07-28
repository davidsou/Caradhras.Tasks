using Caradhras.Tasks.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Repository
{
    public class CashBackRepository : ICashBackRepository
    {

        IConfiguration _config;
        ILogger _logger;
        AppSettings _appSettings;
        private readonly string _connection;

        public CashBackRepository(IConfiguration config, ILogger<TaskRepository> logger, IOptions<AppSettings> appSettings)
        {
            _config = config;
            _logger = logger;
            _appSettings = appSettings.Value;
            _connection = _config.GetConnectionString("BackOffice");
        }

        public async Task<List<CashBackReceipt>> GetByOrderId(List<int> orders)
        {
            var sql = @"select *  from CashBackItem where OrderRequestId in @orders";
            using (var con = new SqlConnection(_connection))
            {
                var result = await con.QueryAsync<CashBackReceipt>(sql, orders);

                return result.ToList();
            }
        }

        public async Task<CashBackReceipt> GetByIdAdjusment(int id)
        {
            var sql = @"select *  from CashBackReceipt where idAdjustment  = @param";
            using (var con = new SqlConnection(_connection))
            {
                var result = await con.QueryFirstAsync<CashBackReceipt>(sql, new { param = id });

                return result;
            }
        }

        public async Task<bool> UpdateCashBackReceipt(CashBackReceipt receipt)
        {

            /*
             lição de casa: 

            trocar o update individual por update em lote
            exemplo:

            select * into #tempcashbacknew  from CashBackReceipt where idadjustment in (<sua lista aqui>) 

            update A
            set codStatusTransfer = T.codStatusTransfer
            FROM CashBackReceipt A
            INNER JOIN #tempcashbacknew T on A.Id = T.Id

             */




            var updateReceipt = $@"UPDATE CashBackReceipt
                                   SET
                                      Success = @Success
                                      ,statusTransfer = @statusTransfer
                                      ,codStatusTransfer = @codStatusTransfer
                                      ,transferenceDate = @transferenceDate
                                      ,transferSuccess = @transferSuccess    
                                      ,ChangedBy = @ChangedBy
                                      ,ChangedAt = @ChangedAt
                                 WHERE Id = @Id";



            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.ExecuteAsync(updateReceipt, receipt);
                return result > 1;

            }


        }




    }

}

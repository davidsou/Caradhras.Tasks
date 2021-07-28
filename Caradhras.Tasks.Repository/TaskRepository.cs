using Caradhras.Tasks.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caradhras.Tasks.Repository
{

    public class TaskRepository : ITaskRepository
    {
        IConfiguration _config;
        ILogger _logger;
        AppSettings _appSettings;
        private readonly string _connection;

        public TaskRepository(IConfiguration config, ILogger<TaskRepository> logger, IOptions<AppSettings> appSettings)
        {
            _config = config;
            _logger = logger;
            _appSettings = appSettings.Value;
            _connection = _config.GetConnectionString("BackOffice");
        }

        public async Task<List<ScheduledItem>> CashbackExist()
        {
            var result = new List<ScheduledItem>();

            var query = $@"select distinct  o.Id as OrderRequestId, cbi.CreatedAt
                        from orderrequest o
                        inner join  cashbackitem cbi 
                        on o.id = cbi.orderrequestid
						inner join cashbackreceipt cbr
						on cbi.id = cbr.cashbackitemid
						where  cbr.ChangedAt is  null";

            using (var conn = new SqlConnection(_connection))
            {
                var resultList = await conn.QueryAsync<ScheduledItem>(query);
                result = resultList.ToList();
            }

            return result;
        }

        public async Task<int> InsertTaskControl(ScheduledControl taskitem)
        {
            var query = $@"Insert Into TaskControl                         
		                   (Ticket
                            ,Status
                           ,StartExecution
                           ,Active                           
                           ,CreatedBy
                           ,CreatedAt)
                            VALUES
                            (@Ticket
                             ,@Status
                             ,@StartExecution
                             ,@Active                             
                             ,@CreatedBy
                             ,@CreatedAt) Select cast(scope_identity() as int)";
            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.QuerySingleAsync<int>(query, taskitem);
                return result;

            }
        }

        public async Task<int> InsertTaskControlItem(ScheduledControlItem taskitem)
        {
            var query = $@"Insert Into TaskControlitem                         
		                   (TaskControlId
                            ,OrderRequestId
                           ,CreatedBy
                           ,CreatedAt)
                            VALUES
                            (
                             @TaskControlId
                             ,@OrderRequestId                            
                             ,@CreatedBy
                             ,@CreatedAt) Select cast(scope_identity() as int)";
            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.QuerySingleAsync<int>(query, taskitem);
                return result;

            }
        }


        public async Task<bool> UpdateTicketTaskControl(int id, string status, string ticket)
        {
            var query = $@"Update TaskControl set Status=@status , Ticket = @ticket, ChangedBy=@changedby, ChangedAt=@changedat where id = @id";

            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.ExecuteAsync(query, new { id, status, ticket, changedby = "Sistema", changedat = DateTime.Now });

                return result > 1;
            }
        }

        public async Task<bool> UpdateStepTaskControl(int id, string status, bool active)
        {
            var query = $@"Update TaskControl set Status=@status , Active = @active, ChangedBy=@changedby, ChangedAt=@changedat  where id = @id";

            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.ExecuteAsync(query, new { id, status, active, changedby = "Sistema", changedat = DateTime.Now });

                return result > 1;
            }
        }




        public async Task<bool> DeleteTaskControlt(int id, string status)
        {
            var query = $@"Update TaskControl set Status=@status , Active=@active, ChangedBy=@changedby, ChangedAt=@changedat  where id = @id";

            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.ExecuteAsync(query, new { id, status, active = false, changedby = "Sistema", changedat = DateTime.Now });

                return result > 1;
            }
        }



        public async Task<List<ScheduledControl>> GetAvailableTasks()
        {
            var query = $@" Select *  from TaskControl where Active = 1 and StartExecution < @param ";

            using (var conn = new SqlConnection(_connection))
            {
                var result = await conn.QueryAsync<ScheduledControl>(query, new { param = DateTime.Now });

                return result.ToList();
            }
        }



    }
}

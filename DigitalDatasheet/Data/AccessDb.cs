using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;

namespace DigitalDatasheet.Data
{
    class AccessDb
    {
        public async Task<List<string>> GetCustomers()
        {
            var customers = new List<string>();
            await using (OdbcConnection conn = new OdbcConnection())
            {
                conn.ConnectionString = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)}; DBQ=\\ptlsrvr4\PTLOffice\PTL Database\ptl_db.accdb;";
                await conn.OpenAsync();
                string query = "select [FolderName] from [customers] order by [FolderName]";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                OdbcDataReader reader = await cmd.ExecuteReaderAsync() as OdbcDataReader;
                while (await reader.ReadAsync())
                {
                    string customer = reader[0].ToString();
                    customers.Add(customer);
                }
                conn.Close();
            }
            return customers;
        }
        public async Task<(DateTime?, bool)> GetDueDateInfo(string fullWorkOrderNumber)
        {
            string workOrderNumber;
            string workOrderNumberDash = string.Empty;
            if (fullWorkOrderNumber.Contains("-"))
            {
                string[] workOrderSplit = fullWorkOrderNumber.Split('-');
                workOrderNumber = workOrderSplit[0].Trim();
                workOrderNumberDash = workOrderSplit[1].Trim();
            }
            else
                workOrderNumber = fullWorkOrderNumber;
            DateTime? dueDate = null;
            bool expedite = false;
            await using (OdbcConnection conn = new OdbcConnection())
            {
                conn.ConnectionString = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)}; DBQ=\\ptlsrvr4\PTLOffice\PTL Database\ptl_db.accdb;";
                await conn.OpenAsync();
                string query = $"select [Date Due], Expedite from [Work Orders] where [Work Order No] = ? and [WO Dash] = ?";
                var cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add(new OdbcParameter("work order no", OdbcType.Int)).Value = workOrderNumber;
                cmd.Parameters.Add(new OdbcParameter("wo dash", OdbcType.Text)).Value = workOrderNumberDash;
                cmd.Prepare();
                var reader = await cmd.ExecuteReaderAsync() as OdbcDataReader;
                if (await reader.ReadAsync())
                {
                    dueDate = reader.GetDate(0);
                    expedite = reader.GetBoolean(1);
                }
                else
                {

                }
            }
            return (dueDate, expedite);
        }
        public async Task<string> GetJobYear(string fullWorkOrderNumber)
        {
            string workOrderNumber;
            string workOrderNumberDash = string.Empty;
            if (fullWorkOrderNumber.Contains("-"))
            {
                string[] workOrderSplit = fullWorkOrderNumber.Split('-');
                workOrderNumber = workOrderSplit[0];
                workOrderNumberDash = workOrderSplit[1];
            }
            else
                workOrderNumber = fullWorkOrderNumber;
            string dateReceivedYear = string.Empty;
            using (var conn = new OdbcConnection())
            {
                conn.ConnectionString = @"Driver={Microsoft Access Driver (*.mdb, *.accdb)}; DBQ=\\ptlsrvr4\PTLOffice\PTL Database\ptl_db.accdb;";
                await conn.OpenAsync();
                string query = $"select [Date Received] from [Work Orders] where [Work Order No] = ? and [WO Dash] = ?";
                var cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add(new OdbcParameter("work order no", OdbcType.Int)).Value = workOrderNumber;
                cmd.Parameters.Add(new OdbcParameter("wo dash", OdbcType.Text)).Value = workOrderNumberDash;
                cmd.Prepare();
                var reader = await cmd.ExecuteReaderAsync() as OdbcDataReader;
                dateReceivedYear = await reader.ReadAsync() ? ((DateTime)reader[0]).Year.ToString() : string.Empty;
                //dateReceivedYear = string.IsNullOrEmpty(reader[0].ToString()) ? string.Empty : ((DateTime)reader[0]).Year.ToString();
                conn.Close();
            }
            return dateReceivedYear;
        }
    }
}

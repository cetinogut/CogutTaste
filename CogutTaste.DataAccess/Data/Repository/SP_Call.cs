using CogutTaste.DataAccess.Data.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository
{
    class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _db;
        private static string ConnectionString = "";

        public SP_Call(ApplicationDbContext db)
        {
            _db = db;
            ConnectionString = db.Database.GetDbConnection().ConnectionString; // retrieves connectionstring and  stores in local..
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public T ExecureReturnScaler<T>(string procedureName, DynamicParameters param = null) // bu bir entity dönen sp
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open(); // open connection
                return (T)Convert.ChangeType(sqlCon.ExecuteScalar<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure), typeof(T)); // sp call
            }
        }

        public void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null)// bu birşey dönmeeyen sp..
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null) // bu liste dönen sp.dir
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}

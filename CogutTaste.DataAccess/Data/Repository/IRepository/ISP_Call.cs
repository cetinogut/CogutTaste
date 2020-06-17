using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CogutTaste.DataAccess.Data.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        // üç tip SP çağrısı için temelleri oluşturuyoruz..
        IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null); // returns a list, like GetAllCategory

        void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null); // if update anything , or do smth that does not return anything
        T ExecureReturnScaler<T>(string procedureName, DynamicParameters param = null); // return smth.
    }
}

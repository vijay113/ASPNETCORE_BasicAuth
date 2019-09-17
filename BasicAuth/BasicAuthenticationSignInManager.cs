using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BasicAuthSample.BasicAuth
{
    public class BasicAuthenticationSignInManager
    {
        public BasicAuthenticationSignInManager(HttpContext context, BasicAuthenticationHeaderValue authenticationHeaderValue)
        {
            _context = context;
            _authenticationHeaderValue = authenticationHeaderValue;           
          }

        private readonly HttpContext _context;
        private readonly BasicAuthenticationHeaderValue _authenticationHeaderValue;
             
        

        public async Task<bool> TrySignInUser()
        {
            bool isValid = false;
            if (_authenticationHeaderValue.IsValidBasicAuthenticationHeaderValue)
            {
                isValid= await GetUserByUsernameOrEmail();               
            }
            return isValid;
        }


        private async Task<bool> GetUserByUsernameOrEmail()
        {
            bool isValidUser = false;
            var Query = "SELECT * FROM [SignalRCore_Demo].[dbo].[Products] WHERE [Name]= '"+ _authenticationHeaderValue.UserIdentifier + "' AND Password = '" + _authenticationHeaderValue.UserPassword + "'";
            DataTable dt = new DataTable();
            var connectionstr = Startup.ConnectionStringAdmin;             
            using (var sqlConnection = new SqlConnection(connectionstr))
            using (var sqlCommand = new SqlCommand(Query, sqlConnection))
            {
                // sqlCommand.CommandTimeout = 0;             
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                //adp to fetch data
                SqlDataAdapter adp = new SqlDataAdapter(sqlCommand);
                DataSet ds = new DataSet();               

                //select command fire here
                adp.Fill(ds);

                dt = ds.Tables[0];
            }

            int totalCount = dt.Rows.Count;
            if (totalCount >0 ) {
                isValidUser = true;
            }
            return isValidUser;
        }


    }
}

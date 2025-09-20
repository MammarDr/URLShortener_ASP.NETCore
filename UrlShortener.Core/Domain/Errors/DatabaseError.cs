using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Services;


namespace UrlShortener.Core.Domain.Errors
{

    public partial record Error
    {
        public class DatabaseError
        {
            
            public static Error Conflict(string description = "Conflict with the data") =>
                    new("Database.Conflict", description, enErrorType.Conflict);

            public static readonly Error RollBack = new("Database.RollBack", "Transaction rollback failed due to an unexpected database error.", enErrorType.Unexpected);

            public static readonly Error Unexpected = new("Database.Unexpected", "A database error occurred.", enErrorType.Unexpected);


        }
    }
}

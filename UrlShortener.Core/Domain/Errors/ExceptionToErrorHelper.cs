using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Domain.Errors;
using UrlShortener.Core.Domain.Results;

namespace UrlShortener.Core.Validators
{
    public static class ExceptionToErrorHelper
    {

        public static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2627 || sqlEx.Number == 2601; // SQL Server unique constraint
            return false;
        }

        public static Error ToDbError(this Exception ex)
        {
            return ex is DbUpdateException ?
                            (IsUniqueConstraintViolation((DbUpdateException)ex) ?
                                Error.DatabaseError.Conflict() :
                                Error.DatabaseError.Unexpected) :
                             Error.DatabaseError.Unexpected;
        }
    }
}

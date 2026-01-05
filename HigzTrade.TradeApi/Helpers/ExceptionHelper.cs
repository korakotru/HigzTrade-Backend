using HigzTrade.Application.Exceptions;
using HigzTrade.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HigzTrade.TradeApi.Helpers
{
    public static class ExceptionHelper
    {
        public static (string Title, int Status) GetErrorInfo(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => ("Unauthorized", 401),
                ArgumentException => ("Invalid argument", 400), //Third-party serivces/library อาจ throw ArgumentException
                DomainException => (ex.Message , 400),
                BusinessException => (ex.Message, 400),
                KeyNotFoundException => ("Resource not found", 404),
                DbUpdateException { InnerException: SqlException { Number: 2627 or 2601 } }  => ("Data already exists", 409),
                DbUpdateConcurrencyException => ("Data was modified by another user. Please reload.", 409),
                SqlException or { InnerException: SqlException } => ("Database connection error", 503), // เคสที่ต่อ database server ไม่ได้
                DbUpdateException => ("Database transaction failed", 500),
                TimeoutException => ("Request timeout", 504),
                _ => ("Internal server error", 500) // unexpect error exception
            };
        }
    }

}

using HigzTrade.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HigzTrade.TradeApi.Helpers
{
    public static class ExceptionHelper
    {
        public static (string Title, int Status) GetErrorInfo(Exception ex)
        {
            return ex switch
            {
                DomainException => ("Domain rule violated", 400),
                ApplicationException => ("Application error", 400),
                DbUpdateException => ("Database update failed", 409),
                _ => ("Internal server error", 500)
            };
        }
    }

}

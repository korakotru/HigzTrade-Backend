using HigzTrade.TradeApi.HttpModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public sealed class ApiSuccessResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult obj &&
            obj.StatusCode is >= 200 and < 300)
        {
            context.Result = new ObjectResult(new ApiSuccessResponse{
                Title = "Success",
                Status = obj.StatusCode,
                Data = obj.Value,
                TraceId = context.HttpContext.TraceIdentifier })
            {
                StatusCode = obj.StatusCode
            };


            //// Content
            //var responseContent = new ApiSuccessResponse
            //{
            //    Title = "Success",
            //    Status = obj.StatusCode,
            //    Data = obj.Value,
            //    TraceId = context.HttpContext.TraceIdentifier
            //};

            //// Wrapper
            //context.Result = new ObjectResult(responseContent)
            //{
            //    StatusCode = obj.StatusCode // หน้าซองจดหมาย
            //};
        }

        await next();
    }
}
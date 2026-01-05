namespace HigzTrade.TradeApi.HttpModels
{
    public sealed class ApiSuccessResponse
    {
        public string Title { get; init; } = "Success";
        public int? Status { get; init; }
        public object? Data { get; init; }
        public string TraceId { get; init; } = default!;
    }
} 
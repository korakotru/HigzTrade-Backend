namespace HigzTrade.TradeApi.HttpResponseModels
{
    public sealed class ApiErrorResponse
    {
        public string Title { get; init; } = default!;
        public int Status { get; init; }
        public IEnumerable<string> Errors { get; init; } = Enumerable.Empty<string>();
        public string TraceId { get; init; } = default!;
    }

}

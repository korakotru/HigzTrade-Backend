using System.Reflection;

namespace HigzTrade.TradeApi.Helpers
{
    public static class AppVersionHelpers
    {
        public static string GetBuildVersion()
        {
            return $"{DateTime.Now.ToString("dd.MM.yyyy")} {Assembly.GetExecutingAssembly()
                         .GetName()
                         .Version?
                         .ToString() ?? "1.0.0.0"}" ;
        }
    }
}

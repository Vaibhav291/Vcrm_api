namespace Vcrm
{
    public class AppSettings
    {
        public const string Development = "Development";
        public const string Qa = "Qa";
        public const string Staging = "Staging";
        public const string Production = "Production";

        public static string EnvironmentName { get; } =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Production;

        public static bool IsDevelopment => EnvironmentName == Development;
        public static bool IsQa => EnvironmentName == Qa;
        public static bool IsStaging => EnvironmentName == Staging;
        public static bool IsProduction => EnvironmentName == Production;
    }
}

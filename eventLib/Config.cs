using Microsoft.Extensions.Configuration;

namespace eventLib
{
    public static class Config
    {
        private static IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        public static string SecureKey => configuration["JWT:SecureKey"]
            ?? "VVGEventManager-Development-Secret-Key-Min32Chars!";
    }
}

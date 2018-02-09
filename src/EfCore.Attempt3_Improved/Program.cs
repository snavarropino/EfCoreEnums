using System;
using Serilog;

namespace EfCore.Attempt3_Improved
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogs();

            Sample3.Run();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void ConfigureLogs()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}

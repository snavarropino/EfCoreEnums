using System;
using Serilog;

namespace Attempt1
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogs();
            
            Sample.Run();

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

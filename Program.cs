using WorkerService.Services;
using WorkerService.Utils;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.Configure<StatementSettings>(context.Configuration.GetSection("StatementSettings"));
                services.AddSingleton<StatementService>();
                services.AddHostedService<Worker>();
            })
            .Build();

            host.Run();
        }
    }
}
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using HttpLoadTester.SignalR;

namespace HttpLoadTester
{
    public class Program
    {
        private static Task[] _tasks;
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseUrls("http://*:5000")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var s = host.Services.GetService(typeof(ServiceRunner)) as ServiceRunner;
            _tasks = new [] {
                    Task.Run(() => s.DoQuickReports())
                    ,Task.Run(() => s.DoLongRunningReports())
                    };

            
            host.Run();
        }
    }
}

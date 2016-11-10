using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using HttpLoadTester.SignalR;

namespace HttpLoadTester
{
    public class Program
    {
        private static Task _task;

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var s = host.Services.GetService(typeof(ServiceRunner)) as ServiceRunner;
            _task = Task.Run(() => s.DoWork());

            
            host.Run();
        }
    }
}

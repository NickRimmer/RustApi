using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oxide.Ext.RustApi.Interfaces;
using Oxide.Ext.RustApi.Models;
using Oxide.Ext.RustApi.Services;

namespace Oxide.Ext.RustApi.Tests.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            /*
            var container = new MicroContainer()
                .Add(typeof(ILogger<>), typeof(ConsoleLogger<>))
                .AddSingle(new ApiServerOptions
                {
                    Endpoint = "http://localhost:6667",
                    Secret = "secret",
                })
                .AddSingle<ApiServer>();

            container.Get<ApiServer>()
                .AddRoute<List<string>>("/test/1", OnTest1)
                .AddRoute("test/2/", () => Console.WriteLine("test 2"))
                .AddRoute<HookRequestModel>("hook", onCallHook)
                .Start();

            while (true) await Task.Delay(TimeSpan.FromSeconds(1));
            */
        }

        private static List<int> OnTest1(List<string> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            return data.Select(x => int.Parse(x)).ToList();
        }

        private static void onCallHook(ApiHookRequest apiHookInfo)
        {

        }
    }
}

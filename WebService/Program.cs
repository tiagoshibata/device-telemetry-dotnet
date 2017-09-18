// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.Runtime;
using Version = Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Version;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService
{
    /// <summary>Application entry point</summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new Config(new ConfigData());

            /*
            Print some information to help development and debugging, like
            runtime and configuration settings
            */
            Console.WriteLine($"[{Uptime.ProcessId}] Starting web service, process ID: " + Uptime.ProcessId);
            Console.WriteLine($"[{Uptime.ProcessId}] Web service listening on port " + config.Port);
            Console.WriteLine($"[{Uptime.ProcessId}] Web service health check at: http://127.0.0.1:" + config.Port + "/" + Version.PATH + "/status");
            Console.WriteLine($"[{Uptime.ProcessId}] Key Value Storage at " + config.ServicesConfig.StorageAdapterApiUrl);

            /*
            Kestrel is a cross-platform HTTP server based on libuv, a
            cross-platform asynchronous I/O library.
            https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers
            */
            var host = new WebHostBuilder()
                .UseUrls("http://*:" + config.Port)
                .UseKestrel(options => { options.AddServerHeader = false; })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime.Configuration;
using Shared;

class Program
{
    static void Main(string[] args)
    {
        // var config = ClientConfiguration.LocalhostSilo(30000);
        // config.TraceFilePattern = "Client.log";
        // GrainClient.Initialize(config);
        // DateTime lastTime = DateTime.Now;
        // var counter = 0;
        var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    // options.ThreadCount = 4;
                    options.NoDelay = true;
                    options.UseConnectionLogging();
                })
                .UseUrls("http://localhost:5000","http://+:5002")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

        host.Run();

    }
}

public class Startup
{
    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole(LogLevel.Trace);
        var logger = loggerFactory.CreateLogger("Default");

        app.Run(async context =>
        {
            var connectionFeature = context.Connection;
            logger.LogDebug($"Peer: {connectionFeature.RemoteIpAddress?.ToString()}:{connectionFeature.RemotePort}"
                + $"{Environment.NewLine}"
                + $"Sock: {connectionFeature.LocalIpAddress?.ToString()}:{connectionFeature.LocalPort}");

            var response = $"Hello, World{Environment.NewLine}";
            context.Response.ContentLength = response.Length;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(response);
        });
    }
}
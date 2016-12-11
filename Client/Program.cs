using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime.Configuration;
using Shared;

class Program
{
    static void Main(string[] args)
    {
        var config = ClientConfiguration.LocalhostSilo(30000);
        config.TraceFilePattern = "Client.log";
        GrainClient.Initialize(config);
        DateTime lastTime = DateTime.Now;
        var counter = 0;
    }
}

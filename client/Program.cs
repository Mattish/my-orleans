using System;
using System.IO;
using Orleans;
using Orleans.Runtime.Configuration;
using shared;

class Program
{
    static void Main(string[] args)
    {
        var config = ClientConfiguration.LocalhostSilo(30000);
        config.TraceFilePattern = "Client.log";
        GrainClient.Initialize(config);
        DateTime lastTime = DateTime.Now;
        while (true)
        {
            var myGrain = GrainClient.GrainFactory.GetGrain<IMyGrainStateful>(0);
            myGrain.AddGreeting("Good morning, my friend!").Wait();
            Console.WriteLine($"GreetingsCount:{myGrain.GetGreetingsCount().Result}");
            Console.ReadKey();
        }
    }
}

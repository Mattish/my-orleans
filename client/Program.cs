using System;
using Orleans;
using Orleans.Runtime.Configuration;
using shared;

class Program
{
    static void Main(string[] args)
    {
        var config = ClientConfiguration.LocalhostSilo();
        GrainClient.Initialize(config);
        DateTime lastTime = DateTime.Now;
        int counter = 0;
        while (!Console.KeyAvailable)
        {
            var myGrain = GrainClient.GrainFactory.GetGrain<IMyGrain>(0);
            var response = myGrain.SayHello("Good morning, my friend!").Result;
            counter++;
            if (lastTime + TimeSpan.FromSeconds(1) < DateTime.Now)
            {
                Console.WriteLine($"Counter:{counter}");
                counter = 0;
                lastTime = DateTime.Now;
            }
        }
        Console.ReadKey();
    }
}

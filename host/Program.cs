using System;
using Orleans.Runtime.Configuration;

class Program
{
    static void Main(string[] args)
    {
        var config = ClusterConfiguration.LocalhostPrimarySilo();
        config.AddMemoryStorageProvider();
        var siloHost = new Orleans.Runtime.Host.SiloHost("MyHost", config);
        siloHost.LoadOrleansConfig();
        siloHost.InitializeOrleansSilo();
        siloHost.StartOrleansSilo();
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
        siloHost.StopOrleansSilo();
        siloHost.Dispose();
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

namespace shared
{
    public interface IMyGrain : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }

    public class MyGrain : Orleans.Grain, IMyGrain
    {
        Task<string> IMyGrain.SayHello(string greeting)
        {
            return Task.FromResult($"This was the greeting you said: {greeting}");
        }
    }


    public class GrainState{
        public List<string> Greetings {get;set;}
    }
    public interface IMyGrainStateful : Orleans.IGrainWithIntegerKey{
        Task<int> GetGreetingsCount();
        Task AddGreeting(string greeting);
    }


    [StorageProvider(ProviderName = "MemoryStore")]
    public class MyGrainStateful : Orleans.Grain<GrainState>, IMyGrainStateful{
        public Task AddGreeting(string greeting){
            if(State.Greetings == null){
                State.Greetings = new List<string>();
            }
            State.Greetings.Add(greeting);
            return base.WriteStateAsync();
        }

        Task<int> IMyGrainStateful.GetGreetingsCount()
        {
            return Task.FromResult(State.Greetings?.Count ?? 0);
        }
    }
}
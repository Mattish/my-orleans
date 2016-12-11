using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Providers;

namespace Shared{
    public class GrainState
    {
        public List<string> Greetings { get; set; }
    }
    public interface IMyGrainStateful : Orleans.IGrainWithIntegerKey
    {
        Task<int> GetGreetingsCount();
        Task AddGreeting(string greeting);
    }


    [StorageProvider(ProviderName = "MyStorageProvider")]
    public class MyGrainStateful : Orleans.Grain<GrainState>, IMyGrainStateful
    {
        public Task AddGreeting(string greeting)
        {
            if (State.Greetings == null)
            {
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
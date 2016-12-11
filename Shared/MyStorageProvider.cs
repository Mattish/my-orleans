using Orleans.Providers;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shared
{
    public class MyStorageProvider : Orleans.Storage.IStorageProvider
    {
        private string _state = "{}";

        public Logger Log
        {
            get; protected set;
        }
        public string Name { get; protected set; }

        protected MyStorageProvider()
        {
        }
        public virtual Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;
            Log = providerRuntime.GetLogger(this.GetType().FullName);
            return TaskDone.Done;
        }
        public Task Close()
        {
            return TaskDone.Done;
        }
        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            JsonConvert.PopulateObject(_state, grainState);
            await TaskDone.Done;
        }
        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            _state = JsonConvert.SerializeObject(grainState);
            return TaskDone.Done;
        }
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            _state = "{}";
            return TaskDone.Done;
        }
    }
}
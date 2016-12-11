using Orleans.Providers;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Shared
{
    public class RedisStorageProvider : Orleans.Storage.IStorageProvider
    {
        ConnectionMultiplexer _redis;

        public Logger Log
        {
            get; protected set;
        }
        public string Name { get; protected set; }

        protected RedisStorageProvider()
        {
        }
        public virtual Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;
            Log = providerRuntime.GetLogger(this.GetType().FullName);
            _redis = ConnectionMultiplexer.Connect("localhost");
            return TaskDone.Done;
        }
        public Task Close()
        {
            return TaskDone.Done;
        }
        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            IDatabase db = _redis.GetDatabase();
            var grainStateKey = $"{grainType}:{grainReference.GetPrimaryKeyString()}";
            var json = db.StringGet(grainStateKey);
            if (!json.IsNullOrEmpty){
                JsonConvert.PopulateObject(json, grainState);
            }
            await TaskDone.Done;
        }
        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            IDatabase db = _redis.GetDatabase();
            var grainStateKey = $"{grainType}:{grainReference.GetPrimaryKeyString()}";
            var json = JsonConvert.SerializeObject(grainState);
            db.StringSet(grainStateKey,json);
            return TaskDone.Done;
        }
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            IDatabase db = _redis.GetDatabase();
            var grainStateKey = $"{grainType}:{grainReference.GetPrimaryKeyString()}";
            db.KeyDelete(grainStateKey);
            return TaskDone.Done;
        }
    }
}
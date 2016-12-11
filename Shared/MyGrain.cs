using System.Threading.Tasks;

namespace Shared
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

    public interface IMyStatelessGrain : Orleans.IGrainWithIntegerKey
    {
        Task<int> Add(int a, int b);
    }

    [Orleans.Concurrency.StatelessWorker]
    public class MyStatelessGrain : Orleans.Grain, IMyStatelessGrain
    {
        public Task<int> Add(int a, int b)
        {
            return Task.FromResult(a + b);
        }
    }
}
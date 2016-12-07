using System.Threading.Tasks;

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
}
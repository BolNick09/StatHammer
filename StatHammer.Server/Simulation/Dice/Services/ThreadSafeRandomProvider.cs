namespace StatHammer.Server.Simulation.Dice.Services
{
    public class ThreadSafeRandomProvider : IRandomProvider, IDisposable
    {
        private static int _seed = Environment.TickCount;

        private readonly ThreadLocal<Random> _threadLocalRandom = new(() =>
        {
            var localSeed = Interlocked.Increment(ref _seed);
            return new Random(localSeed);
        });

        public int Next(int minValue, int maxValue)
        {
            return _threadLocalRandom.Value!.Next(minValue, maxValue);
        }

        public void Dispose()
        {
            _threadLocalRandom.Dispose();
        }
    }
}

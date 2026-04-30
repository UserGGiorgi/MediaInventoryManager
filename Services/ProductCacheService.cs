namespace MediaInventoryManager.Services
{
    public class ProductCacheService
    {
        private CancellationTokenSource _cts = new();

        public CancellationToken GetCurrentToken() => _cts.Token;

        public void Invalidate()
        {
            var oldCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
            oldCts.Cancel();
            oldCts.Dispose();
        }
    }
}

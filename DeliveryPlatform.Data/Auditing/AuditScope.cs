namespace DeliveryPlatform.Data.Auditing;

public sealed class AuditScope : IAuditScope
{
    public bool IsEnabled { get; private set; }

    public IDisposable Enable()
    {
        IsEnabled = true;
        return new Disposer(() => IsEnabled = false);
    }

    private sealed class Disposer : IDisposable
    {
        private readonly Action _onDispose;

        public Disposer(Action onDispose) => _onDispose = onDispose;

        public void Dispose() => _onDispose();
    }
}/*
  1. عند استدعاء Enable() تصبح IsEnabled = true.
2. عند انتهاء using يتم استدعاء Dispose() تلقائيًا.
3. Dispose() تنفّذ الأمر الذي يعيد IsEnabled = false.
  */
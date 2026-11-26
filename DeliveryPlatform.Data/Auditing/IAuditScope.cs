namespace DeliveryPlatform.Data.Auditing;

public interface IAuditScope
{
    bool IsEnabled { get; }
    IDisposable Enable();
}/*
  IsEnabled: تخبرنا هل التدقيق مفعّل حاليًا أم لا.
Enable(): تفعّل التدقيق، وترجع كائنًا من نوع IDisposable حتى نقدر نوقف التدقيق تلقائيًا عند انتهاء using.
  */
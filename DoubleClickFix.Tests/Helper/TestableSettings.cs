namespace DoubleClickFix.Tests.Helper;

/// <summary>
/// Concrete subclass of SettingsBase with no-op Load/Save for unit testing
/// the base class logic (clamping, change notification, IsDragCorrectionEnabled, etc.)
/// without touching the registry or Windows Store.
/// </summary>
class TestableSettings(ILogger logger) : SettingsBase([], logger)
{
    public override void Save() { }
    public override bool Load() => false; // simulates first-ever launch
}

namespace Quibble.Client.Components
{
    public enum UpdateEvent
    {
        None = 0,
        OnBlur = 1,
        OnInput = 2,
        Both = OnBlur | OnInput
    }
}

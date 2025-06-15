public interface ITarget
{
    bool IsDetected { get; }
    bool IsCaptured { get; }

    void ChangeCapturedStatus();
    void ChangeDetectedStatus();
}

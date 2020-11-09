namespace R8.Lib.MethodReturn
{
    public interface IResponseTrack
    {
        bool Success { get; }
        ValidatableResultCollection Errors { get; }
    }
}
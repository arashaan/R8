namespace R8.Lib.MethodReturn
{
    public interface IResponseBase
    {
        bool Success { get; }
        ValidatableResultCollection Errors { get; }
    }
}
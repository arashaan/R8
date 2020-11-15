using R8.Lib.MethodReturn;

namespace R8.Lib.Test.MethodTests
{
    public class FakeResponse<T> : ResponseBase<T> where T : class
    {
    }

    public class FakeResponse : ResponseBase
    {
    }
}
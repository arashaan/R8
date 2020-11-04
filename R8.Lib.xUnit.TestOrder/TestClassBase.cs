using Xunit;

namespace R8.Lib.xUnit.TestOrder
{
    /// <summary>
    /// These tests only succeed if you run all tests in the class.
    /// </summary>
    [TestCaseOrderer(
        CustomTestCaseOrderer.TypeName,
        CustomTestCaseOrderer.AssembyName)]
    public class TestClassBase
    {
        protected static int I;

        protected void AssertTestName(string testName)
        {
            var type = GetType();
            var queue = CustomTestCaseOrderer.QueuedTests[type.FullName];
            var result = queue.TryDequeue(out var dequeuedName);
            Assert.True(result);
            Assert.Equal(testName, dequeuedName);
        }
    }
}
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace R8.Lib.xUnit.TestOrder
{
    /// <summary>
    /// Custom xUnit test case orderer that uses the TestOrderAttribute
    /// </summary>
    public class CustomTestCaseOrderer : ITestCaseOrderer
    {
        public const string TypeName = "xUnitCustom.CustomTestCaseOrderer";

        public const string AssembyName = "xUnitCustom";

        public static readonly ConcurrentDictionary<string, ConcurrentQueue<string>>
            QueuedTests = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
            IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(GetOrder);
        }

        private static int GetOrder<TTestCase>(
            TTestCase testCase)
            where TTestCase : ITestCase
        {
            // Enqueue the test name.
            QueuedTests
                .GetOrAdd(
                    testCase.TestMethod.TestClass.Class.Name,
                    key => new ConcurrentQueue<string>())
                .Enqueue(testCase.TestMethod.Method.Name);

            // Order the test based on the attribute.
            var attr = testCase.TestMethod.Method
                .ToRuntimeMethod()
                .GetCustomAttribute<TestOrderAttribute>();
            return attr?.I ?? 0;
        }
    }
}
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Nunix.Test.Orderers;

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) 
        where TTestCase : ITestCase
    {
        throw new NotImplementedException();
    }
}
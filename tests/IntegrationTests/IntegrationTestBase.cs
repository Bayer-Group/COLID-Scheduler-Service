using System;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly ITestOutputHelper _output;

        protected IntegrationTestBase(ITestOutputHelper output)
        {
        }

        public void Dispose()
        {
        }
    }
}

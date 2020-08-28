using Xunit;

namespace Alidu.xUnitTest
{
    public abstract class BaseServiceTest<TTestStartup> : IClassFixture<TestServiceApplicationFactory<TTestStartup>>
        where TTestStartup : class
    {
        protected TestServiceApplicationFactory<TTestStartup> _factory;

        public BaseServiceTest(TestServiceApplicationFactory<TTestStartup> factory)
        {
            _factory = factory;
            _factory.CreateClient();
        }
    }
}
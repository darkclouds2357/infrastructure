using Alidu.MessageBus;
using Alidu.MessageBus.InMemory;
using Alidu.MessageBus.Settings;
using Alidu.xUnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SampleDomainServiceTest.Setup;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SampleDomainServiceTest.Test_CreateSampleCommand
{
    public class CreateSampleCommandTest : BaseServiceTest<TestStartup>
    {
        public CreateSampleCommandTest(TestServiceApplicationFactory<TestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public Task CreateSampleCommand_SucessTest()
        {
            using var scope = _factory.Server.Host.Services.CreateScope();

            var serviceProvider = scope.ServiceProvider;

            var sucessTestCaseJson = File.ReadAllText("Test_CreateSampleCommand/create_sample_command_sucess.json");

            var sucessTestCase = JsonConvert.DeserializeObject<SampleTestCase>(sucessTestCaseJson);

            var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Publish(SampleDomainService.SampleCommandEnum.CREATE_NEW_SAMPLE_COMMAND, sucessTestCase.RequestCommand);

            var queues = serviceProvider.GetRequiredService<InMemoryQueue>();

            var config = serviceProvider.GetRequiredService<IOptions<MessageBusConfig>>().Value;

            var sampleCreatedChannels = config.ListenedMessageRoutes[SampleDomainService.SampleEventEnum.NEW_SAMPLE_CREATED_EVENT].Channels;

            foreach (var channel in sampleCreatedChannels)
            {
                Assert.True(queues.GetQueues(channel).Any());
            }

            return Task.CompletedTask;
        }
    }
}
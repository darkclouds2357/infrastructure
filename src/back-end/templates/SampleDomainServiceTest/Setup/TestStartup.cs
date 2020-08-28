namespace SampleDomainServiceTest.Setup
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services
                   .AddCQRS(sp =>
                   {
                       var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
                       var mapper = sp.GetRequiredService<IMapper>();
                       return new FakeEventStoreProvider();
                   })
                   .AddStartupServices(Configuration, typeof(Startup))
                   .AddInMemoryMessageBus(Configuration, GetMessageTypeConfig());
            AddHandler(services);
        }
    }
}
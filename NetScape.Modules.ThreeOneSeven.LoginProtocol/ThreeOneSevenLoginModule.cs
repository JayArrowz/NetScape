using Autofac;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Modules.ThreeOneSeven.LoginProtocol.Handlers;

namespace NetScape.Modules.ThreeOneSeven.LoginProtocol
{
    public class ThreeOneSevenLoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>().SingleInstance();
            builder.RegisterType<HandshakeDecoder>();
            builder.RegisterType<LoginEncoder>();
            builder.RegisterType<LoginDecoder>();
            builder.RegisterType<LoginProcessor>().As<ILoginProcessor<Rs2LoginRequest, Rs2LoginResponse>>()
                .As<IStartable>().SingleInstance();
        }
    }
}

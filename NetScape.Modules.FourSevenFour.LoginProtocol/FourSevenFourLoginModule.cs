using Autofac;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Modules.FourSevenFour.LoginProtocol.Handlers;

namespace NetScape.Modules.FourSevenFour.LoginProtocol
{
    public class FourSevenFourLoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>().SingleInstance();
            builder.RegisterType<HandshakeDecoder>();
            builder.RegisterType<LoginEncoder>();
            builder.RegisterType<JS5Decoder>().AsSelf();
            builder.RegisterType<JS5Encoder>().AsSelf();
            builder.RegisterType<LoginDecoder>();
            builder.RegisterType<LoginProcessor>().As<ILoginProcessor<Rs2LoginRequest, Rs2LoginResponse>>()
                .As<IStartable>().SingleInstance();
        }
    }
}

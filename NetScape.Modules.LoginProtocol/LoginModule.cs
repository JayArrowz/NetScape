using Autofac;
using NetScape.Modules.LoginProtocol.Handlers;
using NetScape.Abstractions.Interfaces.Login;

namespace NetScape.Modules.LoginProtocol
{
    public class LoginModule : Module
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

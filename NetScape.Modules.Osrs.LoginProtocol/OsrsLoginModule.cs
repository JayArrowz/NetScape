using Autofac;
using NetScape.Abstractions.Interfaces.Login;

namespace NetScape.Modules.Osrs.LoginProtocol
{
    public class OsrsLoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>().SingleInstance();
            builder.RegisterType<LoginProcessor>().As<ILoginProcessor<Rs2LoginRequest, Rs2LoginResponse>>().SingleInstance();
            builder.RegisterType<JS5Decoder>().AsSelf();
            builder.RegisterType<JS5Encoder>().AsSelf();
            builder.RegisterType<HandshakeDecoder>().AsSelf();
            base.Load(builder);
        }
    }
}

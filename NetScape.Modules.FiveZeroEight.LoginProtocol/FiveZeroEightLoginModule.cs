using Autofac;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol
{
    public class FiveZeroEightLoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>().SingleInstance();
            builder.RegisterType<HandshakeDecoder>();
            builder.RegisterType<LoginEncoder>();
            builder.RegisterType<JS5Decoder>().AsSelf();
            builder.RegisterType<JS5Encoder>().AsSelf();
            builder.RegisterType<WorldListDecoder>().AsSelf();
            builder.RegisterType<WorldListEncoder>().AsSelf();
            builder.RegisterType<LoginDecoder>();
            builder.RegisterType<LoginProcessor>().As<ILoginProcessor<FiveZeroEightLoginRequest, FiveZeroEightLoginResponse>>()
                .As<IStartable>().SingleInstance();
        }
    }
}

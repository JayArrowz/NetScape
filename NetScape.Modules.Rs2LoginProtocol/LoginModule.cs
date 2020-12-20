using Autofac;
using NetScape.Modules.LoginProtocol.Login;
using NetScape.Abstractions.Interfaces.Login;

namespace NetScape.Modules.LoginProtocol
{
    public class LoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>();
            builder.RegisterType<HandshakeDecoder>();
            builder.RegisterType<LoginEncoder>();
            builder.RegisterType<LoginDecoder>();
        }
    }
}

using NetScape.Abstractions.Interfaces.IO.Login;
using NetScape.Modules.LoginProtocol.IO.Login;
using Autofac;

namespace NetScape.Modules.ThreeOneSeven.LoginProtocol
{
    public class LoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>();
        }
    }
}

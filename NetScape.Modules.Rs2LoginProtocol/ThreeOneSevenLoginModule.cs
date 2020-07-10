using NetScape.Abstractions.Interfaces.IO.Login;
using NetScape.Modules.LoginProtocolThreeOneSeven.IO.Login;
using Autofac;

namespace NetScape.Modules.ThreeOneSeven.LoginProtocol
{
    public class ThreeOneSevenLoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>();
        }
    }
}

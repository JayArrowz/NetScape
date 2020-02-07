using ASPNetScape.Abstractions.Interfaces.IO.Login;
using ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Login;
using Autofac;

namespace ASPNetScape.Modules.ThreeOneSeven.LoginProtocol
{
    public class ThreeOneSevenLoginProtocol : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>();
        }
    }
}

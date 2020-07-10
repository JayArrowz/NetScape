using ASPNetScape.Abstractions.Interfaces.IO;
using ASPNetScape.Abstractions.Interfaces.IO.Login;
using ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.JS5;
using ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.Login;
using Autofac;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol
{
    public sealed class SevenOneEightLoginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginProvider>().As<ILoginProvider>();
            builder.RegisterType<JS5PacketSender>().As<IJS5PacketSender>();
        }
    }
}

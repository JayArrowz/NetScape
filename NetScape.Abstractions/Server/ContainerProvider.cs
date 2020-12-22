using Autofac;

namespace NetScape.Abstractions.Interfaces
{
    /// <summary>
    /// Provides the container across the application
    /// </summary>
    public class ContainerProvider
    {
        public IContainer Container { get; set; }
    }
}

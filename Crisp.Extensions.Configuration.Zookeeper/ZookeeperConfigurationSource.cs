using Microsoft.Extensions.Configuration;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public class ZookeeperConfigurationSource : IConfigurationSource
    {
        public ZookeeperOption Option { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ZookeeperConfigurationProvider(this);
        }
    }
}
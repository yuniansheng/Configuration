using Microsoft.Extensions.Configuration;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// Represents a zookeeper nodes as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class ZookeeperConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// use to config zookeeper
        /// </summary>
        public ZookeeperOption Option { get; set; }

        /// <summary>
        /// Builds the <see cref="ZookeeperConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ZookeeperConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ZookeeperConfigurationProvider(Option, new DefaultZooKeeperFactory());
        }
    }
}
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// Extension methods for adding <see cref="ZookeeperConfigurationProvider"/>.
    /// </summary>
    public static class ZookeeperConfigurationExtensions
    {
        /// <summary>
        /// use zookeeper as configuration source
        /// </summary>
        /// <param name="builder">ConfigurationBuilder</param>
        /// <param name="connectionString">the zookeeper connection string</param>
        /// <param name="rootPath">the zookeeper node path which you want to read it's sub node as key-value</param>
        /// <param name="timeout">zookeeper session timeout in millsecond</param>       
        public static void AddZookeeper(this IConfigurationBuilder builder,
            string connectionString, string rootPath, int timeout)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            builder.AddZookeeper(option =>
            {
                option.ConnectionString = connectionString;
                option.RootPath = rootPath;
                option.SessionTimeout = timeout;
            });
        }

        /// <summary>
        /// Adds a zookeeper configuration source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="config">configures the zookeeper optoin</param>
        public static void AddZookeeper(this IConfigurationBuilder builder, Action<ZookeeperOption> config)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var option = ZookeeperOption.Default;
            config?.Invoke(option);

            var source = new ZookeeperConfigurationSource() { Option = option };
            builder.Add(source);
        }
    }
}

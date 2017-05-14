using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public static class ZookeeperConfigurationExtensions
    {
        public static void UseZookeeper(this IConfigurationBuilder builder,
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

            var source = new ZookeeperConfigurationSource()
            {
                ConnectionString = connectionString,
                RootPath = rootPath,
                SessionTimeout = timeout
            };
            builder.Add(source);
        }
    }
}

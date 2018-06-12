using System;
using System.Collections.Generic;
using System.Text;
using org.apache.zookeeper;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// the default zooKeeper factory.
    /// </summary>
    internal class DefaultZooKeeperFactory : IZooKeeperFactory
    {
        /// <summary>
        /// create zooKeeper instance.
        /// </summary>
        public ZooKeeper CreateZooKeeper(string connectionString, int sessionTimeout, out NodeWatcher watcher)
        {
            watcher = new NodeWatcher();
            return new ZooKeeper(connectionString, sessionTimeout, watcher);
        }
    }
}

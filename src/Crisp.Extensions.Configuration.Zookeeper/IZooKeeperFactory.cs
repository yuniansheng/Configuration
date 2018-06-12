using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// a zooKeeper factory used for create <see cref="ZooKeeper" /> instance.
    /// </summary>
    internal interface IZooKeeperFactory
    {
        /// <summary>
        /// create zooKeeper instance.
        /// </summary>
        /// <param name="connectionString">the zookeeper connection string</param>
        /// <param name="sessionTimeout"></param>
        /// <param name="watcher"></param>
        /// <returns></returns>
        ZooKeeper CreateZooKeeper(string connectionString, int sessionTimeout, out NodeWatcher watcher);
    }
}

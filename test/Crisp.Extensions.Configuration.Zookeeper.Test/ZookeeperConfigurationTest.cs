using Moq;
using System;
using Xunit;
using zk = org.apache.zookeeper;

namespace Crisp.Extensions.Configuration.Zookeeper.Test
{
    public class ZookeeperConfigurationTest
    {
        [Fact]
        public void Test1()
        {
            
        }


        private zk.ZooKeeper CreateZookeeper()
        {
            zk.ZooKeeper zk;            
            var client = new Mock<zk.ZooKeeper>(MockBehavior.Strict);
            return client.Object;
        }
    }
}

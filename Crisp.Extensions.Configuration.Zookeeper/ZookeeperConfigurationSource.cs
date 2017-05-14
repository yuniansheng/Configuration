using Microsoft.Extensions.Configuration;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public class ZookeeperConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Zookeeper连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 会话过期时间，毫秒为单位
        /// </summary>
        public int SessionTimeout { get; set; }

        /// <summary>
        /// 存储配置的根路径
        /// </summary>
        public string RootPath { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ZookeeperConfigurationProvider(this);
        }
    }
}
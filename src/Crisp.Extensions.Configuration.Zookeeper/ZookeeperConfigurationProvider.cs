using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zk = org.apache.zookeeper;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// A zookeeper based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class ZookeeperConfigurationProvider : ConfigurationProvider
    {
        private ZookeeperOption _option;
        private ZookeeperClient _client;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="source">The source settings.</param>
        [Obsolete]
        public ZookeeperConfigurationProvider(ZookeeperConfigurationSource source)
        {
            _option = source.Option;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="option">the zookeeper option.</param>
        public ZookeeperConfigurationProvider(ZookeeperOption option)
        {
            _option = option;
        }

        public ZookeeperConfigurationProvider(ZookeeperClient client)
        {
            _client = client;
            _client.SyncConnected += LoadAsync;
            _client.NodeChanged += OnNodeChanged;
        }

        /// <summary>
        /// Loads the configuration data from zookeeper.
        /// </summary>
        public override void Load()
        {
            var isConnected = _client.WaitConnected(_option.ConnectionTimeout);
            if (!isConnected)
            {
                throw new Exception("connect to zookeeper timeout");
            }
            LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            await RecursiveLoadPath(data, _option.RootPath);
            Data = data;
        }

        private async Task OnNodeChanged(zk.WatchedEvent arg)
        {
            var type = arg.get_Type();
            var path = arg.getPath();
            switch (type)
            {
                case zk.Watcher.Event.EventType.NodeDeleted:
                    Data.Remove(path);
                    OnReload();
                    break;
                case zk.Watcher.Event.EventType.NodeDataChanged:
                    await OnNodeDataChanged(path);
                    OnReload();
                    break;
                case zk.Watcher.Event.EventType.NodeChildrenChanged:
                    await OnNodeChildrenChanged(path);
                    OnReload();
                    break;
                default:
                    break;
            }
        }

        private async Task OnNodeDataChanged(string path)
        {
            Data[path] = await _client.GetDataAsync(path);
        }

        private async Task OnNodeChildrenChanged(string path)
        {
            var children = await _client.GetChildrenAsync(path);
            foreach (var childPath in children)
            {
                Data[path] = await _client.GetDataAsync(path + "/" + childPath);
            }
        }

        private async Task RecursiveLoadPath(IDictionary<string, string> data, string path)
        {
            if (path != _option.RootPath)
            {
                data[path] = await _client.GetDataAsync(path);
            }

            var children = await _client.GetChildrenAsync(path);
            foreach (var childPath in children)
            {
                await RecursiveLoadPath(data, path + "/" + childPath);
            }
        }
    }
}
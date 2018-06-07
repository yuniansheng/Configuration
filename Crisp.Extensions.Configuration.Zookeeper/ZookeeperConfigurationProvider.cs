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
        private zk.ZooKeeper _zk;
        private ZookeeperOption _option;
        private AutoResetEvent _connectedEvent;
        private AutoResetEvent _loadCompletedEvent;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="source">The source settings.</param>
        [Obsolete]
        public ZookeeperConfigurationProvider(ZookeeperConfigurationSource source)
        {
            _option = source.Option;
            _zk = CreateZookeeper();
            _connectedEvent = new AutoResetEvent(false);
            _loadCompletedEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="option">the zookeeper option.</param>
        public ZookeeperConfigurationProvider(ZookeeperOption option)
        {
            _option = option;
            _zk = CreateZookeeper();
            _connectedEvent = new AutoResetEvent(false);
            _loadCompletedEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Loads the configuration data from zookeeper.
        /// </summary>
        public override void Load()
        {
            var isConnected = _connectedEvent.WaitOne(_option.ConnectionTimeout);
            if (!isConnected)
            {
                throw new Exception("connect to zookeeper timeout");
            }
            _loadCompletedEvent.WaitOne();
        }

        private async Task OnStateChanged(zk.WatchedEvent arg)
        {
            var state = arg.getState();
            Console.WriteLine(state);
            if (state == zk.Watcher.Event.KeeperState.SyncConnected)
            {
                _connectedEvent.Set();
                await ReloadData();
                _loadCompletedEvent.Set();
                OnReload();
            }
            else if (state == zk.Watcher.Event.KeeperState.Expired)
            {
                _zk = CreateZookeeper();
            }
        }

        private async Task OnNodeChanged(zk.WatchedEvent arg)
        {
            var type = arg.get_Type();
            var path = arg.getPath();
            switch (type)
            {
                case zk.Watcher.Event.EventType.NodeDeleted:
                    Data.Remove(ToKey(path));
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
            var pair = await GetPathData(path);
            Data[pair.Key] = pair.Value;
        }

        private async Task OnNodeChildrenChanged(string path)
        {
            var children = await _zk.getChildrenAsync(path, watch: true);
            if (children != null)
            {
                foreach (var childPath in children.Children)
                {
                    var pair = await GetPathData(path + "/" + childPath);
                    Data[pair.Key] = pair.Value;
                }
            }
        }


        private async Task ReloadData()
        {
            var kvList = new List<KeyValuePair<string, string>>();
            await RecursiveLoadPath(kvList, _option.RootPath);
            lock (Data)
            {
                Data.Clear();
                kvList.ForEach(item => Data.Add(item));
            }
        }

        private async Task RecursiveLoadPath(List<KeyValuePair<string, string>> kvList, string path)
        {
            if (path != _option.RootPath)
            {
                var pair = await GetPathData(path);
                kvList.Add(pair);
            }

            var children = await _zk.getChildrenAsync(path, watch: true);
            if (children != null)
            {
                foreach (var childPath in children.Children)
                {
                    await RecursiveLoadPath(kvList, path + "/" + childPath);
                }
            }
        }

        private async Task<KeyValuePair<string, string>> GetPathData(string path)
        {
            var node = await _zk.getDataAsync(path, watch: true);
            return new KeyValuePair<string, string>(ToKey(path), ToString(node.Data));
        }

        private zk.ZooKeeper CreateZookeeper()
        {
            var watcher = new NodeWatcher();
            watcher.NodeChanged += OnNodeChanged;
            watcher.StateChanged += OnStateChanged;
            return new zk.ZooKeeper(_option.ConnectionString, _option.SessionTimeout, watcher);
        }

        private string ToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        private string ToKey(string path)
        {
            return path.Substring(_option.RootPath.Length + 1).Replace("/", ConfigurationPath.KeyDelimiter);
        }
    }
}
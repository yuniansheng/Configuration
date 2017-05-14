using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using zk = org.apache.zookeeper;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public class ZookeeperConfigurationProvider : ConfigurationProvider
    {
        private zk.ZooKeeper _zk;
        private NodeWatcher _watcher;

        public ZookeeperConfigurationSource Source { get; }

        public ZookeeperConfigurationProvider(ZookeeperConfigurationSource source)
        {
            Source = source;
            _zk = new zk.ZooKeeper(Source.ConnectionString, Source.SessionTimeout, null);
            _watcher = new NodeWatcher();
            _watcher.NodeChanged += OnNodeChanged;
        }

        public override void Load()
        {
            LoadPath(Source.RootPath).Wait();
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
                    await LoadPathData(path);
                    OnReload();
                    break;
                case zk.Watcher.Event.EventType.NodeChildrenChanged:
                    await LoadPath(path);
                    OnReload();
                    break;
                default:
                    break;
            }
        }

        private async Task LoadPath(string path)
        {
            if (path != Source.RootPath)
            {
                await LoadPathData(path);
            }

            var children = await _zk.getChildrenAsync(path, _watcher);
            if (children != null)
            {
                foreach (var childPath in children.Children)
                {
                    await LoadPath(path + "/" + childPath);
                }
            }
        }

        private async Task LoadPathData(string path)
        {
            var node = await _zk.getDataAsync(path, _watcher);
            Data[ToKey(path)] = ToString(node.Data);
        }

        private string ToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        private string ToKey(string path)
        {
            return path.Substring(Source.RootPath.Length + 1).Replace("/", ConfigurationPath.KeyDelimiter);
        }
    }
}
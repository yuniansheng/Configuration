using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public class NodeWatcher : Watcher
    {
        public event Func<WatchedEvent, Task> NodeChanged;

        public event Func<WatchedEvent, Task> StateChanged;

        public override Task process(WatchedEvent @event)
        {
            if (@event.get_Type() == Event.EventType.None)
            {
                StateChanged?.Invoke(@event);
            }
            else
            {
                NodeChanged?.Invoke(@event);
            }
            return Task.CompletedTask;
        }
    }

    public class ZookeeperClient
    {
        private ZooKeeper _zooKeeper;
        private ManualResetEvent _connectedEvent;

        /// <summary>
        /// the zookeeper connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// zookeeper connection timeout in millsecond,exception will throwed when timeout,default 20000
        /// </summary>
        public int ConnectionTimeout { get; set; }

        internal event Func<Task> SyncConnected;

        public event Func<WatchedEvent, Task> NodeChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">zookeeper conectoin string</param>
        /// <param name="connectionTimeout">zookeeper connection timeout in millsecond,exception will throwed when timeout,default 20000</param>
        public ZookeeperClient(string connectionString, int connectionTimeout)
        {
            ConnectionString = connectionString;
            ConnectionTimeout = connectionTimeout;
            _zooKeeper = CreateZookeeper();
            _connectedEvent = new ManualResetEvent(false);
        }

        public async Task<string> GetDataAsync(string path)
        {
            var result = await _zooKeeper.getDataAsync(path, true);
            return Encoding.UTF8.GetString(result.Data);
        }

        public async Task<List<string>> GetChildrenAsync(string path)
        {
            var result = await _zooKeeper.getChildrenAsync(path, true);
            return result.Children;
        }

        public bool WaitConnected(int millisecondsTimeout)
        {
            return _connectedEvent.WaitOne(millisecondsTimeout);
        }

        private Task OnStateChanged(WatchedEvent arg)
        {
            switch (arg.getState())
            {
                case Watcher.Event.KeeperState.Disconnected:
                    _connectedEvent.Reset();
                    break;
                case Watcher.Event.KeeperState.SyncConnected:
                    _connectedEvent.Set();
                    return SyncConnected();
                case Watcher.Event.KeeperState.AuthFailed:
                    //todo:throw custom exception.
                    throw new Exception("connect to zookeeper auth failed");
                case Watcher.Event.KeeperState.ConnectedReadOnly:
                    //we won't connect readonly when instantiate zookeeper.
                    break;
                case Watcher.Event.KeeperState.Expired:
                    _connectedEvent.Reset();
                    _zooKeeper = CreateZookeeper();
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        private Task OnNodeChanged(WatchedEvent arg)
        {
            return NodeChanged(arg);
        }

        private ZooKeeper CreateZookeeper()
        {
            var watcher = new NodeWatcher();
            watcher.NodeChanged += OnNodeChanged;
            watcher.StateChanged += OnStateChanged;
            return new ZooKeeper(ConnectionString, ConnectionTimeout, watcher);
        }
    }
}

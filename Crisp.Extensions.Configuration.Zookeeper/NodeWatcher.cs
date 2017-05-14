using org.apache.zookeeper;
using System;
using System.Threading.Tasks;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public class NodeWatcher : Watcher
    {
        public event Func<WatchedEvent, Task> NodeChanged;

        public async override Task process(WatchedEvent @event)
        {
            NodeChanged?.Invoke(@event);
        }
    }
}

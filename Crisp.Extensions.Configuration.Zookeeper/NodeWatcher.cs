using org.apache.zookeeper;
using System;
using System.Threading.Tasks;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    public class NodeWatcher : Watcher
    {
        public event Func<WatchedEvent, Task> NodeChanged;

        public event Func<WatchedEvent, Task> StateChanged;

        public async override Task process(WatchedEvent @event)
        {
            var path = @event.getPath();
            if (path == null)
            {
                StateChanged?.Invoke(@event);
            }
            else
            {
                NodeChanged?.Invoke(@event);
            }
        }
    }
}

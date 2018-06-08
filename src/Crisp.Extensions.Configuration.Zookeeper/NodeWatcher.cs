using org.apache.zookeeper;
using System;
using System.Threading.Tasks;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    internal class NodeWatcher : Watcher
    {
        public event Func<WatchedEvent, Task> NodeChanged;

        public event Func<WatchedEvent, Task> StateChanged;

        public override Task process(WatchedEvent @event)
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
            return Task.CompletedTask;
        }
    }
}

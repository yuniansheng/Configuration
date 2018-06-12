using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    /// <summary>
    /// the zooKeeper watcher.
    /// </summary>
    internal class NodeWatcher : Watcher
    {
        /// <summary>
        /// indicate that a node changed.
        /// </summary>
        public event Func<WatchedEvent, Task> NodeChanged;

        /// <summary>
        /// indicate that the zooKeeper state changed.
        /// </summary>
        public event Func<WatchedEvent, Task> StateChanged;

        /// <summary>
        /// Processes the specified event.
        /// </summary>
        /// <param name="event">the watched event.</param>
        /// <returns></returns>
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
}

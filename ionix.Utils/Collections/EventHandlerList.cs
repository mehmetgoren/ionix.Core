namespace Ionix.Utils.Collections
{
    using System;

    public sealed class EventHandlerList : IDisposable
    {
        private sealed class ListEntry
        {
            internal readonly EventHandlerList.ListEntry next;

            internal readonly object key;

            internal Delegate handler;

            public ListEntry(object key, Delegate handler, EventHandlerList.ListEntry next)
            {
                this.next = next;
                this.key = key;
                this.handler = handler;
            }
        }

        private EventHandlerList.ListEntry head;

        /// <summary>Gets or sets the delegate for the specified object.</summary>
        /// <returns>The delegate for the specified key, or null if a delegate does not exist.</returns>
        /// <param name="key">An object to find in the list. </param>
        public Delegate this[object key]
        {
            get
            {
                EventHandlerList.ListEntry listEntry = this.Find(key);
                return listEntry?.handler;
            }
            set
            {
                EventHandlerList.ListEntry listEntry = this.Find(key);
                if (listEntry != null)
                {
                    listEntry.handler = value;
                    return;
                }
                this.head = new EventHandlerList.ListEntry(key, value, this.head);
            }
        }

        /// <summary>Adds a delegate to the list.</summary>
        /// <param name="key">The object that owns the event. </param>
        /// <param name="value">The delegate to add to the list. </param>
        public void AddHandler(object key, Delegate value)
        {
            EventHandlerList.ListEntry listEntry = this.Find(key);
            if (listEntry != null)
            {
                listEntry.handler = Delegate.Combine(listEntry.handler, value);
                return;
            }
            this.head = new EventHandlerList.ListEntry(key, value, this.head);
        }

        /// <summary>Adds a list of delegates to the current list.</summary>
        /// <param name="listToAddFrom">The list to add.</param>
        public void AddHandlers(EventHandlerList listToAddFrom)
        {
            for (EventHandlerList.ListEntry next = listToAddFrom.head; next != null; next = next.next)
            {
                this.AddHandler(next.key, next.handler);
            }
        }

        /// <summary>Disposes the delegate list.</summary>
        public void Dispose()
        {
            this.head = null;
        }

        private EventHandlerList.ListEntry Find(object key)
        {
            EventHandlerList.ListEntry next = this.head;
            while (next != null && next.key != key)
            {
                next = next.next;
            }
            return next;
        }

        /// <summary>Removes a delegate from the list.</summary>
        /// <param name="key">The object that owns the event. </param>
        /// <param name="value">The delegate to remove from the list. </param>
        public void RemoveHandler(object key, Delegate value)
        {
            EventHandlerList.ListEntry listEntry = this.Find(key);
            if (listEntry != null)
            {
                listEntry.handler = Delegate.Remove(listEntry.handler, value);
            }
        }
    }
}

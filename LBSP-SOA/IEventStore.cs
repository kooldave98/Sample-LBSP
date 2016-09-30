using System;
using System.Collections.Generic;

namespace LbspSOA
{
    public interface IEventStore
    {
        IEnumerable<RawEvent> get_history();
        void Publish(RawEvent payload, RawEventPointer log_payload = null);
        void Subscribe(string stream_name, string event_type, Action<RawEvent> on_message_received);
        void unsubscribe_all();
    }


    public class RawEvent
    {
        public readonly Guid id;
        public readonly byte[] data;
        public readonly byte[] metadata;
        public readonly string type;
        public readonly string origin_stream;
        public readonly int position_in_stream;

        public RawEvent(Guid id,
                        byte[] data,
                        byte[] metadata,
                        string type,
                        string origin_stream = "",
                        int position_in_stream = -1)
        {
            this.id = id;
            this.data = data;
            this.metadata = metadata;
            this.type = type;
            this.origin_stream = origin_stream;
            this.position_in_stream = position_in_stream;
        }

        public RawEventPointer ToPointer()
        {
            if (string.IsNullOrWhiteSpace(this.origin_stream)) throw new InvalidOperationException("origin stream is empty");
            if (this.position_in_stream < 0) throw new InvalidOperationException("position is invalid");


            return new RawEventPointer(this.origin_stream, this.position_in_stream);
        }
    }

    public class RawEventPointer
    {
        public string stream_name { get; set; }

        public int position { get; set; }

        public RawEventPointer(string stream_name, int position)
        {
            this.stream_name = stream_name;
            this.position = position;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LbspSOA
{
    public interface IEventStore
    {
        IEnumerable<RawEvent> get_history();
        void PublishResponse(IEnumerable<RawEvent> events, RecordedRawEvent origin_event = null);
        void PublishErrors(IEnumerable<RawEvent> raw_events, string origin_stream);
        void Subscribe(string stream_name, Action<RecordedRawEvent> on_message_received);
        void unsubscribe_all();
    }

    public class RecordedRawEvent
    {
        public readonly string origin_stream;
        public readonly int position_in_stream;
        public readonly RawEvent raw_event;

        public RecordedRawEvent(RawEvent raw_event,
                                string origin_stream,
                                int position_in_stream)
        {
            this.raw_event = raw_event;
            this.origin_stream = origin_stream;
            this.position_in_stream = position_in_stream;
        }

        public RecordedRawEventPointer ToPointer()
        {
            if (string.IsNullOrWhiteSpace(this.origin_stream)) throw new InvalidOperationException("origin stream is empty");
            if (this.position_in_stream < 0) throw new InvalidOperationException("position is invalid");


            return new RecordedRawEventPointer(this.origin_stream, this.position_in_stream);
        }
    }

    public static class RawEventExtensions
    {
        public static byte[] ToBytes(this object source)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(source));
        }

        public static string ToJsonString(this byte[] bytes_data)
        {
            return Encoding.UTF8.GetString(bytes_data);
        }

        public static dynamic ToJsonDynamic(this byte[] bytes_data)
        {
            return JToken.Parse(bytes_data.ToJsonString());
        }
    }


    public class RawEvent
    {
        public readonly Guid id;
        public readonly byte[] data;
        public readonly byte[] metadata;
        public readonly string type;

        public RawEvent(Guid id,
                        byte[] data,
                        byte[] metadata,
                        string type)
        {
            this.id = id;
            this.data = data;
            this.metadata = metadata;
            this.type = type;
        }
    }

    public class RecordedRawEventPointer
    {
        public string stream_name { get; set; }

        public int position { get; set; }

        public RecordedRawEventPointer(string stream_name, int position)
        {
            this.stream_name = stream_name;
            this.position = position;
        }
    }
}
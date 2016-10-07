using System;

namespace LbspSOA
{
    public class RawRequest<W> where W : IWorld
    {
        public Guid id { get; private set; }

        public string service_type { get; private set; }

        public string trigger_as_json { get; private set; }

        public dynamic trigger_handler { get; private set; }

        public RawRequest
                (Guid id
                , string trigger_as_json
                , string service_type
                , dynamic trigger_handler)
        {
            this.id = id;
            this.trigger_as_json = trigger_as_json;
            this.service_type = service_type;
            this.trigger_handler = trigger_handler;
        }
    }

    public class RawResponse<W> where W : IWorld
    {
        public RawRequest<W> raw_request { get; private set; }

        public Response<W> core_response { get; private set; }

        public RawResponse
                (RawRequest<W> the_raw_request
                , Response<W> the_core_response)
        {

            this.raw_request = the_raw_request;
            this.core_response = the_core_response;

        }
    }
}

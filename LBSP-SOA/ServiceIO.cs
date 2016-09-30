using System;

namespace LbspSOA
{
    public class RawRequest<W> where W : IWorld
    {
        public Guid id { get; private set; }

        public string service_type { get; private set; }

        public dynamic memento { get; private set; }

        public Func<Request<W>, Response<W>> handler { get; private set; }

        public RawRequest
                (Guid id
                , dynamic attributes
                , string service_type
                , Func<Request<W>, Response<W>> handler)
        {
            this.id = id;
            this.memento = attributes;
            this.service_type = service_type;
            this.handler = handler;
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

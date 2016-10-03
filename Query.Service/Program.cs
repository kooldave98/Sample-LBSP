using System;
using System.Text;
using LbspSOA;
using Newtonsoft.Json.Linq;
using Registration.Interface;

namespace Query.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            //var event_store = new GESEventStore("Query");

            //event_store.Subscribe("RegistrationWorld", raw_event => {

            //    if(raw_event.type == nameof(ParkingHostCreated))
            //    {
            //        Console.WriteLine("Query domain received event");

            //        var data = JArray.Parse(Encoding.UTF8.GetString(raw_event.data)) as dynamic;


            //        var repository = new Repository<Host>();

            //        repository.add(new Host()
            //        {
            //            HostID = data.host_id,
            //            Email = data.email,
            //            Username = data.username
            //        });

            //        repository.commit();


            //        //var payload =
            //        //    new RawEvent(raw_event.id,
            //        //                raw_event.id.ToByteArray(),
            //        //                null,
            //        //                "WfaUpdated");

            //        //event_store.Publish("QueryDomain", payload);

            //        Console.WriteLine("Query domain finished writing event");
            //    }
                
            //});

            //Console.WriteLine("Started listening for events");

            //Console.ReadLine();
        }
    }

}



using System;
using CodeStructures;
using LbspSOA;
using Query.Interface;
using Registration.Interface;

namespace Query.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var event_store = new GESEventStore(Interface.NameService.ContextName);

            event_store.Subscribe(Registration.Interface.NameService.ContextName, raw_event =>
            {

                if (raw_event.raw_event.type == nameof(ParkingHostCreated))
                {
                    Console.WriteLine("Query domain received event");

                    var data = raw_event.raw_event.data.ToJsonDynamic();


                    var repository = new Repository<Host>(new AppDbContext());

                    repository.add(new Host()
                    {
                        HostID = data.host_id,
                        Email = data.email,
                        Username = data.username
                    });

                    repository.commit();
                    

                    var to_publish =
                        new RawEvent(Guid.NewGuid(),
                                    new ParkingHostMaterialised().ToBytes(),
                                    new { parent_id = raw_event.raw_event.id }.ToBytes(),
                                    nameof(ParkingHostMaterialised));

                    event_store.CommitAndPublish(raw_event, to_publish.ToEnumerable());

                    Console.WriteLine("Query domain finished writing event");
                }

            });

            Console.WriteLine("Started listening for events");

            Console.ReadLine();
        }
    }
}
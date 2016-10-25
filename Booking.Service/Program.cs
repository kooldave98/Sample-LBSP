using Booking.Domain;
using LbspSOA;

namespace Booking.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var context_name = Booking.Interface.NameService.ContextName;
            var seed_world = BookingWorld.seed_world();

            new ServiceBootstrap<BookingWorld>(context_name, seed_world)
                .replay()
                .listen_to(Gateway.Interface.NameService.ContextName)
                .StartService();
        }
    }
}

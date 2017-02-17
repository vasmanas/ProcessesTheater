using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessesTheater.Core;
using ProcessesTheater.Core.Eventing;
using ProcessesTheater.Core.Logging;

namespace ProducerConsumer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Bus.SetBus(new InMemoryBus());
            
            var producer =
                new SingleThreadCharacter(
                    new NowCause().WrapWithPause(3),
                    new ActionBehavior<SingleValueEffect<DateTime>>(eff => Bus.Send(new RequestCommand<DateTime>(eff.Value))),
                    TimeSpan.FromSeconds(10));

            var handler = new RequestReplyCause<DateTime>();
            Bus.RegisterHandler(handler);

            var consumer =
                new SingleThreadCharacter(
                    handler.WrapWithPause(1).WrapWithRequired(),
                    new ActionBehavior<SingleValueEffect<DateTime>>(eff => Console.WriteLine("Hello, World! At {0}", eff.Value)),
                    TimeSpan.FromSeconds(10));

            consumer.Start();
            producer.Start();

            Console.ReadLine();
        }
    }
}

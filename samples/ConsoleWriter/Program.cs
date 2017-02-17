using System;
using ProcessesTheater.Core;
using ProcessesTheater.Core.Eventing;
using ProcessesTheater.Core.Logging;

namespace ConsoleWriter
{
    public class Program
    {
        static void Main(string[] args)
        {
            var character =
                new BasicCharacter(
                    new NowCause().WrapWithPause(2),
                    new ActionBehavior<SingleValueEffect<DateTime>>(eff => Console.WriteLine("Hello, World! At {0}", eff.Value)));

            character.Start();
        }
    }
}

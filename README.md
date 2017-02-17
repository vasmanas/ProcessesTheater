# Processes Theater
Easier parallel processes creation, understanding and maintenance.

Want to create multi process aplication it is simple:

```
var character =
    new BasicCharacter(
        new NowCause().WrapWithPause(2),
        new ActionBehavior<SingleValueEffect<DateTime>>(eff => Console.WriteLine("Hello, World! At {0}", eff.Value)));

character.Start();
```

And thats it, your first character is born.
Here is a bit more complex example of producer consumer:

```
/// Creating a bus for communication.
Bus.SetBus(new InMemoryBus());

/// Creating producer for sending notifications.
var producer =
    new SingleThreadCharacter(
        new NowCause().WrapWithPause(3),
        new ActionBehavior<SingleValueEffect<DateTime>>(eff => Bus.Send(new RequestCommand<DateTime>(eff.Value))),
        TimeSpan.FromSeconds(10));

/// Creating listening method
var handler = new RequestReplyCause<DateTime>();
Bus.RegisterHandler(handler);

/// Creating consumer for reading notifications.
var consumer =
    new SingleThreadCharacter(
        handler.WrapWithPause(1).WrapWithRequired(),
        new ActionBehavior<SingleValueEffect<DateTime>>(eff => Console.WriteLine("Hello, World! At {0}", eff.Value)),
        TimeSpan.FromSeconds(10));

/// Starting process
consumer.Start();
producer.Start();
```

You don't need to worry about thread loop, task creation, proper disposing it all be done by characters. Just write your own cause with single method:
```
IEffect Check(CancellationToken cancellationToken);
```

And your own behavior with simple method:
```
void Act(IEffect value, CancellationToken cancellationToken);
```

Pass custom effect or use one of predefined: TimerElapsedEffect, SingleValueEffect, MultipleEffects or BatchEffect.

Just reuse causes and behaviors by wrapping them up.

# About
This library is a helper when developing a multi process application. It uses a small pattern of Cause, Behavior and Character. Cause is initiator or trigger of an action. It chacks if conditions are met and an actions can be started. Behavior is container for an action when cause conditions are met, behavior is initiated and it executes one or more actions. Cause passes an effect object with information about what happened. It can be time, when somethig happened, object from queue to work with or nothig. Character is an agent that wraps cause and behavior and is responsible for their execution. It can be thread, task or any other exection context.

In multi process enviroment there are multiple characters, and they comunicate with each other using events or commands. Event is general information that something happened, command is information to a specific character to do somethig. In order for them to comunicate characters register on bus, and when a message is send, bus distributes it to recipients.

Write to me when information is needed or there are suggestions. Thank you in advance.
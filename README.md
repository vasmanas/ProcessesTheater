# ProcessesTheater
Easier parallel processes creation, understanding and maintenance.

# About
This library is a helper when developing a multi process application. It uses a small pattern of Cause, Behavior and Character. Cause is initiator or trigger of an action. It chacks if conditions are met and an actions can be started. Behavior is container for an action when cause conditions are met, behavior is initiated and it executes one or more actions. Cause passes an effect object with information about what happened. It can be time, when somethig happened, object from queue to work with or nothig. Character is an agent that wraps cause and behavior and is responsible for their execution. It can be thread, task or any other exection context.

In multi process enviroment there are multiple characters, and they comunicate with each other using events or commands. Event is general information that something happened, command is information to a specific character to do somethig. In order for them to comunicate characters register on bus, and when a message is send, bus distributes it to recipients.
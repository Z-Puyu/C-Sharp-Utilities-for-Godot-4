# C\# Utilities for Godot 4

This repository contains utility classes I have written to help facilitate efficient development of my own Godot 4 game projects. You can import them to your own project by simply copying the script files over. The rest of the document gives a brief guide to using these classes.

## `EventBus`: The Global Event Emitter and Handler

The `EventBus` class is a static class to emit, receive and handle C\# events globally, so that details of event connection can be minimised in game objeccts. To put it simply, the event handling cycle in C\# looks like the follows:

1. A custom class extending `EventArgs` is created to capture relevant data of an event type.
2. In the *receiver* object, the custom event type is associated to a delegate representing a function `(object sender, EventArgs e) => void`. This function will be invoked when the correct type of events is captured.
3. An *emitter* object will then be responsible of invoking the event.

The `EventBus` class, however, simplifies the process by offering a global node to transmit events between objects. To demonstrate how to use it, let us first create a custom event class:

```csharp
using System;

public class MyCustomEvent : EventArgs {}
```

Now let us set up the emitter and listener classes:

```csharp
using System;

public class Emitter {}

public class Listener {
    public void OnMyCustomEventReceived(object sender, EventArgs e) {
        Console.WriteLine("MyCustomEvent is received!");
    }
}

class TestClass {
    static void Main(string[] args) {
        Emitter emitter = new Emitter();
        Listener listener = new Listener();
    }
}
```

Here, `OnMyCustomEventReceived` is meant to be executed upon receiving a `MyCustomEvent`. Note that all methods and lambda functions connected to a C\# event must take in 2 parameters of type `object` (the event emitter) and `EventArgs` (the event) and have a return type of `void`.

To make our `listener` actually **listen** to the event, we simply invoke an extension method (defined in `EventBus.cs`):

```csharp
listener.Subscribe<MyCustomEvent>(listener.OnMyCustomEventReceived);
```

Now, to invoke the event and subsequently the method, we only need to **publish** the event from `emitter` using another extension method:

```csharp
emitter.Publish(new MyCustomEvent());
```

### Disconnecting events

When we want to delete the `listener` object from the above example, however, we might wish to stop it from listening to the events to isolate it from the other objects in our code. To do so is also simple, by calling the extension method as follows:

```csharp
listener.Unsubscribe<MyCustomEvent>(listener.OnMyCustomEventReceived);
```

However, in a more complex project, our `listener` object might have been listening to a lot of different events and it is not practical to keep track of all the event types it has subscribed to. In such cases, we can alternatively call the `listener.UnsubscribeAllEvents()` to disconnect it from all previously connected events.

Full example code:

```csharp
using System;
using GodotUtilities;

public class MyCustomEvent : EventArgs {}

public class Emitter {}

public class Listener {
    public void OnMyCustomEventReceived(object sender, EventArgs e) {
        Console.WriteLine("MyCustomEvent is received!");
    }
}

class TestClass {
    static void Main(string[] args) {
        Emitter emitter = new Emitter();
        Listener listener = new Listener();

        // Listen to the event of type MyCustomEvent.
        listener.Subscribe<MyCustomEvent>(listener.OnMyCustomEventReceived);

        // Publish an event of type MyCustomEvent and trigger the OnMyCustomEventReceived method.
        emitter.Publish(new MyCustomEvent());

        // Stop listening to the event of type MyCustomEvent.
        listener.Unsubscribe<MyCustomEvent>(listener.OnMyCustomEventReceived);
        // Or alternatively, stop listening to all events previously connected to the object.
        listener.UnsubscribeAllEvents();
    }
}
```

## Finite State Machine

An general implementation of the *finite state machine* is included under the directory `finite_state_machine`. This implementation is meant to be **used together with the `EventBus`**.

### `State`

`State` is an abstract class extending from `Node`. It encapsulates a state used by a state machine. You should inherit from this class to implement your own custom states. The following functions can be optionally overridden to customise the behaviour of the state:

1. `Enter` and `Exit`: actions to be executed upon entering and exiting the state.
2. `OnReady`: actions to be executed when the state machine managing the state is ready.
3. `Process` and `PhysicsProcess`: handle the `_Process` and `_PhysicsProcess` from the owner node of the state respectively.
4. `OnInput`: handles `InputEvent` received by the owner node of the state.
5. `Handle`: handles custom C\# events.

### `StateMachine`

`StateMachine` is an implementation of a finite state machine. It should generally be **placed as a direct child of the node which uses it** and it should also be the **direct parent of all state nodes managed by it**. It has an export property called `InitialState`, which should be **manually set** in Godot's inspector. The state machine provides implementations for the following functionalities:

1. `ListenToEvent`: a generic method to instruct the state machine to handle a C\# event.
2. `TransitionTo`: transition to another state if the target state is valid. A general guideline is to invoke this method from a `State` node only.
3. `Open`, `Close`, `Destroy`: enable or disable the state machine. `Destroy` also removes the state machine from the scene tree completely.

## `GodotExtensions`: extension methods for Godot's `Node` class

This class offers 2 extension methods for the `Node` class in Godot.

1. `GetSubTree()`: when invoked from a `Node`, this method returns a `Stack<Node>` containing all nodes in the sub-tree rooted at the current node, including the current node itself. When popping from the stack, it is guaranteed that any child will be popped before all of its ancestor nodes.
2. `Recycle()`: this method is meant to be used together with `EventBus`. It will recursively call `UnsubscribeAll` on every node in the sub-tree rooted at the current node, and delete the entire sub-tree afterwards.

## `Utilities`: general utility methods

This class offers some general utility methods.

1. `CantorPairing`: use a bijective map to map a pair of non-negative integers to a single non-negative integer.
2. `UniqueId`: use a bijective map to map a pair of integers to a single non-negative integer.
3. `Randi`, `Rand`: generate a random integer or floating point number (an optional range can be applied).
4. `FairCoin`: generate a boolean value at a 50-50 chance.
5. `Shuffle`: generate a random permutation of an `IList`.
6. `RandomSelect`: randomly select $k$ (default 1) items from an `IList`.

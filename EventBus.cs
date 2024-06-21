using System;
using System.Collections.Generic;

namespace GodotUtilities {
	/// <summary>
	/// A static class serving as a global C# event emitter and handler. This class allows custom Godot signals to be replaced by custom C# events to make the development process more modular. The methods in this class are implemented as extension methods to the <c>object</c> class.
	/// </summary> 
	public static class EventBus {
		private static readonly Dictionary<Type, EventHandler> subscribers = [];
		private static readonly Dictionary<object, Dictionary<Type, HashSet<EventHandler>>> lookup = [];

		/// <summary>
		/// Make <paramref name="src"/> subscribe to an event of type <typeparamref name="T"/> such that <paramref name="eventHandler"/> is invoked upon receiving an event of type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="src">The source object which subscribes to the event.</param>
		/// <param name="eventHandler">The delegate representing the function to invoke upon receiving the event.</param>
		/// <typeparam name="T">The type of event, which is a subclass of <c>EventArgs</c>.</typeparam>
		public static void Subscribe<T>(this object src, EventHandler eventHandler) where T : EventArgs {
			Type eventType = typeof(T);
			if (!EventBus.subscribers.TryAdd(eventType, eventHandler)) {
				EventBus.subscribers[eventType] += eventHandler;
			}
			if (EventBus.lookup.TryGetValue(src, out Dictionary<Type, HashSet<EventHandler>> dict)) {
				if (!dict.TryAdd(eventType, [eventHandler])) {
					dict[eventType].Add(eventHandler);
				}
			}
		}

		/// <summary>
		/// Unsubscribe the event of type <typeparamref name="T"/> from <paramref name="src"/>.
		/// </summary>
		/// <param name="src">The source object which is currently subscribing to the event.</param>
		/// <param name="eventHandler">The delegate representing a function which was meant to be invoked upon receiving the event.</param>
		/// <typeparam name="T">The type of event, which is a subclass of <c>EventArgs</c>.</typeparam>
		public static void Unsubscribe<T>(this object src, EventHandler eventHandler) where T : EventArgs {
			Type eventType = typeof(T);
			if (EventBus.subscribers.ContainsKey(eventType)) {
				EventBus.subscribers[eventType] -= eventHandler;
			}
			if (EventBus.lookup.TryGetValue(src, out Dictionary<Type, HashSet<EventHandler>> dict)) {
                dict[eventType].Remove(eventHandler);
			}
		}

		/// <summary>
		/// Publish an event <paramref name="e"/> globally so that all objects can listen to the event.
		/// </summary>
		/// <param name="sender">The object which emits the event.</param>
		/// <param name="e">The event emitted.</param>
		public static void Publish(this object sender, EventArgs e) {
			Type eventType = e.GetType();
			if (EventBus.subscribers.TryGetValue(eventType, out EventHandler listeners)) {
				listeners(sender, e);
			}
		}

		/// <summary>
		/// Unsubscribe all events from <paramref name="src"/>. Use this method when deleting or disabling an object.
		/// </summary>
		/// <param name="src">The source object which needs all subscriptions to be cancelled.</param>
		public static void UnsubscribeAllEvents(this object src) {
			if (EventBus.lookup.TryGetValue(src, out Dictionary<Type, HashSet<EventHandler>> dict)) {
				foreach (Type eventType in dict.Keys) {
					foreach (EventHandler f in dict[eventType]) {
						EventBus.subscribers[eventType] -= f;
					}
				}
				EventBus.lookup.Remove(src);
			}
		}
	}
}

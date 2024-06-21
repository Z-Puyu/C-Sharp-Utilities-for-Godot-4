using System;
using System.Collections.Generic;
using Godot;
using GodotUtilities;

namespace Utilities.FSM {
    public partial class StateMachine : Node {
        [Export] private NodePath InitialState { set; get; }
        private readonly Dictionary<string, State> states = [];
        private State currState;
        private Node root;

        public Node Root { get => root; }

        public override async void _Ready() {
			base._Ready();
            if (this.Owner != null) {
                await this.ToSignal(this.Owner, SignalName.Ready);
                this.root = this.Owner;
            } else {
                Node parent = this.GetParent();
                await this.ToSignal(parent, SignalName.Ready);
                this.root = parent;
            }
			this.Open();
            foreach (State s in this.states.Values) {
                s.OnReady();
            }
		}

        /// <summary>
        /// Open the state machine by fetching all its states and initialising the initial state.
        /// </summary>
        public void Open() {
            foreach (Node node in this.GetChildren()) {
				if (node is State s) {
					this.states[s.Name] = s;
					s.Fsm = this;
					s.Exit(); // Reset the state.
				}
			}

			this.currState = this.GetNode<State>(this.InitialState);
			this.currState.Enter(); // Activate the initial state.
        }

		/// <summary>
		/// Checks if the state of type <paramref name="t"/> is a valid next state for this FSM.
		/// </summary>
		/// <param name="t"> The type of the state to be checked. </param>
		/// <param name="s"> The next state to transition to. </param>
		/// <returns> <c>true</c> if the state type <paramref name="t"/> exists in this FSM 
		/// and is different from the type of the current state. </returns>
		private bool IsValidState(string name, out State s) {
			return this.states.TryGetValue(name, out s) && s != this.currState;
		}

        /// <summary>
        /// Transition to the state with name <paramref name="name"/>, if it exists.
        /// </summary>
        /// <param name="name">The name of the next state.</param>
		public void TransitionTo(string name) {
			if (this.IsValidState(name, out State s)) {
				this.currState.Exit();
				this.currState = s;
				this.currState.Enter();
			}
		}

        /// <summary>
        /// Subscribe the state machine to an event of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the event to subscribe to.</typeparam>
        public void ListenToEvent<T>() where T : EventArgs {
            this.Subscribe<T>(this.Handle);
        }

        public override void _Process(double delta) {
            this.currState.Process(delta);
        }

        public override void _PhysicsProcess(double delta) {
            this.currState.PhysicsProcess(delta);
        }

        public override void _UnhandledInput(InputEvent e) {
			this.currState.OnInput(e);
		}

        /// <summary>
        /// Handle a C# event in the current state.
        /// </summary>
        /// <param name="sender">The emitter of the event.</param>
        /// <param name="e">The event.</param>
		private void Handle(object sender, EventArgs e) {
			this.currState.Handle(sender, e);
		}

        /// <summary>
        /// Close the state machine.
        /// </summary>
        public void Close() {
            this.UnsubscribeAllEvents();
            this.currState = null;
            this.states.Clear();
        }

        /// <summary>
        /// Remove the state machine from the scene.
        /// </summary>
        public void Destroy() {
            this.Close();
            this.QueueFree();
        }
    }
}
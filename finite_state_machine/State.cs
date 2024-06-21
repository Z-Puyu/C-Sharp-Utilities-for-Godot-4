using System;
using Godot;

namespace Utilities.FSM {
    public abstract partial class State : Node {
        private StateMachine fsm;

        public StateMachine Fsm { set => fsm = value; }

        public virtual void Enter() {}
        public virtual void Exit() {}
        public virtual void OnReady() {}
        public virtual void Process(double delta) {}
        public virtual void PhysicsProcess(double delta) {}
        public virtual void OnInput(InputEvent @event) {}
        public virtual void Handle(object sender, EventArgs e) {}
    }
}
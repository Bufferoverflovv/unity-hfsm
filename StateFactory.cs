using System.Collections.Generic;
using UnityEngine;

namespace Buffer.HFSM
{
    public class StateFactory
    {
        public StateMachine machine { get; }
        private readonly Dictionary<string, StateBase> states = new();

        /// <summary>
        /// Creates a new StateFactory for the given StateMachine.
        /// </summary>
        /// <param name="machine"></param>
        public StateFactory(StateMachine machine)
        {
            this.machine = machine;
        }

        /// <summary>
        /// Returns a state of the given type. If it doesn't exist, it will be created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetState<T>() where T : StateBase, new()
        {
            string key = typeof(T).ToString();
            if (states.ContainsKey(key)) return (T)states[key];
            T state = new T();
            states.Add(key, state);
            state.Setup(machine);
            Debug.Log("Added new state: " + key);
            return state;
        }

        /// <summary>
        /// Returns a state of the given type. If it doesn't exist, it will be created.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public StateBase GetState(StateBase state)
        {
            string key = state.GetType().ToString();
            if (states.ContainsKey(key)) return states[key];
            states.Add(key, state);
            state.Setup(machine);
            Debug.Log("Added new state: " + key);
            return state;
        }
    }
}

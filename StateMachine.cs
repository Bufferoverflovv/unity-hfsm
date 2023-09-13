using UnityEngine;

namespace Buffer.HFSM
{
    public abstract class StateMachine : MonoBehaviour
    {
        public StateFactory factory;
        protected StateBase previousState;
        protected StateBase currentState;

        public StateBase CurrentState { get { return currentState; } }

        protected virtual void Awake()
        {
            factory = new StateFactory(this);
        }

        /// <summary>
        /// Sets up the machine with the given state as the initial state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected virtual void SetupMachine<T>() where T : StateBase, new()
        {
            T initialState = factory.GetState<T>();
            if (initialState.IsRootState)
            {
                currentState = initialState;
                currentState.OnEnterState();
            }
            else 
            {
                Debug.LogError("Initial state is not a root state!");
            }
        }

        /// <summary> Tries to set current state. </summary>
        public void TrySetState<T>() where T : StateBase, new()
        {
            T state = factory.GetState<T>();
            if (state == currentState) return;
            if (state.CanEnterState()) SetState(state);
        }

        /// <summary> Sets current state. </summary>
        protected virtual void SetState(StateBase newState)
        {
            if (newState.IsRootState)
            {
                if (!currentState.CanExit()) return;
                currentState?.OnExitState();
                previousState = currentState;
                currentState = newState;
                currentState.OnEnterState();
            }
        }

        protected virtual void Update()
        {
            currentState.Update(Time.deltaTime);
            currentState.CheckIfShouldSwitchState();
        }

        protected virtual void FixedUpdate()
        {
            currentState.FixedUpdate(Time.fixedDeltaTime);
        }

        protected virtual void LateUpdate()
        {
            currentState.LateUpdate(Time.smoothDeltaTime);
        }

        protected virtual void OnDrawGizmos()
        {
            currentState?.OnDrawGizmos();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            currentState?.OnDrawGizmosSelected();
        }
    }
}

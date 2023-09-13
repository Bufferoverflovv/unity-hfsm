namespace Buffer.HFSM
{
    /// <summary>
    /// Base class for all states.
    /// </summary>
    public abstract class StateBase
    {
        protected bool isRootState;
        protected StateBase currentSuperState;
        protected StateBase currentSubState;

        public bool IsRootState { get { return isRootState; } }
        public StateBase CurrentSuperState { get { return currentSuperState; } }
        public StateBase CurrentSubState { get { return currentSubState; } set { currentSubState = value; } }

        /// <summary> Sets the basic values of the state. </summary>
        /// <param name="machine"> State machine owning this state. </param>
        public virtual void Setup(StateMachine machine)
        {
            Initialize();
        }

        /// <summary>
        /// Sets the super state of this state.
        /// </summary>
        /// <param name="superState"></param>
        public virtual void SetSuperState(StateBase superState)
        {
            currentSuperState = superState;
        }

        /// <summary> Initialization of the state - a method invoked only once, when the state is created. </summary>
        public virtual void Initialize() { }

        /// <summary> Defines if the state can be entered. </summary>
        public virtual bool CanEnterState()
        {
            return true;
        }

        /// <summary> Invoked when the machine enters this state. </summary>
        public virtual void OnEnterState() { }

        /// <summary> Invoked in Update of the machine, if this is its current state. </summary>
        public virtual void Update(float dt) 
        {
            if (currentSubState != null)
                currentSubState.Update(dt);
        }

        /// <summary> Invoked in FixedUpdate of the machine, if this is its current state. </summary>
        public virtual void FixedUpdate(float fdt) 
        {
            if (currentSubState != null)
                currentSubState.FixedUpdate(fdt);
        }

        /// <summary> Invoked in LateUpdate of the machine, if this is its current state. </summary>
        public virtual void LateUpdate(float dt) 
        { 
            if (currentSubState != null)
                currentSubState.LateUpdate(dt);
        }
        
        /// <summary>
        ///  Invoked in LateFixedUpdate of the machine, if this is its current state.
        ///  E.g called AFTER Physics internal update.
        /// </summary>
        /// <param name="fdt"></param>
        public virtual void LateFixedUpdate(float fdt) 
        { 
            if (currentSubState != null)
                currentSubState.LateFixedUpdate(fdt);
        }

        /// <summary> Checks if the machine should switch to another state. </summary>
        public virtual void CheckIfShouldSwitchState() { }

        public virtual bool CanExit() 
        {
            return true;
        }

        /// <summary> Invoked just before the machine exits this state. </summary>
        public virtual void OnExitState() 
        { 
            currentSubState?.OnExitState();
        }

        public virtual void OnDrawGizmos() { }

        public virtual void OnDrawGizmosSelected() { }
    }

    /// <summary> One of the states the StateMachine can use. </summary>
    /// <typeparam name="T">StateMachine that owns this State.</typeparam>
    public abstract class State<T> : StateBase where T : StateMachine
    {
        protected T machine;

        public override void Setup(StateMachine machine)
        {
            this.machine = machine as T;
            base.Setup(machine);
            if (!isRootState)
                currentSuperState = machine.CurrentState;
        }

        /// <summary> Tries to set current state. </summary>
        protected void TrySetState<T1>() where T1 : StateBase, new()
        {
            machine.TrySetState<T1>();
        }

        /// <summary>
        /// Tries to set current substate.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        protected void TrySetSubState<T1>() where T1 : StateBase, new()
        {
            T1 state = machine.factory.GetState<T1>();
            if (state == currentSubState) return;
            if (state.CanEnterState()) SetSubState(state);
        }

        /// <summary>
        /// Can set the substate of a super state from a substate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        protected void TrySetSubState<T1>(StateBase state) where T1 : StateBase, new()
        {
            state = machine.factory.GetState<T1>();
            if (state == currentSubState) return;
            if (state.CanEnterState()) SetSubState(state);
        }

        /// <summary>
        /// Sets the substate
        /// </summary>
        /// <param name="newState"></param>
        protected void SetSubState(StateBase newState)
        {
            // Check if a current substate exists and if it can be exited.
            if (currentSubState != null && !currentSubState.CanExit())
            {
                return;
            }

            // Exit the current substate, if it exists.
            currentSubState?.OnExitState();

            // Set and enter the new substate.
            currentSubState = newState;
            if (currentSubState != null)
            {
                currentSubState.OnEnterState();
                currentSubState.SetSuperState(this);
            }
        }
    }
}

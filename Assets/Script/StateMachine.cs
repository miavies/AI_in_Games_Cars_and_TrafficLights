using UnityEngine;

//State machine is resposible for:
// - Storing current state
// - Switching states (Exit --> Enter)
// - Updating current state
public class StateMachine 
{
    // Holds active state
    // {get private set} --> only this class can change
    public ICarState CurrentCarState { get; private set; }

    // Change() switches the active state to "next"
    public void Change(ICarState next)
    {
        // avoid crashing if next is null, avoid repeating calls
        if(next == null || next == CurrentCarState) return;

        // if there is a current state, call Exit() before switching
        //?. means "call only if not null"
        CurrentCarState?.Exit();

        // set the new state
        CurrentCarState = next;

        // call enter once new state begins
        CurrentCarState.Enter();
    }

    public void Tick()
    {
        CurrentCarState?.Tick();
    }
}

using UnityEngine;

// Any state (Stop/Go/Slowdown) must have these 3 functions
public interface ICarState
{
    void Enter();
    void Tick();
    void Exit();
}

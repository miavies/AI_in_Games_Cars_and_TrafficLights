using UnityEngine;

//0 speed
// stay stop if red light
// switch to slow down or go
public class CarStopState : ICarState
{
    private readonly CarAI car;
    private readonly StateMachine sm;

    // Constructor receives context and machine

    public CarStopState(CarAI car, StateMachine sm)
    {
        this.car = car;
        this.sm = sm;
    }
    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Tick()
    {
        car.SetTargetSpeed(0);

        //light is only considered in traffic light is not null
        bool red = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsRed;

        //if red light or front car is stopped, keep stopping
        if (red || car.CarAheadStoppedClose) return;

        //if orange light while near intersection
        bool orange = car.ActiveTrafficLight !=null && car.ActiveTrafficLight.IsOrange;
        //if orange || there is car ahead, use slow down
        if (orange || car.CarAheadDetected) { sm.Change(car.SlowDownState); }
        //otherwise safe to go
        else { sm.Change(car.GoState); }
    }
}

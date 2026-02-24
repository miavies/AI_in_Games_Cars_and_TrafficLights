using UnityEngine;

public class CarGoState : ICarState
{
    private readonly CarAI car;
    private readonly StateMachine sm;

    // Constructor receives context and machine

    public CarGoState(CarAI car, StateMachine sm)
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
        //if red light
        bool red = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsRed;
        if (red || car.CarAheadStoppedClose)
        {
            sm.Change(car.StopState);
            return;
        }

        //if orange light
        bool orange = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsOrange;
        if (orange || car.CarAheadDetected)
        {
            sm.Change(car.SlowDownState);
            return;
        }
        car.SetTargetSpeed(car.goSpeed);
    }
}

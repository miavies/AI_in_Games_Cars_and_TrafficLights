using UnityEngine;

public class CarSlowdownState : ICarState
{
    private readonly CarAI car;
    private readonly StateMachine sm;

    // Constructor receives context and machine

    public CarSlowdownState(CarAI car, StateMachine sm)
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

        //maintain slower speed
        car.SetTargetSpeed(car.slowSpeed);

        //if green light
        bool green = car.ActiveTrafficLight != null && car.ActiveTrafficLight.IsGreen;
        if (green  && !car.CarAheadDetected)
        {
            sm.Change(car.GoState);
            return;
        }
    }
}

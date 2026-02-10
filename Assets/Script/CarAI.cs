using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Speeds")]
    public float goSpeed;
    public float slowSpeed;
    public float acceleration;
    public float brake;

    [Header("Sensors")]
    public float frontCheckingDistance;
    public float stopDistance;
    public LayerMask carLayer;
   
    public TrafficLight ActiveTrafficLight { get; private set; }
    public float currentSpeed { get; private set; }
    
    //means the Raycast saw a car in front (using raycast)
    public bool CarAheadDetected { get; private set; }

    // a car is detected in front AND car is stopped AND within stopping distance
    public bool CarAheadStoppedClose { get; private set; }
    private StateMachine sm;

    public CarStopState StopState { get; private set; }
    public CarGoState GoState { get; private set; }
    public CarSlowdownState SlowDownState { get; private set; }

    private void Awake()
    {
        sm = new StateMachine();
        StopState = new CarStopState(this, sm);
        GoState = new CarGoState(this, sm);
        SlowDownState = new CarSlowdownState(this, sm); 
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sm.Change(StopState);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSensor();
        MoveForward();
        sm.Tick();
    }

    void MoveForward()
    {
        transform.position += transform.forward * (currentSpeed * Time.deltaTime);
    }

    void UpdateSensor()
    {
        CarAheadDetected = false;
        CarAheadStoppedClose = false;
        Vector3 dir = Vector3.forward;
        Debug.DrawRay(transform.position, dir * frontCheckingDistance);
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, frontCheckingDistance, carLayer))
        {
            //raycast hit car layer
            CarAheadDetected = true;

            //Try to get other car AI
            CarAI other = hit.collider.GetComponent<CarAI>();

            //if other exit, read speed. Else treat it as 0
            float otherSpeed = other != null ? other.currentSpeed : 0;

            //Consider other car stopped if speed is almost zero
            bool otherStopped = otherSpeed <= 0.1f;

            //Consider "very close"
            bool veryclose = hit.distance <= stopDistance;

            //if the other car is stopped and close, we must stop too.
            CarAheadStoppedClose = otherStopped && veryclose;
        }
    }

    //smoothly changes  Current speed towards target speed
    public void SetTargetSpeed(float target)
    {
        //if target is higher than current --> acce;
        //if target is lower than current --> brake;
        float rate = (target > currentSpeed) ? acceleration: brake;

        currentSpeed = Mathf.MoveTowards(currentSpeed, target, rate * Time.deltaTime);
    }

    public void SetActiveTrafficLight(TrafficLight light)
    {
        ActiveTrafficLight = light;
    }

    //Called when exits intersection
    public void ClearActiveTrafficLight(TrafficLight light)
    {
        if(ActiveTrafficLight == light)
            ActiveTrafficLight = null;
    }
}

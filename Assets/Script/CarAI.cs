using UnityEngine;
using UnityEngine.AI;

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
    public Transform sensorOrigin;


    [Header("AI")]
    private NavMeshAgent agent;
    public Transform[] waypoints;
    public bool loop = false;
    public float waypointDistanceReached; // how close the agent is to the waypoint
    int wpIndex;

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
        agent = GetComponent<NavMeshAgent>();
        sm = new StateMachine();
        StopState = new CarStopState(this, sm);
        GoState = new CarGoState(this, sm);
        SlowDownState = new CarSlowdownState(this, sm); 
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sm.Change(StopState);

        //checker if we added waypoints or if they exist
        if (waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
            agent.SetDestination(waypoints[0].position); // tells the navmesh to go to this destination
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSensor();
        //MoveForward();
        DriveRoute();
        sm.Tick();
    }

    //void MoveForward()
    //{
    //    transform.position += transform.forward * (currentSpeed * Time.deltaTime);
    //}

    void DriveRoute()
    {
        //if we have no waypoint do nothing
        if (waypoints == null || waypoints.Length == 0) return;
        ApplyAgentSpeed();
        //if agent is still not calculating a path and we're close enough to our destination point
        if (!agent.pathPending && agent.remainingDistance <= waypointDistanceReached)
        {
            wpIndex++;

            if (wpIndex >= waypoints.Length)
            {
                //if we passed the last waypoint index and looping is enabled
                if (loop) wpIndex = 0;
                else return; // if not enabled stop rerouting
            }

            //if the current waypoint reference is not null
            if (waypoints[wpIndex] != null)
            {
                agent.SetDestination(waypoints[wpIndex].position);
            }
        }
    }

    void ApplyAgentSpeed()
    {
        if(currentSpeed <= 0.01f)
        {
            agent.speed = 0; // makes the speed of agent = 0
            agent.isStopped = true; //stops agent movement
            return;
        }
        agent.isStopped = false; //make the agent move again
        agent.speed = currentSpeed; //set agent speed to current speed
        agent.acceleration = Mathf.Max(agent.acceleration, acceleration);  //set agent speed to current acceleration
    }

    void UpdateSensor()
    {
        Transform origin = sensorOrigin != null ? sensorOrigin : transform;
        Vector3 dir = origin.forward;

        CarAheadDetected = false;
        CarAheadStoppedClose = false;
        Debug.DrawRay(origin.position, dir * frontCheckingDistance);
        if (Physics.Raycast(origin.position, dir, out RaycastHit hit, frontCheckingDistance, carLayer))
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

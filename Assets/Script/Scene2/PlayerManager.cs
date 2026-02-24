using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform enemyPos;
    public float detectionRadius;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        detectionRadius = Vector3.Distance(transform.position, enemyPos.position);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrolling,
        Alerted,
        Chasing
    }

    public Transform player;
    public float normalSpeed = 2f;
    public float alertSpeed = 4f;
    public float chaseSpeed = 6f;
    public float detectionRadius = 10f;
    public float patrolRange = 20f;
    public float playerChaseTime = 5f;
    public State currentState = State.Patrolling;
    
    private NavMeshAgent navMeshAgent;
    private bool playerIsLoud = false;
    private float chaseTimer = 0f;
    private Vector3 lastNoisePosition;
    private Vector3 lastPatrolPosition;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        MoveToRandomPosition();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;

            case State.Alerted:
                MoveToNoisePosition();
                break;

            case State.Chasing:
                ChasePlayer();
                break;
        }

        if (currentState == State.Chasing)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer > playerChaseTime || Vector3.Distance(transform.position, player.position) > detectionRadius * 1.5f)
            {
                currentState = State.Patrolling;
                chaseTimer = 0f;
            }
        }
        else if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            currentState = State.Chasing;
        }
    }

    public void SetPlayerLoud(bool isLoud)
    {
        playerIsLoud = isLoud;
        if (isLoud)
        {
            lastNoisePosition = player.position;
            currentState = State.Alerted;
        }
    }

    private void Patrol()
    {
        navMeshAgent.speed = normalSpeed;
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(LookAround());
            MoveToRandomPosition();
        }
    }

    private void MoveToNoisePosition()
    {
        navMeshAgent.speed = alertSpeed;
        navMeshAgent.SetDestination(lastNoisePosition);

        if (Vector3.Distance(transform.position, lastNoisePosition) < 1f)
        {
            playerIsLoud = false;
            currentState = State.Patrolling;
        }
    }

    private void ChasePlayer()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(player.position);
    }

    private void MoveToRandomPosition()
    {
        Vector3 randomDirection;
        Vector3 finalPosition;
        do
        {
            randomDirection = Random.insideUnitSphere * patrolRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, patrolRange, 1);
            finalPosition = hit.position;
        } while (Vector3.Distance(finalPosition, lastPatrolPosition) < patrolRange / 2);

        lastPatrolPosition = finalPosition;
        navMeshAgent.SetDestination(finalPosition);
    }

    private IEnumerator LookAround()
    {
        navMeshAgent.isStopped = true;

        float lookDuration = 3f;
        float lookTimer = 0f;

        while (lookTimer < lookDuration)
        {
            transform.Rotate(Vector3.up, 120 * Time.deltaTime);
            lookTimer += Time.deltaTime;
            yield return null;
        }

        navMeshAgent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        if (currentState == State.Alerted)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastNoisePosition, 1f);
        }

        if (currentState == State.Chasing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}

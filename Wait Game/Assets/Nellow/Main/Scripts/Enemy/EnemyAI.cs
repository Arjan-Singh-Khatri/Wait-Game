using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrolling,
        Alerted,
        Chasing,
        Looking,
        Stalking,
        Intimidating
    }

    public Transform player;
    public LayerMask obstacleLayer;
    public LayerMask lightLayer;
    public float normalSpeed = 2f;
    public float alertSpeed = 4f;
    public float chaseSpeed = 6f;
    public float detectionRadius = 10f;
    public float patrolRange = 20f;
    public float playerChaseTime = 5f;
    public float alertDuration = 3f;
    public float stalkingDistance = 15f;
    public float followDetectionTime = 3f;
    public State currentState = State.Patrolling;
    public EnemyAnimation enemyAnimation;
    public bool scaredOfLight = true;
    private NavMeshAgent navMeshAgent;
    private bool playerIsLoud = false;
    private float chaseTimer = 0f;
    private float alertTimer = 0f;
    private float followTimer = 0f;
    private Vector3 lastNoisePosition;
    private Vector3 lastPatrolPosition;
    private bool playerSeen = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        MoveToRandomPosition();
    }

    void Update()
    {
        HandleState();

        if (currentState == State.Chasing)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer > playerChaseTime || Vector3.Distance(transform.position, player.position) > detectionRadius * 1.5f)
            {
                TransitionToState(State.Patrolling);
                chaseTimer = 0f;
                MoveToRandomPosition();
            }
        }
        else if (currentState != State.Stalking && Vector3.Distance(transform.position, player.position) < detectionRadius && HasLineOfSight())
        {
            TransitionToState(State.Chasing);
        }

        if (currentState == State.Stalking)
        {
            if (IsPlayerSeeingEnemy())
            {
                SprintAndHide();
            }
            else if (IsPlayerFollowing())
            {
                TransitionToState(State.Intimidating);
            }
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Alerted:
                Alert();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Looking:
                enemyAnimation.UpdateAnimationState(0);
                break;
            case State.Stalking:
                StalkPlayer();
                break;
            case State.Intimidating:
                IntimidatePlayer();
                break;
        }
    }

    private void TransitionToState(State newState)
    {
        if (newState == State.Chasing || currentState != State.Chasing)
        {
            currentState = newState;
        }
        else if (newState == State.Alerted && currentState != State.Chasing && currentState != State.Alerted)
        {
            currentState = newState;
        }
        else if (newState == State.Patrolling && currentState == State.Looking)
        {
            currentState = newState;
        }
        else if (newState == State.Stalking && currentState != State.Chasing)
        {
            currentState = newState;
        }

        if (currentState == State.Looking)
        {
            StopCoroutine(LookAround());
            enemyAnimation.PlayLookAroundAnimation(false);
            navMeshAgent.isStopped = false;
        }
    }

    public void SetPlayerLoud(bool isLoud)
    {
        playerIsLoud = isLoud;
        if (isLoud)
        {
            lastNoisePosition = player.position;
            TransitionToState(State.Alerted);
            alertTimer = 0f;
        }
    }

    public void SetPlayerQuiet(bool isQuiet)
    {
        if (isQuiet && currentState != State.Stalking && currentState != State.Chasing)
        {
            TransitionToState(State.Stalking);
        }
    }

    private void Patrol()
    {
        navMeshAgent.speed = normalSpeed;
        enemyAnimation.UpdateAnimationState(navMeshAgent.velocity.magnitude);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(LookAround());
        }
    }

    private void Alert()
    {
        navMeshAgent.speed = alertSpeed;
        enemyAnimation.UpdateAnimationState(navMeshAgent.velocity.magnitude);
        navMeshAgent.SetDestination(lastNoisePosition);

        alertTimer += Time.deltaTime;
        if (alertTimer >= alertDuration || navMeshAgent.remainingDistance <= 1f)
        {
            StartCoroutine(LookAround());
        }
    }

    private void ChasePlayer()
    {
        navMeshAgent.speed = chaseSpeed;
        Vector3 destination = player.position;
        if (scaredOfLight && IsLightSourceNearby(destination))
        {
            destination = GetNearestSafePosition();
        }
        navMeshAgent.SetDestination(destination);
        enemyAnimation.UpdateAnimationState(navMeshAgent.velocity.magnitude);
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
        currentState = State.Looking;
        navMeshAgent.isStopped = true;

        float lookDuration = 3f;
        float lookTimer = 0f;
        enemyAnimation.PlayLookAroundAnimation(true);

        while (lookTimer < lookDuration)
        {
            if (currentState != State.Looking)
            {
                enemyAnimation.PlayLookAroundAnimation(false);
                navMeshAgent.isStopped = false;
                yield break;
            }

            if (Vector3.Distance(transform.position, player.position) < detectionRadius && HasLineOfSight())
            {
                TransitionToState(State.Chasing);
                navMeshAgent.isStopped = false;
                enemyAnimation.PlayLookAroundAnimation(false);
                yield break;
            }

            lookTimer += Time.deltaTime;
            yield return null;
        }

        enemyAnimation.PlayLookAroundAnimation(false);
        navMeshAgent.isStopped = false;
        TransitionToState(State.Patrolling);
        MoveToRandomPosition();
    }

    private void StalkPlayer()
    {
        navMeshAgent.speed = normalSpeed;
        Vector3 directionToPlayer = player.position - transform.position;
        float desiredDistance = stalkingDistance;
        Vector3 stalkingPosition = player.position - directionToPlayer.normalized * desiredDistance;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, desiredDistance, obstacleLayer))
        {
            Vector3 hidingSpot = hit.point + hit.normal * 1.5f;
            if (NavMesh.SamplePosition(hidingSpot, out NavMeshHit navHit, 1.5f, NavMesh.AllAreas))
            {
                stalkingPosition = navHit.position;
            }
        }

        if (scaredOfLight && IsLightSourceNearby(stalkingPosition))
        {
            stalkingPosition = GetNearestSafePosition();
        }

        navMeshAgent.SetDestination(stalkingPosition);
        enemyAnimation.UpdateAnimationState(navMeshAgent.velocity.magnitude);

        if (Vector3.Distance(transform.position, player.position) < detectionRadius * 0.3f && HasLineOfSight())
        {
            TransitionToState(State.Chasing);
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        if (Physics.Linecast(transform.position, player.position, out RaycastHit hit, obstacleLayer))
        {
            return hit.transform == player;
        }
        return true;
    }

    private bool IsPlayerSeeingEnemy()
    {
        Vector3 toPlayer = player.position - transform.position;
        float angle = Vector3.Angle(player.forward, toPlayer);
        if (angle < 90f && Vector3.Distance(player.position, transform.position) < detectionRadius && HasLineOfSight())
        {
            return true;
        }
        return false;
    }

    private bool IsPlayerFollowing()
    {
        if (Vector3.Distance(transform.position, player.position) < stalkingDistance && HasLineOfSight())
        {
            followTimer += Time.deltaTime;
            if (followTimer >= followDetectionTime)
            {
                followTimer = 0f;
                return true;
            }
        }
        else
        {
            followTimer = 0f;
        }
        return false;
    }

    private void SprintAndHide()
    {
        navMeshAgent.speed = chaseSpeed;
        Vector3 hidingSpot = Vector3.zero;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out RaycastHit hit, detectionRadius, obstacleLayer))
        {
            hidingSpot = hit.point + hit.normal * 2f;
            if (NavMesh.SamplePosition(hidingSpot, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                hidingSpot = navHit.position;
            }
        }

        if (hidingSpot != Vector3.zero)
        {
            navMeshAgent.SetDestination(hidingSpot);
            currentState = State.Stalking;
            playerSeen = true;
        }
    }

    private void IntimidatePlayer()
    {
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(player.position);
        enemyAnimation.UpdateAnimationState(navMeshAgent.velocity.magnitude);

        if (Vector3.Distance(transform.position, player.position) > detectionRadius * 1.5f)
        {
            TransitionToState(State.Patrolling);
        }
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

        if (currentState == State.Stalking)
        {
            Gizmos.color = Color.magenta;
            if (navMeshAgent != null)
            {
                Gizmos.DrawLine(transform.position, navMeshAgent.destination);
            }
            Gizmos.DrawWireSphere(transform.position, stalkingDistance);
        }
    }


    private bool IsLightSourceNearby(Vector3 position)
    {
        Collider[] lightSources = Physics.OverlapSphere(position, detectionRadius, lightLayer);
        return lightSources.Length > 0;
    }

    private Vector3 GetNearestSafePosition()
    {
        Vector3 safePosition = transform.position;
        float maxDistance = detectionRadius * 2f;
        for (int i = 0; i < 36; i++)
        {
            Vector3 direction = Quaternion.Euler(0, i * 10, 0) * Vector3.forward;
            Vector3 potentialPosition = transform.position + direction * maxDistance;
            if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
            {
                if (!IsLightSourceNearby(hit.position))
                {
                    safePosition = hit.position;
                    break;
                }
            }
        }
        return safePosition;
    }
}

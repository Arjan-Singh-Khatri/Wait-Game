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
		Looking
	}

	public Transform player;
	public LayerMask obstacleLayer;
	public float normalSpeed = 2f;
	public float alertSpeed = 4f;
	public float chaseSpeed = 6f;
	public float detectionRadius = 10f;
	public float patrolRange = 20f;
	public float playerChaseTime = 5f;
	public float alertDuration = 3f;
	public State currentState = State.Patrolling;
	public EnemyAnimation enemyAnimation;
	private NavMeshAgent navMeshAgent;
	private bool playerIsLoud = false;
	private float chaseTimer = 0f;
	private float alertTimer = 0f;
	private Vector3 lastNoisePosition;
	private Vector3 lastPatrolPosition;

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
		else if (Vector3.Distance(transform.position, player.position) < detectionRadius && HasLineOfSight())
		{
			TransitionToState(State.Chasing);
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
		navMeshAgent.SetDestination(player.position);
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

	private bool HasLineOfSight()
	{
		Vector3 directionToPlayer = player.position - transform.position;
		if (Physics.Linecast(transform.position, player.position, out RaycastHit hit, obstacleLayer))
		{
			return hit.transform == player;
		}
		return true;
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

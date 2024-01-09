using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public float speedValue;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100f;

    // Patrolling
    public Vector3 walkPoint;
    public Vector3 distanceToWalkPoint;
    public bool walkPointSet;
    public float walkPointRange = 25f;
    public float idleTime = 0f;

    // Attacking
    public float attackDelay = 0f; // Previously 3 but delay is now implemented by animation event to sync w/ anims
    public bool alreadyAttacked;

    // States
    public float sightRange = 75f;
    public float sightAngle = 270f;
    public float attackRange = 2f;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Animations")]
    public Animator animator;

    [Header("Audio")]
    public AudioClip[] zombieMoansSFX;
    public AudioClip[] zombieAttacksSFX;
    public float playSFXDelay;

    private void Awake()
    {
       player = GameObject.Find("FirstPersonPlayer").transform;
       agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.speed = speedValue;
    }

    void Update()
    {
        // Check for sight and attack range
        Vector3 playerDirection = (player.position - transform.position).normalized;
        RaycastHit hit = new RaycastHit();
        if (Vector3.Angle(transform.forward, playerDirection) <= sightAngle * 0.5f) // Check if player is in the viewcone 
        {
            Physics.Raycast(transform.position, playerDirection, out hit, sightRange); // Cast ray from enemy to player (regardless of whether there is an obstacle between them)
        }

        playerInSightRange = hit.collider != null && hit.collider.gameObject.name == "FirstPersonPlayer"; // Check if the first hit object is the player (meaning there is a clear line of sight betwen the enemy and player)
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

        if (Input.GetMouseButtonDown(1))
        {
            Ray movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(movePosition, out var hitInfo))
            {
                agent.SetDestination(hitInfo.point);
			}
		}

        playSFXDelay -= Time.deltaTime;
    }

    private void Patrolling()
    {
        if (playSFXDelay <= 0)
        {
            SoundFXManager.instance.PlayRandomSoundFXClip(zombieMoansSFX, transform, 1f);
            playSFXDelay = Random.Range(5f, 10f);
        }

        if (!walkPointSet)
        {
            agent.speed = 3f;
            if (idleTime <= 0)
            {
                idleTime = 3f;
                SearchWalkPoint();
            } 
            else
            {
                animator.SetInteger("movementState", 0);
                idleTime -= Time.deltaTime;
            }
        }

        if (walkPointSet)
        {
            if (animator.GetInteger("movementState") != 2) animator.SetInteger("movementState", 1);
            agent.SetDestination(walkPoint);
        }
        // Vector3 distanceToWalkPoint = transform.position - walkPoint;
        distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint Reached
        if (distanceToWalkPoint.magnitude <= 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {

        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
    public void ChasePlayer()
    {
        if (playSFXDelay <= 0)
        {
            SoundFXManager.instance.PlayRandomSoundFXClip(zombieMoansSFX, transform, 1f);
            playSFXDelay = Random.Range(2f, 5f);
        }

        animator.SetInteger("movementState", 2);
        agent.speed = 4f;
        agent.SetDestination(player.position);
        walkPoint = player.position;
        walkPointSet = true;
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        animator.SetInteger("movementState", 3);
    }

    private void PerformAttack()
    {
        SoundFXManager.instance.PlayRandomSoundFXClip(zombieAttacksSFX, transform, 1f);
        if (!alreadyAttacked)
        {
            if (Physics.CheckSphere(transform.position, attackRange, whatIsPlayer)) GameObject.Find("FirstPersonPlayer").GetComponent<Health>().takeDamage();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackDelay);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), .5f);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        // Draw the sphere at the origin
        Gizmos.DrawWireSphere(transform.position, 1f);

        // Draw the sphere cast
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);
        Gizmos.DrawWireSphere(transform.position + transform.forward * 3f, 1f);
    }
}

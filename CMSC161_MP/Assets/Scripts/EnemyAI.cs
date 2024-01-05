using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public float speedValue = 10f;

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
    public float attackDelay = 3f;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange = 75f;
    public float sightAngle = 270f;
    public float attackRange = 3f;
    public bool playerInSightRange, playerInAttackRange;


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
    }

    private void Patrolling()
    {
        // if (!walkPointSet) SearchWalkPoint();
        if (!walkPointSet)
        {
            if (idleTime <= 0)
            {
                idleTime = 3f;
                SearchWalkPoint();
            } 
            else
            {
                idleTime -= Time.deltaTime;
            }
        }

        if (walkPointSet)
        {
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
        agent.SetDestination(player.position);
        walkPoint = player.position;
        walkPointSet = true;
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Attack code here
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, FoßßrceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            RaycastHit hit = new RaycastHit();
            Physics.Raycast(transform.position, transform.forward, out hit, attackRange);
            if (hit.collider != null && hit.collider.gameObject.name == "FirstPersonPlayer")
            {
                hit.collider.gameObject.GetComponent<PlayerHealth>().takeDamage();
			}



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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw view funnel
        float halfFOV = sightAngle * 0.5f;
        Vector3 leftRayDirection = Quaternion.Euler(0, -halfFOV, 0) * transform.forward;
        Vector3 rightRayDirection = Quaternion.Euler(0, halfFOV, 0) * transform.forward;

        Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.5f); // Light blue with transparency

        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * sightRange);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * sightRange);
    }
}

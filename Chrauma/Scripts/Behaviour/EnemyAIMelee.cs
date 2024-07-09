/*
 * ======================================================================================
 *                                 Enemy Script
 * ======================================================================================
 * This script manages the behavior of enemy characters in Unity. It handles patrolling,
 * chasing the player, attacking, and taking damage. It also manages enemy health and
 * handles their death, including dropping life items.
 *
 * Key Features:
 * - Patrolling behavior within a specified range.
 * - Detection and chasing of the player within sight and attack ranges.
 * - Handling of attack logic and cooldowns.
 * - Managing enemy health, taking damage, and death.
 * - Integration with object pooling for efficient enemy management.
 * ======================================================================================
 */

using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Pool;

// * Debug
using TMPro;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float startingHealth;
    public float currentHealth;
    public GameObject lifeDropPrefab;

    // * Patrol settings
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // * Attack settings
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // * Detection ranges
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    EnemyAnimator enemyAnimator;
    GameObject _LifeDropTarget;
    
    // * Debug
    [SerializeField] TMP_Text hptext;
    [SerializeField] TMP_Text hptext2;
    public bool debugHP = false;
    [SerializeField] ParticleSystem bloodSplatter;
    [SerializeField] GameObject deathSmoke;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponentInChildren<EnemyAnimator>();

        // * Set current hp when spawned
        currentHealth = startingHealth;
    }

    private void Start()
    {
        // * Find player and life drop target objects
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _LifeDropTarget = GameObject.FindGameObjectWithTag("LifeDropTarget");
        if (debugHP) DebugHP(0);
    }

    private void Update()
    {
        // * Check if player is in sight or attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // * Execute behavior based on detection ranges
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange && !alreadyAttacked) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        // * Set a random walk point if not already set
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        // * Check if close enough to the walk point
        if (distanceToWalkPoint.magnitude < 1)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // * Generate a random walk point within the range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // * Check if the point is on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        // * Chase the player
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // * Stop moving and attack the player
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            // * Trigger attack animation
            enemyAnimator.startAttackAnimation();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        // * Reset attack cooldown
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        // * Reduce health and play blood splatter effect
        currentHealth -= damage;
        if (debugHP) Debug.Log(gameObject.name + " hp:" + currentHealth);
        bloodSplatter.Play();
        if (debugHP) DebugHP(damage);
        if (currentHealth <= 0) EnemyDies();
    }

    private IObjectPool<Enemy> enemyPool;

    public void SetPool(IObjectPool<Enemy> pool)
    {
        // * Set the object pool for this enemy
        enemyPool = pool;
    }

    private void EnemyDies()
    {
        // * Drop life items and instantiate death effects
        for (int i = 0; i < startingHealth / 10; i++)
        {
            var go = Instantiate(lifeDropPrefab, transform.position + new Vector3(0, Random.Range(0, 2)), Quaternion.identity);
            var goscript = go.GetComponent<FollowLifeDrop>();
            goscript.Target = _LifeDropTarget.transform;
            goscript.StartFollowing();
        }
        Instantiate(deathSmoke, transform.position, Quaternion.identity);
        if (enemyPool != null) enemyPool.Release(this);
    }

    private void OnDrawGizmosSelected_()
    {
        // * Draw detection range gizmos in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void DebugHP(int damage)
    {
        // * Update debug HP text
        hptext.text = "" + currentHealth;
        hptext2.text = "" + damage;
    }
}

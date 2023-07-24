using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string attackTargetTag = "Castelo";
    public Transform attackTarget;
    public float attackRange = 2f;
    public float attackInterval = 2f;
    public int attackDamage = 10;
    public int maxHealth = 100;
    public float movementSpeed = 3f; // Velocidade de movimento do inimigo
    public bool isDead = false;

    private NavMeshAgent navMeshAgent;
    private bool isAttacking;
    private float attackTimer;
    private int currentHealth;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        isAttacking = false;
        attackTimer = attackInterval;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        navMeshAgent.speed = movementSpeed; // Configura a velocidade de movimento do inimigo.
        GameObject targetObject = GameObject.FindGameObjectWithTag(attackTargetTag);
        if (targetObject != null)
        {
            attackTarget = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("Nenhum objeto encontrado com a tag " + attackTargetTag);
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackInterval;
            }
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
        if (distanceToTarget <= attackRange)
        {
            isAttacking = true;
            navMeshAgent.isStopped = true;
            Attack();
        }
        else
        {
            navMeshAgent.isStopped = false; // Garante que o NavMeshAgent esteja habilitado para se mover.
            navMeshAgent.SetDestination(attackTarget.position);
        }
    }

    private void Attack()
    {
        Debug.Log("Inimigo atacando!");

        // Lógica de ataque aqui (por exemplo, causar dano ao jogador).
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Chamar a animação de dano (se houver) ou executar outras ações em resposta ao dano.

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo morreu!");

        isDead = true;
        animator.SetBool("Death", true);

        currentHealth = 0;

        // Aqui você pode adicionar outras ações que devem ser realizadas quando o inimigo morre, como remover o GameObject do inimigo do cenário, etc.
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public string attackTargetTag = "Castelo";
    public Transform attackTarget;
    public float attackRange = 2f;
    public float attackInterval = 2f;
    public int attackDamage = 10;
    public int maxHealth = 20;
    public float movementSpeed = 3f; // Velocidade de movimento do inimigo
    public bool isDead = false;

    public WaveManager waveManager;
    public GameManager gameManager;

    private NavMeshAgent navMeshAgent;
    private bool isAttacking;
    private float attackTimer;
    private int currentHealth;
    private Animator animator;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        isAttacking = false;
        attackTimer = attackInterval;
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        waveManager = FindObjectOfType<WaveManager>();
        

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
        // Verifica se o inimigo está morto
        if (isDead)
        {
            HandleDeath();
        }
        else if (isAttacking)
        {
            // Verifica se o inimigo está atacando
            HandleAttack();
        }
        else
        {
            // Comportamento de movimento normal se o inimigo estiver vivo e não estiver atacando
            HandleMovement();
        }
    }

    private void HandleDeath()
    {
        // Adiciona um Debug.Log indicando que o inimigo morreu.
        Debug.Log("Inimigo morreu!");

        // Se o inimigo estiver morto, pare de se mover.
        navMeshAgent.isStopped = true;
    }

    private void HandleAttack()
    {
        // Reduz o tempo de intervalo entre ataques
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            // Realiza o ataque e reinicia o temporizador de ataque
            Attack();
            attackTimer = attackInterval;
        }
    }

    private void HandleMovement()
    {
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
        if (distanceToTarget <= attackRange)
        {
            // O inimigo está na distância de ataque, então começa a atacar
            isAttacking = true;
            navMeshAgent.isStopped = true;
        }
        else
        {
            // O inimigo não está na distância de ataque, continua se movendo em direção ao alvo
            navMeshAgent.isStopped = false; // Garante que o NavMeshAgent esteja habilitado para se mover.
            navMeshAgent.SetDestination(attackTarget.position);
        }
    }



    private void Attack()
    {
        Debug.Log("Inimigo atacando!");
        gameManager.DamageCastle(10);
        Destroy(gameObject);
        waveManager.currentWaveEnemiesAlive--;

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
        waveManager.currentWaveEnemiesAlive--;

        isDead = true;
        gameManager.EarnMoney(40); //colocando dinheiro para o player
        animator.SetBool("Death", true);

        currentHealth = 0;
        gameObject.tag = "InimigoDeath";
        StartCoroutine(DestruirEnemy());
        // Aqui você pode adicionar outras ações que devem ser realizadas quando o inimigo morre, como remover o GameObject do inimigo do cenário, etc.
    }

    private IEnumerator DestruirEnemy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}

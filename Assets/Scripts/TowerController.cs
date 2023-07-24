using UnityEngine;

public class TowerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    public float shootingInterval = 2f;
    public int damage = 20;
    public float rotationSpeed = 5f;
    public float attackRange = 10f; // Alcance de ataque da torre

    private Transform targetEnemy;
    private float shootingTimer = 0f;

    private void Update()
    {
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            // Verificar se o inimigo está dentro do alcance de ataque
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);
            if (distanceToEnemy <= attackRange)
            {
                RotateTowardsEnemy();

                shootingTimer += Time.deltaTime;
                if (shootingTimer >= shootingInterval)
                {
                    shootingTimer = 0f;
                    Shoot();
                }
            }
        }
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Inimigo");

        if (enemies.Length == 0)
        {
            targetEnemy = null;
            return;
        }

        float closestDistance = Vector3.Distance(transform.position, enemies[0].transform.position);
        targetEnemy = enemies[0].transform;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                targetEnemy = enemy.transform;
            }
        }
    }

    private void RotateTowardsEnemy()
    {
        Vector3 targetDirection = targetEnemy.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Suavizar a rotação para evitar que a torre fique tremendo
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (bulletController != null)
        {
            bulletController.damage = damage;
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Desenhar um Gizmo para representar visualmente o alcance de ataque da torre no editor.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

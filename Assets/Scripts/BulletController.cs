using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 40f;
    public int damage = 20;
    public float lifetime = 3f; // Tempo de vida da bala antes de ser destruída automaticamente

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); // Destruir a bala automaticamente após um tempo de vida.
    }

    private void FixedUpdate()
    {
        // Movimentar a bala usando AddForce no FixedUpdate para melhorar a consistência física.
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inimigo"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
    
}

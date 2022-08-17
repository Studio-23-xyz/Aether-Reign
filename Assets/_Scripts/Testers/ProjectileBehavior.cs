using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float ProjectileDamage;
    public float ProjectileSpeed;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.forward * ProjectileSpeed, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with {other.name}");
        if (other.CompareTag(StringResources.EnemyTag))
            return;
        Destroy(gameObject);
        other.GetComponent<IDamageable>()?.TakeDamage(ProjectileDamage);
        GameManager.Instance.EndTurn();
    }
}

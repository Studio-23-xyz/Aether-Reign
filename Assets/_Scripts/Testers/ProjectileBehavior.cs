using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float ProjectileSpeed;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity += Vector3.forward * Time.deltaTime * ProjectileSpeed; 
    }
}

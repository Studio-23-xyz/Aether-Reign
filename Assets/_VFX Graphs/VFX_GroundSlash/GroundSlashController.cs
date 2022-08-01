using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GroundSlashController : MonoBehaviour
{
    private Rigidbody _rb;
    public float ForwardSpeed;
    public float SlowdownRate;
    public float DetectionRange;
    public float TileYOffset;
    
    private bool _hasStopped;

    private void Awake()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            _rb = GetComponent<Rigidbody>();
            SlowDownProjectile();
        }
        else
            Debug.Log($"Rigidbody wher?");
        
        Initialize();
    }

    private void Initialize()
    {
        transform.position = new Vector3(transform.position.x, 0f + TileYOffset, transform.position.z);
        _rb.velocity = transform.forward * ForwardSpeed;
    }

    private void FixedUpdate()
    {
        if (!_hasStopped)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            if (Physics.Raycast(distance, transform.TransformDirection(-Vector3.up), out hit, DetectionRange))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, 0f + TileYOffset, transform.position.z);
            }
        }
    }

    private async void SlowDownProjectile()
    {
        float timer = 1f;
        while (timer > 0f)
        {
            _rb.velocity = Vector3.Lerp(Vector3.zero, _rb.velocity, timer);
            timer -= SlowdownRate;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }

        _hasStopped = true;
    }
}

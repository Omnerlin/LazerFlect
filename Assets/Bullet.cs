using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("How fast this bullet will travel.")]
    public float speed = 5;

    [Tooltip("How long in seconds this bullet will last before being automatically destroyed.")]
    public float lifetime = 8;

    [Tooltip("How many times this bullet will bounce before being destroyed. Use -1 for infinite possible bounces.")]
    public float numBounces;

    private float lifeCounter = 0;
    private Vector3 direction;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        lifeCounter += Time.deltaTime;
        if(lifeCounter >= lifetime)
        {
            Die();
        }
    }

    public void Fire(Vector3 startPosition, Vector3 direction)
    {
        transform.position = startPosition;
        transform.forward = direction;
        this.direction = direction;
        rb.velocity = direction * speed;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.CompareTag("Reflector"))
        {
            if(--numBounces == -1)
            {
                Die();
            }

            direction = Vector3.Reflect(direction, collision.GetContact(0).normal);
            transform.forward = direction;
            rb.velocity = direction * speed;
        }
        else
        {
            Die();
        }
    }
}

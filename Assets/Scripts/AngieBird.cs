using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngieBird : MonoBehaviour
{
    [SerializeField] private AudioClip hitClip;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private AudioSource audioSource;
    
    private bool hasBeenLaunched;
    private bool shouldFaceVelDirection;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        //turn gravity off
        rb.isKinematic = true;
        circleCollider.enabled = false;
    }
    private void FixedUpdate()
    {
        if (hasBeenLaunched && shouldFaceVelDirection)
        {
            transform.right = rb.velocity;
        }
    }
    public void LaunchBird(Vector2 direction, float force)
    {
        //on
        rb.isKinematic=false;
        circleCollider.enabled=true;

        //apply the force
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        hasBeenLaunched = true;
        shouldFaceVelDirection = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        shouldFaceVelDirection=false;
        SoundManager.instance.PlayClip(hitClip, audioSource);
        Destroy(this);
    }
}

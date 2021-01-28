using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxEnemyHealth;
    public float currentEnemyHealth;
    internal bool gotDamage;
    public float damage;
    public float projectileDamage=25;
    public Transform deathParticle;
    SpriteRenderer graph;
    CircleCollider2D circle2D;
    Player player;
    Rigidbody2D body2D;

    //Audio
    AudioSource auSource;
    AudioClip ac_Dead;

    void Start()
    {
        currentEnemyHealth = maxEnemyHealth;
        player = FindObjectOfType<Player>();
        graph = GetComponent<SpriteRenderer>();
        circle2D = GetComponent<CircleCollider2D>();
        body2D = GetComponent<Rigidbody2D>();
        deathParticle = transform.Find("DeathParticle");
        deathParticle.gameObject.SetActive(false);
        auSource = GetComponent<AudioSource>();
        ac_Dead = Resources.Load("SoundEffect/Dead") as AudioClip;
    }

    void Update()
    {
        if (currentEnemyHealth <= 0)
        {
            graph.enabled = false;
            circle2D.enabled = false;
            deathParticle.gameObject.SetActive(true);
            body2D.constraints = RigidbodyConstraints2D.FreezePositionX;
            Destroy(gameObject,1);  
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerItem" && player.canDamage)
        {
            currentEnemyHealth -= damage;
            auSource.PlayOneShot(ac_Dead);
        }

        if (other.tag == "PlayerProjectile")
        {
            currentEnemyHealth -= projectileDamage;
            auSource.PlayOneShot(ac_Dead);
            Destroy(other.gameObject);
        }
    }
}

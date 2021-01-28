using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{

    Rigidbody2D body2D;
    public float knockBackForce;
    BoxCollider2D box2D;
    CircleCollider2D circle2d;
    GameManager gameManager;

    [Tooltip("Karakterin ne kadar hızlı gideceğini belirler")]
    [Range(0, 40)]
    public float playerSpeed;

    [Tooltip("Karakterin ne kadar yükseğe zıplayacağını belirler")]
    public float jumpPower;

    public float doubleJumpPower;
    internal bool canDoubleJump;
    internal bool canDamage;

    [Tooltip("Karakterin yere değip değmeyeceğini kontrol eder")]
    public bool isGrounded;
    Transform groundCheck;
    const float GroundCheckRadius = 0.2f;
    [Tooltip("Yerin ne olduğunu belirler")]
    public LayerMask groundLayer;

    //Karakteri döndürme
    bool facingRight = true;

    //Animator Controller animasyonlarını kontrol eder
    Animator PlayerAnimatorController;

    //Oyuncu canı
    internal int maxPlayerHealth = 100;
    public int currentPlayerHealth;
    internal bool isHurt;

    //Oyuncuyu öldür
    internal bool isDead;
    public float deadForce;

    //Oyuncunun puanları
    internal int currentPoints;
    internal int point = 10;

    //Sound
    AudioSource auSource;
    AudioClip au_Jump;
    AudioClip au_Hurt;
    AudioClip au_Coin;
    AudioClip au_Dead;
    AudioClip au_Shoot;

    //Ateş etme
    Transform firePoint;
    GameObject bullet;
    public float bulletSpeed;

    void Start()
    {
        //Rigibody ayarları
        body2D = GetComponent<Rigidbody2D>();
        body2D.gravityScale = 5;
        body2D.freezeRotation = true;
        body2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        //GroundChecki bul
        groundCheck = transform.Find("GroundCheck");

        //Animator'u bul
        PlayerAnimatorController = GetComponent<Animator>();

        //Canı, max cana eşitle
        currentPlayerHealth = maxPlayerHealth;

        //Collider'ları al
        box2D = GetComponent<BoxCollider2D>();
        circle2d = GetComponent<CircleCollider2D>();

        //GameManager
        gameManager = FindObjectOfType<GameManager>();

        //Sesleri yükleme
        auSource = GetComponent<AudioSource>();
        au_Jump = Resources.Load("SoundEffect/Jump") as AudioClip;
        au_Hurt = Resources.Load("SoundEffect/Hurt") as AudioClip;
        au_Coin = Resources.Load("SoundEffect/PickupCoin") as AudioClip;
        au_Dead = Resources.Load("SoundEffect/Dead") as AudioClip;
        au_Shoot = Resources.Load("SoundEffect/Shoot") as AudioClip;

        //Ateş etme
        firePoint = transform.Find("FirePoint");
        bullet = Resources.Load("Bullet") as GameObject;
    }

    void Update()
    {
        UpdateAnimations();
        ReduceHealth();
        isDead = currentPlayerHealth <= 0;
        if (isDead)
            KillPlayer();

        //Eğer canımız maxCanımızdan yüksekse canımızı maxCana eşitle.
        if (currentPlayerHealth > maxPlayerHealth)
            currentPlayerHealth = maxPlayerHealth;

        if (transform.position.y <= -15)
            isDead = true;
    }

    //Framerate'den bağımsız olarak çalışır. Fizik ile ilgili kodları buraya yazarız
    void FixedUpdate()
    {
        //Yere değiyoruz muyuz diye bak
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, groundLayer);

        //Hareket etme
        float h = Input.GetAxis("Horizontal");
        body2D.velocity = new Vector2(h * playerSpeed, body2D.velocity.y);
        Flip(h);

        if (isGrounded)
            canDamage = false;
    }

    public void Jump()
    {
        //Rigibody'ye dikey yönde(Y) güç ekle
        //body2D.AddForce(new Vector2(0, jumpPower));
        body2D.velocity = new Vector2(0, jumpPower);
        auSource.PlayOneShot(au_Jump);
        auSource.pitch = Random.Range(0.8f, 1.1f);
    }

    public void DoubleJump()
    {
        //Rigibody'ye dikey yönde(Y) ani bir güç ekler
        body2D.AddForce(new Vector2(0, doubleJumpPower), ForceMode2D.Impulse);
        canDamage = true;
        auSource.PlayOneShot(au_Jump);
        auSource.pitch = Random.Range(0.8f, 1.1f);
    }

    //Karakteri döndürme fonksiyonu
    void Flip(float h)
    {
        if (h > 0 && !facingRight || h < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector2 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    void UpdateAnimations()
    {
        PlayerAnimatorController.SetFloat("VelocityX", Mathf.Abs(body2D.velocity.x));
        PlayerAnimatorController.SetBool("isGrounded", isGrounded);
        PlayerAnimatorController.SetFloat("VelocityY", body2D.velocity.y);
        PlayerAnimatorController.SetBool("isDead", isDead);
        if (isHurt && !isDead)
            PlayerAnimatorController.SetTrigger("isHurt");
    }



    //Can azaltma fonksiyonu
    void ReduceHealth()
    {
        if (isHurt)
        {
            /*Eğer canımız         o zaman canımızdan zarar kadar çıkar
               100 ise                     -zarar-
              eğer bu kondisyon doğru ise can-zarar=yeni can  
             */
            isHurt = false;

            //Eğer havadaysak sol veya sağ ve dikey yönde güç uygula
            if (facingRight && !isGrounded)
                body2D.AddForce(new Vector2(-knockBackForce, 1000), ForceMode2D.Force);
            else if (!facingRight && !isGrounded)
                body2D.AddForce(new Vector2(knockBackForce, 1000), ForceMode2D.Force);

            //Eğer yerdeysek sol veya sağ yönde güç uygula
            if (facingRight && isGrounded)
                body2D.AddForce(new Vector2(-knockBackForce, 0), ForceMode2D.Force);
            else if (!facingRight && isGrounded)
                body2D.AddForce(new Vector2(knockBackForce, 0), ForceMode2D.Force);

            if (!isDead)
            {
                auSource.PlayOneShot(au_Hurt);
                auSource.pitch = Random.Range(0.8f, 1.1f);
            }
        }
    }

    //oyuncuyu öldürme fonksiyonu
    void KillPlayer()
    {
        isHurt = false;
        body2D.AddForce(new Vector2(0, deadForce), ForceMode2D.Impulse);
        body2D.drag += Time.deltaTime * 15;
        deadForce -= Time.deltaTime * 20;
        body2D.constraints = RigidbodyConstraints2D.FreezePositionX;
        box2D.enabled = false;
        circle2d.enabled = false;

    }


    public void ShootProjectile()
    {
        GameObject b = Instantiate(bullet) as GameObject;
        b.transform.position = firePoint.transform.position;
        b.transform.rotation = firePoint.transform.rotation;
        b.GetComponent<Rigidbody2D>().AddForce(transform.forward * bulletSpeed, ForceMode2D.Impulse);

        auSource.PlayOneShot(au_Shoot);
        auSource.pitch = Random.Range(0.8f, 1.1f);

        if (transform.localScale.x < 0)
        {
            b.GetComponent<Projectile>().bulletSpeed *= -1;
            b.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            b.GetComponent<Projectile>().bulletSpeed *= 1;
            b.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Coin")
        {
            Transform coinEffect;
            currentPoints += point;
            other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            coinEffect = other.gameObject.transform.Find("CoinEffect");
            coinEffect.gameObject.SetActive(true);
            Destroy(other.gameObject, 1);
            auSource.PlayOneShot(au_Coin);
            auSource.pitch = Random.Range(0.8f, 1.1f);
        }

        if (other.tag == "Enemy" && isDead)
        {
            auSource.PlayOneShot(au_Dead);
        }
    }
}

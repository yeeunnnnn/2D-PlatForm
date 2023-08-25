using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    
    public float maxSpeed;
    public float jumpPower;


    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsulecollider;
    AudioSource audioSource;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();    
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // 꾹 누르는거 -- 지속적인 키 입력 ==>  Fixed Update 에서 하고
    // 단발적인 키입력은  ==> Update 에서 하는게 좋다
    // Why ?? 왜냐 ????? Fixed Updated 는 1초에 50회 정도 돌고 Updated는 실제적으로 도는거 1초에 60번 돈다
    // 단발적인 키입력을 Fixed Updated 에서하면은 키가 씹히는 경우가 있다.!! 손해 보는 10 프레임에서 먹으면 씹히겠군
    // 그래서 단발적인 키입력은 Update에서!!

    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }
            

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2 (rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            //normalized : 벡터 크기를 1로 만든 상태 (단위벡터)
        }

        // Direction Sprite     방향전환
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)           // Mathf : 수학 관련 함수를 제공하는 클래스
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

    }

    void FixedUpdate()  // 디폴트는 1초에 50번
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed

        // 방향키를 꾹 눌렀을 때 힘을 계속 더하니까 계속 빨라지겠군 ==> Max speed 를 정하자
        // Velocity : 리지드 바디의 현재 속도
        if (rigid.velocity.x > maxSpeed)      // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))    // Left Max Speed
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        // Lnading Platform
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            // Attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) 
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            else
            {
                OnDamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");       // Contains(비교문) : 대상 문자열에 비교문이 있으면 true
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;



            // Deactive Item
            collision.gameObject.SetActive(false);

            // Sound
            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            // Next Stage
            gameManager.NextStage();

            //Sound
            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint += 100;

        // Reaction Force
        rigid.AddForce(Vector2.up * 5 , ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();

        // Change Layer (Immortal Active)         ( Layer를 Player 에서 PlayerDamaged 로 바꾸자)
        gameObject.layer = 11;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7 , ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);      // Invoke() 함수로 무적 시간을 결정.
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        // Collider Disable
        capsulecollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }

        audioSource.Play();

    }

}

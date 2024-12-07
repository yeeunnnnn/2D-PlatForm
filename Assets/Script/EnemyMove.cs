using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;  // �ൿ��ǥ�� ������ ���� �ϳ��� ����
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulecollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        // Move
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

        // Platform Check
        // �̵��ϴ� ����� ���¸� �����ؾ� �ϹǷ� ���� üũ�ؾ� ��.
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null )
        {
            Turn();

        }
    }

    //����Լ� : �ڽ��� ������ ȣ���ϴ� �Լ�
    void Think() // �ൿ��ǥ�� �ٲ��� �Լ� �ϳ��� ����
    {

        // Set Next Active
        // Random : ���� ���� �����ϴ� ���� ���� Ŭ����
        nextMove = Random.Range(-1, 2);
        
     
        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
        
        // Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);     // ������ ���� ����Լ��� ����ϴ� ���� ���� ����.!!  ==>> �츮�� �ð��� ���� ���� �� �� �Լ��� �����Ű�� �ʹ�.

    }

    void Turn()
    {
        nextMove *= -1;
        // CancelInvoke() : ���� �۵� ���� ��� Invoke �Լ��� ���ߴ� �Լ�
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y
        spriteRenderer.flipY = true;

        // Collider Disable
        capsulecollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Destory          ��Ȱ��ȭ ������ �ð����� �ξ� �����Ų��.
        Invoke("DeActive", 5);

    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

}
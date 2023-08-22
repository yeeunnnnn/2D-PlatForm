using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;


    public void NextStage()
    {
        //Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        else    // Game Clear
        {
            // Player Control Lock
            Time.timeScale = 0;

            // Result UI
            Debug.Log("���� Ŭ����!");

            // Restart Button UI
        }

        // Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
            health--;
        else
        {
            // Player Die Effect
            player.OnDie();

            // Result Ui
            Debug.Log("�׾����ϴ�!");

            // Retry Button UI
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Player Reposition
            if(health > 1)                  // ������ ü�¿��� ���������� ��, ����ġ ���� �ʱ�.
            {
                PlayerReposition();
            }

            // Health Down
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }


}
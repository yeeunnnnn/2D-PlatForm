using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;          // 전체 점수
    public int stagePoint;          // 현재 스테이지 점수
    public int stageIndex;          // 현재 스테이지 번호
    public int health;              // 플레이어 체력
    public PlayerMove player;       // 플레이어 객체
    public GameObject[] Stages;     // 스테이지 배열

    public Image[] UIhealth;        // UI 체력 이미지 배열
    public Text UIPoint;            // 점수 텍스트
    public Text UIStage;            // 스테이지 텍스트
    public GameObject RestartBtn;   // 재시작 버튼
    public GameObject StartPanel;   // 시작화면 패널

    private bool isGameStarted = false; // 게임 시작 여부 확인

    private void Start()
    {
        if (!isGameStarted)
        {
            // 게임이 처음 시작될 때만 StartPanel 활성화
            Time.timeScale = 0;        // 게임 정지
            StartPanel.SetActive(true);
            RestartBtn.SetActive(false); // 재시작 버튼 숨김
        }
        else
        {
            // 리트라이 상태에서 StartPanel 비활성화
            StartPanel.SetActive(false);
        }

        // 스테이지 초기화
        foreach (var stage in Stages)
        {
            stage.SetActive(false);
        }
        Stages[0].SetActive(true);  // 첫 번째 스테이지 활성화
    }

    private void Update()
    {
        // UI 점수 업데이트
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void StartGame()
    {
        // 시작화면을 비활성화하고 게임 시작
        StartPanel.SetActive(false);
        Time.timeScale = 1;          // 게임 시작
        isGameStarted = true;        // 게임이 시작되었음을 표시

        // 초기화
        totalPoint = 0;
        stagePoint = 0;
        stageIndex = 0;
        health = UIhealth.Length;    // 체력 초기화
        UIStage.text = "STAGE 1";    // 스테이지 UI 초기화
        PlayerReposition();          // 플레이어 위치 초기화
        RestartBtn.SetActive(false); // 리트라이 버튼 숨김
    }

    public void NextStage()
    {
        // 스테이지 이동
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            // 게임 클리어
            Time.timeScale = 0;  // 게임 정지
            Debug.Log("게임 클리어!");

            // 클리어 메시지 및 버튼 활성화
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartBtn.SetActive(true);
        }

        // 점수 계산
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f); // 체력 UI 업데이트
        }
        else
        {
            // 모든 체력 소진
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);

            // 플레이어 사망 처리
            player.OnDie();

            // 재시작 버튼 활성화
            RestartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1)
            {
                PlayerReposition();
            }
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        // 플레이어 초기 위치로 이동
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero(); // 속도 초기화
    }

    public void Restart()
    {
        // 리트라이 시 StartPanel을 비활성화한 상태에서 게임 재시작
        isGameStarted = true;  // 초기화면 표시 금지
        Time.timeScale = 1;    // 게임 속도 복원
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

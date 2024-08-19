using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneRestart : MonoBehaviour
{
    public string gameSceneName = "InGameScene";

    void Update()
    {
        // 마우스 클릭 (왼쪽, 오른쪽, 중간 버튼) 또는 아무 키 입력 감지
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // InGameScene으로 전환
        SceneManager.LoadScene(gameSceneName);
    }
}
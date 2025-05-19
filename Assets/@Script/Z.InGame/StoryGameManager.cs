using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryGameManager : MonoBehaviour
{
    public GameObject GameClearImage;
    private bool gameCleared = false;
    private bool hasFaded = false;
    private bool Story1 = false;

    void Start()
    {
        SoundManager.Instance.BgmSource.Stop();
        SoundManager.Instance.PlayGame1();


        GameClearImage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private float clearTime = 0f;
    private bool canClick = false;

    void Update()
    {
        if (!gameCleared && MonsterController.Instance != null && MonsterController.Instance._isDead)
        {

            Cursor.visible = true;                     // 마우스 커서 보이게
            Cursor.lockState = CursorLockMode.None;    // 커서 잠금 해제

            GameClearImage.SetActive(true);
            gameCleared = true;
            Time.timeScale = 0f;
            clearTime = Time.unscaledTime + 1f; // 현재 시간 + 1초
            canClick = false;
        }

        if (gameCleared && !canClick && Time.unscaledTime >= clearTime)
        {
            canClick = true;
        }

        if (canClick && Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1f;
            Story1 = true;
            hasFaded = true;

            StartCoroutine(LoadWithFade());
        }
    }


    IEnumerator LoadWithFade()
    {
        yield return FadeManager.Instance.FadeOut();

        SceneLoadData.NextSceneName = "StartGame"; // 예: "GameScene"
        SceneManager.LoadScene("Loading");
    }


}

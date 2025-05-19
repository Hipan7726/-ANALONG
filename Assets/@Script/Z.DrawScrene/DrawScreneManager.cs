using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrawScreneManager : MonoBehaviour
{
    public Button BackButton;
    void Start()
    {

        BackButton.onClick.AddListener(Back);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    void Back()
    {
        SoundManager.Instance.PlayButton();
        SoundManager.Instance.BgmSource.Stop();
        SoundManager.Instance.PlayStartGame();


        StartCoroutine(BackScrene());
    }

    IEnumerator BackScrene()
    {

        yield return FadeManager.Instance.FadeOut();
        SceneManager.LoadScene("StartGame");
    }

}

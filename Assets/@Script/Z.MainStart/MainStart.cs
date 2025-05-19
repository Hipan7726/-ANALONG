using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStart : MonoBehaviour
{
    private bool hasFaded = false;

    private void Start()
    {
        SoundManager.Instance.PlayMainStart();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasFaded)
        {
            SoundManager.Instance.BgmSource.Stop();

            hasFaded = true;
            StartCoroutine(LoadWithFade());
        }
    }

    IEnumerator LoadWithFade()
    {
        yield return FadeManager.Instance.FadeOut();

        SceneLoadData.NextSceneName = "StartGame"; // ¿¹: "GameScene"
        SceneManager.LoadScene("Loading");
    }
}

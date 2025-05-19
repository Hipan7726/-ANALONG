using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HDMScreneButton : MonoBehaviour
{
    public Button BackButton;

    private bool hasFaded = false;


    private void Start()
    {
        BackButton.onClick.AddListener(OnClickBack);
    }

    void OnClickBack()
    {
        hasFaded = true;
        StartCoroutine(LoadWithFade());

    }

    IEnumerator LoadWithFade()
    {
        yield return FadeManager.Instance.FadeOut();

        SceneManager.LoadScene("StartGame");
    }

}

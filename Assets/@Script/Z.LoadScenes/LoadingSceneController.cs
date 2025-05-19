using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.5f); // 연출용 대기

        string nextScene = SceneLoadData.NextSceneName;

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("다음 씬 이름이 지정되지 않았습니다.");
            yield break;
        }


        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); // 로딩 연출 마무리

        op.allowSceneActivation = true;
    }
}

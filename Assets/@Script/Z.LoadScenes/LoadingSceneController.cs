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
        yield return new WaitForSeconds(0.5f); // ����� ���

        string nextScene = SceneLoadData.NextSceneName;

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("���� �� �̸��� �������� �ʾҽ��ϴ�.");
            yield break;
        }


        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); // �ε� ���� ������

        op.allowSceneActivation = true;
    }
}

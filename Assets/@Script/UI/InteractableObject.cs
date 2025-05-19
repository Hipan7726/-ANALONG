using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("�ʿ� ����Ʈ ID (���� ������ ��� ����)")]
    public string requiredQuestId;

    private bool hasFaded = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

    }

    public void Interact()
    {
        if (!string.IsNullOrEmpty(requiredQuestId))
        {
            if (!QuestManager.Instance.IsQuestCompleted(requiredQuestId))
            {
                Debug.Log($"[InteractableObject] {requiredQuestId} ����Ʈ�� �Ϸ��ؾ� ����� �� ����");
                return;
            }
        }

        if (!hasFaded)
        {
            SoundManager.Instance.BgmSource.Stop();

            hasFaded = true;
            StartCoroutine(LoadWithFade());
        }
    }

    IEnumerator LoadWithFade()
    {
        yield return FadeManager.Instance.FadeOut();
        SceneLoadData.NextSceneName = "Story1"; // ��: "GameScene"

        SceneManager.LoadScene("Loading");
    }
}

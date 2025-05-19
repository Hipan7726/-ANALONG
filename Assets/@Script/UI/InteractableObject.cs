using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("필요 퀘스트 ID (비우면 누구나 사용 가능)")]
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
                Debug.Log($"[InteractableObject] {requiredQuestId} 퀘스트를 완료해야 사용할 수 있음");
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
        SceneLoadData.NextSceneName = "Story1"; // 예: "GameScene"

        SceneManager.LoadScene("Loading");
    }
}

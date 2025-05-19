using UnityEngine;

/// <summary>
/// 플레이어가 NPC와 상호작용할 수 있게 하는 스크립트
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    // 현재 상호작용할 수 있는 퀘스트 오브젝트 (NPC)
    private QuestObject currentQuestObject;

    private InteractableObject currentInteractable;

    private void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue)
            return;


        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentQuestObject != null)
            {
                string npcName = currentQuestObject.npcName;
                if (QuestManager.Instance.IsQuestCompletedByNPC(npcName)) return;
                currentQuestObject.Interact();
            }
            else if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
            currentQuestObject = other.GetComponent<QuestObject>();
        else if (other.CompareTag("Interactable"))
            currentInteractable = other.GetComponent<InteractableObject>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC") && currentQuestObject != null && currentQuestObject.gameObject == other.gameObject)
            currentQuestObject = null;
        else if (other.CompareTag("Interactable") && currentInteractable != null && currentInteractable.gameObject == other.gameObject)
            currentInteractable = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;

    [Header("Quest")] 
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    Quest activeQuest;

    ItemGiver itemGiver;
    CenfomonGiver cenfomonGiver;
    Healer healer;
    Merchant merchant;
    ProfFonsecaGiver profFonsecaGiver;

    private void Awake()
    {
        itemGiver = GetComponent<ItemGiver>();
        cenfomonGiver = GetComponent<CenfomonGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
        profFonsecaGiver = GetComponent<ProfFonsecaGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if(questToComplete != null)
        {
            var quest = new Quest(questToComplete);
            yield return quest.CompleteQuest(initiator);
            questToComplete = null;

            Debug.Log($"{quest.Base.Name} completado");
        }

        if(itemGiver != null && itemGiver.CanBeGiven())
        {
            yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
        }
        else if (cenfomonGiver != null && cenfomonGiver.CanBeGiven())
        {
            yield return cenfomonGiver.GiveCenfomon(initiator.GetComponent<PlayerController>());
        }
        else if (profFonsecaGiver != null)
        {
            Debug.Log("Empieza la función");
            yield return profFonsecaGiver.ProfFonsecaCenfomon(initiator.GetComponent<PlayerController>());
        }
        else if(questToStart != null)
        {
            activeQuest = new Quest(questToStart);
            yield return activeQuest.StartQuest();
            questToStart = null;

            if (activeQuest.CanBeCompleted())
            {
                yield return activeQuest.CompleteQuest(initiator);
                activeQuest = null;
            }
        }
        else if(activeQuest != null)
        {
            if(activeQuest.CanBeCompleted())
            {
                yield return activeQuest.CompleteQuest(initiator);
                activeQuest = null;
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialogue);
            }
        }
        else if(healer != null)
        {
            yield return healer.Heal(initiator, dialog);
        }
        else if (merchant != null)
        {
            yield return merchant.Trade();
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialog);
        }

        
        //StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();

        if(questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();

        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if(saveData != null)
        {
            activeQuest = (saveData.activeQuest != null)? new Quest(saveData.activeQuest) : null;
            questToStart = (saveData.questToStart != null)? new Quest(saveData.questToStart).Base : null;
            questToComplete = (saveData.questToComplete != null)? new Quest(saveData.questToComplete).Base : null;
        }
    }
}

[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}

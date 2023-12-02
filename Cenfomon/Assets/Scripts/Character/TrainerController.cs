using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    //Estado
    bool battleLost = false;

    public IEnumerator Interact(Transform initiator)
    {
        if(!battleLost)
        {
            //StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartTrainerBattle(this);
        }
        else
        {
            //StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
        
    }
    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        //Mostrar exclamación
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //Mostrar Dialogo
        yield return DialogManager.Instance.ShowDialog(dialog);
        GameController.Instance.StartTrainerBattle(this);
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if(battleLost)
            fov.gameObject.SetActive(false);
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}

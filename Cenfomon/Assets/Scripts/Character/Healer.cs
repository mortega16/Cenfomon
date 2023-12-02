using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = -1;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText("¿Gustaría sanar sus Cenfomones?",
            choices: new List<string>() { "Sí", "No"},
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex));

        if (selectedChoice >= 0)
        {
            if (selectedChoice == 0)
            {
                // Sí
                var playerParty = player.GetComponent<CenfomonParty>();
                playerParty.Cenfomons.ForEach(P => P.Heal());
                playerParty.PartyUpdated();

                yield return DialogManager.Instance.ShowDialogText($"Tus cenfomones están listos para la aventura");
            }
            else if (selectedChoice == 1)
            {
                // No
                yield return DialogManager.Instance.ShowDialogText($"No hay problema, estamos para servirle");
            }
        }
    }
}

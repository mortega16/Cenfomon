using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfFonsecaGiver : MonoBehaviour, ISavable,IProfFonsecaGiver
{
    [SerializeField] Cenfomon starterCenfomon1Fuego;
    [SerializeField] Cenfomon starterCenfomon2Agua;
    [SerializeField] Cenfomon starterCenfomon3Planta;
    [SerializeField] Dialog dialog;

    Cenfomon cenfomonCompa�ero;

    int fuegoCenfomon = 0;
    int aguaCenfomon = 0;
    int plantaCenfomon = 0;

    bool used = false;

    public IEnumerator ProfFonsecaCenfomon(PlayerController player)
    {
        Debug.Log("Empieza la funci�n");
        

        int selectedChoice = -1;
        yield return DialogManager.Instance.ShowDialog(dialog);


        //Primera Pregunta

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText("�Cu�l es tu bioma preferido?",
            choices: new List<string>() { "Desierto", "Oc�ano","Bosque Tropical" },
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex));

        if (selectedChoice >= 0)
        {
            if (selectedChoice == 0)
            {
                // Desierto
                fuegoCenfomon++;
            }
            else if (selectedChoice == 1)
            {
                // Oc�ano
                aguaCenfomon++;
            }
            else if(selectedChoice == 2)
            {
                // Bosque Tropical
                plantaCenfomon++;
            }
        }
        Debug.Log($"Fuego: {fuegoCenfomon}");
        Debug.Log($"Agua: {aguaCenfomon}");
        Debug.Log($"Planta: {plantaCenfomon}");

        //Segunda Pregunta

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText("�Qu� prefieres hacer en tus d�as libres?",
            choices: new List<string>() { "Asados", "Nataci�n", "Senderismo" },
            onChoiceSelected: (choiceIndex2) => selectedChoice = choiceIndex2));

        if (selectedChoice >= 0)
        {
            if (selectedChoice == 0)
            {
                // Asados
                fuegoCenfomon++;
            }
            else if (selectedChoice == 1)
            {
                // Nataci�n
                aguaCenfomon++;
            }
            else if (selectedChoice == 2)
            {
                // Senderismo
                plantaCenfomon++;
            }
        }
        Debug.Log($"Fuego: {fuegoCenfomon}");
        Debug.Log($"Agua: {aguaCenfomon}");
        Debug.Log($"Planta: {plantaCenfomon}");

        //Tercera Pregunta

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText("�Qu� m�sica prefieres?",
            choices: new List<string>() { "Rock", "LoFi", "Jazz" },
            onChoiceSelected: (choiceIndex3) => selectedChoice = choiceIndex3));

        if (selectedChoice >= 0)
        {
            if (selectedChoice == 0)
            {
                // Rock
                fuegoCenfomon++;
            }
            else if (selectedChoice == 1)
            {
                // LoFi
                aguaCenfomon++;
            }
            else if (selectedChoice == 2)
            {
                // Jazz
                plantaCenfomon++;
            }
        }
        Debug.Log($"Fuego: {fuegoCenfomon}");
        Debug.Log($"Agua: {aguaCenfomon}");
        Debug.Log($"Planta: {plantaCenfomon}");

        //Cuarta Pregunta

        yield return DialogManager.Instance.ShowDialogText("��ltima pregunta!");

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText("�Qu� te gusta ver en Cenfoflix?",
            choices: new List<string>() { "Romance", "Dramas", "Documentales" },
            onChoiceSelected: (choiceIndex4) => selectedChoice = choiceIndex4));

        if (selectedChoice >= 0)
        {
            if (selectedChoice == 0)
            {
                // Romance
                fuegoCenfomon++;
            }
            else if (selectedChoice == 1)
            {
                // Dramas
                aguaCenfomon++;
            }
            else if (selectedChoice == 2)
            {
                // Documentales
                plantaCenfomon++;
            }
        }
        Debug.Log($"Fuego: {fuegoCenfomon}");
        Debug.Log($"Agua: {aguaCenfomon}");
        Debug.Log($"Planta: {plantaCenfomon}");

        //Manejar la l�gica para dar el Cenfomon Inicial

        //Gana Fuego
        if(fuegoCenfomon > aguaCenfomon && fuegoCenfomon > plantaCenfomon)
        {
            starterCenfomon1Fuego.Init();
            player.GetComponent<CenfomonParty>().AddCenfomon(starterCenfomon1Fuego);

            used = true;

            string dialogText = $"�{player.Name} ha recibido un {starterCenfomon1Fuego.Base.Name}!";
            yield return DialogManager.Instance.ShowDialogText(dialogText);

            cenfomonCompa�ero = starterCenfomon1Fuego;
        }
        //Gana Agua
        else if(aguaCenfomon > fuegoCenfomon && aguaCenfomon > plantaCenfomon)
        {
            starterCenfomon2Agua.Init();
            player.GetComponent<CenfomonParty>().AddCenfomon(starterCenfomon2Agua);

            used = true;

            string dialogText = $"�{player.Name} ha recibido un {starterCenfomon2Agua.Base.Name}!";
            yield return DialogManager.Instance.ShowDialogText(dialogText);

            cenfomonCompa�ero = starterCenfomon2Agua;
        }
        //Gana Planta
        else if (plantaCenfomon > fuegoCenfomon && plantaCenfomon > aguaCenfomon)
        {
            starterCenfomon3Planta.Init();
            player.GetComponent<CenfomonParty>().AddCenfomon(starterCenfomon3Planta);

            used = true;

            string dialogText = $"�{player.Name} ha recibido un {starterCenfomon3Planta.Base.Name}!";
            yield return DialogManager.Instance.ShowDialogText(dialogText);

            cenfomonCompa�ero = starterCenfomon3Planta;
        }
        //Empatan Fuego y Agua
        else if (fuegoCenfomon == aguaCenfomon)
        {
            Cenfomon randomCenfomon = Random.Range(0, 2) == 0 ? starterCenfomon1Fuego : starterCenfomon2Agua;

            Debug.Log("El Cenfomon seleccionado es: " + randomCenfomon.Base.Name);

            randomCenfomon.Init();
            player.GetComponent<CenfomonParty>().AddCenfomon(randomCenfomon);

            used = true;

            string dialogText = $"�{player.Name} ha recibido un {randomCenfomon.Base.Name}!";
            yield return DialogManager.Instance.ShowDialogText(dialogText);

            cenfomonCompa�ero = randomCenfomon;
        }
        //Empatan Fuego y Planta
        else if (fuegoCenfomon == plantaCenfomon)
        {
            Cenfomon randomCenfomon = Random.Range(0, 2) == 0 ? starterCenfomon1Fuego : starterCenfomon3Planta;

            Debug.Log("El Cenfomon seleccionado es: " + randomCenfomon.Base.Name);

            randomCenfomon.Init();
            player.GetComponent<CenfomonParty>().AddCenfomon(randomCenfomon);

            used = true;

            string dialogText = $"�{player.Name} ha recibido un {randomCenfomon.Base.Name}!";
            yield return DialogManager.Instance.ShowDialogText(dialogText);

            cenfomonCompa�ero = randomCenfomon;
        }
        //Empatan Agua y Planta
        else if (plantaCenfomon == aguaCenfomon)
        {
            Cenfomon randomCenfomon = Random.Range(0, 2) == 0 ? starterCenfomon2Agua : starterCenfomon3Planta;

            Debug.Log("El Cenfomon seleccionado es: " + randomCenfomon.Base.Name);

            randomCenfomon.Init();
            player.GetComponent<CenfomonParty>().AddCenfomon(randomCenfomon);

            used = true;

            string dialogText = $"�{player.Name} ha recibido un {randomCenfomon.Base.Name}!";
            yield return DialogManager.Instance.ShowDialogText(dialogText);

            cenfomonCompa�ero = randomCenfomon;
        }
    }

    public bool CanBeGiven()
    {
        return cenfomonCompa�ero != null && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }

}

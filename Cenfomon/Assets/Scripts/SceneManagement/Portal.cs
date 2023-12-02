using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    public bool TriggerRepeatedly => false;

    IEnumerator SwitchScene()
    {
        //PersistentData.spawnPoint = spawnPoint;

        DontDestroyOnLoad(gameObject);

        //GameController.Instance.PauseGame(true);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        //player.transform.position = PersistentData.spawnPoint.position;

        yield return new WaitForEndOfFrame();

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.transform.position = destPortal.SpawnPoint.position;

        //GameController.Instance.PauseGame(true);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier { A, B, C, D, E}

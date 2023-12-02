using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        
        GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }

    public bool TriggerRepeatedly => false;
}

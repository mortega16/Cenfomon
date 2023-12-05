
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iQuestList 
{
    void AddQuest();
    bool IsStarted();
    QuestList GetQuestList();
    object CaptureState();
    void RestoreState();


}

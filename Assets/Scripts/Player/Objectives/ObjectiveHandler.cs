using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Michsky.UI.Reach;

public class ObjectiveHandler : MonoBehaviour
{
    [SerializeField] QuestItem questItem;
    public List<Objective> objectives;

    public void DisplayObjective()
    {
		questItem.questText = objectives.ElementAt(0).objectiveLabel;        
    } 
    public void DisplayTask()
    {
        
    }
}
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
	public int objectiveIndex = 0;
	public int taskIndex = 0;

    public void DisplayObjective()
    {
		questItem.questText = objectives.ElementAt(objectiveIndex).objectiveLabel;        
    } 
    public void DisplayTask()
    {
		questItem.questText = objectives.ElementAt(objectiveIndex).GetTaskAtIndex(taskIndex).taskLabel;        
        
    }
}
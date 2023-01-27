using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectiveHandler : MonoBehaviour
{
    [SerializeField] TMP_Text objectiveLabel;
    public List<Objective> objectives;

    public void DisplayObjective()
    {
        objectiveLabel.text = objectives.ElementAt(0).objectiveLabel;
        
    } 
    public void DisplayTask()
    {
        
    }


}

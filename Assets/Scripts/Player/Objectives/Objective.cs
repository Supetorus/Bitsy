using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objecive", menuName = "Objective/ObjectiveSO")]
public class Objective: ScriptableObject
{
    [SerializeField] private int index;
    [SerializeField] private bool complete;
    [SerializeField] public string objectiveLabel;
    [SerializeField] private List<Task> tasks = new List<Task>();
}

public class Task
{
    [SerializeField] private int index;
    [SerializeField] private bool optional;
    [SerializeField] private bool complete;
    [SerializeField] private string taskLabel;
}
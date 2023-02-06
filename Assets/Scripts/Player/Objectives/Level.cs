using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Objective/LevelSO")]
public class Level : ScriptableObject
{
	[SerializeField] public List<Objective> objectives;
}

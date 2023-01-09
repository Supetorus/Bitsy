using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildingMakerToolset.PropPlacer
{
    [CreateAssetMenu( fileName = "PrefabGroup", menuName = "BuildingMakerToolset/PrefabGroup", order = 1 )]
    public class PropGroup : ScriptableObject
    {
        public GameObject[] prefabs;
    }

}

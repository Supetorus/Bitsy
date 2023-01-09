using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BuildingMakerToolset.PropPlacer;
using System.Linq;

namespace BuildingMakerToolset
{
    [CreateAssetMenu( fileName = "new SetIsntaller", menuName = "BuildingMakerToolset/BMT-Set-Installer", order = 2 )]
    public class BMTSetInstaller : ScriptableObject
    {
    #if UNITY_EDITOR

        public GameObject[] targetPrefabs;
        public PropGroup[] PropGroupsToAdd;

        public void Install()
        {
            foreach (PropGroup group in PropGroupsToAdd)
            {
                if (group == null)
                    continue;

                foreach (GameObject prefab in targetPrefabs)
                {
                    if (prefab == null)
                        continue;
                    RowPropPlacer rpp = prefab.GetComponent<RowPropPlacer>();
                    if (rpp == null)
                    {
                        Debug.LogWarning( "not able to install prop group "+ group.name + " to prefab " + prefab.name + " because it has no RowPropPlacer component attached" );
                        continue;
                    }

                    if(rpp.prefabGroups.Contains( group ))
                        continue;

                    List <PropGroup> groupList = rpp.prefabGroups.ToList();
                    groupList.Add( group );
                    rpp.prefabGroups = groupList.ToArray();
                    EditorUtility.SetDirty( prefab );
                    Debug.Log( "added " + group.name + " to " + prefab.name );
                }
            }
        }
        public void Uninstall()
        {
            foreach (PropGroup group in PropGroupsToAdd)
            {
                if (group == null)
                    continue;

                foreach (GameObject prefab in targetPrefabs)
                {
                    if (prefab == null)
                        continue;
                    RowPropPlacer rpp = prefab.GetComponent<RowPropPlacer>();
                    if (rpp == null)
                    {
                        continue;
                    }

                    if (!rpp.prefabGroups.Contains( group ))
                        continue;

                    List<PropGroup> groupList = rpp.prefabGroups.ToList();
                    groupList.Remove( group );
                    rpp.prefabGroups = groupList.ToArray();
                    EditorUtility.SetDirty( prefab );
                    Debug.Log( "removed " + group.name + " from " + prefab.name  );
                }
            }
        }
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BuildingMakerToolset {
    [CustomEditor( typeof( BMTSetInstaller ) )]
    public class BMTSetInstallerEditor : Editor
    {
        bool setup = false;
        BMTSetInstaller tgt;
        private void OnEnable()
        {
            tgt = target as BMTSetInstaller;
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button( "install" ))
            {
                tgt.Install();
            }
            if (GUILayout.Button( "remove" ))
            {
                tgt.Uninstall();
            }

            setup = GUILayout.Toggle( setup, "setup" );
            if (setup)
            {
                DrawDefaultInspector();
            }
        }
        
    }
}

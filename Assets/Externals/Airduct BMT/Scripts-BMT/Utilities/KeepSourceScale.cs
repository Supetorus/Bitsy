using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingMakerToolset.PropPlacer {
    [ExecuteInEditMode]
    public class KeepSourceScale : MonoBehaviour
    {
#if UNITY_EDITOR
        public Mesh scaleReference;
        private void Start()
        {
            KeepScale();
        }


        void KeepScale()
        {
            if (!this.isActiveAndEnabled)
                return;

            if (scaleReference == null) {
                MeshFilter mf = BHUtilities.FindMeshRelativeToTransform( transform );
                if(mf!=null)
                    scaleReference = mf.sharedMesh;
            }
           
            float scale = BHUtilities.GetMeshScaleFactor( scaleReference );
            transform.localScale = Vector3.one * scale;
            
            foreach(Transform child in transform)
            {
                if (child == transform)
                    continue;
                child.transform.localScale = Vector3.one * (1 / scale);
            }
            
        }
#endif
    }

}

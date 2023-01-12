using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace BuildingMakerToolset.PropPlacer
{
    [ExecuteInEditMode]
    public class BuildingUnit : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool enableTransformationInheritance = true;
        public List<BuildingUnit> pseudoChildren;
        public BuildingUnit pseudoParent;

        
        protected Vector3 oldPos;
        protected Quaternion oldRot;
        protected Vector3 oldScale;
        public bool forceTransformChanged = false;
        private void Start()
        {
            if (Application.isPlaying)
                this.enabled = false;
            TransformChanged();
        }
        [ExecuteInEditMode]
        private void Update()
        {
            UpdateInternal();
        }
        public void UpdateInternal()
        {
            if (TransformChanged())
            {
                CheckIfValid();
                OnTransformChanged();
            }
        }


        public bool TransformChanged()
        {
            bool retVal = false;
            if (forceTransformChanged)
            {
                forceTransformChanged = false;
                retVal = true;
            }
            if (oldPos != transform.position)
                retVal = true;
            if (oldScale != transform.lossyScale)
                retVal = true;
            if (oldRot != transform.rotation)
                retVal = true;
            oldPos = transform.position;
            oldScale = transform.lossyScale;
            oldRot = transform.rotation;
            return retVal;
        }

        bool CheckForInfLoop(BuildingUnit newChild, BuildingUnit newParent)
        {
            bool checkDone = false;
            bool isLoop = false;
            BuildingUnit curParent = newParent;
            while (!checkDone)
            {
                if (curParent == newChild)
                {
                    checkDone = true;
                    isLoop = true;
                }
                curParent = curParent.pseudoParent;
                if (curParent == null)
                    checkDone = true;
            }

            return isLoop;
        }

        public void AddChild(BuildingUnit child , int addAt = -1)
        {
            Undo.RecordObject( this, "Transform Awareness Change" );
            Undo.RecordObject( this.transform, "Transform Awareness Change" );

            Undo.RecordObject( child, "Transform Awareness Change" );
            Undo.RecordObject( child.transform, "Transform Awareness Change" );


            if (pseudoChildren == null)
                pseudoChildren = new List<BuildingUnit>();

            if (CheckForInfLoop( child, this ))
                return;

            forceTransformChanged = true;

            if (child.pseudoParent != null && child.pseudoParent!= this)
                child.pseudoParent.RemoveChild( child );

          

            child.pseudoParent = this;
            if (!pseudoChildren.Contains( child ))
            {
                if (addAt >= 0 && addAt < pseudoChildren.Count && pseudoChildren[addAt] != null)
                {
                    if (pseudoChildren[addAt] != null)
                    {

                        RemoveChild( pseudoChildren[addAt] );
                        pseudoChildren.Insert( addAt, child );
                    }
                    else
                    {
                        pseudoChildren[addAt] = child;
                        return;
                    }
                }
                else
                    pseudoChildren.Add( child );
            }            
            UpdateInternal();
        }

        public void RemoveChild(BuildingUnit child)
        {
            forceTransformChanged = true;
            if (child == null)
                return;
            Undo.RecordObject( this, "Transform Awareness Change" );
            Undo.RecordObject( this.transform, "Transform Awareness Change" );
            

            Undo.RecordObject( child, "Transform Awareness Change" );
            Undo.RecordObject( child.transform, "Transform Awareness Change" );

            if (child.pseudoParent == this)
                child.pseudoParent = null;
            if (pseudoChildren.Contains( child ))
                pseudoChildren.Remove( child );
        }

        public void CheckIfValid()
        {
            if (pseudoChildren != null && pseudoChildren.Count > 0)
            {
                for (int i = 0; i < pseudoChildren.Count; i++)
                {
                    if (pseudoChildren[i] == null ||  pseudoChildren[i] == this)
                    {
                        pseudoChildren.RemoveAt( i );
                        i--;
                    }
                    else if (pseudoChildren[i].pseudoParent != this)
                    {
                        if (pseudoChildren[i].pseudoParent == null)
                            pseudoChildren[i].pseudoParent = this;
                        else
                            RemoveChild( pseudoChildren[i] );
                        CheckIfValid();
                        return;
                    }
                }
            }
            if (pseudoParent != null && !pseudoParent.pseudoChildren.Contains( this ))
            {
                pseudoParent = null;
            }
        }


        private void OnTransformChanged()
        {
            CheckIfValid();
           
            PassOnTransformationToParent();
            PassOnTransformationToChildren();
        }
        public void PassOnTransformationToChildren()
        {
            if (!this.isActiveAndEnabled || !enableTransformationInheritance)
                return;
            if (pseudoChildren == null || pseudoChildren.Count == 0)
                return;
            foreach (BuildingUnit child in pseudoChildren)
            {
                if (!child.isActiveAndEnabled || !child.enableTransformationInheritance)
                    continue;

                
                Undo.RecordObject( child.transform, "transformation" );
                OnPassOnTransformationToChild( child );
                child.CheckIfValid();
                if (child.TransformChanged())
                    child.PassOnTransformationToChildren();
            }
            
        }
        public void PassOnTransformationToParent()
        {
            if (!this.isActiveAndEnabled || !enableTransformationInheritance)
                return;
            if (pseudoParent == null || !pseudoParent.isActiveAndEnabled || !pseudoParent.enableTransformationInheritance)
            {
                return;
            }
            pseudoParent.OnInheritChildTransformation( this );
            CheckIfValid();

            if (pseudoParent != null)
            {
                Undo.RecordObject( pseudoParent.transform, "transformation" );
                pseudoParent.PassOnTransformationToParent();
            }
            else if (TransformChanged())
                PassOnTransformationToChildren();
                

        }
        /*
        public BuildingUnit GetHeadParent()
        {
            int antiLoop = 0;
            BuildingUnit p = this;
            while (p.pseudoParent != null && antiLoop<1000000)
            {
                p = p.pseudoParent;
                antiLoop++;
            }
            return p;
        }
        */
        public virtual void OnPassOnTransformationToChild(BuildingUnit child)
        {
            if (!child.transform.IsChildOf( transform ))
                child.transform.localScale = transform.lossyScale;


            child.transform.rotation = transform.rotation;
            child.transform.position = transform.position;
        }

        public virtual void OnInheritChildTransformation(BuildingUnit updatedChild)
        {

            if (!updatedChild.transform.IsChildOf( transform ) && !transform.IsChildOf( pseudoParent.transform ))
                transform.localScale = updatedChild.transform.lossyScale;
            
            transform.rotation = updatedChild.transform.rotation;
            transform.position = updatedChild.transform.position;
        }



#endif
    }
}

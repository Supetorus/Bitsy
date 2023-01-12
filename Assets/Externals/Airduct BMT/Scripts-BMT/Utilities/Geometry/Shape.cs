using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingMakerToolset.Geometry
{
   [System.Serializable]
    public class Shape
    {
        public enum ShapeType
        {
            platform, hole, cutout, materialChange
        }
        public int lastCreatedPointIndex = 0;
        public string name = "new Shape";
        public float hightOffset;
        public float thickness;
        public bool hole;
        public bool flip;
        public bool wall;


        public Vector2 uvOffset = Vector2.zero;
        public bool useSimpleScale = true;
        public Vector2 uvScale = Vector2.one;

        public bool offset2Same = true;
        public Vector2 uvOffset2 = Vector2.zero;
        public bool useSimpleScale2 = true;
        public Vector2 uvScale2 = Vector2.one;

        public Vector2 wallUvOffset = Vector2.zero;
        public bool wallUseSimpleScale = true;
        public Vector2 wallUvScale = Vector2.one;

        public Material upMaterial;
        public Material sideMaterial;
        public Material downMaterial;


        public  List<Vector3> points = new List<Vector3>();
    }
}
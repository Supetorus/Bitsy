using System;

namespace moveen.descs {
    
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomSkelControlAttribute : Attribute {
        public float min = 0.01f;
        public float max = 100500;
        public bool solve = true;
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomSkelResultAttribute : Attribute {
    }
}
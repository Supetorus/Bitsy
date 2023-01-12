using System;
using UnityEngine;

namespace moveen.editor {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class WarningAttribute : PropertyAttribute {
        public readonly string header;

        public WarningAttribute(string header) {
            this.header = header;
        }
    }
}
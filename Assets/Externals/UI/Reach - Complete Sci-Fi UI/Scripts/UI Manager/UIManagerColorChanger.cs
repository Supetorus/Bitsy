using UnityEngine;

namespace Michsky.UI.Reach
{
    [AddComponentMenu("Reach UI/UI Manager/UI Manager Color Changer")]
    public class UIManagerColorChanger : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager targetUIManager;

        [Header("Colors")]
        public Color accent = new Color32(0, 200, 255, 255);
        public Color accentMatch = new Color32(25, 35, 45, 255);
        public Color primary = new Color32(255, 255, 255, 255);
        public Color secondary = new Color32(255, 255, 255, 255);
        public Color negative = new Color32(255, 75, 75, 255);
        public Color background = new Color32(25, 35, 45, 255);
        public Color color1 = new Color32(25, 35, 45, 255);
        public Color color2 = new Color32(25, 35, 45, 255);
        public Color color3 = new Color32(25, 35, 45, 255);
        public Color color4 = new Color32(25, 35, 45, 255);
        public Color color5 = new Color32(25, 35, 45, 255);
        public Color color6 = new Color32(25, 35, 45, 255);

        [Header("Settings")]
        [SerializeField] private bool applyOnStart;

        void Start()
        {
            if (applyOnStart == true)
            {
                ApplyColors();
            }
        }

        public void ApplyColors()
        {
            if (targetUIManager == null)
            {
                Debug.LogError("Cannot apply the changes due to missing 'Target UI Manager'.", this);
                return;
            }

            targetUIManager.accentColor = accent;
            targetUIManager.accentColorInvert = accentMatch;
            targetUIManager.primaryColor = primary;
            targetUIManager.secondaryColor = secondary;
            targetUIManager.negativeColor = negative;
            targetUIManager.backgroundColor = background;
            targetUIManager.color1= color1;
            targetUIManager.color2= color2;
            targetUIManager.color3 = color3;
            targetUIManager.color4 = color4; 
            targetUIManager.color5 = color5;
            targetUIManager.color6 = color6;

            if (targetUIManager.enableDynamicUpdate == false)
            {
                targetUIManager.enableDynamicUpdate = true;
                Invoke("DisableDynamicUpdate", 1);
            }
        }

        void DisableDynamicUpdate()
        {
            targetUIManager.enableDynamicUpdate = false;
        }
    }
}
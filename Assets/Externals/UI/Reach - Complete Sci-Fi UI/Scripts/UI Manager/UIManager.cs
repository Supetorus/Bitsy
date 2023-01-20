using UnityEngine;
using TMPro;

namespace Michsky.UI.Reach
{
    [CreateAssetMenu(fileName = "New UI Manager", menuName = "Reach UI/New UI Manager")]
    public class UIManager : ScriptableObject
    {
        public static string buildID = "R201-230107";

        // Settings
        public bool enableDynamicUpdate = true;

        // Achievements
        public AchievementLibrary achievementLibrary;
        public Color commonColor = new Color32(255, 255, 255, 255);
        public Color rareColor = new Color32(255, 255, 255, 255);
        public Color legendaryColor = new Color32(255, 255, 255, 255);

        // Audio
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Colors
        public Color accentColor = new Color32(255, 255, 255, 255);
        public Color accentColorInvert = new Color32(255, 255, 255, 255);
        public Color primaryColor = new Color32(255, 255, 255, 255);
        public Color secondaryColor = new Color32(255, 255, 255, 255);
        public Color negativeColor = new Color32(255, 255, 255, 255);
        public Color backgroundColor = new Color32(255, 255, 255, 255);
        public Color color1 = new Color32(25, 35, 45, 255);
        public Color color2 = new Color32(25, 35, 45, 255);
        public Color color3 = new Color32(25, 35, 45, 255);
        public Color color4 = new Color32(25, 35, 45, 255);
        public Color color5 = new Color32(25, 35, 45, 255);
        public Color color6 = new Color32(25, 35, 45, 255);

        // Fonts
        public TMP_FontAsset fontLight;
        public TMP_FontAsset fontRegular;
        public TMP_FontAsset fontMedium;
        public TMP_FontAsset fontSemiBold;
        public TMP_FontAsset fontBold;
        public TMP_FontAsset customFont;

        // Localization
        public bool enableLocalization;
        public LocalizationSettings localizationSettings;
        public static string localizationSaveKey = "GameLanguage_";
        public LocalizationLanguage currentLanguage;
        public static bool isLocalizationEnabled = false;

        // Logo
        public Sprite brandLogo;
        public Color brandLogoColor = new Color32(255, 255, 255, 255);
        public Sprite gameLogo;
        public Color gameLogoColor = new Color32(255, 255, 255, 255);

        // Splash Screen
        public bool enableSplashScreen = true;
        public PressAnyKeyTextType pakType;
        public string pakText = "Press {Any Key} To Start";
        public string pakLocalizationText = "PAK_Part1 {PAK_Key} PAK_Part2";

        public enum BackgroundType { Transparent, Dynamic, Static }
        public enum PressAnyKeyTextType { Default, Custom }
        public enum InputType { Legacy, InputSystem }
    }
}
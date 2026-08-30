namespace Wey.EditorTheme
{
    public enum WeyEditorThemeColorScheme
    {
        Cyan = 0,
        Warm,
        Lavender,
        Sand,
        Mint,
        Rose,
        UnityDefault,
    }

    public static class WeyEditorThemeColorSchemeUtility
    {
        public static readonly WeyEditorThemeColorScheme[] ArrPopupOrder =
        {
            WeyEditorThemeColorScheme.UnityDefault,
            WeyEditorThemeColorScheme.Cyan,
            WeyEditorThemeColorScheme.Warm,
            WeyEditorThemeColorScheme.Lavender,
            WeyEditorThemeColorScheme.Sand,
            WeyEditorThemeColorScheme.Mint,
            WeyEditorThemeColorScheme.Rose,
        };

        public static bool IsUnityDefault(WeyEditorThemeColorScheme scheme)
        {
            return scheme == WeyEditorThemeColorScheme.UnityDefault;
        }

        public static bool UsesCustomPalette(WeyEditorThemeColorScheme scheme)
        {
            return !IsUnityDefault(scheme);
        }

        public static string GetPaletteFileName(WeyEditorThemeColorScheme scheme)
        {
            switch (scheme)
            {
                case WeyEditorThemeColorScheme.UnityDefault: return "unity_default";
                case WeyEditorThemeColorScheme.Warm: return "warm";
                case WeyEditorThemeColorScheme.Lavender: return "lavender";
                case WeyEditorThemeColorScheme.Sand: return "sand";
                case WeyEditorThemeColorScheme.Mint: return "mint";
                case WeyEditorThemeColorScheme.Rose: return "rose";
                default: return "cyan";
            }
        }

        public static string GetDisplayName(WeyEditorThemeColorScheme scheme)
        {
            switch (scheme)
            {
                case WeyEditorThemeColorScheme.UnityDefault: return "Unity Light (Default)";
                case WeyEditorThemeColorScheme.Warm: return "Warm";
                case WeyEditorThemeColorScheme.Lavender: return "Lavender";
                case WeyEditorThemeColorScheme.Sand: return "Sand";
                case WeyEditorThemeColorScheme.Mint: return "Mint";
                case WeyEditorThemeColorScheme.Rose: return "Rose";
                default: return "Cyan";
            }
        }
    }
}

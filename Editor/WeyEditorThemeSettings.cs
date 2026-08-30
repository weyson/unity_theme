using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wey.EditorTheme
{
    public static class WeyEditorThemeSettings
    {
        private const string StrPrefsEnabled = "Wey.EditorTheme.Enabled";
        private const string StrPrefsColorScheme = "Wey.EditorTheme.ColorScheme";

        public static bool IsEnabled
        {
            get => EditorPrefs.GetBool(StrPrefsEnabled, true);
            set
            {
                if (EditorPrefs.GetBool(StrPrefsEnabled, true) == value)
                    return;
                EditorPrefs.SetBool(StrPrefsEnabled, value);
                WeyEditorThemeInjector.Refresh();
            }
        }

        public static void ApplyColorScheme(WeyEditorThemeColorScheme scheme)
        {
            int intValue = (int)scheme;
            EditorPrefs.SetInt(StrPrefsColorScheme, intValue);
            WeyEditorThemePaletteSync.Sync(scheme);
        }

        public static WeyEditorThemeColorScheme ColorScheme
        {
            get => (WeyEditorThemeColorScheme)EditorPrefs.GetInt(
                StrPrefsColorScheme, (int)WeyEditorThemeColorScheme.Cyan);
            set
            {
                int intValue = (int)value;
                if (EditorPrefs.GetInt(StrPrefsColorScheme, (int)WeyEditorThemeColorScheme.Cyan) == intValue)
                    return;
                ApplyColorScheme(value);
            }
        }
    }

    public static class WeyEditorThemeSettingsProvider
    {
        private static readonly string[] ArrColorSchemeLabels =
        {
            WeyEditorThemeColorSchemeUtility.GetDisplayName(WeyEditorThemeColorScheme.Cyan),
            WeyEditorThemeColorSchemeUtility.GetDisplayName(WeyEditorThemeColorScheme.Warm),
            WeyEditorThemeColorSchemeUtility.GetDisplayName(WeyEditorThemeColorScheme.Lavender),
            WeyEditorThemeColorSchemeUtility.GetDisplayName(WeyEditorThemeColorScheme.Sand),
            WeyEditorThemeColorSchemeUtility.GetDisplayName(WeyEditorThemeColorScheme.Mint),
            WeyEditorThemeColorSchemeUtility.GetDisplayName(WeyEditorThemeColorScheme.Rose),
        };

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/Wey/Editor Theme", SettingsScope.User)
            {
                label = "Editor Theme",
                guiHandler = OnGui,
                keywords = new HashSet<string>(new[]
                {
                    "Wey", "Editor", "Theme", "Light", "Cyan", "Warm", "Lavender", "Sand", "Mint", "Rose", "Toolbar", "Color"
                })
            };
            return provider;
        }

        private static void OnGui(string searchContext)
        {
            EditorGUILayout.LabelField("Wey Editor Theme", EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);

            EditorGUI.BeginChangeCheck();
            int intScheme = EditorGUILayout.Popup(
                "Color Scheme",
                (int)WeyEditorThemeSettings.ColorScheme,
                ArrColorSchemeLabels);
            bool isEnabled = EditorGUILayout.Toggle("Enable Toolbar Theme Injection", WeyEditorThemeSettings.IsEnabled);

            EditorGUILayout.HelpBox(
                "Unity must use Edit > Preferences > General > Editor Theme = Light.",
                MessageType.Info);
            EditorGUILayout.HelpBox(
                "Some editor chrome may need a domain reload to fully apply after changing the color scheme.",
                MessageType.Info);
            EditorGUILayout.HelpBox(
                "Disabling stops toolbar color overrides only. Remove the package to fully restore default editor chrome.",
                MessageType.Warning);

            if (EditorGUI.EndChangeCheck())
            {
                WeyEditorThemeSettings.ApplyColorScheme((WeyEditorThemeColorScheme)intScheme);
                WeyEditorThemeSettings.IsEnabled = isEnabled;
            }
        }
    }
}

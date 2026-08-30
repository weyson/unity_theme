using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Wey.EditorTheme
{
    public static class WeyEditorThemeSettings
    {
        private const string StrPrefsEnabled = "Wey.EditorTheme.Enabled";

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
    }

    public static class WeyEditorThemeSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/Wey/Editor Theme", SettingsScope.User)
            {
                label = "Editor Theme",
                guiHandler = OnGui,
                keywords = new HashSet<string>(new[] { "Wey", "Editor", "Theme", "Light", "Cyan", "Toolbar" })
            };
            return provider;
        }

        private static void OnGui(string searchContext)
        {
            EditorGUILayout.LabelField("Wey Editor Theme", EditorStyles.boldLabel);
            EditorGUILayout.Space(4f);
            EditorGUI.BeginChangeCheck();
            bool isEnabled = EditorGUILayout.Toggle("Enable Toolbar Theme Injection", WeyEditorThemeSettings.IsEnabled);
            EditorGUILayout.HelpBox(
                "Unity must use Edit > Preferences > General > Editor Theme = Light.",
                MessageType.Info);
            EditorGUILayout.HelpBox(
                "Disabling stops toolbar color overrides only. Remove the package to fully restore default editor chrome.",
                MessageType.Warning);
            if (EditorGUI.EndChangeCheck())
                WeyEditorThemeSettings.IsEnabled = isEnabled;
        }
    }
}

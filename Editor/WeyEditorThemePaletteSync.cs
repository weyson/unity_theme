using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Wey.EditorTheme
{
    [InitializeOnLoad]
    public static class WeyEditorThemePaletteSync
    {
        public const string StrPackageName = "com.wey.editor-theme";
        public const string StrLightUssPackagePath = "Packages/com.wey.editor-theme/Editor/StyleSheets/Extensions/light.uss";

        private const string StrLightHeader =
            "/* Wey Editor Theme: active palette (auto-synced from Preferences).\n" +
            "   Enable: Edit > Preferences > General > Editor Theme = Light. */\n\n";

        private static readonly Regex RegexPaletteBody = new Regex(
            @"(\.unity-theme-env-variables,[\s\S]*)",
            RegexOptions.Compiled);

        static WeyEditorThemePaletteSync()
        {
            EditorApplication.delayCall += EnsureSynced;
        }

        public static string GetPalettePackagePath(WeyEditorThemeColorScheme scheme)
        {
            string strFileName = WeyEditorThemeColorSchemeUtility.GetPaletteFileName(scheme);
            return $"Packages/{StrPackageName}/Editor/StyleSheets/Palettes/{strFileName}.uss";
        }

        public static void Sync(WeyEditorThemeColorScheme scheme)
        {
            string strPaletteDiskPath = ResolvePaletteDiskPath(scheme);
            if (string.IsNullOrEmpty(strPaletteDiskPath) || !File.Exists(strPaletteDiskPath))
                return;

            string strPaletteText = File.ReadAllText(strPaletteDiskPath);
            Match matchBody = RegexPaletteBody.Match(strPaletteText);
            if (!matchBody.Success)
                return;

            string strLightContent = StrLightHeader + matchBody.Value.TrimEnd() + "\n";
            string strLightDiskPath = ResolveLightUssDiskPath();
            if (string.IsNullOrEmpty(strLightDiskPath))
                return;

            if (File.ReadAllText(strLightDiskPath) == strLightContent)
                return;

            File.WriteAllText(strLightDiskPath, strLightContent);
            AssetDatabase.ImportAsset(StrLightUssPackagePath);
            WeyEditorThemeInjector.InvalidateSheetCache();
            WeyEditorThemeInjector.Refresh();
        }

        public static void EnsureSynced()
        {
            Sync(WeyEditorThemeSettings.ColorScheme);
        }

        private static string ResolvePackageEditorRoot()
        {
            string[] arrGuids = AssetDatabase.FindAssets("WeyEditorThemeInjector t:Script");
            if (arrGuids.Length == 0)
                return null;
            string strScriptPath = AssetDatabase.GUIDToAssetPath(arrGuids[0]);
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(strScriptPath), ".."));
        }

        private static string ResolvePaletteDiskPath(WeyEditorThemeColorScheme scheme)
        {
            string strPackagePath = GetPalettePackagePath(scheme);
            string strDiskPath = AssetDatabase.GetAssetPath(
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(strPackagePath));
            if (!string.IsNullOrEmpty(strDiskPath) && File.Exists(strDiskPath))
                return strDiskPath;

            string strEditorRoot = ResolvePackageEditorRoot();
            if (string.IsNullOrEmpty(strEditorRoot))
                return null;

            string strFileName = WeyEditorThemeColorSchemeUtility.GetPaletteFileName(scheme);
            return Path.Combine(strEditorRoot, "StyleSheets", "Palettes", strFileName + ".uss");
        }

        private static string ResolveLightUssDiskPath()
        {
            string strDiskPath = AssetDatabase.GetAssetPath(
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(StrLightUssPackagePath));
            if (!string.IsNullOrEmpty(strDiskPath) && File.Exists(strDiskPath))
                return strDiskPath;

            string strEditorRoot = ResolvePackageEditorRoot();
            if (string.IsNullOrEmpty(strEditorRoot))
                return null;

            return Path.Combine(strEditorRoot, "StyleSheets", "Extensions", "light.uss");
        }
    }
}

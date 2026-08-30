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

        private const string StrPrefsSyncNonce = "Wey.EditorTheme.SyncNonce";

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
            string strPaletteDiskPath = ResolveAssetDiskPath(GetPalettePackagePath(scheme));
            if (string.IsNullOrEmpty(strPaletteDiskPath) || !File.Exists(strPaletteDiskPath))
                return;

            string strPaletteText = File.ReadAllText(strPaletteDiskPath);
            Match matchBody = RegexPaletteBody.Match(strPaletteText);
            if (!matchBody.Success)
                return;

            int intNonce = EditorPrefs.GetInt(StrPrefsSyncNonce, 0) + 1;
            EditorPrefs.SetInt(StrPrefsSyncNonce, intNonce);

            string strSchemeName = WeyEditorThemeColorSchemeUtility.GetDisplayName(scheme);
            string strLightHeader =
                "/* Wey Editor Theme: active palette (auto-synced from Preferences).\n" +
                $"   active: {strSchemeName}; sync: {intNonce}\n" +
                "   Enable: Edit > Preferences > General > Editor Theme = Light. */\n\n";

            string strLightContent = strLightHeader + matchBody.Value.TrimEnd() + "\n";
            string strLightDiskPath = ResolveAssetDiskPath(StrLightUssPackagePath);
            if (string.IsNullOrEmpty(strLightDiskPath))
                return;

            File.WriteAllText(strLightDiskPath, strLightContent);
            AssetDatabase.ImportAsset(
                StrLightUssPackagePath,
                ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            WeyEditorThemeInjector.Refresh();
        }

        public static void EnsureSynced()
        {
            Sync(WeyEditorThemeSettings.ColorScheme);
        }

        private static string ResolveAssetDiskPath(string packagePath)
        {
            UnityEditor.PackageManager.PackageInfo packageInfo =
                UnityEditor.PackageManager.PackageInfo.FindForAssetPath(packagePath);
            if (packageInfo != null && !string.IsNullOrEmpty(packageInfo.resolvedPath))
            {
                string strRelative = packagePath;
                string strPackagePrefix = $"Packages/{packageInfo.name}/";
                if (packagePath.StartsWith(strPackagePrefix, System.StringComparison.Ordinal))
                    strRelative = packagePath.Substring(strPackagePrefix.Length);
                string strDiskPath = Path.Combine(
                    packageInfo.resolvedPath,
                    strRelative.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(strDiskPath))
                    return strDiskPath;
            }

            string strEditorRoot = ResolvePackageEditorRoot();
            if (string.IsNullOrEmpty(strEditorRoot))
                return null;

            if (packagePath == StrLightUssPackagePath)
                return Path.Combine(strEditorRoot, "StyleSheets", "Extensions", "light.uss");

            if (packagePath.StartsWith($"Packages/{StrPackageName}/Editor/StyleSheets/Palettes/", System.StringComparison.Ordinal))
            {
                string strFileName = Path.GetFileName(packagePath);
                return Path.Combine(strEditorRoot, "StyleSheets", "Palettes", strFileName);
            }

            return null;
        }

        private static string ResolvePackageEditorRoot()
        {
            string[] arrGuids = AssetDatabase.FindAssets("WeyEditorThemeInjector t:Script");
            if (arrGuids.Length == 0)
                return null;
            string strScriptPath = AssetDatabase.GUIDToAssetPath(arrGuids[0]);
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(strScriptPath), ".."));
        }
    }
}

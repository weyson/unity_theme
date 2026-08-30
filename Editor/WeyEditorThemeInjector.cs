using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Wey.EditorTheme
{
    /// <summary>Injects the active palette USS onto the main toolbar and nodes with local toolbar stylesheets.</summary>
    [InitializeOnLoad]
    public static class WeyEditorThemeInjector
    {
        private static readonly HashSet<string> KnownThemeSheetNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "light", "cyan", "warm", "lavender", "sand", "mint", "rose", "unity_default",
        };

        private static StyleSheet SheetLight;
        private static VisualElement VeInjectedRoot;
        private static int IntInjectedHash;
        private static FieldInfo FieldToolbarGet;
        private static FieldInfo FieldToolbarRoot;

        static WeyEditorThemeInjector()
        {
            EditorApplication.update += InjectToolbarTheme;
        }

        public static void InvalidateSheetCache()
        {
            SheetLight = null;
            IntInjectedHash = 0;
        }

        public static void Refresh()
        {
            StyleSheet sheetOld = SheetLight;
            if (VeInjectedRoot != null && sheetOld != null)
                RemoveSheetRecursive(VeInjectedRoot, sheetOld);
            VeInjectedRoot = null;
            SheetLight = null;
            IntInjectedHash = 0;
            InjectToolbarTheme();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        private static void InjectToolbarTheme()
        {
            if (ShouldSkipInjection())
            {
                ClearToolbarThemeSheets();
                return;
            }
            if (SheetLight == null)
            {
                string strPalettePath = WeyEditorThemePaletteSync.GetPalettePackagePath(
                    WeyEditorThemeSettings.ColorScheme);
                SheetLight = AssetDatabase.LoadAssetAtPath<StyleSheet>(strPalettePath);
            }
            if (SheetLight == null)
                return;
            VisualElement veRoot = TryGetToolbarRoot();
            if (veRoot == null)
                return;
            bool isSameRoot = VeInjectedRoot == veRoot && veRoot.panel != null;
            bool isSameHash = IntInjectedHash == SheetLight.contentHash;
            if (isSameRoot && isSameHash && HasLightSheet(veRoot, SheetLight))
                return;
            AddOrRefreshSheetRecursive(veRoot);
            VeInjectedRoot = veRoot;
            IntInjectedHash = SheetLight.contentHash;
        }

        private static bool ShouldSkipInjection()
        {
            return EditorGUIUtility.isProSkin
                || !WeyEditorThemeSettings.IsEnabled
                || !WeyEditorThemeColorSchemeUtility.UsesCustomPalette(WeyEditorThemeSettings.ColorScheme);
        }

        private static void ClearToolbarThemeSheets()
        {
            StyleSheet sheetOld = SheetLight;
            VisualElement veRoot = VeInjectedRoot ?? TryGetToolbarRoot();
            if (veRoot != null)
            {
                if (sheetOld != null)
                    RemoveSheetRecursive(veRoot, sheetOld);
                ClearStaleThemeSheetsRecursive(veRoot);
            }
            VeInjectedRoot = null;
        }

        private static VisualElement TryGetToolbarRoot()
        {
            if (FieldToolbarGet == null || FieldToolbarRoot == null)
            {
                Type typeToolbar = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                if (typeToolbar == null)
                    return null;
                FieldToolbarGet = typeToolbar.GetField("get", BindingFlags.Public | BindingFlags.Static);
                FieldToolbarRoot = typeToolbar.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (FieldToolbarGet == null || FieldToolbarRoot == null)
                    return null;
            }
            object objToolbar = FieldToolbarGet.GetValue(null);
            if (objToolbar == null)
                return null;
            return FieldToolbarRoot.GetValue(objToolbar) as VisualElement;
        }

        private static void ClearStaleThemeSheetsRecursive(VisualElement veRoot)
        {
            Queue<VisualElement> queueVe = new Queue<VisualElement>();
            queueVe.Enqueue(veRoot);
            while (queueVe.Count > 0)
            {
                VisualElement ve = queueVe.Dequeue();
                RemoveStaleThemeSheets(ve);
                int intChildCount = ve.childCount;
                for (int i = 0; i < intChildCount; i++)
                    queueVe.Enqueue(ve[i]);
            }
        }

        private static void AddOrRefreshSheetRecursive(VisualElement veRoot)
        {
            Queue<VisualElement> queueVe = new Queue<VisualElement>();
            queueVe.Enqueue(veRoot);
            while (queueVe.Count > 0)
            {
                VisualElement ve = queueVe.Dequeue();
                if (ve.styleSheets.count > 0)
                {
                    RemoveStaleThemeSheets(ve);
                    if (!HasLightSheet(ve, SheetLight))
                        ve.styleSheets.Add(SheetLight);
                }
                int intChildCount = ve.childCount;
                for (int i = 0; i < intChildCount; i++)
                    queueVe.Enqueue(ve[i]);
            }
        }

        private static void RemoveStaleThemeSheets(VisualElement ve)
        {
            for (int i = 0; i < ve.styleSheets.count; i++)
            {
                StyleSheet sheet = ve.styleSheets[i];
                if (sheet != null && sheet != SheetLight && IsStaleThemeSheet(sheet))
                {
                    ve.styleSheets.Remove(sheet);
                    i--;
                }
            }
        }

        private static bool IsStaleThemeSheet(StyleSheet sheet)
        {
            return !string.IsNullOrEmpty(sheet.name) && KnownThemeSheetNames.Contains(sheet.name);
        }

        private static void RemoveSheetRecursive(VisualElement veRoot, StyleSheet sheet)
        {
            if (sheet == null)
                return;
            Queue<VisualElement> queueVe = new Queue<VisualElement>();
            queueVe.Enqueue(veRoot);
            while (queueVe.Count > 0)
            {
                VisualElement ve = queueVe.Dequeue();
                if (HasLightSheet(ve, sheet))
                    ve.styleSheets.Remove(sheet);
                int intChildCount = ve.childCount;
                for (int i = 0; i < intChildCount; i++)
                    queueVe.Enqueue(ve[i]);
            }
        }

        private static bool HasLightSheet(VisualElement veRoot, StyleSheet sheet)
        {
            if (sheet == null)
                return false;
            int intCount = veRoot.styleSheets.count;
            for (int i = 0; i < intCount; i++)
            {
                if (veRoot.styleSheets[i] == sheet)
                    return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Wey.EditorTheme
{
    /// <summary>Loads light.uss onto the main toolbar and nodes with local toolbar stylesheets.</summary>
    [InitializeOnLoad]
    public static class WeyEditorThemeInjector
    {
        public const string StrLightUssPackagePath = WeyEditorThemePaletteSync.StrLightUssPackagePath;

        private const string StrLightSheetName = "light";

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
            if (EditorGUIUtility.isProSkin || !WeyEditorThemeSettings.IsEnabled)
            {
                StyleSheet sheetOld = SheetLight;
                if (VeInjectedRoot != null && sheetOld != null)
                    RemoveSheetRecursive(VeInjectedRoot, sheetOld);
                VeInjectedRoot = null;
                return;
            }
            if (SheetLight == null)
                SheetLight = AssetDatabase.LoadAssetAtPath<StyleSheet>(StrLightUssPackagePath);
            if (SheetLight == null)
                return;
            if (FieldToolbarGet == null || FieldToolbarRoot == null)
            {
                Type typeToolbar = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                if (typeToolbar == null)
                    return;
                FieldToolbarGet = typeToolbar.GetField("get", BindingFlags.Public | BindingFlags.Static);
                FieldToolbarRoot = typeToolbar.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (FieldToolbarGet == null || FieldToolbarRoot == null)
                    return;
            }
            object objToolbar = FieldToolbarGet.GetValue(null);
            if (objToolbar == null)
                return;
            VisualElement veRoot = FieldToolbarRoot.GetValue(objToolbar) as VisualElement;
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

        private static void AddOrRefreshSheetRecursive(VisualElement veRoot)
        {
            Queue<VisualElement> queueVe = new Queue<VisualElement>();
            queueVe.Enqueue(veRoot);
            while (queueVe.Count > 0)
            {
                VisualElement ve = queueVe.Dequeue();
                if (ve.styleSheets.count > 0)
                {
                    RemoveStaleLightSheets(ve);
                    if (!HasLightSheet(ve, SheetLight))
                        ve.styleSheets.Add(SheetLight);
                }
                int intChildCount = ve.childCount;
                for (int i = 0; i < intChildCount; i++)
                    queueVe.Enqueue(ve[i]);
            }
        }

        private static void RemoveStaleLightSheets(VisualElement ve)
        {
            for (int i = 0; i < ve.styleSheets.count; i++)
            {
                StyleSheet sheet = ve.styleSheets[i];
                if (sheet != null && sheet != SheetLight && sheet.name == StrLightSheetName)
                {
                    ve.styleSheets.Remove(sheet);
                    i--;
                }
            }
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

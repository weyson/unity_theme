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
        public const string StrLightUssPackagePath = "Packages/com.wey.editor-theme/Editor/StyleSheets/Extensions/light.uss";

        private static StyleSheet SheetLight;
        private static VisualElement VeInjectedRoot;
        private static int IntInjectedHash;
        private static FieldInfo FieldToolbarGet;
        private static FieldInfo FieldToolbarRoot;

        static WeyEditorThemeInjector()
        {
            EditorApplication.update += InjectToolbarTheme;
        }

        public static void Refresh()
        {
            if (VeInjectedRoot != null)
                RemoveSheetRecursive(VeInjectedRoot);
            VeInjectedRoot = null;
            IntInjectedHash = 0;
            InjectToolbarTheme();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        private static void InjectToolbarTheme()
        {
            if (EditorGUIUtility.isProSkin || !WeyEditorThemeSettings.IsEnabled)
            {
                if (VeInjectedRoot != null)
                    RemoveSheetRecursive(VeInjectedRoot);
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
            if (isSameRoot && isSameHash && HasLightSheet(veRoot))
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
                    if (HasLightSheet(ve))
                    {
                        if (IntInjectedHash != SheetLight.contentHash)
                        {
                            ve.styleSheets.Remove(SheetLight);
                            ve.styleSheets.Add(SheetLight);
                        }
                    }
                    else
                        ve.styleSheets.Add(SheetLight);
                }
                int intChildCount = ve.childCount;
                for (int i = 0; i < intChildCount; i++)
                    queueVe.Enqueue(ve[i]);
            }
        }

        private static void RemoveSheetRecursive(VisualElement veRoot)
        {
            if (SheetLight == null)
                return;
            Queue<VisualElement> queueVe = new Queue<VisualElement>();
            queueVe.Enqueue(veRoot);
            while (queueVe.Count > 0)
            {
                VisualElement ve = queueVe.Dequeue();
                if (HasLightSheet(ve))
                    ve.styleSheets.Remove(SheetLight);
                int intChildCount = ve.childCount;
                for (int i = 0; i < intChildCount; i++)
                    queueVe.Enqueue(ve[i]);
            }
        }

        private static bool HasLightSheet(VisualElement veRoot)
        {
            int intCount = veRoot.styleSheets.count;
            for (int i = 0; i < intCount; i++)
            {
                if (veRoot.styleSheets[i] == SheetLight)
                    return true;
            }
            return false;
        }
    }
}

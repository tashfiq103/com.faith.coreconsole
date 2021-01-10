namespace com.faith.coreconsole
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;

    public class CoreConsoleEditorUtility : Editor
    {
        
        #region Editor Module   :   GUI

        public static void ShowScriptReference(SerializedObject serializedObject)
        {

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();
        }

        public static void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public static void DrawHorizontalLineOnGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "", GUI.skin.horizontalSlider);
        }

        public static void DrawSettingsEditor(Object settings, System.Action OnSettingsUpdated, ref bool foldout, ref Editor editor)
        {

            if (settings != null)
            {

                using (var check = new EditorGUI.ChangeCheckScope())
                {

                    foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

                    if (foldout)
                    {

                        CreateCachedEditor(settings, null, ref editor);
                        editor.OnInspectorGUI();

                        if (check.changed)
                        {

                            if (OnSettingsUpdated != null)
                            {

                                OnSettingsUpdated.Invoke();
                            }
                        }
                    }
                }
            }

        }

        #endregion

        #region Editor Module   :   Asset

        public static List<T> GetAsset<T>(bool returnIfGetAny = false, params string[] directoryFilters)
        {

            return GetAsset<T>("t:" + typeof(T).ToString().Replace("UnityEngine.", ""), returnIfGetAny, directoryFilters);
        }

        public static List<T> GetAsset<T>(string nameFilter, bool returnIfGetAny = false, params string[] directoryFilters)
        {

            List<T> listOfAsset = new List<T>();
            string[] GUIDs;
            if (directoryFilters == null) GUIDs = AssetDatabase.FindAssets(nameFilter);
            else GUIDs = AssetDatabase.FindAssets(nameFilter, directoryFilters);

            foreach (string GUID in GUIDs)
            {

                string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
                listOfAsset.Add((T)System.Convert.ChangeType(AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)), typeof(T)));
                if (returnIfGetAny)
                    break;
            }

            return listOfAsset;
        }

        #endregion

        #region Editor Module   :   CodeGenerator

        public static void GenerateEnum(string path, string nameSpace, string nameOfEnum, params string[] enumValue) {

            string code = "";

            code += string.Format("namespace {0}\n{\n\t", nameSpace);
            code += string.Format("public enum {0}\n{\n", nameOfEnum);

            int numberOfEnumValue = enumValue.Length;

            for (int i = 0; i < numberOfEnumValue; i++) {

                code += string.Format("\t\t{0}{1}\n", enumValue[i], (i < numberOfEnumValue - 1) ? "," : "");
            }

            code += "\t}\n}";

            using (StreamWriter streamWriter = new StreamWriter(path)) {

                streamWriter.Write(code);
            }

            AssetDatabase.Refresh();
        }

        #endregion

        #region Editor Module   :   UnityTechnology

        public static bool DropDownToggle(ref bool toggled, GUIContent content, GUIStyle toggleButtonStyle)
        {
            Rect toggleRect = GUILayoutUtility.GetRect(content, toggleButtonStyle);
            Rect arrowRightRect = new Rect(toggleRect.xMax - toggleButtonStyle.padding.right, toggleRect.y, toggleButtonStyle.padding.right, toggleRect.height);
            bool clicked = EditorGUI.DropdownButton(arrowRightRect, GUIContent.none, FocusType.Passive, GUIStyle.none);

            if (!clicked)
            {
                toggled = GUI.Toggle(toggleRect, toggled, content, toggleButtonStyle);
            }

            return clicked;
        }

        //Extended  :   Tashfiq


        #endregion
    }
}


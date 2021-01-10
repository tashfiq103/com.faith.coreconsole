namespace com.faith.coreconsole
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomEditor(typeof(CoreConsoleConfiguretionFile))]
    public class CoreConsoleConfiguretionFileEditor : CoreConsoleBaseEditorClass
    {
        #region Private Variables

        private CoreConsoleConfiguretionFile _reference;

        private SerializedProperty _sp_isUsedByCentralCoreConsole;
        private SerializedProperty _sp_linkWithCentralCoreConsole;

        private SerializedProperty _sp_enableStackTrace;
        private SerializedProperty _sp_numberOfLog;
        private SerializedProperty _sp_clearLogType;
        private SerializedProperty _sp_listOfLogInfo;


        private SerializedProperty _sp_logType;
        private SerializedProperty _sp_prefix;
        private SerializedProperty _sp_colorForLog;
        private SerializedProperty _sp_colorForLogWarning;
        private SerializedProperty _sp_colorForLogError;

        #endregion

        #region Editor

        public override void OnEnable()
        {
            base.OnEnable();

            if (target.GetType() != typeof(CoreConsoleConfiguretionFile))
                return;

            _reference = (CoreConsoleConfiguretionFile)target;

            _sp_isUsedByCentralCoreConsole = serializedObject.FindProperty("_isUsedByCentralCoreConsole");
            _sp_linkWithCentralCoreConsole = serializedObject.FindProperty("_linkWithCentralCoreConsole");

            _sp_enableStackTrace = serializedObject.FindProperty("_enableStackTrace");
            _sp_numberOfLog = serializedObject.FindProperty("_numberOfLog");
            _sp_clearLogType = serializedObject.FindProperty("_clearLogType");
            _sp_listOfLogInfo = serializedObject.FindProperty("_listOfLogInfo");

            _sp_logType = serializedObject.FindProperty("_logType");
            _sp_prefix = serializedObject.FindProperty("prefix");
            _sp_colorForLog = serializedObject.FindProperty("colorForLog");
            _sp_colorForLogWarning = serializedObject.FindProperty("colorForWarning");
            _sp_colorForLogError = serializedObject.FindProperty("colorForLogError");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {

                EditorGUILayout.BeginHorizontal();
                {
                    CoreConsoleEditorUtility.ShowScriptReference(serializedObject);

                    EditorGUI.BeginDisabledGroup(_sp_isUsedByCentralCoreConsole.boolValue);
                    {
                        if (EditorGUILayout.DropdownButton(EditorGUIUtility.TrTextContent("Advance", "Show advance settings for configuretion file"), FocusType.Passive, EditorStyles.toolbarDropDown, GUILayout.Width(100)))
                        {
                            GenericMenu genericMenuForAdvanceOption = new GenericMenu();

                            genericMenuForAdvanceOption.AddItem(
                                    EditorGUIUtility.TrTextContent("Mark as Production", "It will now be used as central point of all the configuretion file for CoreConsole"),
                                    _sp_isUsedByCentralCoreConsole.boolValue,
                                    () =>
                                    {
                                        _sp_isUsedByCentralCoreConsole.boolValue = !_sp_isUsedByCentralCoreConsole.boolValue;
                                        _sp_isUsedByCentralCoreConsole.serializedObject.ApplyModifiedProperties();

                                        _sp_linkWithCentralCoreConsole.boolValue = false;
                                        _sp_linkWithCentralCoreConsole.serializedObject.ApplyModifiedProperties();

                                        List<CoreConsoleConfiguretionFile> _listOfCoreConsoleConfiguretionFile = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
                                        foreach (CoreConsoleConfiguretionFile coreConsoleConfiguretionFile in _listOfCoreConsoleConfiguretionFile)
                                        {
                                            if (coreConsoleConfiguretionFile == _reference)
                                            {
                                                CoreConsoleViewrEditorWindow.productionCoreConsoleAsset = _reference;
                                            }
                                            else
                                            {

                                                SerializedObject newSerializedObject = new SerializedObject(coreConsoleConfiguretionFile);
                                                SerializedProperty isUsedByCentralCoreConsole = newSerializedObject.FindProperty("_isUsedByCentralCoreConsole");
                                                isUsedByCentralCoreConsole.boolValue = false;
                                                isUsedByCentralCoreConsole.serializedObject.ApplyModifiedProperties();
                                                newSerializedObject.ApplyModifiedProperties();
                                            }
                                        }
                                    }
                                );

                            genericMenuForAdvanceOption.AddItem(
                                    EditorGUIUtility.TrTextContent("Override by Production", "Override by the settings by production"),
                                    _sp_linkWithCentralCoreConsole.boolValue,
                                    () =>
                                    {
                                        _sp_linkWithCentralCoreConsole.boolValue = !_sp_linkWithCentralCoreConsole.boolValue;
                                        _sp_linkWithCentralCoreConsole.serializedObject.ApplyModifiedProperties();
                                    }
                                );

                            genericMenuForAdvanceOption.ShowAsContext();
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                }
                EditorGUILayout.EndHorizontal();

                if (_sp_isUsedByCentralCoreConsole.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The following configuretion asset is used in 'CoreConsoleManager'.\nIt can only be revoked if you assigned any other configuretion file for production", MessageType.Info);
                }
                else
                {

                    if (_sp_linkWithCentralCoreConsole.boolValue)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("The following configuretion is now synced with 'CoreConsoleManager'. To make it standalone, unlink it", MessageType.Info);
                    }
                }

                CoreConsoleEditorUtility.DrawHorizontalLine();

            }
            EditorGUILayout.EndVertical();

            if(!_sp_linkWithCentralCoreConsole.boolValue) {

                EditorGUILayout.PropertyField(_sp_logType);
                EditorGUI.indentLevel += 1;
                switch (_sp_logType.enumValueIndex)
                {
                    case (int)CoreConsoleEnums.LogType.None:

                        break;
                    case (int)CoreConsoleEnums.LogType.Error:
                        EditorGUILayout.PropertyField(_sp_prefix);
                        EditorGUILayout.PropertyField(_sp_colorForLogError);
                        break;
                    case (int)CoreConsoleEnums.LogType.Info:
                        EditorGUILayout.PropertyField(_sp_prefix);
                        EditorGUILayout.PropertyField(_sp_colorForLog);
                        EditorGUILayout.PropertyField(_sp_colorForLogError);
                        break;
                    case (int)CoreConsoleEnums.LogType.Verbose:
                        EditorGUILayout.PropertyField(_sp_prefix);
                        EditorGUILayout.PropertyField(_sp_colorForLog);
                        EditorGUILayout.PropertyField(_sp_colorForLogWarning);
                        EditorGUILayout.PropertyField(_sp_colorForLogError);
                        break;
                }

                if (_sp_logType.enumValueIndex != 3)
                {

                    CoreConsoleEditorUtility.DrawHorizontalLine();

                    EditorGUI.BeginChangeCheck();
                    _sp_enableStackTrace.boolValue = EditorGUILayout.Foldout(
                            _sp_enableStackTrace.boolValue,
                            "StackTrace",
                            true
                        );
                    if (EditorGUI.EndChangeCheck())
                    {

                        if (_sp_enableStackTrace.boolValue == false)
                        {

                            _sp_listOfLogInfo.ClearArray();
                            _sp_listOfLogInfo.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    if (_sp_enableStackTrace.boolValue)
                    {

                        EditorGUI.indentLevel += 1;

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PropertyField(_sp_clearLogType);
                            if (GUILayout.Button("Clear", GUILayout.Width(75)))
                            {
                                _reference.ClearLog((LogType)_sp_clearLogType.enumValueIndex);
                            }

                            Color defaultContentColor = GUI.contentColor;
                            GUI.contentColor = Color.yellow;
                            EditorGUILayout.LabelField("|", EditorStyles.boldLabel, GUILayout.Width(5));
                            GUI.contentColor = defaultContentColor;

                            if (GUILayout.Button("ClearAll", GUILayout.Width(75)))
                            {
                                _reference.ClearAllLog();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (!EditorApplication.isPlaying)
                            EditorGUILayout.PropertyField(_sp_numberOfLog);
                        else
                            EditorGUILayout.LabelField("MaxLogSize : " + _sp_numberOfLog.intValue, EditorStyles.boldLabel);

                        EditorGUI.BeginDisabledGroup(true);
                        {
                            EditorGUILayout.PropertyField(_sp_listOfLogInfo);
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.indentLevel -= 1;
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}


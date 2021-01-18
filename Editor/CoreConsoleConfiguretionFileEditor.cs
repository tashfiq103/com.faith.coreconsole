namespace com.faith.coreconsole
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [CustomEditor(typeof(CoreConsoleConfiguretionFile))]
    internal class CoreConsoleConfiguretionFileEditor : CoreConsoleBaseEditorClass
    {
        #region Private Variables

        private CoreConsoleConfiguretionFile _reference;

        private SerializedProperty _sp_isMarkedAsDefaultSetting;
        private SerializedProperty _sp_isLinkedWithDefaultSetting;

        private SerializedProperty _sp_name;
        private SerializedProperty _sp_isStackTraceEnabled;
        private SerializedProperty _sp_numberOfLog;
        private SerializedProperty _sp_clearLogType;
        private SerializedProperty _sp_listOfLogInfo;


        private SerializedProperty _sp_logType;
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

            _sp_isMarkedAsDefaultSetting = serializedObject.FindProperty("_isMarkedAsDefaultSetting");
            _sp_isLinkedWithDefaultSetting = serializedObject.FindProperty("_isLinkedWithDefaultSetting");

            _sp_name = serializedObject.FindProperty("_name");

            _sp_logType = serializedObject.FindProperty("_logType");
            _sp_isStackTraceEnabled = serializedObject.FindProperty("_isStackTraceEnabled");
            _sp_numberOfLog = serializedObject.FindProperty("_numberOfLog");
            _sp_colorForLog = serializedObject.FindProperty("_colorForLog");
            _sp_colorForLogWarning = serializedObject.FindProperty("_colorForWarning");
            _sp_colorForLogError = serializedObject.FindProperty("_colorForLogError");

            _sp_clearLogType = serializedObject.FindProperty("_clearLogType");
            _sp_listOfLogInfo = serializedObject.FindProperty("_listOfLogInfo");

            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {

                EditorGUILayout.BeginHorizontal();
                {
                    CoreConsoleEditorUtility.ShowScriptReference(serializedObject);

                    EditorGUI.BeginDisabledGroup(_sp_isMarkedAsDefaultSetting.boolValue);
                    {
                        if (EditorGUILayout.DropdownButton(EditorGUIUtility.TrTextContent("Advance", "Show advance settings for configuretion file"), FocusType.Passive, EditorStyles.toolbarDropDown, GUILayout.Width(100)))
                        {
                            GenericMenu genericMenuForAdvanceOption = new GenericMenu();

                            genericMenuForAdvanceOption.AddItem(
                                    EditorGUIUtility.TrTextContent("Mark as Default", "It will now be used as default settings for CoreConsole"),
                                    _sp_isMarkedAsDefaultSetting.boolValue,
                                    () =>
                                    {
                                        _sp_isMarkedAsDefaultSetting.boolValue = !_sp_isMarkedAsDefaultSetting.boolValue;
                                        _sp_isMarkedAsDefaultSetting.serializedObject.ApplyModifiedProperties();

                                        _sp_isLinkedWithDefaultSetting.boolValue = false;
                                        _sp_isLinkedWithDefaultSetting.serializedObject.ApplyModifiedProperties();

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
                                                SerializedProperty isMarkedAsDefaultSetting = newSerializedObject.FindProperty("_isMarkedAsDefaultSetting");
                                                isMarkedAsDefaultSetting.boolValue = false;
                                                isMarkedAsDefaultSetting.serializedObject.ApplyModifiedProperties();
                                                newSerializedObject.ApplyModifiedProperties();

                                                EditorUtility.SetDirty(coreConsoleConfiguretionFile);
                                            }
                                        }

                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();
                                    }
                                );

                            genericMenuForAdvanceOption.AddItem(
                                    EditorGUIUtility.TrTextContent("Override by Default", "Override by default settings"),
                                    _sp_isLinkedWithDefaultSetting.boolValue,
                                    () =>
                                    {
                                        _sp_isLinkedWithDefaultSetting.boolValue = !_sp_isLinkedWithDefaultSetting.boolValue;
                                        _sp_isLinkedWithDefaultSetting.serializedObject.ApplyModifiedProperties();

                                        if (_sp_isLinkedWithDefaultSetting.boolValue) {

                                            if (CoreConsoleConfiguretionFile.globalEnableStackTrace)
                                                _reference.EnableStackTrace();
                                            else
                                                _reference.DisableStackTrace();
                                        }
                                    }
                                );

                            genericMenuForAdvanceOption.ShowAsContext();
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                }
                EditorGUILayout.EndHorizontal();

                if (_sp_isMarkedAsDefaultSetting.boolValue)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The following configuretion file is now marked as 'DefaultSettings'", MessageType.Info);
                }
                else
                {

                    if (_sp_isLinkedWithDefaultSetting.boolValue)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("The following configuretion file is override by 'DefaultSettings'. To revoke the override, Go to 'Advance' and 'Uncheck' the 'Override by Default' option", MessageType.Info);
                    }
                }

                CoreConsoleEditorUtility.DrawHorizontalLine();

            }
            EditorGUILayout.EndVertical();

            if(!_sp_isLinkedWithDefaultSetting.boolValue) {

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(_sp_logType);
                }
                if (EditorGUI.EndChangeCheck()) {

                    _sp_logType.serializedObject.ApplyModifiedProperties();
                    if (_sp_isMarkedAsDefaultSetting.boolValue) 
                        CoreConsoleConfiguretionFile.GlobalLogType  = (CoreConsoleEnums.LogType) _sp_logType.enumValueIndex;
                    
                }
                
                EditorGUI.indentLevel += 1;
                switch (_sp_logType.enumValueIndex)
                {
                    case (int)CoreConsoleEnums.LogType.None:

                        break;
                    case (int)CoreConsoleEnums.LogType.Error:

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_sp_colorForLogError);
                        if (EditorGUI.EndChangeCheck()) {

                            _sp_colorForLogError.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                                CoreConsoleConfiguretionFile.globalColorForLogError = _sp_colorForLogError.colorValue;
                        }

                        break;
                    case (int)CoreConsoleEnums.LogType.Info:

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_sp_colorForLog);
                        if (EditorGUI.EndChangeCheck()) {

                            _sp_colorForLog.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                                CoreConsoleConfiguretionFile.globalColorForLog = _sp_colorForLog.colorValue;
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_sp_colorForLogError);
                        if (EditorGUI.EndChangeCheck())
                        {

                            _sp_colorForLogError.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                                CoreConsoleConfiguretionFile.globalColorForLogError = _sp_colorForLogError.colorValue;
                        }

                        break;
                    case (int)CoreConsoleEnums.LogType.Verbose:

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_sp_colorForLog);
                        if (EditorGUI.EndChangeCheck())
                        {

                            _sp_colorForLog.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                                CoreConsoleConfiguretionFile.globalColorForLog = _sp_colorForLog.colorValue;
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_sp_colorForLogWarning);
                        if (EditorGUI.EndChangeCheck())
                        {

                            _sp_colorForLogWarning.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                                CoreConsoleConfiguretionFile.globalColorForLogWarning = _sp_colorForLogWarning.colorValue;
                        }

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(_sp_colorForLogError);
                        if (EditorGUI.EndChangeCheck())
                        {

                            _sp_colorForLogError.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                                CoreConsoleConfiguretionFile.globalColorForLogError = _sp_colorForLogError.colorValue;
                        }
                        break;
                }

                if (_sp_logType.enumValueIndex != 3)
                {
                    CoreConsoleEditorUtility.DrawHorizontalLine();

                    if (!EditorApplication.isPlaying)
                    {
                        EditorGUI.BeginChangeCheck();
                        _sp_isStackTraceEnabled.boolValue = EditorGUILayout.Foldout(
                                _sp_isStackTraceEnabled.boolValue,
                                "StackTrace",
                                true
                            );
                        if (EditorGUI.EndChangeCheck())
                        {
                            _sp_isStackTraceEnabled.serializedObject.ApplyModifiedProperties();

                            if (_sp_isMarkedAsDefaultSetting.boolValue)
                            {
                                CoreConsoleConfiguretionFile.globalEnableStackTrace = _sp_isStackTraceEnabled.boolValue;
                                if (CoreConsoleConfiguretionFile.globalEnableStackTrace)
                                {

                                    List<CoreConsoleConfiguretionFile> _listOfCoreConsoleConfiguretionFile = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
                                    foreach (CoreConsoleConfiguretionFile coreConsoleConfiguretionFile in _listOfCoreConsoleConfiguretionFile)
                                    {

                                        SerializedObject serializedCoreConsoleConfiguretionAsset = new SerializedObject(coreConsoleConfiguretionFile);
                                        SerializedProperty serailizedStackTrace = serializedCoreConsoleConfiguretionAsset.FindProperty("_isStackTraceEnabled");

                                        serailizedStackTrace.boolValue = true;
                                        serailizedStackTrace.serializedObject.ApplyModifiedProperties();
                                        serializedCoreConsoleConfiguretionAsset.ApplyModifiedProperties();

                                        EditorUtility.SetDirty(coreConsoleConfiguretionFile);
                                    }
                                }

                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }

                            if (_sp_isStackTraceEnabled.boolValue)
                            {
                                _reference.EnableStackTrace();
                            }
                            else
                            {

                                _reference.DisableStackTrace();
                                _sp_listOfLogInfo.ClearArray();
                                _sp_listOfLogInfo.serializedObject.ApplyModifiedProperties();
                            }
                        }

                        if (_sp_isStackTraceEnabled.boolValue)
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

                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(_sp_numberOfLog);
                            if (EditorGUI.EndChangeCheck())
                            {

                                _sp_numberOfLog.serializedObject.ApplyModifiedProperties();

                                if (_sp_isMarkedAsDefaultSetting.boolValue)
                                    CoreConsoleConfiguretionFile.globalNumberOfLog = _sp_numberOfLog.intValue;
                            }

                            EditorGUI.BeginDisabledGroup(true);
                            {
                                EditorGUILayout.PropertyField(_sp_listOfLogInfo);
                            }
                            EditorGUI.EndDisabledGroup();

                            EditorGUI.indentLevel -= 1;
                        }

                    }
                    else {

                        EditorGUILayout.HelpBox("You can Enable/Disable 'StackTrace' when editor is not in playmode", MessageType.Info);
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Configuretion

        private void OnUpdate() {

            if (_reference != null && !_sp_name.stringValue.Equals(_reference.name)) {

                _sp_name.stringValue = _reference.name;
                _sp_name.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                CheckDuplicateDefaultSettings();
                TryToGenerateEnum();
                

            }
        }

        private async void CheckDuplicateDefaultSettings() {

            await Task.Delay(100);

            if (_sp_isMarkedAsDefaultSetting.boolValue)
            {

                List<CoreConsoleConfiguretionFile> listOfAsset = new List<CoreConsoleConfiguretionFile>();
                string[] GUIDs = AssetDatabase.FindAssets("t:" + typeof(CoreConsoleConfiguretionFile).ToString().Replace("UnityEngine.", ""));

                foreach (string GUID in GUIDs)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
                    listOfAsset.Add((CoreConsoleConfiguretionFile)System.Convert.ChangeType(AssetDatabase.LoadAssetAtPath(assetPath, typeof(CoreConsoleConfiguretionFile)), typeof(CoreConsoleConfiguretionFile)));
                }

                foreach (CoreConsoleConfiguretionFile coreConsoleConfigFile in listOfAsset)
                {
                    if (coreConsoleConfigFile != _reference && (new SerializedObject(coreConsoleConfigFile).FindProperty("_isMarkedAsDefaultSetting").boolValue))
                    {
                        _sp_isMarkedAsDefaultSetting.boolValue = false;
                        _sp_isMarkedAsDefaultSetting.serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(coreConsoleConfigFile);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        break;
                    }
                }
            }
        }

        private async void TryToGenerateEnum()
        {
            await Task.Delay(100);
            CoreConsoleConfiguretionFileContainer.GenerateEnum();

            await Task.Delay(100);
            CoreConsoleViewrEditorWindow.UpdateListOfCoreConsoleAsset();
        }

        #endregion
    }
}


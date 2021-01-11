namespace com.faith.coreconsole
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;

    public class CoreConsoleViewrEditorWindow : CoreConsoleBaseEditorWindowClass
    {
        #region Public Variables

        public static CoreConsoleConfiguretionFile productionCoreConsoleAsset;

        #endregion

        #region Private Variables

        private static CoreConsoleViewrEditorWindow EditorWindow;

        
        private static List<CoreConsoleConfiguretionFile> _listOfCoreConsoleAsset;
        private static Editor[] _editorForCoreConsoleAsset;
        private static int _numberOfCoreConsoleAsset;
        private static bool[] _isFoldOut;
        private static bool _isFoldOutDefaultCoreConsoleAsset;
        private static bool _isFoldOutOtherCoreConsoleAsset;
        private static bool _isDrawGUI;

        private static Vector2 _scrollPosition;

        private const string _defaultName = "NewConfiguretionFileForCoreConsole";
        private static string _nameOfConfiguretorFile = _defaultName;

        private static GUIStyle HeighlightedBackgroundWithBoldStyle = new GUIStyle();

        #endregion

        #region Configuretion

        private static int IsThereAnyGameConfigAssetWithTheGivenName(string name)
        {
            int _numberOfDuplicateName = 0;
            List<CoreConsoleConfiguretionFile> gameConfiguratorAssets = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
            foreach (CoreConsoleConfiguretionFile gameConfigAsset in gameConfiguratorAssets)
            {
                if (gameConfigAsset.name.Contains(name))
                    _numberOfDuplicateName++;
            }

            return _numberOfDuplicateName;
        }

        private static CoreConsoleConfiguretionFile IsAnyCoreConsoleAssetUsedAsDefaultSettings()
        {

            List<CoreConsoleConfiguretionFile> coreConsoleConfigueAssets = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
            foreach (CoreConsoleConfiguretionFile configAsset in coreConsoleConfigueAssets)
            {
                SerializedProperty isUsedAsDefaultSettings = new SerializedObject(configAsset).FindProperty("_isMarkedAsDefaultSetting");
                if (isUsedAsDefaultSettings.boolValue)
                    return configAsset;
            }
            return null;
        }

        private static void SetLinkStatusWithCentralCoreConsole(bool statusFlag)
        {
            
            if (IsAnyCoreConsoleAssetUsedAsDefaultSettings() != null)
            {

                List<CoreConsoleConfiguretionFile> gameConfiguratorAssets = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();

                foreach (CoreConsoleConfiguretionFile coreConsoleConfigFile in gameConfiguratorAssets)
                {
                    SerializedObject serializedCoreConsoleAsset = new SerializedObject(coreConsoleConfigFile);

                    if (!serializedCoreConsoleAsset.FindProperty("_isMarkedAsDefaultSetting").boolValue)
                    {

                        SerializedProperty _linkWithCentralCoreConsole = serializedCoreConsoleAsset.FindProperty("_linkWithCentralCoreConsole");
                        _linkWithCentralCoreConsole.boolValue = statusFlag;
                        _linkWithCentralCoreConsole.serializedObject.ApplyModifiedProperties();

                        serializedCoreConsoleAsset.ApplyModifiedProperties();
                    }
                }
            }
        }

        public static void UpdateListOfCoreConsoleAsset()
        {
            _listOfCoreConsoleAsset = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
            _numberOfCoreConsoleAsset = _listOfCoreConsoleAsset.Count;

            //Marking   :   Central Game Configuretor Asset
            if (productionCoreConsoleAsset == null)
            {

                foreach (CoreConsoleConfiguretionFile coreConsoleConfigFile in _listOfCoreConsoleAsset)
                {
                    if (new SerializedObject(coreConsoleConfigFile).FindProperty("_isMarkedAsDefaultSetting").boolValue)
                        productionCoreConsoleAsset = coreConsoleConfigFile;
                }
            }

            foreach (CoreConsoleConfiguretionFile coreConsoleConfigFile in _listOfCoreConsoleAsset)
            {
                //if : It is the central game configuretor asset but not matched with the cashed production asset. Remove It From Prodcution
                if (new SerializedObject(coreConsoleConfigFile).FindProperty("_isMarkedAsDefaultSetting").boolValue && productionCoreConsoleAsset != coreConsoleConfigFile)
                {

                    SerializedObject serializedCoreConsoleAsset = new SerializedObject(coreConsoleConfigFile);

                    SerializedProperty _isUsedByCentralCoreConsole = serializedCoreConsoleAsset.FindProperty("_isUsedByCentralCoreConsole");
                    _isUsedByCentralCoreConsole.boolValue = false;
                    _isUsedByCentralCoreConsole.serializedObject.ApplyModifiedProperties();

                    serializedCoreConsoleAsset.ApplyModifiedProperties();
                }
            }

            _isFoldOut = new bool[_numberOfCoreConsoleAsset];
            _editorForCoreConsoleAsset = new Editor[_numberOfCoreConsoleAsset];
        }

        private void CreateNewCoreConsoleAsset()
        {

            if (!Directory.Exists(CoreConsoleConstants.DirectoryForCoreConsoleConfiguretionFile))
                Directory.CreateDirectory(CoreConsoleConstants.DirectoryForCoreConsoleConfiguretionFile);

            _nameOfConfiguretorFile = _nameOfConfiguretorFile.Length == 0 ? _defaultName : _nameOfConfiguretorFile;
            int numberOfDuplicateName = IsThereAnyGameConfigAssetWithTheGivenName(_nameOfConfiguretorFile);
            string absoluteName = _nameOfConfiguretorFile + (numberOfDuplicateName == 0 ? "" : (" " + numberOfDuplicateName));

            CoreConsoleConfiguretionFile newCoreConsoleAsset = CreateInstance<CoreConsoleConfiguretionFile>();

            if (productionCoreConsoleAsset == null) {

                SerializedObject serializedReferenceOfNewCoreConsoleAsset   = new SerializedObject(newCoreConsoleAsset);
                SerializedProperty isMarkedAsDefaulySettings                =  serializedReferenceOfNewCoreConsoleAsset.FindProperty("_isMarkedAsDefaultSetting");
                isMarkedAsDefaulySettings.boolValue                         = true;
                serializedReferenceOfNewCoreConsoleAsset.ApplyModifiedProperties();

                productionCoreConsoleAsset = newCoreConsoleAsset;
            }

            AssetDatabase.CreateAsset(newCoreConsoleAsset, CoreConsoleConstants.DirectoryForCoreConsoleConfiguretionFile + "/" + absoluteName + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = newCoreConsoleAsset;

            UpdateListOfCoreConsoleAsset();
        }

        private void OnEditorUpdate() {

            if (CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>().Count != _numberOfCoreConsoleAsset) {

                _isDrawGUI = false;
                UpdateListOfCoreConsoleAsset();
                CoreConsoleConfiguretionFileContainer.GenerateEnum(100, ()=> {
                    _isDrawGUI = true;
                });
            }
        }

        #endregion

        #region EditorWindow

        [MenuItem("FAITH/CoreConsole/Console Configuretion Viwer", priority = 0)]
        public static void ShowWindow()
        {

            UpdateListOfCoreConsoleAsset();

            EditorWindow = GetWindow<CoreConsoleViewrEditorWindow>("Console Configuretion Viwer", typeof(CoreConsoleViewrEditorWindow));

            EditorWindow.minSize = new Vector2(450f, 240f);
            EditorWindow.Show();
        }

        [MenuItem("FAITH/CoreConsole/Use Default Settings", priority = 1)]
        public static void LinkWithProductionCoreConsole()
        {

            SetLinkStatusWithCentralCoreConsole(true);
        }

        [MenuItem("FAITH/CoreConsole/Use Standalone Settings", priority = 2)]
        public static void UnlinkWithProductionCoreConsole()
        {
            SetLinkStatusWithCentralCoreConsole(false);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            HeighlightedBackgroundWithBoldStyle = new GUIStyle { normal = { background = Texture2D.whiteTexture }, fontStyle = FontStyle.Bold };
            _nameOfConfiguretorFile = _defaultName;


            if (productionCoreConsoleAsset == null)
            {
                CoreConsoleConfiguretionFile defaultSettings = IsAnyCoreConsoleAssetUsedAsDefaultSettings();
                if (defaultSettings != null)
                {
                    productionCoreConsoleAsset = defaultSettings;
                }
                else {

                    _nameOfConfiguretorFile = "DefaultSettings";
                    CreateNewCoreConsoleAsset();
                }
            }

            UpdateListOfCoreConsoleAsset();

            EditorApplication.update += OnEditorUpdate;
            _isDrawGUI = true;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            _isDrawGUI = false;
        }

        public void OnGUI()
        {
            HeaderGUI();
            CoreConsoleGUI();

        }

        #endregion

        #region GUI :   Section

        private void HeaderGUI()
        {

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                _nameOfConfiguretorFile = EditorGUILayout.TextField("Name", _nameOfConfiguretorFile);
                if (GUILayout.Button("+CoreConsoleAsset", GUILayout.Width(175f)))
                {

                    CreateNewCoreConsoleAsset();
                }

                if (GUILayout.Button("Refresh", GUILayout.Width(100f)))
                {

                    UpdateListOfCoreConsoleAsset();
                }
            }
            EditorGUILayout.EndHorizontal();

            CoreConsoleEditorUtility.DrawHorizontalLine();
        }

        private void CoreConsoleGUI()
        {

            Color defaultBackgroundColor = GUI.backgroundColor;
            Color defaultContentColor = GUI.contentColor;

            if (_numberOfCoreConsoleAsset > 0)
            {

                
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    GUI.contentColor = Color.yellow;
                    _isFoldOutDefaultCoreConsoleAsset = EditorGUILayout.Foldout(
                        _isFoldOutDefaultCoreConsoleAsset,
                        "DefaultSettings",true);
                    GUI.contentColor = defaultContentColor;

                    if (_isFoldOutDefaultCoreConsoleAsset) {

                        _isFoldOut[0] = true;
                        CoreConsoleEditorUtility.DrawSettingsEditor(productionCoreConsoleAsset, null, ref _isFoldOut[0], ref _editorForCoreConsoleAsset[0]);
                    }
                }
                EditorGUILayout.EndVertical();
                

                if (_numberOfCoreConsoleAsset > 1)
                {

                    CoreConsoleEditorUtility.DrawHorizontalLine();
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.contentColor = Color.cyan;
                        _isFoldOutOtherCoreConsoleAsset = EditorGUILayout.Foldout(
                                _isFoldOutOtherCoreConsoleAsset,
                                "Other Settings",
                                true
                            );
                        GUI.contentColor = defaultContentColor;
                    }
                    EditorGUILayout.EndVertical();
                    
                }

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                {
                    for (int i = 1; i < _numberOfCoreConsoleAsset; i++)
                    {
                        if (_isFoldOutOtherCoreConsoleAsset)
                        {

                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            {
                                if (productionCoreConsoleAsset == _listOfCoreConsoleAsset[i])
                                    CoreConsoleEditorUtility.DrawSettingsEditor(_listOfCoreConsoleAsset[0], null, ref _isFoldOut[i], ref _editorForCoreConsoleAsset[i],1);
                                else if (productionCoreConsoleAsset != _listOfCoreConsoleAsset[i])
                                    CoreConsoleEditorUtility.DrawSettingsEditor(_listOfCoreConsoleAsset[i], null, ref _isFoldOut[i], ref _editorForCoreConsoleAsset[i],1);
                            }
                            EditorGUILayout.EndVertical();
                            
                        }
                    }

                }
                EditorGUILayout.EndScrollView();
            }
        }
        #endregion
    }
}



﻿namespace com.faith.coreconsole
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
        private static bool _isFoldOutOtherCoreConsoleAsset;


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

        private static bool IsAnyCoreConsoleAssetUsedByCoreConsoleManager()
        {

            List<CoreConsoleConfiguretionFile> gameConfiguratorAssets = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
            foreach (CoreConsoleConfiguretionFile gameConfigAsset in gameConfiguratorAssets)
            {

                if (gameConfigAsset.EditorAccessIfUsedByCentralCoreConsole)
                    return true;
            }
            CoreConsole.LogError("Please assign any of your 'CoreConsoleAsset' to 'CoreConsoleManager'", prefix: "CoreConsoleAsset");
            return false;
        }

        private static void SetLinkStatusWithCentralCoreConsole(bool statusFlag)
        {

            if (IsAnyCoreConsoleAssetUsedByCoreConsoleManager())
            {

                List<CoreConsoleConfiguretionFile> gameConfiguratorAssets = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();

                foreach (CoreConsoleConfiguretionFile gameConfigAsset in gameConfiguratorAssets)
                {
                    if (!gameConfigAsset.EditorAccessIfUsedByCentralCoreConsole)
                    {
                        SerializedObject serializedCoreConsoleAsset = new SerializedObject(gameConfigAsset);

                        SerializedProperty _linkWithCentralCoreConsole = serializedCoreConsoleAsset.FindProperty("_linkWithCentralCoreConsole");
                        _linkWithCentralCoreConsole.boolValue = statusFlag;
                        _linkWithCentralCoreConsole.serializedObject.ApplyModifiedProperties();

                        serializedCoreConsoleAsset.ApplyModifiedProperties();
                    }
                }
            }
        }

        private static void UpdateListOfCoreConsoleAsset()
        {

            _listOfCoreConsoleAsset = CoreConsoleEditorUtility.GetAsset<CoreConsoleConfiguretionFile>();
            _numberOfCoreConsoleAsset = _listOfCoreConsoleAsset.Count;

            //Marking   :   Central Game Configuretor Asset
            if (productionCoreConsoleAsset == null)
            {

                foreach (CoreConsoleConfiguretionFile gameConfiguratorAsset in _listOfCoreConsoleAsset)
                {
                    if (gameConfiguratorAsset.EditorAccessIfUsedByCentralCoreConsole)
                        productionCoreConsoleAsset = gameConfiguratorAsset;
                }
            }

            foreach (CoreConsoleConfiguretionFile gameConfiguratorAsset in _listOfCoreConsoleAsset)
            {
                //if : It is the central game configuretor asset but not matched with the cashed production asset. Remove It From Prodcution
                if (gameConfiguratorAsset.EditorAccessIfUsedByCentralCoreConsole && productionCoreConsoleAsset != gameConfiguratorAsset)
                {

                    SerializedObject serializedCoreConsoleAsset = new SerializedObject(gameConfiguratorAsset);

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

            AssetDatabase.CreateAsset(newCoreConsoleAsset, CoreConsoleConstants.DirectoryForCoreConsoleConfiguretionFile + "/" + absoluteName + ".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = newCoreConsoleAsset;

            UpdateListOfCoreConsoleAsset();
        }

        #endregion

        #region EditorWindow

        [MenuItem("FAITH/CoreConsole/ControlPanel", priority = 0)]
        public static void ShowWindow()
        {

            UpdateListOfCoreConsoleAsset();

            EditorWindow = GetWindow<CoreConsoleViewrEditorWindow>("CoreConsole Viwer", typeof(CoreConsoleViewrEditorWindow));

            EditorWindow.minSize = new Vector2(450f, 240f);
            EditorWindow.Show();
        }

        [MenuItem("FAITH/CoreConsole/Use Production Settings", priority = 1)]
        public static void LinkWithProductionCoreConsole()
        {

            SetLinkStatusWithCentralCoreConsole(true);
        }

        [MenuItem("FAITH/CoreConsole/Use Standalone Settings", priority = 1)]
        public static void UnlinkWithProductionCoreConsole()
        {
            SetLinkStatusWithCentralCoreConsole(false);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            HeighlightedBackgroundWithBoldStyle = new GUIStyle { normal = { background = Texture2D.whiteTexture }, fontStyle = FontStyle.Bold };
            _nameOfConfiguretorFile = _defaultName;

            UpdateListOfCoreConsoleAsset();
        }

        public void OnGUI()
        {

            HeaderGUI();

            if (productionCoreConsoleAsset == null)
            {
                EditorGUILayout.HelpBox("Please assign at least one 'CoreConsoleAsset' to 'CoreConsoleManager' in order configure through 'ControlPanel'", MessageType.Error);
            }
            else
            {
                CoreConsoleGUI();
            }
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

                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.LabelField("GameConfig  :   Production", HeighlightedBackgroundWithBoldStyle);
                GUI.backgroundColor = defaultBackgroundColor;

                EditorGUILayout.Space();
                CoreConsoleEditorUtility.DrawSettingsEditor(productionCoreConsoleAsset, null, ref _isFoldOut[0], ref _editorForCoreConsoleAsset[0]);

                if (_numberOfCoreConsoleAsset > 1)
                {

                    CoreConsoleEditorUtility.DrawHorizontalLine();
                    GUI.contentColor = Color.cyan;
                    _isFoldOutOtherCoreConsoleAsset = EditorGUILayout.Foldout(
                            _isFoldOutOtherCoreConsoleAsset,
                            "GameConfig  :   Others",
                            true
                        );
                    GUI.contentColor = defaultContentColor;
                }


                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                {
                    for (int i = 1; i < _numberOfCoreConsoleAsset; i++)
                    {
                        if (_isFoldOutOtherCoreConsoleAsset)
                        {
                            if (productionCoreConsoleAsset == _listOfCoreConsoleAsset[i])
                                CoreConsoleEditorUtility.DrawSettingsEditor(_listOfCoreConsoleAsset[0], null, ref _isFoldOut[i], ref _editorForCoreConsoleAsset[i]);
                            else if (productionCoreConsoleAsset != _listOfCoreConsoleAsset[i])
                                CoreConsoleEditorUtility.DrawSettingsEditor(_listOfCoreConsoleAsset[i], null, ref _isFoldOut[i], ref _editorForCoreConsoleAsset[i]);




                            if (i < _numberOfCoreConsoleAsset - 1)
                            {
                                EditorGUILayout.Space();
                                CoreConsoleEditorUtility.DrawHorizontalLine();
                                EditorGUILayout.Space();
                            }
                        }
                    }

                }
                EditorGUILayout.EndScrollView();
            }
        }
        #endregion
    }
}



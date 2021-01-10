﻿namespace com.faith.coreconsole
{
    using UnityEngine;
    using System.IO;
    using System.Text;

    [DefaultExecutionOrder(CoreConsoleConstants.EXECUTION_ORDER_FOR_CONFIGUREION_FILE_CONTAINER)]
    public class CoreConsoleConfiguretionFileContainer
    {
        #region Private Variables

        private static CoreConsoleConfiguretionFile[] _arrayOfConfiguretionFile;

        #endregion


        #region Public Callback

#if UNITY_EDITOR

        public static void GenerateEnum()
        {
            FetchCoreConsoleConfiguretionFile();

            string nameSpace = "com.faith.coreconsole";
            string nameOfEnum = "ConfiguretionFileID";
            string code = "";
            string path = "";

            //Defining  :   Path
            if (UnityEditor.AssetDatabase.FindAssets(nameSpace, new string[] { "Packages" }).Length > 0)
            {
                path = string.Format("Packages/{0}/Runtime", nameSpace);
            }
            else {
                path = "Assets";
                //path = string.Format("Assets/{0}/Runtime", nameSpace);
            }

            //Filtering :   EnumNames
            int numberOfEnumValue = _arrayOfConfiguretionFile.Length;
            string[] filtererdNameOfEnum = new string[numberOfEnumValue];

            for (int i = 0; i < numberOfEnumValue; i++) {

                string fileName     = _arrayOfConfiguretionFile[i].name;
                byte[] asciiValue   = Encoding.ASCII.GetBytes(fileName);

                filtererdNameOfEnum[i] = "";
                int lengthOfFile = fileName.Length;
                for (int j = 0; j < lengthOfFile; j++) {

                    if ((asciiValue[j] >= 65 && asciiValue[j] <= 90)
                     || (asciiValue[j] >= 97 && asciiValue[j] <= 122)
                     || (asciiValue[j] >= 48 && asciiValue[j] <= 57)
                     || asciiValue[j] == 95) {

                        filtererdNameOfEnum[i] += fileName[j];
                    }
                }
            }

            //Check :   Duplication
            for (int i = 0; i < numberOfEnumValue - 1; i++) {

                for (int j = i + 1; j < numberOfEnumValue; j++) {

                    if (string.Equals(filtererdNameOfEnum[i], filtererdNameOfEnum[j])) {

                        bool result = UnityEditor.EditorUtility.DisplayDialog(
                                "Failed to generate enum",
                                string.Format("Configuretion files '{0} : {1}' <> '{2} : {3}' are having same name as enum, please rename your configuretion file to resolve this issue and press 'Refresh' from the 'Control Panel'",
                                _arrayOfConfiguretionFile[i].name,
                                filtererdNameOfEnum[i],
                                _arrayOfConfiguretionFile[j].name,
                                filtererdNameOfEnum[j]),
                                "Ok"
                            );
                        return;
                    }
                }
            }

            code += "namespace " + nameSpace + "\n{\n\t";
            code += "public enum  " + nameOfEnum + "\n\t{\n";


            for (int i = 0; i < numberOfEnumValue; i++)
            {
                code += "\t\t"+ filtererdNameOfEnum[i] + ",\n";
            }
            code += "\t\tnone";

            code += "\n\t}\n}";

            Debug.Log("Path = " + path);
            Debug.Log("Code:\n" + code);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(code);
            }

            UnityEditor.AssetDatabase.Refresh();
        }

#endif

        [RuntimeInitializeOnLoadMethod()]
        public static void FetchCoreConsoleConfiguretionFile()
        {
            _arrayOfConfiguretionFile = Resources.LoadAll<CoreConsoleConfiguretionFile>("ConfiguretionFile");
        }


        public static CoreConsoleConfiguretionFile GetConfiguretionFile(ConfiguretionFileID configuretionFileID) {

            int indexValue = (int)configuretionFileID;
            if (indexValue >= 0 && indexValue < _arrayOfConfiguretionFile.Length) {

                return _arrayOfConfiguretionFile[indexValue];
            }

            CoreConsole.LogError(string.Format("Invalid Enum = {0} : ID = {1}", configuretionFileID, indexValue));

            return null;
        }

        #endregion

    }
}

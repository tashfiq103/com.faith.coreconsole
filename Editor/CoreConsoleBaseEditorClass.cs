
namespace com.faith.coreconsole
{
    using UnityEditor;

    internal class CoreConsoleBaseEditorClass : Editor
    {
        protected static CoreConsoleEnums.PackageStatus _packageStatus = CoreConsoleEnums.PackageStatus.InDevelopment;
        protected float singleLineHeight;

        #region OnEditor

        public virtual void OnEnable()
        {
            if (_packageStatus == CoreConsoleEnums.PackageStatus.InDevelopment)
                _packageStatus = AssetDatabase.FindAssets("com.faith.core", new string[] { "Packages" }).Length > 0 ? CoreConsoleEnums.PackageStatus.Production : CoreConsoleEnums.PackageStatus.InDevelopment;

            singleLineHeight = EditorGUIUtility.singleLineHeight;

        }

        #endregion
    }
}


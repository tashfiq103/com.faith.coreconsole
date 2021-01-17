namespace com.faith.coreconsole
{
    using UnityEditor;

    internal class CoreConsoleBaseEditorWindowClass : EditorWindow
    {
        protected static CoreConsoleEnums.PackageStatus _packageStatus = CoreConsoleEnums.PackageStatus.InDevelopment;

        #region OnEditor

        public virtual void OnEnable()
        {
            if (_packageStatus == CoreConsoleEnums.PackageStatus.InDevelopment)
                _packageStatus = AssetDatabase.FindAssets("com.faith.core", new string[] { "Packages" }).Length > 0 ? CoreConsoleEnums.PackageStatus.Production : CoreConsoleEnums.PackageStatus.InDevelopment;

        }

        #endregion
    }
}

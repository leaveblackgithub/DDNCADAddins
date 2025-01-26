using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CADAddins.Archive
{
    internal class O_CadHelper
    {
        private O_DocHelper _curDocHelper;

        private O_CadHelper()
        {
        }

        public static O_DocHelper CurDocHelper =>
            GetInstance()._curDocHelper ?? (GetInstance()._curDocHelper = new O_DocHelper(Application.DocumentManager.MdiActiveDocument));


        public static O_CadHelper GetInstance()
        {
            return InnerInstance.Instance;
        }


        public static bool SetSystemVariable(string name, object value)
        {
            if (Application.GetSystemVariable(name).ToString() == value.ToString()) return false;
            Application.SetSystemVariable(name, value);
            return true;
        }

        public static void Quit()
        {
            GetInstance()._curDocHelper.Dispose();
            GetInstance()._curDocHelper = null;
        }
        private class InnerInstance
        {
            internal static O_CadHelper Instance = new O_CadHelper();

            /// <summary>
            ///     当一个类有静态构造函数时，它的静态成员变量不会被beforefieldinit修饰
            ///     就会确保在被引用的时候才会实例化，而不是程序启动的时候实例化
            /// </summary>
            static InnerInstance()
            {
            }
        }
    }
}
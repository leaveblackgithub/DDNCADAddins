using Autodesk.AutoCAD.ApplicationServices.Core;

namespace DDNCADAddins.Environments
{
    internal class CadHelper
    {
        private DocHelper _curDocHelper;

        private CadHelper()
        {
        }

        public static DocHelper CurDocHelper =>
            GetInstance()._curDocHelper ?? (GetInstance()._curDocHelper =
                new DocHelper(Application.DocumentManager.MdiActiveDocument));


        public static CadHelper GetInstance()
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
            internal static readonly CadHelper Instance = new CadHelper();

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
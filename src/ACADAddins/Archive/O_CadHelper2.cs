using Autodesk.AutoCAD.ApplicationServices.Core;

namespace ACADAddins.Archive
{
    internal class O_CadHelper2
    {
        private O_DocHelper2 _curODocHelper2;

        private O_CadHelper2()
        {
        }

        public static O_DocHelper2 CurODocHelper2 =>
            GetInstance()._curODocHelper2 ?? (GetInstance()._curODocHelper2 =
                new O_DocHelper2(Application.DocumentManager.MdiActiveDocument));


        public static O_CadHelper2 GetInstance()
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
            GetInstance()._curODocHelper2.Dispose();
            GetInstance()._curODocHelper2 = null;
        }

        private class InnerInstance
        {
            internal static readonly O_CadHelper2 Instance = new O_CadHelper2();

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
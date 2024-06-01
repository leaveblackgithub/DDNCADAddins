using System;

namespace CommonUtils.Misc
{
    /// <summary>
    /// 静态内部类单例模式，线程安全
    /// </summary>
    public class LazySingleton
    {
        private static readonly Lazy<LazySingleton> _instance =
            new Lazy<LazySingleton>(() => new LazySingleton());

        public static LazySingleton Instance
        {
            get { return _instance.Value; }
        }
    }
}
using System;

namespace CADAddins.General
{
    public abstract class DisposableClass : IDisposable
    {
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); //标记gc不在调用析构函数
        }
        ~DisposableClass()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                DisposeManaged();
                //TODO:释放本对象中管理的托管资源
            }

            DisposeUnManaged();
            //TODO:释放非托管资源
            _disposed = true;
        }

        protected abstract void DisposeUnManaged();

        protected abstract void DisposeManaged();
    }
}
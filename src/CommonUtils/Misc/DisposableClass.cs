using System;

namespace CommonUtils.Misc
{
    public abstract class DisposableClass : IDisposable
    {
        //是否回收完毕
        private bool _disposed;

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
            if (disposing) DisposeManaged();
            DisposeUnManaged();
            _disposed = true;
        }

        protected abstract void DisposeUnManaged();

        protected abstract void DisposeManaged();
    }
}
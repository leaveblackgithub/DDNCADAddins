using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACADTests
{
    public interface ICommand
    {
        void Execute();
    }

    public static class CommandExcution
    {
        public static void ExecuteCommand<T>() where T : ICommand
        {
            try
            {
                var cmd = Activator.CreateInstance<T>();
                cmd.Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
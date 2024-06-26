using OnlineJournal.MVVM;
using System;
using System.Windows;

namespace OnlineJournal.Servises
{
    public class WindowService<T> : IWindowService<T>
    {
        public void OpenWindow(Type windowType, T dataContext)
        {
            Window newWindow = (Window)Activator.CreateInstance(windowType);
            newWindow.DataContext = dataContext;
            newWindow.ShowDialog();
        }

        public void OpenWindowShutdownOnClosing(Type windowType, T dataContext)
        {
            Window newWindow = (Window)Activator.CreateInstance(windowType);
            newWindow.DataContext = dataContext;
            newWindow.Closing += (dataContext as ViewModelBase).CloseApplication;
            newWindow.ShowDialog();
        }
    }
}

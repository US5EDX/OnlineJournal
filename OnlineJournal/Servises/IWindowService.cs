using System;

namespace OnlineJournal.Servises
{
    public interface IWindowService<T>
    {
        void OpenWindow(Type windowType, T dataContext);
        void OpenWindowShutdownOnClosing(Type windowType, T dataContext);
    }
}

namespace Ionix.Utils
{
    using System;


    public interface IPrototype<out T>
    {
        T Copy();
    }

    public interface ILockable
    {
        void Lock();
        void Unlock();
        bool IsLocked { get; }
    }
}

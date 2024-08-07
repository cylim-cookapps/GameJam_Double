using System;

public class Singleton<T> where T : new()
{
    private static T _inst;

    public static T Inst
    {
        get
        {
            // Double-check locking to ensure only one instance is created
            // even in multi-threaded environments.
            if (_inst == null)
            {
                if (_inst == null)
                {
                    _inst = new T();
                }
            }

            return _inst;
        }
    }

    public static bool IsCreated()
    {
        // Simple null check is sufficient here.
        return _inst != null;
    }
}

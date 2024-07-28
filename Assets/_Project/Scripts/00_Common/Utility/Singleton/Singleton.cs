namespace Pxp
{
    public abstract class Singleton<T> where T : new()
    {
        private static T _inst;

        private static readonly object SyncRoot = new();

        public static T Inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (SyncRoot)
                    {
                        if (_inst == null)
                        {
                            _inst = new T();
                        }
                    }
                }
                return _inst;
            }
        }

        static Singleton()
        {
        }

        ~Singleton()
        {
            _inst = default;
        }

        protected Singleton() { }

        public static bool IsCreated()
        {
            return _inst != null;
        }
    }
}

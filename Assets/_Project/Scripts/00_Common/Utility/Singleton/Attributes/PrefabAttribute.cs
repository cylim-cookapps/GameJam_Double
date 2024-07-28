using System;

namespace Pxp
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrefabAttribute : Attribute
    {
        public readonly string Name;

        public PrefabAttribute(string name)
        {
            int index = name.LastIndexOf('.');
            ReadOnlySpan<char> span = name.AsSpan();
            ReadOnlySpan<char> spanName = span[++index..];

            Name = spanName.ToString();
        }

        public PrefabAttribute()
        {
            Name = null;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NotAutoSingletonAttribute : Attribute
    {
    }
}

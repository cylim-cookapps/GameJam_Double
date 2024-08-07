using UnityEngine;

public abstract class BoardBase<T>:MonoBehaviour
{
    public virtual void OnInitialize(T parent)
    {
        Parent = parent;
    }
    public abstract void Show();
    public abstract void Hide();
    public abstract T Parent { get; protected set; }
}

using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Stack<T> _stack = new();

    public ObjectPool(T prefab, int prewarm, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < prewarm; i++)
            _stack.Push(Create());
    }

    public T Get()
    {
        if (_stack.Count > 0)
        {
            var item = _stack.Pop();
            item.gameObject.SetActive(true);
            return item;
        }
        return Create();
    }

    public void Release(T item)
    {
        item.gameObject.SetActive(false);
        _stack.Push(item);
    }

    private T Create()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        return obj;
    }
}

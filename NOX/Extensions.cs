using UnityEngine;

namespace NOX;

internal static class Extensions
{
    extension(GameObject go)
    {
        public GameObject GetChildByName(string name)
        {
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                var ch = go.transform.GetChild(i).gameObject;
                if (ch.name == name) return ch;
            }
            return null;
        }
        public T GetChildComponentByName<T>(string name)
        {
            return go.GetChildByName(name).GetComponent<T>();
        }
    }

    extension(Component go)
    {
        public GameObject GetChildByName(string name)
        {
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                var ch = go.transform.GetChild(i).gameObject;
                if (ch.name == name) return ch;
            }
            return null;
        }
        public T GetChildComponentByName<T>(string name)
        {
            return go.GetChildByName(name).GetComponent<T>();
        }

        public T CreateChildObjectWithComponent<T>(string name) where T : Component
        {
            return CreateObjectWithComponent<T>(name, go);
        }
    }

    public static T CreateObjectWithComponent<T>(string name) where T : Component
    {
        var go = new GameObject(name);
        return go.AddComponent<T>();
    }

    public static T CreateObjectWithComponent<T>(string name, Component parent) where T : Component
    {
        var obj = CreateObjectWithComponent<T>(name);
        obj.transform.SetParent(parent.transform, false);
        return obj;
    }

    public static T CreateObjectWithComponent<T>(string name, GameObject parent) where T : Component
    {
        var obj = CreateObjectWithComponent<T>(name);
        obj.transform.SetParent(parent.transform, false);
        return obj;
    }
}
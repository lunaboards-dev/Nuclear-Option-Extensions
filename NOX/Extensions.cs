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
}
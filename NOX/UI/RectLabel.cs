using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI;
// this class exists because i'm lazy
public class RectLabel : MonoBehaviour
{
    public Text text;
    public RectTransform rect;

    void Awake()
    {
        text = gameObject.AddComponent<Text>();
        rect = gameObject.GetComponent<RectTransform>();
    }
}
using UnityEngine;

namespace NOX.RWRs;

class RWRManager : MonoBehaviour
{
    IRWRDisplay RWR;
    GameObject root;
    AudioSource source;
    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable()
    {
        RWR.Init();
    }

    void Update()
    {
        RWR.Update();
    }
    
    void OnDisable()
    {
        RWR.Destroy();
    }
}
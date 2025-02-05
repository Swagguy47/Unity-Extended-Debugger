using UnityEngine;

//  initiates the framework & creates the canvas gameobject

namespace ExtendedDebugFramework
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    static class EDF_Manager
    {
        static bool initialized;
        static GameObject canvas;

#if UNITY_EDITOR
        static EDF_Manager()
        {
            Initialize();
        }
#endif
#if UNITY_STANDALONE
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static void Initialize()
        {
            //  prevent canvas creation in editor outside of playmode
            if (!Application.isPlaying) return;

            //  create canvas
            canvas = GameObject.Instantiate(Resources.Load("ExtDebugCanvas", typeof(GameObject)) as GameObject);
            //  make persistent across all scenes
            Object.DontDestroyOnLoad(canvas);
            EDF_Manager.initialized = (Application.isEditor | Debug.isDebugBuild) & canvas;
            //  just to be safe
            if (!initialized & canvas)
                GameObject.Destroy(canvas);
        }
    }
}

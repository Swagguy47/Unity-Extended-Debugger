using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace ExtendedDebugFramework
{
    //  creates a backup eventsystem to use if one does not already exist
    public class EDF_EventSystem : MonoBehaviour
    {
        EventSystem thisSystem;
#if ENABLE_INPUT_SYSTEM
        InputSystemUIInputModule thisInput;
#endif

        //  sceneload callback
        private void Start()
        {
            SceneManager.sceneLoaded += OnNewScene;
        }

        void OnNewScene(Scene scene, LoadSceneMode loadSceneMode)
        {
            OnEnable();
        }

        //  expensive
        void OnEnable()
        {
            var oneExists = GameObject.FindAnyObjectByType<EventSystem>();

            if (oneExists & !thisSystem)
                return;

            //  destroy systems
            if(oneExists & thisSystem)
            {
#if ENABLE_INPUT_SYSTEM
                //  failsafe for XR projects
                if (!thisInput) thisInput = GetComponent<InputSystemUIInputModule>();
                Destroy(thisInput);
#endif
                Destroy(thisSystem);
            }

            //  create input/event systems
            if (!oneExists & !thisSystem)
            {
                thisSystem = gameObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
                if (!GameObject.FindAnyObjectByType<InputSystemUIInputModule>())
                    thisInput = gameObject.AddComponent<InputSystemUIInputModule>();
#endif
            }
        }
    }
}

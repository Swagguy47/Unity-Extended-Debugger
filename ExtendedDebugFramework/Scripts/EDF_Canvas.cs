using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ExtendedDebugFramework
{
    public class EDF_Canvas : MonoBehaviour
    {
        [ReadOnly] [SerializeField] public Transform[] widgetAnchors;
        [ReadOnly] [SerializeField] GameObject genericLabel;
        [ReadOnly] public static bool consoleActive;

        //  universal callback to self
        public static EDF_Canvas self;

        //  cursor state prior to ui opening
        [HideInInspector] public bool cursorActive;

        void Awake()
        {
            self = this;
            //  initilization popup
            CreateLabel("<color=yellow><b>Debug options avaliable</b></color>", EDF_Globals.EDF_WidgetAnchor.TopCenter, 3);
        }

        //  ------------------------------------
        //  console widget toggling
        void Update()
        {
            //  new input system
            #if ENABLE_INPUT_SYSTEM
            if(Keyboard.current.backquoteKey.wasPressedThisFrame)
                ToggleConsoleWidget();

            //  legacy input
            #else
            if (Input.GetKeyDown(KeyCode.BackQuote) | Input.GetKeyDown(KeyCode.Tilde))
                ToggleConsoleWidget();
            #endif
        }

        void ToggleConsoleWidget()
        {
            var widgetParent = widgetAnchors[9].gameObject;

            //  toggle cursor if needed
            ToggleCursor(widgetParent);

            widgetParent.SetActive(!widgetParent.activeSelf);

            //  clear input, helps prevent typing ` when toggling
            EDF_Console.self.consoleInput.text = string.Empty;

            //  static bool for use in external scripts, like player controllers
            consoleActive = widgetParent.activeSelf;

            if (!consoleActive)
                return;
            
            EDF_Console.self.consoleInput.Select();
            EDF_Console.self.consoleInput.ActivateInputField();
        }

        void ToggleCursor(GameObject widgetParent)
        {
            bool newState;

            //  enabling
            if (!widgetParent.activeSelf) { // enabling canvas
                cursorActive = Cursor.visible;
                newState = true;
            }
            else   //   disabling canvas
                newState = cursorActive & widgetParent.activeSelf;

            Cursor.visible = newState;
            Cursor.lockState = newState ? CursorLockMode.None : CursorLockMode.Locked;
        }
        //  ------------------------------------


        //  ==================
        //  GENERIC FUNCTIONS:
        //  ==================

        //  -----------------------
        //  Label
        public static GameObject CreateLabel(string text, EDF_Globals.EDF_WidgetAnchor anchor)
        {
            GameObject widget = CreateWidget(self.genericLabel, anchor);

            widget.GetComponent<Text>().text = text;
            return widget;
        }

        public static GameObject CreateLabel(string text, EDF_Globals.EDF_WidgetAnchor anchor, float time)
        {
            GameObject widget = CreateWidget(self.genericLabel, anchor);

            widget.GetComponent<Text>().text = text;
            widget.AddComponent<EDFWidget_Temporary>().delay = time;
            return widget;
        }

        public static GameObject CreateLabel(string text, EDF_Globals.EDF_WidgetAnchor anchor, float time, GameObject customWidget)
        {
            GameObject widget = CreateWidget(customWidget, anchor);

            widget.GetComponent<Text>().text = text;
            widget.AddComponent<EDFWidget_Temporary>().delay = time;
            return widget;
        }

        public static GameObject CreateLabel(string text, EDF_Globals.EDF_WidgetAnchor anchor, GameObject customWidget)
        {
            GameObject widget = CreateWidget(customWidget, anchor);

            widget.GetComponent<Text>().text = text;
            return widget;
        }
        //  -----------------------

        public static GameObject CreateWidget(GameObject widget, EDF_Globals.EDF_WidgetAnchor anchor)
        {
            return Instantiate(widget, self.widgetAnchors[(int)anchor], false);
        }
    }
}

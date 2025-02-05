using UnityEngine;
using UnityEngine.UI;

namespace ExtendedDebugFramework
{
    //  universal input for EDF_ActionGUI
    public class EDF_GUIInput : MonoBehaviour
    {
        //  event actions managed by ActionGUI
        [HideInInspector] public string Input;
        public Text label;

        public virtual void OnInput(string var)
        {
            Input = var;
        }

        public virtual void OnInput(bool var)
        {
            Input = var.ToString();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtendedDebugFramework
{
    //  TODO: refactor
    //  button for opening GUI pages & executing commands
    public class EDF_GUIButton : MonoBehaviour
    {
        public Text label;
        public Transform inputRoot;
        [HideInInspector] public string command;
        [HideInInspector] public List<EDF_GUIInput> inputs = new();
        //  for pages
        [HideInInspector] public GameObject pageToggle;

        public void RunAction()
        {
            //  opens page
            if (pageToggle)
                EDF_ActionGUI.self.ShowPage(pageToggle);
            //  runs command
            else {
                var cmd = command;
                //  get args
                for(int i = 0; i < inputs.Count; i++)
                {
                    cmd += " " + inputs[i].Input;
                }
                EDF_Commands.RunCommand(cmd);
            }
        }
    }
}

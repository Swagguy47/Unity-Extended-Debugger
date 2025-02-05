using System;
using UnityEngine;
using UnityEngine.Events;

namespace ExtendedDebugFramework
{
    //  list of commands to be registered in scriptableobject form
    //  after creating one, place it in any 'Resources' folder for them to be loaded
    [CreateAssetMenu(fileName = "CmdList", menuName = "Debug Framework/Command List", order = 1)]
    public class EDF_CommandList : ScriptableObject
    {
        [Serializable]
        public struct EDF_Command
        {
            public string command, description;
            public UnityEvent<String[]> action;
            [Tooltip("(Optional) - Parameters that can be sent to your method as a string[]. Make sure to setup the action with dynamic input.")]
            public EDF_Globals.EDF_CmdArg[] arguments;
            [Header("Show this command as a UI button?")]
            [Tooltip("(Optional) - Display this command as a button to the left of the console for easy usage")]
            public bool ShowGUI;
            //  Deprecated
            /*[Header("Type out as a file path for submenus")]
            public string GUIPath;*/
        }

        public EDF_Command[] commands;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ExtendedDebugFramework
{
    //  Display command suggestions that match what is being typed
    //  & fill command textbox when pressing tab
    [RequireComponent(typeof(Text))]
    public class EDF_Autofill : MonoBehaviour
    {
        List<string> parsedCommands = new();
        List<string> canidates = new();

        Text txt;
        int selectedCanidate;

        private void Start()
        {
            txt = GetComponent<Text>();
            ParseCommands();
            EDF_Commands.commandsRegistered += ParseCommands;
        }

        private void Update()
        {
            //  -------------------------
            //          Autofill
            //  -------------------------

            //  new input system
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current.tabKey.wasPressedThisFrame)
                AutoComplete();

            //  legacy input
#else
            if (Input.GetKeyDown(KeyCode.Tab))
                AutoComplete();
#endif
            //  -------------------------
            //      Scroll Canidates
            //  -------------------------

            //  new input system
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current.leftAltKey.wasPressedThisFrame)
                ScrollCanidates();

            //  legacy input
#else
            if (Input.GetKeyDown(KeyCode.LeftAlt))
                ScrollCanidates();
#endif
        }

        //  move & loop around command results for tab autofill
        void ScrollCanidates()
        {
            selectedCanidate = selectedCanidate == canidates.Count -1 ? 0 : selectedCanidate+=1;
            SetText();
        }

        //  press tab to autofill textbox with final result
        //  only if one result exists
        void AutoComplete()
        {
            EDF_Console.self.consoleInput.text = canidates[selectedCanidate];
            EDF_Console.self.consoleInput.caretPosition = canidates[selectedCanidate].Length;
        }

        //  make list of all commands as strings
        //  TODO: make this listen to EDF_Commands.RegisterCommands to allow runtime reregistering
        void ParseCommands()
        {
            parsedCommands.Clear();
            foreach (var cmd in EDF_Commands.commands)
                if(!parsedCommands.Contains(cmd.command))
                    parsedCommands.Add(cmd.command);
        }

        //  when the console input is changed
        public void OnEdit()
        {
            if (EDF_Console.self.consoleInput.text == string.Empty)
            {
                Clear();
                return;
            }

            var input = EDF_Console.self.consoleInput.text;

            switch(input.Contains(" "))
            {
                case true: { HandleArgument(input); break; }
                case false: { HandleCommand(input); break; }
            }
        }

        //  display autofill results for commands
        void HandleCommand(string input)
        {
            selectedCanidate = 0;
            canidates.Clear();
            canidates = parsedCommands.FindAll(w => w.StartsWith(input));

            SetText();
            //txt.color = canidates[0] == input ? Color.yellow : Color.cyan;
        }

        //  create text & colors
        void SetText()
        {
            txt.text = string.Empty;
            for(int i = 0; i < canidates.Count; i++)
                txt.text += "\n" + (i == selectedCanidate ? "<color=lime>" : "<color=cyan>") + canidates[i] + "</color>";

            if (canidates.Count == 0)
                return;
        }

        //  display autofill results for arguments
        void HandleArgument(string input)
        {
            var splitInput = input.Split(' ');

            //  get current command
            EDF_Globals.EDF_CmdArg[] args = null;
            for(int i = 0; i < parsedCommands.Count; i++)
            {
                var twinargs = EDF_Commands.commands[i].arguments.Length >= splitInput.Length - 1;
                if (parsedCommands[i] == splitInput[0] & twinargs) {
                    args = EDF_Commands.commands[i].arguments;
                    break;
                }
            }

            //  stop if no arguments exist
            if (args == null)
                return;
            if (args.Length == 0)
                return;

            //  setup
            txt.color = Color.cyan;
            var currentArg = splitInput.Length - 2;

            //  prevent errors if too many arguments are inputted
            if (currentArg >= args.Length)
                return;

            //  prep argument strings
            var argType = args[currentArg].arg.ToString();
            var argHelper = ArgHelper(argType);

            //  set text
            txt.text = args[currentArg].name == string.Empty ? 
                argType.Substring(1, argType.Length -1) : 
                args[currentArg].name + argHelper;
        }

        //  extra appended string after argument name
        string ArgHelper(string type)
        {
            switch (type)
            {
                case "_Bool": return " (True/False)";
                default: return "";
            }
        }

        //  also called when done editing
        public void Clear()
        {
            txt.text = string.Empty;
        }
    }
}

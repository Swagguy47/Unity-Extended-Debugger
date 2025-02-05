using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ExtendedDebugFramework
{
    public class EDF_Commands : MonoBehaviour
    {
        [ReadOnly] public static List<EDF_CommandList.EDF_Command> commands = new();
        static List<EDF_CommandList.EDF_Command> runtimeCommands = new();

        [ReadOnly] static List<string> cmdHistory = new List<string>();
        [ReadOnly] static int historyIndex = 0;

        public delegate void RegisterCallback();
        public static event RegisterCallback commandsRegistered;


        private void Awake()
        {
            RegisterCommands();
        }

        //  find all commandlists in Resources
        //  TODO:
        //  Runtime re-register doesn't work
        public static void RegisterCommands()
        {
            commands.Clear();

            try
            {
                // This is the short hand version and requires that you include the "using System.Linq;" at the top of the file.
                var commandLists = Resources.LoadAll("", typeof(EDF_CommandList)).Cast<EDF_CommandList>();
                foreach (var list in commandLists)
                {
                    foreach(EDF_CommandList.EDF_Command cmd in list.commands)
                        commands.Add(cmd);
                    
                    Resources.UnloadAsset(list);
                }
                // Casts each individual UnityEngine.Object into UnityEngine.GameObject and adds it to an actual list of GameObject type. 
            }
            catch (Exception e)
            {
                Debug.LogWarning("Commands failed to register with this exception:");
                Debug.LogWarning(e);
            }

            //  add runtime commands
            foreach (EDF_CommandList.EDF_Command cmd in runtimeCommands)
                commands.Add(cmd);

            //Debug.Log("count: " + commands.Count + " rand: " + UnityEngine.Random.Range(1000, 9999));

            //  prevent callback if there are no listeners
            if (commandsRegistered == null) return;

            commandsRegistered.DynamicInvoke();
        }

        public static void RunCommand(string command)
        {
            if (command == "")
                return;

            //  add to history
            cmdHistory.Add(command);
            historyIndex = cmdHistory.Count;
            //  reset input
            EDF_Console.self.consoleInput.text = "";
            //  refocus
            EDF_Console.self.consoleInput.Select();
            EDF_Console.self.consoleInput.ActivateInputField();

            //  display executed command in console
            EDF_Console.Space();
            EDF_Console.AddLine("<color=lime>"+ command +"</color>");
            EDF_Console.Space();

            //  setup variables
            string[] lastCommand = command.Split(" ");
            command = "";
            bool ran = false;

            //  check for commands of identical name (useful for having multiple versions with different arguments)
            var twinCheck = -1;

            //  check if command is valid
            for(int i = 0; i < commands.Count; i++)
            {
                if (lastCommand[0] == commands[i].command)  //  is valid
                {
                    //  check if enough arguments / too many are provided
                    if (lastCommand.Length - 1 != commands[i].arguments.Length)  
                    {
                        twinCheck = i;
                        continue;
                    }

                    //  parse args
                    string[] arguments = new string[lastCommand.Length - 1];
                    for(int l = 0; l < arguments.Length; l++)
                    {
                        arguments[l] = lastCommand[l + 1];
                    }

                    //  invoke command action
                    commands[i].action.Invoke(arguments);
                    ran = true;
                    break;
                }
            }
            if(twinCheck > -1 & !ran)
                ArgumentSyntaxError(lastCommand, twinCheck);

            if (!ran)
                SyntaxError(command, lastCommand);
        }

        //  console error message for if you use the wrong amount of arguments
        static void ArgumentSyntaxError(string[] lastCommand, int i)
        {
            EDF_Console.AddDivider("-");
            EDF_Console.AddLine("<color=red>Argument mismatch\n'" + lastCommand[0] + "' expects " + commands[i].arguments.Length + " argument(s)!</color>");
            
            //  get all argument names/types(if name is empty) as a string
            var allArgs = "";
            for (int l = 0; l < commands[i].arguments.Length; l++) {
                allArgs += " ";
                var argName = commands[i].arguments[l].name;
                var argType = commands[i].arguments[l].arg.ToString();
                allArgs += argName == string.Empty ? argType.Substring(1, argType.Length -1) : argName;
            }

            //  show correct usage of the command
            EDF_Console.Space();
            EDF_Console.AddLine("<color=red>Correct usage: '<i>" + commands[i].command + allArgs + "</i>'</color>");
            EDF_Console.AddDivider("-");
        }

        //  generic invalid command syntax error console message
        static public void SyntaxError(string cmd, string[] lastCommand)
        {
            string fullcmd = "";
            for (int i = 0; i < lastCommand.Length; i++)
                fullcmd += lastCommand[i] + " ";

            EDF_Console.AddDivider("-");
            EDF_Console.AddLine("<color=red>'" + fullcmd + "' is not a valid command</color>");
            EDF_Console.AddDivider("-");
            HelpPrompt();
        }

        static public void HelpPrompt()
        {
            EDF_Console.AddLine("? for commands");
        }

        //  ------------------------------------
        //  Traveling up/down command history

        void Update()
        {
            if (!EDF_Console.self.consoleInput.isFocused) return;

            //  new input system
            #if ENABLE_INPUT_SYSTEM
            if(Keyboard.current.upArrowKey.wasPressedThisFrame)
                IncrimentCmdHistory(1);
            else if(Keyboard.current.downArrowKey.wasPressedThisFrame)
                IncrimentCmdHistory(-1);

            //  legacy input
            #else
            if (Input.GetKeyDown(KeyCode.UpArrow))
                IncrimentCmdHistory(1);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                IncrimentCmdHistory(-1);
            #endif
        }

        void IncrimentCmdHistory(int amount)
        {
            if (historyIndex - amount < 0 | historyIndex - amount > cmdHistory.Count - 1) return;
            
            historyIndex -= amount;
            EDF_Console.self.consoleInput.text = cmdHistory[historyIndex];
        }

        //  ------------------------------------

        //  External functions

        //  add a new command at runtime & reregister
        public static void AddCommand(EDF_CommandList.EDF_Command command)
        {
            runtimeCommands.Add(command);

            RegisterCommands(); 
        }

        //  removes specific runtime command & reregister
        public static void RemoveRuntimeCommand(EDF_CommandList.EDF_Command command)
        {
            runtimeCommands.Remove(command);

            RegisterCommands();
        }

        //  clears all runtime commands & reregister
        public static void ClearRuntimeCommands()
        {
            runtimeCommands.Clear();

            RegisterCommands();
        }

        //  slightly easier way to make commands?
        public static EDF_CommandList.EDF_Command MakeCommand(string command, EDF_Globals.EDF_CmdArg[] arguments, UnityEngine.Events.UnityEvent<string[]> action)
        {
            var cmd = new EDF_CommandList.EDF_Command();
            cmd.command = command;
            //  failsafe for arguments == null
            EDF_Globals.EDF_CmdArg[] args = { };
            cmd.arguments = arguments == null ? args : arguments;
            cmd.description = string.Empty;
            cmd.action = action;
            cmd.ShowGUI = true;

            return cmd;
        }

        public static EDF_CommandList.EDF_Command MakeCommand(string command, EDF_Globals.EDF_CmdArg[] arguments, UnityEngine.Events.UnityEvent<string[]> action, string description, bool createButton)
        {
            var cmd = new EDF_CommandList.EDF_Command();
            cmd.command = command;
            //  failsafe for arguments == null
            EDF_Globals.EDF_CmdArg[] args = { };
            cmd.arguments = arguments == null ? args : arguments;
            cmd.description = description;
            cmd.action = action;
            cmd.ShowGUI = createButton;

            return cmd;
        }

        public static EDF_CommandList.EDF_Command MakeCommand(string command, EDF_Globals.EDF_CmdArg[] arguments, UnityEngine.Events.UnityEvent<string[]> action, string description, bool createButton, bool autoRegister)
        {
            var cmd = new EDF_CommandList.EDF_Command();
            cmd.command = command;
            //  failsafe for arguments == null
            EDF_Globals.EDF_CmdArg[] args = { };
            cmd.arguments = arguments == null ? args : arguments;
            cmd.description = description;
            cmd.action = action;
            cmd.ShowGUI = createButton;

            if (autoRegister)
                AddCommand(cmd);

            return cmd;
        }
    }
}

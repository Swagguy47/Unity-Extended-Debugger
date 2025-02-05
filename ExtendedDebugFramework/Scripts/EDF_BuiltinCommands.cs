using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExtendedDebugFramework
{
    public class EDF_BuiltinCommands : MonoBehaviour
    {
        private void Start()
        {
            if(PlayerPrefs.HasKey("EDF_ScreenLog"))
                Application.logMessageReceived += UnityLog;
        }

        //  list all registered commands, their arguments & descriptions
        public void Cmd_Help()
        {
            EDF_Console.AddDivider("-");
            for(int i = 0; i < EDF_Commands.commands.Count; i++)
            {
                string cmd = EDF_Commands.commands[i].command;

                cmd += "<color=lightblue>";
                for(int l = 0; l < EDF_Commands.commands[i].arguments.Length; l++)
                {
                    var argString = EDF_Commands.commands[i].arguments[l].arg.ToString();
                    cmd += " [" + argString.Substring(1, argString.Length - 1) + "] ";
                }
                cmd += "        </color><color=silver><i>" + EDF_Commands.commands[i].description + "</i></color>";

                EDF_Console.AddLine(cmd);
            }
            EDF_Console.AddDivider("-");
        }

        //  delete all widgets from every anchor
        public void Cmd_ClearWidgets()
        {
            for(int i = 0; i < 9; i++)
            {
                foreach(Transform child in EDF_Canvas.self.widgetAnchors[i])
                    Destroy(child.gameObject);
            }
        }

        //  fps counter widget
        public void Cmd_ToggleFps()
        {
            var counter = GameObject.FindAnyObjectByType<EDFWidget_FpsCounter>();
            if (counter) {
                Destroy(counter.gameObject);
                return;
            }

            EDF_Canvas.CreateWidget(Resources.Load<GameObject>("Widgets/FpsCounter"), EDF_Globals.EDF_WidgetAnchor.TopRight);
        }

        //  camera position widget
        public void Cmd_ToggleCamPos()
        {
            var pos = GameObject.FindAnyObjectByType<EDFWidget_CamPos>();
            if (pos)
            {
                Destroy(pos.gameObject);
                return;
            }

            EDF_Canvas.CreateWidget(Resources.Load<GameObject>("Widgets/CamPos"), EDF_Globals.EDF_WidgetAnchor.TopRight);
        }

        //  -----------------------------
        //  unity logs as widgets
        public void Cmd_VisualLog()
        {
            string prefKey = "EDF_ScreenLog";
            if (PlayerPrefs.HasKey(prefKey)) {
                PlayerPrefs.DeleteKey(prefKey);
                Application.logMessageReceived -= UnityLog;
            }
            else {
                PlayerPrefs.SetInt(prefKey, 1);
                Application.logMessageReceived += UnityLog;
            }
            PlayerPrefs.Save();

            EDF_Console.AddLine("Screenlog set to: " + PlayerPrefs.HasKey(prefKey));
        }

        void UnityLog(string condition, string stackTrace, LogType type)
        {
            GameObject label = EDF_Canvas.CreateLabel("<color=" + EDF_Console.GetLogColor(type) + ">(log) " + condition + "</color>", EDF_Globals.EDF_WidgetAnchor.TopLeft, 7, Resources.Load<GameObject>("Widgets/ScreenLog"));
            label.transform.SetAsFirstSibling();
        }
        //  -----------------------------

        //  reload game
        public void Cmd_Restart()
        {
            SceneManager.LoadScene(0);
        }

        //  reload active scene
        public void Cmd_Reset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //  load scene from index or name
        public void Cmd_SceneLoad(string[] vars)
        {
            bool additive = bool.Parse(vars[1]);

            int test;
            if (int.TryParse(vars[0], out test))
                SceneManager.LoadScene(test, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);     //  index
            else
                SceneManager.LoadScene(vars[0], additive ? LoadSceneMode.Additive : LoadSceneMode.Single);  //  name
        }

        //  short-hand non additive version of the above method
        public void Cmd_SceneLoadShort(string[] vars)
        {
            int test;
            if (int.TryParse(vars[0], out test))
                SceneManager.LoadScene(test);     //  index
            else
                SceneManager.LoadScene(vars[0]);  //  name
        }

        //  clear in-game console
        public void Cmd_Clear()
        {
            EDF_Console.ClearLines();
        }

        // instantiate object from resources folder
        public void Cmd_LoadResource(string[] vars)
        {
            Instantiate(Resources.Load<GameObject>(vars[0]), Camera.main.transform.position, Quaternion.identity, null);
        }

        //  create widgets displaying build info
        // mostly useful for autoexec
        public void Cmd_BuildInfo()
        {
            EDF_Canvas.CreateLabel(Application.isEditor ? "No Build GUID" : Application.buildGUID, EDF_Globals.EDF_WidgetAnchor.TopCenter);
            EDF_Canvas.CreateLabel(Application.productName + " " + Application.version, EDF_Globals.EDF_WidgetAnchor.TopCenter);
            EDF_Canvas.CreateLabel(Environment.UserName + " on " + Application.platform.ToString(), EDF_Globals.EDF_WidgetAnchor.BotCenter);
            EDF_Canvas.CreateLabel(DateTime.Now.ToString(), EDF_Globals.EDF_WidgetAnchor.BotCenter);
        }

        //  reregister commands
        public void Cmd_Reregister()
        {
            EDF_Commands.RegisterCommands();
        }

        //  clear runtime commands
        public void Cmd_ClearRuntimeCmd()
        {
            EDF_Commands.ClearRuntimeCommands();
        }

        //  cursor toggle
        public void Cmd_ToggleCursor()
        {
            EDF_Canvas.self.cursorActive = !EDF_Canvas.self.cursorActive;
            EDF_Console.AddLine(EDF_Canvas.self.cursorActive.ToString());
        }

        //  close application
        public void Cmd_Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }

        //  list all scenes in edfConsole
        public void Cmd_SceneList()
        {
            EDF_Console.AddDivider("-");
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                EDF_Console.AddLine(i + " | " + SceneUtility.GetScenePathByBuildIndex(i));// SceneManager.GetSceneByBuildIndex(i).name);
            EDF_Console.AddDivider("-");
        }
    }
}

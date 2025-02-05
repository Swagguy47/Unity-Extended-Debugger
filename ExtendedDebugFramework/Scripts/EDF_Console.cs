using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ExtendedDebugFramework
{
    public class EDF_Console : MonoBehaviour
    {
        [ReadOnly] public InputField consoleInput;
        [ReadOnly][SerializeField] Text consoleTxt;
        [ReadOnly][SerializeField] Transform guiRoot;
        [ReadOnly][SerializeField] GameObject guiButton;

        public static EDF_Console self;

        void Awake()
        {
            self = this;
            Application.logMessageReceived += ParseUnityLog;
        }

        //  ------------------------------------
        //  Intercepting Unity Logs
        
        void ParseUnityLog(string condition, string stackTrace, LogType type)
        {
            AddLine("<color=" + GetLogColor(type) + ">(log) " + condition + "</color>"); 
        }

        public static string GetLogColor(LogType type)
        {
            string color = string.Empty;
            
            switch (type)
            {
                case LogType.Log: color = "white"; break;
                case LogType.Warning: color = "yellow"; break;
                case LogType.Error: color = "red"; break;
                case LogType.Exception: color = "magenta"; break;
                case LogType.Assert: color = "cyan"; break;
            }

            return color;
        }
        //  ------------------------------------

        //  ==================
        //  GENERIC FUNCTIONS:
        //  ==================

        public static void Space()
        {
            AddLine(string.Empty);
        }

        public static void Space(int lines)
        {
            for (int i = 0; i < lines; i++)
                Space();
        }

        public static void AddLine(string addition)
        {
            self.consoleTxt.text += "\n" + addition;
            //  truncate text if it exceeds the character threshold to prevent text mesh generation errors
            var threshold = 5000;
            var offset = self.consoleTxt.text.Length - threshold;
            if (offset <= 0)
                return;

            //  split by line to help prevent rich text errors
            var consoleLines = self.consoleTxt.text.Split('\n');
            var lineOffset = 0;
            for(int i = 0; i < consoleLines.Length; i++)
            {
                if (lineOffset >= offset)
                    break;

                lineOffset += consoleLines[i].Length;
            }

            self.consoleTxt.text = self.consoleTxt.text.Substring(lineOffset, self.consoleTxt.text.Length - lineOffset);
        }

        public static void AddDivider(string character)
        {
            AddDivider(character, 25);
        }

        public static void AddDivider(string character, int length)
        {
            string line = "";
            for (int i = 0; i < length; i++)
            {
                line += character;
            }

            AddLine(line);
        }

        public static void ClearLines()
        {
            self.consoleTxt.text = string.Empty;
        }
    }
}

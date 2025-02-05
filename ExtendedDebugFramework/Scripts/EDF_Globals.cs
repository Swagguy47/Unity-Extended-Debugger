using System;

namespace ExtendedDebugFramework
{
    public static class EDF_Globals
    {
        public enum EDF_WidgetAnchor { 
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MidLeft = 3,
            MidCenter = 4,
            MidRight = 5,
            BotLeft = 6,
            BotCenter = 7,
            BotRight = 8,
            ConsoleOverlay = 9
        }

        [Serializable]
        public struct EDF_CmdArg {
            public string name;
            public EDF_CmdArgType arg;
            public bool required;
        }

        public enum EDF_CmdArgType { 
            _Bool = 0,
            _Float = 1,
            _Int = 2,
            _String = 4
        }
    }
}

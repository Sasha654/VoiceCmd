using System;

namespace VoiceCmd
{
    public class Command
    {
        public Action Action { get; set; }
        public string CommandText { get; set; }
        public bool CommandEnable { get; set; }
    }
}

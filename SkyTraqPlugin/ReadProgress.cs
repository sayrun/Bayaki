using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyTraqPlugin
{
    internal delegate void ReadProgressEventHandler(ReadProgressEvent progress);

    internal class ReadProgressEvent
    {
        public enum READ_PHASE
        {
            UNSTART,    // 開始前
            INIT,       // 初期化
            READ,       // 読み出し
            CONVERT,    // 変換
            RESTART     // 初期化
        };

        public readonly READ_PHASE Phase;
        public readonly int Value;
        public readonly int Max;

        public ReadProgressEvent(READ_PHASE phase, int value, int max)
        {
            Phase = phase;
            Value = value;
            Max = max;
        }

        public string PhaseName
        {
            get
            {
                switch(Phase)
                {
                    case READ_PHASE.UNSTART:
                        return Properties.Resources.MSG17;
                    case READ_PHASE.INIT:
                        return Properties.Resources.MSG18;
                    case READ_PHASE.READ:
                        return Properties.Resources.MSG19;
                    case READ_PHASE.CONVERT:
                        return Properties.Resources.MSG20;
                    case READ_PHASE.RESTART:
                        return Properties.Resources.MSG21;
                    default:
                        return string.Empty;
                }
            }
        }
    }
}

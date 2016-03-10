using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            RESTERT     // 初期化
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
                        return "開始前";
                    case READ_PHASE.INIT:
                        return "初期化処理";
                    case READ_PHASE.READ:
                        return "読出処理";
                    case READ_PHASE.CONVERT:
                        return "変換処理";
                    case READ_PHASE.RESTERT:
                        return "再起動";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}

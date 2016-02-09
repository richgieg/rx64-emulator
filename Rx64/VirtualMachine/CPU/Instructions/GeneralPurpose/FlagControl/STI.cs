using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void STI()   // opcode: 0xfb
        {
            SetFlag(FlagsEnum.IF_InterruptEnableFlag);
        }
    }
}
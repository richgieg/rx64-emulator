using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void STD()   // opcode: 0xfd
        {
            SetFlag(FlagsEnum.DF_DirectionFlag);
        }
    }
}
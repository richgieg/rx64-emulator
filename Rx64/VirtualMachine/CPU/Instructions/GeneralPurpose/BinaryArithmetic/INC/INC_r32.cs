using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void INC_r32(byte regcode)   // opcode: 40+rd
        {
            uint value;
            value = IncDwordAndSetFlags(GetReg32(regcode));
            SetReg32(regcode, value);
        }
    }
}
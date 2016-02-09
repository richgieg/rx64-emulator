using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {
        private void INC_r16(byte regcode)   // opcode: 40+rw
        {
            ushort value;
            value = IncWordAndSetFlags(GetReg16(regcode));
            SetReg16(regcode, value);
        }
    }
}
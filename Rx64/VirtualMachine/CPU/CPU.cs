using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rx64
{
    public partial class CPU
    {

        #region Enum Declarations


        public enum AddressSizeEnum
        {
            Bits16, Bits32, Bits64
        }

        public enum OperandSizeEnum
        {
            Bits16, Bits32
        }

        public enum OpModeEnum
        {
            RealAddressMode,
            ProtectedMode,
            IA32e_CompatibilityMode,
            IA32e_64bitMode
        }

        private enum RegMemTypeEnum
        {
            Register,
            Memory
        }

        public enum SegmentEnum
        {
            CS,
            DS,
            ES,
            FS,
            GS,
            SS
        }

        public enum FlagsEnum : ulong
        {
                                                    //  Pos   	Abbr	Name                        Type		Notes
                                                    //---------------------------------------------------------------------------------
            CF_CarryFlag = 1,                       //  0       CF	    Carry Flag			        Status
            __Reserved_01 = 2,                      //  1	    --  	Reserved			        N/A		    Should be set
            PF_ParityFlag = 4,                      //  2	    PF	    Parity Flag			        Status
            __Reserved_03 = 8,                      //  3	    --  	Reserved			        N/A		    Should not be set
            AF_AuxCarryFlag = 16,                   //  4	    AF  	Auxiliary Carry Flag	    Status
            __Reserved_05 = 32,                     //  5	    --  	Reserved			        N/A		    Should not be set
            ZF_ZeroFlag = 64,                       //  6	    ZF  	Zero Flag			        Status
            SF_SignFlag = 128,                      //  7 	    SF	    Sign Flag			        Status
            TF_TrapFlag = 256,                      //  8 	    TF	    Trap Flag			        System
            IF_InterruptEnableFlag = 512,           //  9	    IF	    Interrupt Enable Flag		System
            DF_DirectionFlag = 1024,                //  10	    DF	    Direction Flag			    Control
            OF_OverflowFlag = 2048,                 //  11	    OF	    Overflow Flag			    Status
            IOPL_IOPrivilegeLevelBit0 = 4096,       //  12-13	IOPL	I/O Privilege Level		    System
            IOPL_IOPrivilegeLevelBit1 = 8192,
            NT_NestedTask = 16384,                  //  14	    NT	    Nested Task			        System
            __Reserved_15 = 32768,                  //  15	    --	    Reserved			        N/A		    Should not be set
            RF_ResumeFlag = 65536,                  //  16	    RF	    Resume Flag			        System
            VM_Virtual8086Mode = 131072,            //  17	    VM	    Virtual-8086 Mode		    System
            AC_AlignmentCheck = 262144,             //  18	    AC	    Alignment Check			    System
            VIF_VirtualInterruptFlag = 524288,      //  19	    VIF	    Virtual Interrupt Flag		System
            VIP_VirtualInterruptPending = 1048576,  //  20	    VIP	    Virutal Interrupt Pending	System
            ID_IDFlag = 2097152                     //  21	    ID	    ID Flag				        System
                                                    //  22-31   --	    Reserved			        N/A		    Should not be set
        }

        private enum ExceptionEnum
        {                                       
                                                //  # Description                       RealMode        Virtual8086     8086
                                                //-----------------------------------------------------------------------------------
            DivideError,                        //  0 Divide Error (#DE)                Yes             Yes             Yes
            DebugException,                     //  1 Debug Exception (#DB)             Yes             Yes             No
            NMIInterrupt,                       //  2 NMI Interrupt                     Yes             Yes             Yes
            Breakpoint,                         //  3 Breakpoint (#BP)                  Yes             Yes             Yes
            Overflow,                           //  4 Overflow (#OF)                    Yes             Yes             Yes
            BoundRangeExceeded,                 //  5 BOUND Range Exceeded (#BR)        Yes             Yes             Reserved
            InvalidOpcode,                      //  6 Invalid Opcode (#UD)              Yes             Yes             Reserved
            DeviceNotAvailable,                 //  7 Device Not Available (#NM)        Yes             Yes             Reserved
            DoubleFault,                        //  8 Double Fault (#DF)                Yes             Yes             Reserved
            Vector9_Reserved_DoNotUse,          //  9 (Intel reserved. Do not use.)     Reserved        Reserved        Reserved
            InvalidTSS,                         //  10 Invalid TSS (#TS)                Reserved        Yes             Reserved
            SegmentNotPresent,                  //  11 Segment Not Present (#NP)        Reserved        Yes             Reserved
            StackFault,                         //  12 Stack Fault (#SS)                Yes             Yes             Reserved
            GeneralProtection,                  //  13 General Protection (#GP)*        Yes             Yes             Reserved
            PageFault,                          //  14 Page Fault (#PF)                 Reserved        Yes             Reserved
            Vector15_Reserved_DoNotUse,         //  15 (Intel reserved. Do not use.)    Reserved        Reserved        Reserved
            FloatingPointError,                 //  16 Floating-Point Error (#MF)       Yes             Yes             Reserved
            AlignmentCheck,                     //  17 Alignment Check (#AC)            Reserved        Yes             Reserved
            MachineCheck                        //  18 Machine Check (#MC)              Yes             Yes             Reserved
                                                //  19-31 (Intel reserved. Do not use.) Reserved        Reserved        Reserved
                                                //  32-255 User Defined Interrupts      Yes             Yes             Yes

            // * In the real-address mode, vector 13 is the segment overrun exception. In protected and virtual-
            // 8086 modes, this exception covers all general-protection error conditions, including traps
            // to the virtual-8086 monitor from virtual-8086 mode.
        }

        enum ExceptionDetailEnum
        {
            Null,
            AttemptedToLoadCS,
            InstructionPointerSegmentOverrun,
            InvalidModValueInModRM
        }


        #endregion
        
        #region Struct Declarations


        private struct ModRM_Byte
        {
            public byte ActualByte;
            public uint ModValue;
            public uint RegOpValue;
            public RegMemTypeEnum RegMemType;
            public uint RegMemValue;
            public ulong EffectiveAddress;
            public SegmentEnum DefaultSegment;
        }

        private struct SIB_Byte
        {
            public uint SS;
            public uint Index;
            public uint Base;
            public uint SIB_Ptr;
        }

        private struct MemoryOffset
        {
            public ulong Offset;
            public int Length;
        }

        private struct SegmentOverrideStruct
        {
            public bool Active;
            public SegmentEnum Value;
        }



        #endregion

        #region Registers


        // RAX, EAX, AX, AH, AL
        private ulong rax = 0;
        private uint eax
        {
            get
            {
                return (uint)rax;
            }
            set
            {
                rax = (rax & 0xffffffff00000000) | value;
            }
        }
        private ushort ax
        {
            get
            {
                return (ushort)rax;
            }
            set
            {
                rax = (rax & 0xffffffffffff0000) | value;
            }
        }
        private byte ah
        {
            get
            {
                return (byte)(rax >> 8);
            }
            set
            {
                rax = (rax & 0xffffffffffff00ff) | (ushort)(value << 8);
            }
        }
        private byte al
        {
            get
            {
                return (byte)(rax);
            }
            set
            {
                rax = (rax & 0xffffffffffffff00) | value;
            }
        }
        public ulong R64_RAX
        {
            get
            {
                return rax;
            }
        }
        public uint R32_EAX
        {
            get { return eax; }
        }
        public ushort R16_AX
        {
            get { return ax; }
        }

        // RCX, ECX, CX, CH, CL
        private ulong rcx = 0;
        private uint ecx
        {
            get
            {
                return (uint)rcx;
            }
            set
            {
                rcx = (rcx & 0xffffffff00000000) | value;
            }
        }
        private ushort cx
        {
            get
            {
                return (ushort)rcx;
            }
            set
            {
                rcx = (rcx & 0xffffffffffff0000) | value;
            }
        }
        private byte ch
        {
            get
            {
                return (byte)(rcx >> 8);
            }
            set
            {
                rcx = (rcx & 0xffffffffffff00ff) | (ushort)(value << 8);
            }
        }
        private byte cl
        {
            get
            {
                return (byte)(rcx);
            }
            set
            {
                rcx = (rcx & 0xffffffffffffff00) | value;
            }
        }
        public ulong R64_RCX
        {
            get
            {
                return rcx;
            }
        }
        public uint R32_ECX
        {
            get { return ecx; }
        }
        public ushort R16_CX
        {
            get { return cx; }
        }

        // RDX, EDX, DX, DH, DL
        private ulong rdx = 0;
        private uint edx
        {
            get
            {
                return (uint)rdx;
            }
            set
            {
                rdx = (rdx & 0xffffffff00000000) | value;
            }
        }
        private ushort dx
        {
            get
            {
                return (ushort)rdx;
            }
            set
            {
                rdx = (rdx & 0xffffffffffff0000) | value;
            }
        }
        private byte dh
        {
            get
            {
                return (byte)(rdx >> 8);
            }
            set
            {
                rdx = (rdx & 0xffffffffffff00ff) | (ushort)(value << 8);
            }
        }
        private byte dl
        {
            get
            {
                return (byte)(rdx);
            }
            set
            {
                rdx = (rdx & 0xffffffffffffff00) | value;
            }
        }
        public ulong R64_RDX
        {
            get
            {
                return rdx;
            }
        }
        public uint R32_EDX
        {
            get { return edx; }
        }
        public ushort R16_DX
        {
            get { return dx; }
        }

        // RBX, EBX, BX, BH, BL
        private ulong rbx = 0;
        private uint ebx
        {
            get
            {
                return (uint)rbx;
            }
            set
            {
                rbx = (rbx & 0xffffffff00000000) | value;
            }
        }
        private ushort bx
        {
            get
            {
                return (ushort)rbx;
            }
            set
            {
                rbx = (rbx & 0xffffffffffff0000) | value;
            }
        }
        private byte bh
        {
            get
            {
                return (byte)(rbx >> 8);
            }
            set
            {
                rbx = (rbx & 0xffffffffffff00ff) | (ushort)(value << 8);
            }
        }
        private byte bl
        {
            get
            {
                return (byte)(rbx);
            }
            set
            {
                rbx = (rbx & 0xffffffffffffff00) | value;
            }
        }
        public ulong R64_RBX
        {
            get
            {
                return rbx;
            }
        }
        public uint R32_EBX
        {
            get { return ebx; }
        }
        public ushort R16_BX
        {
            get { return bx; }
        }

        // RSP, ESP, SP
        private ulong rsp = 0;
        private uint esp
        {
            get
            {
                return (uint)rsp;
            }
            set
            {
                rsp = (rsp & 0xffffffff00000000) | value;
            }
        }
        private ushort sp
        {
            get
            {
                return (ushort)rsp;
            }
            set
            {
                rsp = (rsp & 0xffffffffffff0000) | value;
            }
        }
        public ulong R64_RSP
        {
            get
            {
                return rsp;
            }
        }
        public uint R32_ESP
        {
            get { return esp; }
        }
        public ushort R16_SP
        {
            get { return sp; }
        }

        // RBP, EBP, BP
        private ulong rbp = 0;
        private uint ebp
        {
            get
            {
                return (uint)rbp;
            }
            set
            {
                rbp = (rbp & 0xffffffff00000000) | value;
            }
        }
        private ushort bp
        {
            get
            {
                return (ushort)rbp;
            }
            set
            {
                rbp = (rbp & 0xffffffffffff0000) | value;
            }
        }
        public ulong R64_RBP
        {
            get
            {
                return rbp;
            }
        }
        public uint R32_EBP
        {
            get { return ebp; }
        }
        public ushort R16_BP
        {
            get { return bp; }
        }

        // RSI, ESI, SI, SIL
        private ulong rsi = 0;
        private uint esi
        {
            get
            {
                return (uint)rsi;
            }
            set
            {
                rsi = (rsi & 0xffffffff00000000) | value;
            }
        }
        private ushort si
        {
            get
            {
                return (ushort)rsi;
            }
            set
            {
                rsi = (rsi & 0xffffffffffff0000) | value;
            }
        }
        private byte sil
        {
            get
            {
                return (byte)(rsi);
            }
            set
            {
                rsi = (rsi & 0xffffffffffffff00) | value;
            }
        }
        public ulong R64_RSI
        {
            get
            {
                return rsi;
            }
        }
        public uint R32_ESI
        {
            get { return esi; }
        }
        public ushort R16_SI
        {
            get { return si; }
        }

        // RDI, EDI, DI, DIL
        private ulong rdi = 0;
        private uint edi
        {
            get
            {
                return (uint)rdi;
            }
            set
            {
                rdi = (rdi & 0xffffffff00000000) | value;
            }
        }
        private ushort di
        {
            get
            {
                return (ushort)rdi;
            }
            set
            {
                rdi = (rdi & 0xffffffffffff0000) | value;
            }
        }
        private byte dil
        {
            get
            {
                return (byte)(rdi);
            }
            set
            {
                rdi = (rdi & 0xffffffffffffff00) | value;
            }
        }
        public ulong R64_RDI
        {
            get
            {
                return rdi;
            }
        }
        public uint R32_EDI
        {
            get { return edi; }
        }
        public ushort R16_DI
        {
            get { return di; }
        }

        // RIP, EIP, IP
        private ulong rip = 0;
        private uint eip
        {
            get
            {
                return (uint)rip;
            }
            set
            {
                rip = (rip & 0xffffffff00000000) | value;
            }
        }
        private ushort ip
        {
            get
            {
                return (ushort)rip;
            }
            set
            {
                rip = (rip & 0xffffffffffff0000) | value;
            }
        }
        public ulong R64_RIP
        {
            get
            {
                return rip;
            }
        }
        public uint R32_EIP
        {
            get { return eip; }
        }
        public ushort R16_IP
        {
            get { return ip; }
        }

        // CS - code segment
        private ushort cs = 0;
        public ushort R_CS
        {
            get
            {
                return cs;
            }
        }

        // DS - data segment
        private ushort ds = 0;
        public ushort R_DS
        {
            get
            {
                return ds;
            }
        }

        // SS - stack segment
        private ushort ss = 0;
        public ushort R_SS
        {
            get
            {
                return ss;
            }
        }

        // ES - general segment
        private ushort es = 0;
        public ushort R_ES
        {
            get
            {
                return es;
            }
        }

        // FS - general segment
        private ushort fs = 0;
        public ushort R_FS
        {
            get
            {
                return fs;
            }
        }

        // GS - general segment
        private ushort gs = 0;
        public ushort R_GS
        {
            get
            {
                return gs;
            }
        }


        #endregion

        #region Flags


        // RFLAGS, EFLAGS, FLAGS
        private ulong rflags = 0x2;
        private uint eflags
        {
            get
            {
                return (uint)rflags;
            }
            set
            {
                rflags = (rflags & 0xffffffff00000000) | value;
            }
        }
        private ushort flags
        {
            get
            {
                return (ushort)rflags;
            }
            set
            {
                rflags = (rflags & 0xffffffffffff0000) | value;
            }
        }
        public ulong R64_RFLAGS
        {
            get
            {
                return rflags;
            }
        }
        public uint R32_EFLAGS
        {
            get { return eflags; }
        }
        public ushort R16_FLAGS
        {
            get { return flags; }
        }
      

        #endregion

        #region Psuedo Registers


        private ushort old_ip;
        private uint old_eip;
        private ulong old_rip;

        private ushort temp_ip;
        private uint temp_eip;
        private ulong temp_rip;


        #endregion

        #region Misc Variables


        private OpModeEnum opMode = 0;
        public OpModeEnum OpMode
        {
            get
            {
                return opMode;
            }
        }

        private AddressSizeEnum addressSize = 0;
        public AddressSizeEnum AddressSize
        {
            get
            {
                return addressSize;
            }
        }

        private OperandSizeEnum operandSize = 0;
        public OperandSizeEnum OperandSize
        {
            get
            {
                return operandSize;
            }
        }

        private MemoryController mch;
        private Thread execution_thread;
        private AutoResetEvent execution_thread_auto_event;
        private bool singleStepping = true;
        public bool SingleStepping
        {
            get
            {
                return singleStepping;
            }
            set
            {
                if (value)
                {
                    singleStepping = true;
                    //if (execution_thread.IsAlive)
                        //execution_thread_auto_event.Set();
                }
                else
                {
                    singleStepping = false;
                    execution_thread_auto_event.Set();
                }
            }
        }

        private ushort executionDelay = 100;
        public ushort ExecutionDelay
        {
            get { return executionDelay; }
            set { executionDelay = value; }
        }

        private bool operandSizeOverride;
        private bool addressSizeOverride;
        private bool haltRequest = false;
        private bool maskA20 = true;                // masks A20 line - if seg = 0xffff and offset = 0xffff, effective address is 0xffef (instead of 0x10ffef)
        private int prefixLength;            
        private SegmentOverrideStruct segmentOverride;
        private bool jumpInstruction;
        private string[] exceptionDetailTable;

        #endregion
        
        #region Methods


        public CPU(MemoryController MCH, ulong InstructionPointer)
        {
            InitializeExceptionDetailTable();
            mch = MCH;
            SetOpMode(OpModeEnum.RealAddressMode);
            //SetOpMode(OpModeEnum.ProtectedMode);
            rip = InstructionPointer;

            execution_thread = new Thread(this.InitializeExecutionThread);
            execution_thread.Start();
        }

        private int Halt()
        {
            MessageBox.Show(Form1.ActiveForm, "CPU has been halted.");
            haltRequest = true;
            return 1;
        }

        public void Kill()
        {
            execution_thread.Abort();
        }

        public void Step()
        {
            if (singleStepping)
                if (execution_thread.IsAlive)
                    execution_thread_auto_event.Set();
        }

        private void InitializeExecutionThread()
        {
            execution_thread_auto_event = new AutoResetEvent(false);
            Cycle();
        }

        private void Cycle()
        {
            while (!haltRequest)
            {         
                if (singleStepping)
                {
                    execution_thread_auto_event.WaitOne();
                    ExecuteNextInstruction();
                }
                else
                {
                    if(executionDelay != 0)
                        Thread.Sleep(executionDelay);                  // delay execution

                    ExecuteNextInstruction();
                }
            }
        }

        #region ExecuteNextInstruction* Methods


        private void ExecuteNextInstruction()
        {
            operandSizeOverride = false;
            addressSizeOverride = false;
            segmentOverride.Active = false;
            jumpInstruction = false;

            // PREFIX DISCOVERY LOOP:
            // Instruction prefixes are divided into four groups, each with a set of allowable prefix
            // codes. For each instruction, it is only useful to include up to one prefix code from
            // each of the four groups (Groups 1, 2, 3, 4). Groups 1 through 4 may be placed in any
            // order relative to each other. Prefix can only be four bytes long.
            // - Group 1 - Lock and repeat prefixes
            // - Group 2 — Segment override prefixes
            // - Group 3 - Operand-size override prefix
            // - Group 4 - Address-size override prefix

            prefixLength = 0;
            for(int i = 0; i < 4; i++)          // prefix can only be four bytes long
            {
                switch(GetByte(SegmentEnum.CS, GetInstructionPointer(prefixLength)))
                {
                    case 0x26:
                        segmentOverride.Value = SegmentEnum.ES;
                        segmentOverride.Active = true;
                        prefixLength++;
                        break;

                    case 0x2e:
                        segmentOverride.Value = SegmentEnum.CS;
                        segmentOverride.Active = true;
                        prefixLength++;
                        break;

                    case 0x36:
                        segmentOverride.Value = SegmentEnum.SS;
                        segmentOverride.Active = true;
                        prefixLength++;
                        break;

                    case 0x3e:
                        segmentOverride.Value = SegmentEnum.DS;
                        segmentOverride.Active = true;
                        prefixLength++;
                        break;

                    case 0x64:
                        segmentOverride.Value = SegmentEnum.FS;
                        segmentOverride.Active = true;
                        prefixLength++;
                        break;

                    case 0x65:
                        segmentOverride.Value = SegmentEnum.GS;
                        segmentOverride.Active = true;
                        prefixLength++;
                        break;

                    case 0x66:
                        operandSizeOverride = true;
                        prefixLength++;
                        break;

                    case 0x67:
                        addressSizeOverride = true;
                        prefixLength++;
                        break;

                    default:
                        i = 4;                  // if we run into an invalid prefix, this will break out of the "for" loop
                        break;
                }
            }

            try
            {
                switch (OpMode)
                {
                    case OpModeEnum.RealAddressMode:
                        old_ip = ip;                                // used for displaying ip in exception messages
                        temp_ip = (ushort)(ip + prefixLength);      // advance temp_ip beyond the prefix bytes (if any)
                        if (ip > temp_ip)                           // if temp_ip wrapped before getting to the primary opcode, segment overrun occured
                            RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                        
                        ExecuteNextInstruction_RealAddressMode();   // execute the next instruction (opcode)

                        if (!jumpInstruction)                       // if previous instruction wasn't a JMP instruction, advance the instruction pointer
                        {
                            if (ip > temp_ip)                       // if temp_ip wrapped, segment overrun occured
                            {
                                ip = temp_ip;
                                RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                            }
                            else
                            {
                                ip = temp_ip;                       // if all is well, advance the real ip register
                            }
                        }
                        break;

                    case OpModeEnum.ProtectedMode:
                        old_eip = eip;
                        ExecuteNextInstruction_ProtectedMode();
                        break;

                    case OpModeEnum.IA32e_CompatibilityMode:
                        old_eip = eip;
                        ExecuteNextInstruction_IA32e_CompatibilityMode();
                        break;

                    default:                    // OpMode.IA32e_64bitMode:
                        old_rip = rip;
                        ExecuteNextInstruction_IA32e_64bitMode();
                        break;
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show(Form1.ActiveForm, e.Message);
                
                if(singleStepping == false)
                    Halt();
                // skip instruction
                // call Interrupt function
            }
        }

        private void ExecuteNextInstruction_RealAddressMode()
        {
            byte code = GetNextInstructionByte();
            byte code5msb = (byte)(code & 0xf8);                    // 5 most significant bits of opcode
            byte regcode = (byte)(code & 0x7);                     // 3 least significant bits of opcode (aka: reg code)
            byte regOp = 0;

            if (code5msb == 0x40)
            {
                if (operandSizeOverride) INC_r32(regcode); else INC_r16(regcode);
            }
            else if (code5msb == 0x48)
            {
                if (operandSizeOverride) DEC_r32(regcode); else DEC_r16(regcode);
            }
            else if (code5msb == 0x50)
            {
                if (operandSizeOverride) PUSH_r32(regcode); else PUSH_r16(regcode);
            }
            else if (code5msb == 0x58)
            {
                if (operandSizeOverride) POP_r32(regcode); else POP_r16(regcode);
            }
            else if (code5msb == 0xb0)
            {
                MOV_r8_imm8(regcode);
            }
            else if (code5msb == 0xb8)
            {
                if (operandSizeOverride) MOV_r32_imm32(regcode); else MOV_r16_imm16(regcode);
            }
            else
            {
                switch (code)
                {
                    case 0x00:
                        ADD_rm8_r8();
                        break;

                    case 0x01:
                        if (operandSizeOverride) ADD_rm32_r32(); else ADD_rm16_r16();
                        break;

                    case 0x02:
                        ADD_r8_rm8();
                        break;

                    case 0x03:
                        if (operandSizeOverride) ADD_r32_rm32(); else ADD_r16_rm16();
                        break;

                    case 0x04:
                        ADD_AL_imm8();
                        break;

                    case 0x05:
                        if (operandSizeOverride) ADD_EAX_imm32(); else ADD_AX_imm16();
                        break;

                    case 0x06:
                        if (operandSizeOverride) PUSH_ES_o32(); else PUSH_ES_o16();
                        break;

                    case 0x07:
                        if (operandSizeOverride) POP_ES_o32(); else POP_ES_o16();
                        break;

                    case 0x08:
                        OR_rm8_r8();
                        break;

                    case 0x09:
                        if (operandSizeOverride) OR_rm32_r32(); else OR_rm16_r16();
                        break;

                    case 0x0a:
                        OR_r8_rm8();
                        break;

                    case 0x0b:
                        if (operandSizeOverride) OR_r32_rm32(); else OR_r16_rm16();
                        break;

                    case 0x0c:
                        OR_AL_imm8();
                        break;

                    case 0x0d:
                        if (operandSizeOverride) OR_EAX_imm32(); else OR_AX_imm16();
                        break;

                    case 0x0e:
                        if (operandSizeOverride) PUSH_CS_o32(); else PUSH_CS_o16();
                        break;

                    case 0x10:
                        ADC_rm8_r8();
                        break;

                    case 0x11:
                        if (operandSizeOverride) ADC_rm32_r32(); else ADC_rm16_r16();
                        break;

                    case 0x12:
                        ADC_r8_rm8();
                        break;

                    case 0x13:
                        if (operandSizeOverride) ADC_r32_rm32(); else ADC_r16_rm16();
                        break;

                    case 0x14:
                        ADC_AL_imm8();
                        break;

                    case 0x15:
                        if (operandSizeOverride) ADC_EAX_imm32(); else ADC_AX_imm16();
                        break;

                    case 0x16:
                        if (operandSizeOverride) PUSH_SS_o32(); else PUSH_SS_o16();
                        break;

                    case 0x17:
                        if (operandSizeOverride) POP_SS_o32(); else POP_SS_o16();
                        break;

                    case 0x18:
                        SBB_rm8_r8();
                        break;

                    case 0x19:
                        if (operandSizeOverride) SBB_rm32_r32(); else SBB_rm16_r16();
                        break;

                    case 0x1a:
                        SBB_r8_rm8();
                        break;

                    case 0x1b:
                        if (operandSizeOverride) SBB_r32_rm32(); else SBB_r16_rm16();
                        break;

                    case 0x1c:
                        SBB_AL_imm8();
                        break;

                    case 0x1d:
                        if (operandSizeOverride) SBB_EAX_imm32(); else SBB_AX_imm16();
                        break;

                    case 0x1e:
                        if (operandSizeOverride) PUSH_DS_o32(); else PUSH_DS_o16();
                        break;

                    case 0x1f:
                        if (operandSizeOverride) POP_DS_o32(); else POP_DS_o16();
                        break;

                    case 0x20:
                        AND_rm8_r8();
                        break;

                    case 0x21:
                        if (operandSizeOverride) AND_rm32_r32(); else AND_rm16_r16();
                        break;

                    case 0x22:
                        AND_r8_rm8();
                        break;

                    case 0x23:
                        if (operandSizeOverride) AND_r32_rm32(); else AND_r16_rm16();
                        break;

                    case 0x24:
                        AND_AL_imm8();
                        break;

                    case 0x25:
                        if (operandSizeOverride) AND_EAX_imm32(); else AND_AX_imm16();
                        break;

                    case 0x28:
                        SUB_rm8_r8();
                        break;

                    case 0x29:
                        if (operandSizeOverride) SUB_rm32_r32(); else SUB_rm16_r16();
                        break;

                    case 0x2a:
                        SUB_r8_rm8();
                        break;

                    case 0x2b:
                        if (operandSizeOverride) SUB_r32_rm32(); else SUB_r16_rm16();
                        break;

                    case 0x2c:
                        SUB_AL_imm8();
                        break;

                    case 0x2d:
                        if (operandSizeOverride) SUB_EAX_imm32(); else SUB_AX_imm16();
                        break;

                    case 0x30:
                        XOR_rm8_r8();
                        break;

                    case 0x31:
                        if (operandSizeOverride) XOR_rm32_r32(); else XOR_rm16_r16();
                        break;

                    case 0x32:
                        XOR_r8_rm8();
                        break;

                    case 0x33:
                        if (operandSizeOverride) XOR_r32_rm32(); else XOR_r16_rm16();
                        break;

                    case 0x34:
                        XOR_AL_imm8();
                        break;

                    case 0x35:
                        if (operandSizeOverride) XOR_EAX_imm32(); else XOR_AX_imm16();
                        break;

                    case 0x38:
                        CMP_rm8_r8();
                        break;

                    case 0x39:
                        if (operandSizeOverride) CMP_rm32_r32(); else CMP_rm16_r16();
                        break;

                    case 0x3a:
                        CMP_r8_rm8();
                        break;

                    case 0x3b:
                        if (operandSizeOverride) CMP_r32_rm32(); else CMP_r16_rm16();
                        break;

                    case 0x3c:
                        CMP_AL_imm8();
                        break;

                    case 0x3d:
                        if (operandSizeOverride) CMP_EAX_imm32(); else CMP_AX_imm16();
                        break;

                    case 0x68:
                        if (operandSizeOverride) PUSH_imm32(); else PUSH_imm16();
                        break;

                    case 0x6a:
                        if (operandSizeOverride) PUSH_imm8_o32(); else PUSH_imm8_o16();
                        break;

                    case 0x70:
                        JO_rel8();
                        break;

                    case 0x71:
                        JNO_rel8();
                        break;

                    case 0x72:
                        JC_JB_JNAE_rel8();
                        break;

                    case 0x73:
                        JNC_JAE_JNB_rel8();
                        break;

                    case 0x74:
                        JE_JZ_rel8();
                        break;

                    case 0x75:
                        JNE_JNZ_rel8();
                        break;

                    case 0x76:
                        JBE_JNA_rel8();
                        break;

                    case 0x77:
                        JA_JNBE_rel8();
                        break;

                    case 0x78:
                        JS_rel8();
                        break;

                    case 0x79:
                        JNS_rel8();
                        break;

                    case 0x7a:
                        JP_JPE_rel8();
                        break;

                    case 0x7b:
                        JNP_JPO_rel8();
                        break;

                    case 0x7c:
                        JL_JNGE_rel8();
                        break;

                    case 0x7d:
                        JGE_JNL_rel8();
                        break;

                    case 0x7e:
                        JLE_JNG_rel8();
                        break;

                    case 0x7f:
                        JG_JNLE_rel8();
                        break;

                    case 0x80:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // 80 /0
                                ADD_rm8_imm8();
                                break;

                            case 1:     // 80 /1
                                OR_rm8_imm8();
                                break;

                            case 2:     // 80 /2
                                ADC_rm8_imm8();
                                break;

                            case 3:     // 80 /3
                                SBB_rm8_imm8();
                                break;

                            case 4:     // 80 /4
                                AND_rm8_imm8();
                                break;

                            case 5:     // 80 /5
                                SUB_rm8_imm8();
                                break;

                            case 6:     // 80 /6
                                XOR_rm8_imm8();
                                break;

                            case 7:     // 80 /7
                                CMP_rm8_imm8();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0x81:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // 81 /0
                                if (operandSizeOverride) ADD_rm32_imm32(); else ADD_rm16_imm16();
                                break;

                            case 1:     // 81 /1
                                if (operandSizeOverride) OR_rm32_imm32(); else OR_rm16_imm16();
                                break;

                            case 2:     // 81 /2
                                if (operandSizeOverride) ADC_rm32_imm32(); else ADC_rm16_imm16();
                                break;

                            case 3:     // 81 /3
                                if (operandSizeOverride) SBB_rm32_imm32(); else SBB_rm16_imm16();
                                break;

                            case 4:     // 81 /4
                                if (operandSizeOverride) AND_rm32_imm32(); else AND_rm16_imm16();
                                break;

                            case 5:     // 81 /5
                                if (operandSizeOverride) SUB_rm32_imm32(); else SUB_rm16_imm16();
                                break;

                            case 6:     // 81 /6
                                if (operandSizeOverride) XOR_rm32_imm32(); else XOR_rm16_imm16();
                                break;

                            case 7:     // 81 /7
                                if (operandSizeOverride) CMP_rm32_imm32(); else CMP_rm16_imm16();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0x83:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // 83 /0
                                if (operandSizeOverride) ADD_rm32_imm8(); else ADD_rm16_imm8();
                                break;

                            case 1:     // 83 /1
                                if (operandSizeOverride) OR_rm32_imm8(); else OR_rm16_imm8();
                                break;

                            case 2:     // 83 /2
                                if (operandSizeOverride) ADC_rm32_imm8(); else ADC_rm16_imm8();
                                break;

                            case 3:     // 83 /3
                                if (operandSizeOverride) SBB_rm32_imm8(); else SBB_rm16_imm8();
                                break;

                            case 4:     // 83 /4
                                if (operandSizeOverride) AND_rm32_imm8(); else AND_rm16_imm8();
                                break;

                            case 5:     // 83 /5
                                if (operandSizeOverride) SUB_rm32_imm8(); else SUB_rm16_imm8();
                                break;

                            case 6:     // 83 /6
                                if (operandSizeOverride) XOR_rm32_imm8(); else XOR_rm16_imm8();
                                break;

                            case 7:     // 83 /7
                                if (operandSizeOverride) CMP_rm32_imm8(); else CMP_rm16_imm8();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0x84:
                        TEST_rm8_r8();
                        break;

                    case 0x85:
                        if (operandSizeOverride) TEST_rm32_r32(); else TEST_rm16_r16();
                        break;

                    case 0x88:
                        MOV_rm8_r8();
                        break;

                    case 0x89:
                        if (operandSizeOverride) MOV_rm32_r32(); else MOV_rm16_r16();
                        break;

                    case 0x8a:
                        MOV_r8_rm8();
                        break;

                    case 0x8b:
                        if (operandSizeOverride) MOV_r32_rm32(); else MOV_r16_rm16();
                        break;

                    case 0x8c:
                        MOV_rm16_Sreg();
                        break;

                    case 0x8e:
                        MOV_Sreg_rm16();
                        break;

                    case 0x8f:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // 8F /0
                                if (operandSizeOverride) POP_rm32(); else POP_rm16();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0x9a:
                        if (operandSizeOverride) CALL_ptr16_32(); else CALL_ptr16_16();
                        break;

                    case 0xa0:
                        MOV_AL_moffs8();
                        break;

                    case 0xa1:
                        if (operandSizeOverride) MOV_EAX_moffs32(); else MOV_AX_moffs16();
                        break;

                    case 0xa2:
                        MOV_moffs8_AL();
                        break;

                    case 0xa3:
                        if (operandSizeOverride) MOV_moffs32_EAX(); else MOV_moffs16_AX();
                        break;

                    case 0xa8:
                        TEST_AL_imm8();
                        break;

                    case 0xa9:
                        if (operandSizeOverride) TEST_EAX_imm32(); else TEST_AX_imm16();
                        break;

                    case 0xc3:
                        if (operandSizeOverride) RET_o32(); else RET_o16();
                        break;

                    case 0xc6:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // C6 /0
                                MOV_rm8_imm8();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0xc7:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // C7 /0
                                if (operandSizeOverride) MOV_rm32_imm32(); else MOV_rm16_imm16();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0xcb:
                        if (operandSizeOverride) RET_FAR_o32(); else RET_FAR_o16();
                        break;

                    case 0xe0:
                        if (addressSizeOverride) LOOPNE_LOOPNZ_rel8_a32(); else LOOPNE_LOOPNZ_rel8_a16();
                        break;

                    case 0xe1:
                        if (addressSizeOverride) LOOPE_LOOPZ_rel8_a32(); else LOOPE_LOOPZ_rel8_a16();
                        break;

                    case 0xe2:
                        if (addressSizeOverride) LOOP_rel8_a32(); else LOOP_rel8_a16();
                        break;

                    case 0xe3:
                        if (addressSizeOverride) JECXZ_rel8(); else JCXZ_rel8();
                        break;

                    case 0xe8:
                        if (operandSizeOverride) CALL_rel32(); else CALL_rel16();
                        break;

                    case 0xe9:
                        if (operandSizeOverride) JMP_rel32(); else JMP_rel16();
                        break;

                    case 0xea:
                        if (operandSizeOverride) JMP_ptr16_32(); else JMP_ptr16_16();
                        break;

                    case 0xeb:
                        JMP_rel8();
                        break;

                    case 0xf4:
                        Halt();
                        break;

                    case 0xf5:
                        CMC();
                        break;

                    case 0xf6:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // F6 /0
                                TEST_rm8_imm8();
                                break;

                            case 2:     // F6 /2
                                NOT_rm8();
                                break;

                            case 3:     // F6 /3
                                NEG_rm8();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0xf7:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // F7 /0
                                if (operandSizeOverride) TEST_rm32_imm32(); else TEST_rm16_imm16();
                                break;

                            case 2:     // F7 /2
                                if (operandSizeOverride) NOT_rm32(); else NOT_rm16();
                                break;

                            case 3:     // F7 /3
                                if (operandSizeOverride) NEG_rm32(); else NEG_rm16();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0xf8:
                        CLC();
                        break;

                    case 0xf9:
                        STC();
                        break;

                    case 0xfa:
                        CLI();
                        break;

                    case 0xfb:
                        STI();
                        break;

                    case 0xfc:
                        CLD();
                        break;

                    case 0xfd:
                        STD();
                        break;

                    case 0xfe:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // FE /0
                                INC_rm8();
                                break;

                            case 1:     // FE /0
                                DEC_rm8();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0xff:
                        regOp = GetRegOpFieldFromModRM();
                        switch (regOp)
                        {
                            case 0:     // FF /0
                                if (operandSizeOverride) INC_rm32(); else INC_rm16();
                                break;

                            case 1:     // FF /1
                                if (operandSizeOverride) DEC_rm32(); else DEC_rm16();
                                break;

                            case 2:     // FF /2
                                if (operandSizeOverride) CALL_rm32(); else CALL_rm16();
                                break;

                            case 3:     // FF /3
                                if (operandSizeOverride) CALL_m16_32(); else CALL_m16_16();
                                break;

                            case 4:     // FF /4
                                if (operandSizeOverride) JMP_rm32(); else JMP_rm16();
                                break;

                            case 5:     // FF /5
                                if (operandSizeOverride) JMP_m16_32(); else JMP_m16_16();
                                break;

                            case 6:     // FF /6
                                if (operandSizeOverride) PUSH_rm32(); else PUSH_rm16();
                                break;

                            default:
                                OpcodeAndDigitNotYetSupported(code, regOp);
                                break;
                        }
                        break;

                    case 0x0f:                                                                  // escape opcode byte - first byte of two-byte opcode
                        code = GetNextInstructionByte();
                        //code5msb = (byte)(code & 0xf8);                                         // 5 most significant bits of opcode

                        switch (code)
                        {
                            case 0x80:
                                if (operandSizeOverride) JO_rel32(); else JO_rel16();
                                break;

                            case 0x81:
                                if (operandSizeOverride) JNO_rel32(); else JNO_rel16();
                                break;

                            case 0x82:
                                if (operandSizeOverride) JC_JB_JNAE_rel32(); else JC_JB_JNAE_rel16();
                                break;

                            case 0x83:
                                if (operandSizeOverride) JNC_JAE_JNB_rel32(); else JNC_JAE_JNB_rel16();
                                break;

                            case 0x84:
                                if (operandSizeOverride) JE_JZ_rel32(); else JE_JZ_rel16();
                                break;

                            case 0x85:
                                if (operandSizeOverride) JNE_JNZ_rel32(); else JNE_JNZ_rel16();
                                break;

                            case 0x86:
                                if (operandSizeOverride) JBE_JNA_rel32(); else JBE_JNA_rel16();
                                break;

                            case 0x87:
                                if (operandSizeOverride) JA_JNBE_rel32(); else JA_JNBE_rel16();
                                break;

                            case 0x88:
                                if (operandSizeOverride) JS_rel32(); else JS_rel16();
                                break;

                            case 0x89:
                                if (operandSizeOverride) JNS_rel32(); else JNS_rel16();
                                break;

                            case 0x8a:
                                if (operandSizeOverride) JP_JPE_rel32(); else JP_JPE_rel16();
                                break;

                            case 0x8b:
                                if (operandSizeOverride) JNP_JPO_rel32(); else JNP_JPO_rel16();
                                break;

                            case 0x8c:
                                if (operandSizeOverride) JL_JNGE_rel32(); else JL_JNGE_rel16();
                                break;

                            case 0x8d:
                                if (operandSizeOverride) JGE_JNL_rel32(); else JGE_JNL_rel16();
                                break;

                            case 0x8e:
                                if (operandSizeOverride) JLE_JNG_rel32(); else JLE_JNG_rel16();
                                break;

                            case 0x8f:
                                if (operandSizeOverride) JG_JNLE_rel32(); else JG_JNLE_rel16();
                                break;

                            case 0xa0:
                                if (operandSizeOverride) PUSH_FS_o32(); else PUSH_FS_o16();
                                break;

                            case 0xa1:
                                if (operandSizeOverride) POP_FS_o32(); else POP_FS_o16();
                                break;

                            case 0xa8:
                                if (operandSizeOverride) PUSH_GS_o32(); else PUSH_GS_o16();
                                break;

                            case 0xa9:
                                if (operandSizeOverride) POP_GS_o32(); else POP_GS_o16();
                                break;

                            //default:    need some kind of exception?
                        }
                        break;

                    default:
                        RaiseException(ExceptionEnum.InvalidOpcode, ExceptionDetailEnum.Null);
                        break;
                }
            }
        }

        private int ExecuteNextInstruction_ProtectedMode()
        {
            int iLength = 0;
            //byte code = GetByte(SegmentEnum.CS, GetInstructionPointer(prefixLength));

            //if ((code & 0xf8) == 0xb0)
            //{
            //    iLength = MOV_r8_imm8();
            //}
            //else if ((code & 0xf8) == 0xb8)
            //{
            //    iLength = operandSizeOverride ? MOV_r16_imm16() : MOV_r32_imm32();
            //}
            //else
            //{
            //    switch (code)
            //    {
            //        case 0x88:
            //            iLength = MOV_rm8_r8();
            //            break;

            //        case 0x89:
            //            iLength = operandSizeOverride ? MOV_rm16_r16() : MOV_rm32_r32();
            //            break;

            //        case 0x8a:
            //            iLength = MOV_r8_rm8();
            //            break;

            //        case 0x8b:
            //            iLength = operandSizeOverride ? MOV_r16_rm16() : MOV_r32_rm32();
            //            break;

            //        case 0x8c:
            //            iLength = MOV_rm16_Sreg();
            //            break;

            //        case 0x8e:
            //            iLength = MOV_Sreg_rm16();
            //            break;

            //        case 0xa0:
            //            MOV_AL_moffs8();
            //            break;

            //        case 0xa1:
            //            iLength = operandSizeOverride ? MOV_AX_moffs16() : MOV_EAX_moffs32();
            //            break;

            //        case 0xa2:
            //            iLength = MOV_moffs8_AL();
            //            break;

            //        case 0xa3:
            //            iLength = operandSizeOverride ? MOV_moffs16_AX() : MOV_moffs32_EAX();
            //            break;

            //        case 0xc6:
            //            iLength = MOV_rm8_imm8();
            //            break;

            //        case 0xc7:
            //            iLength = operandSizeOverride ? MOV_rm16_imm16() : MOV_rm32_imm32();
            //            break;

            //        case 0xe9:
            //            JMP_rel32();
            //            break;

            //        case 0xeb:
            //            JMP_rel8();
            //            break;

            //        case 0xf4:
            //            iLength = Halt();
            //            break;

            //        default:
            //            RaiseException(ExceptionEnum.InvalidOpcode, ExceptionDetailEnum.Null);
            //            break;
            //    }
            //}

            return iLength;
        }

        private int ExecuteNextInstruction_IA32e_CompatibilityMode()
        {
            MessageBox.Show(Form1.ActiveForm, "IA32e_CompatibilityMode not implemented.");
            return Halt();
        }

        private int ExecuteNextInstruction_IA32e_64bitMode()
        {
            MessageBox.Show(Form1.ActiveForm, "IA32e_64bitMode not implemented.");
            return Halt();
        }


        #endregion

        private ModRM_Byte GetModRM()
        {
            byte modRM = GetNextInstructionByte();
            ModRM_Byte temp;
            temp.ActualByte = modRM;
            temp.ModValue = (uint)modRM >> 6;
            temp.RegOpValue = (uint)(modRM & 0x38) >> 3;
            temp.RegMemValue = (uint)modRM & 0x7;
            temp.DefaultSegment = SegmentEnum.DS;
            temp.EffectiveAddress = 0;

            if (temp.ModValue == 3)
            {
                temp.RegMemType = RegMemTypeEnum.Register;
            }
            else
            {
                temp.RegMemType = RegMemTypeEnum.Memory;
                #region if (GetCurrentAddressMode() == AddressSizeEnum.Bits16)
                if (GetCurrentAddressMode() == AddressSizeEnum.Bits16)
                {
                    ushort effectiveAddress = 0;

                    if (temp.ModValue == 0)
                    {
                        switch (temp.RegMemValue)
                        {
                            case 0:
                                effectiveAddress = (ushort)(bx + si);
                                break;
                                
                            case 1:
                                effectiveAddress = (ushort)(bx + di);
                                break;

                            case 2:
                                effectiveAddress = (ushort)(bp + si);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 3:
                                effectiveAddress = (ushort)(bp + di);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 4:
                                effectiveAddress = si;
                                break; 

                            case 5:
                                effectiveAddress = di;
                                break;

                            case 6:
                                effectiveAddress = GetNextInstructionWord();
                                break;

                            case 7:
                                effectiveAddress = bx;
                                break;
                        }
                    }
                    else if (temp.ModValue == 1)
                    {
                        sbyte disp8 = (sbyte)GetNextInstructionByte();
                        switch (temp.RegMemValue)
                        {
                            case 0:
                                effectiveAddress = (ushort)(bx + si + disp8);
                                break;

                            case 1:
                                effectiveAddress = (ushort)(bx + di + disp8);
                                break;

                            case 2:
                                effectiveAddress = (ushort)(bp + si + disp8);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 3:
                                effectiveAddress = (ushort)(bp + di + disp8);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 4:
                                effectiveAddress = (ushort)(si + disp8);
                                break;

                            case 5:
                                effectiveAddress = (ushort)(di + disp8);
                                break;

                            case 6:
                                effectiveAddress = (ushort)(bp + disp8);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 7:
                                effectiveAddress = (ushort)(bx + disp8);
                                break;
                        }
                    }
                    else if (temp.ModValue == 2)
                    {
                        ushort disp16 = GetNextInstructionWord();
                        switch (temp.RegMemValue)
                        {
                            case 0:
                                effectiveAddress = (ushort)(bx + si + disp16);
                                break;

                            case 1:
                                effectiveAddress = (ushort)(bx + di + disp16);
                                break;

                            case 2:
                                effectiveAddress = (ushort)(bp + si + disp16);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 3:
                                effectiveAddress = (ushort)(bp + di + disp16);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 4:
                                effectiveAddress = (ushort)(si + disp16);
                                break;

                            case 5:
                                effectiveAddress = (ushort)(di + disp16);
                                break;

                            case 6:
                                effectiveAddress = (ushort)(bp + disp16);
                                temp.DefaultSegment = SegmentEnum.SS;
                                break;

                            case 7:
                                effectiveAddress = (ushort)(bx + disp16);
                                break;
                        }
                    }

                    temp.EffectiveAddress = effectiveAddress;
                }
                #endregion
                #region else if (GetCurrentAddressMode() == AddressSizeEnum.Bits32)
                else if (GetCurrentAddressMode() == AddressSizeEnum.Bits32)
                {
                    //uint effectiveAddress = 0;

                    if (temp.ModValue == 0)
                    {

                    }
                    else if (temp.ModValue == 1)
                    {

                    }

                }
                #endregion
            }
 
            return temp;
        }

        private byte GetRegOpFieldFromModRM()
        {
            // this function grabs the RegOpcode field from the ModRM byte at the address held in the instruction pointer
            // *** INSTRUCTION POINTER IS NOT INCREMENTED ***

            byte modRM = GetNextInstructionByteWithoutInstructionPointerIncrement();
            return (byte)((modRM & 0x38) >> 3);
        }

        private SIB_Byte BreakSIB(uint SIB_Ptr)
        {
            byte SIB = GetByte(SegmentEnum.CS, SIB_Ptr);
            SIB_Byte temp;
            temp.SS = (uint)SIB >> 6;
            temp.Index = (uint)(SIB & 0x38) >> 3;
            temp.Base = (uint)SIB & 0x7;
            temp.SIB_Ptr = SIB_Ptr;
            return temp;
        }

        //private uint GetEffectiveAddressFromSIB(uint SIB_Ptr, ModRM_Byte ModRM)
        //{
        //    // should have code that calculates address based on current addressing mode

        //    SIB_Byte SIB = BreakSIB(SIB_Ptr);
        //    uint base_address = 0;
        //    uint index = 0;
        //    uint multiplier = 0;

        //    switch (SIB.Base)
        //    {
        //        case 0:
        //            base_address = eax;
        //            break;

        //        case 1:
        //            base_address = ecx;
        //            break;

        //        case 2:
        //            base_address = edx;
        //            break;

        //        case 3:
        //            base_address = ebx;
        //            break;

        //        case 4:
        //            base_address = esp;
        //            break;

        //        case 5:
        //            if (ModRM.ModValue != 0)
        //            {
        //                base_address = ebp;
        //            }
        //            else
        //            {
        //                base_address = GetDword(SegmentEnum.CS, SIB.SIB_Ptr + 1);
        //                //disp32_present = true;
        //            }
        //            break;

        //        case 6:
        //            base_address = esi;
        //            break;

        //        case 7:
        //            base_address = edi;
        //            break;
        //    }

        //    switch (SIB.SS)
        //    {
        //        case 0:
        //            multiplier = 1;
        //            break;
                    
        //        case 1:
        //            multiplier = 2;
        //            break;

        //        case 2:
        //            multiplier = 4;
        //            break;

        //        case 3:
        //            multiplier = 8;
        //            break;
        //    }

        //    switch (SIB.Index)
        //    {
        //        case 0:
        //            index = eax * multiplier;
        //            break;

        //        case 1:
        //            index = ecx * multiplier;
        //            break;

        //        case 2:
        //            index = edx * multiplier;
        //            break;

        //        case 3:
        //            index = ebx * multiplier;
        //            break;

        //        case 4:
        //            index = 0;
        //            break;

        //        case 5:
        //            index = ebp * multiplier;
        //            break;

        //        case 6:
        //            index = esi * multiplier;
        //            break;

        //        case 7:
        //            index = edi * multiplier;
        //            break;
        //    }

        //    return base_address + index;
        //}

        #region SetReg* Methods


        private void SetReg8(ulong RegCode, byte Value)
        {
            switch (RegCode)
            {
                case 0: // AL
                    al = Value;
                    break;

                case 1: // CL
                    cl = Value;
                    break;

                case 2: // DL
                    dl = Value;
                    break;

                case 3: // BL
                    bl = Value;
                    break;

                case 4: // AH
                    ah = Value;
                    break;

                case 5: // CH
                    ch = Value;
                    break;

                case 6: // DH
                    dh = Value;
                    break;

                case 7: // BH
                    bh = Value;
                    break;
            }
        }

        private void SetReg16(ulong RegCode, ushort Value)
        {
            switch (RegCode)
            {
                case 0: // AX
                    ax = Value;
                    break;

                case 1: // CX
                    cx = Value;
                    break;

                case 2: // DX
                    dx = Value;
                    break;

                case 3: // BX
                    bx = Value;
                    break;

                case 4: // SP
                    sp = Value;
                    break;

                case 5: // BP
                    bp = Value;
                    break;

                case 6: // SI
                    si = Value;
                    break;

                case 7: // DI
                    di = Value;
                    break;
            }
        }

        private void SetReg32(ulong RegCode, uint Value)
        {
            switch (RegCode)
            {
                case 0: // EAX
                    eax = Value;
                    break;

                case 1: // ECX
                    ecx = Value;
                    break;

                case 2: // EDX
                    edx = Value;
                    break;

                case 3: // EBX
                    ebx = Value;
                    break;

                case 4: // ESP
                    esp = Value;
                    break;

                case 5: // EBP
                    ebp = Value;
                    break;

                case 6: // ESI
                    esi = Value;
                    break;

                case 7: // EDI
                    edi = Value;
                    break;
            }
        }

        private void SetReg64(ulong RegCode, ulong Value)
        {
            switch (RegCode)
            {
                case 0: // RAX
                    rax = Value;
                    break;

                case 1: // RCX
                    rcx = Value;
                    break;

                case 2: // RDX
                    rdx = Value;
                    break;

                case 3: // RBX
                    rbx = Value;
                    break;

                case 4: // RSP
                    rsp = Value;
                    break;

                case 5: // RBP
                    rbp = Value;
                    break;

                case 6: // RSI
                    rsi = Value;
                    break;

                case 7: // RDI
                    rdi = Value;
                    break;
            }
        }

        private void SetRegSeg(ulong RegCode, ushort Value)
        {
            switch (RegCode)
            {
                case 0: // ES
                    es = Value;
                    break;

                case 1: // CS
                    RaiseException(ExceptionEnum.InvalidOpcode, ExceptionDetailEnum.AttemptedToLoadCS);
                    break;

                case 2: // SS
                    ss = Value;
                    break;

                case 3: // DS
                    ds = Value;
                    break;

                case 4: // FS
                    fs = Value;
                    break;

                case 5: // GS
                    gs = Value;
                    break;
            }
        }


        #endregion

        #region GetReg* Methods


        private byte GetReg8(ulong RegCode)
        {
            switch (RegCode)
            {
                case 0: // AL
                    return al;

                case 1: // CL
                    return cl;

                case 2: // DL
                    return dl;

                case 3: // BL
                    return bl;

                case 4: // AH
                    return ah;

                case 5: // CH
                    return ch;

                case 6: // DH
                    return dh;

                case 7: // BH
                    return bh;
            }
            return 0;
        }

        private ushort GetReg16(ulong RegCode)
        {
            switch (RegCode)
            {
                case 0: // AX
                    return ax;

                case 1: // CX
                    return cx;

                case 2: // DX
                    return dx;

                case 3: // BX
                    return bx;

                case 4: // SP
                    return sp;

                case 5: // BP
                    return bp;

                case 6: // SI
                    return si;

                case 7: // DI
                    return di;
            }
            return 0;
        }

        private uint GetReg32(ulong RegCode)
        {
            switch (RegCode)
            {
                case 0: // EAX
                    return eax;

                case 1: // ECX
                    return ecx;

                case 2: // EDX
                    return edx;

                case 3: // EBX
                    return ebx;

                case 4: // ESP
                    return esp;

                case 5: // EBP
                    return ebp;

                case 6: // ESI
                    return esi;

                case 7: // EDI
                    return edi;
            }
            return 0;
        }

        private ulong GetReg64(ulong RegCode)
        {
            switch (RegCode)
            {
                case 0: // RAX
                    return rax;

                case 1: // RCX
                    return rcx;

                case 2: // RDX
                    return rdx;

                case 3: // RBX
                    return rbx;

                case 4: // RSP
                    return rsp;

                case 5: // RBP
                    return rbp;

                case 6: // RSI
                    return rsi;

                case 7: // RDI
                    return rdi;
            }
            return 0;
        }

        private ushort GetRegSeg(ulong RegCode)
        {
            switch (RegCode)
            {
                case 0: // ES
                    return es;

                case 2: // SS
                    return ss;

                case 3: // DS
                    return ds;

                case 4: // FS
                    return fs;

                case 5: // GS
                    return gs;
            }
            return 0;
        }


        #endregion

        #region Flag Methods (Get, Set, Clear, Complement)


        private void SetFlag(FlagsEnum Flag)
        {
            rflags |= (ulong)Flag;
        }

        private void ClearFlag(FlagsEnum Flag)
        {
            rflags &= ~(ulong)Flag;
        }

        private void ComplementFlag(FlagsEnum Flag)
        {
            rflags = rflags ^ (ulong)Flag;
        }

        public bool GetFlag(FlagsEnum Flag)
        {
            if ((rflags & (ulong)Flag) == 0)
                return false;
            else
                return true;
        }

        private void SetZeroFlagByByte(byte Value)
        {
            if (Value == 0)
                SetFlag(FlagsEnum.ZF_ZeroFlag);
            else
                ClearFlag(FlagsEnum.ZF_ZeroFlag);
        }

        private void SetSignFlagByByte(byte Value)
        {
            if ((Value & 128) == 0)
                ClearFlag(FlagsEnum.SF_SignFlag);
            else
                SetFlag(FlagsEnum.SF_SignFlag);
        }
        
        private void SetParityFlagByByte(byte Value)
        {
            uint count = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((Value & 1) != 0)
                    count++;
                Value = (byte)(Value >> 1);
            }

            if ((count & 1) == 0)
                SetFlag(FlagsEnum.PF_ParityFlag);
            else
                ClearFlag(FlagsEnum.PF_ParityFlag);
        }

        private void SetZeroFlagByWord(ushort Word)
        {
            if (Word == 0)
                SetFlag(FlagsEnum.ZF_ZeroFlag);
            else
                ClearFlag(FlagsEnum.ZF_ZeroFlag);
        }

        private void SetSignFlagByWord(ushort Word)
        {
            if ((Word & 32768) == 0)
                ClearFlag(FlagsEnum.SF_SignFlag);
            else
                SetFlag(FlagsEnum.SF_SignFlag);
        }

        private void SetParityFlagByWord(ushort Word)
        {
            byte lsb = (byte)Word;

            uint count = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((lsb & 1) != 0)
                    count++;
                lsb = (byte)(lsb >> 1);
            }

            if ((count & 1) == 0)
                SetFlag(FlagsEnum.PF_ParityFlag);
            else
                ClearFlag(FlagsEnum.PF_ParityFlag);
        }

        private void SetZeroFlagByDword(uint Dword)
        {
            if (Dword == 0)
                SetFlag(FlagsEnum.ZF_ZeroFlag);
            else
                ClearFlag(FlagsEnum.ZF_ZeroFlag);
        }

        private void SetSignFlagByDword(uint Dword)
        {
            if ((Dword & 2147483648) == 0)
                ClearFlag(FlagsEnum.SF_SignFlag);
            else
                SetFlag(FlagsEnum.SF_SignFlag);
        }

        private void SetParityFlagByDword(uint Dword)
        {
            byte lsb = (byte)Dword;

            uint count = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((lsb & 1) != 0)
                    count++;
                lsb = (byte)(lsb >> 1);
            }

            if ((count & 1) == 0)
                SetFlag(FlagsEnum.PF_ParityFlag);
            else
                ClearFlag(FlagsEnum.PF_ParityFlag);
        }

        private void SetIOPrivilegeLevel(byte Value)
        {
            if (Value > 3) return;

            if ((Value & 1) != 0)
                rflags |= (ulong)FlagsEnum.IOPL_IOPrivilegeLevelBit0;
            else
                rflags &= ~(ulong)FlagsEnum.IOPL_IOPrivilegeLevelBit0;

            if ((Value & 2) != 0)
                rflags |= (ulong)FlagsEnum.IOPL_IOPrivilegeLevelBit1;
            else
                rflags &= ~(ulong)FlagsEnum.IOPL_IOPrivilegeLevelBit1;
        }

        public byte GetIOPrivilegeLevel()
        {
            byte level = 0;

            if ((rflags & (ulong)FlagsEnum.IOPL_IOPrivilegeLevelBit0) != 0)
                level = 1;

            if ((rflags & (ulong)FlagsEnum.IOPL_IOPrivilegeLevelBit1) != 0)
                level = (byte)(level + 2);

            return level;
        }


        #endregion

        #region Set(Byte, Word, Dword, Qword) Methods


        private void SetByte(SegmentEnum Segment, ulong Address, byte Value)
        {
            mch.SetByte(GetRealAddress(Segment, Address), Value);
        }

        private void SetWord(SegmentEnum Segment, ulong Address, ushort Value)
        {
            if ((opMode == OpModeEnum.RealAddressMode))
            {
                if ((Segment == SegmentEnum.SS) && (Address > 0xfffe))
                    RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
            }

            mch.SetWord(GetRealAddress(Segment, Address), Value);
        }

        private void SetDword(SegmentEnum Segment, ulong Address, uint Value)
        {
            if ((opMode == OpModeEnum.RealAddressMode))
            {
                if ((Segment == SegmentEnum.SS) && (Address > 0xfffc))
                    RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
            }

            mch.SetDword(GetRealAddress(Segment, Address), Value);
        }

        private void SetQword(SegmentEnum Segment, ulong Address, ulong Value)
        {
            mch.SetQword(GetRealAddress(Segment, Address), Value);
        }


        #endregion

        #region Get(Byte, Word, Dword, Qword) Methods


        private byte GetByte(SegmentEnum Segment, ulong Address)
        {
            return mch.GetByte(GetRealAddress(Segment, Address));
        }

        private ushort GetWord(SegmentEnum Segment, ulong Address)
        {
            if ((opMode == OpModeEnum.RealAddressMode))
            {
                if ((Segment == SegmentEnum.SS) && (Address > 0xfffe))
                    RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
            }

            return mch.GetWord(GetRealAddress(Segment, Address));
        }

        private uint GetDword(SegmentEnum Segment, ulong Address)
        {
            if ((opMode == OpModeEnum.RealAddressMode))
            {
                if ((Segment == SegmentEnum.SS) && (Address > 0xfffc))
                    RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
            }

            return mch.GetDword(GetRealAddress(Segment,Address));
        }

        private ulong GetQword(SegmentEnum Segment, ulong Address)
        {
            return mch.GetQword(GetRealAddress(Segment, Address));
        }


        #endregion

        #region GetNextInstruction(Byte, Word, Dword, Qword, MemoryOffset) Methods


        byte GetNextInstructionByte()
        {
            ulong address = 0;

            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    if (ip > temp_ip)                   // if temp_ip has wrapped to zero while fetching the current instruction throw exception!
                    {
                        MessageBox.Show(Form1.ActiveForm, "GetNextInstructionByte() - temp_ip wrapped");
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        address = temp_ip;
                        temp_ip++;
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, address));
                    }
                    break;

                case OpModeEnum.ProtectedMode:
                    if (eip > temp_eip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        address = temp_eip;
                        temp_eip++;
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, address));
                    }
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    if (eip > temp_eip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        address = temp_eip;
                        temp_eip++;
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, address));
                    }
                    break;

                case OpModeEnum.IA32e_64bitMode:
                    if (rip > temp_rip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        address = temp_rip;
                        temp_rip++;
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, address));
                    }
                    break;
            }

            return 0;
        }

        byte GetNextInstructionByteWithoutInstructionPointerIncrement()
        {
            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    if (ip > temp_ip)                   // if temp_ip has wrapped to zero while fetching the current instruction throw exception!
                    {
                        MessageBox.Show(Form1.ActiveForm, "GetNextInstructionByte() - temp_ip wrapped");
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, temp_ip));
                    }
                    break;

                case OpModeEnum.ProtectedMode:
                    if (eip > temp_eip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, temp_eip));
                    }
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    if (eip > temp_eip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, temp_eip));
                    }
                    break;

                case OpModeEnum.IA32e_64bitMode:
                    if (rip > temp_rip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        return mch.GetByte(GetRealAddress(SegmentEnum.CS, temp_eip));
                    }
                    break;
            }

            return 0;
        }

        ushort GetNextInstructionWord()
        {
            ulong address = 0;

            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    if (ip > temp_ip)
                    {
                        MessageBox.Show(Form1.ActiveForm, "GetNextInstructionWord() - temp_ip wrapped");
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        if (temp_ip < 0xffff)
                        {
                            address = temp_ip;
                            temp_ip += 2;
                            return mch.GetWord(GetRealAddress(SegmentEnum.CS, address));
                        }
                        else
                        {
                            MessageBox.Show(Form1.ActiveForm, "GetNextInstructionWord() - most significant part of word is past segment boundary");
                            RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                        }
                    }
                    break;

                case OpModeEnum.ProtectedMode:
                    if (eip > temp_eip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        if (temp_eip < 0xffffffff)
                        {
                            address = temp_eip;
                            temp_eip += 2;
                            return mch.GetWord(GetRealAddress(SegmentEnum.CS, address));
                        }
                        else
                        {
                            RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                        }
                    }
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    return 0;

                case OpModeEnum.IA32e_64bitMode:
                    return 0;
            }

            return 0;
        }

        uint GetNextInstructionDword()
        {
            ulong address = 0;

            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    if (ip > temp_ip)
                    {
                        MessageBox.Show(Form1.ActiveForm, "GetNextInstructionDword()");
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        if (temp_ip < 0xfffd)
                        {
                            address = temp_ip;
                            temp_ip += 4;
                            return mch.GetDword(GetRealAddress(SegmentEnum.CS, address));
                        }
                        else
                        {
                            MessageBox.Show(Form1.ActiveForm, "GetNextInstructionWord() - most significant part of dword is past segment boundary");
                            RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                        }
                    }
                    break;

                case OpModeEnum.ProtectedMode:
                    if (eip > temp_eip)
                    {
                        RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                    }
                    else
                    {
                        if (temp_eip < 0xfffffffd)
                        {
                            address = temp_eip;
                            temp_eip += 4;
                            return mch.GetDword(GetRealAddress(SegmentEnum.CS, address));
                        }
                        else
                        {
                            RaiseException(ExceptionEnum.GeneralProtection, ExceptionDetailEnum.InstructionPointerSegmentOverrun);
                        }
                    }
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    return 0;

                case OpModeEnum.IA32e_64bitMode:
                    return 0;
            }

            return 0;
        }

        private ulong GetNextInstructionMemoryOffset()
        {
            ulong offset = 0;

            if (opMode == OpModeEnum.RealAddressMode)
            {
                if (GetCurrentAddressMode() == AddressSizeEnum.Bits16)
                {
                    offset = GetNextInstructionWord();
                }
                else if (GetCurrentAddressMode() == AddressSizeEnum.Bits32)
                {
                    offset = GetNextInstructionDword();
                }
            }

            return offset;
        }
        
        
        #endregion

        #region Set(Byte, Word, Dword, Qword)InEffectiveSegment Methods


        private void SetByteInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address, byte Value)
        {
            SetByte(GetEffectiveSegment(PreferredSegment), Address, Value);
        }

        private void SetWordInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address, ushort Value)
        {
            SetWord(GetEffectiveSegment(PreferredSegment), Address, Value);
        }

        private void SetDwordInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address, uint Value)
        {
            SetDword(GetEffectiveSegment(PreferredSegment), Address, Value);
        }

        private void SetQwordInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address, ulong Value)
        {
            SetQword(GetEffectiveSegment(PreferredSegment), Address, Value);
        }


        #endregion

        #region Get(Byte, Word, Dword, Qword)InEffectiveSegment Methods


        private byte GetByteInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address)
        {
            return GetByte(GetEffectiveSegment(PreferredSegment), Address);
        }

        private ushort GetWordInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address)
        {
            return GetWord(GetEffectiveSegment(PreferredSegment), Address);
        }

        private uint GetDwordInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address)
        {
            return GetDword(GetEffectiveSegment(PreferredSegment), Address);
        }

        private ulong GetQwordInEffectiveSegment(SegmentEnum PreferredSegment, ulong Address)
        {
            return GetQword(GetEffectiveSegment(PreferredSegment), Address);
        }


        #endregion

        #region Add* Methods


        private byte AddTwoBytesAndSetFlags(byte FirstOperand, byte SecondOperand)
        {
            byte result = (byte)(FirstOperand + SecondOperand);

            // set CF to correct state
            if (result < FirstOperand)
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            byte temp = (byte)(FirstOperand & 0x80);                                // if both operands are positive or negative, make sure
            if ((temp == (SecondOperand & 0x80) && (temp != (result & 0x80))))      // that the result is the same sign, otherwise an overflow occurred
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);
    
            // set AF to correct state                          
            if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
            else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort AddTwoWordsAndSetFlags(ushort FirstOperand, ushort SecondOperand)
        {
            ushort result = (ushort)(FirstOperand + SecondOperand);

            // set CF to correct state
            if (result < FirstOperand)
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            ushort temp = (ushort)(FirstOperand & 0x8000);                              // if both operands are positive or negative, make sure
            if ((temp == (SecondOperand & 0x8000) && (temp != (result & 0x8000))))      // that the result is the same sign, otherwise an overflow occurred
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
            else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint AddTwoDwordsAndSetFlags(uint FirstOperand, uint SecondOperand)
        {
            uint result = (uint)(FirstOperand + SecondOperand);

            // set CF to correct state
            if (result < FirstOperand)
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            uint temp = (uint)(FirstOperand & 0x80000000);                                     // if both operands are positive or negative, make sure
            if ((temp == (SecondOperand & 0x80000000) && (temp != (result & 0x80000000))))     // that the result is the same sign, otherwise an overflow occurred
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
            else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }

        private byte AddTwoBytesWithCarryAndSetFlags(byte FirstOperand, byte SecondOperand)
        {
            byte result;

            if (GetFlag(FlagsEnum.CF_CarryFlag))
            {
                result = (byte)(FirstOperand + SecondOperand + 1);

                // set CF to correct state
                if ((FirstOperand == 0xff) || (SecondOperand == 0xff))          // if either operand has all bits set, a carry has definitely occurred
                {                                                               // because we're adding 1!
                    SetFlag(FlagsEnum.CF_CarryFlag);
                }                                                               // otherwise, evaluate the carry condition as normal...
                else
                {
                    if (result < FirstOperand)
                        SetFlag(FlagsEnum.CF_CarryFlag);
                    else
                        ClearFlag(FlagsEnum.CF_CarryFlag);
                }

                // set AF to correct state
                if (((FirstOperand & 0xf) == 0xf) || ((SecondOperand & 0xf) == 0xf))    // if the four least-significant bits are set in either operand,
                {                                                                       // an aux carry has definitely occurred because we're adding 1!
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);
                }
                else
                {                                                       // otherwise, evaluate the aux carry condition as normal...
                    if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                        SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
                    else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                        ClearFlag(FlagsEnum.AF_AuxCarryFlag);
                }
            }
            else
            {
                result = (byte)(FirstOperand + SecondOperand);

                // set CF to correct state
                if (result < FirstOperand)
                    SetFlag(FlagsEnum.CF_CarryFlag);
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
                else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }

            // set OF to correct state
            byte temp = (byte)(FirstOperand & 0x80);                                // if both operands are positive or both negative, make sure
            if ((temp == (SecondOperand & 0x80) && (temp != (result & 0x80))))      // that the result is the same sign, otherwise an overflow occurred
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort AddTwoWordsWithCarryAndSetFlags(ushort FirstOperand, ushort SecondOperand)
        {
            ushort result;

            if (GetFlag(FlagsEnum.CF_CarryFlag))
            {
                result = (ushort)(FirstOperand + SecondOperand + 1);

                // set CF to correct state
                if ((FirstOperand == 0xffff) || (SecondOperand == 0xffff))      // if either operand has all bits set, a carry has definitely occurred
                {                                                               // because we're adding 1!
                    SetFlag(FlagsEnum.CF_CarryFlag);
                }                                                               // otherwise, evaluate the carry condition as normal...
                else
                {
                    if (result < FirstOperand)
                        SetFlag(FlagsEnum.CF_CarryFlag);
                    else
                        ClearFlag(FlagsEnum.CF_CarryFlag);
                }

                // set AF to correct state
                if (((FirstOperand & 0xf) == 0xf) || ((SecondOperand & 0xf) == 0xf))    // if the four least-significant bits are set in either operand,
                {                                                                       // an aux carry has definitely occurred because we're adding 1!
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);
                }
                else
                {                                                       // otherwise, evaluate the aux carry condition as normal...
                    if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                        SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
                    else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                        ClearFlag(FlagsEnum.AF_AuxCarryFlag);
                }
            }
            else
            {
                result = (ushort)(FirstOperand + SecondOperand);

                // set CF to correct state
                if (result < FirstOperand)
                    SetFlag(FlagsEnum.CF_CarryFlag);
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
                else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }

            // set OF to correct state
            ushort temp = (ushort)(FirstOperand & 0x8000);                              // if both operands are positive or negative, make sure
            if ((temp == (SecondOperand & 0x8000)) && (temp != (result & 0x8000)))      // that the result is the same sign, otherwise an overflow occurred
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint AddTwoDwordsWithCarryAndSetFlags(uint FirstOperand, uint SecondOperand)
        {
            uint result;

            if (GetFlag(FlagsEnum.CF_CarryFlag))
            {
                result = FirstOperand + SecondOperand + 1;

                // set CF to correct state
                if ((FirstOperand == 0xffffffff) || (SecondOperand == 0xffffffff))     // if either operand has all bits set, a carry has definitely occurred
                {                                                                      // because we're adding 1!
                    SetFlag(FlagsEnum.CF_CarryFlag);
                }                                                                      // otherwise, evaluate the carry condition as normal...
                else
                {
                    if (result < FirstOperand)
                        SetFlag(FlagsEnum.CF_CarryFlag);
                    else
                        ClearFlag(FlagsEnum.CF_CarryFlag);
                }

                // set AF to correct state
                if (((FirstOperand & 0xf) == 0xf) || ((SecondOperand & 0xf) == 0xf))    // if the four least-significant bits are set in either operand,
                {                                                                       // an aux carry has definitely occurred because we're adding 1!
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);
                }
                else
                {                                                       // otherwise, evaluate the aux carry condition as normal...
                    if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                        SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
                    else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                        ClearFlag(FlagsEnum.AF_AuxCarryFlag);
                }
            }
            else
            {
                result = FirstOperand + SecondOperand;

                // set CF to correct state
                if (result < FirstOperand)
                    SetFlag(FlagsEnum.CF_CarryFlag);
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((result & 0xf) < (FirstOperand & 0xf))          // if four least significant bits of result are less than four
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of one of the operands, a carry has
                else                                                // occurred at bit 3, so set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }

            // set OF to correct state
            uint temp = FirstOperand & 0x80000000;                                              // if both operands are positive or negative, make sure
            if ((temp == (SecondOperand & 0x80000000)) && (temp != (result & 0x80000000)))      // that the result is the same sign, otherwise an overflow occurred
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }


        #endregion

        #region Inc* Methods


        private byte IncByteAndSetFlags(byte Value)
        {
            byte result = (byte)(Value + 1);

            // set OF to correct state
            if (Value == 0x7f)                                  // if Value is 0x7f it will become a negative once incremented by one... overflow!
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) == 0xf)                           // if four least significant bits of the original value are on, a carry will occur
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // at bit 3, otherwise no carry will occur, since we're only adding 1.
            else                                                // if a bit 3 carry occurred, set AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort IncWordAndSetFlags(ushort Value)
        {
            ushort result = (ushort)(Value + 1);

            // set OF to correct state
            if (Value == 0x7fff)                                // if Value is 0x7f it will become a negative once incremented by one... overflow!
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) == 0xf)                           // if four least significant bits of the original value are on, a carry will occur
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // at bit 3, otherwise no carry will occur, since we're only adding 1.
            else                                                // if a bit 3 carry occurred, set AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint IncDwordAndSetFlags(uint Value)
        {
            uint result = Value + 1;

            // set OF to correct state
            if (Value == 0x7fffffff)                            // if Value is 0x7f it will become a negative once incremented by one... overflow!
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) == 0xf)                           // if four least significant bits of the original value are on, a carry will occur
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // at bit 3, otherwise no carry will occur, since we're only adding 1.
            else                                                // if a bit 3 carry occurred, set AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }


        #endregion

        #region Sub* Methods


        private byte SubTwoBytesAndSetFlags(byte FirstOperand, byte SecondOperand)
        {
            byte result = (byte)(FirstOperand - SecondOperand);

            // set CF to correct state
            if (SecondOperand > FirstOperand)                   // if SecondOperand is greater, the subtraction will result in a borrow
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            byte temp = (byte)(FirstOperand & 0x80);
            if (temp == (SecondOperand & 0x80))                 // if both operands have the same sign, no chance of overflow... clear the OF flag
                ClearFlag(FlagsEnum.OF_OverflowFlag);
            else
            {                                                   // if the operands have different signs, verify that the result has the same sign
                if (temp != (result & 0x80))                    // as the first operand... if it doesn't, an overflow occurred... set the OF flag
                    SetFlag(FlagsEnum.OF_OverflowFlag);
                else
                    ClearFlag(FlagsEnum.OF_OverflowFlag);
            }

            // set AF to correct state                          
            if ((SecondOperand & 0xf) > (FirstOperand & 0xf))   // if four least significant bits of SecondOperand are greater than four
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of FirstOperand, a borrow has occurred at bit 3...
            else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort SubTwoWordsAndSetFlags(ushort FirstOperand, ushort SecondOperand)
        {
            ushort result = (ushort)(FirstOperand - SecondOperand);

            // set CF to correct state
            if (SecondOperand > FirstOperand)                   // if SecondOperand is greater, the subtraction will result in a borrow
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            ushort temp = (ushort)(FirstOperand & 0x8000);
            if (temp == (SecondOperand & 0x8000))               // if both operands have the same sign, no chance of overflow... clear the OF flag
                ClearFlag(FlagsEnum.OF_OverflowFlag);
            else
            {                                                   // if the operands have different signs, verify that the result has the same sign
                if (temp != (result & 0x8000))                  // as the first operand... if it doesn't, an overflow occurred... set the OF flag
                    SetFlag(FlagsEnum.OF_OverflowFlag);
                else
                    ClearFlag(FlagsEnum.OF_OverflowFlag);
            }

            // set AF to correct state                          
            if ((SecondOperand & 0xf) > (FirstOperand & 0xf))   // if four least significant bits of SecondOperand are greater than four
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of FirstOperand, a borrow has occurred at bit 3...
            else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint SubTwoDwordsAndSetFlags(uint FirstOperand, uint SecondOperand)
        {
            uint result = FirstOperand - SecondOperand;

            // set CF to correct state
            if (SecondOperand > FirstOperand)                   // if SecondOperand is greater, the subtraction will result in a borrow
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            uint temp = FirstOperand & 0x80000000;
            if (temp == (SecondOperand & 0x80000000))           // if both operands have the same sign, no chance of overflow... clear the OF flag
                ClearFlag(FlagsEnum.OF_OverflowFlag);
            else
            {                                                   // if the operands have different signs, verify that the result has the same sign
                if (temp != (result & 0x80000000))              // as the first operand... if it doesn't, an overflow occurred... set the OF flag
                    SetFlag(FlagsEnum.OF_OverflowFlag);
                else
                    ClearFlag(FlagsEnum.OF_OverflowFlag);
            }

            // set AF to correct state                          
            if ((SecondOperand & 0xf) > (FirstOperand & 0xf))   // if four least significant bits of SecondOperand are greater than four
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of FirstOperand, a borrow has occurred at bit 3...
            else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }

        private byte SubTwoBytesWithBorrowAndSetFlags(byte FirstOperand, byte SecondOperand)
        {
            byte result; 

            if (GetFlag(FlagsEnum.CF_CarryFlag))
            {
                result = (byte)(FirstOperand - SecondOperand - 1);

                // set CF to correct state
                if (SecondOperand >= FirstOperand)                  // if SecondOperand is greater, or equal (since we're subtracting the borrow),
                    SetFlag(FlagsEnum.CF_CarryFlag);                // the subtraction will result in a borrow
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((SecondOperand & 0xf) >= (FirstOperand & 0xf))  // if four least significant bits of SecondOperand are greater than, or equal to,
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // the four least significant bits of FirstOperand, a borrow has occurred at bit 3...
                else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }
            else
            {
                result = (byte)(FirstOperand - SecondOperand);

                // set CF to correct state
                if (SecondOperand > FirstOperand)                   // if SecondOperand is greater, or equal, the subtraction will result in a borrow
                    SetFlag(FlagsEnum.CF_CarryFlag);
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((SecondOperand & 0xf) > (FirstOperand & 0xf))   // if four least significant bits of SecondOperand are greater than four
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of FirstOperand, a borrow has occurred at bit 3...
                else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }

            // set OF to correct state
            byte temp = (byte)(FirstOperand & 0x80);
            if (temp == (SecondOperand & 0x80))                 // if both operands have the same sign, no chance of overflow... clear the OF flag
                ClearFlag(FlagsEnum.OF_OverflowFlag);
            else
            {                                                   // if the operands have different signs, verify that the result has the same sign
                if (temp != (result & 0x80))                    // as the first operand... if it doesn't, an overflow occurred... set the OF flag
                    SetFlag(FlagsEnum.OF_OverflowFlag);
                else
                    ClearFlag(FlagsEnum.OF_OverflowFlag);
            }

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort SubTwoWordsWithBorrowAndSetFlags(ushort FirstOperand, ushort SecondOperand)
        {
            ushort result;

            if (GetFlag(FlagsEnum.CF_CarryFlag))
            {
                result = (ushort)(FirstOperand - SecondOperand - 1);

                // set CF to correct state
                if (SecondOperand >= FirstOperand)                  // if SecondOperand is greater, or equal (since we're subtracting the borrow),
                    SetFlag(FlagsEnum.CF_CarryFlag);                // the subtraction will result in a borrow
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((SecondOperand & 0xf) >= (FirstOperand & 0xf))  // if four least significant bits of SecondOperand are greater than, or equal to,
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // the four least significant bits of FirstOperand, a borrow has occurred at bit 3...
                else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }
            else
            {
                result = (ushort)(FirstOperand - SecondOperand);

                // set CF to correct state
                if (SecondOperand > FirstOperand)                   // if SecondOperand is greater, or equal, the subtraction will result in a borrow
                    SetFlag(FlagsEnum.CF_CarryFlag);
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((SecondOperand & 0xf) > (FirstOperand & 0xf))   // if four least significant bits of SecondOperand are greater than four
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of FirstOperand, a borrow has occurred at bit 3...
                else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }

            // set OF to correct state
            ushort temp = (ushort)(FirstOperand & 0x8000);
            if (temp == (SecondOperand & 0x8000))               // if both operands have the same sign, no chance of overflow... clear the OF flag
                ClearFlag(FlagsEnum.OF_OverflowFlag);
            else
            {                                                   // if the operands have different signs, verify that the result has the same sign
                if (temp != (result & 0x8000))                  // as the first operand... if it doesn't, an overflow occurred... set the OF flag
                    SetFlag(FlagsEnum.OF_OverflowFlag);
                else
                    ClearFlag(FlagsEnum.OF_OverflowFlag);
            }

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint SubTwoDwordsWithBorrowAndSetFlags(uint FirstOperand, uint SecondOperand)
        {
            uint result;

            if (GetFlag(FlagsEnum.CF_CarryFlag))                    // if borrow flag is set, the CF and AF setting logic is slightly different
            {
                result = FirstOperand - SecondOperand - 1;

                // set CF to correct state
                if (SecondOperand >= FirstOperand)                  // if SecondOperand is greater, or equal (since we're subtracting the borrow),
                    SetFlag(FlagsEnum.CF_CarryFlag);                // the subtraction will result in a borrow
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((SecondOperand & 0xf) >= (FirstOperand & 0xf))  // if four least significant bits of SecondOperand are greater than, or equal to,
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // the four least significant bits of FirstOperand, a borrow has occurred at bit 3...
                else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }
            else
            {
                result = FirstOperand - SecondOperand;

                // set CF to correct state
                if (SecondOperand > FirstOperand)                   // if SecondOperand is greater, or equal, the subtraction will result in a borrow
                    SetFlag(FlagsEnum.CF_CarryFlag);
                else
                    ClearFlag(FlagsEnum.CF_CarryFlag);

                // set AF to correct state                          
                if ((SecondOperand & 0xf) > (FirstOperand & 0xf))   // if four least significant bits of SecondOperand are greater than four
                    SetFlag(FlagsEnum.AF_AuxCarryFlag);             // least significant bits of FirstOperand, a borrow has occurred at bit 3...
                else                                                // set the AuxCarryFlag (aka. Adjust Flag)
                    ClearFlag(FlagsEnum.AF_AuxCarryFlag);
            }

            // set OF to correct state
            uint temp = FirstOperand & 0x80000000;
            if (temp == (SecondOperand & 0x80000000))           // if both operands have the same sign, no chance of overflow... clear the OF flag
                ClearFlag(FlagsEnum.OF_OverflowFlag);
            else
            {                                                   // if the operands have different signs, verify that the result has the same sign
                if (temp != (result & 0x80000000))              // as the first operand... if it doesn't, an overflow occurred... set the OF flag
                    SetFlag(FlagsEnum.OF_OverflowFlag);
                else
                    ClearFlag(FlagsEnum.OF_OverflowFlag);
            }

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }


        #endregion

        #region Dec* Methods


        private byte DecByteAndSetFlags(byte Value)
        {
            byte result = (byte)(Value - 1);

            // set OF to correct state
            if (Value == 0x80)                                  // if Value is 0x80 (-128), subtracting 1 will cause it to become +127... overflow!
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) == 0)                             // if four least significant bits of the original value are off, a borrow will occur
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // into bit 3, otherwise no borrow will occur, since we're only subtracting by 1.
            else                                                // if a borrow into bit 3 occurred, set AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort DecWordAndSetFlags(ushort Value)
        {
            ushort result = (ushort)(Value - 1);

            // set OF to correct state
            if (Value == 0x8000)                                  // if Value is 0x8000, subtracting 1 will cause it to become positive... overflow!
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) == 0)                             // if four least significant bits of the original value are off, a borrow will occur
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // into bit 3, otherwise no borrow will occur, since we're only subtracting by 1.
            else                                                // if a borrow into bit 3 occurred, set AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint DecDwordAndSetFlags(uint Value)
        {
            uint result = Value - 1;

            // set OF to correct state
            if (Value == 0x80000000)                            // if Value is 0x80000000, subtracting 1 will cause it to become positive... overflow!
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) == 0)                             // if four least significant bits of the original value are off, a borrow will occur
                SetFlag(FlagsEnum.AF_AuxCarryFlag);             // into bit 3, otherwise no borrow will occur, since we're only subtracting by 1.
            else                                                // if a borrow into bit 3 occurred, set AuxCarryFlag (aka. Adjust Flag)
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }


        #endregion

        #region Neg* Methods


        private byte NegByteAndSetFlags(byte Value)
        {
            byte result = (byte)(0 - Value);

            // set CF to correct state
            if (Value != 0)                                     // if value is not 0, carry occurred... since we're subtracting from 0
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            if (Value == 0x80)                                  // if value is smallest negative number, an overflow will occur
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) != 0)                             // if four least significant bits of value don't equal 0, borrow will occur into bit 3
                SetFlag(FlagsEnum.AF_AuxCarryFlag);
            else
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByByte(result);
            SetZeroFlagByByte(result);
            SetParityFlagByByte(result);

            return result;
        }

        private ushort NegWordAndSetFlags(ushort Value)
        {
            ushort result = (ushort)(0 - Value);

            // set CF to correct state
            if (Value != 0)                                     // if value is not 0, carry occurred... since we're subtracting from 0
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            if (Value == 0x8000)                                // if value is smallest negative number, an overflow will occur
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) != 0)                             // if four least significant bits of value don't equal 0, borrow will occur into bit 3
                SetFlag(FlagsEnum.AF_AuxCarryFlag);
            else
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByWord(result);
            SetZeroFlagByWord(result);
            SetParityFlagByWord(result);

            return result;
        }

        private uint NegDwordAndSetFlags(uint Value)
        {
            uint result = 0 - Value;

            // set CF to correct state
            if (Value != 0)                                     // if value is not 0, carry occurred... since we're subtracting from 0
                SetFlag(FlagsEnum.CF_CarryFlag);
            else
                ClearFlag(FlagsEnum.CF_CarryFlag);

            // set OF to correct state
            if (Value == 0x80000000)                            // if value is smallest negative number, an overflow will occur
                SetFlag(FlagsEnum.OF_OverflowFlag);
            else
                ClearFlag(FlagsEnum.OF_OverflowFlag);

            // set AF to correct state                          
            if ((Value & 0xf) != 0)                             // if four least significant bits of value don't equal 0, borrow will occur into bit 3
                SetFlag(FlagsEnum.AF_AuxCarryFlag);
            else
                ClearFlag(FlagsEnum.AF_AuxCarryFlag);

            SetSignFlagByDword(result);
            SetZeroFlagByDword(result);
            SetParityFlagByDword(result);

            return result;
        }


        #endregion

        private ulong GetRealAddress(SegmentEnum Segment, ulong Address)
        // gets the linear address which will be sent to the memory controller
        {
            ushort segValue = 0;

            switch (Segment)
            {
                case SegmentEnum.CS:
                    segValue = cs;
                    break;

                case SegmentEnum.DS:
                    segValue = ds;
                    break;

                case SegmentEnum.ES:
                    segValue = es;
                    break;

                case SegmentEnum.FS:
                    segValue = fs;
                    break;

                case SegmentEnum.GS:
                    segValue = gs;
                    break;

                case SegmentEnum.SS:
                    segValue = ss;
                    break;
            }

            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    return ((ulong)(segValue << 4) + Address) & (uint)(maskA20 ? 0xfffff : 0x1fffff);

                case OpModeEnum.ProtectedMode:
                    return (uint)Address;

                case OpModeEnum.IA32e_CompatibilityMode:
                    return (uint)Address;

                default:                    // OpMode.IA32e_64bitMode:
                    return Address;
            }
        }

        private void SetOpMode(OpModeEnum OpMode)
        // sets the operational mode of the processor (Real Mode, Protected Mode, Long Mode, etc.)
        {
            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    addressSize = AddressSizeEnum.Bits16;
                    operandSize = OperandSizeEnum.Bits16;
                    break;

                case OpModeEnum.ProtectedMode:
                    addressSize = AddressSizeEnum.Bits32;
                    operandSize = OperandSizeEnum.Bits32;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    addressSize = AddressSizeEnum.Bits32;
                    operandSize = OperandSizeEnum.Bits32;
                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    addressSize = AddressSizeEnum.Bits64;
                    operandSize = OperandSizeEnum.Bits32;
                    break;
            }           

            opMode = OpMode;
        }

        private AddressSizeEnum GetCurrentAddressMode()
        // returns the current address mode (default address mode or, if overridden, the effective address mode)
        {
            if (addressSize == AddressSizeEnum.Bits16)
            {
                if (addressSizeOverride == true)
                    return AddressSizeEnum.Bits32;
                else
                    return AddressSizeEnum.Bits16;
            }
            else if (addressSize == AddressSizeEnum.Bits32)
            {
                if (addressSizeOverride == true)
                    return AddressSizeEnum.Bits16;
                else
                    return AddressSizeEnum.Bits32;
            }
            else // (addressSize == AddressSizeEnum.Bits64)
            {
                return AddressSizeEnum.Bits64;
            }
        }

        private MemoryOffset GetMemoryOffset(ulong Address)
        // abstracts the addressing mode from the instruction functions
        {
            MemoryOffset m;
            m.Offset = 0;
            m.Length = 0;

            if (GetCurrentAddressMode() == AddressSizeEnum.Bits16)
            {
                m.Offset = GetWord(SegmentEnum.CS, Address);
                m.Length = 2;
            }
            else if (GetCurrentAddressMode() == AddressSizeEnum.Bits32)
            {
                m.Offset = GetDword(SegmentEnum.CS, Address);
                m.Length = 4;
            }

            return m;
        }

        #region Instruction Pointer Methods


        private ulong GetInstructionPointer(int Offset)
        // serves to abstract the processor's operational mode from the instruction functions
        {
            ulong pointer = 0;

            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    pointer = (ushort)(ip + Offset);
                    break;

                case OpModeEnum.ProtectedMode:
                    pointer = (uint)(eip + Offset);
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:

                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    pointer = rip + (ulong)Offset;
                    break;
            }

            return pointer;
        }

        private ulong GetTempInstructionPointer()
        // serves to abstract the processor's operational mode from the instruction functions
        {
            ulong pointer = 0;

            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    pointer = temp_ip;
                    break;

                case OpModeEnum.ProtectedMode:
                    pointer = temp_eip;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:

                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    pointer = temp_rip;
                    break;
            }

            return pointer;
        }

        private void SetInstructionPointer(ulong Value)
        // serves to abstract the processor's operational mode from the instruction functions
        {
            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    ip = (ushort)Value;
                    break;

                case OpModeEnum.ProtectedMode:
                    eip = (uint)Value;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:

                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    rip = Value;
                    break;
            }
        }

        private void InstructionPointerJumpShort(sbyte Displacement)
        // serves to abstract the processor's operational mode from the jump short/near relative instructions
        {
            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    ip = (ushort)(temp_ip + Displacement);
                    break;

                case OpModeEnum.ProtectedMode:
                    eip = (uint)(temp_eip + Displacement);
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    break;
            }
        }

        private void InstructionPointerJumpRel16(short Displacement)
        // serves to abstract the processor's operational mode from the jump short/near relative instructions
        {
            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    ip = (ushort)(temp_ip + Displacement);
                    break;

                case OpModeEnum.ProtectedMode:
                    eip = (ushort)(temp_eip + Displacement);
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    break;
            }
        }

        private void InstructionPointerJumpRel32(int Displacement)
        // serves to abstract the processor's operational mode from the jump short/near relative instructions
        {
            switch (OpMode)
            {
                case OpModeEnum.RealAddressMode:
                    ip = (ushort)(temp_ip + Displacement);
                    break;

                case OpModeEnum.ProtectedMode:
                    eip = (uint)(temp_eip + Displacement);
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    break;

                default:                    // OpMode.IA32e_64bitMode:
                    break;
            }
        }


        #endregion

        #region Stack Pointer Methods


        private void IncrementStackPointerByWord()
        {
            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    ushort new_sp = (ushort)(sp + 2);
                    if (new_sp < sp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        sp = new_sp;
                    break;

                case OpModeEnum.ProtectedMode:
                    uint new_esp = esp + 2;
                    if (new_esp < esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    new_esp = esp + 2;
                    if (new_esp < esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                default:    // 64-bit mode
                    break;
            }
        }

        private void IncrementStackPointerByDword()
        {
            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    ushort new_sp = (ushort)(sp + 4);
                    if (new_sp < sp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        sp = new_sp;
                    break;

                case OpModeEnum.ProtectedMode:
                    uint new_esp = esp + 4;
                    if (new_esp < esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    new_esp = esp + 4;
                    if (new_esp < esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                default:    // 64-bit mode
                    break;
            }
        }

        private void DecrementStackPointerByWord()
        {
            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    ushort new_sp = (ushort)(sp - 2);
                    if (new_sp > sp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        sp = new_sp;
                    break;

                case OpModeEnum.ProtectedMode:
                    uint new_esp = esp - 2;
                    if (new_esp > esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    new_esp = esp - 2;
                    if (new_esp > esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                default:    // 64-bit mode
                    break;
            }
        }

        private void DecrementStackPointerByDword()
        {
            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    ushort new_sp = (ushort)(sp - 4);
                    if (new_sp > sp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        sp = new_sp;
                    break;

                case OpModeEnum.ProtectedMode:
                    uint new_esp = esp - 4;
                    if (new_esp > esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    new_esp = esp - 4;
                    if (new_esp > esp)
                        RaiseException(ExceptionEnum.StackFault, ExceptionDetailEnum.Null);
                    else
                        esp = new_esp;
                    break;

                default:    // 64-bit mode
                    break;
            }
        }

        private ulong GetStackPointer()
        {
            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    return sp;

                case OpModeEnum.ProtectedMode:
                    return esp;

                case OpModeEnum.IA32e_CompatibilityMode:
                    return esp;

                default:    // 64-bit mode
                    return rip;
            }
        }


        #endregion

        private SegmentEnum GetEffectiveSegment(SegmentEnum PreferredSegment)
        // returns PreferredSegment, unless a segment override prefix has been encountered... in which case it returns the overriding segment
        {
            if (!segmentOverride.Active)
            {
                return PreferredSegment;
            }
            else
            {
                return segmentOverride.Value;
            }
        }

        private void RaiseException(ExceptionEnum Exception, ExceptionDetailEnum ExceptionDetail)
        // processor's exception mechanism
        {
            string exception;
            string message = string.Empty;

            if ((Exception == ExceptionEnum.GeneralProtection) && (opMode == OpModeEnum.RealAddressMode))
                exception = "SegmentOverrun";
            else
                exception = Exception.ToString();

            message = "CPU Exception Occurred!\n\nException #" + ((int)Exception).ToString() + " - " + exception;


            if (Exception != ExceptionEnum.InvalidOpcode)
            {
                if (opMode == OpModeEnum.RealAddressMode)
                {
                    message += "\n\nCS : IP = " + string.Format("0x{0:X4} : 0x{1:X4}", cs, old_ip);
                }
                else if (opMode == OpModeEnum.ProtectedMode)
                {
                    message += "\n\nCS : EIP = " + string.Format("0x{0:X4} : 0x{1:X8}", cs, old_eip);
                }
            }
            else
            {
                switch (opMode)
                {
                    case OpModeEnum.RealAddressMode:
                        message += String.Format("\n\nOpcode = 0x{0:X2}", GetByte(SegmentEnum.CS, GetInstructionPointer(prefixLength)));
                        message += String.Format("\nCS : IP = 0x{0:X4} : 0x{1:X4}", cs, GetInstructionPointer(prefixLength));
                        break;

                    case OpModeEnum.ProtectedMode:
                        message += String.Format("\n\nOpcode = 0x{0:X2}", GetByte(SegmentEnum.CS, GetInstructionPointer(prefixLength)));
                        message += String.Format("\nCS : IP = 0x{0:X4} : 0x{1:X8}", cs, GetInstructionPointer(prefixLength));
                        break;

                    case OpModeEnum.IA32e_CompatibilityMode:
                        break;

                    default:    // OpModeEnum.IA32e_64bitMode
                        break;
                }
            }

            if (ExceptionDetail != ExceptionDetailEnum.Null)
                message += "\n\n\"" + exceptionDetailTable[(int)ExceptionDetail] + "\"";

            throw new System.Exception(message);
        }

        private void InitializeExceptionDetailTable()
        {
            exceptionDetailTable = new string[100];
            exceptionDetailTable[(int)ExceptionDetailEnum.Null] = "";
            exceptionDetailTable[(int)ExceptionDetailEnum.AttemptedToLoadCS] = "Instruction attempted to load CS segment register.";
            exceptionDetailTable[(int)ExceptionDetailEnum.InstructionPointerSegmentOverrun] = "Instruction pointer segment overrun.";
            exceptionDetailTable[(int)ExceptionDetailEnum.InvalidModValueInModRM] = "Invalid Mod value in ModR/M byte for the current instruction.";
        }

        private void OpcodeAndDigitNotYetSupported(ulong Opcode, ulong Digit)
        {
            MessageBox.Show(Form1.ActiveForm, String.Format("Opcode {0:X} /{1} not yet supported!", Opcode, Digit));

            switch (opMode)
            {
                case OpModeEnum.RealAddressMode:
                    temp_ip = ip;
                    break;

                case OpModeEnum.ProtectedMode:
                    temp_eip = eip;
                    break;

                case OpModeEnum.IA32e_CompatibilityMode:
                    temp_eip = eip;
                    break;

                case OpModeEnum.IA32e_64bitMode:
                    temp_rip = rip;
                    break;
            }

            Halt();
        }


        #endregion

    }
}

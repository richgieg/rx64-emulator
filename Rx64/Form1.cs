using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Rx64
{
    public partial class Form1 : Form
    {
        private VirtualMachine vm;
        private Thread debug_output_thread;

        public Form1()
        {
            InitializeComponent();

            vm = new VirtualMachine(textModeScreen);
            debug_output_thread = new Thread(updateDebugOutput);
            debug_output_thread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            vm.PowerOff();

            debug_output_thread.Abort();
        }

        private void btn_NextInstruction_Click(object sender, EventArgs e)
        {
            vm.cpu0.Step();
        }

        private void btn_SingleStepping_Click(object sender, EventArgs e)
        {
            if (vm.cpu0.SingleStepping)
            {
               btn_SingleStepping.Text = "Single Stepping: Off";
               vm.cpu0.SingleStepping = false;
            }
            else
            {
                btn_SingleStepping.Text = "Single Stepping: On";
                vm.cpu0.SingleStepping = true;
            }   
        }

        private void updateDebugOutput()
        {
            StringBuilder regs = new StringBuilder();

            while (true)
            {
                regs = new StringBuilder();

                regs.Append("Available Memory: " + vm.mch.MegabytesAvailable.ToString() + " MB").AppendLine();
                for (int i = 0; i < vm.mch.MemoryModuleSizes.Length; i ++)
                {
                    regs.Append("Slot " + i.ToString() + ": " + vm.mch.MemoryModuleSizes[i].ToString() + " bytes   ");
                }
                regs.AppendLine().AppendLine();

                regs.Append("rax=" + String.Format("{0:x16} ", vm.cpu0.R64_RAX));
                regs.Append("rbx=" + String.Format("{0:x16} ", vm.cpu0.R64_RBX));
                regs.Append("rcx=" + String.Format("{0:x16} ", vm.cpu0.R64_RCX)).AppendLine();
                regs.Append("rdx=" + String.Format("{0:x16} ", vm.cpu0.R64_RDX));
                regs.Append("rsi=" + String.Format("{0:x16} ", vm.cpu0.R64_RSI));
                regs.Append("rdi=" + String.Format("{0:x16} ", vm.cpu0.R64_RDI)).AppendLine();

                regs.Append("rip=" + String.Format("{0:x16} ", vm.cpu0.R64_RIP));
                regs.Append("rsp=" + String.Format("{0:x16} ", vm.cpu0.R64_RSP));
                regs.Append("rbp=" + String.Format("{0:x16} ", vm.cpu0.R64_RBP)).AppendLine();

                string flags = string.Empty;
                flags += "iopl=" + vm.cpu0.GetIOPrivilegeLevel().ToString() + " ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.VIP_VirtualInterruptPending) ? "vip " : "    ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.VIF_VirtualInterruptFlag) ? "vif " : "    ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.OF_OverflowFlag) ? "ov " : "nv ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.DF_DirectionFlag) ? "dn " : "up ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.IF_InterruptEnableFlag) ? "ei " : "di ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.SF_SignFlag) ? "ng " : "pl ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.ZF_ZeroFlag) ? "zr " : "nz ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.AF_AuxCarryFlag) ? "ac " : "na ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.PF_ParityFlag) ? "pe " : "po ";
                flags += vm.cpu0.GetFlag(CPU.FlagsEnum.CF_CarryFlag) ? "cy " : "nc ";
                regs.Append(flags).AppendLine();

                regs.Append("cs=" + String.Format("{0:x4}  ", vm.cpu0.R_CS));
                regs.Append("ss=" + String.Format("{0:x4}  ", vm.cpu0.R_SS));
                regs.Append("ds=" + String.Format("{0:x4}  ", vm.cpu0.R_DS));
                regs.Append("es=" + String.Format("{0:x4}  ", vm.cpu0.R_ES));
                regs.Append("fs=" + String.Format("{0:x4}  ", vm.cpu0.R_FS));
                regs.Append("gs=" + String.Format("{0:x4}  ", vm.cpu0.R_GS));
                    
                regs.Append("           efl=" + String.Format("0x{0:x8}  ", vm.cpu0.R32_EFLAGS));
                    
                label_Register_Status.Text = regs.ToString();
                Thread.Sleep(10);
            }
        }

        private void btn_MemoryDump_Click(object sender, EventArgs e)
        {
            ulong start = 0;
            ulong end = vm.mch.BytesAvailable;

            if (cb_MemDumpStart.Checked)
                start = Convert.ToUInt64(tb_MemDumpStart.Text) * 1024 * 1024;

            if (cb_MemDumpEnd.Checked)
                end = Convert.ToUInt64(tb_MemDumpEnd.Text) * 1024 * 1024;

            SaveFileDialog save = new SaveFileDialog();
            save.ShowDialog();

            if (save.FileName != String.Empty)
            {
                vm.WriteMemoryDump(start, end, save.FileName);
                MessageBox.Show(Form1.ActiveForm, "Dump file written successfully.");
            }

            save.Dispose();
        }

        private void trackbar_ExecutionSpeed_ValueChanged(object sender, EventArgs e)
        {
            vm.cpu0.ExecutionDelay = (ushort)(100 - (trackbar_ExecutionSpeed.Value * 10));
        }
    }
}

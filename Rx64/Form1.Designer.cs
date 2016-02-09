namespace Rx64
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textModeScreen = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_NextInstruction = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label_Register_Status = new System.Windows.Forms.Label();
            this.btn_SingleStepping = new System.Windows.Forms.Button();
            this.btn_MemoryDump = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_MemDumpEnd = new System.Windows.Forms.TextBox();
            this.cb_MemDumpEnd = new System.Windows.Forms.CheckBox();
            this.tb_MemDumpStart = new System.Windows.Forms.TextBox();
            this.cb_MemDumpStart = new System.Windows.Forms.CheckBox();
            this.trackbar_ExecutionSpeed = new System.Windows.Forms.TrackBar();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_ExecutionSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // textModeScreen
            // 
            this.textModeScreen.AutoSize = true;
            this.textModeScreen.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textModeScreen.ForeColor = System.Drawing.Color.White;
            this.textModeScreen.Location = new System.Drawing.Point(-3, 0);
            this.textModeScreen.Name = "textModeScreen";
            this.textModeScreen.Size = new System.Drawing.Size(138, 18);
            this.textModeScreen.TabIndex = 0;
            this.textModeScreen.Text = "RichBIOS v0.1";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.textModeScreen);
            this.panel3.Location = new System.Drawing.Point(13, 16);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(807, 455);
            this.panel3.TabIndex = 2;
            // 
            // btn_NextInstruction
            // 
            this.btn_NextInstruction.Location = new System.Drawing.Point(156, 655);
            this.btn_NextInstruction.Name = "btn_NextInstruction";
            this.btn_NextInstruction.Size = new System.Drawing.Size(68, 23);
            this.btn_NextInstruction.TabIndex = 1;
            this.btn_NextInstruction.Text = "Step";
            this.btn_NextInstruction.UseVisualStyleBackColor = true;
            this.btn_NextInstruction.Click += new System.EventHandler(this.btn_NextInstruction_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label_Register_Status);
            this.panel2.Location = new System.Drawing.Point(13, 489);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(807, 150);
            this.panel2.TabIndex = 2;
            // 
            // label_Register_Status
            // 
            this.label_Register_Status.AutoSize = true;
            this.label_Register_Status.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Register_Status.ForeColor = System.Drawing.Color.Black;
            this.label_Register_Status.Location = new System.Drawing.Point(4, 4);
            this.label_Register_Status.Name = "label_Register_Status";
            this.label_Register_Status.Size = new System.Drawing.Size(176, 17);
            this.label_Register_Status.TabIndex = 0;
            this.label_Register_Status.Text = "label_Register_Status";
            // 
            // btn_SingleStepping
            // 
            this.btn_SingleStepping.Location = new System.Drawing.Point(12, 655);
            this.btn_SingleStepping.Name = "btn_SingleStepping";
            this.btn_SingleStepping.Size = new System.Drawing.Size(138, 23);
            this.btn_SingleStepping.TabIndex = 3;
            this.btn_SingleStepping.Text = "Single Stepping: On";
            this.btn_SingleStepping.UseVisualStyleBackColor = true;
            this.btn_SingleStepping.Click += new System.EventHandler(this.btn_SingleStepping_Click);
            // 
            // btn_MemoryDump
            // 
            this.btn_MemoryDump.Location = new System.Drawing.Point(154, 24);
            this.btn_MemoryDump.Name = "btn_MemoryDump";
            this.btn_MemoryDump.Size = new System.Drawing.Size(70, 43);
            this.btn_MemoryDump.TabIndex = 4;
            this.btn_MemoryDump.Text = "Create";
            this.btn_MemoryDump.UseVisualStyleBackColor = true;
            this.btn_MemoryDump.Click += new System.EventHandler(this.btn_MemoryDump_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_MemDumpEnd);
            this.groupBox1.Controls.Add(this.cb_MemDumpEnd);
            this.groupBox1.Controls.Add(this.tb_MemDumpStart);
            this.groupBox1.Controls.Add(this.cb_MemDumpStart);
            this.groupBox1.Controls.Add(this.btn_MemoryDump);
            this.groupBox1.Location = new System.Drawing.Point(584, 655);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 83);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory Dump";
            // 
            // tb_MemDumpEnd
            // 
            this.tb_MemDumpEnd.Location = new System.Drawing.Point(91, 47);
            this.tb_MemDumpEnd.Name = "tb_MemDumpEnd";
            this.tb_MemDumpEnd.Size = new System.Drawing.Size(47, 20);
            this.tb_MemDumpEnd.TabIndex = 8;
            // 
            // cb_MemDumpEnd
            // 
            this.cb_MemDumpEnd.AutoSize = true;
            this.cb_MemDumpEnd.Location = new System.Drawing.Point(12, 49);
            this.cb_MemDumpEnd.Name = "cb_MemDumpEnd";
            this.cb_MemDumpEnd.Size = new System.Drawing.Size(73, 17);
            this.cb_MemDumpEnd.TabIndex = 7;
            this.cb_MemDumpEnd.Text = "End (MB):";
            this.cb_MemDumpEnd.UseVisualStyleBackColor = true;
            // 
            // tb_MemDumpStart
            // 
            this.tb_MemDumpStart.Location = new System.Drawing.Point(91, 24);
            this.tb_MemDumpStart.Name = "tb_MemDumpStart";
            this.tb_MemDumpStart.Size = new System.Drawing.Size(47, 20);
            this.tb_MemDumpStart.TabIndex = 6;
            // 
            // cb_MemDumpStart
            // 
            this.cb_MemDumpStart.AutoSize = true;
            this.cb_MemDumpStart.Location = new System.Drawing.Point(12, 26);
            this.cb_MemDumpStart.Name = "cb_MemDumpStart";
            this.cb_MemDumpStart.Size = new System.Drawing.Size(76, 17);
            this.cb_MemDumpStart.TabIndex = 5;
            this.cb_MemDumpStart.Text = "Start (MB):";
            this.cb_MemDumpStart.UseVisualStyleBackColor = true;
            // 
            // trackbar_ExecutionSpeed
            // 
            this.trackbar_ExecutionSpeed.Location = new System.Drawing.Point(12, 693);
            this.trackbar_ExecutionSpeed.Name = "trackbar_ExecutionSpeed";
            this.trackbar_ExecutionSpeed.Size = new System.Drawing.Size(212, 45);
            this.trackbar_ExecutionSpeed.TabIndex = 6;
            this.trackbar_ExecutionSpeed.ValueChanged += new System.EventHandler(this.trackbar_ExecutionSpeed_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(833, 750);
            this.Controls.Add(this.trackbar_ExecutionSpeed);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_SingleStepping);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btn_NextInstruction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Rx64 - PC Emulator (Intel 64 CPU Architecture)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_ExecutionSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label textModeScreen;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_NextInstruction;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label_Register_Status;
        private System.Windows.Forms.Button btn_SingleStepping;
        private System.Windows.Forms.Button btn_MemoryDump;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_MemDumpStart;
        private System.Windows.Forms.TextBox tb_MemDumpStart;
        private System.Windows.Forms.TextBox tb_MemDumpEnd;
        private System.Windows.Forms.CheckBox cb_MemDumpEnd;
        private System.Windows.Forms.TrackBar trackbar_ExecutionSpeed;
    }
}


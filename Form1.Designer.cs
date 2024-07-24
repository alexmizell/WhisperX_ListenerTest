using System;

namespace WhisperX_ListenerTest
{
    partial class formMain
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
            this.textOutput = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.cbMomentary = new System.Windows.Forms.CheckBox();
            this.comboInputs = new System.Windows.Forms.ComboBox();
            this.lblInput = new System.Windows.Forms.Label();
            this.progressBarAudioLevel = new System.Windows.Forms.ProgressBar();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnListen = new System.Windows.Forms.Button();
            this.textPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.cbConnected = new System.Windows.Forms.CheckBox();
            this.cbStream = new System.Windows.Forms.CheckBox();
            this.textHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.tbGain = new System.Windows.Forms.TrackBar();
            this.lblGain = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbGain)).BeginInit();
            this.SuspendLayout();
            // 
            // textOutput
            // 
            this.textOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textOutput.Location = new System.Drawing.Point(279, 36);
            this.textOutput.Multiline = true;
            this.textOutput.Name = "textOutput";
            this.textOutput.Size = new System.Drawing.Size(497, 397);
            this.textOutput.TabIndex = 0;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(277, 20);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(39, 13);
            this.lblOutput.TabIndex = 1;
            this.lblOutput.Text = "Output";
            // 
            // cbMomentary
            // 
            this.cbMomentary.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMomentary.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbMomentary.Location = new System.Drawing.Point(61, 91);
            this.cbMomentary.Name = "cbMomentary";
            this.cbMomentary.Size = new System.Drawing.Size(152, 40);
            this.cbMomentary.TabIndex = 3;
            this.cbMomentary.Text = "Hold To Listen";
            this.cbMomentary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMomentary.UseVisualStyleBackColor = true;
            // 
            // comboInputs
            // 
            this.comboInputs.FormattingEnabled = true;
            this.comboInputs.Location = new System.Drawing.Point(15, 37);
            this.comboInputs.Name = "comboInputs";
            this.comboInputs.Size = new System.Drawing.Size(246, 21);
            this.comboInputs.TabIndex = 4;
            this.comboInputs.SelectedIndexChanged += new System.EventHandler(this.comboInputs_SelectedIndexChanged);
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(12, 20);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(31, 13);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Input";
            // 
            // progressBarAudioLevel
            // 
            this.progressBarAudioLevel.Location = new System.Drawing.Point(15, 64);
            this.progressBarAudioLevel.Margin = new System.Windows.Forms.Padding(2);
            this.progressBarAudioLevel.Maximum = 32768;
            this.progressBarAudioLevel.Name = "progressBarAudioLevel";
            this.progressBarAudioLevel.Size = new System.Drawing.Size(246, 19);
            this.progressBarAudioLevel.TabIndex = 5;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(177, 376);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(84, 47);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(87, 376);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(84, 47);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "Listen";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Visible = false;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(216, 182);
            this.textPort.Margin = new System.Windows.Forms.Padding(2);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(40, 20);
            this.textPort.TabIndex = 6;
            this.textPort.Text = "43007";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(163, 185);
            this.lblPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(53, 13);
            this.lblPort.TabIndex = 7;
            this.lblPort.Text = "TCP Port:";
            // 
            // cbConnected
            // 
            this.cbConnected.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbConnected.AutoSize = true;
            this.cbConnected.Location = new System.Drawing.Point(96, 149);
            this.cbConnected.Margin = new System.Windows.Forms.Padding(2);
            this.cbConnected.Name = "cbConnected";
            this.cbConnected.Size = new System.Drawing.Size(84, 23);
            this.cbConnected.TabIndex = 8;
            this.cbConnected.Text = "CONNECTED";
            this.cbConnected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbConnected.UseVisualStyleBackColor = true;
            this.cbConnected.CheckedChanged += new System.EventHandler(this.cbConnected_CheckedChanged);
            // 
            // cbStream
            // 
            this.cbStream.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbStream.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbStream.Location = new System.Drawing.Point(61, 209);
            this.cbStream.Name = "cbStream";
            this.cbStream.Size = new System.Drawing.Size(152, 40);
            this.cbStream.TabIndex = 3;
            this.cbStream.Text = "Hold To Stream";
            this.cbStream.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbStream.UseVisualStyleBackColor = true;
            // 
            // textHost
            // 
            this.textHost.Location = new System.Drawing.Point(46, 181);
            this.textHost.Margin = new System.Windows.Forms.Padding(2);
            this.textHost.Name = "textHost";
            this.textHost.Size = new System.Drawing.Size(111, 20);
            this.textHost.TabIndex = 6;
            this.textHost.Text = "localhost";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(14, 183);
            this.lblHost.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(32, 13);
            this.lblHost.TabIndex = 7;
            this.lblHost.Text = "Host:";
            // 
            // tbGain
            // 
            this.tbGain.Location = new System.Drawing.Point(27, 319);
            this.tbGain.Maximum = 100;
            this.tbGain.Name = "tbGain";
            this.tbGain.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbGain.Size = new System.Drawing.Size(45, 104);
            this.tbGain.TabIndex = 9;
            this.tbGain.TickFrequency = 20;
            this.tbGain.Value = 95;
            this.tbGain.Visible = false;
            this.tbGain.Scroll += new System.EventHandler(this.tbGain_Scroll);
            // 
            // lblGain
            // 
            this.lblGain.AutoSize = true;
            this.lblGain.Location = new System.Drawing.Point(16, 421);
            this.lblGain.Name = "lblGain";
            this.lblGain.Size = new System.Drawing.Size(49, 13);
            this.lblGain.TabIndex = 10;
            this.lblGain.Text = "Mic Gain";
            this.lblGain.Visible = false;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 446);
            this.Controls.Add(this.lblGain);
            this.Controls.Add(this.tbGain);
            this.Controls.Add(this.cbConnected);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.textHost);
            this.Controls.Add(this.textPort);
            this.Controls.Add(this.progressBarAudioLevel);
            this.Controls.Add(this.comboInputs);
            this.Controls.Add(this.cbStream);
            this.Controls.Add(this.cbMomentary);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.textOutput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "formMain";
            this.Text = "WhisperX Listener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tbGain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void comboInputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.TextBox textOutput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.CheckBox cbMomentary;
        private System.Windows.Forms.ComboBox comboInputs;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.ProgressBar progressBarAudioLevel;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.CheckBox cbConnected;
        private System.Windows.Forms.CheckBox cbStream;
        private System.Windows.Forms.TextBox textHost;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TrackBar tbGain;
        private System.Windows.Forms.Label lblGain;
    }
}


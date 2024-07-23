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
            this.btnListen = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.cbMomentary = new System.Windows.Forms.CheckBox();
            this.comboInputs = new System.Windows.Forms.ComboBox();
            this.lblInput = new System.Windows.Forms.Label();
            this.progressBarAudioLevel = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textOutput
            // 
            this.textOutput.Location = new System.Drawing.Point(512, 66);
            this.textOutput.Margin = new System.Windows.Forms.Padding(6);
            this.textOutput.Multiline = true;
            this.textOutput.Name = "textOutput";
            this.textOutput.Size = new System.Drawing.Size(908, 730);
            this.textOutput.TabIndex = 0;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(507, 37);
            this.lblOutput.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(71, 25);
            this.lblOutput.TabIndex = 1;
            this.lblOutput.Text = "Output";
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(112, 596);
            this.btnListen.Margin = new System.Windows.Forms.Padding(6);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(279, 87);
            this.btnListen.TabIndex = 2;
            this.btnListen.Text = "Listen";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Visible = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(112, 695);
            this.btnStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(279, 87);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            // 
            // cbMomentary
            // 
            this.cbMomentary.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMomentary.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbMomentary.Location = new System.Drawing.Point(112, 189);
            this.cbMomentary.Margin = new System.Windows.Forms.Padding(6);
            this.cbMomentary.Name = "cbMomentary";
            this.cbMomentary.Size = new System.Drawing.Size(279, 73);
            this.cbMomentary.TabIndex = 3;
            this.cbMomentary.Text = "Listen";
            this.cbMomentary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMomentary.UseVisualStyleBackColor = true;
            // 
            // comboInputs
            // 
            this.comboInputs.FormattingEnabled = true;
            this.comboInputs.Location = new System.Drawing.Point(28, 68);
            this.comboInputs.Margin = new System.Windows.Forms.Padding(6);
            this.comboInputs.Name = "comboInputs";
            this.comboInputs.Size = new System.Drawing.Size(447, 32);
            this.comboInputs.TabIndex = 4;
            this.comboInputs.SelectedIndexChanged += new System.EventHandler(this.comboInputs_SelectedIndexChanged);
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(38, 37);
            this.lblInput.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(55, 25);
            this.lblInput.TabIndex = 1;
            this.lblInput.Text = "Input";
            // 
            // progressBarAudioLevel
            // 
            this.progressBarAudioLevel.Location = new System.Drawing.Point(28, 119);
            this.progressBarAudioLevel.Maximum = 32768;
            this.progressBarAudioLevel.Name = "progressBarAudioLevel";
            this.progressBarAudioLevel.Size = new System.Drawing.Size(447, 35);
            this.progressBarAudioLevel.TabIndex = 5;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1438, 824);
            this.Controls.Add(this.progressBarAudioLevel);
            this.Controls.Add(this.comboInputs);
            this.Controls.Add(this.cbMomentary);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.textOutput);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "formMain";
            this.Text = "WhisperX Listener";
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
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.CheckBox cbMomentary;
        private System.Windows.Forms.ComboBox comboInputs;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.ProgressBar progressBarAudioLevel;
    }
}


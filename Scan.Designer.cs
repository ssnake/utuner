namespace uTuner
{
    partial class Scan
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.channelsTreeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.grpChannelProp = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbChanVideoStandart = new System.Windows.Forms.ComboBox();
            this.edtFreq = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.edtName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.edtCurrFreq = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.edtStep = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.edtTo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.edtFrom = new System.Windows.Forms.TextBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.cbVideoStandarts = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.grpChannelProp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtFreq)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(157, 445);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(275, 445);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // channelsTreeView
            // 
            this.channelsTreeView.AllowDrop = true;
            this.channelsTreeView.ContextMenuStrip = this.contextMenuStrip1;
            this.channelsTreeView.Location = new System.Drawing.Point(12, 12);
            this.channelsTreeView.Name = "channelsTreeView";
            this.channelsTreeView.Size = new System.Drawing.Size(220, 411);
            this.channelsTreeView.TabIndex = 2;
            this.channelsTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.channelsTreeView_NodeMouseClick);
            this.channelsTreeView.Click += new System.EventHandler(this.channelsTreeView_Click);
            this.channelsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.channelsTreeView_DragEnter);
            this.channelsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.channelsTreeView_DragOver);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
            // 
            // miDelete
            // 
            this.miDelete.Name = "miDelete";
            this.miDelete.Size = new System.Drawing.Size(107, 22);
            this.miDelete.Text = "Delete";
            // 
            // grpChannelProp
            // 
            this.grpChannelProp.Controls.Add(this.label9);
            this.grpChannelProp.Controls.Add(this.cbChanVideoStandart);
            this.grpChannelProp.Controls.Add(this.edtFreq);
            this.grpChannelProp.Controls.Add(this.label6);
            this.grpChannelProp.Controls.Add(this.edtName);
            this.grpChannelProp.Controls.Add(this.label7);
            this.grpChannelProp.Controls.Add(this.lblID);
            this.grpChannelProp.Controls.Add(this.label5);
            this.grpChannelProp.Location = new System.Drawing.Point(238, 12);
            this.grpChannelProp.Name = "grpChannelProp";
            this.grpChannelProp.Size = new System.Drawing.Size(246, 174);
            this.grpChannelProp.TabIndex = 3;
            this.grpChannelProp.TabStop = false;
            this.grpChannelProp.Text = "Channel Properties";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Video Standarts";
            // 
            // cbChanVideoStandart
            // 
            this.cbChanVideoStandart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChanVideoStandart.FormattingEnabled = true;
            this.cbChanVideoStandart.Location = new System.Drawing.Point(107, 141);
            this.cbChanVideoStandart.Name = "cbChanVideoStandart";
            this.cbChanVideoStandart.Size = new System.Drawing.Size(120, 21);
            this.cbChanVideoStandart.TabIndex = 11;
            // 
            // edtFreq
            // 
            this.edtFreq.Location = new System.Drawing.Point(107, 99);
            this.edtFreq.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.edtFreq.Name = "edtFreq";
            this.edtFreq.Size = new System.Drawing.Size(120, 20);
            this.edtFreq.TabIndex = 6;
            this.edtFreq.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.edtFreq.ValueChanged += new System.EventHandler(this.edtFreq_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Freq.";
            // 
            // edtName
            // 
            this.edtName.Location = new System.Drawing.Point(107, 57);
            this.edtName.Name = "edtName";
            this.edtName.Size = new System.Drawing.Size(120, 20);
            this.edtName.TabIndex = 3;
            this.edtName.TextChanged += new System.EventHandler(this.edtName_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Name";
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(50, 25);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(35, 13);
            this.lblID.TabIndex = 1;
            this.lblID.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "ID: ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.edtCurrFreq);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.edtStep);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.edtTo);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.edtFrom);
            this.groupBox3.Location = new System.Drawing.Point(238, 192);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(246, 109);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Frequency  Scan Settings, Hz";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Current";
            // 
            // edtCurrFreq
            // 
            this.edtCurrFreq.Location = new System.Drawing.Point(47, 82);
            this.edtCurrFreq.Name = "edtCurrFreq";
            this.edtCurrFreq.Size = new System.Drawing.Size(82, 20);
            this.edtCurrFreq.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Step";
            // 
            // edtStep
            // 
            this.edtStep.Location = new System.Drawing.Point(173, 30);
            this.edtStep.Name = "edtStep";
            this.edtStep.Size = new System.Drawing.Size(67, 20);
            this.edtStep.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "To";
            // 
            // edtTo
            // 
            this.edtTo.Location = new System.Drawing.Point(47, 56);
            this.edtTo.Name = "edtTo";
            this.edtTo.Size = new System.Drawing.Size(82, 20);
            this.edtTo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "From";
            // 
            // edtFrom
            // 
            this.edtFrom.Location = new System.Drawing.Point(47, 30);
            this.edtFrom.Name = "edtFrom";
            this.edtFrom.Size = new System.Drawing.Size(82, 20);
            this.edtFrom.TabIndex = 0;
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(247, 307);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 23);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            // 
            // cbVideoStandarts
            // 
            this.cbVideoStandarts.FormattingEnabled = true;
            this.cbVideoStandarts.Location = new System.Drawing.Point(340, 328);
            this.cbVideoStandarts.Name = "cbVideoStandarts";
            this.cbVideoStandarts.Size = new System.Drawing.Size(127, 21);
            this.cbVideoStandarts.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(337, 312);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Video Standarts";
            // 
            // Scan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 480);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cbVideoStandarts);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.grpChannelProp);
            this.Controls.Add(this.channelsTreeView);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Scan";
            this.Text = "Scan";
            this.contextMenuStrip1.ResumeLayout(false);
            this.grpChannelProp.ResumeLayout(false);
            this.grpChannelProp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edtFreq)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Cancel;
        public System.Windows.Forms.TreeView channelsTreeView;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox edtStep;
        public System.Windows.Forms.TextBox edtTo;
        public System.Windows.Forms.TextBox edtFrom;
        public System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox edtCurrFreq;
        public System.Windows.Forms.NumericUpDown edtFreq;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox edtName;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.ComboBox cbVideoStandarts;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.ComboBox cbChanVideoStandart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.ToolStripMenuItem miDelete;
        public System.Windows.Forms.GroupBox grpChannelProp;
    }
}
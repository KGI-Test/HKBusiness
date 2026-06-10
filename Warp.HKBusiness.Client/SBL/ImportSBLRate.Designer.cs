namespace Warp.HKBusiness.Client
{
    partial class ImportSBLRate
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            this.gbQueryConditions = new Infragistics.Win.Misc.UltraGroupBox();
            this.lbImportDate = new Infragistics.Win.Misc.UltraLabel();
            this.dteDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtFileName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.ugDeposit = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.WinForm_Fill_Panel.ClientArea.SuspendLayout();
            this.WinForm_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbQueryConditions)).BeginInit();
            this.gbQueryConditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFileName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugDeposit)).BeginInit();
            this.SuspendLayout();
            // 
            // WinForm_Fill_Panel
            // 
            // 
            // WinForm_Fill_Panel.ClientArea
            // 
            this.WinForm_Fill_Panel.ClientArea.Controls.Add(this.ugDeposit);
            this.WinForm_Fill_Panel.ClientArea.Controls.Add(this.ultraSplitter1);
            this.WinForm_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox2);
            this.WinForm_Fill_Panel.ClientArea.Controls.Add(this.gbQueryConditions);
            this.WinForm_Fill_Panel.Location = new System.Drawing.Point(0, 80);
            this.WinForm_Fill_Panel.Margin = new System.Windows.Forms.Padding(6);
            this.WinForm_Fill_Panel.Size = new System.Drawing.Size(1722, 768);
            // 
            // gbQueryConditions
            // 
            this.gbQueryConditions.Controls.Add(this.lbImportDate);
            this.gbQueryConditions.Controls.Add(this.dteDate);
            this.gbQueryConditions.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbQueryConditions.Location = new System.Drawing.Point(0, 0);
            this.gbQueryConditions.Margin = new System.Windows.Forms.Padding(4);
            this.gbQueryConditions.Name = "gbQueryConditions";
            this.gbQueryConditions.Size = new System.Drawing.Size(1722, 69);
            this.gbQueryConditions.TabIndex = 2;
            this.gbQueryConditions.Text = "Query Conditions";
            // 
            // lbImportDate
            // 
            this.lbImportDate.Location = new System.Drawing.Point(123, 28);
            this.lbImportDate.Margin = new System.Windows.Forms.Padding(4);
            this.lbImportDate.Name = "lbImportDate";
            this.lbImportDate.Size = new System.Drawing.Size(66, 26);
            this.lbImportDate.TabIndex = 11;
            this.lbImportDate.Text = "Date";
            // 
            // dteDate
            // 
            this.dteDate.DateTime = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dteDate.FormatString = "";
            this.dteDate.Location = new System.Drawing.Point(204, 28);
            this.dteDate.Name = "dteDate";
            this.dteDate.PromptChar = ' ';
            this.dteDate.Size = new System.Drawing.Size(192, 28);
            this.dteDate.TabIndex = 13;
            this.dteDate.Value = null;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.txtFileName);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraGroupBox2.Location = new System.Drawing.Point(0, 69);
            this.ultraGroupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(1722, 98);
            this.ultraGroupBox2.TabIndex = 3;
            this.ultraGroupBox2.Text = "Input File";
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            editorButton1.Text = "...";
            this.txtFileName.ButtonsRight.Add(editorButton1);
            this.txtFileName.Location = new System.Drawing.Point(208, 40);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(4);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(663, 28);
            this.txtFileName.TabIndex = 4;
            this.txtFileName.Tag = "MARGIN_FILENAME";
            this.txtFileName.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtFileName_EditorButtonClick);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(123, 48);
            this.ultraLabel1.Margin = new System.Windows.Forms.Padding(4);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(76, 34);
            this.ultraLabel1.TabIndex = 3;
            this.ultraLabel1.Text = "Excel";
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(0, 167);
            this.ultraSplitter1.Margin = new System.Windows.Forms.Padding(4);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 111;
            this.ultraSplitter1.Size = new System.Drawing.Size(1722, 15);
            this.ultraSplitter1.TabIndex = 14;
            // 
            // ugDeposit
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugDeposit.DisplayLayout.Appearance = appearance1;
            this.ugDeposit.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugDeposit.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.ugDeposit.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugDeposit.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.ugDeposit.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugDeposit.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.ugDeposit.DisplayLayout.MaxColScrollRegions = 1;
            this.ugDeposit.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugDeposit.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugDeposit.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.ugDeposit.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugDeposit.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.ugDeposit.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugDeposit.DisplayLayout.Override.CellAppearance = appearance8;
            this.ugDeposit.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ugDeposit.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.ugDeposit.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.ugDeposit.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.ugDeposit.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugDeposit.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.ugDeposit.DisplayLayout.Override.RowAppearance = appearance11;
            this.ugDeposit.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugDeposit.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.ugDeposit.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugDeposit.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugDeposit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugDeposit.Location = new System.Drawing.Point(0, 182);
            this.ugDeposit.Margin = new System.Windows.Forms.Padding(4);
            this.ugDeposit.Name = "ugDeposit";
            this.ugDeposit.Size = new System.Drawing.Size(1722, 586);
            this.ugDeposit.TabIndex = 15;
            this.ugDeposit.Text = "ultraGrid1";
            // 
            // ImportSBLRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1722, 848);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ImportSBLRate";
            this.Text = "Import SBL Rate";
            this.MenuButtonClick += new System.EventHandler<EFramework.Client.WindowForms.WinForms.WinFormMenuClickEventArgs>(this.ImportStargateWebfrontEndReport_MenuButtonClick);
            this.ReciveInternalMessage += new System.EventHandler<EFramework.Client.WindowForms.WinForms.WinFormReciveInternalMessageEventArgs>(this.ImportStargateWebfrontEndReport_ReciveInternalMessage);
            this.Load += new System.EventHandler(this.ImportStargateWebfrontEndReport_Load);
            this.WinForm_Fill_Panel.ClientArea.ResumeLayout(false);
            this.WinForm_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbQueryConditions)).EndInit();
            this.gbQueryConditions.ResumeLayout(false);
            this.gbQueryConditions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dteDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFileName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugDeposit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox gbQueryConditions;
        private Infragistics.Win.Misc.UltraLabel lbImportDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dteDate;
        private Infragistics.Win.UltraWinGrid.UltraGrid ugDeposit;
        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFileName;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
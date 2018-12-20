namespace Reconciliation
{
    partial class Reconciliation
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
            this.reconciliationTab = new System.Windows.Forms.TabControl();
            this.creditCardTab = new System.Windows.Forms.TabPage();
            this.processingMessageLabel = new System.Windows.Forms.Label();
            this.selectedPathLabel = new System.Windows.Forms.Label();
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.selectFolderLabel = new System.Windows.Forms.Label();
            this.errorLabel = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.reconcileButton = new System.Windows.Forms.Button();
            this.reconciliationTab.SuspendLayout();
            this.creditCardTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // reconciliationTab
            // 
            this.reconciliationTab.Controls.Add(this.creditCardTab);
            this.reconciliationTab.Location = new System.Drawing.Point(12, 12);
            this.reconciliationTab.Name = "reconciliationTab";
            this.reconciliationTab.SelectedIndex = 0;
            this.reconciliationTab.Size = new System.Drawing.Size(436, 144);
            this.reconciliationTab.TabIndex = 0;
            // 
            // creditCardTab
            // 
            this.creditCardTab.Controls.Add(this.processingMessageLabel);
            this.creditCardTab.Controls.Add(this.selectedPathLabel);
            this.creditCardTab.Controls.Add(this.selectFolderButton);
            this.creditCardTab.Controls.Add(this.selectFolderLabel);
            this.creditCardTab.Controls.Add(this.errorLabel);
            this.creditCardTab.Controls.Add(this.resetButton);
            this.creditCardTab.Controls.Add(this.reconcileButton);
            this.creditCardTab.Location = new System.Drawing.Point(4, 22);
            this.creditCardTab.Name = "creditCardTab";
            this.creditCardTab.Padding = new System.Windows.Forms.Padding(3);
            this.creditCardTab.Size = new System.Drawing.Size(428, 118);
            this.creditCardTab.TabIndex = 0;
            this.creditCardTab.Text = "Credit Card";
            this.creditCardTab.UseVisualStyleBackColor = true;
            // 
            // processingMessageLabel
            // 
            this.processingMessageLabel.AutoSize = true;
            this.processingMessageLabel.Location = new System.Drawing.Point(11, 45);
            this.processingMessageLabel.Name = "processingMessageLabel";
            this.processingMessageLabel.Size = new System.Drawing.Size(0, 13);
            this.processingMessageLabel.TabIndex = 14;
            this.processingMessageLabel.Visible = false;
            // 
            // selectedPathLabel
            // 
            this.selectedPathLabel.AutoSize = true;
            this.selectedPathLabel.Location = new System.Drawing.Point(89, 14);
            this.selectedPathLabel.Name = "selectedPathLabel";
            this.selectedPathLabel.Size = new System.Drawing.Size(0, 13);
            this.selectedPathLabel.TabIndex = 13;
            this.selectedPathLabel.Visible = false;
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(125, 9);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(285, 23);
            this.selectFolderButton.TabIndex = 12;
            this.selectFolderButton.Text = "Select Folder";
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // selectFolderLabel
            // 
            this.selectFolderLabel.AutoSize = true;
            this.selectFolderLabel.Location = new System.Drawing.Point(8, 14);
            this.selectFolderLabel.Name = "selectFolderLabel";
            this.selectFolderLabel.Size = new System.Drawing.Size(72, 13);
            this.selectFolderLabel.TabIndex = 11;
            this.selectFolderLabel.Text = "Select Folder:";
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.BackColor = System.Drawing.Color.Transparent;
            this.errorLabel.ForeColor = System.Drawing.Color.Red;
            this.errorLabel.Location = new System.Drawing.Point(8, 45);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 13);
            this.errorLabel.TabIndex = 10;
            this.errorLabel.Visible = false;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(266, 89);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(63, 23);
            this.resetButton.TabIndex = 7;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // reconcileButton
            // 
            this.reconcileButton.Location = new System.Drawing.Point(335, 89);
            this.reconcileButton.Name = "reconcileButton";
            this.reconcileButton.Size = new System.Drawing.Size(75, 23);
            this.reconcileButton.TabIndex = 4;
            this.reconcileButton.Text = "Reconcile";
            this.reconcileButton.UseVisualStyleBackColor = true;
            this.reconcileButton.Click += new System.EventHandler(this.reconcileButton_Click);
            // 
            // Reconciliation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 168);
            this.Controls.Add(this.reconciliationTab);
            this.Name = "Reconciliation";
            this.Text = "Reconciliation";
            this.reconciliationTab.ResumeLayout(false);
            this.creditCardTab.ResumeLayout(false);
            this.creditCardTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl reconciliationTab;
        private System.Windows.Forms.TabPage creditCardTab;
        private System.Windows.Forms.Button reconcileButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Label selectFolderLabel;
        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.Label selectedPathLabel;
        private System.Windows.Forms.Label processingMessageLabel;
    }
}


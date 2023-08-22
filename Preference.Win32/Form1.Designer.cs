namespace Preference;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.ScreenShot = new System.Windows.Forms.CheckBox();
            this.KeytwoEnter = new System.Windows.Forms.CheckBox();
            this.Register = new System.Windows.Forms.Button();
            this.Unregister = new System.Windows.Forms.Button();
            this.LEPathTextbox = new System.Windows.Forms.TextBox();
            this.LEPathDiaboxButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ProcessComboBox = new System.Windows.Forms.ComboBox();
            this.StartProcess = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.KeytoEnterValue = new System.Windows.Forms.ComboBox();
            this.KeytoEnterLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ModernSleepCheckBox = new System.Windows.Forms.CheckBox();
            this.DeleteConfigButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScreenShot
            // 
            this.ScreenShot.AutoSize = true;
            this.ScreenShot.Location = new System.Drawing.Point(38, 216);
            this.ScreenShot.Name = "ScreenShot";
            this.ScreenShot.Size = new System.Drawing.Size(498, 52);
            this.ScreenShot.TabIndex = 0;
            this.ScreenShot.Text = "Make TachiChan use Alt+PrintScreen instead \r\nof Win+Shift+S for built-in screensh" +
    "ot.";
            this.ScreenShot.UseVisualStyleBackColor = true;
            this.ScreenShot.CheckedChanged += new System.EventHandler(this.ScreenShot_CheckedChanged);
            // 
            // KeytwoEnter
            // 
            this.KeytwoEnter.AutoSize = true;
            this.KeytwoEnter.Location = new System.Drawing.Point(0, 13);
            this.KeytwoEnter.Name = "KeytwoEnter";
            this.KeytwoEnter.Size = new System.Drawing.Size(28, 27);
            this.KeytwoEnter.TabIndex = 1;
            this.KeytwoEnter.UseVisualStyleBackColor = true;
            this.KeytwoEnter.CheckedChanged += new System.EventHandler(this.KeytwoEnter_CheckedChanged);
            // 
            // Register
            // 
            this.Register.Location = new System.Drawing.Point(59, 69);
            this.Register.Name = "Register";
            this.Register.Size = new System.Drawing.Size(160, 64);
            this.Register.TabIndex = 2;
            this.Register.Text = "Register";
            this.Register.UseVisualStyleBackColor = true;
            this.Register.Click += new System.EventHandler(this.Register_Click);
            // 
            // Unregister
            // 
            this.Unregister.Location = new System.Drawing.Point(244, 69);
            this.Unregister.Name = "Unregister";
            this.Unregister.Size = new System.Drawing.Size(160, 64);
            this.Unregister.TabIndex = 3;
            this.Unregister.Text = "Unregister";
            this.Unregister.UseVisualStyleBackColor = true;
            this.Unregister.Click += new System.EventHandler(this.Unregister_Click);
            // 
            // LEPathTextbox
            // 
            this.LEPathTextbox.Location = new System.Drawing.Point(38, 172);
            this.LEPathTextbox.Name = "LEPathTextbox";
            this.LEPathTextbox.ReadOnly = true;
            this.LEPathTextbox.Size = new System.Drawing.Size(263, 31);
            this.LEPathTextbox.TabIndex = 8;
            // 
            // LEPathDiaboxButton
            // 
            this.LEPathDiaboxButton.Location = new System.Drawing.Point(327, 139);
            this.LEPathDiaboxButton.Name = "LEPathDiaboxButton";
            this.LEPathDiaboxButton.Size = new System.Drawing.Size(128, 64);
            this.LEPathDiaboxButton.TabIndex = 9;
            this.LEPathDiaboxButton.Text = "Select Path...";
            this.LEPathDiaboxButton.UseVisualStyleBackColor = true;
            this.LEPathDiaboxButton.Click += new System.EventHandler(this.LEPathDialogButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Open by other application";
            // 
            // ProcessComboBox
            // 
            this.ProcessComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProcessComboBox.FormattingEnabled = true;
            this.ProcessComboBox.Location = new System.Drawing.Point(38, 83);
            this.ProcessComboBox.Name = "ProcessComboBox";
            this.ProcessComboBox.Size = new System.Drawing.Size(263, 32);
            this.ProcessComboBox.TabIndex = 11;
            this.ProcessComboBox.DropDown += new System.EventHandler(this.ProcessComboBox_DropDown);
            this.ProcessComboBox.SelectedIndexChanged += new System.EventHandler(this.ProcessComboBox_SelectedIndexChanged);
            // 
            // StartProcess
            // 
            this.StartProcess.Location = new System.Drawing.Point(327, 51);
            this.StartProcess.Name = "StartProcess";
            this.StartProcess.Size = new System.Drawing.Size(128, 64);
            this.StartProcess.TabIndex = 12;
            this.StartProcess.Text = "Start";
            this.StartProcess.UseVisualStyleBackColor = true;
            this.StartProcess.Click += new System.EventHandler(this.StartProcess_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ModernSleepCheckBox);
            this.groupBox1.Controls.Add(this.DeleteConfigButton);
            this.groupBox1.Controls.Add(this.ProcessComboBox);
            this.groupBox1.Controls.Add(this.StartProcess);
            this.groupBox1.Controls.Add(this.ScreenShot);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LEPathDiaboxButton);
            this.groupBox1.Controls.Add(this.LEPathTextbox);
            this.groupBox1.Location = new System.Drawing.Point(59, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 414);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.KeytoEnterValue);
            this.panel2.Controls.Add(this.KeytoEnterLabel);
            this.panel2.Controls.Add(this.KeytwoEnter);
            this.panel2.Location = new System.Drawing.Point(38, 275);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(417, 53);
            this.panel2.TabIndex = 17;
            // 
            // KeytoEnterValue
            // 
            this.KeytoEnterValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.KeytoEnterValue.FormattingEnabled = true;
            this.KeytoEnterValue.Items.AddRange(new object[] {
            "Z",
            "A",
            "Q",
            "Space"});
            this.KeytoEnterValue.Location = new System.Drawing.Point(75, 10);
            this.KeytoEnterValue.Name = "KeytoEnterValue";
            this.KeytoEnterValue.Size = new System.Drawing.Size(61, 32);
            this.KeytoEnterValue.TabIndex = 16;
            this.KeytoEnterValue.SelectedIndexChanged += new System.EventHandler(this.KeytoEnterValue_SelectedIndexChanged);
            // 
            // KeytoEnterLabel
            // 
            this.KeytoEnterLabel.AutoSize = true;
            this.KeytoEnterLabel.Location = new System.Drawing.Point(29, 13);
            this.KeytoEnterLabel.Name = "KeytoEnterLabel";
            this.KeytoEnterLabel.Size = new System.Drawing.Size(384, 24);
            this.KeytoEnterLabel.TabIndex = 18;
            this.KeytoEnterLabel.Text = "The          key maps to the Enter key.";
            this.KeytoEnterLabel.Click += new System.EventHandler(this.KeytoEnterLabel_Click);
            this.KeytoEnterLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.KeytoEnterLabel_Down);
            this.KeytoEnterLabel.MouseEnter += new System.EventHandler(this.KeytoEnterLabel_Enter);
            this.KeytoEnterLabel.MouseLeave += new System.EventHandler(this.KeytoEnterLabel_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 24);
            this.label3.TabIndex = 15;
            this.label3.Text = "Select Game";
            // 
            // ModernSleepCheckBox
            // 
            this.ModernSleepCheckBox.AutoSize = true;
            this.ModernSleepCheckBox.Location = new System.Drawing.Point(38, 334);
            this.ModernSleepCheckBox.Name = "ModernSleepCheckBox";
            this.ModernSleepCheckBox.Size = new System.Drawing.Size(282, 76);
            this.ModernSleepCheckBox.TabIndex = 14;
            this.ModernSleepCheckBox.Text = "Sleep computer after 10\r\nminutes in no interact. \r\n(not stable work)";
            this.ModernSleepCheckBox.UseVisualStyleBackColor = true;
            this.ModernSleepCheckBox.CheckedChanged += new System.EventHandler(this.ModernSleep_CheckedChanged);
            // 
            // DeleteConfigButton
            // 
            this.DeleteConfigButton.Location = new System.Drawing.Point(354, 334);
            this.DeleteConfigButton.Name = "DeleteConfigButton";
            this.DeleteConfigButton.Size = new System.Drawing.Size(160, 64);
            this.DeleteConfigButton.TabIndex = 13;
            this.DeleteConfigButton.Text = "Clear Config";
            this.DeleteConfigButton.UseVisualStyleBackColor = true;
            this.DeleteConfigButton.Click += new System.EventHandler(this.DeleteConfigButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(371, 24);
            this.label2.TabIndex = 15;
            this.label2.Text = "Register TachiChan in context menu";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 585);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Unregister);
            this.Controls.Add(this.Register);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preference - V0.0.0.0";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private CheckBox ScreenShot;
    private CheckBox KeytwoEnter;
    private Button Register;
    private Button Unregister;
    private TextBox LEPathTextbox;
    private Button LEPathDiaboxButton;
    private Label label1;
    private ComboBox ProcessComboBox;
    private Button StartProcess;
    private GroupBox groupBox1;
    private Button DeleteConfigButton;
    private CheckBox ModernSleepCheckBox;
    private Label label2;
    private Label label3;
    private ComboBox KeytoEnterValue;
    private Panel panel2;
    private Label KeytoEnterLabel;
}

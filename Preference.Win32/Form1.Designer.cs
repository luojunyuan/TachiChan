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
            this.DeleteConfigButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScreenShot
            // 
            this.ScreenShot.AutoSize = true;
            this.ScreenShot.Location = new System.Drawing.Point(18, 108);
            this.ScreenShot.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.ScreenShot.Name = "ScreenShot";
            this.ScreenShot.Size = new System.Drawing.Size(260, 28);
            this.ScreenShot.TabIndex = 0;
            this.ScreenShot.Text = "Make TachiChan use Alt+PrintScreen instead \r\nof Win+Shift+S for built-in screensh" +
    "ot.";
            this.ScreenShot.UseVisualStyleBackColor = true;
            this.ScreenShot.CheckedChanged += new System.EventHandler(this.ScreenShot_CheckedChanged);
            // 
            // KeytwoEnter
            // 
            this.KeytwoEnter.AutoSize = true;
            this.KeytwoEnter.Location = new System.Drawing.Point(0, 6);
            this.KeytwoEnter.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.KeytwoEnter.Name = "KeytwoEnter";
            this.KeytwoEnter.Size = new System.Drawing.Size(15, 14);
            this.KeytwoEnter.TabIndex = 1;
            this.KeytwoEnter.UseVisualStyleBackColor = true;
            this.KeytwoEnter.CheckedChanged += new System.EventHandler(this.KeytwoEnter_CheckedChanged);
            // 
            // Register
            // 
            this.Register.Location = new System.Drawing.Point(27, 34);
            this.Register.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.Register.Name = "Register";
            this.Register.Size = new System.Drawing.Size(74, 32);
            this.Register.TabIndex = 2;
            this.Register.Text = "Register";
            this.Register.UseVisualStyleBackColor = true;
            this.Register.Click += new System.EventHandler(this.Register_Click);
            // 
            // Unregister
            // 
            this.Unregister.Location = new System.Drawing.Point(113, 34);
            this.Unregister.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.Unregister.Name = "Unregister";
            this.Unregister.Size = new System.Drawing.Size(74, 32);
            this.Unregister.TabIndex = 3;
            this.Unregister.Text = "Unregister";
            this.Unregister.UseVisualStyleBackColor = true;
            this.Unregister.Click += new System.EventHandler(this.Unregister_Click);
            // 
            // LEPathTextbox
            // 
            this.LEPathTextbox.Location = new System.Drawing.Point(18, 86);
            this.LEPathTextbox.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.LEPathTextbox.Name = "LEPathTextbox";
            this.LEPathTextbox.ReadOnly = true;
            this.LEPathTextbox.Size = new System.Drawing.Size(124, 19);
            this.LEPathTextbox.TabIndex = 8;
            // 
            // LEPathDiaboxButton
            // 
            this.LEPathDiaboxButton.Location = new System.Drawing.Point(151, 70);
            this.LEPathDiaboxButton.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.LEPathDiaboxButton.Name = "LEPathDiaboxButton";
            this.LEPathDiaboxButton.Size = new System.Drawing.Size(59, 32);
            this.LEPathDiaboxButton.TabIndex = 9;
            this.LEPathDiaboxButton.Text = "Select Path...";
            this.LEPathDiaboxButton.UseVisualStyleBackColor = true;
            this.LEPathDiaboxButton.Click += new System.EventHandler(this.LEPathDialogButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Open by other application";
            // 
            // ProcessComboBox
            // 
            this.ProcessComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProcessComboBox.FormattingEnabled = true;
            this.ProcessComboBox.Location = new System.Drawing.Point(18, 42);
            this.ProcessComboBox.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.ProcessComboBox.Name = "ProcessComboBox";
            this.ProcessComboBox.Size = new System.Drawing.Size(124, 20);
            this.ProcessComboBox.TabIndex = 11;
            this.ProcessComboBox.DropDown += new System.EventHandler(this.ProcessComboBox_DropDown);
            this.ProcessComboBox.SelectedIndexChanged += new System.EventHandler(this.ProcessComboBox_SelectedIndexChanged);
            // 
            // StartProcess
            // 
            this.StartProcess.Location = new System.Drawing.Point(151, 26);
            this.StartProcess.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.StartProcess.Name = "StartProcess";
            this.StartProcess.Size = new System.Drawing.Size(59, 32);
            this.StartProcess.TabIndex = 12;
            this.StartProcess.Text = "Start";
            this.StartProcess.UseVisualStyleBackColor = true;
            this.StartProcess.Click += new System.EventHandler(this.StartProcess_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.DeleteConfigButton);
            this.groupBox1.Controls.Add(this.ProcessComboBox);
            this.groupBox1.Controls.Add(this.StartProcess);
            this.groupBox1.Controls.Add(this.ScreenShot);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LEPathDiaboxButton);
            this.groupBox1.Controls.Add(this.LEPathTextbox);
            this.groupBox1.Location = new System.Drawing.Point(27, 80);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.groupBox1.Size = new System.Drawing.Size(249, 207);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.KeytoEnterValue);
            this.panel2.Controls.Add(this.KeytoEnterLabel);
            this.panel2.Controls.Add(this.KeytwoEnter);
            this.panel2.Location = new System.Drawing.Point(18, 138);
            this.panel2.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(192, 26);
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
            this.KeytoEnterValue.Location = new System.Drawing.Point(35, 5);
            this.KeytoEnterValue.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.KeytoEnterValue.Name = "KeytoEnterValue";
            this.KeytoEnterValue.Size = new System.Drawing.Size(30, 20);
            this.KeytoEnterValue.TabIndex = 16;
            this.KeytoEnterValue.SelectedIndexChanged += new System.EventHandler(this.KeytoEnterValue_SelectedIndexChanged);
            // 
            // KeytoEnterLabel
            // 
            this.KeytoEnterLabel.AutoSize = true;
            this.KeytoEnterLabel.Location = new System.Drawing.Point(13, 6);
            this.KeytoEnterLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.KeytoEnterLabel.Name = "KeytoEnterLabel";
            this.KeytoEnterLabel.Size = new System.Drawing.Size(202, 12);
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
            this.label3.Location = new System.Drawing.Point(16, 21);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "Select Game";
            // 
            // DeleteConfigButton
            // 
            this.DeleteConfigButton.Location = new System.Drawing.Point(163, 167);
            this.DeleteConfigButton.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.DeleteConfigButton.Name = "DeleteConfigButton";
            this.DeleteConfigButton.Size = new System.Drawing.Size(74, 32);
            this.DeleteConfigButton.TabIndex = 13;
            this.DeleteConfigButton.Text = "Clear Config";
            this.DeleteConfigButton.UseVisualStyleBackColor = true;
            this.DeleteConfigButton.Click += new System.EventHandler(this.DeleteConfigButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "Register TachiChan in context menu";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 292);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Unregister);
            this.Controls.Add(this.Register);
            this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
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
    private Label label2;
    private Label label3;
    private ComboBox KeytoEnterValue;
    private Panel panel2;
    private Label KeytoEnterLabel;
}

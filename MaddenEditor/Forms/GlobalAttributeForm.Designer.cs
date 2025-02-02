namespace MaddenEditor.Forms
{
	partial class GlobalAttributeForm
	{
		/// <summary>
		/// 
        /// 
        /// 
        /// uired designer variable.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalAttributeForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.attributeCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkArchetypeFilter = new System.Windows.Forms.CheckBox();
            this.filterArchetypeComboBox = new System.Windows.Forms.ComboBox();
            this.rbYearsProLessThan = new System.Windows.Forms.RadioButton();
            this.rbYearsProEqualTo = new System.Windows.Forms.RadioButton();
            this.rbYearsProGreaterThan = new System.Windows.Forms.RadioButton();
            this.nudYearsProFilter = new System.Windows.Forms.NumericUpDown();
            this.rbAgeLessThan = new System.Windows.Forms.RadioButton();
            this.rbAgeEqualTo = new System.Windows.Forms.RadioButton();
            this.rbAgeGreaterThan = new System.Windows.Forms.RadioButton();
            this.nudAgeFilter = new System.Windows.Forms.NumericUpDown();
            this.chkAgeFilter = new System.Windows.Forms.CheckBox();
            this.chkYearsProFilter = new System.Windows.Forms.CheckBox();
            this.filterPositionComboBox = new System.Windows.Forms.ComboBox();
            this.filterTeamComboBox = new System.Windows.Forms.ComboBox();
            this.chkPositionFilter = new System.Windows.Forms.CheckBox();
            this.chkTeamFilter = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.GlobalTraitOption = new System.Windows.Forms.GroupBox();
            this.incrementNumeric = new System.Windows.Forms.NumericUpDown();
            this.decrementNumeric = new System.Windows.Forms.NumericUpDown();
            this.setNumeric = new System.Windows.Forms.NumericUpDown();
            this.incrementCheckBox = new System.Windows.Forms.RadioButton();
            this.decrementCheckBox = new System.Windows.Forms.RadioButton();
            this.setCheckBox = new System.Windows.Forms.RadioButton();
            this.TraitON = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TraitOptionsCombo = new System.Windows.Forms.ComboBox();
            this.traitCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TraitOFF = new System.Windows.Forms.CheckBox();
            this.MiscCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.MiscOptionsCombo = new System.Windows.Forms.ComboBox();
            this.EquipOptions = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.EquipCombo = new System.Windows.Forms.ComboBox();
            this.chkProBowlFilter = new System.Windows.Forms.CheckBox();
            this.filterProBowlcomboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYearsProFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAgeFilter)).BeginInit();
            this.GlobalTraitOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.incrementNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.decrementNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(600, 33);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = "Using the available options below, structure your query to edit the attributes yo" +
    "u want changed on a group of selected players  Changes will not take effect unti" +
    "l APPLY is selected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Change Attribute";
            // 
            // attributeCombo
            // 
            this.attributeCombo.FormattingEnabled = true;
            this.attributeCombo.Location = new System.Drawing.Point(93, 74);
            this.attributeCombo.Name = "attributeCombo";
            this.attributeCombo.Size = new System.Drawing.Size(121, 21);
            this.attributeCombo.TabIndex = 0;
            this.attributeCombo.SelectedIndexChanged += new System.EventHandler(this.attributeCombo_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.filterProBowlcomboBox);
            this.groupBox1.Controls.Add(this.chkProBowlFilter);
            this.groupBox1.Controls.Add(this.chkArchetypeFilter);
            this.groupBox1.Controls.Add(this.filterArchetypeComboBox);
            this.groupBox1.Controls.Add(this.rbYearsProLessThan);
            this.groupBox1.Controls.Add(this.rbYearsProEqualTo);
            this.groupBox1.Controls.Add(this.rbYearsProGreaterThan);
            this.groupBox1.Controls.Add(this.nudYearsProFilter);
            this.groupBox1.Controls.Add(this.rbAgeLessThan);
            this.groupBox1.Controls.Add(this.rbAgeEqualTo);
            this.groupBox1.Controls.Add(this.rbAgeGreaterThan);
            this.groupBox1.Controls.Add(this.nudAgeFilter);
            this.groupBox1.Controls.Add(this.chkAgeFilter);
            this.groupBox1.Controls.Add(this.chkYearsProFilter);
            this.groupBox1.Controls.Add(this.filterPositionComboBox);
            this.groupBox1.Controls.Add(this.filterTeamComboBox);
            this.groupBox1.Controls.Add(this.chkPositionFilter);
            this.groupBox1.Controls.Add(this.chkTeamFilter);
            this.groupBox1.Location = new System.Drawing.Point(288, 210);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 191);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Settings";
            // 
            // chkArchetypeFilter
            // 
            this.chkArchetypeFilter.AutoSize = true;
            this.chkArchetypeFilter.Location = new System.Drawing.Point(7, 85);
            this.chkArchetypeFilter.Name = "chkArchetypeFilter";
            this.chkArchetypeFilter.Size = new System.Drawing.Size(74, 17);
            this.chkArchetypeFilter.TabIndex = 15;
            this.chkArchetypeFilter.Text = "Archetype";
            // 
            // filterArchetypeComboBox
            // 
            this.filterArchetypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterArchetypeComboBox.FormattingEnabled = true;
            this.filterArchetypeComboBox.Location = new System.Drawing.Point(81, 83);
            this.filterArchetypeComboBox.Name = "filterArchetypeComboBox";
            this.filterArchetypeComboBox.Size = new System.Drawing.Size(224, 21);
            this.filterArchetypeComboBox.TabIndex = 14;
            // 
            // rbYearsProLessThan
            // 
            this.rbYearsProLessThan.AutoSize = true;
            this.rbYearsProLessThan.Location = new System.Drawing.Point(165, 168);
            this.rbYearsProLessThan.Name = "rbYearsProLessThan";
            this.rbYearsProLessThan.Size = new System.Drawing.Size(31, 17);
            this.rbYearsProLessThan.TabIndex = 12;
            this.rbYearsProLessThan.Text = "<";
            // 
            // rbYearsProEqualTo
            // 
            this.rbYearsProEqualTo.AutoSize = true;
            this.rbYearsProEqualTo.Location = new System.Drawing.Point(202, 168);
            this.rbYearsProEqualTo.Name = "rbYearsProEqualTo";
            this.rbYearsProEqualTo.Size = new System.Drawing.Size(31, 17);
            this.rbYearsProEqualTo.TabIndex = 13;
            this.rbYearsProEqualTo.Text = "=";
            // 
            // rbYearsProGreaterThan
            // 
            this.rbYearsProGreaterThan.AutoSize = true;
            this.rbYearsProGreaterThan.Checked = true;
            this.rbYearsProGreaterThan.Location = new System.Drawing.Point(133, 168);
            this.rbYearsProGreaterThan.Name = "rbYearsProGreaterThan";
            this.rbYearsProGreaterThan.Size = new System.Drawing.Size(31, 17);
            this.rbYearsProGreaterThan.TabIndex = 11;
            this.rbYearsProGreaterThan.TabStop = true;
            this.rbYearsProGreaterThan.Text = ">";
            this.rbYearsProGreaterThan.CheckedChanged += new System.EventHandler(this.rbYearsProGreaterThan_CheckedChanged);
            // 
            // nudYearsProFilter
            // 
            this.nudYearsProFilter.Location = new System.Drawing.Point(81, 165);
            this.nudYearsProFilter.Name = "nudYearsProFilter";
            this.nudYearsProFilter.Size = new System.Drawing.Size(46, 20);
            this.nudYearsProFilter.TabIndex = 10;
            // 
            // rbAgeLessThan
            // 
            this.rbAgeLessThan.AutoSize = true;
            this.rbAgeLessThan.Location = new System.Drawing.Point(165, 140);
            this.rbAgeLessThan.Name = "rbAgeLessThan";
            this.rbAgeLessThan.Size = new System.Drawing.Size(31, 17);
            this.rbAgeLessThan.TabIndex = 7;
            this.rbAgeLessThan.Text = "<";
            // 
            // rbAgeEqualTo
            // 
            this.rbAgeEqualTo.AutoSize = true;
            this.rbAgeEqualTo.Location = new System.Drawing.Point(202, 140);
            this.rbAgeEqualTo.Name = "rbAgeEqualTo";
            this.rbAgeEqualTo.Size = new System.Drawing.Size(31, 17);
            this.rbAgeEqualTo.TabIndex = 8;
            this.rbAgeEqualTo.Text = "=";
            // 
            // rbAgeGreaterThan
            // 
            this.rbAgeGreaterThan.AutoSize = true;
            this.rbAgeGreaterThan.Checked = true;
            this.rbAgeGreaterThan.Location = new System.Drawing.Point(133, 140);
            this.rbAgeGreaterThan.Name = "rbAgeGreaterThan";
            this.rbAgeGreaterThan.Size = new System.Drawing.Size(31, 17);
            this.rbAgeGreaterThan.TabIndex = 6;
            this.rbAgeGreaterThan.TabStop = true;
            this.rbAgeGreaterThan.Text = ">";
            // 
            // nudAgeFilter
            // 
            this.nudAgeFilter.Location = new System.Drawing.Point(81, 137);
            this.nudAgeFilter.Name = "nudAgeFilter";
            this.nudAgeFilter.Size = new System.Drawing.Size(46, 20);
            this.nudAgeFilter.TabIndex = 5;
            // 
            // chkAgeFilter
            // 
            this.chkAgeFilter.AutoSize = true;
            this.chkAgeFilter.Location = new System.Drawing.Point(7, 136);
            this.chkAgeFilter.Name = "chkAgeFilter";
            this.chkAgeFilter.Size = new System.Drawing.Size(45, 17);
            this.chkAgeFilter.TabIndex = 4;
            this.chkAgeFilter.Text = "Age";
            this.chkAgeFilter.CheckedChanged += new System.EventHandler(this.chkAgeFilter_CheckedChanged);
            // 
            // chkYearsProFilter
            // 
            this.chkYearsProFilter.AutoSize = true;
            this.chkYearsProFilter.Location = new System.Drawing.Point(7, 166);
            this.chkYearsProFilter.Name = "chkYearsProFilter";
            this.chkYearsProFilter.Size = new System.Drawing.Size(72, 17);
            this.chkYearsProFilter.TabIndex = 9;
            this.chkYearsProFilter.Text = "Years Pro";
            // 
            // filterPositionComboBox
            // 
            this.filterPositionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterPositionComboBox.FormattingEnabled = true;
            this.filterPositionComboBox.Location = new System.Drawing.Point(81, 52);
            this.filterPositionComboBox.Name = "filterPositionComboBox";
            this.filterPositionComboBox.Size = new System.Drawing.Size(224, 21);
            this.filterPositionComboBox.TabIndex = 3;
            // 
            // filterTeamComboBox
            // 
            this.filterTeamComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTeamComboBox.FormattingEnabled = true;
            this.filterTeamComboBox.Location = new System.Drawing.Point(81, 22);
            this.filterTeamComboBox.Name = "filterTeamComboBox";
            this.filterTeamComboBox.Size = new System.Drawing.Size(224, 21);
            this.filterTeamComboBox.TabIndex = 1;
            this.filterTeamComboBox.SelectedIndexChanged += new System.EventHandler(this.filterTeamComboBox_SelectedIndexChanged);
            // 
            // chkPositionFilter
            // 
            this.chkPositionFilter.AutoSize = true;
            this.chkPositionFilter.Location = new System.Drawing.Point(7, 54);
            this.chkPositionFilter.Name = "chkPositionFilter";
            this.chkPositionFilter.Size = new System.Drawing.Size(63, 17);
            this.chkPositionFilter.TabIndex = 2;
            this.chkPositionFilter.Text = "Position";
            // 
            // chkTeamFilter
            // 
            this.chkTeamFilter.AutoSize = true;
            this.chkTeamFilter.Location = new System.Drawing.Point(7, 24);
            this.chkTeamFilter.Name = "chkTeamFilter";
            this.chkTeamFilter.Size = new System.Drawing.Size(53, 17);
            this.chkTeamFilter.TabIndex = 0;
            this.chkTeamFilter.Text = "Team";
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(456, 411);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(537, 411);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Exit";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // GlobalTraitOption
            // 
            this.GlobalTraitOption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GlobalTraitOption.Controls.Add(this.incrementNumeric);
            this.GlobalTraitOption.Controls.Add(this.decrementNumeric);
            this.GlobalTraitOption.Controls.Add(this.setNumeric);
            this.GlobalTraitOption.Controls.Add(this.incrementCheckBox);
            this.GlobalTraitOption.Controls.Add(this.decrementCheckBox);
            this.GlobalTraitOption.Controls.Add(this.setCheckBox);
            this.GlobalTraitOption.Location = new System.Drawing.Point(12, 257);
            this.GlobalTraitOption.Name = "GlobalTraitOption";
            this.GlobalTraitOption.Size = new System.Drawing.Size(252, 144);
            this.GlobalTraitOption.TabIndex = 2;
            this.GlobalTraitOption.TabStop = false;
            this.GlobalTraitOption.Text = "Change by";
            // 
            // incrementNumeric
            // 
            this.incrementNumeric.Location = new System.Drawing.Point(124, 76);
            this.incrementNumeric.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.incrementNumeric.Name = "incrementNumeric";
            this.incrementNumeric.Size = new System.Drawing.Size(73, 20);
            this.incrementNumeric.TabIndex = 5;
            // 
            // decrementNumeric
            // 
            this.decrementNumeric.Location = new System.Drawing.Point(124, 51);
            this.decrementNumeric.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.decrementNumeric.Name = "decrementNumeric";
            this.decrementNumeric.Size = new System.Drawing.Size(73, 20);
            this.decrementNumeric.TabIndex = 3;
            // 
            // setNumeric
            // 
            this.setNumeric.Location = new System.Drawing.Point(124, 25);
            this.setNumeric.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.setNumeric.Name = "setNumeric";
            this.setNumeric.Size = new System.Drawing.Size(73, 20);
            this.setNumeric.TabIndex = 1;
            // 
            // incrementCheckBox
            // 
            this.incrementCheckBox.AutoSize = true;
            this.incrementCheckBox.Location = new System.Drawing.Point(18, 79);
            this.incrementCheckBox.Name = "incrementCheckBox";
            this.incrementCheckBox.Size = new System.Drawing.Size(86, 17);
            this.incrementCheckBox.TabIndex = 4;
            this.incrementCheckBox.Text = "Increment by";
            // 
            // decrementCheckBox
            // 
            this.decrementCheckBox.AutoSize = true;
            this.decrementCheckBox.Checked = true;
            this.decrementCheckBox.Location = new System.Drawing.Point(18, 54);
            this.decrementCheckBox.Name = "decrementCheckBox";
            this.decrementCheckBox.Size = new System.Drawing.Size(91, 17);
            this.decrementCheckBox.TabIndex = 2;
            this.decrementCheckBox.TabStop = true;
            this.decrementCheckBox.Text = "Decrement by";
            // 
            // setCheckBox
            // 
            this.setCheckBox.AutoSize = true;
            this.setCheckBox.Location = new System.Drawing.Point(18, 28);
            this.setCheckBox.Name = "setCheckBox";
            this.setCheckBox.Size = new System.Drawing.Size(53, 17);
            this.setCheckBox.TabIndex = 0;
            this.setCheckBox.Text = "Set to";
            // 
            // TraitON
            // 
            this.TraitON.AutoSize = true;
            this.TraitON.Location = new System.Drawing.Point(93, 133);
            this.TraitON.Name = "TraitON";
            this.TraitON.Size = new System.Drawing.Size(66, 17);
            this.TraitON.TabIndex = 13;
            this.TraitON.Text = "Trait ON";
            this.TraitON.UseVisualStyleBackColor = true;
            this.TraitON.CheckedChanged += new System.EventHandler(this.TraitON_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(232, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Trait Option";
            // 
            // TraitOptionsCombo
            // 
            this.TraitOptionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TraitOptionsCombo.FormattingEnabled = true;
            this.TraitOptionsCombo.Location = new System.Drawing.Point(297, 108);
            this.TraitOptionsCombo.Name = "TraitOptionsCombo";
            this.TraitOptionsCombo.Size = new System.Drawing.Size(296, 21);
            this.TraitOptionsCombo.TabIndex = 11;
            this.TraitOptionsCombo.SelectedIndexChanged += new System.EventHandler(this.TraitOptionsCombo_SelectedIndexChanged);
            // 
            // traitCombo
            // 
            this.traitCombo.FormattingEnabled = true;
            this.traitCombo.Location = new System.Drawing.Point(93, 106);
            this.traitCombo.Name = "traitCombo";
            this.traitCombo.Size = new System.Drawing.Size(121, 21);
            this.traitCombo.TabIndex = 9;
            this.traitCombo.SelectedIndexChanged += new System.EventHandler(this.traitCombo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Change Trait";
            // 
            // TraitOFF
            // 
            this.TraitOFF.AutoSize = true;
            this.TraitOFF.Location = new System.Drawing.Point(165, 133);
            this.TraitOFF.Name = "TraitOFF";
            this.TraitOFF.Size = new System.Drawing.Size(70, 17);
            this.TraitOFF.TabIndex = 6;
            this.TraitOFF.Text = "Trait OFF";
            this.TraitOFF.UseVisualStyleBackColor = true;
            this.TraitOFF.CheckedChanged += new System.EventHandler(this.TraitOFF_CheckedChanged);
            // 
            // MiscCombo
            // 
            this.MiscCombo.FormattingEnabled = true;
            this.MiscCombo.Location = new System.Drawing.Point(93, 156);
            this.MiscCombo.Name = "MiscCombo";
            this.MiscCombo.Size = new System.Drawing.Size(121, 21);
            this.MiscCombo.TabIndex = 14;
            this.MiscCombo.SelectedIndexChanged += new System.EventHandler(this.MiscCombo_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Misc Option";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(60, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Misc";
            // 
            // MiscOptionsCombo
            // 
            this.MiscOptionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MiscOptionsCombo.FormattingEnabled = true;
            this.MiscOptionsCombo.Location = new System.Drawing.Point(297, 156);
            this.MiscOptionsCombo.Name = "MiscOptionsCombo";
            this.MiscOptionsCombo.Size = new System.Drawing.Size(296, 21);
            this.MiscOptionsCombo.TabIndex = 17;
            this.MiscOptionsCombo.SelectedIndexChanged += new System.EventHandler(this.MiscOptionsCombo_SelectedIndexChanged);
            // 
            // EquipOptions
            // 
            this.EquipOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EquipOptions.FormattingEnabled = true;
            this.EquipOptions.Location = new System.Drawing.Point(297, 183);
            this.EquipOptions.Name = "EquipOptions";
            this.EquipOptions.Size = new System.Drawing.Size(296, 21);
            this.EquipOptions.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Equipment";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(220, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Equip. Option";
            // 
            // EquipCombo
            // 
            this.EquipCombo.FormattingEnabled = true;
            this.EquipCombo.Location = new System.Drawing.Point(93, 183);
            this.EquipCombo.Name = "EquipCombo";
            this.EquipCombo.Size = new System.Drawing.Size(121, 21);
            this.EquipCombo.TabIndex = 18;
            this.EquipCombo.SelectedIndexChanged += new System.EventHandler(this.EquipCombo_SelectedIndexChanged);
            // 
            // chkProBowlFilter
            // 
            this.chkProBowlFilter.AutoSize = true;
            this.chkProBowlFilter.Location = new System.Drawing.Point(7, 110);
            this.chkProBowlFilter.Name = "chkProBowlFilter";
            this.chkProBowlFilter.Size = new System.Drawing.Size(68, 17);
            this.chkProBowlFilter.TabIndex = 16;
            this.chkProBowlFilter.Text = "Pro Bowl";
            // 
            // filterProBowlcomboBox
            // 
            this.filterProBowlcomboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterProBowlcomboBox.FormattingEnabled = true;
            this.filterProBowlcomboBox.Location = new System.Drawing.Point(81, 110);
            this.filterProBowlcomboBox.Name = "filterProBowlcomboBox";
            this.filterProBowlcomboBox.Size = new System.Drawing.Size(224, 21);
            this.filterProBowlcomboBox.TabIndex = 17;
            // 
            // GlobalAttributeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.EquipOptions);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.EquipCombo);
            this.Controls.Add(this.MiscOptionsCombo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MiscCombo);
            this.Controls.Add(this.TraitOFF);
            this.Controls.Add(this.TraitON);
            this.Controls.Add(this.GlobalTraitOption);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.TraitOptionsCombo);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.traitCombo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.attributeCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GlobalAttributeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Global Player Attribute Editor Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYearsProFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAgeFilter)).EndInit();
            this.GlobalTraitOption.ResumeLayout(false);
            this.GlobalTraitOption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.incrementNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.decrementNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox attributeCombo;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkPositionFilter;
		private System.Windows.Forms.CheckBox chkTeamFilter;
		private System.Windows.Forms.ComboBox filterPositionComboBox;
		private System.Windows.Forms.ComboBox filterTeamComboBox;
		private System.Windows.Forms.Button applyButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox GlobalTraitOption;
		private System.Windows.Forms.RadioButton setCheckBox;
		private System.Windows.Forms.RadioButton decrementCheckBox;
		private System.Windows.Forms.RadioButton incrementCheckBox;
		private System.Windows.Forms.NumericUpDown incrementNumeric;
		private System.Windows.Forms.NumericUpDown decrementNumeric;
		private System.Windows.Forms.NumericUpDown setNumeric;
		private System.Windows.Forms.CheckBox chkYearsProFilter;
		private System.Windows.Forms.CheckBox chkAgeFilter;
		private System.Windows.Forms.RadioButton rbAgeLessThan;
		private System.Windows.Forms.RadioButton rbAgeEqualTo;
		private System.Windows.Forms.RadioButton rbAgeGreaterThan;
		private System.Windows.Forms.NumericUpDown nudAgeFilter;
		private System.Windows.Forms.RadioButton rbYearsProLessThan;
		private System.Windows.Forms.RadioButton rbYearsProEqualTo;
		private System.Windows.Forms.RadioButton rbYearsProGreaterThan;
		private System.Windows.Forms.NumericUpDown nudYearsProFilter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox traitCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox TraitOptionsCombo;
        private System.Windows.Forms.CheckBox TraitON;
        private System.Windows.Forms.CheckBox TraitOFF;
        private System.Windows.Forms.ComboBox MiscCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox MiscOptionsCombo;
        private System.Windows.Forms.ComboBox EquipOptions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox EquipCombo;
        private System.Windows.Forms.CheckBox chkArchetypeFilter;
        private System.Windows.Forms.ComboBox filterArchetypeComboBox;
        private System.Windows.Forms.CheckBox chkProBowlFilter;
        private System.Windows.Forms.ComboBox filterProBowlcomboBox;
    }
}
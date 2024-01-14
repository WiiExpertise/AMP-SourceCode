/******************************************************************************
 * MaddenAmp
 * Copyright (C) 2005 Colin Goudie
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 * http://maddenamp.sourceforge.net/
 * 
 * maddeneditor@tributech.com.au
 * 
 *****************************************************************************/
using System;
using System.Drawing;
using System.Windows.Forms;

using MaddenEditor.Core;
using MaddenEditor.Core.Record;

namespace MaddenEditor.Forms
{
    public partial class GlobalAttributeForm : Form, IEditorForm
    {
        private EditorModel model = null;

        private enum EditableAttributes
        {
            NA,
            AGE,
            YEARS_EXP,
            SPEED,
            STRENGTH,
            AWARENESS,
            AGILITY,
            ACCELERATION,
            CATCHING,
            CARRYING,
            JUMPING,
            BREAK_TACKLE,
            TACKLE,
            THROW_ACCURACY,
            THROW_POWER,
            PASS_BLOCKING,
            RUN_BLOCKING,
            KICK_POWER,
            KICK_ACCURACY,
            KICK_RETURN,
            STAMINA,
            INJURY,
            TOUGHNESS,
            IMPORTANCE,
            MORALE,
            HEIGHT,
            WEIGHT,
            Throw_Short,
            Throw_Medium,
            Throw_Deep,
            Power_Moves,
            Finesse_Moves,
            Short_Route,
            Medium_Route,
            Deep_Route,
            Trucking,
            Ball_Carry_Vision,
            Press_Coverage,
            Man_Coverage,
            Zone_Coverage,
            Pursuit,
            Change_Of_Direction,
            Play_Recognition,
            Block_Shedding,
            Hit_Power,
            Release,
            Catch_In_Traffic,
            Spectacular_Catch,
            Juke_Move,
            Stiff_Arm,
            Throw_On_Run,
            Spin_Move,
            Break_Sack,
            Throw_Under_Pressure,
            Impact_Block,
            Lead_Block,
            Play_Action,
            Pass_Block_Strength,
            Pass_Block_Footwork,
            Run_Block_Strength,
            Run_Block_Footwork


        }

        private enum EditableTraits
        {
            BigHitter,
            Clutch,
            PossCatch,
            SpinMove,
            DropsPasses,
            FeetInBounds,
            FightForYards,
            HighMotor,
            AggrCatch,
            SwimMove,
            RunAfterCatch,
            ThrowsAway,
            ThrowSpiral,
            StripsBall,
            BullRush,
            CoversBall,
            ForcesPasses,
            PlaysBall,
            LBStyle,
            Penalty,
            SensePressure,
            QBPlayStyle,
            DevelopmentTrait,
            CareerPhase,
            RunningTechnique,
            QBThrowingMotion
        }

        private enum MiscGlobals
        {
            Celebrations,
            MinimumContracts
        }

        private enum EquipGlobals
        {
            Helmet,
            FaceMask,
            ShoeBoth,
            ShoeLeft,
            ShoeRight,
            HandBoth,
            HandLeft,
            HandRight,
            WristBoth,
            WristLeft,
            WristRight,
            SleeveBoth,
            SleeveLeft,
            SleeveRight,
            Undershirt,
            ElbowBoth,
            ElbowLeft,
            ElbowRight,
            KneeBoth,
            KneeLeft,
            KneeRight,
            AnkleBoth,
            AnkleLeft,
            AnkleRight,
            Neckpad,
            Visor,
            EyePaint,
            MouthPiece,
            Socks,
            JerseySleeve,
            Towel,
            HandWarmer,
            FlakJacket, //new
            BackPlate,  //new
        }

    



private bool isInitializing = false;

        public GlobalAttributeForm(EditorModel model)
        {
            this.model = model;
            InitializeComponent();
        }

        private void filterTeamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #region IEditorForm Members

        public EditorModel Model
        {
            set { }
        }

        public void InitialiseUI()
        {
            isInitializing = true;

            label3.Visible = false;
            label4.Visible = false;
            TraitON.Visible = false;
            TraitOFF.Visible = false;
            traitCombo.Visible = false;
            TraitOptionsCombo.Visible = false;

            foreach (TeamRecord team in model.TeamModel.GetTeams())
            {
                filterTeamComboBox.Items.Add(team);
            }

            for (int p = 0; p <= 20; p++)
            {
                string pos = Enum.GetName(typeof(MaddenPositions), p);
                filterPositionComboBox.Items.Add(pos);
            }

            {
                filterArchetypeComboBox.Items.Add("Field General QB"); // this line needs to be edited as well (Archetypes updating but need correct names which  means generating a list)
                filterArchetypeComboBox.Items.Add("Strong Arm QB");
                filterArchetypeComboBox.Items.Add("Improviser QB");
                filterArchetypeComboBox.Items.Add("Scrambler QB");
                filterArchetypeComboBox.Items.Add("Power Back RB");
                filterArchetypeComboBox.Items.Add("Elusive RB");
                filterArchetypeComboBox.Items.Add("Receiving RB");
                filterArchetypeComboBox.Items.Add("Blocking FB");
                filterArchetypeComboBox.Items.Add("Utility FB");
                filterArchetypeComboBox.Items.Add("Deep Threat WR");
                filterArchetypeComboBox.Items.Add("Route Running WR");
                filterArchetypeComboBox.Items.Add("Physical WR");
                filterArchetypeComboBox.Items.Add("Slot WR");
                filterArchetypeComboBox.Items.Add("Blocking TE");
                filterArchetypeComboBox.Items.Add("Vertical Threat TE");
                filterArchetypeComboBox.Items.Add("Possession TE");
                filterArchetypeComboBox.Items.Add("Pass Protect C");
                filterArchetypeComboBox.Items.Add("Power C");
                filterArchetypeComboBox.Items.Add("Agile C");
                filterArchetypeComboBox.Items.Add("Pass Protect T");
                filterArchetypeComboBox.Items.Add("Power T");
                filterArchetypeComboBox.Items.Add("Agile T");
                filterArchetypeComboBox.Items.Add("Pass Protect G");
                filterArchetypeComboBox.Items.Add("Power G");
                filterArchetypeComboBox.Items.Add("Agile G");
                filterArchetypeComboBox.Items.Add("Speed Rusher DE");
                filterArchetypeComboBox.Items.Add("Power Rusher DE");
                filterArchetypeComboBox.Items.Add("Run Stopper DE");
                filterArchetypeComboBox.Items.Add("Run Stopper DT");
                filterArchetypeComboBox.Items.Add("Speed Rusher DT");
                filterArchetypeComboBox.Items.Add("Power Rusher DT");
                filterArchetypeComboBox.Items.Add("Speed Rusher OLB");
                filterArchetypeComboBox.Items.Add("Power Rusher OLB");
                filterArchetypeComboBox.Items.Add("Pass Coverage OLB");
                filterArchetypeComboBox.Items.Add("Run Stopper OLB");
                filterArchetypeComboBox.Items.Add("Field General MLB");
                filterArchetypeComboBox.Items.Add("Pass Coverage MLB");
                filterArchetypeComboBox.Items.Add("Run Stopper MLB");
                filterArchetypeComboBox.Items.Add("Man to Man CB");
                filterArchetypeComboBox.Items.Add("Slot CB");
                filterArchetypeComboBox.Items.Add("Zone CB");
                filterArchetypeComboBox.Items.Add("Zone S");
                filterArchetypeComboBox.Items.Add("Hybrid S");
                filterArchetypeComboBox.Items.Add("Run Support S");
                filterArchetypeComboBox.Items.Add("Accurate K");
                filterArchetypeComboBox.Items.Add("Power K");
            }

            {
                filterProBowlcomboBox.Items.Add("All Players");
                filterProBowlcomboBox.Items.Add("Pro Bowlers Only");
            }

            filterArchetypeComboBox.Text = filterArchetypeComboBox.Items[0].ToString(); //Added by Prime (Arch Filter)
            filterProBowlcomboBox.Text = filterProBowlcomboBox.Items[0].ToString(); //Added by Prime (ProBowl Filter)
            filterPositionComboBox.Text = filterPositionComboBox.Items[0].ToString();
            filterTeamComboBox.Text = filterTeamComboBox.Items[0].ToString();

            if (model.MadVersion >= MaddenFileVersion.Ver2019)
            {
                label3.Visible = true;
                label4.Visible = true;
                TraitON.Visible = true;
                TraitOFF.Visible = true;
                traitCombo.Visible = true;
                TraitOptionsCombo.Visible = true;
                TraitON.Enabled = false;
                TraitOFF.Enabled = false;
                TraitOptionsCombo.Enabled = false;
                foreach (string a in Enum.GetNames(typeof(EditableAttributes)))
                    attributeCombo.Items.Add(a);
                foreach (string t in Enum.GetNames(typeof(EditableTraits)))
                    traitCombo.Items.Add(t);
                foreach (string m in Enum.GetNames(typeof(MiscGlobals)))
                    MiscCombo.Items.Add(m);
                foreach (string q in Enum.GetNames(typeof(EquipGlobals)))
                    EquipCombo.Items.Add(q);

            }


            isInitializing = false;
        }

        public void CleanUI()
        {
            isInitializing = true;
            filterTeamComboBox.Items.Clear();
            filterPositionComboBox.Items.Clear();
            attributeCombo.Items.Clear();
            isInitializing = false;
        }

        #endregion

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (attributeCombo.SelectedIndex <= 0 && traitCombo.SelectedIndex == -1 && MiscCombo.SelectedIndex == -1 && EquipCombo.SelectedIndex == -1)
                return;

            this.Cursor = Cursors.WaitCursor;
            int count = 0;
            foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                if (record.Deleted)
                    continue;

                PlayerRecord playerRecord = (PlayerRecord)record;

                if (chkTeamFilter.Checked)
                {
                    if (playerRecord.TeamId != ((TeamRecord)filterTeamComboBox.SelectedItem).TeamId)
                    {
                        continue;
                    }
                }
                if (chkPositionFilter.Checked)
                {
                    if (playerRecord.PositionId != filterPositionComboBox.SelectedIndex)
                    {
                        continue;
                    }
                }
                if (chkArchetypeFilter.Checked)
                {
                    if (playerRecord.PlayerType != filterArchetypeComboBox.SelectedIndex) //edited by Prime (Archetype Filter attempt)
                    {
                        continue;
                    }
                }
                if (chkProBowlFilter.Checked)
                {
                    if (playerRecord.ProBowlFilter != filterProBowlcomboBox.SelectedIndex) //edited by Prime (Archetype Filter attempt)
                    {
                        continue;
                    }
                }
                if (chkAgeFilter.Checked)
                {
                    if (rbAgeGreaterThan.Checked)
                    {
                        if (playerRecord.Age <= (int)nudAgeFilter.Value)
                        {
                            continue;
                        }
                    }
                    else if (rbAgeLessThan.Checked)
                    {
                        if (playerRecord.Age >= (int)nudAgeFilter.Value)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (playerRecord.Age != (int)nudAgeFilter.Value)
                        {
                            continue;
                        }
                    }
                }
                if (chkYearsProFilter.Checked)
                {
                    if (rbYearsProGreaterThan.Checked)
                    {
                        if (playerRecord.YearsPro <= (int)nudYearsProFilter.Value)
                        {
                            continue;
                        }
                    }
                    else if (rbYearsProLessThan.Checked)
                    {
                        if (playerRecord.YearsPro >= (int)nudYearsProFilter.Value)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (playerRecord.YearsPro != (int)nudYearsProFilter.Value)
                        {
                            continue;
                        }
                    }
                }

                bool absoluteValue = true;
                int value = 0;
                if (setCheckBox.Checked)
                {
                    value = (int)setNumeric.Value;
                }
                else if (incrementCheckBox.Checked)
                {
                    absoluteValue = false;
                    value = (int)incrementNumeric.Value;
                }
                else
                {
                    absoluteValue = false;
                    value = 0 - (int)decrementNumeric.Value;
                }

                if (attributeCombo.SelectedIndex > 0)
                    ChangeAttribute(playerRecord, (EditableAttributes)attributeCombo.SelectedIndex, absoluteValue, value);

                if (traitCombo.SelectedIndex != -1)
                    ChangeTrait(playerRecord, (EditableTraits)traitCombo.SelectedIndex);

                if (MiscCombo.SelectedIndex != -1)
                    ChangeMisc(playerRecord, (MiscGlobals)MiscCombo.SelectedIndex);

                if (EquipCombo.SelectedIndex != -1)
                    ChangeEquipment(playerRecord, (EquipGlobals)EquipCombo.SelectedIndex);

                count++;

            }

            this.Cursor = Cursors.Default;
            //DialogResult = DialogResult.OK;
            //this.Close();
            MessageBox.Show(count + " players successfully updated", "Change OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ChangeEquipment(PlayerRecord record, EquipGlobals equip)
        {
            switch (equip)
            {
                case EquipGlobals.Helmet:
                    record.Helmet = model.PlayerModel.GetHelmet(EquipOptions.Text);
                    break;
                case EquipGlobals.FaceMask:
                    record.FaceMask = model.PlayerModel.GetFaceMask(EquipOptions.Text);
                    break;
                case EquipGlobals.ShoeBoth:
                    record.LeftShoe = model.PlayerModel.GetShoe(EquipOptions.Text);
                    record.RightShoe = model.PlayerModel.GetShoe(EquipOptions.Text);
                    break;
                case EquipGlobals.ShoeLeft:
                    record.LeftShoe = model.PlayerModel.GetShoe(EquipOptions.Text);
                    break;
                case EquipGlobals.ShoeRight:
                    record.RightShoe = model.PlayerModel.GetShoe(EquipOptions.Text);
                    break;
                case EquipGlobals.HandBoth:
                    record.LeftHand = model.PlayerModel.GetGloves(EquipOptions.Text);
                    record.RightHand = model.PlayerModel.GetGloves(EquipOptions.Text);
                    break;
                case EquipGlobals.HandLeft:
                    record.LeftHand = model.PlayerModel.GetGloves(EquipOptions.Text);
                    break;
                case EquipGlobals.HandRight:
                    record.RightHand = model.PlayerModel.GetGloves(EquipOptions.Text);
                    break;
                case EquipGlobals.WristBoth:
                    record.LeftWrist = model.PlayerModel.GetWrist(EquipOptions.Text);
                    record.RightWrist = model.PlayerModel.GetWrist(EquipOptions.Text);
                    break;
                case EquipGlobals.WristLeft:
                    record.LeftWrist = model.PlayerModel.GetWrist(EquipOptions.Text);
                    break;
                case EquipGlobals.WristRight:
                    record.RightWrist = model.PlayerModel.GetWrist(EquipOptions.Text);
                    break;
                case EquipGlobals.SleeveBoth:
                    record.SleevesLeft = model.PlayerModel.GetSleeve(EquipOptions.Text);
                    record.SleevesRight = model.PlayerModel.GetSleeve(EquipOptions.Text);
                    break;
                case EquipGlobals.SleeveLeft:
                    record.SleevesLeft = model.PlayerModel.GetSleeve(EquipOptions.Text);
                    break;
                case EquipGlobals.SleeveRight:
                    record.SleevesRight = model.PlayerModel.GetSleeve(EquipOptions.Text);
                    break;
                case EquipGlobals.Undershirt:
                    record.JerseyTucked = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.ElbowBoth:
                    record.LeftElbow = model.PlayerModel.GetElbow(EquipOptions.Text);
                    record.RightElbow = model.PlayerModel.GetElbow(EquipOptions.Text);
                    break;
                case EquipGlobals.ElbowLeft:
                    record.LeftElbow = model.PlayerModel.GetElbow(EquipOptions.Text);
                    break;
                case EquipGlobals.ElbowRight:
                    record.RightElbow = model.PlayerModel.GetElbow(EquipOptions.Text);
                    break;
                case EquipGlobals.KneeBoth:
                    record.KneeLeft = EquipOptions.SelectedIndex;
                    record.KneeRight = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.KneeLeft:
                    record.KneeLeft = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.KneeRight:
                    record.KneeRight = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.AnkleBoth:
                    record.AnkleLeft = model.PlayerModel.GetAnkle(EquipOptions.Text);
                    record.AnkleRight = model.PlayerModel.GetAnkle(EquipOptions.Text);
                    break;
                case EquipGlobals.AnkleLeft:
                    record.AnkleLeft = model.PlayerModel.GetAnkle(EquipOptions.Text);
                    break;
                case EquipGlobals.AnkleRight:
                    record.AnkleRight = model.PlayerModel.GetAnkle(EquipOptions.Text);
                    break;
                case EquipGlobals.Visor:
                    record.Visor = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.EyePaint:
                    record.EyePaint = model.PlayerModel.GetFaceMark(EquipOptions.Text);
                    break;
                case EquipGlobals.MouthPiece:
                    record.MouthPiece = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.Socks:
                    if (model.MadVersion == MaddenFileVersion.Ver2019)
                        record.SockHeight = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.JerseySleeve:
                    record.JerseySleeve = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.Towel:
                    record.PlayerTowel = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.HandWarmer:
                    record.HandWarmer = EquipOptions.SelectedIndex;
                    break;
                case EquipGlobals.FlakJacket:
                    if (EquipOptions.SelectedIndex == 0)
                    {
                        record.FlakJacket = true; // Assign true for the first choice
                    }
                    else if (EquipOptions.SelectedIndex == 1)
                    {
                        record.FlakJacket = false; // Assign false for the second choice
                    }
                    else
                    {
                        // Handle the case when the selected index is out of bounds or invalid.
                        // You might want to set a default value or show an error message.
                    }
                    break;
                case EquipGlobals.BackPlate:
                    if (EquipOptions.SelectedIndex == 0)
                    {
                        record.BackPlate = true; // Assign true for the first choice
                    }
                    else if (EquipOptions.SelectedIndex == 1)
                    {
                        record.BackPlate = false; // Assign false for the second choice
                    }
                    else
                    {
                        // Handle the case when the selected index is out of bounds or invalid.
                        // You might want to set a default value or show an error message.
                    }
                    break;
            }
        }

        private void ChangeMisc(PlayerRecord record, MiscGlobals misc)
        {
            switch (misc)
            {
                case MiscGlobals.Celebrations:
                    record.EndPlay = model.PlayerModel.GetEndPlay(MiscOptionsCombo.Text);
                    break;
                case MiscGlobals.MinimumContracts:
                    if (record.TeamId >= 1 && record.TeamId <= 32 && record.TotalSalary == 0)
                    {
                        record.ContractLength = 1;
                        record.ContractYearsLeft = 1;
                        record.Salary0 = 75;
                        record.TotalSalary = 75;
                    }
                    break;

            }
        }

        private void ChangeTrait(PlayerRecord record, EditableTraits trait)
        {
            if (traitCombo.SelectedIndex == -1)
                return;

            switch (trait)
            {
                case EditableTraits.BigHitter:
                    if (TraitON.Checked)
                        record.BigHitter = true;
                    else record.BigHitter = false;
                    break;
                case EditableTraits.Clutch:
                    if (TraitON.Checked)
                        record.Clutch = true;
                    else record.Clutch = false;
                    break;
                case EditableTraits.PossCatch:
                    if (TraitON.Checked)
                        record.PossessionCatch = true;
                    else record.PossessionCatch = false;
                    break;
                case EditableTraits.SpinMove:
                    if (TraitON.Checked)
                        record.DLSpinmove = true;
                    else record.DLSpinmove = false;
                    break;
                case EditableTraits.DropsPasses:
                    if (TraitON.Checked)
                        record.DropPasses = true;
                    else record.DropPasses = false;
                    break;
                case EditableTraits.FeetInBounds:
                    if (TraitON.Checked)
                        record.FeetInBounds = true;
                    else record.FeetInBounds = false;
                    break;
                case EditableTraits.FightForYards:
                    if (TraitON.Checked)
                        record.FightYards = true;
                    else record.FightYards = false;
                    break;
                case EditableTraits.HighMotor:
                    if (TraitON.Checked)
                        record.HighMotor = true;
                    else record.HighMotor = false;
                    break;
                case EditableTraits.AggrCatch:
                    if (TraitON.Checked)
                        record.AggressiveCatch = true;
                    else record.AggressiveCatch = false;
                    break;
                case EditableTraits.SwimMove:
                    if (TraitON.Checked)
                        record.DLSwim = true;
                    else record.DLSwim = false;
                    break;
                case EditableTraits.RunAfterCatch:
                    if (TraitON.Checked)
                        record.RunAfterCatch = true;
                    else record.RunAfterCatch = false;
                    break;
                case EditableTraits.ThrowsAway:
                    if (TraitON.Checked)
                        record.ThrowAway = true;
                    else record.ThrowAway = false;
                    break;
                case EditableTraits.ThrowSpiral:
                    if (TraitON.Checked)
                        record.ThrowSpiral = true;
                    else record.ThrowSpiral = false;
                    break;
                case EditableTraits.StripsBall:
                    if (TraitON.Checked)
                        record.StripsBall = true;
                    else record.StripsBall = false;
                    break;
                case EditableTraits.BullRush:
                    if (TraitON.Checked)
                        record.DLBullrush = true;
                    else record.DLBullrush = false;
                    break;
                case EditableTraits.CoversBall:
                    record.CoversBall = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.ForcesPasses:
                    record.ForcePasses = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.PlaysBall:
                    record.PlaysBall = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.LBStyle:
                    record.SidelineCatch = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.Penalty:
                    record.Penalty = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.SensePressure:
                    record.SensePressure = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.QBPlayStyle:
                    record.TuckRun = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.DevelopmentTrait:
                    record.DevTrait = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.CareerPhase:
                    record.CareerPhase = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.RunningTechnique:
                    record.RunStyle = TraitOptionsCombo.SelectedIndex;
                    break;
                case EditableTraits.QBThrowingMotion:
                    record.QBStyle = TraitOptionsCombo.SelectedIndex;
                    break;
            }
        }

        private void ChangeAttribute(PlayerRecord record, EditableAttributes attribute, bool absolutevalue, int value)
        {
            switch (attribute)
            {
                case EditableAttributes.ACCELERATION:
                    if (absolutevalue)
                        record.Acceleration = value;
                    else
                        record.Acceleration = record.Acceleration + value;

                    if (record.Acceleration < 0) record.Acceleration = 0;
                    if (record.Acceleration > 99) record.Acceleration = 99;
                    break;
                case EditableAttributes.AGE:
                    if (absolutevalue)
                        record.Age = value;
                    else
                        record.Age = record.Age + value;
                    if (record.Age < 0) record.Age = 0;
                    break;
                case EditableAttributes.AGILITY:
                    if (absolutevalue)
                        record.Agility = value;
                    else
                        record.Agility = record.Agility + value;
                    if (record.Agility < 0) record.Agility = 0;
                    if (record.Agility > 99) record.Agility = 99;
                    break;
                case EditableAttributes.AWARENESS:
                    if (absolutevalue)
                        record.Awareness = value;
                    else
                        record.Awareness = record.Awareness + value;
                    if (record.Awareness < 0) record.Awareness = 0;
                    if (record.Awareness > 99) record.Awareness = 99;
                    break;
                case EditableAttributes.BREAK_TACKLE:
                    if (absolutevalue)
                        record.BreakTackle = value;
                    else
                        record.BreakTackle = record.BreakTackle + value;
                    if (record.BreakTackle < 0) record.BreakTackle = 0;
                    if (record.BreakTackle > 99) record.BreakTackle = 99;
                    break;
                case EditableAttributes.CARRYING:
                    if (absolutevalue)
                        record.Carrying = value;
                    else
                        record.Carrying = record.Carrying + value;
                    if (record.Carrying < 0) record.Carrying = 0;
                    if (record.Carrying > 99) record.Carrying = 99;
                    break;
                case EditableAttributes.CATCHING:
                    if (absolutevalue)
                        record.Catching = value;
                    else
                        record.Catching = record.Catching + value;
                    if (record.Catching < 0) record.Catching = 0;
                    if (record.Catching > 99) record.Catching = 99;
                    break;
                case EditableAttributes.IMPORTANCE:
                    if (absolutevalue)
                        record.Importance = value;
                    else
                        record.Importance = record.Importance + value;
                    if (record.Importance < 0) record.Importance = 0;
                    if (record.Importance > 99) record.Importance = 99;
                    break;
                case EditableAttributes.INJURY:
                    if (absolutevalue)
                        record.Injury = value;
                    else
                        record.Injury = record.Injury + value;
                    if (record.Injury < 0) record.Injury = 0;
                    if (record.Injury > 99) record.Injury = 99;
                    break;
                case EditableAttributes.JUMPING:
                    if (absolutevalue)
                        record.Jumping = value;
                    else
                        record.Jumping = record.Jumping + value;
                    if (record.Jumping < 0) record.Jumping = 0;
                    if (record.Jumping > 99) record.Jumping = 99;
                    break;
                case EditableAttributes.KICK_ACCURACY:
                    if (absolutevalue)
                        record.KickAccuracy = value;
                    else
                        record.KickAccuracy = record.KickAccuracy + value;
                    if (record.KickAccuracy < 0) record.KickAccuracy = 0;
                    if (record.KickAccuracy > 99) record.KickAccuracy = 99;
                    break;
                case EditableAttributes.KICK_POWER:
                    if (absolutevalue)
                        record.KickPower = value;
                    else
                        record.KickPower = record.KickPower + value;
                    if (record.KickPower < 0) record.KickPower = 0;
                    if (record.KickPower > 99) record.KickPower = 99;
                    break;
                case EditableAttributes.KICK_RETURN:
                    if (absolutevalue)
                        record.KickReturn = value;
                    else
                        record.KickReturn = record.KickReturn + value;
                    if (record.KickReturn < 0) record.KickReturn = 0;
                    if (record.KickReturn > 99) record.KickReturn = 99;
                    break;
                case EditableAttributes.MORALE:
                    if (absolutevalue)
                        record.Morale = value;
                    else
                        record.Morale = record.Morale + value;
                    if (record.Morale < 0) record.Morale = 0;
                    if (record.Morale > 99) record.Morale = 99;
                    break;
                case EditableAttributes.PASS_BLOCKING:
                    if (absolutevalue)
                        record.PassBlocking = value;
                    else
                        record.PassBlocking = record.PassBlocking + value;
                    if (record.PassBlocking < 0) record.PassBlocking = 0;
                    if (record.PassBlocking > 99) record.PassBlocking = 99;
                    break;
                case EditableAttributes.RUN_BLOCKING:
                    if (absolutevalue)
                        record.RunBlocking = value;
                    else
                        record.RunBlocking = record.RunBlocking + value;
                    if (record.RunBlocking < 0) record.RunBlocking = 0;
                    if (record.RunBlocking > 99) record.RunBlocking = 99;
                    break;
                case EditableAttributes.SPEED:
                    if (absolutevalue)
                        record.Speed = value;
                    else
                        record.Speed = record.Speed + value;
                    if (record.Speed < 0) record.Speed = 0;
                    if (record.Speed > 99) record.Speed = 99;
                    break;
                case EditableAttributes.STAMINA:
                    if (absolutevalue)
                        record.Stamina = value;
                    else
                        record.Stamina = record.Stamina + value;
                    if (record.Stamina < 0) record.Stamina = 0;
                    if (record.Stamina > 99) record.Stamina = 99;
                    break;
                case EditableAttributes.STRENGTH:
                    if (absolutevalue)
                        record.Strength = value;
                    else
                        record.Strength = record.Strength + value;
                    if (record.Strength < 0) record.Strength = 0;
                    if (record.Strength > 99) record.Strength = 99;
                    break;
                case EditableAttributes.TACKLE:
                    if (absolutevalue)
                        record.Tackle = value;
                    else
                        record.Tackle = record.Tackle + value;
                    if (record.Tackle < 0) record.Tackle = 0;
                    if (record.Tackle > 99) record.Tackle = 99;
                    break;
                case EditableAttributes.THROW_ACCURACY:
                    if (absolutevalue)
                        record.ThrowAccuracy = value;
                    else
                        record.ThrowAccuracy = record.ThrowAccuracy + value;
                    if (record.ThrowAccuracy < 0) record.ThrowAccuracy = 0;
                    if (record.ThrowAccuracy > 99) record.ThrowAccuracy = 99;
                    break;
                case EditableAttributes.THROW_POWER:
                    if (absolutevalue)
                        record.ThrowPower = value;
                    else
                        record.ThrowPower = record.ThrowPower + value;
                    if (record.ThrowPower < 0) record.ThrowPower = 0;
                    if (record.ThrowPower > 99) record.ThrowPower = 99;
                    break;
                case EditableAttributes.TOUGHNESS:
                    if (absolutevalue)
                        record.Toughness = value;
                    else
                        record.Toughness = record.Toughness + value;
                    if (record.Toughness < 0) record.Toughness = 0;
                    if (record.Toughness > 99) record.Toughness = 99;
                    break;
                case EditableAttributes.YEARS_EXP:
                    if (absolutevalue)
                        record.YearsPro = value;
                    else
                        record.YearsPro = record.YearsPro + value;
                    if (record.YearsPro < 0) record.YearsPro = 0;
                    break;
                case EditableAttributes.HEIGHT:
                    if (absolutevalue)
                        record.Height = value;
                    else
                        record.Height = record.Height + value;
                    if (record.Height < 0) record.Height = 0;
                    break;
                case EditableAttributes.WEIGHT:
                    if (absolutevalue)
                        record.Weight = value;
                    else
                        record.Weight = record.Weight + value;
                    if (record.Weight < 0) record.Weight = 0;
                    break;
                case EditableAttributes.Throw_Short:
                    if (absolutevalue)
                        record.ThrowShort = value;
                    else
                        record.ThrowShort = record.ThrowShort + value;

                    if (record.ThrowShort < 0) record.ThrowShort = 0;
                    if (record.ThrowShort > 99) record.ThrowShort = 99;
                    break;
                case EditableAttributes.Throw_Medium:
                    if (absolutevalue)
                        record.ThrowMedium = value;
                    else
                        record.ThrowMedium = record.ThrowMedium + value;

                    if (record.ThrowMedium < 0) record.ThrowMedium = 0;
                    if (record.ThrowMedium > 99) record.ThrowMedium = 99;
                    break;
                case EditableAttributes.Throw_Deep:
                    if (absolutevalue)
                        record.ThrowDeep = value;
                    else
                        record.ThrowDeep = record.ThrowDeep + value;

                    if (record.ThrowDeep < 0) record.ThrowDeep = 0;
                    if (record.ThrowDeep > 99) record.ThrowDeep = 99;
                    break;
                case EditableAttributes.Power_Moves:
                    if (absolutevalue)
                        record.PowerMoves = value;
                    else
                        record.PowerMoves = record.PowerMoves + value;

                    if (record.PowerMoves < 0) record.PowerMoves = 0;
                    if (record.PowerMoves > 99) record.PowerMoves = 99;
                    break;
                case EditableAttributes.Finesse_Moves:
                    if (absolutevalue)
                        record.FinesseMoves = value;
                    else
                        record.FinesseMoves = record.FinesseMoves + value;

                    if (record.FinesseMoves < 0) record.FinesseMoves = 0;
                    if (record.FinesseMoves > 99) record.FinesseMoves = 99;
                    break;
                case EditableAttributes.Short_Route:
                    if (absolutevalue)
                        record.ShortRoute = value;
                    else
                        record.ShortRoute = record.ShortRoute + value;

                    if (record.ShortRoute < 0) record.ShortRoute = 0;
                    if (record.ShortRoute > 99) record.ShortRoute = 99;
                    break;
                case EditableAttributes.Medium_Route:
                    if (absolutevalue)
                        record.MediumRoute = value;
                    else
                        record.MediumRoute = record.MediumRoute + value;

                    if (record.MediumRoute < 0) record.MediumRoute = 0;
                    if (record.MediumRoute > 99) record.MediumRoute = 99;
                    break;
                case EditableAttributes.Deep_Route:
                    if (absolutevalue)
                        record.DeepRoute = value;
                    else
                        record.DeepRoute = record.DeepRoute + value;

                    if (record.DeepRoute < 0) record.DeepRoute = 0;
                    if (record.DeepRoute > 99) record.DeepRoute = 99;
                    break;
                case EditableAttributes.Trucking:
                    if (absolutevalue)
                        record.Trucking = value;
                    else
                        record.Trucking = record.Trucking + value;

                    if (record.Trucking < 0) record.Trucking = 0;
                    if (record.Trucking > 99) record.Trucking = 99;
                    break;
                case EditableAttributes.Ball_Carry_Vision:
                    if (absolutevalue)
                        record.RB_Vision = value;
                    else
                        record.RB_Vision = record.RB_Vision + value;

                    if (record.RB_Vision < 0) record.RB_Vision = 0;
                    if (record.RB_Vision > 99) record.RB_Vision = 99;
                    break;
                case EditableAttributes.Press_Coverage:
                    if (absolutevalue)
                        record.PressCover = value;
                    else
                        record.PressCover = record.PressCover + value;

                    if (record.PressCover < 0) record.PressCover = 0;
                    if (record.PressCover > 99) record.PressCover = 99;
                    break;
                case EditableAttributes.Man_Coverage:
                    if (absolutevalue)
                        record.ManCoverage = value;
                    else
                        record.ManCoverage = record.ManCoverage + value;

                    if (record.ManCoverage < 0) record.ManCoverage = 0;
                    if (record.ManCoverage > 99) record.ManCoverage = 99;
                    break;
                case EditableAttributes.Zone_Coverage:
                    if (absolutevalue)
                        record.ZoneCoverage = value;
                    else
                        record.ZoneCoverage = record.ZoneCoverage + value;

                    if (record.ZoneCoverage < 0) record.ZoneCoverage = 0;
                    if (record.ZoneCoverage > 99) record.ZoneCoverage = 99;
                    break;
                case EditableAttributes.Pursuit:
                    if (absolutevalue)
                        record.Pursuit = value;
                    else
                        record.Pursuit = record.Pursuit + value;

                    if (record.Pursuit < 0) record.Pursuit = 0;
                    if (record.Pursuit > 99) record.Pursuit = 99;
                    break;
                case EditableAttributes.Change_Of_Direction:
                    if (absolutevalue)
                        record.Elusive = value;
                    else
                        record.Elusive = record.Elusive + value;

                    if (record.Elusive < 0) record.Elusive = 0;
                    if (record.Elusive > 99) record.Elusive = 99;
                    break;
                case EditableAttributes.Play_Recognition:
                    if (absolutevalue)
                        record.PlayRecognition = value;
                    else
                        record.PlayRecognition = record.PlayRecognition + value;

                    if (record.PlayRecognition < 0) record.PlayRecognition = 0;
                    if (record.PlayRecognition > 99) record.PlayRecognition = 99;
                    break;
                case EditableAttributes.Block_Shedding:
                    if (absolutevalue)
                        record.BlockShedding = value;
                    else
                        record.BlockShedding = record.BlockShedding + value;

                    if (record.BlockShedding < 0) record.BlockShedding = 0;
                    if (record.BlockShedding > 99) record.BlockShedding = 99;
                    break;
                case EditableAttributes.Hit_Power:
                    if (absolutevalue)
                        record.HitPower = value;
                    else
                        record.HitPower = record.HitPower + value;

                    if (record.HitPower < 0) record.HitPower = 0;
                    if (record.HitPower > 99) record.HitPower = 99;
                    break;
                case EditableAttributes.Release:
                    if (absolutevalue)
                        record.Release = value;
                    else
                        record.Release = record.Release + value;

                    if (record.Release < 0) record.Release = 0;
                    if (record.Release > 99) record.Release = 99;
                    break;
                case EditableAttributes.Catch_In_Traffic:
                    if (absolutevalue)
                        record.CatchTraffic = value;
                    else
                        record.CatchTraffic = record.CatchTraffic + value;

                    if (record.CatchTraffic < 0) record.CatchTraffic = 0;
                    if (record.CatchTraffic > 99) record.CatchTraffic = 99;
                    break;
                case EditableAttributes.Spectacular_Catch:
                    if (absolutevalue)
                        record.SpecCatch = value;
                    else
                        record.SpecCatch = record.SpecCatch + value;

                    if (record.SpecCatch < 0) record.SpecCatch = 0;
                    if (record.SpecCatch > 99) record.SpecCatch = 99;
                    break;
                case EditableAttributes.Juke_Move:
                    if (absolutevalue)
                        record.JukeMove = value;
                    else
                        record.JukeMove = record.JukeMove + value;

                    if (record.JukeMove < 0) record.JukeMove = 0;
                    if (record.JukeMove > 99) record.JukeMove = 99;
                    break;
                case EditableAttributes.Stiff_Arm:
                    if (absolutevalue)
                        record.StiffArm = value;
                    else
                        record.StiffArm = record.StiffArm + value;

                    if (record.StiffArm < 0) record.StiffArm = 0;
                    if (record.StiffArm > 99) record.StiffArm = 99;
                    break;
                case EditableAttributes.Throw_On_Run:
                    if (absolutevalue)
                        record.ThrowOnRun = value;
                    else
                        record.ThrowOnRun = record.ThrowOnRun + value;

                    if (record.ThrowOnRun < 0) record.ThrowOnRun = 0;
                    if (record.ThrowOnRun > 99) record.ThrowOnRun = 99;
                    break;
                case EditableAttributes.Spin_Move:
                    if (absolutevalue)
                        record.SpinMove = value;
                    else
                        record.SpinMove = record.SpinMove + value;

                    if (record.SpinMove < 0) record.SpinMove = 0;
                    if (record.SpinMove > 99) record.SpinMove = 99;
                    break;
                case EditableAttributes.Break_Sack:
                    if (absolutevalue)
                        record.BreakSack = value;
                    else
                        record.BreakSack = record.BreakSack + value;

                    if (record.BreakSack < 0) record.BreakSack = 0;
                    if (record.BreakSack > 99) record.BreakSack = 99;
                    break;
                case EditableAttributes.Throw_Under_Pressure:
                    if (absolutevalue)
                        record.ThrowPressure = value;
                    else
                        record.ThrowPressure = record.ThrowPressure + value;

                    if (record.ThrowPressure < 0) record.ThrowPressure = 0;
                    if (record.ThrowPressure > 99) record.ThrowPressure = 99;
                    break;
                case EditableAttributes.Impact_Block:
                    if (absolutevalue)
                        record.ImpactBlocking = value;
                    else
                        record.ImpactBlocking = record.ImpactBlocking + value;

                    if (record.ImpactBlocking < 0) record.ImpactBlocking = 0;
                    if (record.ImpactBlocking > 99) record.ImpactBlocking = 99;
                    break;
                case EditableAttributes.Lead_Block:
                    if (absolutevalue)
                        record.LeadBlock = value;
                    else
                        record.LeadBlock = record.LeadBlock + value;

                    if (record.LeadBlock < 0) record.LeadBlock = 0;
                    if (record.LeadBlock > 99) record.LeadBlock = 99;
                    break;
                case EditableAttributes.Play_Action:
                    if (absolutevalue)
                        record.PlayAction = value;
                    else
                        record.PlayAction = record.PlayAction + value;

                    if (record.PlayAction < 0) record.PlayAction = 0;
                    if (record.PlayAction > 99) record.PlayAction = 99;
                    break;
                case EditableAttributes.Pass_Block_Footwork:
                    if (absolutevalue)
                        record.PassBlockFootwork = value;
                    else
                        record.PassBlockFootwork = record.PassBlockFootwork + value;

                    if (record.PassBlockFootwork < 0) record.PassBlockFootwork = 0;
                    if (record.PassBlockFootwork > 99) record.PassBlockFootwork = 99;
                    break;
                case EditableAttributes.Pass_Block_Strength:
                    if (absolutevalue)
                        record.PassBlockStrength = value;
                    else
                        record.PassBlockStrength = record.PassBlockStrength + value;

                    if (record.PassBlockStrength < 0) record.PassBlockStrength = 0;
                    if (record.PassBlockStrength > 99) record.PassBlockStrength = 99;
                    break;
                case EditableAttributes.Run_Block_Footwork:
                    if (absolutevalue)
                        record.RunBlockFootwork = value;
                    else
                        record.RunBlockFootwork = record.RunBlockFootwork + value;

                    if (record.RunBlockFootwork < 0) record.RunBlockFootwork = 0;
                    if (record.RunBlockFootwork > 99) record.RunBlockFootwork = 99;
                    break;
                case EditableAttributes.Run_Block_Strength:
                    if (absolutevalue)
                        record.RunBlockStrength = value;
                    else
                        record.RunBlockStrength = record.RunBlockStrength + value;

                    if (record.RunBlockStrength < 0) record.RunBlockStrength = 0;
                    if (record.RunBlockStrength > 99) record.RunBlockStrength = 99;
                    break;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void attributeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                GlobalTraitOption.BackColor = SystemColors.Control;
                attributeCombo.BackColor = SystemColors.Control;
                if (attributeCombo.SelectedIndex <= 0)
                    return;
                else
                {
                    attributeCombo.BackColor = Color.LightBlue;
                    GlobalTraitOption.BackColor = Color.LightBlue;
                }
            }
        }

        private void traitCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                InitTraitOptions();
            }
        }

        public void InitTraitOptions()
        {
            TraitON.Enabled = false;
            TraitOFF.Enabled = true;
            TraitON.Checked = true;
            TraitOptionsCombo.Enabled = false;
            TraitOptionsCombo.Items.Clear();

            if (traitCombo.SelectedIndex < 0)
                return;
            else if (traitCombo.SelectedIndex <= 14)
            {
                TraitON.Enabled = true;
                TraitOFF.Enabled = true;
                TraitOptionsCombo.Enabled = false;
            }
            else if (traitCombo.SelectedIndex >= 15)
            {
                TraitON.Enabled = false;
                TraitOptionsCombo.Enabled = true;
                if (traitCombo.Text == "CoversBall")
                {
                    TraitOptionsCombo.Items.Add("Always");
                    TraitOptionsCombo.Items.Add("Never");
                    TraitOptionsCombo.Items.Add("Brace Big");
                    TraitOptionsCombo.Items.Add("Brace Med");
                    TraitOptionsCombo.Items.Add("Brace ALL");
                }
                else if (traitCombo.Text == "ForcesPasses")
                {
                    TraitOptionsCombo.Items.Add("Conservative");
                    TraitOptionsCombo.Items.Add("Ideal");
                    TraitOptionsCombo.Items.Add("Aggressive");
                }
                else if (traitCombo.Text == "PlaysBall")
                {
                    TraitOptionsCombo.Items.Add("Conservative");
                    TraitOptionsCombo.Items.Add("Balanced");
                    TraitOptionsCombo.Items.Add("Aggressive");
                }
                else if (traitCombo.Text == "LBStyle")
                {
                    TraitOptionsCombo.Items.Add("Pass Rush");
                    TraitOptionsCombo.Items.Add("Balance");
                    TraitOptionsCombo.Items.Add("Coverage");
                }
                else if (traitCombo.Text == "Penalty")
                {
                    TraitOptionsCombo.Items.Add("Undisciplined");
                    TraitOptionsCombo.Items.Add("Normal");
                    TraitOptionsCombo.Items.Add("Disciplined");
                }
                else if (traitCombo.Text == "SensePressure")
                {
                    TraitOptionsCombo.Items.Add("Paranoid");
                    TraitOptionsCombo.Items.Add("Trigger Happy");
                    TraitOptionsCombo.Items.Add("Ideal");
                    TraitOptionsCombo.Items.Add("Average");
                    TraitOptionsCombo.Items.Add("Oblivious");
                }
                else if (traitCombo.Text == "QBPlayStyle")
                {
                    TraitOptionsCombo.Items.Add("Balance");
                    TraitOptionsCombo.Items.Add("Pocket");
                    TraitOptionsCombo.Items.Add("Scramble");
                }
                else if (traitCombo.Text == "DevelopmentTrait")
                {
                    TraitOptionsCombo.Items.Add("Normal");
                    TraitOptionsCombo.Items.Add("Star");
                    TraitOptionsCombo.Items.Add("Superstar");
                    TraitOptionsCombo.Items.Add("X-Factor");
                }
                else if (traitCombo.Text == "CareerPhase")
                {
                    TraitOptionsCombo.Items.Add("Progression Phase");
                    TraitOptionsCombo.Items.Add("Plateau Phase");
                    TraitOptionsCombo.Items.Add("Regression Phase");
                }
                else if (traitCombo.Text == "RunningTechnique")
                {
                    TraitOptionsCombo.Items.Add("*Do Not Select*");
                    TraitOptionsCombo.Items.Add("Default");
                    TraitOptionsCombo.Items.Add("DefaultStrideHighAndTight");
                    TraitOptionsCombo.Items.Add("DefaultStrideLoaf");
                    TraitOptionsCombo.Items.Add("DefaultStrideBreadLoose");
                    TraitOptionsCombo.Items.Add("DefaultStrideAwkward");
                    TraitOptionsCombo.Items.Add("ShortStrideDefault");
                    TraitOptionsCombo.Items.Add("ShortStrideHighAndTight");
                    TraitOptionsCombo.Items.Add("ShortStrideLoose");
                    TraitOptionsCombo.Items.Add("ShortStrideBreadLoaf");
                    TraitOptionsCombo.Items.Add("ShortStrideAwkward");
                    TraitOptionsCombo.Items.Add("LongStrideDefault");
                    TraitOptionsCombo.Items.Add("LongStrideHighAdTight");
                    TraitOptionsCombo.Items.Add("LongStrideLoose");
                    TraitOptionsCombo.Items.Add("LongStrideBreadLoaf");
                    TraitOptionsCombo.Items.Add("LongStrideAwkward");
                }
                else if (traitCombo.Text == "QBThrowingMotion")
                {
                    TraitOptionsCombo.Items.Add("Generic 1");
                    TraitOptionsCombo.Items.Add("Generic 2");
                    TraitOptionsCombo.Items.Add("Traditional 1");
                    TraitOptionsCombo.Items.Add("Traditional 2");
                    TraitOptionsCombo.Items.Add("Traditional 3");
                    TraitOptionsCombo.Items.Add("Traditional 4");
                    TraitOptionsCombo.Items.Add("Traditional 5");
                    TraitOptionsCombo.Items.Add("Slinger 1");
                    TraitOptionsCombo.Items.Add("Slinger 2");
                    TraitOptionsCombo.Items.Add("Slinger 3");
                    TraitOptionsCombo.Items.Add("Slinger 4");
                    TraitOptionsCombo.Items.Add("Slinger 5");
                    TraitOptionsCombo.Items.Add("Slinger 6");
                    TraitOptionsCombo.Items.Add("Slinger 7");
                }

            }

        }

        private void TraitON_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;
                if (TraitON.Checked)
                    TraitOFF.Checked = false;
                else TraitOFF.Checked = true;
                isInitializing = false;
            }

        }

        private void TraitOptionsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TraitOFF_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;
                if (TraitOFF.Checked)
                    TraitON.Checked = false;
                else TraitON.Checked = true;
                isInitializing = false;
            }
        }

        private void MiscCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
                InitMiscOptions();
        }

        public void InitMiscOptions()
        {
            if (MiscCombo.SelectedIndex < 0)
                return;
            else MiscOptionsCombo.Items.Clear();

            if (MiscCombo.SelectedIndex == 0)
            {
                for (int c = 0; c < model.PlayerModel.EndPlayList.Count; c++)
                    MiscOptionsCombo.Items.Add(model.PlayerModel.GetEndPlay(c));
            }
        }

        public void InitEquipOptions()
        {
            if (EquipCombo.SelectedIndex < 0)
                return;
            else EquipOptions.Items.Clear();

            if (EquipCombo.SelectedIndex == 0)
            {
                for (int c = 0; c < model.PlayerModel.HelmetStyleList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetHelmet(c));
            }
            else if (EquipCombo.SelectedIndex == 1)
            {
                for (int c = 0; c < model.PlayerModel.FacemaskList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetFaceMask(c));
            }
            else if (EquipCombo.SelectedIndex >= 2 && EquipCombo.SelectedIndex <= 4)
            {
                for (int c = 0; c < model.PlayerModel.ShoeList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetShoe(c));
            }
            else if (EquipCombo.SelectedIndex >= 5 && EquipCombo.SelectedIndex <= 7)
            {
                for (int c = 0; c < model.PlayerModel.GloveList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetGloves(c));
            }
            else if (EquipCombo.SelectedIndex >= 8 && EquipCombo.SelectedIndex <= 10)
            {
                for (int c = 0; c < model.PlayerModel.WristBandList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetWrist(c));
            }
            else if (EquipCombo.SelectedIndex >= 11 && EquipCombo.SelectedIndex <= 13)
            {
                for (int c = 0; c < model.PlayerModel.SleeveList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetSleeve(c));
            }
            else if (EquipCombo.SelectedIndex == 14)        
            {
                EquipOptions.Items.Add("Tucked in");
                EquipOptions.Items.Add("Untucked");
                EquipOptions.Items.Add("Rolled up");
                EquipOptions.Items.Add("*Compression ShortSleeve Nike White");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Red");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Pink");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Navy");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Green");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Gold");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Black");    //Vanity Items
                EquipOptions.Items.Add("*Compression ShortSleeve Nike Bleige");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike Bleige");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike Black");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike Gold");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike Green");    //Vanity Item
                EquipOptions.Items.Add("*Compression LongSleeve Nike Navy");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike Pink");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike Red");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Nike White");    //Vanity Items
                EquipOptions.Items.Add("*Compression LongSleeve Miami 80s Blue");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike White");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Red");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Pink");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Navy");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Green");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Gold");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Black");
                EquipOptions.Items.Add("*Hoodie Sleeveless Nike Bleige");
                EquipOptions.Items.Add("*Hoodie Sleeveless Brotherhood Orange");
                EquipOptions.Items.Add("*Hoodie Sleeveless Brotherhood Black");
            }
            else if (EquipCombo.SelectedIndex >= 15 && EquipCombo.SelectedIndex <= 17)
            {
                for (int c = 0; c < model.PlayerModel.ElbowList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetElbow(c));
            }
            else if (EquipCombo.SelectedIndex >= 18 && EquipCombo.SelectedIndex <= 20)
            {
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                {
                    EquipOptions.Items.Add("Normal");
                    EquipOptions.Items.Add("Brace");
                }
                else
                {
                    EquipOptions.Items.Add("Nike");
                    EquipOptions.Items.Add("Regular");
                }
            }
            else if (EquipCombo.SelectedIndex >= 21 && EquipCombo.SelectedIndex <= 23)
            {
                for (int c = 0; c < model.PlayerModel.AnkleList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetAnkle(c));
            }
            else if (EquipCombo.SelectedIndex == 24)
            {
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                {
                    EquipOptions.Items.Add("None");
                    EquipOptions.Items.Add("Normal");
                    EquipOptions.Items.Add("Extended");
                }
                else
                {
                    EquipOptions.Items.Add("None");
                    EquipOptions.Items.Add("Vintage Neck Roll");
                    EquipOptions.Items.Add("Butterfly");
                    EquipOptions.Items.Add("Vintage Single");
                    EquipOptions.Items.Add("Cowboy Collar");
                }
            }
            else if (EquipCombo.SelectedIndex == 25)
            {
                for (int c = 0; c < model.PlayerModel.VisorList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetVisor(c));
            }
            else if (EquipCombo.SelectedIndex == 26)
            {
                for (int c = 0; c < model.PlayerModel.FaceMarkList.Count; c++)
                    EquipOptions.Items.Add(model.PlayerModel.GetFaceMark(c));
            }
            else if (EquipCombo.SelectedIndex == 27)
            {
                EquipOptions.Items.Add("None");
                EquipOptions.Items.Add("Pacifier White");
                EquipOptions.Items.Add("Pacifier Black");
                EquipOptions.Items.Add("Pacifier Team");
                EquipOptions.Items.Add("Pacifier Secondary");
            }
            else if (EquipCombo.SelectedIndex == 28)
            {
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                {
                    EquipOptions.Items.Add("Standard");
                    EquipOptions.Items.Add("Low");
                    EquipOptions.Items.Add("High");
                    EquipOptions.Items.Add("Tucked");
                }
            }
            else if (EquipCombo.SelectedIndex == 29)
            {
                EquipOptions.Items.Add("Tight");
                EquipOptions.Items.Add("Standard");
                EquipOptions.Items.Add("Long");
            }
            else if (EquipCombo.SelectedIndex == 30)
            {
                EquipOptions.Items.Add("None");
                EquipOptions.Items.Add("Front-Center");
                EquipOptions.Items.Add("Front-Left");
                EquipOptions.Items.Add("Left Side");
                EquipOptions.Items.Add("Back-Left");
                EquipOptions.Items.Add("Back-Center");
                EquipOptions.Items.Add("Back-Right");
                EquipOptions.Items.Add("Right Side");
                //EquipOptions.Items.Add("Front-Right"); Disabled for now 11-18-23
            }
            else if (EquipCombo.SelectedIndex == 31)
            {
                EquipOptions.Items.Add("None");
                EquipOptions.Items.Add("Front Side");
                EquipOptions.Items.Add("Back Side");
            }
            else if (EquipCombo.SelectedIndex == 32)
            {
                EquipOptions.Items.Add("Equipped");
                EquipOptions.Items.Add("Unequipped");
            }
            else if (EquipCombo.SelectedIndex == 33)
            {
                EquipOptions.Items.Add("Equipped");
                EquipOptions.Items.Add("Unequipped");
            }
            else if (EquipCombo.SelectedIndex == 34)
            {
                EquipOptions.Items.Add("Untucked");
                EquipOptions.Items.Add("Tucked In");
            }
        }

        private void EquipCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
                InitEquipOptions();
        }

        private void MiscOptionsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rbYearsProGreaterThan_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAgeFilter_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}


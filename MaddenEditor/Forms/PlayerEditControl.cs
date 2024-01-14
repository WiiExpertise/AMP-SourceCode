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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaddenEditor.Core;
using MaddenEditor.Core.Record;
using MaddenEditor.Core.Record.Stats;
using MaddenEditor.Db;
using System.Diagnostics;


namespace MaddenEditor.Forms
{
    //  TO DO:  FIX previous bonuses owed, taken from cap room


    public partial class PlayerEditControl : UserControl, IEditorForm
    {
        private EditorModel model = null;
        private PlayerRecord lastLoadedRecord = null;
        private bool isInitialising = false;
        public Overall playeroverall = new Overall();  //check here
        public List<string> CurrentPlayers = new List<string>();
        public int currentplayerrow = 0;
        public int year = 0;
        public int selectedyear = -1;
        public int baseyear = 2007;
        Dictionary<int, int> teamsalaries = new Dictionary<int, int>();
        public bool Madden19 = false;
        private MGMT _manager;
        private object playersidelineCatch;
        private object playerRunStyle;

        public MGMT manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        public PlayerEditControl()
        {
            isInitialising = true;

            InitializeComponent();

            isInitialising = false;
        }

        #region IEditorForm Members

        public void InitialiseUI()
        {
            isInitialising = true;
            SuspendLayout();

            #region Stats Tab
            if (model.FileType != MaddenFileType.Franchise)
                tabControl.TabPages.Remove(statsPage);
            #endregion

            InitCollegeList();

            // TO DO : try to move this to when file is processed
            #region Get Total Number of Players
            EditorModel.totalplayers = 0;
            foreach (PlayerRecord rec in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                if (rec.PlayerId > EditorModel.totalplayers)
                    EditorModel.totalplayers = rec.PlayerId;
            }
            #endregion

            #region Player Info & Player Ratings Tab
            PortraitID_Large.Visible = false;
            playerAsset.Text = "";
            playerAsset.Enabled = false;
            playerHometown.Text = "";
            playerHometown.Enabled = false;
            playerStateCombo.SelectedIndex = -1;
            playerStateCombo.Enabled = false;
            playerBirthday.Text = "";
            playerBirthday.Enabled = false;
            playerDraftRound.Maximum = 15;
            playerDraftRoundIndex.Maximum = 63;
            AudioCombobox.Enabled = false;

            #region Height
            string feet = " '";
            string inches = " \"";
            for (int c = 0; c < 12; c++)
            {
                if (c == 0)
                {
                    PlayerHeight_Feet.Items.Add("");
                    PlayerHeight_Inches.Items.Add("");
                }
                if (c != 0 && c < 11)
                    PlayerHeight_Feet.Items.Add(c.ToString() + feet);
                if (c != 0)
                    PlayerHeight_Inches.Items.Add(c.ToString() + inches);
            }
            #endregion

            filterTeamComboBox.Items.Add("*ALL*");
            foreach (TableRecordModel record in model.TableModels[EditorModel.TEAM_TABLE].GetRecords())
            {
                // Only add teams and 'Free Agents' to the combo box, not probowl teams (AFC,NFC)
                if (record.GetIntField("TGID") <= 1009)
                {
                    Team_Combo.Items.Add(record);
                    filterTeamComboBox.Items.Add(record);
                }
            }
            // Add Retired to the team lists
            if (model.FileType == MaddenFileType.Franchise)
            {
                Team_Combo.Items.Add(EditorModel.RETIRED);
                filterTeamComboBox.Items.Add(EditorModel.RETIRED);
            }

            filterPositionComboBox.Items.Add("*ALL*");
            for (int p = 0; p < 21; p++)
            {
                string pos = Enum.GetName(typeof(MaddenPositions), p);
                positionComboBox.Items.Add(pos);
                filterPositionComboBox.Items.Add(pos);
                OriginalPosition_Combo.Items.Add(pos);
            }
            FilterCollegecomboBox.Items.Add("*ALL*"); //new
            foreach (KeyValuePair<int, college_entry> col in model.Colleges)
            {
                string name = col.Value.name;
                FilterCollegecomboBox.Items.Add(name);
                FilterCollegecomboBox.Update();

            }
            FilterDevTraitcomboBox.Items.Add("*ALL*"); //new
     
            {
                FilterDevTraitcomboBox.Items.Add("Normal"); //new
                FilterDevTraitcomboBox.Items.Add("Star"); //new
                FilterDevTraitcomboBox.Items.Add("Superstar"); //new
                FilterDevTraitcomboBox.Items.Add("X-Factor"); //new

            }
            FilterArchetypecomboBox.Items.Add("*ALL*"); //new

            {
                FilterArchetypecomboBox.Items.Add("Field General QB"); //new
                FilterArchetypecomboBox.Items.Add("Strong Arm QB"); //new
                FilterArchetypecomboBox.Items.Add("Improviser QB"); //new
                FilterArchetypecomboBox.Items.Add("Scrambler QB"); //new
                FilterArchetypecomboBox.Items.Add("Power Back RB"); //new
                FilterArchetypecomboBox.Items.Add("Elusive Back RB"); //new
                FilterArchetypecomboBox.Items.Add("Receiving Back RB"); //new
                FilterArchetypecomboBox.Items.Add("Blocking FB"); //new
                FilterArchetypecomboBox.Items.Add("Utility FB"); //new
                FilterArchetypecomboBox.Items.Add("Deep Threat WR"); //new
                FilterArchetypecomboBox.Items.Add("Route Runner WR"); //new
                FilterArchetypecomboBox.Items.Add("Physical WR"); //new
                FilterArchetypecomboBox.Items.Add("Slot WR"); //new
                FilterArchetypecomboBox.Items.Add("Blocking TE"); //new
                FilterArchetypecomboBox.Items.Add("Vertical Threat TE"); //new
                FilterArchetypecomboBox.Items.Add("Possession TE"); //new
                FilterArchetypecomboBox.Items.Add("Pass Prot. C"); //new
                FilterArchetypecomboBox.Items.Add("Power C"); //new
                FilterArchetypecomboBox.Items.Add("Agile C"); //new
                FilterArchetypecomboBox.Items.Add("Pass Prot. T"); //new
                FilterArchetypecomboBox.Items.Add("Power T"); //new
                FilterArchetypecomboBox.Items.Add("Agile T"); //new
                FilterArchetypecomboBox.Items.Add("Pass Prot. G"); //new
                FilterArchetypecomboBox.Items.Add("Power G"); //new
                FilterArchetypecomboBox.Items.Add("Agile G"); //new
                FilterArchetypecomboBox.Items.Add("Speed Rusher DE"); //new
                FilterArchetypecomboBox.Items.Add("Power Rusher DE"); //new
                FilterArchetypecomboBox.Items.Add("Run Stopper DE"); //new
                FilterArchetypecomboBox.Items.Add("Run Stopper DT"); //new
                FilterArchetypecomboBox.Items.Add("Speed Rusher DT"); //new
                FilterArchetypecomboBox.Items.Add("Power Rusher DT"); //new
                FilterArchetypecomboBox.Items.Add("Speed Rusher OLB"); //new
                FilterArchetypecomboBox.Items.Add("Power Rusher OLB"); //new
                FilterArchetypecomboBox.Items.Add("Pass Coverage OLB"); //new
                FilterArchetypecomboBox.Items.Add("Run Stopper OLB"); //new
                FilterArchetypecomboBox.Items.Add("Field General MLB"); //new
                FilterArchetypecomboBox.Items.Add("Pass Coverage MLB"); //new
                FilterArchetypecomboBox.Items.Add("Run Stopper MLB"); //new
                FilterArchetypecomboBox.Items.Add("Man to Man CB"); //new
                FilterArchetypecomboBox.Items.Add("Slot CB"); //new
                FilterArchetypecomboBox.Items.Add("Zone CB"); //new
                FilterArchetypecomboBox.Items.Add("Zone S"); //new
                FilterArchetypecomboBox.Items.Add("Hybrid S"); //new
                FilterArchetypecomboBox.Items.Add("Run Stopper S"); //new
                FilterArchetypecomboBox.Items.Add("Accurate K"); //new
                FilterArchetypecomboBox.Items.Add("Power K"); //new
            }
                FilterYearsProcomboBox.Items.Add("*ALL*"); //new

            {
                FilterYearsProcomboBox.Items.Add("Rookie"); //new
                FilterYearsProcomboBox.Items.Add("Year 1"); //new
                FilterYearsProcomboBox.Items.Add("Year 2"); //new
                FilterYearsProcomboBox.Items.Add("Year 3"); //new
                FilterYearsProcomboBox.Items.Add("Year 4"); //new
                FilterYearsProcomboBox.Items.Add("Year 5"); //new
                FilterYearsProcomboBox.Items.Add("Year 6"); //new
                FilterYearsProcomboBox.Items.Add("Year 7"); //new
                FilterYearsProcomboBox.Items.Add("Year 8"); //new
                FilterYearsProcomboBox.Items.Add("Year 9"); //new
                FilterYearsProcomboBox.Items.Add("Year 10"); //new
                FilterYearsProcomboBox.Items.Add("Year 11"); //new
                FilterYearsProcomboBox.Items.Add("Year 12"); //new
                FilterYearsProcomboBox.Items.Add("Year 13"); //new
                FilterYearsProcomboBox.Items.Add("Year 14"); //new
                FilterYearsProcomboBox.Items.Add("Year 15"); //new
                FilterYearsProcomboBox.Items.Add("Year 16"); //new
                FilterYearsProcomboBox.Items.Add("Year 17"); //new
                FilterYearsProcomboBox.Items.Add("Year 18"); //new
                FilterYearsProcomboBox.Items.Add("Year 19"); //new
                FilterYearsProcomboBox.Items.Add("Year 20"); //new
                FilterYearsProcomboBox.Items.Add("Year 21"); //new
                FilterYearsProcomboBox.Items.Add("Year 22"); //new
                FilterYearsProcomboBox.Items.Add("Year 23"); //new
                FilterYearsProcomboBox.Items.Add("Year 24"); //new
                FilterYearsProcomboBox.Items.Add("Year 25"); //new
                FilterYearsProcomboBox.Items.Add("Year 26"); //new
                FilterYearsProcomboBox.Items.Add("Year 27"); //new
                FilterYearsProcomboBox.Items.Add("Year 28"); //new
                FilterYearsProcomboBox.Items.Add("Year 29"); //new
                FilterYearsProcomboBox.Items.Add("Year 30"); //new
            }

            FilterCollegecomboBox.SelectedIndex = 0;    //new
            filterTeamComboBox.SelectedIndex = 0;
            filterPositionComboBox.SelectedIndex = 0;
            FilterDevTraitcomboBox.SelectedIndex = 0;   //new
            FilterArchetypecomboBox.SelectedIndex = 0;  //new
            FilterYearsProcomboBox.SelectedIndex = 0;   //new
            PlayerTendency.Enabled = false;
            RoleLabel.Visible = false;
            WeaponLabel.Visible = false;
            PlayerRolecomboBox.Visible = false;
            PlayerWeaponcomboBox.Visible = false;

            PlayerHoldOut.Enabled = true;
            PlayerInactive.Visible = false;

            SevereLabel.Visible = false;
            ReturnLabel.Visible = false;
            playerInjuryReturn.Visible = false;
            playerInjurySevere.Visible = false;
            playerCaptain.Visible = false;

            playerOVRArchetypeCombo.Items.Clear();
            playerOVRArchetypeCombo.Enabled = false;
            playerOVRArchetype.Text = "NA";
            playerOVRArchetype.Enabled = false;

            Ratings19_Panel.Visible = false;
            TraitsPanel.Visible = false;

            if (model.MadVersion == MaddenFileVersion.Ver2004)
            {
                playerNFLIcon.Enabled = false;
            }
            if (model.MadVersion >= MaddenFileVersion.Ver2005 && model.FileType == MaddenFileType.Franchise)
            {
                PlayerInactive.Visible = true;
                playerCaptain.Visible = true;
            }

            if (model.MadVersion >= MaddenFileVersion.Ver2005 && model.MadVersion < MaddenFileVersion.Ver2019)
            {
                playerNFLIcon.Enabled = true;
            }

            if (model.MadVersion == MaddenFileVersion.Ver2007 || model.MadVersion == MaddenFileVersion.Ver2008)
            {
                RoleLabel.Visible = true;
                PlayerRolecomboBox.Visible = true;
                // stats
                ComeBacks_Label.Visible = true;
                FirstDowns_Label.Visible = true;
                comebacks.Visible = true;
                Firstdowns.Visible = true;

                model.InitRoles(manager.stream_model);
                List<int> roles = model.PlayerRole.Keys.ToList();
                roles.Sort();
                foreach (int r in roles)
                {
                    PlayerRolecomboBox.Items.Add(model.PlayerRole[r]);
                    PlayerWeaponcomboBox.Items.Add(model.PlayerRole[r]);
                }

                if (model.MadVersion == MaddenFileVersion.Ver2008)
                {
                    WeaponLabel.Visible = true;
                    PlayerWeaponcomboBox.Visible = true;
                }
            }

            if (model.MadVersion >= MaddenFileVersion.Ver2019)
            {
                calculateOverallButton.Enabled = true;
                playerPortraitId.Maximum = 9999;
                PortraitID_Large.Visible = true;
                firstNameTextBox.MaxLength = 13;
                lastNameTextBox.MaxLength = 17;
                playerAsset.Enabled = true;
                playerHometown.Enabled = true;
                playerStateCombo.Enabled = true;
                playerBirthday.Enabled = true;
                playerDraftRound.Maximum = 63;
                playerDraftRoundIndex.Maximum = 511;
                OriginalPosition_Combo.SelectedIndex = -1;
                OriginalPosition_Combo.Enabled = false;
                PlayerHoldOut.Visible = true;
                playerCaptain.Visible = true;
                playerCaptain.Enabled = true;

                DevTrait.Items.Add("Normal");
                if (model.MadVersion == MaddenFileVersion.Ver2019)
                {
                    DevTrait.Items.Add("Quick");
                    DevTrait.Items.Add("Star");
                    DevTrait.Items.Add("Superstar");
                }
                else if (model.MadVersion == MaddenFileVersion.Ver2020)
                {
                    DevTrait.Items.Add("Star");
                    DevTrait.Items.Add("SuperStar");
                    DevTrait.Items.Add("X-Factor");
                }



                #region QB Style
                QBStyle.Items.Clear();
                if (model.MadVersion == MaddenFileVersion.Ver2019)
                {
                    QBStyle.Items.Add("Generic");
                    QBStyle.Items.Add("TEdwards");
                    QBStyle.Items.Add("Pennington");
                    QBStyle.Items.Add("Favre");
                    QBStyle.Items.Add("Brady");
                    QBStyle.Items.Add("DAnderson");
                    QBStyle.Items.Add("Garcia");
                    QBStyle.Items.Add("Roethlisberger");
                    QBStyle.Items.Add("PManning");
                    QBStyle.Items.Add("Garrard");
                    QBStyle.Items.Add("Orton");
                    QBStyle.Items.Add("Young");
                    QBStyle.Items.Add("Cutler");
                    QBStyle.Items.Add("Rivers");
                    QBStyle.Items.Add("Flacco");
                    QBStyle.Items.Add("JRussell");
                    QBStyle.Items.Add("Romo");
                    QBStyle.Items.Add("McNabb");
                    QBStyle.Items.Add("EManning");
                    QBStyle.Items.Add("Campbell");
                    QBStyle.Items.Add("Grossman");
                    QBStyle.Items.Add("Kitna");
                    QBStyle.Items.Add("Rodgers");
                    QBStyle.Items.Add("Cassel");
                    QBStyle.Items.Add("Stafford");
                    QBStyle.Items.Add("Ryan");
                    QBStyle.Items.Add("Delhomme");
                    QBStyle.Items.Add("Brees");
                    QBStyle.Items.Add("ASmith");
                    QBStyle.Items.Add("Warner");
                    QBStyle.Items.Add("Bulger");
                    QBStyle.Items.Add("Hasselbeck");
                    QBStyle.Items.Add("Palmer");
                    QBStyle.Items.Add("Sanchez");
                    QBStyle.Items.Add("Bradford");
                    QBStyle.Items.Add("Clausen");
                    QBStyle.Items.Add("Tebow");
                    QBStyle.Items.Add("Vick");
                    QBStyle.Items.Add("Freeman");
                    QBStyle.Items.Add("Aikman");
                    QBStyle.Items.Add("Blanda");
                    QBStyle.Items.Add("Elway");
                    QBStyle.Items.Add("Graham");
                    QBStyle.Items.Add("Montana");
                    QBStyle.Items.Add("Moon");
                    QBStyle.Items.Add("SYoung");
                }
                else if (model.MadVersion == MaddenFileVersion.Ver2020)
                {
                    QBStyle.Items.Add("Generic 1 - Slow");
                    QBStyle.Items.Add("Generic 2 - Generic");
                    QBStyle.Items.Add("Traditional 1 - Tom Brady");
                    QBStyle.Items.Add("Traditional 2 - Joe Burrow");
                    QBStyle.Items.Add("Traditional 3 - Mac Jones");
                    QBStyle.Items.Add("Traditional 4 - Lamar Jackson");
                    QBStyle.Items.Add("Traditional 5 - Russell Wilson");
                    QBStyle.Items.Add("Slinger 1 - Aaron Rodgers");
                    QBStyle.Items.Add("Slinger 2 - Patrick Mahomes");
                    QBStyle.Items.Add("Slinger 3 - Josh Allen");
                    QBStyle.Items.Add("Slinger 4 - Kyler Murray");
                    QBStyle.Items.Add("Slinger 5 - Justin Herbert");
                    QBStyle.Items.Add("Slinger 6 - Matt Stafford");
                    QBStyle.Items.Add("Slinger 7 - Baker Mayfield");
                    QBStyle.Items.Add("(Removed)");
                    QBStyle.Items.Add("(Removed)");
                    QBStyle.Items.Add("(Removed)");
                    QBStyle.Items.Add("(Removed)");
                    QBStyle.Items.Add("(Removed)");
                }

                #endregion

                #region Injuries
                SevereLabel.Visible = true;
                ReturnLabel.Visible = true;
                playerInjuryReturn.Visible = true;
                playerInjurySevere.Visible = true;
                playerInjuryCombo.Items.Clear();
                if (model.PlayerModel.InjuryList != null)
                {
                    foreach (GenericRecord rec in model.PlayerModel.InjuryList)
                        playerInjuryCombo.Items.Add(rec);
                }
                #endregion

                #region MouthPiece
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                {
                    playerMouthPieceCombo.Items.Clear();
                    playerMouthPieceCombo.Items.Add("None");
                    playerMouthPieceCombo.Items.Add("Pacifier White");
                    playerMouthPieceCombo.Items.Add("Pacifier Black");
                    playerMouthPieceCombo.Items.Add("Pacifier Team");
                    if (model.MadVersion == MaddenFileVersion.Ver2020)
                        playerMouthPieceCombo.Items.Add("Pacifier Secondary");
                    playerMouthPieceCombo.Items.Add("(Removed)");
                    playerMouthPieceCombo.Items.Add("(Removed)");
                    playerMouthPieceCombo.Items.Add("(Removed)");
                    playerMouthPieceCombo.Items.Add("*Dual Black V Crew Pacifier");
                }
                #endregion

                TendencyLabel.Visible = false;
                PlayerTendency.Visible = false;
                OriginalPosition_Label.Visible = false;
                OriginalPosition_Combo.Visible = false;

                playerOVRArchetypeCombo.Enabled = true;
                playerOVRArchetype.Enabled = true;
                playerMorale.Visible = false;
                MoraleLabel.Visible = false;
                Ratings19_Panel.Visible = true;
                playeroverall.InitRatings19(); //check here
                TraitsPanel.Visible = true;

                AudioCombobox.Enabled = true;
                foreach (KeyValuePair<string, int> id in model.PlayerModel.PlayerComments)
                {
                    AudioCombobox.Items.Add(id.Key);
                }
            }

            #region Portraits
            if (manager.PlayerPortDAT.isterf)
            {
                ImportPlayerPort_Button.Visible = true;
                PlayerPortraitExport_Button.Visible = true;
            }
            else
            {
                ImportPlayerPort_Button.Visible = false;
                PlayerPortraitExport_Button.Visible = false;
            }
            #endregion
            #endregion

            #region Appearance / Equipment

            NextGenPanel.Visible = false;
            LegacyPanel.Visible = true;   //Body section
            playerSockHeight.Enabled = false;

            #region Equipment

            #region Helmet
            playerHelmetStyleCombo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.HelmetStyleList)
            {
                playerHelmetStyleCombo.Items.Add(rec);
            }
            playerHelmetStyleCombo.Sorted = true;
            #endregion

            #region FaceMask
            playerFaceMaskCombo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.FacemaskList)
            {
                playerFaceMaskCombo.Items.Add(rec);
            }
            playerFaceMaskCombo.Sorted = true;
            #endregion

            #region Shoes
            playerLeftShoeCombo.Items.Clear();
            playerRightShoeCombo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.ShoeList)
            {
                playerLeftShoeCombo.Items.Add(rec);
                playerRightShoeCombo.Items.Add(rec);
            }
            #endregion

            #region Hands
            playerLeftHandCombo.Items.Clear();
            playerRightHandCombo.Items.Clear();
            T_LeftHand_Combo.Items.Clear();
            T_RightHand_Combo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.GloveList)
            {
                playerLeftHandCombo.Items.Add(rec);
                playerRightHandCombo.Items.Add(rec);
                T_LeftHand_Combo.Items.Add(rec);
                T_RightHand_Combo.Items.Add(rec);
            }
            playerLeftHandCombo.Sorted = true;
            playerRightHandCombo.Sorted = true;
            #endregion

            #region Wrist
            playerLeftWristCombo.Items.Clear();
            playerRightWristCombo.Items.Clear();
            T_LeftWrist_Combo.Items.Clear();
            T_RightWrist_Combo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.WristBandList)
            {
                playerLeftWristCombo.Items.Add(rec);
                playerRightWristCombo.Items.Add(rec);
                T_LeftWrist_Combo.Items.Add(rec);
                T_RightWrist_Combo.Items.Add(rec);
            }
            playerLeftWristCombo.Sorted = true;
            playerRightWristCombo.Sorted = true;
            #endregion

            #region Sleeves
            playerLeftSleeve.Items.Clear();
            playerRightSleeve.Items.Clear();
            T_Sleeves_Combo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.SleeveList)
            {
                playerLeftSleeve.Items.Add(rec);
                playerRightSleeve.Items.Add(rec);
                T_Sleeves_Combo.Items.Add(rec);
            }
            playerLeftSleeve.Sorted = true;
            playerRightSleeve.Sorted = true;
            #endregion

            #region Elbows
            playerLeftElbowCombo.Items.Clear();
            playerRightElbowCombo.Items.Clear();
            T_LeftElbow_Combo.Items.Clear();
            T_RightElbow_Combo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.ElbowList)
            {
                playerLeftElbowCombo.Items.Add(rec);
                playerRightElbowCombo.Items.Add(rec);
                T_LeftElbow_Combo.Items.Add(rec);
                T_RightElbow_Combo.Items.Add(rec);
            }
            playerLeftElbowCombo.Sorted = true;
            playerRightElbowCombo.Sorted = true;
            #endregion

            #region Ankles
            playerLeftAnkleCombo.Items.Clear();
            playerRightAnkleCombo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.AnkleList)
            {
                playerLeftAnkleCombo.Items.Add(rec);
                playerRightAnkleCombo.Items.Add(rec);
            }
            #endregion

            #region Visor
            playerVisorCombo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.VisorList)
            {
                playerVisorCombo.Items.Add(rec);
            }
            #endregion

            #region EyePaint / Face Marks
            playerEyePaintCombo.Items.Clear();
            foreach (GenericRecord rec in model.PlayerModel.FaceMarkList)
            {
                playerEyePaintCombo.Items.Add(rec);
            }
            playerEyePaintCombo.Sorted = true;
            #endregion

            #endregion

            if (model.MadVersion <= MaddenFileVersion.Ver2005)
            {
                playerEquipmentThighPads.Enabled = true;
            }
            if (model.MadVersion >= MaddenFileVersion.Ver2004 && model.MadVersion <= MaddenFileVersion.Ver2008)
            {
                LegacyPanel.Visible = true;

                #region Tattoos
                if (model.MadVersion == MaddenFileVersion.Ver2004)
                {
                    Tattoo_Left.Maximum = 31;
                    Tattoo_Right.Maximum = 31;
                }
                else if (model.MadVersion == MaddenFileVersion.Ver2005)
                {
                    Tattoo_Left.Maximum = 15;
                    Tattoo_Right.Maximum = 15;
                }
                else
                {
                    Tattoo_Left.Maximum = 63;
                    Tattoo_Right.Maximum = 63;
                }
                #endregion

                LeftSleeveLabel.Text = "Sleeves";
                RightSleeveLabel.Visible = false;
                playerRightSleeve.Visible = false;
            }
            if (model.MadVersion >= MaddenFileVersion.Ver2019)
            {
                FixAudioID_Button.Visible = true;
                NextGenPanel.Visible = true;
                Madden20HairStyle_Label.Visible = false;
                Madden20HairStyle.Visible = false;

                #region QB Style
                playerStance.Items.Clear();
                if (model.PlayerModel.QBStyleList != null)
                {
                    foreach (GenericRecord rec in model.PlayerModel.QBStyleList)
                        playerStance.Items.Add(rec);
                }
                #endregion

                #region EndPlay
                playerEndPlay.Items.Clear();
                if (model.PlayerModel.EndPlayList != null)
                {
                    foreach (GenericRecord rec in model.PlayerModel.EndPlayList)
                        playerEndPlay.Items.Add(rec);
                }
                #endregion

                #region Sideline Headgear
                SidelineHeadGear_Combo.Items.Clear();
                if (model.PlayerModel.SidelineHeadGear != null)
                {
                    foreach (GenericRecord rec in model.PlayerModel.SidelineHeadGear)
                        SidelineHeadGear_Combo.Items.Add(rec);
                }
                #endregion

                #region Knee
                playerLeftKneeCombo.Items.Clear();
                playerRightKneeCombo.Items.Clear();
                playerLeftKneeCombo.Items.Add("None");
                playerRightKneeCombo.Items.Add("None");
                if (model.MadVersion == MaddenFileVersion.Ver2019)
                {
                    playerLeftKneeCombo.Items.Add("Nike");
                    playerRightKneeCombo.Items.Add("Nike");
                    playerLeftKneeCombo.Items.Add("Regular");
                    playerRightKneeCombo.Items.Add("Regular");
                }
                else if (model.MadVersion == MaddenFileVersion.Ver2020)
                {
                    playerLeftKneeCombo.Items.Add("Large");
                    playerRightKneeCombo.Items.Add("Large");
                    playerLeftKneeCombo.Items.Add("Small");
                    playerRightKneeCombo.Items.Add("Small");
                }

                #endregion

                #region NeckRoll
                playerNeckRollCombo.Items.Clear();
                playerNeckRollCombo.Items.Add("None");
                playerNeckRollCombo.Items.Add("Vintage Neck Roll");
                playerNeckRollCombo.Items.Add("Butterfly");
                playerNeckRollCombo.Items.Add("Vintage Single");
                playerNeckRollCombo.Items.Add("Cowboy Collar");
                #endregion

                playerSockHeight.Enabled = true;

                if (model.MadVersion >= MaddenFileVersion.Ver2020)
                {
                    Madden20HairStyle_Label.Visible = false;
                    Madden20HairStyle.Visible = false;
                    Madden20HairStyle.Items.Clear();
                    Madden20HairStyle.Items.Add("None");
                    Madden20HairStyle.Items.Add("AfroCleanShort");
                    Madden20HairStyle.Items.Add("AfroMohawk");
                    Madden20HairStyle.Items.Add("AfroShort1");
                    Madden20HairStyle.Items.Add("AfroShort2");
                    Madden20HairStyle.Items.Add("AfroShortReceding");
                    Madden20HairStyle.Items.Add("AfroShortShavedSides");
                    Madden20HairStyle.Items.Add("ShortStraight");
                    Madden20HairStyle.Items.Add("Cornrows");
                    Madden20HairStyle.Items.Add("DreadsLong");
                    Madden20HairStyle.Items.Add("DreadsMessy");
                    Madden20HairStyle.Items.Add("DreadsPonytail");
                    Madden20HairStyle.Items.Add("LongCurly");
                    Madden20HairStyle.Items.Add("LongStraight");
                    Madden20HairStyle.Items.Add("LongWavy1");
                    Madden20HairStyle.Items.Add("LongWavy2");
                    Madden20HairStyle.Items.Add("MediumCurlyMullet");
                    Madden20HairStyle.Items.Add("MediumSidewardsSpikey");
                    Madden20HairStyle.Items.Add("ShavedBaldingTop");
                    Madden20HairStyle.Items.Add("ShavedCurly");
                    Madden20HairStyle.Items.Add("ShavedDesign");
                    Madden20HairStyle.Items.Add("ShavedStraight");
                    Madden20HairStyle.Items.Add("ShortCurly");
                    Madden20HairStyle.Items.Add("ShortMessyWet");
                    Madden20HairStyle.Items.Add("ShortMohawk");
                    Madden20HairStyle.Items.Add("ShortMohawkShaved");
                    Madden20HairStyle.Items.Add("ShortRecedingMessy");
                    Madden20HairStyle.Items.Add("ShortSidewardsShaved");
                    Madden20HairStyle.Items.Add("ShortSpikeyClean");
                    Madden20HairStyle.Items.Add("ShortSlickedShaved");
                    Madden20HairStyle.Items.Add("ShortSpikeyShavedSides");
                    Madden20HairStyle.Items.Add("ShortWavy");
                }
            }

            #endregion

            #region Contract Tab
            PlayerContractDetails_Panel.Enabled = false;
            MiscSalary_Panel.Enabled = false;

            #region Player Contract Panel
            UseLeagueMinimum_Checkbox.Enabled = false;      //view at some point
            LeagueMinimum.Enabled = false;
            CurrentYear.Enabled = true;
            CurrentYear.Value = 2023;   //11-12-23 prime
            #endregion

            #region Player Contract Details Panel
            PlayerCapHit.Enabled = false;

            PlayerBonus0.Enabled = false;
            PlayerBonus1.Enabled = false;
            PlayerBonus2.Enabled = false;
            PlayerBonus3.Enabled = false;
            PlayerBonus4.Enabled = false;
            PlayerBonus5.Enabled = false;
            PlayerBonus6.Enabled = false;
            PlayerSalary0.Enabled = false;
            PlayerSalary1.Enabled = false;
            PlayerSalary2.Enabled = false;
            PlayerSalary3.Enabled = false;
            PlayerSalary4.Enabled = false;
            PlayerSalary5.Enabled = false;
            PlayerSalary6.Enabled = false;
            ContractYearlyIncrease.Enabled = false;

            #endregion

            #region Misc Salary Panel            
            if (model.LeagueCap.ContainsKey(model.CurrentYear))
                UseActualNFLSalaryCap_Checkbox.Enabled = true;
            else UseActualNFLSalaryCap_Checkbox.Enabled = false;

            CalcTeamSalary_Checkbox.Enabled = true;
            CalcTeamSalary_Checkbox.Checked = true;
            SalaryCap.Enabled = false;
            TeamSalary.Enabled = false;
            CalcTeamSalary.Text = "NA";
            TeamCapRoom.Text = "NA";
            Penalty0_Label.Visible = false;
            Penalty0.Visible = false;
            Penalty1_Label.Visible = false;
            Penalty1.Visible = false;
            SalaryRankCombo.Enabled = true;
            SalaryRankCombo.Items.Add("NFL");
            SalaryRankCombo.Items.Add("Conf");
            SalaryRankCombo.Items.Add("Div");
            SalaryRankCombo.SelectedIndex = -1;
            #endregion

            if (model.FileType == MaddenFileType.Franchise || model.MadVersion >= MaddenFileVersion.Ver2019)
            {
                PlayerContractDetails_Panel.Enabled = true;

                PlayerBonus0.Enabled = true;
                PlayerBonus1.Enabled = true;
                PlayerBonus2.Enabled = true;
                PlayerBonus3.Enabled = true;
                PlayerBonus4.Enabled = true;
                PlayerBonus5.Enabled = true;
                PlayerBonus6.Enabled = true;
                PlayerSalary0.Enabled = true;
                PlayerSalary1.Enabled = true;
                PlayerSalary2.Enabled = true;
                PlayerSalary3.Enabled = true;
                PlayerSalary4.Enabled = true;
                PlayerSalary5.Enabled = true;
                PlayerSalary6.Enabled = true;
                ContractYearlyIncrease.Enabled = true;


                if (!Madden19)
                {
                    SalaryCap.Enabled = true;
                    Penalty0_Label.Visible = true;
                    Penalty0.Visible = true;
                    Penalty1_Label.Visible = true;
                    Penalty1.Visible = true;
                }
            }

            #endregion

            #region Player Comments


            #endregion

            InitPlayerList();

            ResumeLayout();
            isInitialising = false;
        }

        public void CleanUI()   //filter clean up
        {
            CollegeCombo.Items.Clear();
            Team_Combo.Items.Clear();
            filterPositionComboBox.Items.Clear();
            FilterCollegecomboBox.Items.Clear();    //New
            FilterDevTraitcomboBox.Items.Clear();   //New
            FilterArchetypecomboBox.Items.Clear();  //new
            FilterYearsProcomboBox.Items.Clear();   //new
            OriginalPosition_Combo.Items.Clear();
            positionComboBox.Items.Clear();
            filterTeamComboBox.Items.Clear();
            PlayerRolecomboBox.Items.Clear();
        }

        public EditorModel Model
        {
            set
            {
                model = value;
                if (model.MadVersion == MaddenFileVersion.Ver2004)
                    baseyear = 2003;
                else if (model.MadVersion == MaddenFileVersion.Ver2005)
                    baseyear = 2004;
                else if (model.MadVersion == MaddenFileVersion.Ver2006)
                    baseyear = 2005;
                else if (model.MadVersion == MaddenFileVersion.Ver2007)
                    baseyear = 2006;
                else if (model.MadVersion == MaddenFileVersion.Ver2008)
                    baseyear = 2007;
                else if (model.MadVersion == MaddenFileVersion.Ver2019)
                    baseyear = 2018;
                else if (model.MadVersion == MaddenFileVersion.Ver2020) //edited
                    baseyear = 2023;

                if (model.MadVersion >= MaddenFileVersion.Ver2019 && model.FileType == MaddenFileType.Roster || model.FileType == MaddenFileType.DBTeam)
                    Madden19 = true;
            }
        }

        #endregion


        #region Misc Functions

        public void InitCollegeList()
        {
            CollegeCombo.Items.Clear();
            foreach (KeyValuePair<int, college_entry> col in model.Colleges)
            {
                string name = col.Value.name;
                CollegeCombo.Items.Add(name);
            }
            CollegeCombo.Sorted = true;
            CollegeCombo.Update();
        }

        public void InitPlayerList()
        {
            isInitialising = true;

            CurrentPlayers.Clear();
            CurrentPlayers = model.PlayerModel.GetPlayerList();

            PlayerGridView.Rows.Clear();
            PlayerGridView.Refresh();
            PlayerGridView.MultiSelect = false;
            PlayerGridView.RowHeadersVisible = false;
            PlayerGridView.AutoGenerateColumns = false;
            PlayerGridView.AllowUserToAddRows = false;
            PlayerGridView.ColumnCount = 2;
            PlayerGridView.Columns[0].Name = "ID";
            PlayerGridView.Columns[0].Width = 35;
            PlayerGridView.Columns[1].Name = "Player";
            PlayerGridView.Columns[1].Width = 100;
            foreach (KeyValuePair<int, string> player in model.PlayerModel.playernames)
            {
                object[] o = { (int)player.Key, (string)player.Value };
                PlayerGridView.Rows.Add(o);
            }

            int playerCount = model.PlayerModel.GetActivePlayerCount(); // Get the player count after applying filters
            MessageBox.Show($"Number of Players: {playerCount}", "Player Count", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (model.PlayerModel.playernames.Count > 0)
            {
                PlayerGridView.Rows[0].Selected = true;
                int idnum = (int)PlayerGridView.Rows[0].Cells[0].Value;
                LoadPlayerInfo(model.PlayerModel.GetPlayerByPlayerId(idnum));
            }
            else
            {
                LoadPlayerInfo(null);
            }
            //MessageBox.Show("No Records available.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            isInitialising = false;
        }



        private string DecodeTendancy(MaddenPositions pos, int type)
        {
            if (type == 2)
            {
                return "Balanced";
            }

            switch (pos)
            {
                case MaddenPositions.QB:
                    return (type == 0 ? "Pocket" : "Scrambling");
                case MaddenPositions.HB:
                    return (type == 0 ? "Power" : "Speed");
                case MaddenPositions.FB:
                case MaddenPositions.TE:
                    return (type == 0 ? "Blocking" : "Receiving");
                case MaddenPositions.WR:
                    return (type == 0 ? "Possession" : "Speed");
                case MaddenPositions.LT:
                case MaddenPositions.LG:
                case MaddenPositions.C:
                case MaddenPositions.RG:
                case MaddenPositions.RT:
                    return (type == 0 ? "Run Blocking" : "Pass Blocking");
                case MaddenPositions.LE:
                case MaddenPositions.RE:
                case MaddenPositions.DT:
                    {
                        if (model.MadVersion >= MaddenFileVersion.Ver2019)
                            return (type == 0 ? "Cover LB" : "Pass Rush");
                        else return (type == 0 ? "Pass Rushing" : "Run Stopping");
                    }
                case MaddenPositions.LOLB:
                case MaddenPositions.MLB:
                case MaddenPositions.ROLB:
                    return (type == 0 ? "Coverage" : "Run Stopping");
                case MaddenPositions.CB:
                case MaddenPositions.SS:
                case MaddenPositions.FS:
                    return (type == 0 ? "Coverage" : "Hard Hitting");
                case MaddenPositions.K:
                case MaddenPositions.P:
                    return (type == 0 ? "Power" : "Accurate");
            }

            return "";
        }

        public void FixCareerStats(PlayerRecord player)
        {
            int baseyear = 2003;
            if (model.MadVersion == MaddenFileVersion.Ver2005)
                baseyear = 2004;
            if (model.MadVersion == MaddenFileVersion.Ver2006)
                baseyear = 2005;
            if (model.MadVersion == MaddenFileVersion.Ver2007)
                baseyear = 2006;
            if (model.MadVersion == MaddenFileVersion.Ver2008)
                baseyear = 2007;

            //  offense
            int totalpassatt = 0;
            int totalpasscomp = 0;


            for (int count = 0; count < player.YearsPro; count++)
            {
                if ((string)statsyear.Items[count] == "Career")
                    continue;
                else
                {
                    int year = (int)statsyear.Items[0] - baseyear;

                    SeasonStatsOffenseRecord off = model.PlayerModel.GetOffStats(player.PlayerId, year);
                    if (off == null)
                        continue;
                    totalpassatt += off.SeaPassAtt;
                    totalpasscomp += off.SeaComp;
                }
            }
        }

        public void DisplayPlayerPort()
        {
            if (PlayerPortBox.Image != null)
                PlayerPortBox.Image = null;
            PlayerPortBox.BackColor = Color.White;
            PlayerPortBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (!manager.PlayerPortDAT.isterf)
            {
                return;
            }

            int portid = model.PlayerModel.CurrentPlayerRecord.PortraitId + 1;

            if (manager.PlayerPortDAT.ParentTerf.files >= portid + 1)
            {
                if (manager.PlayerPortDAT.ParentTerf.Data.DataFiles[portid].filetype == "MMAP")
                    PlayerPortBox.Image = manager.PlayerPortDAT.ParentTerf.Data.DataFiles[portid].mmap_data.GetPortraitDisplay();
                else if (manager.PlayerPortDAT.ParentTerf.Data.DataFiles[portid].filetype == "COMP") PlayerPortBox.BackColor = Color.Green;
                else PlayerPortBox.BackColor = Color.Red;
                return;
            }

            PlayerPortBox.BackColor = Color.Black;
        }

        public bool CheckPlayerUniqueness(int pgid, int poid)
        {
            isInitialising = true;
            bool exists = model.PlayerModel.CheckIDExists(pgid, poid);
            if (exists)
            {
                MessageBox.Show("IDs need to be unique", "IDs already exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadPlayerInfo(model.PlayerModel.CurrentPlayerRecord);
            }
            isInitialising = false;
            return exists;
        }

        private void LoadFreeAgents(PlayerRecord record)
        {
            FreeAgents.DataBindings.Clear();
            FreeAgents.RowHeadersVisible = false;
            if (FreeAgents.Columns.Count == 0)
            {
                FreeAgents.Columns.Add("Name", "Name");
                FreeAgents.Columns[0].Width = 80;
                FreeAgents.Columns.Add("OVR", "Ovr");
                FreeAgents.Columns[1].Width = 32;
                FreeAgents.Columns.Add("Age", "Age");
                FreeAgents.Columns[2].Width = 32;
            }
            List<PlayerRecord> fa = new List<PlayerRecord>();
            foreach (PlayerRecord rec in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                PlayerRecord newplayer = (PlayerRecord)rec;
                if (record.PositionId == newplayer.PositionId && newplayer.TeamId == 1009)
                    fa.Add(newplayer);
            }

            fa.Sort(delegate (PlayerRecord x, PlayerRecord y)
            { return ((decimal)y.Overall).CompareTo(x.Overall); });

            for (int c = 0; c < fa.Count; c++)
            {
                FreeAgents.Rows.Add();
                FreeAgents.Rows[c].Cells[0].Value = fa[c].FirstName[0] + "." + fa[c].LastName;
                FreeAgents.Rows[c].Cells[1].Value = fa[c].Overall;
                FreeAgents.Rows[c].Cells[2].Value = fa[c].Age;
                FreeAgents.Rows[c].ReadOnly = true;
            }
        }

        private void LoadPositionSalaries(PlayerRecord record)
        {
            PositionSalary.DataBindings.Clear();
            PositionSalary.RowHeadersVisible = false;
            if (PositionSalary.Columns.Count == 0 || PositionSalary.Columns.Count == 5 && IncludeOverall.Checked || PositionSalary.Columns.Count == 6 && !IncludeOverall.Checked)
            {
                PositionSalary.Columns.Clear();
                PositionSalary.Columns.Add("Name", "Name");
                PositionSalary.Columns[PositionSalary.ColumnCount - 1].Width = 80;
                PositionSalary.Columns.Add("OVR", "OVR");
                PositionSalary.Columns[PositionSalary.ColumnCount - 1].Width = 40;
                PositionSalary.Columns.Add("Cap", "CAP");
                PositionSalary.Columns[PositionSalary.ColumnCount - 1].Width = 40;
                PositionSalary.Columns.Add("Total", "Total");
                PositionSalary.Columns[PositionSalary.ColumnCount - 1].Width = 40;
                PositionSalary.Columns.Add("Yrs", "Yrs");
                PositionSalary.Columns[PositionSalary.ColumnCount - 1].Width = 40;
                PositionSalary.Columns.Add("Avg", "Avg");
                PositionSalary.Columns[PositionSalary.ColumnCount - 1].Width = 40;
            }

            List<PlayerRecord> top = new List<PlayerRecord>();
            foreach (PlayerRecord rec in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                PlayerRecord newplayer = (PlayerRecord)rec;
                if (record.PositionId == newplayer.PositionId && record.TeamId != 1009 && record.TeamId != 1010 && record.TeamId != 1014
                    && record.TeamId != 1015 && record.TeamId != 1023 && newplayer.ContractYearsLeft != 0)
                {
                    if (model.MadVersion < MaddenFileVersion.Ver2019 && model.FileType == MaddenFileType.Roster)
                    {
                        if (newplayer.YearlySalary == null)
                            newplayer.SetContract(false, false, 30);
                    }

                    top.Add(newplayer);
                }
            }
            List<int> top10 = new List<int>();
            int current = 0;
            top.Sort(delegate (PlayerRecord x, PlayerRecord y)
            { return ((decimal)y.GetCurrentSalary()).CompareTo(x.GetCurrentSalary()); });
            for (int c = 0; c < top.Count; c++)
            {
                top10.Add((int)top[c].GetCurrentSalary());
                current += (int)top[c].GetCurrentSalary();
                if (c == 4)
                    Top5.Value = (decimal)current / 500;
                if (c == 9)
                    Top10.Value = current / 1000;
            }

            LeagueAVG.Value = current / (100 * top10.Count);

            for (int c = 0; c < top.Count; c++)
            {
                PositionSalary.Rows.Add();
                PositionSalary.Rows[c].Cells[0].Value = top[c].FirstName[0] + "." + top[c].LastName;
                PositionSalary.Rows[c].Cells[1].Value = top[c].Overall;
                PositionSalary.Rows[c].Cells[PositionSalary.Rows[c].Cells.Count - 4].Value = (decimal)top[c].GetCurrentSalary() / 100;
                PositionSalary.Rows[c].Cells[PositionSalary.Rows[c].Cells.Count - 3].Value = (decimal)top[c].TotalSalary / 100;
                PositionSalary.Rows[c].Cells[PositionSalary.Rows[c].Cells.Count - 2].Value = top[c].ContractLength;
                PositionSalary.Rows[c].Cells[PositionSalary.Rows[c].Cells.Count - 1].Value = Math.Round((decimal)(top[c].TotalSalary / top[c].ContractLength) / 100, 2);
                PositionSalary.Rows[c].ReadOnly = true;
            }

            // sort list highest avg salary per year
            top.Sort(delegate (PlayerRecord x, PlayerRecord y)
            { return ((decimal)y.TotalSalary / y.ContractLength).CompareTo((decimal)x.TotalSalary / x.ContractLength); });
            List<decimal> topdec = new List<decimal>();
            decimal currentdec = 0;
            for (int c = 0; c < top.Count; c++)
            {
                topdec.Add((decimal)top[c].TotalSalary / top[c].ContractLength);
                currentdec += topdec[c];
                if (c == 4)
                    Top5AVG.Value = currentdec / 500;
                if (c == 9)
                    Top10AVG.Value = currentdec / 1000;
            }
            LeagueContAVG.Value = currentdec / (top.Count * 100);

            if (model.FileType == MaddenFileType.Franchise)
            {
                foreach (SalaryYearsPro pm in model.TableModels[EditorModel.PLAYER_MINIMUM_SALARY_TABLE].GetRecords())
                {
                    SalaryYearsPro min = (SalaryYearsPro)pm;
                    if (min.YearsPro == record.YearsPro)
                        LeagueMinimum.Value = (decimal)min.MinimumSalary / 1000000;
                }
            }
        }

        private void TeamNeeds(PlayerRecord record)
        {
            List<int> roster = new List<int>();
            for (int c = 0; c < 21; c++)
                roster.Add(0);
            int neededplayers = 0;
            int maxplayers = 55;
            if (model.MadVersion >= MaddenFileVersion.Ver2019)
                maxplayers = 75;

            foreach (PlayerRecord rec in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                PlayerRecord r = (PlayerRecord)rec;
                if (r.TeamId == record.TeamId && r.ContractYearsLeft != 0)
                    roster[r.PositionId]++;
            }
            // QB
            ReqQB.Text = (Math.Abs(2 - roster[0])).ToString();
            if (roster[0] < 2)
            {
                ReqQB.BackColor = Color.PaleVioletRed;
                neededplayers += 2 - roster[0];
            }
            else if (roster[0] > 2)
                ReqQB.BackColor = Color.LightGreen;
            else ReqQB.BackColor = SystemColors.Window;
            ReqHB.Text = (Math.Abs(3 - roster[1])).ToString();
            if (roster[1] < 3)
            {
                ReqHB.BackColor = Color.PaleVioletRed;
                neededplayers += 3 - roster[1];
            }
            else if (roster[1] > 3)
                ReqHB.BackColor = Color.LightGreen;
            else ReqHB.BackColor = SystemColors.Window;
            ReqFB.Text = (Math.Abs(1 - roster[2])).ToString();
            if (roster[2] < 1)
            {
                ReqFB.BackColor = Color.PaleVioletRed;
                neededplayers++;
            }
            else if (roster[2] > 1)
                ReqFB.BackColor = Color.LightGreen;
            else ReqFB.BackColor = SystemColors.Window;
            ReqWR.Text = (Math.Abs(5 - roster[3])).ToString();
            if (roster[3] < 5)
            {
                ReqWR.BackColor = Color.PaleVioletRed;
                neededplayers += 5 - roster[3];
            }
            else if (roster[3] > 5)
                ReqWR.BackColor = Color.LightGreen;
            else ReqWR.BackColor = SystemColors.Window;
            ReqTE.Text = (Math.Abs(3 - roster[4])).ToString();
            if (roster[4] < 3)
            {
                ReqTE.BackColor = Color.PaleVioletRed;
                neededplayers += 3 - roster[4];
            }
            else if (roster[4] > 3)
                ReqTE.BackColor = Color.LightGreen;
            else ReqTE.BackColor = SystemColors.Window;
            ReqP.Text = (Math.Abs(1 - roster[20])).ToString();
            if (roster[20] < 1)
            {
                ReqP.BackColor = Color.PaleVioletRed;
                neededplayers++;
            }
            else if (roster[20] > 1)
                ReqP.BackColor = Color.LightGreen;
            else ReqP.BackColor = SystemColors.Window;
            ReqT.Text = (Math.Abs(4 - (roster[5] + roster[9]))).ToString();
            if (roster[5] + roster[9] < 4)
            {
                ReqT.BackColor = Color.PaleVioletRed;
                neededplayers += 4 - (roster[5] + roster[9]);
            }
            else if (roster[5] + roster[9] > 4)
                ReqT.BackColor = Color.LightGreen;
            else ReqT.BackColor = SystemColors.Window;
            ReqG.Text = (Math.Abs(4 - (roster[6] + roster[8]))).ToString();
            if (roster[6] + roster[8] < 4)
            {
                ReqG.BackColor = Color.PaleVioletRed;
                neededplayers += 4 - (roster[6] + roster[8]);
            }
            else if (roster[6] + roster[8] > 4)
                ReqG.BackColor = Color.LightGreen;
            else ReqG.BackColor = SystemColors.Window;
            ReqC.Text = (Math.Abs(2 - roster[7])).ToString();
            if (roster[7] < 2)
            {
                ReqC.BackColor = Color.PaleVioletRed;
                neededplayers += 2 - roster[7];
            }
            else if (roster[7] > 2)
                ReqC.BackColor = Color.LightGreen;
            else ReqC.BackColor = SystemColors.Window;
            ReqDE.Text = (Math.Abs(4 - (roster[10] + roster[11]))).ToString();
            if (roster[10] + roster[11] < 4)
            {
                ReqDE.BackColor = Color.PaleVioletRed;
                neededplayers += 4 - (roster[10] + roster[11]);
            }
            else if (roster[10] + roster[11] > 4)
                ReqDE.BackColor = Color.LightGreen;
            else ReqDE.BackColor = SystemColors.Window;
            ReqDT.Text = (Math.Abs(3 - roster[12])).ToString();
            if (roster[12] < 3)
            {
                ReqDT.BackColor = Color.PaleVioletRed;
                neededplayers += 3 - roster[12];
            }
            else if (roster[12] > 3)
                ReqDT.BackColor = Color.LightGreen;
            else ReqDT.BackColor = SystemColors.Window;
            ReqK.Text = (Math.Abs(1 - roster[19])).ToString();
            if (roster[19] < 1)
            {
                ReqK.BackColor = Color.PaleVioletRed;
                neededplayers++;
            }
            else if (roster[19] > 1)
                ReqK.BackColor = Color.LightGreen;
            else ReqK.BackColor = SystemColors.Window;
            ReqOLB.Text = (Math.Abs(4 - (roster[13] + roster[15]))).ToString();
            if (roster[13] + roster[15] < 4)
            {
                ReqOLB.BackColor = Color.PaleVioletRed;
                neededplayers += 4 - (roster[13] + roster[15]);
            }
            else if (roster[13] + roster[15] > 4)
                ReqOLB.BackColor = Color.LightGreen;
            else ReqOLB.BackColor = SystemColors.Window;
            ReqMLB.Text = (Math.Abs(2 - roster[14])).ToString();
            if (roster[14] < 2)
            {
                ReqMLB.BackColor = Color.PaleVioletRed;
                neededplayers += 2 - roster[14];
            }
            else if (roster[14] > 2)
                ReqMLB.BackColor = Color.LightGreen;
            else ReqMLB.BackColor = SystemColors.Window;
            ReqCB.Text = (Math.Abs(5 - roster[16])).ToString();
            if (roster[16] < 5)
            {
                ReqCB.BackColor = Color.PaleVioletRed;
                neededplayers += 5 - roster[16];
            }
            else if (roster[16] > 5)
                ReqCB.BackColor = Color.LightGreen;
            else ReqCB.BackColor = SystemColors.Window;
            ReqFS.Text = (Math.Abs(2 - roster[17])).ToString();
            if (roster[17] < 2)
            {
                ReqFS.BackColor = Color.PaleVioletRed;
                neededplayers += 2 - roster[17];
            }
            else if (roster[17] > 2)
                ReqFS.BackColor = Color.LightGreen;
            else ReqFS.BackColor = SystemColors.Window;
            ReqSS.Text = (Math.Abs(2 - roster[18])).ToString();
            if (roster[18] < 2)
            {
                ReqSS.BackColor = Color.PaleVioletRed;
                neededplayers += 2 - roster[18];
            }
            else if (roster[18] > 2)
                ReqSS.BackColor = Color.LightGreen;
            else ReqSS.BackColor = SystemColors.Window;

            int totalplayers = 0;
            foreach (int x in roster)
                totalplayers += x;
            if (totalplayers < 53)
            {
                NeededPlayers_Label.Text = "*Need to Sign*";
                NeededPlayers.Value = Math.Abs(53 - totalplayers);
                NeededPlayers.BackColor = Color.PaleVioletRed;
                NeededPlayers.Visible = true; // Hide the control

            }
            else if (totalplayers > maxplayers)
            {
                NeededPlayers_Label.Text = "*Need to Release*";
                NeededPlayers.Value = Math.Abs(totalplayers - maxplayers);
                NeededPlayers.BackColor = Color.PaleVioletRed;
                NeededPlayers.Visible = true; // Hide the control
            }
            else
            {
                if (neededplayers > 0)
                {
                    NeededPlayers_Label.Text = "*Check Positions*";
                    NeededPlayers.Value = neededplayers;
                    NeededPlayers.BackColor = Color.PaleVioletRed;
                    NeededPlayers.Visible = true; // Hide the control
                }
                else
                {
                    NeededPlayers_Label.Text = " ";
                    NeededPlayers.Value = 0;
                    NeededPlayers.BackColor = Color.Green;
                    NeededPlayers.Visible = false; // Hide the control
                }
            }

        }

        #endregion

        public void LoadPlayerInfo(PlayerRecord record)
        {
            isInitialising = true;

            if (record == null)
            {
                MessageBox.Show("No Records available.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                filterTeamComboBox.SelectedIndex = 0;
                filterPositionComboBox.SelectedIndex = 0;
                FilterCollegecomboBox.SelectedIndex = 0;    //New
                FilterDevTraitcomboBox.SelectedIndex = 0;   //new
                FilterArchetypecomboBox.SelectedIndex = 0;  //new
                FilterYearsProcomboBox.SelectedIndex = 0;   //new
                model.PlayerModel.filterteam = -1;
                model.PlayerModel.filterposition = -1;
                model.PlayerModel.filtercollege = -1;   //new
                model.PlayerModel.filterdevtrait = -1;  //new
                model.PlayerModel.filterarchetypes = -1;    //new
                model.PlayerModel.filterYearsPro = -1;  //new
                InitPlayerList();
                isInitialising = false;
                deletePlayerButton.Enabled = true;
                return;
            }

            SuspendLayout();
            model.PlayerModel.CurrentPlayerRecord = record;
            deletePlayerButton.Enabled = true;

            try
            {
                TeamRecord team = model.TeamModel.GetTeamRecord(record.TeamId);

                #region Player Info

                firstNameTextBox.Text = record.FirstName;
                lastNameTextBox.Text = record.LastName;
                PlayerID_Updown.Value = record.PlayerId;
                NFL_Updown.Value = record.NFLID;
                playerPortraitId.Value = record.PortraitId;
                playerComment.Value = record.PlayerComment;
                playerAge.Value = record.Age;
                playerYearsPro.Value = record.YearsPro;
                playerDraftRound.Value = record.DraftRound;
                playerDraftRoundIndex.Value = record.DraftRoundIndex;
                CareerPhase_Combo.SelectedIndex = record.CareerPhase;

                if (record.Height < 12)
                    PlayerHeight_Feet.SelectedIndex = -1;
                else PlayerHeight_Feet.SelectedIndex = record.Height / 12;
                PlayerHeight_Inches.SelectedIndex = record.Height - (PlayerHeight_Feet.SelectedIndex * 12);

                playerWeight.Value = record.Weight + 160;

                if (record.JerseyNumber > 99)
                {
                    playerJerseyNumber.Enabled = false;
                }
                else
                {
                    playerJerseyNumber.Enabled = true;
                    playerJerseyNumber.Value = record.JerseyNumber;
                }

                playerHairColorCombo.SelectedIndex = record.HairColor;
                CollegeCombo.Text = model.Colleges[model.PlayerModel.CurrentPlayerRecord.CollegeId].name;
                if (model.FileType != MaddenFileType.Roster && record.TeamId == 1014)
                    Team_Combo.Text = "Retired";
                else Team_Combo.SelectedItem = (object)team;

                positionComboBox.Text = positionComboBox.Items[record.PositionId].ToString();
                playerThrowingStyle.SelectedIndex = Convert.ToInt32(record.ThrowStyle);

                playerDominantHand.Checked = record.DominantHand;
                playerProBowl.Checked = record.ProBowl;
                #endregion

                #region Legacy Ratings
                Overall.Value = record.Overall;
                playerSpeed.Value = record.Speed;
                playerStrength.Value = record.Strength;
                playerAwareness.Value = record.Awareness;
                playerAgility.Value = record.Agility;
                playerAcceleration.Value = record.Acceleration;
                playerCatching.Value = record.Catching;
                playerCarrying.Value = record.Carrying;
                playerJumping.Value = record.Jumping;
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    playerBreakTackle.Value = record.BreakTackle19;
                else playerBreakTackle.Value = record.BreakTackle;
                playerTackle.Value = record.Tackle;
                playerImportance.Value = record.Importance;
                playerThrowPower.Value = record.ThrowPower;
                playerThrowAccuracy.Value = record.ThrowAccuracy;
                playerPassBlocking.Value = record.PassBlocking;
                playerRunBlocking.Value = record.RunBlocking;
                playerKickPower.Value = record.KickPower;
                playerKickAccuracy.Value = record.KickAccuracy;
                playerKickReturn.Value = record.KickReturn;
                playerStamina.Value = record.Stamina;
                playerInjury.Value = record.Injury;
                playerToughness.Value = record.Toughness;
                #endregion

                #region Injury
                InjuryRecord injury = model.PlayerModel.GetPlayersInjuryRecord(record.PlayerId);
                if (injury == null)
                {
                    playerInjuryCombo.Enabled = false;
                    playerInjuryCombo.Text = "";
                    playerInjuryLength.Enabled = false;
                    playerInjuryLength.Value = 0;
                    playerRemoveInjuryButton.Enabled = false;
                    playerInjuryReserve.Enabled = false;
                    playerAddInjuryButton.Enabled = true;
                    injuryLengthDescriptionTextBox.Enabled = false;
                    injuryLengthDescriptionTextBox.Text = "";

                    if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    {
                        playerInjurySevere.Value = 0;
                        playerInjurySevere.Enabled = false;
                        playerInjuryReturn.Value = 0;
                        playerInjuryReturn.Enabled = false;
                    }
                }
                else
                {
                    playerInjuryCombo.Enabled = true;
                    playerInjuryLength.Enabled = true;
                    playerRemoveInjuryButton.Enabled = true;
                    playerInjuryReserve.Enabled = true;
                    playerAddInjuryButton.Enabled = false;
                    playerInjuryLength.Value = injury.InjuryLength;
                    playerInjuryReserve.Checked = injury.IR;
                    injuryLengthDescriptionTextBox.Text = injury.LengthDescription;

                    if (model.MadVersion < MaddenFileVersion.Ver2019)
                    {
                        playerInjuryCombo.Text = playerInjuryCombo.Items[injury.InjuryType].ToString();
                    }
                    else
                    {
                        playerInjuryCombo.Text = model.PlayerModel.GetInjury(injury.InjuryType);
                        playerInjurySevere.Value = injury.InjurySeverity;
                        playerInjurySevere.Enabled = true;
                        playerInjuryReturn.Value = injury.InjuryReturn; ;
                        playerInjuryReturn.Enabled = true;
                    }
                }
                #endregion

                #region Appearance / Equipment
                playerHelmetStyleCombo.Text = model.PlayerModel.GetHelmet(record.Helmet);
                playerFaceMaskCombo.Text = model.PlayerModel.GetFaceMask(record.FaceMask);
                playerLeftShoeCombo.Text = model.PlayerModel.GetShoe(record.LeftShoe);
                playerRightShoeCombo.Text = model.PlayerModel.GetShoe(record.RightShoe);
                playerLeftHandCombo.Text = model.PlayerModel.GetGloves(record.LeftHand);
                playerRightHandCombo.Text = model.PlayerModel.GetGloves(record.RightHand);
                playerLeftWristCombo.Text = model.PlayerModel.GetWrist(record.LeftWrist);
                playerRightWristCombo.Text = model.PlayerModel.GetWrist(record.RightWrist);
                playerLeftSleeve.Text = model.PlayerModel.GetSleeve(record.SleevesLeft);
                playerRightSleeve.Text = model.PlayerModel.GetSleeve(record.SleevesRight);
                playerLeftElbowCombo.Text = model.PlayerModel.GetElbow(record.LeftElbow);
                playerRightElbowCombo.Text = model.PlayerModel.GetElbow(record.RightElbow);
                playerLeftKneeCombo.SelectedIndex = record.KneeLeft;
                playerRightKneeCombo.SelectedIndex = record.KneeRight;
                playerLeftAnkleCombo.Text = model.PlayerModel.GetAnkle(record.AnkleLeft);
                playerRightAnkleCombo.Text = model.PlayerModel.GetAnkle(record.AnkleRight);
                playerNeckRollCombo.Text = playerNeckRollCombo.Items[record.NeckRoll].ToString();
                playerVisorCombo.Text = playerVisorCombo.Items[record.Visor].ToString();
                playerEyePaintCombo.Text = model.PlayerModel.GetFaceMark(record.EyePaint);
                playerMouthPieceCombo.Text = playerMouthPieceCombo.Items[record.MouthPiece].ToString();
                playerJerseySleeves.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.JerseySleeve;
                playerLeftThighCombo.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.ThighLeft;
                playerRightThighCombo.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.ThighRight;
                HandWarmerscomboBox.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.HandWarmer;
                TowelscomboBox.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.PlayerTowel;
                #endregion


                #region Contracts

                // Fix errors introduced by NZA and dbeditor
                if (record.ContractLength == 0)
                    record.ClearContract();
                else if (record.ContractYearsLeft > record.ContractLength)
                    record.ContractYearsLeft = record.ContractLength;

                playerContractLength.Value = record.ContractLength;
                playerContractYearsLeft.Value = record.ContractYearsLeft;

                #endregion

                #region Version Specific Checks
                if (model.FileType == MaddenFileType.Franchise)
                {
                    #region Player Inactive
                    if (model.MadVersion > MaddenFileVersion.Ver2004)
                    {
                        PlayerInactive.Checked = false;

                        foreach (InactiveRecord ia in model.TableModels[EditorModel.INACTIVE_TABLE].GetRecords())
                        {
                            //InactiveRecord iar = (InactiveRecord)ia;
                            if (ia.Deleted)
                                continue;
                            if (ia.PlayerID == record.PlayerId)
                            {
                                PlayerInactive.Checked = true;
                                break;
                            }
                        }
                    }
                    #endregion
                    InitStatsYear(record);
                    LoadPlayerStats(record);
                }

                #region 2004-2005
                if (model.MadVersion <= MaddenFileVersion.Ver2005)
                {
                    playerEgo.Maximum = 127;
                    playerEgo.Value = record.Pcel;
                    playerEquipmentThighPads.Value = record.LegsThighPads;
                }
                #endregion

                #region 2005-2008
                if (model.MadVersion >= MaddenFileVersion.Ver2005)
                {
                    playerNFLIcon.Checked = record.NFLIcon;
                    PlayerHoldOut.Checked = record.Holdout;

                    if (model.MadVersion < MaddenFileVersion.Ver2019)
                    {
                        //Load the player tendancy and reinitialise the combo
                        PlayerTendency.Enabled = true;
                        PlayerTendency.Items.Clear();
                        for (int i = 0; i < 3; i++)
                        {
                            PlayerTendency.Items.Add(DecodeTendancy((MaddenPositions)record.PositionId, i));
                        }
                        PlayerTendency.SelectedIndex = record.Tendency;

                        playerMorale.Visible = true;
                        MoraleLabel.Visible = true;
                        playerMorale.Value = record.Morale;
                    }
                }
                #endregion

                #region 2006-2008
                if (model.MadVersion >= MaddenFileVersion.Ver2006)
                {
                    playerEgo.Value = record.Ego;
                }
                #endregion

                #region 2007-2008 only
                if (model.MadVersion >= MaddenFileVersion.Ver2007 && model.MadVersion <= MaddenFileVersion.Ver2008)
                {
                    RoleLabel.Visible = true;
                    PlayerRolecomboBox.Visible = true;
                    PlayerRolecomboBox.Text = model.PlayerRole[record.PlayerRole];

                    if (model.MadVersion == MaddenFileVersion.Ver2008)
                    {
                        WeaponLabel.Visible = true;
                        PlayerWeaponcomboBox.Visible = true;
                        PlayerWeaponcomboBox.Text = model.PlayerRole[record.PlayerWeapon];
                    }
                }
                #endregion

                #region Legacy only

                {
                    PlayerFaceId.Value = (int)record.FaceId;
                    OriginalPosition_Combo.SelectedIndex = record.OriginalPositionId;

                    #region Legacy Panel
                    playerBodyOverall.Value = (decimal)record.WaistSize;   //Waist Size BSWT
                    playerBodyWeight.Value = (decimal)record.WaistDefn; //Waist Definition BSWA
                    playerBodyMuscle.Value = (decimal)record.GutDefn; // Gut Definition BSGA
                    playerBodyFat.Value = (decimal)record.GutSize;   // Gut Size BSGT

                    playerEquipmentPadHeight.Value = (decimal)record.ShoulderHeight; //Shoulder height BSPA
                    playerEquipmentPadWidth.Value = (decimal)record.PadSize;   //pad size BSPT
                    playerEquipmentPadShelf.Value = (decimal)record.ShoulderDefn;   //Shoulder definition BSSA
                    playerEquipmentFlakJacket.Value = (decimal)record.ShoulderSize;  //Shoulder size BSST

                    playerArmsMuscle.Value = (decimal)record.ArmDefn; //Arm Definition BSAA
                    playerArmsFat.Value = (decimal)record.ArmSize;   //Arm Size BSAT
                    playerLegsThighMuscle.Value = (decimal)record.ThighDefn;   //thigh definition BSTA
                    playerLegsThighFat.Value = (decimal)record.ThighSize; //thigh size BSTT
                    playerLegsCalfMuscle.Value = (decimal)record.CalfDefn; // Calf Definition BSCA
                    playerLegsCalfFat.Value = (decimal)record.CalfSize;   // Calf Size BSCT

                    playerRearMuscle.Value = record.RearRearFat;
                    playerRearRearFat.Value = (decimal)record.ButtSize; //edit Rear Size BSBT
                    playerRearShape.Value = (decimal)record.ButtDefn;   //edit Rear Definition BSBA

                    playerHairStyleCombo.SelectedIndex = record.HairStyle;
                    playerNasalStripCombo.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.NasalStrip;
                    Tattoo_Left.Value = (decimal)record.FootDefn;  //edit BSFA
                    Tattoo_Right.Value = (decimal)record.FootSize;    //edit bSFT

                    T_Sleeves_Combo.Text = model.PlayerModel.GetSleeve(model.PlayerModel.CurrentPlayerRecord.TempSleeves);
                    T_LeftElbow_Combo.Text = model.PlayerModel.GetElbow(model.PlayerModel.CurrentPlayerRecord.TeamLeftElbow);
                    T_RightElbow_Combo.Text = model.PlayerModel.GetElbow(model.PlayerModel.CurrentPlayerRecord.TeamRightElbow);
                    T_LeftHand_Combo.Text = model.PlayerModel.GetGloves(model.PlayerModel.CurrentPlayerRecord.TeamLeftHand);
                    T_RightHand_Combo.Text = model.PlayerModel.GetGloves(model.PlayerModel.CurrentPlayerRecord.TeamRightHand);
                    T_LeftWrist_Combo.Text = model.PlayerModel.GetWrist(model.PlayerModel.CurrentPlayerRecord.TeamLeftWrist);
                    T_RightWrist_Combo.Text = model.PlayerModel.GetWrist(model.PlayerModel.CurrentPlayerRecord.TeamRightWrist);
                    #endregion
                }
                #endregion

                #region 2019 2020 only
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                {
                    PortraitID_Large.Value = record.PortraitId + 10000;
                    PlayerFaceId.Value = (int)record.FaceID_19;
                    playerAsset.Text = record.Asset;
                    playerHometown.Text = record.Hometown;
                    playerStateCombo.SelectedIndex = record.HomeState;
                    playerBirthday.Text = record.GetBirthday();
                    DevTrait.SelectedIndex = record.DevTrait;

                    foreach (KeyValuePair<string, int> id in model.PlayerModel.PlayerComments)
                    {
                        AudioCombobox.SelectedIndex = -1;   //here
                        if (id.Value == record.PlayerComment)
                        {
                            AudioCombobox.Text = id.Key;
                            break;
                        }
                    }

                    #region 
                    #region Init Archetypes and set combos
                    int start = 0;
                    int end = 0;

                    #region Get Start and End Types
                    switch (record.PositionId)
                    {
                        case (int)MaddenPositions.QB:
                            end = 3;
                            break;
                        case (int)MaddenPositions.HB:
                            {
                                start = 4;
                                end = 6;
                            }
                            break;
                        case (int)MaddenPositions.FB:
                            {
                                start = 7;
                                end = 8;
                            }
                            break;
                        case (int)MaddenPositions.WR:
                            {
                                start = 9;
                                end = 12;
                            }
                            break;
                        case (int)MaddenPositions.TE:
                            {
                                start = 13;
                                end = 15;
                            }
                            break;
                        case (int)MaddenPositions.C:
                            {
                                start = 16;
                                end = 18;
                            }
                            break;
                        case (int)MaddenPositions.LT:
                        case (int)MaddenPositions.RT:
                            {
                                start = 19;
                                end = 21;
                            }
                            break;
                        case (int)MaddenPositions.LG:
                        case (int)MaddenPositions.RG:
                            {
                                start = 22;
                                end = 24;
                            }
                            break;
                        case (int)MaddenPositions.LE:
                        case (int)MaddenPositions.RE:
                            {
                                start = 25;
                                end = 27;
                            }
                            break;
                        case (int)MaddenPositions.DT:
                            {
                                start = 28;
                                end = 30;
                            }
                            break;
                        case (int)MaddenPositions.LOLB:
                        case (int)MaddenPositions.ROLB:
                            {
                                start = 31;
                                end = 34;
                            }
                            break;
                        case (int)MaddenPositions.MLB:
                            {
                                start = 35;
                                end = 37;
                            }
                            break;
                        case (int)MaddenPositions.CB:
                            {
                                start = 38;
                                end = 40;
                            }
                            break;
                        case (int)MaddenPositions.FS:
                        case (int)MaddenPositions.SS:
                            {
                                start = 41;
                                end = 43;
                            }
                            break;
                        case (int)MaddenPositions.K:
                        case (int)MaddenPositions.P:
                            {
                                start = 44;
                                end = 45;
                            }
                            break;
                    }
                    #endregion

                    playerArchetype.Items.Clear();
                    playerOVRArchetypeCombo.Items.Clear();
                    for (int c = start; c <= end; c++)
                    {
                        playerArchetype.Items.Add(model.PlayerModel.ArchetypeList[c]);
                        if (record.PositionId <= 32)
                            playerOVRArchetypeCombo.Items.Add(model.PlayerModel.ArchetypeList[c]);
                    }
                    playerOVRArchetype.Text = ""; //check here
                    #endregion

                    if (record.PositionId <= 32)
                    {
                        playerOVRArchetype.Text = playeroverall.GetOverall19(record, record.PositionId, record.PlayerType).ToString(); //check here
                        playerOVRArchetypeCombo.Text = model.PlayerModel.GetArchetype(record.PlayerType);
                        playerOVRArchetypeCombo.Enabled = true;
                    }

                    #endregion

                    #region Ratings 2019
                    playerPotential.Value = (int)record.Potential;
                    playerQBStance.Value = (int)record.Stance;
                    playerThrowShort.Value = (int)record.ThrowShort;
                    playerThrowMedium.Value = (int)record.ThrowMedium;
                    playerThrowDeep.Value = (int)record.ThrowDeep;
                    playerThrowOnRun.Value = (int)record.ThrowOnRun;
                    playerThrowPressure.Value = (int)record.ThrowPressure;
                    playerBreakSack.Value = (int)record.BreakSack;
                    playerPlayAction.Value = (int)record.PlayAction;
                    playerTrucking.Value = (int)record.Trucking;
                    playerElusive.Value = (int)record.Elusive;
                    playerRB_Vision.Value = (int)record.RB_Vision;
                    playerStiffArm.Value = (int)record.StiffArm;
                    playerSpinMove.Value = (int)record.SpinMove;
                    playerJukeMove.Value = (int)record.JukeMove;
                    playerImpactBlock.Value = (int)record.ImpactBlocking;
                    playerLeadBlock.Value = (int)record.LeadBlock;
                    playerRelease.Value = (int)record.Release;
                    playerPowerMoves.Value = (int)record.PowerMoves;
                    playerFinesseMoves.Value = (int)record.FinesseMoves;
                    playerShortRoute.Value = (int)record.ShortRoute;
                    playerMediumRoute.Value = (int)record.MediumRoute;
                    playerDeepRoute.Value = (int)record.DeepRoute;
                    playerCatchTraffic.Value = (int)record.CatchTraffic;
                    playerSpecCatch.Value = (int)record.SpecCatch;
                    playerRunBlockFinesse.Value = (int)record.RunBlockFootwork;
                    playerRunBlockStrength.Value = (int)record.RunBlockStrength;
                    playerPassBlockFootwork.Value = (int)record.PassBlockFootwork;
                    playerPassBlockStr.Value = (int)record.PassBlockStrength;
                    playerPlayRecog.Value = (int)record.PlayRecognition;
                    playerPursuit.Value = (int)record.Pursuit;
                    playerBlockShed.Value = (int)record.BlockShedding;
                    playerZoneCoverage.Value = (int)record.ZoneCoverage;
                    playerManCover.Value = (int)record.ManCoverage;
                    playerPressCover.Value = (int)record.PressCover;
                    playerHitPower.Value = (int)record.HitPower;
                    playerConfidence.Value = record.Confidence;

                    playerSensePressure.SelectedIndex = record.SensePressure;
                    playerForcePass.SelectedIndex = record.ForcePasses;
                    playerCoversBall.SelectedIndex = (int)record.CoversBall - 1;
                    playerArchetype.Text = model.PlayerModel.GetArchetype(record.PlayerType);
                    playerPlaysBall.SelectedIndex = record.PlaysBall;
                    playerPenalty.SelectedIndex = record.Penalty;
                    playerTuckRun.SelectedIndex = record.TuckRun;
                    playersidelinecatch.SelectedIndex = record.SidelineCatch;
                    RunMotion.SelectedIndex = record.RunStyle;
                    #endregion

                    #region Traits
                    playerAggressiveCatch.Checked = record.AggressiveCatch;
                    playerFightYards.Checked = record.FightYards;
                    playerClutch.Checked = record.Clutch;
                    playerBullrush.Checked = record.DLBullrush;
                    playerFeetInBounds.Checked = record.FeetInBounds;
                    playerThrowSpiral.Checked = record.ThrowSpiral;
                    playerHighMotor.Checked = record.HighMotor;
                    playerBigHitter.Checked = record.BigHitter;
                    playerDropsPasses.Checked = record.DropPasses;
                    playerStripsBall.Checked = record.StripsBall;
                    playerDLSwim.Checked = record.DLSwim;
                    playerThrowAway.Checked = record.ThrowAway;
                    playerPossCatch.Checked = record.PossessionCatch;
                    playerRAC.Checked = record.RunAfterCatch;
                    playerDLSpin.Checked = record.DLSpinmove;
                    #endregion

                    playerCaptain.Checked = record.IsCaptain;

                    #region Appearance / Equipment

                    #region NextGen Panel
                    playerStance.Text = model.PlayerModel.GetStance(record.Stance);
                    playerEndPlay.Text = model.PlayerModel.GetEndPlay(record.EndPlay);
                    SidelineHeadGear_Combo.SelectedIndex = record.SidelineHeadgear;
                    playerFlakJacket.Checked = record.FlakJacket;
                    playerBackPlate.Checked = record.BackPlate;
                    #endregion

                    playerUndershirt.SelectedIndex = record.JerseyTucked;
                    EyePaintLabel.Text = "Face Marks";
                    EyePaintLabel.Location = new Point(327, 487);
                    playerJerseySleeves.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.JerseySleeve;
                    playerSockHeight.SelectedIndex = record.SockHeight;


                    QBStyle.SelectedIndex = record.QBStyle;

                    #endregion

                    Madden20HairStyle.SelectedIndex = record.HairStyle20;
                }
                #endregion

                #endregion

                LoadPlayerSalaries(record);
                DisplayPlayerPort();

                #region Commented out
                /*
                if (model.FileType == MaddenFileType.Franchise)
                {
                    model.PlayerModel.SetProgressionRank();
                    TopForOVR_Updown.Value = model.PlayerModel.ProgRank[0];
                    TopForPhase_Updown.Value = model.PlayerModel.ProgRank[1];
                    TopAvgOvr_Updown.Value = model.PlayerModel.AvgOVR[0];

                    if (model.PlayerModel.CurrentPlayerRecord.YearsPro > 0 && model.PlayerModel.CurrentPlayerRecord.Plpl > 0)
                    {
                        if ((decimal)model.PlayerModel.CurrentPlayerRecord.Ppsp / ((decimal)model.PlayerModel.CurrentPlayerRecord.Plpl / 100) < 0)
                            BaseRank_Updown.Value = 0;
                        else BaseRank_Updown.Value = (decimal)model.PlayerModel.CurrentPlayerRecord.Ppsp / ((decimal)model.PlayerModel.CurrentPlayerRecord.Plpl / 100);
                    }
                    else BaseRank_Updown.Value = 0;

                    Both_Updown.Value = model.PlayerModel.ProgRank[2];
                }
                 * */
                #endregion

            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Occured loading this Player:\r\nCaused by " + e.Source + "\r\n" + e.ToString(), "Exception Loading Player", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadPlayerInfo(lastLoadedRecord);
                return;
            }
            finally
            {
                ResumeLayout();
            }

            lastLoadedRecord = record;
            isInitialising = false;
        }


        #region Controls



        #region Player Ratings Page

        #region Navigation Functions & Create/Delete

        private void filterTeamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (filterTeamComboBox.SelectedIndex == 0)
                    model.PlayerModel.filterteam = -1;
                else if (filterTeamComboBox.SelectedItem.ToString() == "Retired")
                    model.PlayerModel.filterteam = 1014;
                else
                {
                    TeamRecord tr = (TeamRecord)filterTeamComboBox.SelectedItem;
                    model.PlayerModel.filterteam = tr.TeamId;
                }
                isInitialising = true;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void filterPositionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                if (filterPositionComboBox.SelectedIndex == 0)
                    model.PlayerModel.filterposition = -1;
                else model.PlayerModel.filterposition = filterPositionComboBox.SelectedIndex - 1;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void FilterCollegecombobox_SelectedIndexChanged(object sender, EventArgs e)     //New
        {
            if (!isInitialising)
            {
                isInitialising = true;
                if (FilterCollegecomboBox.SelectedIndex == 0)
                    model.PlayerModel.filtercollege = -1;
                else model.PlayerModel.filtercollege = FilterCollegecomboBox.SelectedIndex -1;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void FilterDevTraitComboBox_SelectedIndexChanged (object sender, EventArgs e)       //new
        {
            if (!isInitialising)
            {
                isInitialising = true;
                if (FilterDevTraitcomboBox.SelectedIndex == 0)
                    model.PlayerModel.filterdevtrait = -1;
                else model.PlayerModel.filterdevtrait = FilterDevTraitcomboBox.SelectedIndex - 1;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void filterYearsProComboBox_SelectedIndexChanged(object sender, EventArgs e)        //new
        {
            if (!isInitialising)
            {
                isInitialising = true;
                if (FilterYearsProcomboBox.SelectedIndex == 0)
                    model.PlayerModel.filterYearsPro = -1;
                else model.PlayerModel.filterYearsPro = FilterYearsProcomboBox.SelectedIndex - 1;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void FilterArchetypecombobox_SelectedIndexChanged(object sender, EventArgs e)       //new
        {
            if (!isInitialising)
            {
                isInitialising = true;
                if (FilterArchetypecomboBox.SelectedIndex == 0)
                    model.PlayerModel.filterarchetypes = -1;
                else model.PlayerModel.filterarchetypes = FilterArchetypecomboBox.SelectedIndex - 1;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void PlayerGridViewChange(bool update, int pgid)
        {
            if (PlayerGridView.CurrentRow.Index < 0)
                return;
            DataGridViewRow row = PlayerGridView.CurrentRow;

            if (update)
            {
                if (model.PlayerModel.playernames.ContainsKey(model.PlayerModel.CurrentPlayerRecord.PlayerId))
                {
                    model.PlayerModel.playernames[model.PlayerModel.CurrentPlayerRecord.PlayerId] = model.PlayerModel.CurrentPlayerRecord.FirstName
                        + " " + model.PlayerModel.CurrentPlayerRecord.LastName;
                }
                else
                {
                    model.PlayerModel.playernames.Add(model.PlayerModel.CurrentPlayerRecord.PlayerId, model.PlayerModel.CurrentPlayerRecord.FirstName
                        + " " + model.PlayerModel.CurrentPlayerRecord.LastName);
                }

                row.Cells[1].Value = model.PlayerModel.playernames[pgid];
            }

            if (pgid != -1)
                row.Cells[0].Value = pgid;
            int r = (int)row.Cells[0].Value;
            LoadPlayerInfo(model.PlayerModel.GetPlayerByPlayerId(r));
        }

        private void PlayerGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            isInitialising = true;

            PlayerGridViewChange(false, -1);

            isInitialising = false;
        }

        private void DraftClass_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.filterdraft = DraftClass_Checkbox.Checked;
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void ProBowlcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.filterprobowl = ProBowlcheckBox.Checked; //check here
                InitPlayerList();
                isInitialising = false;
            }
        }

        private void deletePlayerButton_Click(object sender, EventArgs e)
        {
            isInitialising = true;

            DialogResult result = MessageBox.Show("Are you sure you want to delete this player?\r\n\r\nAlthough this player will disappear from the editor\r\nchanges will not take effect until you save.", "About to Delete " + model.PlayerModel.CurrentPlayerRecord.FirstName + " " + model.PlayerModel.CurrentPlayerRecord.LastName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int current = PlayerGridView.CurrentRow.Index;
                model.PlayerModel.DeletePlayerRecord(model.PlayerModel.CurrentPlayerRecord);
                PlayerGridView.Rows.RemoveAt(current);
                if (PlayerGridView.Rows.Count > 0)
                {
                    PlayerGridViewChange(false, -1);
                }
                else
                {
                    // No players left to load, reset filters back to ALL
                    LoadPlayerInfo(null);
                }
            }

            isInitialising = true;
        }

        // TO DO : Fix CreatePlayer, need to set all player info to some sort of defaults
        // before it is displayed.  Set player ID #  Need to reset everything to defaults
        private void createPlayerButton_Click(object sender, EventArgs e)
        {
            PlayerRecord newRecord = model.PlayerModel.CreateNewPlayerRecord();
            // Add the player to free agents
            newRecord.TeamId = EditorModel.FREE_AGENT_TEAM_ID;
            EditorModel.totalplayers++;
            // Need to set unique PLAYER ID
            newRecord.PlayerId = EditorModel.totalplayers;
            // This sets unique POID
            newRecord.NFLID = EditorModel.totalplayers;
            newRecord.FirstName = "NA";
            newRecord.LastName = "NA";
            //Most variables start off at zero but some can't like height and weight so set them
            newRecord.Height = 72; // 6'0"
            newRecord.Weight = 40; // 200#
            newRecord.Overall = 25;
            model.PlayerModel.CurrentPlayerRecord = newRecord;

            isInitialising = true;
            string newname = newRecord.FirstName + " " + newRecord.LastName;
            model.PlayerModel.playernames.Add(newRecord.PlayerId, newname);
            object[] entry = { newRecord.PlayerId, newname };
            PlayerGridView.Rows.Add(entry);
            PlayerGridView.CurrentCell = PlayerGridView.Rows[PlayerGridView.Rows.Count - 1].Cells[0];
            PlayerGridViewChange(true, -1);
            isInitialising = false;
        }

        private void ExportPlayerCSV_Button_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            Stream myStream = null;

            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = fileDialog.OpenFile()) != null)
                    {
                        PlayerRecord rec = model.PlayerModel.CurrentPlayerRecord;
                        TableModel table = model.TableModels["PLAY"];
                        List<TdbFieldProperties> props = table.GetFieldList();
                        StringBuilder hbuilder = new StringBuilder();
                        StringBuilder builder = new StringBuilder();
                        StreamWriter writer = new StreamWriter(myStream);
                        hbuilder.Append("PLAY");
                        hbuilder.Append(",");
                        if (model.MadVersion == MaddenFileVersion.Ver2019)
                            hbuilder.Append("2019");
                        if (model.MadVersion == MaddenFileVersion.Ver2020)
                            hbuilder.Append("2020");
                        hbuilder.Append(",");
                        hbuilder.Append("No");
                        hbuilder.Append(",");
                        writer.WriteLine(hbuilder);
                        writer.Flush();

                        hbuilder.Clear();
                        foreach (TdbFieldProperties tdb in props)
                        {
                            hbuilder.Append(tdb.Name);
                            hbuilder.Append(",");
                        }
                        writer.WriteLine(hbuilder.ToString());
                        writer.Flush();

                        foreach (TdbFieldProperties tdb in props)
                        {
                            if (tdb.FieldType == TdbFieldType.tdbString)
                                builder.Append(rec.GetStringField(tdb.Name));
                            else if (tdb.FieldType == TdbFieldType.tdbFloat)
                                builder.Append(rec.GetFloatField(tdb.Name));
                            else
                            {
                                int test = rec.GetIntField(tdb.Name);
                                builder.Append(test);
                            }
                            builder.Append(",");
                        }
                        writer.WriteLine(builder.ToString());
                        writer.Flush();
                        writer.Close();
                    }
                }
                catch (IOException err)
                {
                    err = err;
                    MessageBox.Show("Error opening file\r\n\r\n Check that the file is not already opened", "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ImportPlayerPort_Button_Click(object sender, EventArgs e)
        {
            isInitialising = true;
            OpenFileDialog fileDialog = new OpenFileDialog();
            Stream myStream = null;
            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;
            isInitialising = true;

            DisplayPlayerPort();
        }

        private void PlayerPortraitExport_Button_Click(object sender, EventArgs e)
        {
            string savefilename = "";
            SaveFileDialog portsavedialog = new SaveFileDialog();
            portsavedialog.Title = "Save Player Portrait";
            portsavedialog.Filter = "BMP Image | *.BMP";
            portsavedialog.CheckPathExists = true;

            if (portsavedialog.ShowDialog() == DialogResult.OK)
                savefilename = portsavedialog.FileName;
            if (savefilename == "")
                return;

            Image image = manager.PlayerPortDAT.ParentTerf.Data.DataFiles[model.PlayerModel.CurrentPlayerRecord.PortraitId + 1].mmap_data.GetPortraitDisplay();
            image.Save(savefilename, ImageFormat.Bmp);
        }

        #endregion

        #region Player General Controls

        private void firstNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (firstNameTextBox.Text != model.PlayerModel.CurrentPlayerRecord.FirstName)
                {
                    isInitialising = true;
                    model.PlayerModel.CurrentPlayerRecord.FirstName = firstNameTextBox.Text;
                    model.PlayerModel.playernames[model.PlayerModel.CurrentPlayerRecord.PlayerId] = model.PlayerModel.CurrentPlayerRecord.FirstName +
                        " " + model.PlayerModel.CurrentPlayerRecord.LastName;
                    PlayerGridViewChange(true, model.PlayerModel.CurrentPlayerRecord.PlayerId);
                    isInitialising = false;
                }
            }
        }

        private void lastNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (lastNameTextBox.Text != model.PlayerModel.CurrentPlayerRecord.LastName)
                {
                    isInitialising = true;
                    model.PlayerModel.CurrentPlayerRecord.LastName = lastNameTextBox.Text;
                    model.PlayerModel.playernames[model.PlayerModel.CurrentPlayerRecord.PlayerId] = model.PlayerModel.CurrentPlayerRecord.FirstName +
                        " " + model.PlayerModel.CurrentPlayerRecord.LastName;
                    PlayerGridViewChange(true, model.PlayerModel.CurrentPlayerRecord.PlayerId);
                    isInitialising = false;

                    if (model.PlayerModel.PlayerComments.ContainsKey(model.PlayerModel.CurrentPlayerRecord.LastName))
                    {
                        playerComment.Value = model.PlayerModel.PlayerComments[model.PlayerModel.CurrentPlayerRecord.LastName];
                    }
                }
            }
        }

        private void PlayerID_Updown_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (PlayerID_Updown.Value != model.PlayerModel.CurrentPlayerRecord.PlayerId)
                {
                    isInitialising = true;
                    // changing to a new player id
                    if (!CheckPlayerUniqueness((int)PlayerID_Updown.Value, -1))
                    {
                        int oldpgid = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        string name = "";
                        if (model.PlayerModel.playernames.ContainsKey(oldpgid))
                        {
                            // This should always happen, but name will be "unknown" if id doesnt exist
                            name = model.PlayerModel.playernames[oldpgid];
                            model.PlayerModel.playernames.Remove(oldpgid);
                        }
                        else name = "unknown";

                        model.PlayerModel.playernames.Add((int)PlayerID_Updown.Value, name);
                        model.PlayerModel.CurrentPlayerRecord.PlayerId = (int)PlayerID_Updown.Value;
                        PlayerGridViewChange(true, model.PlayerModel.CurrentPlayerRecord.PlayerId);
                    }
                    isInitialising = false;
                }
            }
        }

        private void NFL_Updown_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (NFL_Updown.Value != model.PlayerModel.CurrentPlayerRecord.NFLID)
                {
                    if (!CheckPlayerUniqueness(-1, (int)NFL_Updown.Value))
                    {
                        model.PlayerModel.CurrentPlayerRecord.NFLID = (int)NFL_Updown.Value;
                    }
                }
            }
        }

        private void playerPortraitId_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (manager.PlayerPortDAT.isterf && manager.PlayerPortDAT.ParentTerf.files < playerPortraitId.Value + 1)
                {
                    playerPortraitId.Value = model.PlayerModel.CurrentPlayerRecord.PortraitId;
                    return;
                }

                model.PlayerModel.CurrentPlayerRecord.PortraitId = (int)playerPortraitId.Value;
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    PortraitID_Large.Value = model.PlayerModel.CurrentPlayerRecord.PortraitId + 10000;

                DisplayPlayerPort();
            }
        }

        private void playerFaceID_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.FaceID_19 = (int)PlayerFaceId.Value;
                else model.PlayerModel.CurrentPlayerRecord.FaceId = (int)PlayerFaceId.Value;
            }
        }

        private void playerComment_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.PlayerComment = (int)playerComment.Value;

                isInitialising = true;
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                {
                    foreach (KeyValuePair<string, int> id in model.PlayerModel.PlayerComments)
                    {
                        if (id.Value == playerComment.Value)
                        {
                            AudioCombobox.Text = id.Key;
                            break;
                        }
                    }
                }

                isInitialising = false;
            }
        }

        private void AudioCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                playerComment.Value = model.PlayerModel.PlayerComments[AudioCombobox.Text];
            }
        }

        private void playerAge_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Age = (int)playerAge.Value;
            }
        }

        private void playerYearsPro_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.YearsPro = (int)playerYearsPro.Value;
                //  Add more seasons to the players career
                isInitialising = true;
                if (model.MadVersion <= MaddenFileVersion.Ver2008)
                    InitStatsYear(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void playerAsset_TextChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Asset = playerAsset.Text;
        }

        private void playerHometown_TextChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Hometown = playerHometown.Text;
        }

        private void playerStateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.HomeState = (int)playerStateCombo.SelectedIndex;
        }

        private void playerBirthday_TextChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SetBirthday(playerBirthday.Text);
        }

        private void playerDraftRound_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.DraftRound = (int)playerDraftRound.Value;
            }
        }

        private void playerDraftRoundIndex_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.DraftRoundIndex = (int)playerDraftRoundIndex.Value;
            }
        }

        private void CareerPhase_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.CareerPhase = CareerPhase_Combo.SelectedIndex;
        }

        private void PlayerHeight_Feet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                SetPlayerHeight();
        }

        private void PlayerHeight_Inches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                SetPlayerHeight();
        }

        private void SetPlayerHeight()
        {
            int height = (int)PlayerHeight_Feet.SelectedIndex * 12 + (int)PlayerHeight_Inches.SelectedIndex;
            if (height > 127)
                height = 127;
            else if (height == 0)
                height = 1;
            model.PlayerModel.CurrentPlayerRecord.Height = height;

            PlayerHeight_Feet.SelectedIndex = (int)model.PlayerModel.CurrentPlayerRecord.Height / 12;
            PlayerHeight_Inches.SelectedIndex = model.PlayerModel.CurrentPlayerRecord.Height - (int)(PlayerHeight_Feet.SelectedIndex * 12);
        }

        private void playerWeight_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Weight = (int)playerWeight.Value - 160;
            }
        }

        private void playerJerseyNumber_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.JerseyNumber = (int)playerJerseyNumber.Value;
            }
        }

        private void playerHairColorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.HairColor = playerHairColorCombo.SelectedIndex;
            }
        }

        private void CollegeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                foreach (KeyValuePair<int, college_entry> playcol in model.Colleges)
                {
                    if (playcol.Value.name == CollegeCombo.Text)
                    {
                        model.PlayerModel.CurrentPlayerRecord.CollegeId = playcol.Key;
                    }
                }
            }
        }

        private void teamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.PreviousTeamId = model.PlayerModel.CurrentPlayerRecord.TeamId;

                if ((string)Team_Combo.Text == "Retired")
                {
                    if (model.FileType == MaddenFileType.Franchise)
                    {
                        model.PlayerModel.CurrentPlayerRecord.TeamId = 1014;
                        model.PlayerModel.RemovePlayerFromDepthChart(model.PlayerModel.CurrentPlayerRecord.PlayerId);
                    }
                    else
                    {
                        model.PlayerModel.DeletePlayerRecord(model.PlayerModel.CurrentPlayerRecord);
                    }
                }
                else
                {
                    model.PlayerModel.ChangePlayersTeam(((TeamRecord)Team_Combo.SelectedItem));
                }

                isInitialising = true;
                InitPlayerList();
                isInitialising = false;

                //s68 
                //Salary cap penalty.  Not for free agents.  Not sure we really want to mess with this, going to comment out and leave it for roster
                // makers to adjust manually
                //if (model.PlayerModel.CurrentPlayerRecord.TeamId == 1009)
                //    return;
                //else if (model.FileType == MaddenFileType.Franchise && model.PlayerModel.CurrentPlayerRecord.ContractYearsLeft > 0)
                //{
                //    int total = model.TeamModel.GetTeamRecord(model.PlayerModel.CurrentPlayerRecord.PreviousTeamId).SalaryCapPenalty1;

                //    for (int t = model.PlayerModel.CurrentPlayerRecord.ContractLength - model.PlayerModel.CurrentPlayerRecord.ContractYearsLeft; t < model.PlayerModel.CurrentPlayerRecord.ContractLength; t++)
                //        total += model.PlayerModel.CurrentPlayerRecord.GetSigningBonusAtYear(t);

                //    model.TeamModel.GetTeamRecord(model.PlayerModel.CurrentPlayerRecord.PreviousTeamId).SalaryCapPenalty1 = total;
                //}
            }
        }

        private void positionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.PositionId = (int)positionComboBox.SelectedIndex;
            }
        }

        private void OriginalPosition_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.OriginalPositionId = (int)OriginalPosition_Combo.SelectedIndex;
            }
        }

        private void playerThrowingStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.ThrowStyle = (playerThrowingStyle.SelectedIndex == 1);
            }
        }

        private void playerTendency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Tendency = PlayerTendency.SelectedIndex;
            }
        }

        private void PlayerRolecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion < MaddenFileVersion.Ver2007)
                    return;
                else
                {
                    int res = -1;
                    foreach (KeyValuePair<int, string> role in model.PlayerRole)
                        if (role.Value == PlayerRolecomboBox.Text)
                            res = role.Key;
                    if (res != -1)
                        model.PlayerModel.CurrentPlayerRecord.PlayerRole = res;
                }
            }
        }

        private void PlayerWeaponcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion > MaddenFileVersion.Ver2008)
                    return;
                int res = -1;
                foreach (KeyValuePair<int, string> role in model.PlayerRole)
                    if (role.Value == PlayerWeaponcomboBox.Text)
                        res = role.Key;
                if (res != -1)
                    model.PlayerModel.CurrentPlayerRecord.PlayerWeapon = res;
            }
        }

        private void playerDominantHand_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.DominantHand = playerDominantHand.Checked;
            }
        }

        private void playerNFLIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.NFLIcon = playerNFLIcon.Checked;
            }
        }

        private void playerCaptain_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.IsCaptain = playerCaptain.Checked;
        }

        private void playerProBowl_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.ProBowl = playerProBowl.Checked;
            }
        }

        private void PlayerHoldOut_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Holdout = PlayerHoldOut.Checked;
            }
        }

        private void InactiveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                List<InactiveRecord> remove = new List<InactiveRecord>();
                if (!PlayerInactive.Checked)
                {
                    foreach (TableRecordModel iar in model.TableModels[EditorModel.INACTIVE_TABLE].GetRecords())
                    {
                        if (iar.Deleted)
                            continue;
                        InactiveRecord i = (InactiveRecord)iar;
                        if (i.PlayerID == model.PlayerModel.CurrentPlayerRecord.PlayerId)
                            remove.Add(i);
                    }
                    foreach (InactiveRecord test in remove)
                        test.SetDeleteFlag(true);
                }
                else
                {
                    InactiveRecord record = (InactiveRecord)model.TableModels[EditorModel.INACTIVE_TABLE].CreateNewRecord(true);
                    record.PlayerID = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                    record.TeamId = model.PlayerModel.CurrentPlayerRecord.TeamId;
                }
            }
        }

        #endregion

        #region Player Overall

        private void calculateOverallButton_Click(object sender, EventArgs e)   //new
        {
            if (model.MadVersion != MaddenFileVersion.Ver2019)
            {
                int archetypeId = model.PlayerModel.GetArchetype(playerOVRArchetypeCombo.Text);
                int positionId = model.PlayerModel.CurrentPlayerRecord.PositionId;
                double overallDouble = playeroverall.GetOverall19(model.PlayerModel.CurrentPlayerRecord, positionId, archetypeId);
                int overall = (int)overallDouble;

                // Limit the overall rating to 99 if it exceeds 99
                if (overall > 99)
                {
                    overall = 99;
                }
                // Set a minimum overall rating of 25
                if (overall < 25 || double.IsNaN(overallDouble))
                {
                    overall = 25;
                }
                model.PlayerModel.CurrentPlayerRecord.Overall = overall;
                Overall.Value = overall;
            }
        }




        private void Overall_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Overall = (int)Overall.Value;
            }
        }

        private void playerOVRArchetypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                int type = model.PlayerModel.GetArchetype(playerOVRArchetypeCombo.Text);
                playerOVRArchetype.Text = playeroverall.GetOverall19(model.PlayerModel.CurrentPlayerRecord, model.PlayerModel.CurrentPlayerRecord.PositionId, type).ToString(); //check here
            }

        }

        #endregion

        #region Player Ratings

        private void playerSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Speed = (int)playerSpeed.Value;
            }
        }
        
        private void playerStrength_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Strength = (int)playerStrength.Value;
            }
        }

        private void playerAwareness_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Awareness = (int)playerAwareness.Value;
            }
        }

        private void playerAgility_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Agility = (int)playerAgility.Value;
            }
        }

        private void playerAcceleration_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Acceleration = (int)playerAcceleration.Value;
            }
        }

        private void playerCatching_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Catching = (int)playerCatching.Value;
            }
        }

        private void playerCarrying_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Carrying = (int)playerCarrying.Value;
            }
        }

        private void playerJumping_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Jumping = (int)playerJumping.Value;
            }
        }

        private void playerBreakTackle_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.BreakTackle19 = (int)playerBreakTackle.Value;
                else
                    model.PlayerModel.CurrentPlayerRecord.BreakTackle = (int)playerBreakTackle.Value;
            }
        }

        private void playerTackle_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Tackle = (int)playerTackle.Value;
            }
        }

        private void playerThrowPower_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.ThrowPower = (int)playerThrowPower.Value;
            }
        }

        private void playerThrowAccuracy_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.ThrowAccuracy = (int)playerThrowAccuracy.Value;
            }
        }

        private void playerPassBlocking_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.PassBlocking = (int)playerPassBlocking.Value;
            }
        }

        private void playerRunBlocking_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.RunBlocking = (int)playerRunBlocking.Value;
            }
        }

        private void playerKickPower_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.KickPower = (int)playerKickPower.Value;
            }
        }

        private void playerKickAccuracy_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.KickAccuracy = (int)playerKickAccuracy.Value;
            }
        }

        private void playerKickReturn_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.KickReturn = (int)playerKickReturn.Value;
            }
        }

        private void playerStamina_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Stamina = (int)playerStamina.Value;
            }
        }

        private void playerInjury_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Injury = (int)playerInjury.Value;
            }
        }

        private void playerToughness_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Toughness = (int)playerToughness.Value;
            }
        }


        private void playerImportance_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Importance = (int)playerImportance.Value;
            }
        }

        private void playerMorale_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                // 2019 field isnt morale
                if (model.MadVersion > MaddenFileVersion.Ver2008)
                    return;
                else model.PlayerModel.CurrentPlayerRecord.Morale = (int)playerMorale.Value;
            }
        }

        private void playerEgo_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion <= MaddenFileVersion.Ver2006)
                    model.PlayerModel.CurrentPlayerRecord.Pcel = (int)playerEgo.Value;
                else model.PlayerModel.CurrentPlayerRecord.Ego = (int)playerEgo.Value;
            }
        }






        private void playerEquipmentThighPads_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.LegsThighPads = (int)playerEquipmentThighPads.Value;
            }
        }








        #endregion

        #region 2019 Ratings and Traits

        #region 2019 Traits

        private void playerThrowAway_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowAway = playerThrowAway.Checked;
        }

        private void playerThrowSpiral_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowSpiral = playerThrowSpiral.Checked;
        }

        private void playerForcePasses_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ForcePasses = playerForcePass.SelectedIndex;
        }

        private void playerFightYards_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.FightYards = playerFightYards.Checked;
        }

        private void playerPlaysBall_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PlaysBall = playerPlaysBall.SelectedIndex;
        }

        private void playerHighMotor_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.HighMotor = playerHighMotor.Checked;
        }

        private void playerDLSwim_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.DLSwim = playerDLSwim.Checked;
        }

        private void playerBullrush_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.DLBullrush = playerBullrush.Checked;
        }

        private void playerDLSpin_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.DLSpinmove = playerDLSpin.Checked;
        }

        private void playerBigHitter_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.BigHitter = playerBigHitter.Checked;
        }

        private void playerClutch_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Clutch = playerClutch.Checked;
        }

        private void playerTuckRun_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TuckRun = (int)playerTuckRun.SelectedIndex;
        }

        private void playerStripsBall_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.StripsBall = playerStripsBall.Checked;
        }

        private void playerHighPointCatch_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PossessionCatch = playerPossCatch.Checked;
        }

        private void playerTRFB_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.FeetInBounds = playerFeetInBounds.Checked;
        }

        private void playerTRIC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Penalty = playerPenalty.SelectedIndex;
        }

        private void playerTRSC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SidelineCatch = playersidelinecatch.SelectedIndex;
        }

        private void playerPRSE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.RunStyle = RunMotion.SelectedIndex;
        }

        private void playerTRJR_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.AggressiveCatch = playerAggressiveCatch.Checked;
        }

        private void playerRAC_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.RunAfterCatch = playerRAC.Checked;
        }

        private void playerDropsPasses_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.DropPasses = playerDropsPasses.Checked;
        }

        private void playerCoversBall_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.CoversBall = playerCoversBall.SelectedIndex + 1;
        }

        private void playerPlaysBall_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PlaysBall = playerPlaysBall.SelectedIndex;
        }

        private void playerForcePass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ForcePasses = playerForcePass.SelectedIndex;
        }

        private void playerSensePressure_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SensePressure = playerSensePressure.SelectedIndex;
        }

        private void playerFightYards_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.FightYards = playerFightYards.Checked;
        }

        #endregion

        #region 2019 Ratings

        private void playerPotential_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Potential = (int)playerPotential.Value;
        }

        private void playerQBStance_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Stance = (int)playerQBStance.Value;
        }

        private void playerRelease_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Release = (int)playerRelease.Value;
        }

        private void playerThrowShort_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowShort = (int)playerThrowShort.Value;
        }

        private void playerThrowMedium_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowMedium = (int)playerThrowMedium.Value;
        }

        private void playerThrowDeep_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowDeep = (int)playerThrowDeep.Value;
        }

        private void playerThrowOnRun_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowOnRun = (int)playerThrowOnRun.Value;
        }

        private void playerThrowPressure_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThrowPressure = (int)playerThrowPressure.Value;
        }

        private void playerBreakSack_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.BreakSack = (int)playerBreakSack.Value;
        }

        private void playerPlayAction_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PlayAction = (int)playerPlayAction.Value;
        }

        private void playerTrucking_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Trucking = (int)playerTrucking.Value;
        }

        private void playerElusive_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Elusive = (int)playerElusive.Value;
        }

        private void playerRB_Vision_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.RB_Vision = (int)playerRB_Vision.Value;
        }

        private void playerStiffArm_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.StiffArm = (int)playerStiffArm.Value;
        }

        private void playerSpinMove_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SpinMove = (int)playerSpinMove.Value;
        }

        private void playerJukeMove_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.JukeMove = (int)playerJukeMove.Value;
        }

        private void playerImpactBlock_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ImpactBlocking = (int)playerImpactBlock.Value;
        }

        private void playerLeadBlock_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.LeadBlock = (int)playerLeadBlock.Value;
        }

        private void playerMoves_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PowerMoves = (int)playerPowerMoves.Value;
        }

        private void playerFinesseMoves_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.FinesseMoves = (int)playerFinesseMoves.Value;
        }

        private void playerShortRoute_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ShortRoute = (int)playerShortRoute.Value;
        }

        private void playerMediumRoute_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.MediumRoute = (int)playerMediumRoute.Value;
        }

        private void playerDeepRoute_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.DeepRoute = (int)playerDeepRoute.Value;
        }

        private void playerCatchTraffic_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.CatchTraffic = (int)playerCatchTraffic.Value;
        }

        private void playerSpecCatch_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SpecCatch = (int)playerSpecCatch.Value;
        }

        private void playerRunBlockFinesse_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.RunBlockFootwork = (int)playerRunBlockFinesse.Value;
        }

        private void playerRunBlockStrength_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.RunBlockStrength = (int)playerRunBlockStrength.Value;
        }

        private void playerPassBlockFootwork_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PassBlockFootwork = (int)playerPassBlockFootwork.Value;
        }

        private void playerPassBlockStr_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PassBlockStrength = (int)playerPassBlockStr.Value;
        }

        private void playerPlayRecog_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PlayRecognition = (int)playerPlayRecog.Value;
        }

        private void playerPursuit_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Pursuit = (int)playerPursuit.Value;
        }

        private void playerBlockShed_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.BlockShedding = (int)playerBlockShed.Value;
        }

        private void playerZoneCoverage_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ZoneCoverage = (int)playerZoneCoverage.Value;
        }

        private void playerManCover_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ManCoverage = (int)playerManCover.Value;
        }

        private void playerPressCover_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PressCover = (int)playerPressCover.Value;
        }

        private void playerHitPower_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.HitPower = (int)playerHitPower.Value;
        }



        private void playerConfidence_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Confidence = (int)playerConfidence.Value;
        }

        private void playerArchetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.PlayerType = model.PlayerModel.GetArchetype(playerArchetype.Text);
        }


        #endregion

        #endregion

        #endregion

        #region Player Appearance Equipment Misc

        #region 2019 Equipment Misc

        private void playerStance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Stance = model.PlayerModel.GetStance(playerStance.Text);
        }

        private void playerEndPlay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.EndPlay = model.PlayerModel.GetEndPlay(playerEndPlay.Text);
        }

        private void SidelineHeadGear_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SidelineHeadgear = SidelineHeadGear_Combo.SelectedIndex;
        }

        private void playerFlakJacket_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.FlakJacket = playerFlakJacket.Checked;
            }
        }

        private void playerBackPlate_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.BackPlate = playerBackPlate.Checked;
            }
        }


        #endregion

        #region Legacy appearance

        private void playerBodyOverall_ValueChanged(object sender, EventArgs e) //BSWT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.25f, 0.3f, 0.325f, 0.35f, 0.4f, 0.45f, 0.475f, 0.5f, 0.525f, 0.55f, 0.575f, 0.6f, 0.625f, 0.65f, 0.675f, 0.7f, 0.75f, 0.8f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerBodyOverall.Value - v)).First();

                // Set the value to the nearest allowed value
                playerBodyOverall.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.WaistSize = nearestValue;
            }
        }

        private void playerBodyWeight_ValueChanged(object sender, EventArgs e)  //BSWA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.2f, 0.25f, 0.3f,0.35f, 0.4f, 0.5f, 0.55f, 0.6f, 0.625f, 0.65f, 0.675f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 0.95f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerBodyWeight.Value - v)).First();

                // Set the value to the nearest allowed value
                playerBodyWeight.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.WaistDefn = nearestValue;
            }
        }

        private void playerBodyMuscle_ValueChanged(object sender, EventArgs e)  //BSGA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.902f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerBodyMuscle.Value - v)).First();

                // Set the value to the nearest allowed value
                playerBodyMuscle.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.GutDefn = nearestValue;
            }
        }

        private void playerBodyFat_ValueChanged(object sender, EventArgs e) //BSGT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.251f, 1, 2, 2.461f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerBodyFat.Value - v)).First();

                // Set the value to the nearest allowed value
                playerBodyFat.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.GutSize= nearestValue;
            }
        }

        private void playerEquipmentPadHeight_ValueChanged(object sender, EventArgs e)  //BSPA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.025f, 0.03f, 0.035f, 0.04f, 0.045f, 0.05f, 0.055f, 0.06f, 0.07f, 0.075f, 0.085f, 0.1f, 0.102f, 0.0115f, 0.12f, 0.125f, 0.13f, 0.135f, 0.15f, 0.16f, 0.175f, 0.185f, 0.2f, 0.25f, 0.275f, 0.3f, 0.4f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerEquipmentPadHeight.Value - v)).First();

                // Set the value to the nearest allowed value
                playerEquipmentPadHeight.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ShoulderHeight = (int)nearestValue; //edit
            }
        }

        private void playerEquipmentPadWidth_ValueChanged(object sender, EventArgs e)   //BSPT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.01f, 0.025f, 0.035f, 0.04f, 0.05f, 0.06f, 0.075f, 0.1f, 0.117f, 0.125f, 0.135f, 0.15f, 0.175f, 0.185f, 0.23f, 0.252f, 0.275f, 0.285f, 0.325f, 0.35f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerEquipmentPadWidth.Value - v)).First();

                // Set the value to the nearest allowed value
                playerEquipmentPadWidth.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.PadSize = nearestValue;   //edit
            }
        }

        private void playerEquipmentPadShelf_ValueChanged(object sender, EventArgs e)   //BSSA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.5f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerEquipmentPadShelf.Value - v)).First();

                // Set the value to the nearest allowed value
                playerEquipmentPadShelf.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ShoulderDefn = nearestValue;    //body edit
            }
        }

        private void playerEquipmentFlakJacket_ValueChanged(object sender, EventArgs e) //BSST 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.205f, 1, 2, 2.581f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerEquipmentFlakJacket.Value - v)).First();

                // Set the value to the nearest allowed value
                playerEquipmentFlakJacket.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ShoulderSize = nearestValue;
            }
        }

        private void playerArmsMuscle_ValueChanged(object sender, EventArgs e)  //BSAA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.15f, 0.25f, 0.35f, 0.4f, 0.5f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerArmsMuscle.Value - v)).First();

                // Set the value to the nearest allowed value
                playerArmsMuscle.Value = (decimal)nearestValue;

                // Update your model or perform other actions as needed
                model.PlayerModel.CurrentPlayerRecord.ArmDefn = nearestValue;
            }
        }

        private void playerArmsFat_ValueChanged(object sender, EventArgs e) //BSAT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.175f, 0.5f, 0.75f, 0.8f, 1, 1.1f, 1.15f, 1.25f, 1.35f, 1.4f, 1.5f, 1.6f, 1.75f, 1.85f, 2, 2.507f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerArmsFat.Value - v)).First();

                // Set the value to the nearest allowed value
                playerArmsFat.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ArmSize = nearestValue;   //body edit
            }
        }

        private void playerLegsThighMuscle_ValueChanged(object sender, EventArgs e) //BSTA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerLegsThighMuscle.Value - v)).First();

                // Set the value to the nearest allowed value
                playerLegsThighMuscle.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ThighDefn = nearestValue; //body edit
            }
        }

        private void playerLegsThighFat_ValueChanged(object sender, EventArgs e)    //BSTT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.25f, 1, 2, 2.415f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerLegsThighFat.Value - v)).First();

                // Set the value to the nearest allowed value
                playerLegsThighFat.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ThighSize = nearestValue;    //body edit
            }
        }

        private void playerLegsCalfMuscle_ValueChanged(object sender, EventArgs e)  //BSCA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.758f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerLegsCalfMuscle.Value - v)).First();

                // Set the value to the nearest allowed value
                playerLegsCalfMuscle.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.CalfDefn = nearestValue;
            }
        }

        private void playerLegsCalfFat_ValueChanged(object sender, EventArgs e) //BSCT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.103f, 1, 2, 2.681f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerLegsCalfFat.Value - v)).First();

                // Set the value to the nearest allowed value
                playerLegsCalfFat.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.CalfSize = nearestValue;
            }
        }

        private void playerRearRearFat_ValueChanged(object sender, EventArgs e) //BSBT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 1, 1, 2, 2.353f, 2.65f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerRearRearFat.Value - v)).First();

                // Set the value to the nearest allowed value
                playerRearRearFat.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ButtSize = nearestValue;   //edit
            }
        }

        private void playerLeftTattoo_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.LeftTattoo = (int)playerRearRearFat.Value;   
            }
        }
       
        private void playerRightTattoo_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.RightTattoo = (int)playerRearRearFat.Value;  
            }
        }

        private void playerRearShape_ValueChanged(object sender, EventArgs e)   //BSBA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.7f, 0.8f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)playerRearShape.Value - v)).First();

                // Set the value to the nearest allowed value
                playerRearShape.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.ButtDefn = nearestValue;    //edit
            }
        }


        private void playerHairStyleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.HairStyle = playerHairStyleCombo.SelectedIndex;
            }
        }

        private void Tattoo_Left_ValueChanged(object sender, EventArgs e)   //BSFA 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 1 };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)Tattoo_Left.Value - v)).First();

                // Set the value to the nearest allowed value
                Tattoo_Left.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.FootDefn = nearestValue;    //11-20-23
            }
        }

        private void Tattoo_Right_ValueChanged(object sender, EventArgs e)  // BSFT 11-21-23
        {
            if (!isInitialising)
            {
                // Define the allowed values
                float[] allowedValues = new float[] { 0.00f, 0.2f, 1, 2, 2.481f };

                // Find the nearest allowed value
                float nearestValue = allowedValues.OrderBy(v => Math.Abs((float)Tattoo_Right.Value - v)).First();

                // Set the value to the nearest allowed value
                Tattoo_Right.Value = (decimal)nearestValue;

                model.PlayerModel.CurrentPlayerRecord.FootSize = nearestValue;   //11-20-23
            }
        }

        #endregion

        #region Player Equipment Controls

        #region Equipment

        private void playerJerseySleeves_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.JerseySleeve = playerJerseySleeves.SelectedIndex;
        }

        private void playerEyePaintCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.EyePaint = model.PlayerModel.GetFaceMark(playerEyePaintCombo.Text);
            }
        }

        private void playerNeckRollCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.NeckRoll = playerNeckRollCombo.SelectedIndex;
            }
        }

        private void playerVisorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.Visor = model.PlayerModel.GetVisor(playerVisorCombo.Text);
            }
        }

        private void playerMouthPieceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.MouthPiece = playerMouthPieceCombo.SelectedIndex;
            }
        }

        private void playerLeftElbowCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.LeftElbow = model.PlayerModel.GetElbow(playerLeftElbowCombo.Text);
            }
        }

        private void playerRightElbowCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.RightElbow = model.PlayerModel.GetElbow(playerRightElbowCombo.Text);
            }
        }

        private void playerLeftWristCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.LeftWrist = model.PlayerModel.GetWrist(playerLeftWristCombo.Text);
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.TeamLeftWrist = model.PlayerModel.GetWrist(playerLeftWristCombo.Text);
            }
        }

        private void playerRightWristCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.RightWrist = model.PlayerModel.GetWrist(playerRightWristCombo.Text);
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.TeamRightWrist = model.PlayerModel.GetWrist(playerRightWristCombo.Text);
            }
        }

        private void playerLeftHandCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.LeftHand = model.PlayerModel.GetGloves(playerLeftHandCombo.Text);
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.TeamLeftHand = model.PlayerModel.GetGloves(playerLeftHandCombo.Text);
            }
        }

        private void playerRightHandCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.RightHand = model.PlayerModel.GetGloves(playerRightHandCombo.Text);
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.TeamRightHand = model.PlayerModel.GetGloves(playerRightHandCombo.Text);
            }
        }

        private void playerLeftKneeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.KneeLeft = playerLeftKneeCombo.SelectedIndex;
            }
        }

        private void playerRightKneeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.KneeRight = playerRightKneeCombo.SelectedIndex;
            }
        }

        private void playerLeftAnkleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.AnkleLeft = model.PlayerModel.GetAnkle(playerLeftAnkleCombo.Text);
            }
        }

        private void playerRightAnkleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.AnkleRight = model.PlayerModel.GetAnkle(playerRightAnkleCombo.Text);
            }
        }

        private void playerNasalStripCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.NasalStrip = playerNasalStripCombo.SelectedIndex;
            }
        }

        private void playerHelmetStyleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.Helmet = model.PlayerModel.GetHelmet(playerHelmetStyleCombo.Text);
        }

        private void playerFaceMaskCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.FaceMask = model.PlayerModel.GetFaceMask(playerFaceMaskCombo.Text);
            }
        }

        private void playerLeftShoeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.LeftShoe = model.PlayerModel.GetShoe(playerLeftShoeCombo.Text);
            }
        }

        private void playerRightShoeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.RightShoe = model.PlayerModel.GetShoe(playerRightShoeCombo.Text);
            }
        }

        private void playerLeftSleeve_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.SleevesLeft = model.PlayerModel.GetSleeve(playerLeftSleeve.Text);
            }
        }

        private void playerRightSleeve_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.SleevesRight = model.PlayerModel.GetSleeve(playerRightSleeve.Text);
                else if (model.MadVersion < MaddenFileVersion.Ver2019)
                    model.PlayerModel.CurrentPlayerRecord.TempSleeves = model.PlayerModel.GetSleeve(playerRightSleeve.Text);
            }
        }

        private void playerSockHeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.SockHeight = (int)playerSockHeight.SelectedIndex;
        }

        private void playerUndershirt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.JerseyTucked = playerUndershirt.SelectedIndex;
        }
        private void playerLeftThighCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThighLeft = playerLeftThighCombo.SelectedIndex;
        }
        private void playerRightThighCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.ThighRight = playerRightThighCombo.SelectedIndex;
        }
        private void handwarmersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.HandWarmer = HandWarmerscomboBox.SelectedIndex;
        }
        private void towelsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
             model.PlayerModel.CurrentPlayerRecord.PlayerTowel = TowelscomboBox.SelectedIndex;
        }
            
        



        #endregion

        #region Legacy Specific Controls

        private void T_Sleeves_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TempSleeves = model.PlayerModel.GetSleeve(T_Sleeves_Combo.Text);
        }

        private void T_LeftElbow_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TeamLeftElbow = model.PlayerModel.GetElbow(T_LeftElbow_Combo.Text);
        }

        private void T_RightElbow_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TeamRightElbow = model.PlayerModel.GetElbow(T_RightElbow_Combo.Text);
        }

        private void T_LeftHand_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TeamLeftHand = model.PlayerModel.GetGloves(T_LeftHand_Combo.Text);
        }

        private void T_RightHand_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TeamRightHand = model.PlayerModel.GetGloves(T_RightHand_Combo.Text);
        }

        private void T_LeftWrist_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TeamLeftWrist = model.PlayerModel.GetGloves(T_LeftWrist_Combo.Text);
        }

        private void T_RightWrist_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.TeamRightWrist = model.PlayerModel.GetGloves(T_RightWrist_Combo.Text);
        }

        #endregion

        #endregion

        #endregion

        #region Injury

        private void playerAddInjuryButton_Click(object sender, EventArgs e)
        {
            InjuryRecord injRec = null;
            try
            {
                injRec = model.PlayerModel.CreateNewInjuryRecord();
            }
            catch (ApplicationException err)
            {
                MessageBox.Show("Error adding Injury\r\n" + err.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            injRec.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
            injRec.TeamId = model.PlayerModel.CurrentPlayerRecord.TeamId;
            injRec.InjuryLength = 0;
            injRec.IR = false;
            injRec.InjuryType = 0;
            if (model.MadVersion >= MaddenFileVersion.Ver2019)
            {
                injRec.InjurySeverity = 5;
            }

            isInitialising = true;
            playerInjuryCombo.SelectedIndex = -1;
            LoadPlayerInfo(model.PlayerModel.CurrentPlayerRecord);
            isInitialising = false;
        }

        private void playerRemoveInjuryButton_Click(object sender, EventArgs e)
        {
            //Mark the record for deletion
            model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).SetDeleteFlag(true);

            isInitialising = true;
            LoadPlayerInfo(model.PlayerModel.CurrentPlayerRecord);
            isInitialising = false;
        }

        private void playerInjuryCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion < MaddenFileVersion.Ver2019)
                    model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).InjuryType = playerInjuryCombo.SelectedIndex;
                else
                {
                    int test = model.PlayerModel.GetInjury(playerInjuryCombo.Text);
                    model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).InjuryType = test;
                }
            }
        }

        private void playerInjuryReserve_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).IR = playerInjuryReserve.Checked;
            }
        }

        private void playerInjuryLength_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).InjuryLength = (int)playerInjuryLength.Value;
                injuryLengthDescriptionTextBox.Text = model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).LengthDescription;
            }
        }

        private void playerInjurySevere_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                {
                    model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).InjurySeverity = (int)playerInjurySevere.Value;
                }
        }

        private void playerInjuryReturn_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                if (model.MadVersion >= MaddenFileVersion.Ver2019)
                {
                    model.PlayerModel.GetPlayersInjuryRecord(model.PlayerModel.CurrentPlayerRecord.PlayerId).InjuryReturn = (int)playerInjuryReturn.Value;
                }
        }

        #endregion

        #region Player Contracts

        #region Contract/Salary  Functions

        public void ClearPlayerSalaryDetails()
        {
            PlayerContract_Panel.Enabled = false;
            PlayerContractDetails_Panel.Enabled = false;
            MiscSalary_Panel.Enabled = false;

            playerTotalSalary.Value = 0;
            playerTotalBonus.Value = 0;
            playerContractLength.Value = 0;
            playerContractYearsLeft.Value = 0;
            for (int c = 0; c < 2; c++)
            {
                string name = "Player";
                if (c == 0)
                    name += "Bonus";
                else name += "Salary";
                for (int m = 0; m < 7; m++)
                {
                    string newname = name + m;
                    NumericUpDown updown = this.Controls.Find(newname, true).First() as NumericUpDown;
                    updown.BackColor = SystemColors.Window;
                    updown.Value = 0;
                }
            }
            /*
            PlayerBonus0.Value = 0;
            PlayerBonus1.Value = 0;
            PlayerBonus2.Value = 0;
            PlayerBonus3.Value = 0;
            PlayerBonus4.Value = 0;
            PlayerBonus5.Value = 0;
            PlayerBonus6.Value = 0;
            PlayerSalary0.Value = 0;
            PlayerSalary1.Value = 0;
            PlayerSalary2.Value = 0;
            PlayerSalary3.Value = 0;
            PlayerSalary4.Value = 0;
            PlayerSalary5.Value = 0;
            PlayerSalary6.Value = 0;
            */

            PlayerCapHit.Text = "";
            TeamSalary.Text = "";
            CalcTeamSalary.Text = "";
            TeamCapRoom.Text = "";
            TeamSalaryRank.Text = "";
            Top5.Value = 0;
            Top10.Value = 0;
            LeagueAVG.Value = 0;
            Top5AVG.Value = 0;
            Top10AVG.Value = 0;
            LeagueContAVG.Value = 0;
        }

        private void LoadPlayerSalaries(PlayerRecord record)
        {
            ClearPlayerSalaryDetails();
            // Not on a current team so return
            if (record.TeamId == 1009 || record.TeamId == 1010 || record.TeamId == 1014 || record.TeamId == 1015 || record.TeamId == 1023)
                return;
            PlayerContract_Panel.Enabled = true;
            MiscSalary_Panel.Enabled = true;
            TeamRecord teamRecord = model.TeamModel.GetTeamRecord(record.TeamId);

            if (model.FileType == MaddenFileType.Franchise || Madden19)
            {
                // Set total salary and total bonus from their yearly totals
                record.FixYearlyContract();

                PlayerContractDetails_Panel.Enabled = true;
                PlayerBonus0.Value = (decimal)record.Bonus0 / 100;
                PlayerBonus1.Value = (decimal)record.Bonus1 / 100;
                PlayerBonus2.Value = (decimal)record.Bonus2 / 100;
                PlayerBonus3.Value = (decimal)record.Bonus3 / 100;
                PlayerBonus4.Value = (decimal)record.Bonus4 / 100;
                PlayerBonus5.Value = (decimal)record.Bonus5 / 100;
                PlayerBonus6.Value = (decimal)record.Bonus6 / 100;
                PlayerSalary0.Value = (decimal)record.Salary0 / 100;
                PlayerSalary1.Value = (decimal)record.Salary1 / 100;
                PlayerSalary2.Value = (decimal)record.Salary2 / 100;
                PlayerSalary3.Value = (decimal)record.Salary3 / 100;
                PlayerSalary4.Value = (decimal)record.Salary4 / 100;
                PlayerSalary5.Value = (decimal)record.Salary5 / 100;
                PlayerSalary6.Value = (decimal)record.Salary6 / 100;

                if (!Madden19)
                {
                    UseLeagueMinimum_Checkbox.Enabled = true;
                    LeagueMinimum.Enabled = true;
                    Penalty0.Value = (decimal)teamRecord.SalaryCapPenalty0 / 100;
                    Penalty1.Value = (decimal)teamRecord.SalaryCapPenalty1 / 100;
                }
            }
            else
            {
                if (record.YearlySalary == null)
                    record.SetContract(false, false, 5);
                PlayerBonus0.Value = (decimal)record.YearlyBonus[0] / 100;
                PlayerBonus1.Value = (decimal)record.YearlyBonus[1] / 100;
                PlayerBonus2.Value = (decimal)record.YearlyBonus[2] / 100;
                PlayerBonus3.Value = (decimal)record.YearlyBonus[3] / 100;
                PlayerBonus4.Value = (decimal)record.YearlyBonus[4] / 100;
                PlayerBonus5.Value = (decimal)record.YearlyBonus[5] / 100;
                PlayerBonus6.Value = (decimal)record.YearlyBonus[6] / 100;
                PlayerSalary0.Value = (decimal)record.YearlySalary[0] / 100;
                PlayerSalary1.Value = (decimal)record.YearlySalary[1] / 100;
                PlayerSalary2.Value = (decimal)record.YearlySalary[2] / 100;
                PlayerSalary3.Value = (decimal)record.YearlySalary[3] / 100;
                PlayerSalary4.Value = (decimal)record.YearlySalary[4] / 100;
                PlayerSalary5.Value = (decimal)record.YearlySalary[5] / 100;
                PlayerSalary6.Value = (decimal)record.YearlySalary[6] / 100;
            }

            playerTotalSalary.Value = (decimal)record.TotalSalary / 100;
            playerTotalBonus.Value = (decimal)record.TotalBonus / 100;
            playerContractLength.Value = (int)record.ContractLength;
            playerContractYearsLeft.Value = (int)record.ContractYearsLeft;
            if (record.YearsPro == 0)
                ContractYearlyIncrease.Value = 5;
            else ContractYearlyIncrease.Value = 5;

            playerPreviousCon.Value = record.PreviousContractLength;
            playerPreviousTotal.Value = (decimal)record.PreviousTotalSalary / 100;
            playerPreviousBonus.Value = (decimal)record.PreviousSigningBonus / 100;

            if (record.ContractYearsLeft > 0)
            {
                int conyear = record.ContractLength - record.ContractYearsLeft;
                string name = "Player";
                for (int c = 0; c < 2; c++)
                {
                    string newname = "";
                    if (c == 0)
                        newname = name + "Salary" + conyear;
                    else newname = name + "Bonus" + conyear;
                    NumericUpDown updown = this.Controls.Find(newname, true).First() as NumericUpDown;
                    updown.BackColor = Color.Yellow;
                }
            }

            PlayerCapHit.Text = Math.Round((double)record.GetCurrentSalary() / 100, 2).ToString();

            TeamSalary.Text = "" + ((double)teamRecord.Salary / 100.0).ToString();
            if (CalcTeamSalary_Checkbox.Checked)
            {
                int teamcalc = GetTeamSalaryCap(record.TeamId);
                CalcTeamSalary.Text = Math.Round((double)teamcalc / 100, 2).ToString();
                TeamCapRoom.Text = (SalaryCap.Value - Convert.ToDecimal(CalcTeamSalary.Text)).ToString();
            }
            else TeamCapRoom.Text = Math.Round((double)SalaryCap.Value - (double)teamRecord.Salary / 100, 2).ToString();

            TeamNeeds(record);
            LoadPositionSalaries(record);
            LoadFreeAgents(record);
            GetTeamSalaries();
        }

        private void GetTeamSalaries()
        {
            teamsalaries.Clear();
            List<int> topsals = new List<int>();
            TeamRecord comp = model.TeamModel.GetTeamRecord(model.PlayerModel.CurrentPlayerRecord.TeamId);

            foreach (TeamRecord tr in model.TableModels[EditorModel.TEAM_TABLE].GetRecords())
            {
                if (tr.Deleted)
                    continue;
                TeamRecord rec = (TeamRecord)tr;

                if (rec.TeamId == 1009 || rec.TeamId == 1010 || rec.TeamId == 1014 || rec.TeamId == 1015 || rec.TeamId == 1023)
                    continue;
                else if (SalaryRankCombo.SelectedIndex == 2 && comp.DivisionId != rec.DivisionId)
                    continue;
                else if (SalaryRankCombo.SelectedIndex == 1 && comp.ConferenceId != rec.ConferenceId)
                    continue;
                else teamsalaries.Add(rec.TeamId, GetTeamSalaryCap(rec.TeamId));
            }
            foreach (KeyValuePair<int, int> pair in teamsalaries)
                topsals.Add(pair.Value);
            topsals.Sort(delegate (int x, int y)
            { return y.CompareTo(x); });

            int rank = 1;
            bool tie = false;
            for (int c = 0; c < topsals.Count; c++)
            {
                if (c < topsals.Count - 1)
                    if (topsals[c + 1] == topsals[c])
                        tie = true;
                    else tie = false;

                if (topsals[c] == teamsalaries[model.PlayerModel.CurrentPlayerRecord.TeamId])
                    break;
                else if (tie)
                    rank += 2;
                else rank++;
            }

            string text = "";
            if (tie)
                text += "T";
            text += rank.ToString();
            TeamSalaryRank.Text = text;
        }

        public int GetTeamSalaryCap(int TeamId)
        {
            int tempcap = 0;
            List<int> PlayerIDs = new List<int>();

            try
            {
                foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                {
                    if (record.Deleted)
                        continue;
                    PlayerRecord rec = (PlayerRecord)record;

                    if (rec.TeamId == TeamId)
                    {
                        bool valid = true;

                        if (model.FileType == MaddenFileType.Franchise)
                        {
                            foreach (TableRecordModel injrec in model.TableModels[EditorModel.INJURY_TABLE].GetRecords())
                            {
                                InjuryRecord playerinjury = (InjuryRecord)injrec;
                                if (playerinjury.PlayerId == rec.PlayerId)
                                {
                                    if (playerinjury.IR)
                                        valid = false;
                                }
                            }
                        }

                        if (valid && !PlayerIDs.Contains(rec.PlayerId))
                            PlayerIDs.Add(rec.PlayerId);
                        // if player is not on injured reserve count his salary for total team salary
                        if (model.FileType == MaddenFileType.Franchise && valid)
                            tempcap += rec.CurrentSalary;
                        else if (model.FileType == MaddenFileType.Roster)
                        {
                            if (rec.ContractYearsLeft > 0)
                            {
                                if (rec.YearlySalary == null)
                                    rec.SetContract(true, false, 35);
                                tempcap += (int)rec.GetCurrentSalary();
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return tempcap;
        }

        #endregion


        #region Player Contract Controls

        private void SubmitContract_Click(object sender, EventArgs e)
        {
            isInitialising = true;
            //CalculateCapHit(model.PlayerModel.CurrentPlayerRecord, ContractYearlyIncrease.Value, true);
            model.PlayerModel.CurrentPlayerRecord.ContractLength = (int)playerContractLength.Value;
            model.PlayerModel.CurrentPlayerRecord.ContractYearsLeft = (int)playerContractYearsLeft.Value;
            model.PlayerModel.CurrentPlayerRecord.TotalSalary = (int)(playerTotalSalary.Value * 100);
            model.PlayerModel.CurrentPlayerRecord.TotalBonus = (int)(playerTotalBonus.Value * 100);
            model.PlayerModel.CurrentPlayerRecord.SetContract(false, true, (double)ContractYearlyIncrease.Value);
            LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
            isInitialising = false;
        }

        private void UseActualNFLSalaryCap_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                SalaryCap.Value = 0;
                if (UseActualNFLSalaryCap_Checkbox.Checked)
                {
                    int curyear = (int)CurrentYear.Value;
                    if (model.LeagueCap.ContainsKey(curyear))
                        SalaryCap.Value = (decimal)model.LeagueCap[curyear];
                }
                else
                {
                    if (model.FileType == MaddenFileType.Franchise)
                        SalaryCap.Value = Math.Round((decimal)model.SalaryCapModel.SalaryCap / 1000000, 2);
                }
                isInitialising = false;
            }
        }

        private void playerContractLength_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                //Before we set it make sure its ok
                if (playerContractLength.Value < playerContractYearsLeft.Value)
                    playerContractYearsLeft.Value = playerContractLength.Value;

                if (UseLeagueMinimum_Checkbox.Checked)
                {
                    int holder = (int)(LeagueMinimum.Value * playerContractLength.Value * 100);
                    playerTotalSalary.Minimum = (decimal)holder / 100;
                    if ((double)(LeagueMinimum.Value * playerContractLength.Value * 100) > holder)
                        playerTotalSalary.Minimum += (decimal).01;
                    if (playerTotalSalary.Value < playerTotalSalary.Minimum)
                        playerTotalSalary.Value = playerTotalSalary.Minimum;
                }
            }
        }

        private void playerContractYearsLeft_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                //Before we set it make sure its ok
                if (playerContractYearsLeft.Value > playerContractLength.Value)
                {
                    playerContractLength.Value = playerContractYearsLeft.Value;
                }
            }
        }

        private void playerSigningBonus_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.TotalBonus = (int)playerTotalBonus.Value * 100;
            }
        }

        private void playerTotalSalary_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                playerTotalSalary.Minimum = 0;

                if (playerTotalSalary.Value < playerTotalSalary.Minimum)                // dont need this but...             
                    playerTotalSalary.Value = playerTotalSalary.Minimum;

                decimal temp = playerTotalSalary.Value;
                int holder = (int)(playerTotalSalary.Value * 100);
                isInitialising = true;
                playerTotalSalary.Value = (decimal)holder / 100;
                if (temp * 100 > holder)
                {
                    playerTotalSalary.Value += (decimal).01;
                }

                if (UseLeagueMinimum_Checkbox.Checked)
                {
                    holder = (int)(LeagueMinimum.Value * playerContractLength.Value * 100);
                    playerTotalSalary.Minimum = (decimal)holder / 100;
                    if ((double)(LeagueMinimum.Value * playerContractLength.Value * 100) > holder)
                        playerTotalSalary.Minimum += (decimal).01;
                    if (playerTotalSalary.Value < playerTotalSalary.Minimum)
                    {
                        playerTotalSalary.Value = playerTotalSalary.Minimum;
                    }
                }
                isInitialising = false;
            }
        }

        private void SalaryCap_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                if (model.FileType == MaddenFileType.Franchise)
                    model.SalaryCapModel.SalaryCap = (int)(SalaryCap.Value * 1000000);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void Penalty0_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.TeamModel.GetTeamRecord(model.PlayerModel.CurrentPlayerRecord.TeamId).SalaryCapPenalty0 = (int)(Penalty0.Value * 100);
            }
        }

        private void Penalty1_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.TeamModel.GetTeamRecord(model.PlayerModel.CurrentPlayerRecord.TeamId).SalaryCapPenalty1 = (int)(Penalty1.Value * 100);
            }
        }

        #region Specific Years Salary and Bonus

        private void PlayerSalary0_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary0 = (int)(PlayerSalary0.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerSalary1_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary1 = (int)(PlayerSalary1.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerSalary2_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary2 = (int)(PlayerSalary2.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerSalary3_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary3 = (int)(PlayerSalary3.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerSalary4_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary4 = (int)(PlayerSalary4.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerSalary5_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary5 = (int)(PlayerSalary5.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerSalary6_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Salary6 = (int)(PlayerSalary6.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus0_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus0 = (int)(PlayerBonus0.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus1_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus1 = (int)(PlayerBonus1.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus2_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus2 = (int)(PlayerBonus2.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus3_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus3 = (int)(PlayerBonus3.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus4_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus4 = (int)(PlayerBonus4.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus5_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus5 = (int)(PlayerBonus5.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void PlayerBonus6_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                isInitialising = true;
                model.PlayerModel.CurrentPlayerRecord.Bonus6 = (int)(PlayerBonus6.Value * 100);
                LoadPlayerSalaries(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Player Stats

        public void InitStatsYear(PlayerRecord record)
        {
            if (!isInitialising)
                return;

            //  Get current Season and week
            int currentyear = model.FranchiseTime.Year;

            statsyear.Items.Clear();
            statsyear.Items.Add("Career");
            int endyear = currentyear + baseyear;
            int startyear = endyear - record.YearsPro;
            for (int t = endyear; t > startyear - 1; t--)
                statsyear.Items.Add(t);

            //  Set for last selected year if it is available

            if (selectedyear != -1 && statsyear.Items.Contains(selectedyear))
                statsyear.SelectedIndex = selectedyear - baseyear + 1;
            else statsyear.SelectedIndex = 0;
        }

        public void LoadPlayerGamesPlayed(PlayerRecord record, int index, bool career)
        {
            CareerGamesPlayedRecord careergamesplayed = model.PlayerModel.GetPlayersGamesCareer(record.PlayerId);
            SeasonGamesPlayedRecord seasongamesplayed = model.PlayerModel.GetSeasonGames(record.PlayerId, year);

            // Controls
            GamesPlayedPanel.Enabled = false;
            gamesplayed.Enabled = true;
            gamesplayed.Value = 0;
            gamesstarted.Value = 0;
            DownsPlayed.Value = 0;

            if (model.MadVersion == MaddenFileVersion.Ver2004)
            {
                gamesstarted.Enabled = false;
                DownsPlayed.Enabled = false;
            }
            else
            {
                gamesstarted.Enabled = true;
                DownsPlayed.Enabled = true;
            }

            if (career && careergamesplayed != null)
            {
                GamesPlayedPanel.Enabled = true;

                if (model.MadVersion == MaddenFileVersion.Ver2004)
                {
                    gamesplayed.Value = careergamesplayed.GamesPlayed04;
                }
                else
                {
                    gamesplayed.Value = careergamesplayed.GamesPlayed;
                    gamesstarted.Value = careergamesplayed.GamesStarted;
                    DownsPlayed.Value = careergamesplayed.DownsPlayed;
                }
            }
            else if (!career && seasongamesplayed != null)
            {
                GamesPlayedPanel.Enabled = true;

                gamesplayed.Value = seasongamesplayed.GamesPlayed;

                if (model.MadVersion != MaddenFileVersion.Ver2004)
                {
                    gamesstarted.Value = seasongamesplayed.GamesStarted;
                    DownsPlayed.Value = seasongamesplayed.DownsPlayed;
                }
            }
        }

        public void LoadPlayerPuntKick(PlayerRecord record, int index, bool career)
        {
            CareerPuntKickRecord careerpuntkick = model.PlayerModel.GetPlayersCareerPuntKick(record.PlayerId);
            SeasonPuntKickRecord seasonpuntkick = model.PlayerModel.GetPuntKick(record.PlayerId, year);

            KickPuntPanel.Enabled = false;

            fga.Value = 0;
            fgm.Value = 0;
            fgbl.Value = 0;
            fgl.Value = 0;
            xpa.Value = 0;
            xpm.Value = 0;
            xpb.Value = 0;
            fga_129.Value = 0;
            fga_3039.Value = 0;
            fga_4049.Value = 0;
            fga_50.Value = 0;
            fgm_129.Value = 0;
            fgm_3039.Value = 0;
            fgm_4049.Value = 0;
            fgm_50.Value = 0;
            puntatt.Value = 0;
            puntyds.Value = 0;
            puntlong.Value = 0;
            puntin20.Value = 0;
            puntny.Value = 0;
            punttb.Value = 0;
            puntblk.Value = 0;
            touchbacks.Value = 0;
            kickoffs.Value = 0;

            if (career && careerpuntkick != null)
            {
                KickPuntPanel.Enabled = true;

                fga.Value = careerpuntkick.Fga;
                fgm.Value = careerpuntkick.Fgm;
                fgbl.Value = careerpuntkick.Fgbl;
                fgl.Value = careerpuntkick.Fgl;
                xpa.Value = careerpuntkick.Xpa;
                xpm.Value = careerpuntkick.Xpm;
                xpb.Value = careerpuntkick.Xpb;
                fga_129.Value = careerpuntkick.Fga_129;
                fga_3039.Value = careerpuntkick.Fga_3039;
                fga_4049.Value = careerpuntkick.Fga_4049;
                fga_50.Value = careerpuntkick.Fga_50;
                fgm_129.Value = careerpuntkick.Fgm_129;
                fgm_3039.Value = careerpuntkick.Fgm_3039;
                fgm_4049.Value = careerpuntkick.Fgm_4049;
                fgm_50.Value = careerpuntkick.Fgm_50;
                puntatt.Value = careerpuntkick.Puntatt;
                puntblk.Value = careerpuntkick.Puntblk;
                puntin20.Value = careerpuntkick.Puntin20;
                puntlong.Value = careerpuntkick.Puntlong;
                puntny.Value = careerpuntkick.Puntny;
                punttb.Value = careerpuntkick.Punttb;
                puntyds.Value = careerpuntkick.Puntyds;
                touchbacks.Value = careerpuntkick.Touchbacks;
                kickoffs.Value = careerpuntkick.Kickoffs;
            }

            else if (!career && seasonpuntkick != null)
            {
                KickPuntPanel.Enabled = true;

                fga.Value = seasonpuntkick.Fga;
                fgm.Value = seasonpuntkick.Fgm;
                fgbl.Value = seasonpuntkick.Fgbl;
                fgl.Value = seasonpuntkick.Fgl;
                xpa.Value = seasonpuntkick.Xpa;
                xpm.Value = seasonpuntkick.Xpm;
                xpb.Value = seasonpuntkick.Xpb;
                fga_129.Value = seasonpuntkick.Fga_129;
                fga_3039.Value = seasonpuntkick.Fga_3039;
                fga_4049.Value = seasonpuntkick.Fga_4049;
                fga_50.Value = seasonpuntkick.Fga_50;
                fgm_129.Value = seasonpuntkick.Fgm_129;
                fgm_3039.Value = seasonpuntkick.Fgm_3039;
                fgm_4049.Value = seasonpuntkick.Fgm_4049;
                fgm_50.Value = seasonpuntkick.Fgm_50;
                puntatt.Value = seasonpuntkick.Puntatt;
                puntblk.Value = seasonpuntkick.Puntblk;
                puntin20.Value = seasonpuntkick.Puntin20;
                puntlong.Value = seasonpuntkick.Puntlong;
                puntny.Value = seasonpuntkick.Puntny;
                punttb.Value = seasonpuntkick.Punttb;
                puntyds.Value = seasonpuntkick.Puntyds;
                touchbacks.Value = seasonpuntkick.Touchbacks;
                kickoffs.Value = seasonpuntkick.Kickoffs;
            }

            else return;
        }

        public void LoadPlayerOffense(PlayerRecord record, int index, bool career)
        {
            CareerStatsOffenseRecord careeroffensestats = model.PlayerModel.GetPlayersOffenseCareer(record.PlayerId);
            SeasonStatsOffenseRecord seasonoffense = model.PlayerModel.GetOffStats(record.PlayerId, year);

            // Set controls
            OffensePanel.Enabled = false;
            if (model.MadVersion >= MaddenFileVersion.Ver2007)
            {
                comebacks.Enabled = true;
                Firstdowns.Enabled = true;
            }
            else
            {
                comebacks.Enabled = false;
                Firstdowns.Enabled = false;
            }

            pass_att.Value = 0;
            pass_comp.Value = 0;
            pass_yds.Value = 0;
            pass_int.Value = 0;
            pass_long.Value = 0;
            pass_tds.Value = 0;
            receiving_recs.Value = 0;
            receiving_drops.Value = 0;
            receiving_tds.Value = 0;
            receiving_yds.Value = 0;
            receiving_yac.Value = 0;
            receiving_long.Value = 0;
            fumbles.Value = 0;
            rushingattempts.Value = 0;
            rushingyards.Value = 0;
            rushing_tds.Value = 0;
            rushing_long.Value = 0;
            rushing_yac.Value = 0;
            rushing_20.Value = 0;
            rushing_bt.Value = 0;
            comebacks.Value = 0;
            Firstdowns.Value = 0;

            //  Set career stats
            if (career && careeroffensestats != null)
            {
                OffensePanel.Enabled = true;

                pass_att.Value = (int)careeroffensestats.Pass_att;
                pass_comp.Value = (int)careeroffensestats.Pass_comp;
                pass_yds.Value = (int)careeroffensestats.Pass_yds;
                pass_int.Value = (int)careeroffensestats.Pass_int;
                pass_long.Value = (int)careeroffensestats.Pass_long;
                pass_tds.Value = (int)careeroffensestats.Pass_tds;
                receiving_recs.Value = (int)careeroffensestats.Receiving_recs;
                receiving_drops.Value = (int)careeroffensestats.Receiving_drops;
                receiving_tds.Value = (int)careeroffensestats.Receiving_tds;
                receiving_yds.Value = (int)careeroffensestats.Receiving_yards;
                receiving_yac.Value = (int)careeroffensestats.Receiving_yac;
                receiving_long.Value = (int)careeroffensestats.Receiving_long;
                fumbles.Value = (int)careeroffensestats.Fumbles;
                rushingattempts.Value = (int)careeroffensestats.RushingAttempts;
                rushingyards.Value = (int)careeroffensestats.RushingYards;
                rushing_tds.Value = (int)careeroffensestats.Rushing_tds;
                rushing_long.Value = (int)careeroffensestats.Rushing_long;
                rushing_yac.Value = (int)careeroffensestats.Rushing_yac;
                rushing_20.Value = (int)careeroffensestats.Rushing_20;
                rushing_bt.Value = (int)careeroffensestats.Rushing_bt;

                if (model.MadVersion >= MaddenFileVersion.Ver2007)
                {
                    comebacks.Value = (int)careeroffensestats.Comebacks;
                    Firstdowns.Value = (int)careeroffensestats.FirstDowns;
                }
            }

            // Set season stats
            else if (seasonoffense != null && !career)
            {
                OffensePanel.Enabled = true;

                pass_att.Value = (int)seasonoffense.SeaPassAtt;
                pass_comp.Value = (int)seasonoffense.SeaComp;
                pass_yds.Value = (int)seasonoffense.SeaPassYds;
                pass_int.Value = (int)seasonoffense.SeaPassInt;
                pass_long.Value = (int)seasonoffense.SeaPassLong;
                pass_tds.Value = (int)seasonoffense.SeaPassTd;
                receiving_recs.Value = (int)seasonoffense.SeaRec;
                receiving_drops.Value = (int)seasonoffense.SeaDrops;
                receiving_tds.Value = (int)seasonoffense.SeaRecTd;
                receiving_yds.Value = (int)seasonoffense.SeaRecYds;
                receiving_yac.Value = (int)seasonoffense.SeaRecYac;
                receiving_long.Value = (int)seasonoffense.SeaRecLong;
                fumbles.Value = (int)seasonoffense.SeaFumbles;
                rushingattempts.Value = (int)seasonoffense.SeaRushAtt;
                rushingyards.Value = (int)seasonoffense.SeaRushYds;
                rushing_tds.Value = (int)seasonoffense.SeaRushTd;
                rushing_long.Value = (int)seasonoffense.SeaRushLong;
                rushing_yac.Value = (int)seasonoffense.SeaRushYac;
                rushing_20.Value = (int)seasonoffense.SeaRush20;
                rushing_bt.Value = (int)seasonoffense.SeaRushBtk;

                if (model.MadVersion >= MaddenFileVersion.Ver2007)
                {
                    comebacks.Value = (int)seasonoffense.SeaComebacks;
                    Firstdowns.Value = (int)seasonoffense.SeaFirstDowns;
                }
            }
        }

        public void LoadPlayerDefense(PlayerRecord record, int index, bool career)
        {
            CareerStatsDefenseRecord careerdefensestats = model.PlayerModel.GetPlayersDefenseCareer(record.PlayerId);
            SeasonStatsDefenseRecord seasondefensestats = model.PlayerModel.GetDefenseStats(record.PlayerId, year);

            DefensePanel.Enabled = false;

            passesdefended.Value = 0;
            tackles.Value = 0;
            tacklesforloss.Value = 0;
            sacks.Value = 0;
            blocks.Value = 0;
            fumblesrecovered.Value = 0;
            fumblesforced.Value = 0;
            fumbleyards.Value = 0;
            fumbles_td.Value = 0;
            safeties.Value = 0;
            def_int.Value = 0;
            int_td.Value = 0;
            int_yards.Value = 0;
            int_long.Value = 0;
            CatchesAllowed.Value = 0;
            BigHits.Value = 0;

            if (model.MadVersion >= MaddenFileVersion.Ver2007)
            {
                CatchesAllowed.Enabled = true;
                BigHits.Enabled = true;
            }
            else
            {
                CatchesAllowed.Enabled = false;
                BigHits.Enabled = false;
            }

            if (career && careerdefensestats != null)
            {
                DefensePanel.Enabled = true;

                passesdefended.Value = careerdefensestats.PassesDefended;
                tackles.Value = careerdefensestats.Tackles;
                tacklesforloss.Value = careerdefensestats.TacklesForLoss;
                sacks.Value = careerdefensestats.Sacks;
                blocks.Value = careerdefensestats.Blocks;
                safeties.Value = careerdefensestats.Safeties;
                fumblesrecovered.Value = careerdefensestats.FumblesRecovered;
                fumblesforced.Value = careerdefensestats.FumblesForced;
                fumbleyards.Value = careerdefensestats.FumbleYards;
                fumbles_td.Value = careerdefensestats.Fumbles_td;
                def_int.Value = careerdefensestats.Def_int;
                int_long.Value = careerdefensestats.Int_long;
                int_td.Value = careerdefensestats.Int_td;
                int_yards.Value = careerdefensestats.Int_yards;

                if (model.MadVersion >= MaddenFileVersion.Ver2007)
                {
                    BigHits.Value = careerdefensestats.BigHits;
                    CatchesAllowed.Value = careerdefensestats.CatchesAllowed;
                }
            }

            else if (!career && seasondefensestats != null)
            {
                DefensePanel.Enabled = true;

                passesdefended.Value = seasondefensestats.PassesDefended;
                tackles.Value = seasondefensestats.Tackles;
                tacklesforloss.Value = seasondefensestats.TacklesForLoss;
                sacks.Value = seasondefensestats.Sacks;
                blocks.Value = seasondefensestats.Blocks;
                safeties.Value = seasondefensestats.Safeties;
                fumblesrecovered.Value = seasondefensestats.FumblesRecovered;
                fumblesforced.Value = seasondefensestats.FumblesForced;
                fumbleyards.Value = seasondefensestats.FumbleYards;
                fumbles_td.Value = seasondefensestats.FumbleTDS;
                def_int.Value = seasondefensestats.Interceptions;
                int_long.Value = seasondefensestats.InterceptionLong;
                int_td.Value = seasondefensestats.InterceptionTDS;
                int_yards.Value = seasondefensestats.InterceptionYards;

                if (model.MadVersion >= MaddenFileVersion.Ver2007)
                {
                    BigHits.Value = seasondefensestats.BigHits;
                    CatchesAllowed.Value = seasondefensestats.CatchesAllowed;
                }
            }
        }

        public void LoadPlayerOL(PlayerRecord record, int index, bool career)
        {
            CareerStatsOffensiveLineRecord careerOLstats = model.PlayerModel.GetPlayersOLCareer(record.PlayerId);
            SeasonStatsOffensiveLineRecord seaOLstats = model.PlayerModel.GetOLstats(record.PlayerId, year);

            OLPanel.Enabled = false;
            pancakes.Value = 0;
            sacksallowed.Value = 0;

            if (career && careerOLstats != null)
            {
                OLPanel.Enabled = true;
                pancakes.Value = careerOLstats.Pancakes;
                sacksallowed.Value = careerOLstats.SacksAllowed;
            }

            else if (!career && seaOLstats != null)
            {
                OLPanel.Enabled = true;
                pancakes.Value = seaOLstats.Pancakes;
                sacksallowed.Value = seaOLstats.SacksAllowed;
            }
        }

        public void LoadPlayerPKReturn(PlayerRecord record, int index, bool career)
        {
            CareerPKReturnRecord careerpkreturn = model.PlayerModel.GetPlayersCareerPKReturn(record.PlayerId);
            SeasonPKReturnRecord seasonpkreturn = model.PlayerModel.GetPKReturn(record.PlayerId, year);

            ReturnPanel.Enabled = false;
            kra.Value = 0;
            kryds.Value = 0;
            krl.Value = 0;
            krtd.Value = 0;
            pra.Value = 0;
            pryds.Value = 0;
            prl.Value = 0;
            prtd.Value = 0;

            if (career && careerpkreturn != null)
            {
                ReturnPanel.Enabled = true;

                kra.Value = careerpkreturn.Kra;
                kryds.Value = careerpkreturn.Kryds;
                krl.Value = careerpkreturn.Krl;
                krtd.Value = careerpkreturn.Krtd;
                pra.Value = careerpkreturn.Pra;
                pryds.Value = careerpkreturn.Pryds;
                prl.Value = careerpkreturn.Prl;
                prtd.Value = careerpkreturn.Prtd;
            }

            else if (!career && seasonpkreturn != null)
            {
                ReturnPanel.Enabled = true;

                kra.Value = seasonpkreturn.Kra;
                kryds.Value = seasonpkreturn.Kryds;
                krl.Value = seasonpkreturn.Krl;
                krtd.Value = seasonpkreturn.Krtd;
                pra.Value = seasonpkreturn.Pra;
                pryds.Value = seasonpkreturn.Pryds;
                prl.Value = seasonpkreturn.Prl;
                prtd.Value = seasonpkreturn.Prtd;
            }
        }

        public void LoadPlayerStats(PlayerRecord record)
        {
            bool holder = isInitialising;
            isInitialising = true;

            bool career = false;
            if (selectedyear == -1)
                career = true;

            year = GetStatsYear();

            LoadPlayerGamesPlayed(record, year, career);
            LoadPlayerPuntKick(record, year, career);
            LoadPlayerOffense(record, year, career);
            LoadPlayerDefense(record, year, career);
            LoadPlayerOL(record, year, career);
            LoadPlayerPKReturn(record, year, career);

            if (holder)
                isInitialising = true;
            else isInitialising = false;
        }

        public int GetStatsYear()
        {
            if (statsyear.SelectedIndex > 0)
                year = (int)statsyear.SelectedItem - baseyear;

            return year;
        }

        #region Stats Functions

        private void AddStats_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AddStats_Combo.SelectedIndex == -1)
                return;

            if (!isInitialising)
            {
                isInitialising = true;

                if (AddStats_Combo.Text == "Games Played")
                {
                    if (statsyear.SelectedIndex == 0)
                    {
                        if (model.PlayerModel.GetPlayersGamesCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId) == null)
                        {
                            CareerGamesPlayedRecord cgp = (CareerGamesPlayedRecord)model.TableModels[EditorModel.CAREER_GAMES_PLAYED_TABLE].CreateNewRecord(true);
                            cgp.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        }
                    }
                    else if (statsyear.SelectedIndex != 0)
                    {
                        SeasonGamesPlayedRecord sgp = (SeasonGamesPlayedRecord)model.TableModels[EditorModel.SEASON_GAMES_PLAYED_TABLE].CreateNewRecord(true);
                        sgp.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        sgp.Season = GetStatsYear();
                    }
                }
                else if (AddStats_Combo.Text == "Offense")
                {
                    if (statsyear.SelectedIndex == 0)
                    {
                        if (model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId) == null)
                        {
                            CareerStatsOffenseRecord co = (CareerStatsOffenseRecord)model.TableModels[EditorModel.CAREER_STATS_OFFENSE_TABLE].CreateNewRecord(true);
                            co.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        }
                    }
                    else if (statsyear.SelectedIndex != 0)
                    {
                        SeasonStatsOffenseRecord so = (SeasonStatsOffenseRecord)model.TableModels[EditorModel.SEASON_STATS_OFFENSE_TABLE].CreateNewRecord(true);
                        so.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        so.Season = GetStatsYear();
                    }
                }
                else if (AddStats_Combo.Text == "Defense")
                {
                    if (statsyear.SelectedIndex == 0)
                    {
                        if (model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId) == null)
                        {
                            CareerStatsDefenseRecord cd = (CareerStatsDefenseRecord)model.TableModels[EditorModel.CAREER_STATS_DEFENSE_TABLE].CreateNewRecord(true);
                            cd.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        }
                    }
                    else if (statsyear.SelectedIndex != 0)
                    {
                        SeasonStatsDefenseRecord sd = (SeasonStatsDefenseRecord)model.TableModels[EditorModel.SEASON_STATS_DEFENSE_TABLE].CreateNewRecord(true);
                        sd.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        sd.Season = GetStatsYear();
                    }
                }
                else if (AddStats_Combo.Text == "Punt Kick")
                {
                    if (statsyear.SelectedIndex == 0)
                    {
                        if (model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId) == null)
                        {
                            CareerPuntKickRecord cpk = (CareerPuntKickRecord)model.TableModels[EditorModel.CAREER_STATS_KICKPUNT_TABLE].CreateNewRecord(true);
                            cpk.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        }
                    }
                    else if (statsyear.SelectedIndex != 0)
                    {
                        SeasonPuntKickRecord spk = (SeasonPuntKickRecord)model.TableModels[EditorModel.SEASON_STATS_KICKPUNT_TABLE].CreateNewRecord(true);
                        spk.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        spk.Season = GetStatsYear();
                    }
                }
                else if (AddStats_Combo.Text == "Returns")
                {
                    if (statsyear.SelectedIndex == 0)
                    {
                        if (model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId) == null)
                        {
                            CareerPKReturnRecord cr = (CareerPKReturnRecord)model.TableModels[EditorModel.CAREER_STATS_KICKPUNT_RETURN_TABLE].CreateNewRecord(true);
                            cr.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        }
                    }
                    else if (statsyear.SelectedIndex != 0)
                    {
                        SeasonPuntKickRecord sr = (SeasonPuntKickRecord)model.TableModels[EditorModel.SEASON_STATS_KICKPUNT_RETURN_TABLE].CreateNewRecord(true);
                        sr.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        sr.Season = GetStatsYear();
                    }
                }
                else if (AddStats_Combo.Text == "O-Line")
                {
                    if (statsyear.SelectedIndex == 0)
                    {
                        if (model.PlayerModel.GetPlayersOLCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId) == null)
                        {
                            CareerStatsOffensiveLineRecord col = (CareerStatsOffensiveLineRecord)model.TableModels[EditorModel.CAREER_STATS_OFFENSIVE_LINE_TABLE].CreateNewRecord(true);
                            col.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        }
                    }
                    else if (statsyear.SelectedIndex != 0)
                    {
                        SeasonStatsOffensiveLineRecord sol = (SeasonStatsOffensiveLineRecord)model.TableModels[EditorModel.SEASON_STATS_OFFENSIVE_LINE_TABLE].CreateNewRecord(true);
                        sol.PlayerId = model.PlayerModel.CurrentPlayerRecord.PlayerId;
                        sol.Season = GetStatsYear();
                    }
                }

                AddStats_Combo.SelectedIndex = -1;
                LoadPlayerInfo(model.PlayerModel.CurrentPlayerRecord);
                isInitialising = false;
            }
        }

        private void statsyear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                //  Get Stats for year or career as selected
                isInitialising = true;
                if (model.FileType == MaddenFileType.Franchise)
                {
                    if (statsyear.SelectedIndex == 0)
                        selectedyear = -1;
                    else selectedyear = (int)statsyear.SelectedItem;
                    LoadPlayerStats(model.PlayerModel.CurrentPlayerRecord);
                }
                isInitialising = false;
            }
        }

        #region Offense Stats

        private void pass_att_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_att = (int)pass_att.Value;
                else
                    model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaPassAtt = (int)pass_att.Value;
            }
        }

        private void pass_comp_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_comp = (int)pass_comp.Value;
                else
                    model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaComp = (int)pass_comp.Value;
            }
        }

        private void pass_yds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_yds = (int)pass_yds.Value;
                else
                    model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaPassYds = (int)pass_yds.Value;
            }
        }

        private void pass_tds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_tds = (int)pass_tds.Value;
                else
                    model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaPassTd = (int)pass_tds.Value;
            }
        }

        private void pass_int_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_int = (int)pass_int.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaPassInt = (int)pass_int.Value;
            }
        }

        private void pass_long_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_long = (int)pass_long.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaPassLong = (int)pass_long.Value;
            }
        }

        private void pass_sacked_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pass_sacked = (int)pass_sacked.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaSacked = (int)pass_sacked.Value;
            }
        }

        private void receiving_recs_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Receiving_recs = (int)receiving_recs.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRec = (int)receiving_recs.Value;
            }
        }

        private void receiving_yds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Receiving_yards = (int)receiving_yds.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRecYds = (int)receiving_yds.Value;
            }
        }

        private void receiving_tds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Receiving_tds = (int)receiving_tds.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRecTd = (int)receiving_tds.Value;
            }
        }

        private void receiving_drops_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Receiving_drops = (int)receiving_drops.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaDrops = (int)receiving_drops.Value;
            }
        }

        private void receiving_long_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Receiving_long = (int)receiving_long.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRecLong = (int)receiving_long.Value;
            }
        }

        private void receiving_yac_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Receiving_yac = (int)receiving_yac.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRecYac = (int)receiving_yac.Value;
            }
        }

        private void rushingattempts_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).RushingAttempts = (int)rushingattempts.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRushAtt = (int)rushingattempts.Value;
            }
        }

        private void rushingyards_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).RushingYards = (int)rushingyards.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRushYds = (int)rushingyards.Value;
            }
        }

        private void rushing_tds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Rushing_tds = (int)rushing_tds.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRushTd = (int)rushing_tds.Value;
            }
        }

        private void fumbles_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fumbles = (int)fumbles.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaFumbles = (int)fumbles.Value;
            }
        }

        private void rushing_20_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Rushing_20 = (int)rushing_20.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRush20 = (int)rushing_20.Value;
            }
        }

        private void rushing_long_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Rushing_long = (int)rushing_long.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRushLong = (int)rushing_long.Value;
            }
        }

        private void rushing_bt_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Rushing_bt = (int)rushing_bt.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRushBtk = (int)rushing_bt.Value;
            }
        }

        private void rushing_yac_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Rushing_yac = (int)rushing_yac.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaRushYac = (int)rushing_yac.Value;
            }
        }

        private void Firstdowns_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).FirstDowns = (int)Firstdowns.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaFirstDowns = (int)Firstdowns.Value;
            }
        }

        private void comebacks_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOffenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Comebacks = (int)comebacks.Value;
                else model.PlayerModel.GetOffStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SeaComebacks = (int)comebacks.Value;
            }
        }

        #endregion

        #region OLine Stats

        private void pancakes_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOLCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pancakes = (int)pancakes.Value;
                else model.PlayerModel.GetOLstats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Pancakes = (int)pancakes.Value;
            }
        }

        private void sacksallowed_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersOLCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).SacksAllowed = (int)sacksallowed.Value;
                else model.PlayerModel.GetOLstats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).SacksAllowed = (int)sacksallowed.Value;
            }
        }

        #endregion

        #region Defense Stats

        private void tackles_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Tackles = (int)tackles.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Tackles = (int)tackles.Value;
            }
        }

        private void tacklesforloss_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).TacklesForLoss = (int)tacklesforloss.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).TacklesForLoss = (int)tacklesforloss.Value;
            }
        }

        private void sacks_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Sacks = (int)sacks.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Sacks = (int)sacks.Value;
            }
        }

        private void fumblesforced_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).FumblesForced = (int)fumblesforced.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).FumblesForced = (int)fumblesforced.Value;
            }
        }

        private void fumblesrecovered_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).FumblesRecovered = (int)fumblesrecovered.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).FumblesRecovered = (int)fumblesrecovered.Value;
            }
        }

        private void fumbles_td_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fumbles_td = (int)fumbles_td.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).FumbleTDS = (int)fumbles_td.Value;
            }
        }

        private void fumbleyards_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).FumbleYards = (int)fumbleyards.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).FumbleYards = (int)fumbleyards.Value;
            }
        }

        private void blocks_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Blocks = (int)blocks.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Blocks = (int)blocks.Value;
            }
        }

        private void safeties_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Safeties = (int)safeties.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Safeties = (int)safeties.Value;
            }
        }

        private void passesdefended_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).PassesDefended = (int)passesdefended.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).PassesDefended = (int)passesdefended.Value;
            }
        }

        private void def_int_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Def_int = (int)def_int.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Interceptions = (int)def_int.Value;
            }
        }

        private void int_td_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Int_td = (int)int_td.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).InterceptionTDS = (int)int_td.Value;
            }
        }

        private void int_yards_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Int_yards = (int)int_yards.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).InterceptionYards = (int)int_yards.Value;
            }
        }

        private void int_long_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersDefenseCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).Int_long = (int)int_long.Value;
                else model.PlayerModel.GetDefenseStats(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).InterceptionLong = (int)int_long.Value;
            }
        }

        private void CatchesAllowed_ValueChanged(object sender, EventArgs e)
        {

        }

        private void BigHits_ValueChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region Games Played

        // fix
        private void gamesstarted_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (model.MadVersion != MaddenFileVersion.Ver2004 && statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersGamesCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).GamesStarted = (int)gamesstarted.Value;
                else model.PlayerModel.GetSeasonGames(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).GamesStarted = (int)gamesstarted.Value;
            }
        }

        private void gamesplayed_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                {
                    if (model.MadVersion == MaddenFileVersion.Ver2004)
                        model.PlayerModel.GetPlayersGamesCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).GamesPlayed04 = (int)gamesplayed.Value;
                    else model.PlayerModel.GetPlayersGamesCareer(model.PlayerModel.CurrentPlayerRecord.PlayerId).GamesPlayed = (int)gamesplayed.Value;
                }
                else
                {
                    model.PlayerModel.GetSeasonGames(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).GamesPlayed = (int)gamesplayed.Value;
                }
            }
        }

        #endregion

        //fix
        #region Punt/Kick

        private void fga_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fga = (int)fga.Value;
                else model.PlayerModel.GetPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId, year).Fga = (int)fga.Value;
            }
        }

        private void fgm_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgm = (int)fgm.Value;
            }
        }

        private void fgbl_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgbl = (int)fgbl.Value;
            }
        }

        private void fgl_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgl = (int)fgl.Value;
            }
        }

        private void xpa_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Xpa = (int)xpa.Value;
            }
        }

        private void xpm_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Xpm = (int)xpm.Value;
            }
        }

        private void xpb_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Xpb = (int)xpb.Value;
            }
        }

        private void fga_129_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fga_129 = (int)fga_129.Value;
            }
        }

        private void fga_3039_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fga_3039 = (int)fga_3039.Value;
            }
        }

        private void fga_4049_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fga_4049 = (int)fga_4049.Value;
            }
        }

        private void fga_50_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fga_50 = (int)fga_50.Value;
            }
        }

        private void fgm_129_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgm_129 = (int)fgm_129.Value;
            }
        }

        private void fgm_3039_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgm_3039 = (int)fgm_3039.Value;
            }
        }

        private void fgm_4049_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgm_4049 = (int)fgm_4049.Value;
            }
        }

        private void fgm_50_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Fgm_50 = (int)fgm_50.Value;
            }
        }

        private void kickoffs_ValueChanged(object sender, EventArgs e)
        {

        }

        private void touchbacks_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Touchbacks = (int)touchbacks.Value;
            }
        }

        private void puntatt_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Puntatt = (int)puntatt.Value;
            }
        }

        private void puntyds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Puntyds = (int)puntyds.Value;
            }
        }

        private void puntlong_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Puntlong = (int)puntlong.Value;
            }
        }

        private void puntny_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Puntny = (int)puntny.Value;
            }
        }

        private void puntin20_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Puntin20 = (int)puntin20.Value;
            }
        }

        private void punttb_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Punttb = (int)punttb.Value;
            }
        }

        private void puntblk_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPuntKick(model.PlayerModel.CurrentPlayerRecord.PlayerId).Puntblk = (int)puntblk.Value;
            }
        }

        #endregion

        #region Punt/Kick Returns

        private void kra_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Kra = (int)kra.Value;
            }
        }

        private void kryds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Kryds = (int)kryds.Value;
            }
        }

        private void krl_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Krl = (int)krl.Value;
            }
        }

        private void krtd_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Krtd = (int)krtd.Value;
            }
        }

        private void pra_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pra = (int)pra.Value;
            }
        }

        private void pryds_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Pryds = (int)pryds.Value;
            }
        }

        private void prl_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Prl = (int)prl.Value;
            }
        }

        private void prtd_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                if (statsyear.Text == "Career")
                    model.PlayerModel.GetPlayersCareerPKReturn(model.PlayerModel.CurrentPlayerRecord.PlayerId).Prtd = (int)prtd.Value;
            }
        }

        #endregion





        #endregion



        #endregion

        #endregion

        private void FixAudioID_Button_Click(object sender, EventArgs e)
        {
            isInitialising = true;

            foreach (PlayerRecord rec in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                if (model.PlayerModel.PlayerComments.ContainsKey(rec.LastName))
                {
                    rec.PlayerComment = model.PlayerModel.PlayerComments[rec.LastName];
                }
                else rec.PlayerComment = 0;
            }

            LoadPlayerInfo(model.PlayerModel.CurrentPlayerRecord);
            isInitialising = false;
        }

        private void QBStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                model.PlayerModel.CurrentPlayerRecord.QBStyle = (int)QBStyle.SelectedIndex;
            }
        }

        private void DevTrait_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
                model.PlayerModel.CurrentPlayerRecord.DevTrait = (int)DevTrait.SelectedIndex;
        }

        private void SetFranchiseTagtbutton_Click(object sender, EventArgs e)   //new 11-19-23
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set contract details based on PositionId
                        switch (currentRecord.PositionId)
                        {
                            case 0: // QB
                                    // Set QB contract details
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 3242;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 3242;
                                break;

                            case 1: // RB
                                    // Set contract details for RB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1010;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1010;
                                break;

                            case 2: //  FB
                                    // Set contract details for WR
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1010;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1010;
                                break;

                            case 3: //  WR
                                    // Set contract details for WR
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1974;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1974;
                                break;

                            case 4: // TE
                                    // Set contract details for TE
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1136;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1136;
                                break;

                            case 5: // LT
                                    // Set contract details for LT
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1824;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1824;
                                break;

                            case 6: // LG
                                    // Set contract details for LG
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1824;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1824;
                                break;

                            case 7: // C
                                    // Set contract details for C
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1824;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1824;
                                break;

                            case 8: // RG
                                    // Set contract details for RG
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1824;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1824;
                                break;

                            case 9: // RT
                                    // Set contract details for RT
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1824;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1824;
                                break;

                            case 10: // RE
                                     // Set contract details for RE
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1973;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1973;
                                break;

                            case 11: // LE
                                     // Set contract details for LE
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1973;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1973;
                                break;

                            case 12: // DT
                                     // Set contract details for DT
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1894;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1894;
                                break;

                            case 13: // LOLB
                                     // Set contract details for LOLB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 2093;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 2093;
                                break;

                            case 14: // MLB
                                     // Set contract details for MLB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 2093;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 2093;
                                break;

                            case 15: // ROLB
                                     // Set contract details for ROLB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 2093;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 2093;
                                break;

                            case 16: // CB
                                     // Set contract details for CB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1814;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1814;
                                break;

                            case 17: // SS
                                     // Set contract details for SS
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1446;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1446;
                                break;

                            case 18: // FS
                                     // Set contract details for FS
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1446;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1446;
                                break;

                            // Add more cases for other positions as needed
                            // ...

                            default:
                                // Default contract details
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 539;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 539;
                                break;
                        }

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("The Franchise Tag has successfully applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTransitionTagtbutton_Click(object sender, EventArgs e)   //new 11-19-23
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set contract details based on PositionId
                        switch (currentRecord.PositionId)
                        {
                            case 0: // QB
                                    // Set QB contract details
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 2950;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 2950;
                                break;

                            case 1: // RB
                                    // Set contract details for RB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 843;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 843;
                                break;

                            case 2: //  FB
                                    // Set contract details for WR
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 843;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 843;
                                break;

                            case 3: //  WR
                                    // Set contract details for WR
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1799;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1799;
                                break;

                            case 4: // TE
                                    // Set contract details for TE
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 972;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 972;
                                break;

                            case 5: // LT
                                    // Set contract details for LT
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1666;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1666;
                                break;

                            case 6: // LG
                                    // Set contract details for LG
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1666;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1666;
                                break;

                            case 7: // C
                                    // Set contract details for C
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1666;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1666;
                                break;

                            case 8: // RG
                                    // Set contract details for RG
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1666;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1666;
                                break;

                            case 9: // RT
                                    // Set contract details for RT
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1666;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1666;
                                break;

                            case 10: // RE
                                     // Set contract details for RE
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1745;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1745;
                                break;

                            case 11: // LE
                                     // Set contract details for LE
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1745;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1745;
                                break;

                            case 12: // DT
                                     // Set contract details for DT
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1610;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1610;
                                break;

                            case 13: // LOLB
                                     // Set contract details for LOLB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1748;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1748;
                                break;

                            case 14: // MLB
                                     // Set contract details for MLB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1748;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1748;
                                break;

                            case 15: // ROLB
                                     // Set contract details for ROLB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1748;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1748;
                                break;

                            case 16: // CB
                                     // Set contract details for CB
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1579;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1579;
                                break;

                            case 17: // SS
                                     // Set contract details for SS
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1187;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1187;
                                break;

                            case 18: // FS
                                     // Set contract details for FS
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 1187;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 1187;
                                break;

                            // Add more cases for other positions as needed
                            // ...

                            default:
                                // Default contract details
                                currentRecord.ContractLength = 1;
                                currentRecord.ContractYearsLeft = 1;
                                currentRecord.Salary0 = 487;
                                currentRecord.Salary1 = 0;
                                currentRecord.Salary2 = 0;
                                currentRecord.Salary3 = 0;
                                currentRecord.Salary4 = 0;
                                currentRecord.Salary5 = 0;
                                currentRecord.Salary6 = 0;
                                currentRecord.Bonus0 = 0;
                                currentRecord.Bonus1 = 0;
                                currentRecord.Bonus2 = 0;
                                currentRecord.Bonus3 = 0;
                                currentRecord.Bonus4 = 0;
                                currentRecord.Bonus5 = 0;
                                currentRecord.Bonus6 = 0;
                                currentRecord.TotalBonus = 0;
                                currentRecord.TotalSalary = 487;
                                break;
                        }

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("The Transition Tag has successfully applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RookieScaleContractsbutton_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    bool changesApplied = false; // Flag to track if any changes were applied

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;

                        // Check if the team ID is within a specific range and the total salary is 0
                        if (playerRecord.YearsPro ==0 && playerRecord.DraftRound ==1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;
                            playerRecord.TotalBonus = 1200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;
                            playerRecord.TotalBonus = 1200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;
                            playerRecord.TotalBonus = 1200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)   // Rookie Option
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 700; // Minimum value
                                int maxSalary4 = 900; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 300;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 900; // Minimum value
                                int maxSalary4 = 1200; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 300;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;
                            playerRecord.TotalBonus = 400;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;
                            playerRecord.TotalBonus = 400;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;
                            playerRecord.TotalBonus = 400;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 500; // Minimum value
                                int maxSalary4 = 700; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 100;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 850; // Minimum value
                                int maxSalary4 = 1150; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 100;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;
                            playerRecord.TotalBonus = 300;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;
                            playerRecord.TotalBonus = 300;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;
                            playerRecord.TotalBonus = 300;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 450; // Minimum value
                                int maxSalary4 = 650; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 75;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 800; // Minimum value
                                int maxSalary4 = 1100; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 75;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;
                            playerRecord.TotalBonus = 200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;
                            playerRecord.TotalBonus = 200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;
                            playerRecord.TotalBonus = 200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 400; // Minimum value
                                int maxSalary4 = 600; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 50;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 750; // Minimum value
                                int maxSalary4 = 1050; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 50;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;
                            playerRecord.TotalBonus = 100;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;
                            playerRecord.TotalBonus = 100;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;
                            playerRecord.TotalBonus = 100;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 350; // Minimum value
                                int maxSalary4 = 550; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 25;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 700; // Minimum value
                                int maxSalary4 = 1000; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 25;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;
                            playerRecord.TotalBonus = 60;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;
                            playerRecord.TotalBonus = 60;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;
                            playerRecord.TotalBonus = 60;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 300; // Minimum value
                                int maxSalary4 = 500; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 15;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 650; // Minimum value
                                int maxSalary4 = 950; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 15;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;
                            playerRecord.TotalBonus = 40;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;
                            playerRecord.TotalBonus = 40;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;
                            playerRecord.TotalBonus = 40;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 250; // Minimum value
                                int maxSalary4 = 450; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 10;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 600; // Minimum value
                                int maxSalary4 = 900; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 10;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;
                            playerRecord.TotalBonus = 0;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;
                            playerRecord.TotalBonus = 0;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;
                            playerRecord.TotalBonus = 0;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 200; // Minimum value
                                int maxSalary4 = 400; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 500; // Minimum value
                                int maxSalary4 = 800; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                    }
                    // Display a message if changes were applied
                    if (changesApplied)
                    {
                        MessageBox.Show("Rookie Scale contracts applied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No changes were applied.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void MinimumContractsbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    bool changesApplied = false; // Flag to track if any changes were applied

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;

                        // Check if the team ID is within a specific range and the total salary is 0
                        if (playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32 && playerRecord.TotalSalary == 0)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 1;
                            playerRecord.ContractYearsLeft = 1;
                            playerRecord.Salary0 = 75;
                            playerRecord.TotalSalary = 75;

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                    }

                    // Display a message if changes were applied
                    if (changesApplied)
                    {
                        MessageBox.Show("Minimum contracts applied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No changes were applied.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateUI));
                return;
            }

            // Your UI update logic here
        }



        private void ClearContractbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set contract details for the minimum contract scenario
                        currentRecord.ContractLength = 0;
                        currentRecord.ContractYearsLeft = 0;
                        currentRecord.Salary0 = 0;
                        currentRecord.Salary1 = 0;
                        currentRecord.Salary2 = 0;
                        currentRecord.Salary3 = 0;
                        currentRecord.Salary4 = 0;
                        currentRecord.Salary5 = 0;
                        currentRecord.Salary6 = 0;
                        currentRecord.Bonus0 = 0;
                        currentRecord.Bonus1 = 0;
                        currentRecord.Bonus2 = 0;
                        currentRecord.Bonus3 = 0;
                        currentRecord.Bonus4 = 0;
                        currentRecord.Bonus5 = 0;
                        currentRecord.Bonus6 = 0;
                        currentRecord.TotalBonus = 0;
                        currentRecord.TotalSalary = 0;

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("This contract has successfully cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RandomLowEndContractButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set random contract details
                        SetRandomLowEndContract(currentRecord);

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("Random Low Tier contract details applied successfully to the current entry.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetRandomLowEndContract(PlayerRecord currentRecord)    //verified 11-19-23
        {
            // Generate random values for ContractLength and ContractYearsLeft
            Random random = new Random();
            currentRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
            currentRecord.ContractYearsLeft = currentRecord.ContractLength;

            // Reset all salary and bonus values to 0
            for (int i = 0; i <= 6; i++)
            {
                SetPropertyValue(currentRecord, $"Salary{i}", 0);
                SetPropertyValue(currentRecord, $"Bonus{i}", 0);
            }

            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
            int baseSalary = random.Next(75, 150); // Initial salary value
            int baseBonus = random.Next(0, 100); // Initial bonus value
            for (int i = 0; i < currentRecord.ContractLength; i++)
            {
                // Group positions with the same parameters
                switch (currentRecord.PositionId)
                {
                    case 0: // QB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(175, 200) + additionalAmountTier1Low);
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(10, 20));
                        break;

                    case 13: // LOLB
                    case 14: // MLB
                    case 15: // ROLB
                    case 3: //  WR
                    case 10: // RE
                    case 11: // LE
                    case 12: // DT
                    case 16: // CB
                    case 5: //LT
                    case 6: //LG
                    case 7: //C
                    case 8: //RG
                    case 9: //RT
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(125, 150) + additionalAmountTier2Low);
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15));
                        break;

                    case 17: // SS
                    case 18: // FS
                    case 4: // TE
                    case 1: // RB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100));
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 10));
                        break;

                    // Add more cases for other positions as needed
                    // ...

                    default:
                        // Increment the base salary and bonus for each iteration
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75));
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15)); // Default bonus range
                        break;
                }
            }
        


            // Calculate TotalBonus as the sum of Bonus0-Bonus6
            currentRecord.TotalBonus = CalculateTotalBonus(currentRecord);

            // Calculate TotalSalary as the sum of Salary0-Salary6
            currentRecord.TotalSalary = CalculateTotalSalary(currentRecord);
        }

        private int additionalAmountTier1Low = 200; //salary bump
        private int additionalAmountTier2Low = 100; //salary bump

        private void SetPropertyValue(PlayerRecord currentRecord, string propertyName, int value)
        {
            var property = typeof(PlayerRecord).GetProperty(propertyName);
            property?.SetValue(currentRecord, value);
        }

        private int CalculateTotalBonus(PlayerRecord currentRecord)
        {
            int totalBonus = 0;
            for (int i = 0; i <= 6; i++)
            {
                object bonusValue = typeof(PlayerRecord).GetProperty($"Bonus{i}")?.GetValue(currentRecord);
                totalBonus += bonusValue != null ? (int)bonusValue : 0;
            }
            return totalBonus;
        }

        private int CalculateTotalSalary(PlayerRecord currentRecord)
        {
            int totalSalary = 0;
            for (int i = 0; i <= 6; i++)
            {
                object salaryValue = typeof(PlayerRecord).GetProperty($"Salary{i}")?.GetValue(currentRecord);
                totalSalary += salaryValue != null ? (int)salaryValue : 0;
            }
            return totalSalary;
        }

        private void RandomMidEndContractButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set random contract details
                        SetRandomMidEndContract(currentRecord);

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("Random Mid Tier contract details applied successfully to the current entry.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetRandomMidEndContract(PlayerRecord currentRecord)    //verified 11-19-23
        {
            // Generate random values for ContractLength and ContractYearsLeft
            Random random = new Random();
            currentRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
            currentRecord.ContractYearsLeft = currentRecord.ContractLength;

            // Reset all salary and bonus values to 0
            for (int i = 0; i <= 6; i++)
            {
                SetPropertyValue(currentRecord, $"Salary{i}", 0);
                SetPropertyValue(currentRecord, $"Bonus{i}", 0);
            }

            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
            int baseSalary = random.Next(150, 450); // Initial salary value
            int baseBonus = random.Next(100, 200); // Initial bonus value
            for (int i = 0; i < currentRecord.ContractLength; i++)
            {
                // Group positions with the same parameters
                switch (currentRecord.PositionId)
                {
                    case 0: // QB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(175, 200) + additionalAmountTier1Mid);   //verified 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(50, 75));
                        break;

                    case 13: // LOLB
                    case 14: // MLB
                    case 15: // ROLB
                    case 3: //  WR
                    case 10: // RE
                    case 11: // LE
                    case 12: // DT
                    case 16: // CB
                    case 5: //LT
                    case 6: //LG
                    case 7: //C
                    case 8: //RG
                    case 9: //RT
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(125, 150) + additionalAmountTier2Mid);
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(15, 25));
                        break;

                    case 17: // SS
                    case 18: // FS
                    case 4: // TE
                    case 1: // RB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100));
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 10));
                        break;

                    // Add more cases for other positions as needed
                    // ...

                    default:
                        // Increment the base salary and bonus for each iteration
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75) + additionalAmountTier4Mid);
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15));
                        break;
                }
            }

                    // Calculate TotalBonus as the sum of Bonus0-Bonus6
                    currentRecord.TotalBonus = CalculateTotalBonus(currentRecord);

                     // Calculate TotalSalary as the sum of Salary0-Salary6
                     currentRecord.TotalSalary = CalculateTotalSalary(currentRecord);
        }
        private int additionalAmountTier1Mid = 700; //salary bump
        private int additionalAmountTier2Mid = 350; //salary bump
        private int additionalAmountTier4Mid = -50; //salary reduction


        private void RandomHighEndContractButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set random contract details
                        SetRandomHighEndContract(currentRecord);

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("Random High Tier contract details applied successfully to the current entry.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetRandomHighEndContract(PlayerRecord currentRecord)       //verified 11-19-23
        {
            // Generate random values for ContractLength and ContractYearsLeft
            Random random = new Random();
            currentRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
            currentRecord.ContractYearsLeft = currentRecord.ContractLength;

            // Reset all salary and bonus values to 0
            for (int i = 0; i <= 6; i++)
            {
                SetPropertyValue(currentRecord, $"Salary{i}", 0);
                SetPropertyValue(currentRecord, $"Bonus{i}", 0);
            }

            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
            int baseSalary = random.Next(950, 1150); // Initial salary value
            int baseBonus = random.Next(200, 600); // Initial bonus value
            for (int i = 0; i < currentRecord.ContractLength; i++)
            {
                // Group positions with the same parameters
                switch (currentRecord.PositionId)
                {
                    case 0: // QB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(400, 450) + additionalAmountTier1High);  //verified 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(50, 75));
                        break;

                    case 13: // LOLB
                    case 14: // MLB
                    case 15: // ROLB
                    case 3: //  WR
                    case 10: // RE
                    case 11: // LE
                    case 12: // DT
                    case 16: // CB
                    case 5: //LT
                    case 6: //LG
                    case 7: //C
                    case 8: //RG
                    case 9: //RT
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(225, 250) + additionalAmountTier2High); //verified 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(15, 25));
                        break;

                    case 17: // SS
                    case 18: // FS
                    case 4: // TE
                    case 1: // RB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100) + additionalAmountTier3High);   //verified 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 10));
                        break;

                    // Add more cases for other positions as needed
                    // ...

                    default:
                        // Increment the base salary and bonus for each iteration
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75) + additionalAmountTier4High);    //verified 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15) + addittionalBonusAmountForTier4High);  //verified 11-19-23
                        break;
                }
            }


            // Calculate TotalBonus as the sum of Bonus0-Bonus6
            currentRecord.TotalBonus = CalculateTotalBonus(currentRecord);

            // Calculate TotalSalary as the sum of Salary0-Salary6
            currentRecord.TotalSalary = CalculateTotalSalary(currentRecord);
        }
        private int additionalAmountTier1High = 1000;   // QB Salary Bump
        private int additionalAmountTier2High = 250;   //Salary Bump 
        private int additionalAmountTier3High = -300;   //Salary Dump
        private int additionalAmountTier4High = -700;  //salary Dump
        private int addittionalBonusAmountForTier4High = -150; //bonus reduction

        private void RandomEliteContractButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set random contract details
                        SetRandomEliteContract(currentRecord);

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("Random Elite contract details applied successfully to the current entry.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetRandomEliteContract(PlayerRecord currentRecord)     //verified 11-19-23
        {
            // Generate random values for ContractLength and ContractYearsLeft
            Random random = new Random();
            currentRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
            currentRecord.ContractYearsLeft = currentRecord.ContractLength;

            // Reset all salary and bonus values to 0
            for (int i = 0; i <= 6; i++)
            {
                SetPropertyValue(currentRecord, $"Salary{i}", 0);
                SetPropertyValue(currentRecord, $"Bonus{i}", 0);
            }

            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
            int baseSalary = random.Next(1250, 1650); // Initial salary value
            int baseBonus = random.Next(400, 800); // Initial bonus value
            for (int i = 0; i < currentRecord.ContractLength; i++)
            {
                // Group positions with the same parameters
                switch (currentRecord.PositionId)
                {
                    case 0: // QB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(450, 525) + additionalAmountForTier1Elite); //verfied 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(100, 150));
                        break;

                    case 13: // LOLB
                    case 14: // MLB
                    case 15: // ROLB
                    case 3: //  WR
                    case 10: // RE
                    case 11: // LE
                    case 12: // DT
                    case 16: // CB
                    case 5: //LT
                    case 6: //LG
                    case 7: //C
                    case 8: //RG
                    case 9: //RT
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(325, 350) + additionalAmountForTier2Elite); 
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(75, 100));
                        break;

                    case 17: // SS
                    case 18: // FS
                    case 4: // TE
                    case 1: // RB
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100) + additionalAmountForTier3Elite);
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(50, 75));
                        break;

                    // Add more cases for other positions as needed
                    // ...

                    default:
                        // Increment the base salary and bonus for each iteration
                        SetPropertyValue(currentRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75) + additionalAmountForTier4Elite);    //verified 11-19-23
                        SetPropertyValue(currentRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15) + addittionalBonusAmountForTier4Elite); //verfiied 11-19-23
                        break;
                }
            }


            // Calculate TotalBonus as the sum of Bonus0-Bonus6
            currentRecord.TotalBonus = CalculateTotalBonus(currentRecord);

            // Calculate TotalSalary as the sum of Salary0-Salary6
            currentRecord.TotalSalary = CalculateTotalSalary(currentRecord);
        }

        private int additionalAmountForTier1Elite = 1650; // Salary Bump
        private int additionalAmountForTier2Elite = 550;   // Salary Bump
        private int additionalAmountForTier3Elite = -300; //Salary Reduction
        private int additionalAmountForTier4Elite = -950; //Salary Reduction
        private int addittionalBonusAmountForTier4Elite = -350; //bonus reduction

        private void ManualUpDateContractClick(object sender, EventArgs e)
        {
            try
            {
                // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                // Check the individual salary values
                for (int i = 0; i <= 6; i++)
                {
                    Console.WriteLine($"Salary{i}: {currentRecord.GetType().GetProperty($"Salary{i}").GetValue(currentRecord)}");
                }

                // Calculate the sum of Salary0-Salary6
                int totalSalary = currentRecord.Salary0 + currentRecord.Salary1 + currentRecord.Salary2 +
                                  currentRecord.Salary3 + currentRecord.Salary4 + currentRecord.Salary5 + currentRecord.Salary6;

                // Update UI elements to reflect the changes
                UpdateUI(); // Call a method to update UI elements

                // Display the total salary or use it as needed
                MessageBox.Show($"Total Salary: {totalSalary}", "Total Salary", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log the exception details)
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void MinimumContractbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set contract details for the minimum contract scenario
                            currentRecord.ContractLength = 1;
                            currentRecord.ContractYearsLeft = 1;
                            currentRecord.Salary0 = 75;
                            currentRecord.TotalSalary = 75;
                            currentRecord.Salary1 = 0;
                            currentRecord.Salary2 = 0;
                            currentRecord.Salary3 = 0;
                            currentRecord.Salary4 = 0;
                            currentRecord.Salary5 = 0;
                            currentRecord.Salary6 = 0;
                            currentRecord.Bonus0 = 0;
                            currentRecord.Bonus1 = 0;
                            currentRecord.Bonus2 = 0;
                            currentRecord.Bonus3 = 0;
                            currentRecord.Bonus4 = 0;
                            currentRecord.Bonus5 = 0;
                            currentRecord.Bonus6 = 0;
                            currentRecord.TotalBonus = 0;

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("League Minimum contract details applied successfully to the current entry.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FaceIDConverterbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;


                        // Check if the face ID is equal to 2
                        if (playerRecord.FaceID_19 == 1)
                        {
                            // Set the face ID to 3
                            playerRecord.FaceID_19 = 1;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                        else if (playerRecord.FaceID_19 == 2)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 56;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 3)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 4;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 4)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 6;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 5)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 11;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 6)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 12;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 8)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 11;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 9)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 11;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 11)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 14;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 14)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 17;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 15)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 89;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 16)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 19;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 17)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 20;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 20)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 22;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 21)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 22;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 22)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 55;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 24)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 450;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 25)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 23;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 26)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 23;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 27)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 36;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 28)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 57;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 29)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 37;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 30)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 41;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 31)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 42;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 32)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 52;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 33)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 53;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 34)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 55;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 35)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 62;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 36)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 62;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 37)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 63;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 38)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 66;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 39)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 70;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 44)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 90;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 45)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 114;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 46)
                        {
                            // Do your existing logic for FaceID_19 == 1 here
                            playerRecord.FaceID_19 = 210;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.FaceID_19 == 47)
                        {
                            playerRecord.FaceID_19 = 85;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 48)
                        {
                            playerRecord.FaceID_19 = 141;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 51)
                        {
                            playerRecord.FaceID_19 = 19;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 53)
                        {
                            playerRecord.FaceID_19 = 93;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 57)
                        {
                            playerRecord.FaceID_19 = 96;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 59)
                        {
                            playerRecord.FaceID_19 = 106;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 60)
                        {
                            playerRecord.FaceID_19 = 40;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 63)
                        {
                            playerRecord.FaceID_19 = 115;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 64)
                        {
                            playerRecord.FaceID_19 = 453;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 65)
                        {
                            playerRecord.FaceID_19 = 117;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 66)
                        {
                            playerRecord.FaceID_19 = 120;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 67)
                        {
                            playerRecord.FaceID_19 = 451;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 69)
                        {
                            playerRecord.FaceID_19 = 123;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 71)
                        {
                            playerRecord.FaceID_19 = 252;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 74)
                        {
                            playerRecord.FaceID_19 = 210;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 76)
                        {
                            playerRecord.FaceID_19 = 135;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 77)
                        {
                            playerRecord.FaceID_19 = 137;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 79)
                        {
                            playerRecord.FaceID_19 = 145;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 87)
                        {
                            playerRecord.FaceID_19 = 146;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 88)
                        {
                            playerRecord.FaceID_19 = 148;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 89)
                        {
                            playerRecord.FaceID_19 = 150;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 91)
                        {
                            playerRecord.FaceID_19 = 256;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 95)
                        {
                            playerRecord.FaceID_19 = 155;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 97)
                        {
                            playerRecord.FaceID_19 = 165;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 99)
                        {
                            playerRecord.FaceID_19 = 185;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 102)
                        {
                            playerRecord.FaceID_19 = 191;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 106)
                        {
                            playerRecord.FaceID_19 = 185;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 109)
                        {
                            playerRecord.FaceID_19 = 197;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 110)
                        {
                            playerRecord.FaceID_19 = 201;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 111)
                        {
                            playerRecord.FaceID_19 = 203;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 113)
                        {
                            playerRecord.FaceID_19 = 207;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 115)
                        {
                            playerRecord.FaceID_19 = 210;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 117)
                        {
                            playerRecord.FaceID_19 = 211;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 120)     //edited
                        {
                            playerRecord.FaceID_19 = 128;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 123)
                        {
                            playerRecord.FaceID_19 = 263;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 124)
                        {
                            playerRecord.FaceID_19 = 264;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 126)
                        {
                            playerRecord.FaceID_19 = 265;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 130)
                        {
                            playerRecord.FaceID_19 = 275;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 131)
                        {
                            playerRecord.FaceID_19 = 275;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 132)
                        {
                            playerRecord.FaceID_19 = 281;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 133)
                        {
                            playerRecord.FaceID_19 = 286;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 134)
                        {
                            playerRecord.FaceID_19 = 287;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 135)
                        {
                            playerRecord.FaceID_19 = 288;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 136)
                        {
                            playerRecord.FaceID_19 = 204;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 137)
                        {
                            playerRecord.FaceID_19 = 212;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 138)
                        {
                            playerRecord.FaceID_19 = 213;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 139)
                        {
                            playerRecord.FaceID_19 = 235;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 140)
                        {
                            playerRecord.FaceID_19 = 220;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 141)
                        {
                            playerRecord.FaceID_19 = 221;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 143)
                        {
                            playerRecord.FaceID_19 = 235;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 145)
                        {
                            playerRecord.FaceID_19 = 266;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 146)
                        {
                            playerRecord.FaceID_19 = 267;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 147)
                        {
                            playerRecord.FaceID_19 = 269;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 148)
                        {
                            playerRecord.FaceID_19 = 269;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 154)
                        {
                            playerRecord.FaceID_19 = 289;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 155)
                        {
                            playerRecord.FaceID_19 = 290;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 158)
                        {
                            playerRecord.FaceID_19 = 13;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 159)
                        {
                            playerRecord.FaceID_19 = 48;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 162)
                        {
                            playerRecord.FaceID_19 = 1;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 163)
                        {
                            playerRecord.FaceID_19 = 47;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 164)
                        {
                            playerRecord.FaceID_19 = 47;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 165)
                        {
                            playerRecord.FaceID_19 = 1;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 166)
                        {
                            playerRecord.FaceID_19 = 48;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 167)
                        {
                            playerRecord.FaceID_19 = 51;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 169)
                        {
                            playerRecord.FaceID_19 = 59;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 170)
                        {
                            playerRecord.FaceID_19 = 60;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 171)
                        {
                            playerRecord.FaceID_19 = 22;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 172)
                        {
                            playerRecord.FaceID_19 = 71;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 174)
                        {
                            playerRecord.FaceID_19 = 74;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 175)
                        {
                            playerRecord.FaceID_19 = 87;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 177)
                        {
                            playerRecord.FaceID_19 = 93;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 178)
                        {
                            playerRecord.FaceID_19 = 113;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 179)
                        {
                            playerRecord.FaceID_19 = 210;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 180)
                        {
                            playerRecord.FaceID_19 = 138;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 181)
                        {
                            playerRecord.FaceID_19 = 139;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 182)
                        {
                            playerRecord.FaceID_19 = 140;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 183)
                        {
                            playerRecord.FaceID_19 = 144;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 184)
                        {
                            playerRecord.FaceID_19 = 149;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 185)
                        {
                            playerRecord.FaceID_19 = 154;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 186)
                        {
                            playerRecord.FaceID_19 = 177;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 187)
                        {
                            playerRecord.FaceID_19 = 256;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 188)
                        {
                            playerRecord.FaceID_19 = 182;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 189)
                        {
                            playerRecord.FaceID_19 = 183;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 190)
                        {
                            playerRecord.FaceID_19 = 179;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 191)
                        {
                            playerRecord.FaceID_19 = 180;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 192)
                        {
                            playerRecord.FaceID_19 = 184;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 193)
                        {
                            playerRecord.FaceID_19 = 95;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 194)
                        {
                            playerRecord.FaceID_19 = 189;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 195)
                        {
                            playerRecord.FaceID_19 = 191;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 196)
                        {
                            playerRecord.FaceID_19 = 196;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 197)
                        {
                            playerRecord.FaceID_19 = 202;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 198)
                        {
                            playerRecord.FaceID_19 = 237;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 199)
                        {
                            playerRecord.FaceID_19 = 238;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 200)
                        {
                            playerRecord.FaceID_19 = 239;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 201)
                        {
                            playerRecord.FaceID_19 = 40;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 202)
                        {
                            playerRecord.FaceID_19 = 241;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 203)
                        {
                            playerRecord.FaceID_19 = 242;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 204)
                        {
                            playerRecord.FaceID_19 = 243;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 205)
                        {
                            playerRecord.FaceID_19 = 244;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 206)
                        {
                            playerRecord.FaceID_19 = 245;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 207)
                        {
                            playerRecord.FaceID_19 = 246;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 208)
                        {
                            playerRecord.FaceID_19 = 248;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 209)
                        {
                            playerRecord.FaceID_19 = 210;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 210)
                        {
                            playerRecord.FaceID_19 = 251;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 211)
                        {
                            playerRecord.FaceID_19 = 210;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 212)
                        {
                            playerRecord.FaceID_19 = 253;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 213)
                        {
                            playerRecord.FaceID_19 = 256;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 214)     //edited
                        {
                            playerRecord.FaceID_19 = 290;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 215)
                        {
                            playerRecord.FaceID_19 = 262;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 218)
                        {
                            playerRecord.FaceID_19 = 274;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 219)
                        {
                            playerRecord.FaceID_19 = 277;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 221)     //edited
                        {
                            playerRecord.FaceID_19 = 238;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 222)         //edited
                        {
                            playerRecord.FaceID_19 = 128;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 223)
                        {
                            playerRecord.FaceID_19 = 286;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 226)
                        {
                            playerRecord.FaceID_19 = 238;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 227)
                        {
                            playerRecord.FaceID_19 = 288;       //edited
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 228)
                        {
                            playerRecord.FaceID_19 = 255;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 229)
                        {
                            playerRecord.FaceID_19 = 254;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 230)
                        {
                            playerRecord.FaceID_19 = 258;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 231)
                        {
                            playerRecord.FaceID_19 = 259;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 233)
                        {
                            playerRecord.FaceID_19 = 278;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 234)
                        {
                            playerRecord.FaceID_19 = 279;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 236)
                        {
                            playerRecord.FaceID_19 = 5;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 237)
                        {
                            playerRecord.FaceID_19 = 21;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 238)
                        {
                            playerRecord.FaceID_19 = 15;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 239)
                        {
                            playerRecord.FaceID_19 = 16;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 240)
                        {
                            playerRecord.FaceID_19 = 26;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 241)
                        {
                            playerRecord.FaceID_19 = 56;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 244)
                        {
                            playerRecord.FaceID_19 = 210;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 245)
                        {
                            playerRecord.FaceID_19 = 91;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 246)
                        {
                            playerRecord.FaceID_19 = 30;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 247)
                        {
                            playerRecord.FaceID_19 = 104;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 248)
                        {
                            playerRecord.FaceID_19 = 121;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 249)
                        {
                            playerRecord.FaceID_19 = 110;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 250)
                        {
                            playerRecord.FaceID_19 = 114;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 251)
                        {
                            playerRecord.FaceID_19 = 127;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 252)
                        {
                            playerRecord.FaceID_19 = 132;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 253)
                        {
                            playerRecord.FaceID_19 = 131;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 254)
                        {
                            playerRecord.FaceID_19 = 151;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 255)
                        {
                            playerRecord.FaceID_19 = 147;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 256)
                        {
                            playerRecord.FaceID_19 = 152;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 257)
                        {
                            playerRecord.FaceID_19 = 145;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 258)
                        {
                            playerRecord.FaceID_19 = 141;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 259)
                        {
                            playerRecord.FaceID_19 = 142;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 260)
                        {
                            playerRecord.FaceID_19 = 171;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 261)
                        {
                            playerRecord.FaceID_19 = 172;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 262)
                        {
                            playerRecord.FaceID_19 = 161;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 263)
                        {
                            playerRecord.FaceID_19 = 173;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 264)
                        {
                            playerRecord.FaceID_19 = 198;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 265)
                        {
                            playerRecord.FaceID_19 = 199;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 266)
                        {
                            playerRecord.FaceID_19 = 284;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 267)
                        {
                            playerRecord.FaceID_19 = 193;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 268)
                        {
                            playerRecord.FaceID_19 = 190;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 269)
                        {
                            playerRecord.FaceID_19 = 186;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 270)
                        {
                            playerRecord.FaceID_19 = 231;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 271)
                        {
                            playerRecord.FaceID_19 = 232;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 272)
                        {
                            playerRecord.FaceID_19 = 233;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 273)
                        {
                            playerRecord.FaceID_19 = 272;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 274)
                        {
                            playerRecord.FaceID_19 = 273;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 275)
                        {
                            playerRecord.FaceID_19 = 270;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 276)
                        {
                            playerRecord.FaceID_19 = 261;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 277)
                        {
                            playerRecord.FaceID_19 = 69;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 278)
                        {
                            playerRecord.FaceID_19 = 268;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 279)
                        {
                            playerRecord.FaceID_19 = 8;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 280)
                        {
                            playerRecord.FaceID_19 = 67;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 281)
                        {
                            playerRecord.FaceID_19 = 57;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 282)
                        {
                            playerRecord.FaceID_19 = 90;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 283)
                        {
                            playerRecord.FaceID_19 = 128;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 284)
                        {
                            playerRecord.FaceID_19 = 156;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 285)
                        {
                            playerRecord.FaceID_19 = 59;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 286)
                        {
                            playerRecord.FaceID_19 = 194;
                            UpdateUI();
                        }
                        else if (playerRecord.FaceID_19 == 287)
                        {
                            playerRecord.FaceID_19 = 119;
                        }
                    }
                    // Add more conditions for additional weight ranges if needed

                    // Update UI elements to reflect the changes
                    UpdateUI(); // Call a method to update UI elements
                    MessageBox.Show("The Madden 20 to Madden 22-24 Face Converter Process was successful and Changes have been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Display a message indicating that the conditions were not met for applying changes
                    MessageBox.Show("Conditions not met for applying changes to any entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        


        private void PracticeSquadContractbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;

                    // Check if the team ID is within a specific range and the total salary is greater than or equal to 0
                    if (currentRecord.TeamId >= 1 && currentRecord.TeamId <= 32 && currentRecord.TotalSalary >= 0)
                    {
                        // Set contract details for the minimum contract scenario
                        currentRecord.ContractLength = 1;
                        currentRecord.ContractYearsLeft = 1;
                        currentRecord.Salary0 = 22;
                        currentRecord.TotalSalary = 22;
                        currentRecord.Salary1 = 0;
                        currentRecord.Salary2 = 0;
                        currentRecord.Salary3 = 0;
                        currentRecord.Salary4 = 0;
                        currentRecord.Salary5 = 0;
                        currentRecord.Salary6 = 0;
                        currentRecord.Bonus0 = 0;
                        currentRecord.Bonus1 = 0;
                        currentRecord.Bonus2 = 0;
                        currentRecord.Bonus3 = 0;
                        currentRecord.Bonus4 = 0;
                        currentRecord.Bonus5 = 0;
                        currentRecord.Bonus6 = 0;
                        currentRecord.TotalBonus = 0;

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements

                        // Display a message indicating that changes were applied
                        MessageBox.Show("Practice Squad contract details applied successfully to the current entry.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to the current entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void MassBodyEditbutton_Click(object sender, EventArgs e)   // revised 12-24-23 adjusted body edits by height and every 6 inches, up to 7 ft+ and 450+ pounds
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    bool changesApplied = false; // Flag to track if any changes were applied

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;

                        // Check if the team ID is within a specific range and the total salary is 0
                        if (playerRecord.Weight >= 0 && playerRecord.Weight < 5 && playerRecord.Height <=66 )
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 1;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 5 && playerRecord.Weight < 10 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if  (playerRecord.Weight >= 10 && playerRecord.Weight < 15 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 15 && playerRecord.Weight < 20 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 20 && playerRecord.Weight < 25 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 25 && playerRecord.Weight < 30 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.9f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 30 && playerRecord.Weight < 35 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 35 && playerRecord.Weight < 40 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0.75f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 40 && playerRecord.Weight < 45 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0.75f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 45 && playerRecord.Weight < 50 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 50 && playerRecord.Weight < 55 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 55 && playerRecord.Weight < 60 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 60 && playerRecord.Weight < 65 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1.15f;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 65 && playerRecord.Weight < 70 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.6f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 70 && playerRecord.Weight < 75 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 75 && playerRecord.Weight < 80 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 80 && playerRecord.Weight < 85 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 85 && playerRecord.Weight < 90 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 90 && playerRecord.Weight < 95 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 1;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 95 && playerRecord.Weight < 100 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0.05f;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 1;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 100 && playerRecord.Weight < 105 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 105 && playerRecord.Weight < 110 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 2;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 2;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 2;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 2;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 2;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0.285f;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 2;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 2;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 110 && playerRecord.Weight < 115 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 115 && playerRecord.Weight < 120 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 120 && playerRecord.Weight < 125 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 125 && playerRecord.Weight < 130 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 130 && playerRecord.Weight < 135 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0.05f;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 135 && playerRecord.Weight < 250 && playerRecord.Height <= 66)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 2;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 2;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 2;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 2;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 2;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 2;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 2;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;

                        }
                        if (playerRecord.Weight >= 0 && playerRecord.Weight < 5 && playerRecord.Height >= 67 && playerRecord.Height < 72)   //here
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 1;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 5 && playerRecord.Weight < 10 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 10 && playerRecord.Weight < 15 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 15 && playerRecord.Weight < 20 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 20 && playerRecord.Weight < 25 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 25 && playerRecord.Weight < 30 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.9f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 30 && playerRecord.Weight < 35 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 35 && playerRecord.Weight < 40 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 40 && playerRecord.Weight < 45 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 45 && playerRecord.Weight < 50 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 50 && playerRecord.Weight < 55 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 55 && playerRecord.Weight < 60 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 60 && playerRecord.Weight < 65 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 65 && playerRecord.Weight < 70 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 70 && playerRecord.Weight < 75 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 75 && playerRecord.Weight < 80 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {
                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 80 && playerRecord.Weight < 85 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 85 && playerRecord.Weight < 90 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 90 && playerRecord.Weight < 95 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 95 && playerRecord.Weight < 100 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 100 && playerRecord.Weight < 105 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 105 && playerRecord.Weight < 110 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 110 && playerRecord.Weight < 115 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 115 && playerRecord.Weight < 120 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 120 && playerRecord.Weight < 125 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 125 && playerRecord.Weight < 130 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 130 && playerRecord.Weight < 135 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0.05f;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 135 && playerRecord.Weight < 250 && playerRecord.Height >= 67 && playerRecord.Height < 72)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 2;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 2;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 2;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 2;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 2;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 2;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 2;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;

                        }
                        if (playerRecord.Weight >= 0 && playerRecord.Weight < 5 && playerRecord.Height >= 72 && playerRecord.Height < 78)   //here
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 1;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 5 && playerRecord.Weight < 10 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 10 && playerRecord.Weight < 15 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 15 && playerRecord.Weight < 20 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 20 && playerRecord.Weight < 25 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 25 && playerRecord.Weight < 30 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.9f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 30 && playerRecord.Weight < 35 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 35 && playerRecord.Weight < 40 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 40 && playerRecord.Weight < 45 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 45 && playerRecord.Weight < 50 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 50 && playerRecord.Weight < 55 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 55 && playerRecord.Weight < 60 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 60 && playerRecord.Weight < 65 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.5f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 65 && playerRecord.Weight < 70 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.5f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 70 && playerRecord.Weight < 75 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.5f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 75 && playerRecord.Weight < 80 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.5f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 80 && playerRecord.Weight < 85 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.75f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 85 && playerRecord.Weight < 90 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 0.75f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 90 && playerRecord.Weight < 95 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 95 && playerRecord.Weight < 100 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 100 && playerRecord.Weight < 105 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 105 && playerRecord.Weight < 110 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 110 && playerRecord.Weight < 115 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 115 && playerRecord.Weight < 120 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 120 && playerRecord.Weight < 125 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 125 && playerRecord.Weight < 130 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 130 && playerRecord.Weight < 135 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0.05f;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 135 && playerRecord.Weight < 250 && playerRecord.Height >= 72 && playerRecord.Height < 78)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 2;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 2;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 2;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 2;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 2;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 2;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 2;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;

                        }
                        if (playerRecord.Weight >= 0 && playerRecord.Weight < 5 && playerRecord.Height >= 78 && playerRecord.Height < 84)   //here
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 1;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 5 && playerRecord.Weight < 10 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 10 && playerRecord.Weight < 15 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 15 && playerRecord.Weight < 20 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 20 && playerRecord.Weight < 25 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 25 && playerRecord.Weight < 30 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.9f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 30 && playerRecord.Weight < 35 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0.35f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 35 && playerRecord.Weight < 40 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 40 && playerRecord.Weight < 45 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 45 && playerRecord.Weight < 50 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0.25f;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 50 && playerRecord.Weight < 55 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 55 && playerRecord.Weight < 60 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 60 && playerRecord.Weight < 65 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 65 && playerRecord.Weight < 70 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 70 && playerRecord.Weight < 75 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 75 && playerRecord.Weight < 80 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 80 && playerRecord.Weight < 85 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 85 && playerRecord.Weight < 90 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 90 && playerRecord.Weight < 95 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 95 && playerRecord.Weight < 100 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 100 && playerRecord.Weight < 105 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 105 && playerRecord.Weight < 110 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 0;
                            playerRecord.ArmSize = 0;
                            playerRecord.ButtDefn = 0;
                            playerRecord.ButtSize = 0;
                            playerRecord.CalfDefn = 0;
                            playerRecord.CalfSize = 0;
                            playerRecord.FootDefn = 0;
                            playerRecord.FootSize = 0;
                            playerRecord.GutDefn = 0;
                            playerRecord.GutSize = 0;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 0;
                            playerRecord.ShoulderSize = 0;
                            playerRecord.ThighDefn = 0;
                            playerRecord.ThighSize = 0;
                            playerRecord.WaistDefn = 0.8f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 110 && playerRecord.Weight < 115 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 115 && playerRecord.Weight < 120 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 120 && playerRecord.Weight < 125 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.65f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 125 && playerRecord.Weight < 130 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 130 && playerRecord.Weight < 135 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0.05f;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;
                        }
                        else if (playerRecord.Weight >= 135 && playerRecord.Weight < 250 && playerRecord.Height >= 78 && playerRecord.Height < 84)
                        {

                            playerRecord.ArmDefn = 1;
                            playerRecord.ArmSize = 1;
                            playerRecord.ButtDefn = 1;
                            playerRecord.ButtSize = 1;
                            playerRecord.CalfDefn = 1;
                            playerRecord.CalfSize = 1;
                            playerRecord.FootDefn = 1;
                            playerRecord.FootSize = 1;
                            playerRecord.GutDefn = 1;
                            playerRecord.GutSize = 1;
                            playerRecord.PadSize = 0.05f;
                            playerRecord.ShoulderHeight = 0;
                            playerRecord.ShoulderDefn = 1;
                            playerRecord.ShoulderSize = 1;
                            playerRecord.ThighDefn = 1;
                            playerRecord.ThighSize = 1;
                            playerRecord.WaistDefn = 0.5f;
                            playerRecord.WaistSize = 0.5f;

                            changesApplied = true;

                        }
                        // Add more conditions for additional weight ranges if needed

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements
                    }

                    // Display a message indicating that changes were applied
                    if (changesApplied)
                    {
                        MessageBox.Show("The Mass Body Edits have successfully been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to any entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SkinnyBodyEditbutton_Click(object sender, EventArgs e) //new 11-26-23
        {
            try
            {
                bool changesApplied = false; // Flag to track if any changes were applied
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;


                        // Check if the team ID is within a specific range and the total salary is 0

                        {

                            currentRecord.ArmDefn = 0;
                            currentRecord.ArmSize = 0;
                            currentRecord.ButtDefn = 0;
                            currentRecord.ButtSize = 0;
                            currentRecord.CalfDefn = 0;
                            currentRecord.CalfSize = 0;
                            currentRecord.FootDefn = 0;
                            currentRecord.FootSize = 0;
                            currentRecord.GutDefn = 0;
                            currentRecord.GutSize = 0;
                            currentRecord.PadSize = 0;
                            currentRecord.ShoulderHeight = 0;
                            currentRecord.ShoulderDefn = 0;
                            currentRecord.ShoulderSize = 0;
                            currentRecord.ThighDefn = 0;
                            currentRecord.ThighSize = 0;
                            currentRecord.WaistDefn = 0;
                            currentRecord.WaistSize = 0;

                            changesApplied = true;
                        }
                        // Add more conditions for additional weight ranges if needed

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements
                    }

                    // Display a message indicating that changes were applied
                    if (changesApplied)
                    {
                        MessageBox.Show("The Skinny Body Edit has successfully been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to any entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MuscleBodyEditbutton_Click(object sender, EventArgs e) //new 11-26-23
        {
            try
            {
                bool changesApplied = false; // Flag to track if any changes were applied
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;


                    // Check if the team ID is within a specific range and the total salary is 0

                    {

                        currentRecord.ArmDefn = 0.75f;
                        currentRecord.ArmSize = 1;
                        currentRecord.ButtDefn = 1;
                        currentRecord.ButtSize = 1;
                        currentRecord.CalfDefn = 1;
                        currentRecord.CalfSize = 1;
                        currentRecord.FootDefn = 1;
                        currentRecord.FootSize = 1;
                        currentRecord.GutDefn = 1;
                        currentRecord.GutSize = 1;
                        currentRecord.PadSize = 0;
                        currentRecord.ShoulderHeight = 0;
                        currentRecord.ShoulderDefn = 1;
                        currentRecord.ShoulderSize = 1;
                        currentRecord.ThighDefn = 1;
                        currentRecord.ThighSize = 1;
                        currentRecord.WaistDefn = 0.5f;
                        currentRecord.WaistSize = 0.5f;

                        changesApplied = true;
                    }
                    // Add more conditions for additional weight ranges if needed

                    // Update UI elements to reflect the changes
                    UpdateUI(); // Call a method to update UI elements
                }

                // Display a message indicating that changes were applied
                if (changesApplied)
                {
                    MessageBox.Show("The Skinny Body Edit has successfully been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Display a message indicating that the conditions were not met for applying changes
                    MessageBox.Show("Conditions not met for applying changes to any entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FatBodyEditbutton_Click(object sender, EventArgs e) //new 11-26-23
        {
            try
            {
                bool changesApplied = false; // Flag to track if any changes were applied
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    // Assuming you have access to the current player record instance, replace 'currentRecord' with your actual instance.
                    PlayerRecord currentRecord = model.PlayerModel.CurrentPlayerRecord;


                    // Check if the team ID is within a specific range and the total salary is 0

                    {
                        currentRecord.ArmDefn = 1;
                        currentRecord.ArmSize = 2;
                        currentRecord.ButtDefn = 1;
                        currentRecord.ButtSize = 2;
                        currentRecord.CalfDefn = 1;
                        currentRecord.CalfSize = 2;
                        currentRecord.FootDefn = 1;
                        currentRecord.FootSize = 2;
                        currentRecord.GutDefn = 1;
                        currentRecord.GutSize = 2;
                        currentRecord.PadSize = 0;
                        currentRecord.ShoulderHeight = 0;
                        currentRecord.ShoulderDefn = 1;
                        currentRecord.ShoulderSize = 2;
                        currentRecord.ThighDefn = 1;
                        currentRecord.ThighSize = 2;
                        currentRecord.WaistDefn = 0.5f;
                        currentRecord.WaistSize = 0.5f;

                        changesApplied = true;
                    }
                    // Add more conditions for additional weight ranges if needed

                    // Update UI elements to reflect the changes
                    UpdateUI(); // Call a method to update UI elements
                }

                // Display a message indicating that changes were applied
                if (changesApplied)
                {
                    MessageBox.Show("The Fat Body Edit has successfully been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Display a message indicating that the conditions were not met for applying changes
                    MessageBox.Show("Conditions not met for applying changes to any entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MassContractGeneratorbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    bool changesApplied = false; // Flag to track if any changes were applied

                    Random random = new Random();

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;

                        // Check if the team ID is within a specific range and the total salary is 0

                        // Check if the team ID is within a specific range and the total salary is 0
                        if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;
                            playerRecord.TotalBonus = 1200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;
                            playerRecord.TotalBonus = 1200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;
                            playerRecord.TotalBonus = 1200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 1 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 700; // Minimum value
                                int maxSalary4 = 900; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 300;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 900; // Minimum value
                                int maxSalary4 = 1200; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 300;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 300;
                            playerRecord.Bonus1 = 300;
                            playerRecord.Bonus2 = 300;
                            playerRecord.Bonus3 = 300;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;
                            playerRecord.TotalBonus = 400;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;
                            playerRecord.TotalBonus = 400;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;
                            playerRecord.TotalBonus = 400;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 2 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 500; // Minimum value
                                int maxSalary4 = 700; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 100;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 850; // Minimum value
                                int maxSalary4 = 1150; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 100;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 100;
                            playerRecord.Bonus1 = 100;
                            playerRecord.Bonus2 = 100;
                            playerRecord.Bonus3 = 100;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;
                            playerRecord.TotalBonus = 300;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;
                            playerRecord.TotalBonus = 300;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;
                            playerRecord.TotalBonus = 300;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 3 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 450; // Minimum value
                                int maxSalary4 = 650; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 75;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 800; // Minimum value
                                int maxSalary4 = 1100; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 75;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 75;
                            playerRecord.Bonus1 = 75;
                            playerRecord.Bonus2 = 75;
                            playerRecord.Bonus3 = 75;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;
                            playerRecord.TotalBonus = 200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;
                            playerRecord.TotalBonus = 200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;
                            playerRecord.TotalBonus = 200;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 4 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 400; // Minimum value
                                int maxSalary4 = 600; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 50;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 750; // Minimum value
                                int maxSalary4 = 1050; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 50;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 50;
                            playerRecord.Bonus1 = 50;
                            playerRecord.Bonus2 = 50;
                            playerRecord.Bonus3 = 50;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;
                            playerRecord.TotalBonus = 100;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;
                            playerRecord.TotalBonus = 100;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;
                            playerRecord.TotalBonus = 100;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 5 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 350; // Minimum value
                                int maxSalary4 = 550; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 25;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 700; // Minimum value
                                int maxSalary4 = 1000; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 25;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 25;
                            playerRecord.Bonus1 = 25;
                            playerRecord.Bonus2 = 25;
                            playerRecord.Bonus3 = 25;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;
                            playerRecord.TotalBonus = 60;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;
                            playerRecord.TotalBonus = 60;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;
                            playerRecord.TotalBonus = 60;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 6 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 300; // Minimum value
                                int maxSalary4 = 500; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 15;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 650; // Minimum value
                                int maxSalary4 = 950; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 15;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 15;
                            playerRecord.Bonus1 = 15;
                            playerRecord.Bonus2 = 15;
                            playerRecord.Bonus3 = 15;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }

                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;
                            playerRecord.TotalBonus = 40;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;
                            playerRecord.TotalBonus = 40;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;
                            playerRecord.TotalBonus = 40;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 7 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 250; // Minimum value
                                int maxSalary4 = 450; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 10;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 600; // Minimum value
                                int maxSalary4 = 900; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 10;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 10;
                            playerRecord.Bonus1 = 10;
                            playerRecord.Bonus2 = 10;
                            playerRecord.Bonus3 = 10;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }

                        else if (playerRecord.YearsPro == 0 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 4;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;
                            playerRecord.TotalBonus = 0;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 1 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 3;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;
                            playerRecord.TotalBonus = 0;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 2 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;
                            playerRecord.ContractYearsLeft = 2;
                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;
                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;
                            playerRecord.TotalBonus = 0;
                            playerRecord.Salary4 = 0;
                            playerRecord.Salary5 = 0;
                            playerRecord.Salary6 = 0;
                            playerRecord.Bonus4 = 0;
                            playerRecord.Bonus5 = 0;
                            playerRecord.Bonus6 = 0;

                            changesApplied = true; // Set the flag to true


                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro == 3 && playerRecord.DraftRound == 63 && playerRecord.TeamId >= 1 && playerRecord.TeamId <= 32)
                        {
                            // Set contract details for the minimum contract scenario
                            playerRecord.ContractLength = 4;

                            // Check if overall is above 90 to determine the contract length and years left
                            if (playerRecord.Overall > 80)
                            {
                                playerRecord.ContractLength = 5;
                                playerRecord.ContractYearsLeft = 2;
                            }
                            else
                            {
                                playerRecord.ContractYearsLeft = 1;
                            }

                            playerRecord.Salary0 = 75;
                            playerRecord.Salary1 = 100;
                            playerRecord.Salary2 = 125;
                            playerRecord.Salary3 = 150;

                            // Check if overall is between 80 and 90
                            if (playerRecord.Overall > 80 && playerRecord.Overall <= 90)
                            {
                                // Set Salary4 to a random value within a different specified range
                                int minSalary4 = 200; // Minimum value
                                int maxSalary4 = 400; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else if (playerRecord.Overall > 90)
                            {
                                // Set Salary4 to a random value within the specified range
                                int minSalary4 = 500; // Minimum value
                                int maxSalary4 = 800; // Maximum value
                                playerRecord.Salary4 = random.Next(minSalary4, maxSalary4 + 1); // Add 1 to include the upper bound

                                // Set Bonus4 to 300
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }
                            else
                            {
                                // Set Salary4 and Bonus4 to 0 if the overall is not greater than 80
                                playerRecord.Salary4 = 0;
                                playerRecord.Bonus4 = 0;

                                // Set Salary5, Salary6, Bonus5, Bonus6 to 0
                                playerRecord.Salary5 = 0;
                                playerRecord.Salary6 = 0;
                                playerRecord.Bonus5 = 0;
                                playerRecord.Bonus6 = 0;
                            }

                            playerRecord.TotalSalary = 450;
                            playerRecord.Bonus0 = 0;
                            playerRecord.Bonus1 = 0;
                            playerRecord.Bonus2 = 0;
                            playerRecord.Bonus3 = 0;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus3 plus Bonus4
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary3 plus Salary4
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }

                        else if (playerRecord.YearsPro > 3 && playerRecord.Overall >= 12 && playerRecord.Overall < 80)    //Low End Contract
                        {
                            // Generate random values for ContractLength and ContractYearsLeft

                            playerRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
                            playerRecord.ContractYearsLeft = playerRecord.ContractLength;

                            // Reset all salary and bonus values to 0
                            for (int i = 0; i <= 6; i++)
                            {
                                SetPropertyValue(playerRecord, $"Salary{i}", 0);
                                SetPropertyValue(playerRecord, $"Bonus{i}", 0);
                            }

                            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
                            int baseSalary = random.Next(75, 150); // Initial salary value
                            int baseBonus = random.Next(0, 50); // Initial bonus value
                            for (int i = 0; i < playerRecord.ContractLength; i++)
                            {
                                // Group positions with the same parameters
                                switch (playerRecord.PositionId)
                                {
                                    case 0: // QB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(175, 200) + additionalAmountTier1Low);
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(10, 20));
                                        break;

                                    case 13: // LOLB
                                    case 14: // MLB
                                    case 15: // ROLB
                                    case 3: //  WR
                                    case 10: // RE
                                    case 11: // LE
                                    case 12: // DT
                                    case 16: // CB
                                    case 5: //LT
                                    case 6: //LG
                                    case 7: //C
                                    case 8: //RG
                                    case 9: //RT
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(125, 150) + additionalAmountTier2Low);
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15));
                                        break;

                                    case 17: // SS
                                    case 18: // FS
                                    case 4: // TE
                                    case 1: // RB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100));
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 10));
                                        break;

                                    // Add more cases for other positions as needed
                                    // ...

                                    default:
                                        // Increment the base salary and bonus for each iteration
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75));
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15)); // Default bonus range
                                        break;
                                }
                            }
                            // Check if the total team salary exceeds the limit
                            if (GetTeamSalaryCap(playerRecord.TeamId) > 215.80)
                            {
                                // If the limit is exceeded, adjust salaries equally with a minimum Salary0 of 75
                                AdjustSalariesEqually(GetTeamPlayers(playerRecord.TeamId), 215.80, 75);
                            }

                            changesApplied = true;

                            // Calculate TotalBonus as the sum of Bonus0-Bonus6
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary6
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);


                            changesApplied = true;
                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro > 3 && playerRecord.Overall >= 80 && playerRecord.Overall < 85)   // Mid Level Contract
                        {
                            // Generate random values for ContractLength and ContractYearsLeft

                            playerRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
                            playerRecord.ContractYearsLeft = playerRecord.ContractLength;

                            // Reset all salary and bonus values to 0
                            for (int i = 0; i <= 6; i++)
                            {
                                SetPropertyValue(playerRecord, $"Salary{i}", 0);
                                SetPropertyValue(playerRecord, $"Bonus{i}", 0);
                            }

                            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
                            int baseSalary = random.Next(250, 550); // Initial salary value
                            int baseBonus = random.Next(10, 100); // Initial bonus value
                            for (int i = 0; i < playerRecord.ContractLength; i++)
                            {
                                // Group positions with the same parameters
                                switch (playerRecord.PositionId)
                                {
                                    case 0: // QB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(175, 200) + additionalAmountTier1Mid);   //verified 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(50, 75));
                                        break;

                                    case 13: // LOLB
                                    case 14: // MLB
                                    case 15: // ROLB
                                    case 3: //  WR
                                    case 10: // RE
                                    case 11: // LE
                                    case 12: // DT
                                    case 16: // CB
                                    case 5: //LT
                                    case 6: //LG
                                    case 7: //C
                                    case 8: //RG
                                    case 9: //RT
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(125, 150));
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(15, 25));
                                        break;

                                    case 17: // SS
                                    case 18: // FS
                                    case 4: // TE
                                    case 1: // RB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100));
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 10));
                                        break;

                                    // Add more cases for other positions as needed
                                    // ...

                                    default:
                                        // Increment the base salary and bonus for each iteration
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75) + additionalAmountTier4Mid);
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15));
                                        break;
                                }
                            }

                            // Calculate TotalBonus as the sum of Bonus0-Bonus6
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary6
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true;

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                            for (int teamId = 1; teamId <= 32; teamId++)
                            {
                                double teamSalaryCap = GetTeamSalaryCap(teamId);

                                if (teamSalaryCap > 224.83 && playerRecord.Overall > 80)
                                {
                                    // Subtract 10% from the player's salary
                                    for (int i = 0; i < playerRecord.ContractLength; i++)
                                    {
                                        string salaryPropertyName = $"Salary{i}";
                                        var currentSalaryObj = playerRecord.GetType().GetProperty(salaryPropertyName)?.GetValue(playerRecord);

                                        if (currentSalaryObj != null && currentSalaryObj is int)
                                        {
                                            int currentSalary = (int)currentSalaryObj;
                                            int newSalary = (int)(currentSalary * 0.99); // Subtracting 10%
                                            playerRecord.GetType().GetProperty(salaryPropertyName)?.SetValue(playerRecord, newSalary);
                                        }
                                    }
                                }
                                else
                                {
                                    // Team is not over the cap or player's overall is <= 80, no salary reduction needed
                                }
                            }
                        }
                        else if (playerRecord.YearsPro > 3 && playerRecord.Overall >= 85 && playerRecord.Overall < 90)   // High End Contract
                        {
                            // Generate random values for ContractLength and ContractYearsLeft

                            playerRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
                            playerRecord.ContractYearsLeft = playerRecord.ContractLength;

                            // Reset all salary and bonus values to 0
                            for (int i = 0; i <= 6; i++)
                            {
                                SetPropertyValue(playerRecord, $"Salary{i}", 0);
                                SetPropertyValue(playerRecord, $"Bonus{i}", 0);
                            }

                            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
                            int baseSalary = random.Next(350, 950); // Initial salary value
                            int baseBonus = random.Next(50, 300); // Initial bonus value
                            for (int i = 0; i < playerRecord.ContractLength; i++)
                            {
                                // Group positions with the same parameters
                                switch (playerRecord.PositionId)
                                {
                                    case 0: // QB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(400, 450) + additionalAmountTier1High);  //verified 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(50, 75));
                                        break;

                                    case 13: // LOLB
                                    case 14: // MLB
                                    case 15: // ROLB
                                    case 3: //  WR
                                    case 10: // RE
                                    case 11: // LE
                                    case 12: // DT
                                    case 16: // CB
                                    case 5: //LT
                                    case 6: //LG
                                    case 7: //C
                                    case 8: //RG
                                    case 9: //RT
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(225, 250)); //verified 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(15, 25));
                                        break;

                                    case 17: // SS
                                    case 18: // FS
                                    case 4: // TE
                                    case 1: // RB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(75, 100));   //verified 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 10));
                                        break;

                                    // Add more cases for other positions as needed
                                    // ...

                                    default:
                                        // Increment the base salary and bonus for each iteration
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75) + additionalAmountTier4High);    //verified 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15) + addittionalBonusAmountForTier4High);  //verified 11-19-23
                                        break;
                                }
                            }

                            // Calculate TotalBonus as the sum of Bonus0-Bonus6
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary6
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true;
                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements

                            for (int teamId = 1; teamId <= 32; teamId++)
                            {
                                double teamSalaryCap = GetTeamSalaryCap(teamId);

                                if (teamSalaryCap > 224.83 && playerRecord.Overall > 85)
                                {
                                    // Subtract 10% from the player's salary
                                    for (int i = 0; i < playerRecord.ContractLength; i++)
                                    {
                                        string salaryPropertyName = $"Salary{i}";
                                        var currentSalaryObj = playerRecord.GetType().GetProperty(salaryPropertyName)?.GetValue(playerRecord);

                                        if (currentSalaryObj != null && currentSalaryObj is int)
                                        {
                                            int currentSalary = (int)currentSalaryObj;
                                            int newSalary = (int)(currentSalary * 0.99); // Subtracting 10%
                                            playerRecord.GetType().GetProperty(salaryPropertyName)?.SetValue(playerRecord, newSalary);
                                        }
                                    }
                                }
                                else
                                {
                                    // Team is not over the cap or player's overall is <= 80, no salary reduction needed
                                }
                            }

                            // Calculate TotalBonus as the sum of Bonus0-Bonus6
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary6
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);



                            changesApplied = true;
                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        else if (playerRecord.YearsPro > 3 && playerRecord.Overall >= 90 && playerRecord.Overall < 125)   // Elite End Contract
                        {
                            // Generate random values for ContractLength and ContractYearsLeft

                            playerRecord.ContractLength = random.Next(1, 8); // Assuming a maximum contract length of 6, adjust as needed
                            playerRecord.ContractYearsLeft = playerRecord.ContractLength;

                            // Reset all salary and bonus values to 0
                            for (int i = 0; i <= 6; i++)
                            {
                                SetPropertyValue(playerRecord, $"Salary{i}", 0);
                                SetPropertyValue(playerRecord, $"Bonus{i}", 0);
                            }

                            // Generate random values for Salary0-Salary(N-1) and Bonus0-Bonus(N-1) based on the contract length
                            int baseSalary = random.Next(1050, 1550); // Initial salary value
                            int baseBonus = random.Next(100, 300); // Initial bonus value
                            for (int i = 0; i < playerRecord.ContractLength; i++)
                            {
                                // Group positions with the same parameters
                                switch (playerRecord.PositionId)
                                {
                                    case 0: // QB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(400, 425) + additionalAmountForTier1Elite); //verfied 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(100, 150));
                                        break;

                                    case 13: // LOLB
                                    case 14: // MLB
                                    case 15: // ROLB
                                    case 3: //  WR
                                    case 10: // RE
                                    case 11: // LE
                                    case 12: // DT
                                    case 16: // CB
                                    case 5: //LT
                                    case 6: //LG
                                    case 7: //C
                                    case 8: //RG
                                    case 9: //RT
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(325, 350));
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(75, 100));
                                        break;

                                    case 17: // SS
                                    case 18: // FS
                                    case 4: // TE
                                    case 1: // RB
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(225, 250));
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(50, 75));
                                        break;

                                    // Add more cases for other positions as needed
                                    // ...

                                    default:
                                        // Increment the base salary and bonus for each iteration
                                        SetPropertyValue(playerRecord, $"Salary{i}", baseSalary + i * random.Next(50, 75) + additionalAmountForTier4Elite);    //verified 11-19-23
                                        SetPropertyValue(playerRecord, $"Bonus{i}", baseBonus + i * random.Next(5, 15) + addittionalBonusAmountForTier4Elite); //verfiied 11-19-23
                                        break;

                                }
                            }
                            // Calculate TotalBonus as the sum of Bonus0-Bonus6
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary6
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true;


                            for (int teamId = 1; teamId <= 32; teamId++)
                            {
                                double teamSalaryCap = GetTeamSalaryCap(teamId);

                                if (teamSalaryCap > 224.83 && playerRecord.Overall >= 95)
                                {
                                    // Subtract 10% from the player's salary
                                    for (int i = 0; i < playerRecord.ContractLength; i++)
                                    {
                                        string salaryPropertyName = $"Salary{i}";
                                        var currentSalaryObj = playerRecord.GetType().GetProperty(salaryPropertyName)?.GetValue(playerRecord);

                                        if (currentSalaryObj != null && currentSalaryObj is int)
                                        {
                                            int currentSalary = (int)currentSalaryObj;
                                            int newSalary = (int)(currentSalary * 0.99); // Subtracting 10%
                                            playerRecord.GetType().GetProperty(salaryPropertyName)?.SetValue(playerRecord, newSalary);
                                        }
                                    }
                                }
                                else
                                {
                                    // Team is not over the cap or player's overall is <= 80, no salary reduction needed
                                }
                            }

                            // Calculate TotalBonus as the sum of Bonus0-Bonus6
                            playerRecord.TotalBonus = CalculateTotalBonus(playerRecord);

                            // Calculate TotalSalary as the sum of Salary0-Salary6
                            playerRecord.TotalSalary = CalculateTotalSalary(playerRecord);

                            changesApplied = true;
                        }

                        // Add more conditions for additional weight ranges if needed

                        // Update UI elements to reflect the changes
                        UpdateUI(); // Call a method to update UI elements
                    }

                    // Display a message indicating that changes were applied
                    if (changesApplied)
                    {
                        MessageBox.Show("The Mass Contract Generator have successfully been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Display a message indicating that the conditions were not met for applying changes
                        MessageBox.Show("Conditions not met for applying changes to any entry.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Helper method to check if the team salary exceeds the limit
        private bool IsTeamSalaryExceedingLimit(PlayerRecord playerRecord, double limit, Func<int, List<PlayerRecord>> getTeamPlayers)
        {
            // Implement logic to get the current total salary for the team with 'teamId'
            double currentTeamSalary = GetTeamTotalSalary(playerRecord.TeamId, getTeamPlayers);

            // Check if the total salary after adding the player's contract exceeds the limit
            return (currentTeamSalary + playerRecord.TotalSalary) > limit;
        }


        private void AdjustSalariesEqually(List<PlayerRecord> players, double limit, int minSalary0)
        {
            double totalTeamSalaryBefore = players.Sum(player => player.TotalSalary);

            // Check if the total team salary exceeds the limit
            if (totalTeamSalaryBefore > limit)
            {
                // Calculate the excess amount
                double excess = totalTeamSalaryBefore - limit;

                // Determine the adjustment per player
                double adjustmentPerPlayer = excess / players.Count;

                Console.WriteLine($"Initial Total Team Salary: {totalTeamSalaryBefore}, Limit: {limit}");
                Console.WriteLine($"Excess: {excess}, Adjustment Per Player: {adjustmentPerPlayer}");

                foreach (PlayerRecord player in players)
                {
                    Console.WriteLine($"Player {player.PlayerId} - Before Adjustment: Total Salary: {player.TotalSalary}");

                    // Adjust Salary0 with a minimum of 75
                    int salary0 = (int)player.GetType().GetProperty("Salary0").GetValue(player, null);
                    int adjustedSalary0 = Math.Max(minSalary0, salary0 - (int)(adjustmentPerPlayer));
                    player.GetType().GetProperty("Salary0").SetValue(player, adjustedSalary0);

                    // Adjust the remaining salary and bonus equally
                    for (int i = 1; i <= 6; i++)
                    {
                        int currentSalary = (int)player.GetType().GetProperty($"Salary{i}").GetValue(player, null);
                        int currentBonus = (int)player.GetType().GetProperty($"Bonus{i}").GetValue(player, null);

                        int adjustedSalary = Math.Max(0, currentSalary - (int)(adjustmentPerPlayer));
                        int adjustedBonus = Math.Max(0, currentBonus - (int)(adjustmentPerPlayer));

                        player.GetType().GetProperty($"Salary{i}").SetValue(player, adjustedSalary);
                        player.GetType().GetProperty($"Bonus{i}").SetValue(player, adjustedBonus);
                    }

                    // Recalculate TotalBonus and TotalSalary
                    player.TotalBonus = CalculateTotalBonus(player);
                    player.TotalSalary = CalculateTotalSalary(player);

                    Console.WriteLine($"Player {player.PlayerId} - After Adjustment: Total Salary: {player.TotalSalary}");
                }

                double totalTeamSalaryAfter = players.Sum(player => player.TotalSalary);
                Console.WriteLine($"Final Total Team Salary: {totalTeamSalaryAfter}");
            }
        }

        

        // Helper method to get the current total salary for a team
        private double GetTeamTotalSalary(int teamId, Func<int, List<PlayerRecord>> getTeamPlayers)
        {
            // Use your existing method to calculate the total team salary
            List<PlayerRecord> teamPlayers = getTeamPlayers(teamId);
            return teamPlayers.Sum(player => player.TotalSalary);
        }

        // Placeholder method to get the list of players for a team
        private List<PlayerRecord> GetTeamPlayers(int teamId)
        {
            // Implement this method based on your application's structure
            // It should return the list of PlayerRecord objects for the specified teamId
            // You might need to fetch players from your data source (e.g., database or list)
            // and filter them based on the teamId
            // For simplicity, I'm assuming a List<PlayerRecord> for demonstration purposes
            return new List<PlayerRecord>(); // You should replace this with your actual logic
        }

        private void TraitsGeneratorbutton_Click(object sender, EventArgs e)    //New 12-12-23
        {
            try
            {
                // Check if the model's MadVersion is not Ver2019
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    bool changesApplied = false; // Flag to track if any changes were applied

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;


                        if (playerRecord.PlayerType == 33 || playerRecord.PlayerType == 36) //Pass Coverage LBs
                        {

                            playerRecord.SidelineCatch = 2;     // This is LB Style - Pass Coverage

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI elements
                        }
                        if (playerRecord.PlayerType == 25 || playerRecord.PlayerType == 26 || playerRecord.PlayerType == 29 || playerRecord.PlayerType == 30 || playerRecord.PlayerType == 31 || playerRecord.PlayerType == 32) //Pass Rush LBs
                        {
                            playerRecord.SidelineCatch = 0;     // This is LB Style - Pass Rush

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.PlayerType == 0 || playerRecord.PlayerType == 1) // Field Gen and Strong Arm
                        {
                            playerRecord.TuckRun = 1;     // This is QB Style - Pocket

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.PlayerType == 3 || playerRecord.PlayerType == 2 && playerRecord.Speed >= 85) // Scrambler + Improvisier 85+ speed
                        {
                            playerRecord.TuckRun = 2;     // This is QB Style - Scramble

                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Awareness >= 85) // Penalty Trait
                        {
                            playerRecord.Penalty = 2;     // Disciplined


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Awareness >= 70) // Penalty Trait
                        {
                            playerRecord.Penalty = 1;     // Normal


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Awareness <= 69) // Penalty Trait
                        {
                            playerRecord.Penalty = 0;     // Normal


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.HitPower >= 85) // Big Hitter Trait
                        {
                            playerRecord.BigHitter = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.HitPower <= 84) // Big Hitter Trait
                        {
                            playerRecord.BigHitter = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.PositionId == 0 && playerRecord.Awareness >= 75) // Throw Away Trait
                        {
                            playerRecord.ThrowAway = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Awareness <= 74) // Throw Away Trait
                        {
                            playerRecord.ThrowAway = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Overall >= 90) // High Motor Trait
                        {
                            playerRecord.HighMotor = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Overall <= 89) // High Motor Trait
                        {
                            playerRecord.HighMotor = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Overall >= 95) // Clutch Trait
                        {
                            playerRecord.Clutch = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Overall <= 94) // Clutch Trait
                        {
                            playerRecord.Clutch = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Catching >= 75) // Drop Passes Trait
                        {
                            playerRecord.DropPasses = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Catching <= 74) // Drop Passes Trait
                        {
                            playerRecord.DropPasses = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Trucking >= 75 || playerRecord.BreakTackle >= 75 || playerRecord.BreakSack >= 75)  // Fight Yards Trait
                        {
                            playerRecord.FightYards = true;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Trucking <= 74 || playerRecord.BreakTackle <= 74 || playerRecord.BreakSack <= 74)  // Fight Yards Trait
                        {
                            playerRecord.FightYards = false;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.PositionId == 0 && playerRecord.ThrowPower >= 65) // Throw Spiral Trait
                        {
                            playerRecord.ThrowSpiral = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.ThrowPower <= 75) // Throw Spiral Trait
                        {
                            playerRecord.ThrowSpiral = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.BlockShedding >= 80) // Bullrush Trait
                        {
                            playerRecord.DLBullrush = true;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.BlockShedding <= 80) // Bullrush Trait
                        {
                            playerRecord.DLBullrush = false;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.PowerMoves >= 80) // Swim Move Trait
                        {
                            playerRecord.DLSwim = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.PowerMoves <= 80) // Swim Move Trait
                        {
                            playerRecord.DLSwim = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.FinesseMoves >= 80) // DL Spin Move Trait
                        {
                            playerRecord.DLSpinmove = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.FinesseMoves <= 80) // Swim Move Trait
                        {
                            playerRecord.DLSpinmove = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.SpecCatch >= 75) // Aggressive Catch Trait
                        {
                            playerRecord.AggressiveCatch = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.SpecCatch <= 75) // Aggressive Catch Trait
                        {
                            playerRecord.AggressiveCatch = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.CatchTraffic >= 75) // Possession Catch Trait
                        {
                            playerRecord.PossessionCatch = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.CatchTraffic <= 75) // Possession Catch Trait
                        {
                            playerRecord.PossessionCatch = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Awareness >= 80 && playerRecord.Tackle >= 75) // Strips Ball Trait
                        {
                            playerRecord.StripsBall = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Awareness <= 79 && playerRecord.Tackle <= 74) // Strips Ball Trait
                        {
                            playerRecord.StripsBall = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Awareness >= 80 && playerRecord.PositionId == 3 || playerRecord.Awareness >= 80 && playerRecord.PositionId == 4 || playerRecord.Awareness >= 80 && playerRecord.PlayerType == 6) // Feet in Bounds Trait
                        {
                            playerRecord.FeetInBounds = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Awareness <= 79 && playerRecord.PositionId == 3 || playerRecord.Awareness <= 79 && playerRecord.PositionId == 4 || playerRecord.Awareness <= 79 && playerRecord.PlayerType == 6) // Feet in Bounds Trait
                        {
                            playerRecord.FeetInBounds = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Awareness >= 75 && playerRecord.PositionId == 3 || playerRecord.Awareness >= 75 && playerRecord.PositionId == 4 || playerRecord.Awareness >= 75 && playerRecord.PlayerType == 6) // Run After Catch Trait
                        {
                            playerRecord.RunAfterCatch = true;     // Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        else if (playerRecord.Awareness <= 74 && playerRecord.PositionId == 3 || playerRecord.Awareness <= 74 && playerRecord.PositionId == 4 || playerRecord.Awareness <= 74 && playerRecord.PlayerType == 6) // Run After Catch Trait
                        {
                            playerRecord.RunAfterCatch = false;     // Not Active


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Carrying >= 85) // Cover Ball Trait
                        {
                            playerRecord.CoversBall = 4;     // Brace for All


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Carrying >= 80) // Cover Ball Trait
                        {
                            playerRecord.CoversBall = 2;     // Brace Big


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Carrying >= 75) // Cover Ball Trait
                        {
                            playerRecord.CoversBall = 3;     // Brace Medium


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.Carrying <= 74) // Cover Ball Trait
                        {
                            playerRecord.CoversBall = 1;     // Brace Medium


                            changesApplied = true; // Set the flag to true

                            // Update UI elements to reflect the changes
                            UpdateUI(); // Call a method to update UI element
                        }
                        if (playerRecord.PositionId == 0)
                        {
                            if (playerRecord.Awareness >= 90)
                            {
                                playerRecord.SensePressure = 2; // Active
                            }
                            else if (playerRecord.Awareness >= 80)
                            {
                                playerRecord.SensePressure = 3; // Not Active
                            }
                            else if (playerRecord.Awareness >= 70)
                            {
                                playerRecord.SensePressure = 0; // Not Active
                            }
                            else if (playerRecord.Awareness >= 60)
                            {
                                playerRecord.SensePressure = 1; // Not Active
                            }
                            else if (playerRecord.Awareness <= 50)
                            {
                                playerRecord.SensePressure = 4; // Not Active
                            }

                            // Common actions for all cases
                            changesApplied = true;
                            UpdateUI();
                        }

                        if (playerRecord.PositionId == 0)
                        {
                            if (playerRecord.Awareness >= 90)
                            {
                                playerRecord.ForcePasses = 1; // Active
                            }
                            else if (playerRecord.Awareness >= 80)
                            {
                                playerRecord.ForcePasses = 2; // Not Active
                            }
                            else if (playerRecord.Awareness <= 70)
                            {
                                playerRecord.ForcePasses = 0; // Not Active
                            }

                            // Common actions for all cases
                            changesApplied = true;
                            UpdateUI();
                        }

                    }

                    // Display a message if changes were applied
                    if (changesApplied)
                    {
                        MessageBox.Show("Generated Traits has been applied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No changes were applied.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrimePFRbutton_Click(object sender, EventArgs e)        //new 12-23-23
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "Prime's PFR AV Madden Encyclopedia v3.0.xlsm");

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Open the file with the default text editor
                    Process.Start(filePath);
                }
                else
                {
                    MessageBox.Show("PortingInstructions.txt not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label301_Click(object sender, EventArgs e)
        {

        }

        private void label48_Click(object sender, EventArgs e)
        {

        }

        // TO DO :
        // qb player style is tendency
        // LB style ?

    }
}
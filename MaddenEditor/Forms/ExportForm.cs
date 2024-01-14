/******************************************************************************
 * MaddenAmp
 * Copyright (C) 2018 StingRay68
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaddenEditor.Core;
using MaddenEditor.Core.Record;
using MaddenEditor.Db;
using Newtonsoft.Json.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;

namespace MaddenEditor.Forms
{
    public partial class ExportForm : Form, IEditorForm
    {
        private bool isInitializing = false;
        private EditorModel model = null;
        public List<string> tables_avail = new List<string>();
        public List<string> tables_export = new List<string>();
        public Dictionary<string, List<string>> fields_avail = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> fields_export = new Dictionary<string, List<string>>();
        public MaddenFileVersion csvVersion;
        public Dictionary<int, string> import_fields_avail = new Dictionary<int, string>();
        public List<string> import_fields = new List<string>();
        public string currenttablename = "";
        public int linenumber = 0;
        public List<string> errors = new List<string>();
        public int currentrec;
        private string sourceFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Madden NFL 24");  //new 12-23-23
        private string tempRosterStorageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Madden NFL 24", "temprosterstorage");  //new 12-23-23
        private string savesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Madden NFL 24", "saves");  //new 12-23-23

        List<List<string>> CSVRecords = new List<List<string>>();
        public FB FB_Draft = new FB();

        public ExportForm(EditorModel model)
        {
            this.model = model;
            FB_Draft = new FB();
            InitializeComponent();
        }

        #region IEditorForm Members

        public EditorModel Model
        {
            set { }
        }

        # region College List for Export
        // Would like to use the list in PlayerEditControl but how to do it?
        public string[] collegenames =
            {
            "Abilene Chr.",
            "Air Force",
            "Akron",
            "Alabama",
            "Alabama A&M",
            "Alabama St.",
            "Alcorn St.",
            "Appalach. St.",
            "Arizona    ",
            "Arizona St.",
            "Arkansas",
            "Arkansas P.B.",
            "Arkansas St.",
            "Army",
            "Auburn",
            "Austin Peay",
            "Ball State",
            "Baylor",
            "Beth Cookman",
            "Boise State",
            "Boston Coll.",
            "Bowl. Green",
            "Brown",
            "Bucknell",
            "Buffalo",
            "Butler",
            "BYU",
            "Cal Poly SLO",
            "California",
            "Cal-Nrthridge",
            "Cal-Sacrmnto",
            "Canisius",
            "Cent Conn St.    ",
            "Central MI   ",
            "Central St Ohio    ",
            "Charleston S.    ",
            "Cincinnati   ",
            "Citadel    ",
            "Clemson   ",
            "Clinch Valley    ",
            "Colgate    ",
            "Colorado   ",
            "Colorado St.   ",
            "Columbia  ",
            "Cornell  ",
            "Culver-Stockton    ",
            "Dartmouth  ",
            "Davidson    ",
            "Dayton    ",
            "Delaware    ",
            "Delaware St.  ",
            "Drake    ",
            "Duke   ",
            "Duquesne    ",
            "E. Carolina   ",
            "E. Illinois    ",
            "E. Kentucky    ",
            "E. Tenn. St.   ",
            "East. Mich.  ",
            "Eastern Wash.   ",
            "Elon College ",
            "Fairfield  ",
            "Florida    ",
            "Florida A&M",
            "Florida State",
            "Fordham  ",
            "Fresno State   ",
            "Furman     ",
            "Ga. Southern ",
            "Georgetown  ",
            "Georgia     ",
            "Georgia Tech    ",
            "Grambling St. ",
            "Grand Valley St.    ",
            "Hampton    ",
            "Harvard     ",
            "Hawaii       ",
            "Henderson St.    ",
            "Hofstra          ",
            "Holy Cross       ",
            "Houston       ",
            "Howard        ",
            "Idaho           ",
            "Idaho State   ",
            "Illinois     ",
            "Illinois St.    ",
            "Indiana       ",
            "Indiana St.    ",
            "Iona            ",
            "Iowa           ",
            "Iowa State    ",
            "J. Madison     ",
            "Jackson St.    ",
            "Jacksonv. St.    ",
            "John Carroll    ",
            "Kansas        ",
            "Kansas State   ",
            "Kent State      ",
            "Kentucky      ",
            "Kutztown        ",
            "La Salle        ",
            "LA. Tech        ",
            "Lambuth           ",
            "Lehigh           ",
            "Liberty         ",
            "Louisville      ",
            "LSU              ",
            "M. Valley St.  ",
            "Maine           ",
            "Marist           ",
            "Marshall         ",
            "Maryland         ",
            "Massachusetts    ",
            "McNeese St.     ",
            "Memphis         ",
            "Miami           ",
            "Miami Univ.      ",
            "Michigan         ",
            "Michigan St.     ",
            "Mid Tenn St.     ",
            "Minnesota        ",
            "Miss. State      ",
            "Missouri         ",
            "Monmouth         ",
            "Montana           ",
            "Montana State   ",
            "Morehead St.   ",
            "Morehouse      ",
            "Morgan St.    ",
            "Morris Brown       ",
            "Mt S. Antonio      ",
            "Murray State       ",
            "N. Alabama        ",
            "N. Arizona       ",
            "N. Car A&T       ",
            "N. Carolina       ",
            "N. Colorado       ",
            "N. Illinois       ",
            "N.C. State      ",
            "Navy             ",
            "NC Central        ",
            "Nebr.-Omaha      ",
            "Nebraska        ",
            "Nevada          ",
            "New Mex. St.      ",
            "New Mexico        ",
            "Nicholls St.     ",
            "Norfolk State    ",
            "North Texas     ",
            "Northeastern      ",
            "Northern Iowa      ",
            "Northwestern    ",
            "Notre Dame        ",
            "NW Oklahoma St.   ",
            "N\'western St.     ",
            "Ohio              ",
            "Ohio State        ",
            "Oklahoma         ",
            "Oklahoma St.     ",
            "Ole Miss        ",
            "Oregon           ",
            "Oregon State     ",
            "P. View A&M      ",
            "Penn             ",
            "Penn State      ",
            "Pittsburg St.   ",
            "Pittsburgh       ",
            "Portland St.     ",
            "Princeton       ",
            "Purdue           ",
            "Rhode Island     ",
            "Rice             ",
            "Richmond         ",
            "Robert Morris      ",
            "Rowan             ",
            "Rutgers         ",
            "S. Carolina     ",
            "S. Dakota St.      ",
            "S. Illinois       ",
            "S.C. State       ",
            "S.D. State      ",
            "S.F. Austin        ",
            "Sacred Heart      ",
            "Sam Houston        ",
            "Samford            ",
            "San Jose St.      ",
            "Savannah St.       ",
            "SE Missouri        ",
            "SE Missouri St.    ",
            "Shippensburg       ",
            "Siena              ",
            "Simon Fraser      ",
            "SMU              ",
            "Southern        ",
            "Southern Miss     ",
            "Southern Utah    ",
            "St. Francis      ",
            "St. John\'s        ",
            "St. Mary\'s        ",
            "St. Peters        ",
            "Stanford         ",
            "Stony Brook        ",
            "SUNY Albany        ",
            "SW Miss St        ",
            "SW Texas St.      ",
            "Syracuse         ",
            "T A&M K\'ville     ",
            "TCU              ",
            "Temple          ",
            "Tenn. Tech        ",
            "Tenn-Chat         ",
            "Tennessee         ",
            "Tennessee St.      ",
            "Tenn-Martin        ",
            "Texas             ",
            "Texas A&M         ",
            "Texas South.     ",
            "Texas Tech        ",
            "Toledo           ",
            "Towson State       ",
            "Troy State       ",
            "Tulane            ",
            "Tulsa             ",
            "Tuskegee           ",
            "UAB               ",
            "UCF               ",
            "UCLA              ",
            "UConn            ",
            "UL Lafayette      ",
            "UL Monroe         ",
            "UNLV             ",
            "USC            ",
            "USF             ",
            "Utah             ",
            "Utah State      ",
            "UTEP            ",
            "Valdosta St.    ",
            "Valparaiso       ",
            "Vanderbilt       ",
            "Villanova          ",
            "Virginia        ",
            "Virginia Tech    ",
            "VMI                ",
            "W. Carolina      ",
            "W. Illinois      ",
            "W. Kentucky       ",
            "W. Michigan      ",
            "W. Texas A&M    ",
            "Wagner           ",
            "Wake Forest      ",
            "Walla Walla      ",
            "Wash. St.       ",
            "Washington       ",
            "Weber State      ",
            "West Virginia  ",
            "Westminster      ",
            "Will. & Mary    ",
            "Winston Salem    ",
            "Wisconsin      ",
            "Wofford         ",
            "Wyoming        ",
            "Yale            ",
            "Youngstwn St.    ",
            "Sonoma St.       ",
            "No College       ",
            "N/A               ",
            "New Hampshire      ",
            "UW Lacrosse       ",
            "Hastings College    ",
            "Midwestern St.     ",
            "North Dakota       ",
            "Wayne State        ",
            "UW Stevens Pt.   ",
            "Indiana(Penn.)    ",
            "Saginaw Valley    ",
            "Central St.(OK)   ",
            "Emporia State     "
            };
        #endregion

        public void InitialiseUI()
        {
            isInitializing = true;
            InitTables();

            if (model.MadVersion >= MaddenFileVersion.Ver2019)
            {
                ExportFilter_Panel.Visible = true;
                filterDraftClassCheckbox.Enabled = false;
                MainSkillsOnly_Checkbox.Enabled = false;
                ExportVersion.SelectedIndex = 0;
                if (model.MadVersion == MaddenFileVersion.Ver2020)
                    ExportVersion.SelectedIndex = 1;
            }

            else ExportFilter_Panel.Visible = true;

            ColumnHeader header = new ColumnHeader();
            header.Text = "Tables";
            header.Name = "Tables";
            AvailTables_ListView.Columns.Add(header);
            AvailTables_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            InitAvailableTablesList();

            header = new ColumnHeader();
            header.Text = "Export Tables";
            header.Name = "Export Tables";
            ExportTables_ListView.Columns.Add(header);
            ExportTables_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            InitExportTables();

            header = new ColumnHeader();
            header.Text = "Fields";
            header.Name = "Fields";
            AvailFields_ListView.Columns.Add(header);
            AvailFields_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            header = new ColumnHeader();
            header.Text = "Export Fields";
            header.Name = "Export Fields";
            ExportFields_ListView.Columns.Add(header);
            ExportFields_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            if (model.FileType != MaddenFileType.Streameddata && model.FileType != MaddenFileType.DataRam && model.FileType != MaddenFileType.StaticData)
            {
                foreach (TeamRecord team in model.TeamModel.GetTeams())
                {
                    filterTeamCombo.Items.Add(team);
                }

                for (int p = 0; p < 21; p++)
                {
                    string pos = Enum.GetName(typeof(MaddenPositions), p);
                    filterPositionCombo.Items.Add(pos);
                }

                foreach (KeyValuePair<int, college_entry> col in model.Colleges)    //11-11-23
                {
                    string name = col.Value.name;
                    FilterCollegeComboBox.Items.Add(name);

                }

                    {
                    filterArchetypeComboBox.Items.Add("Field General QB"); // 11-11-23
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
                    DevTraitComboBox.Items.Add("Normal");
                    DevTraitComboBox.Items.Add("Star"); 
                    DevTraitComboBox.Items.Add("Superstar"); 
                    DevTraitComboBox.Items.Add("X-Factor"); 
                }

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

                {
                    ProBowlComboBox.Items.Add("All Players");
                    ProBowlComboBox.Items.Add("Pro Bowlers Only");
                }

                filterPositionCombo.Text = filterPositionCombo.Items[0].ToString();
                filterTeamCombo.Text = filterTeamCombo.Items[0].ToString();
                filterArchetypeComboBox.Text = filterArchetypeComboBox.Items[0].ToString(); //11-11-23
                FilterCollegeComboBox.Text = FilterCollegeComboBox.Items[0].ToString(); //11-11-23
                DevTraitComboBox.Text = DevTraitComboBox.Items[0].ToString();   //11-11-23
                FilterYearsProcomboBox.Text = FilterYearsProcomboBox.Items[0].ToString();   //11-11-23
                ProBowlComboBox.Text = ProBowlComboBox.Items[0].ToString();   //11-11-23
            }
            isInitializing = false;

            ProcessRecords_Button.Enabled = false;
        }

        public void CleanUI()
        {
            AvailTables_ListView.Items.Clear();
            filterTeamCombo.Items.Clear();  
            filterPositionCombo.Items.Clear();
            filterArchetypeComboBox.Items.Clear();  //11-11-23
            FilterCollegeComboBox.Items.Clear();    //11-11-23
            DevTraitComboBox.Items.Clear(); //11-11-23
            FilterYearsProcomboBox.Items.Clear(); //11-11-23
            ProBowlComboBox.Items.Clear(); //11-11-23
        }

        #endregion

        public void SetSerial(FB roster)
        {
            this.FB_Draft.Serial = roster.Serial.ToArray();
        }

        public string ConvertBE(string name)
        {
            char[] charArray = name.ToCharArray();
            Array.Reverse(charArray);
            string rev = new string(charArray);
            return rev;
        }

        #region Inits
        public void InitTables()
        {
            AvailTables_ListView.Items.Clear();
            tables_avail.Clear();
            fields_avail.Clear();
            tables_export.Clear();

            foreach (KeyValuePair<string, int> pair in model.TableNames)
            {
                string name = pair.Key;
                if (model.BigEndian)
                    name = ConvertBE(pair.Key);
                tables_avail.Add(name);
                AvailTables_ListView.Items.Add(name);

                if (!model.TableModels.ContainsKey(pair.Key))
                {
                    model.ProcessTable(model.TableNames[pair.Key]);
                }

                TableModel table = model.TableModels[name];
                List<TdbFieldProperties> props = table.GetFieldList();
                List<string> fields = new List<string>();
                foreach (TdbFieldProperties p in props)
                    fields.Add(p.Name);
                fields.Sort();
                fields_avail.Add(name, new List<string>(fields));
                fields_export.Add(name, new List<string>(fields));
            }
            tables_avail.Sort();
        }

        public void InitAvailableTablesList()
        {
            AvailTables_ListView.Items.Clear();
            foreach (string s in tables_avail)
            {
                AvailTables_ListView.Items.Add(s);
            }
        }

        public void InitExportTables()
        {
            ExportTables_ListView.Items.Clear();
            foreach (string s in tables_export)
            {
                ExportTables_ListView.Items.Add(s);
            }
        }

        public void InitAvailableFields(string name)
        {
            AvailFields_ListView.Items.Clear();
            foreach (string f in fields_avail[name])
            {
                AvailFields_ListView.Items.Add(f);
            }
        }

        public void InitExportFields(string name)
        {
            ExportFields_ListView.Items.Clear();
            foreach (string s in fields_export[name])
                ExportFields_ListView.Items.Add(s);
        }

        public void InitProcessButton()
        {
            if (UpdateRecs_Checkbox.Checked == false && DeleteCurrentRecs_Checkbox.Checked == false)
                ProcessRecords_Button.Enabled = false;
            else ProcessRecords_Button.Enabled = true;
        }

        #endregion


        private string GetDirectory()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Choose DIR for Table Extraction";
            if (folderDialog.ShowDialog() == DialogResult.OK)
                return folderDialog.SelectedPath;
            return "";
        }

        private void ExportPlay_Button_Click(object sender, EventArgs e)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            if (tables_export.Count == 0)
                return;
            string dir = "";
            if (ExtractByTableName.Checked)
                dir = GetDirectory();

            foreach (string t in tables_export)
            {
                try
                {
                    string tablename = t;
                    if (model.BigEndian)
                        tablename = ConvertBE(t);

                    string filename = "";
                    if (ExtractByTableName.Checked)
                        filename = Path.Combine(dir, t + ".csv");
                    else
                    {
                        SaveFileDialog fileDialog = new SaveFileDialog();
                        fileDialog.Filter = t + " as csv file (*.csv)|*.csv|All files (*.*)|*.*";
                        fileDialog.RestoreDirectory = true;

                        if (fileDialog.ShowDialog() == DialogResult.OK)
                            filename = fileDialog.FileName;
                    }

                    if (filename == "")
                        continue;

                    StreamWriter writer = new StreamWriter(filename);
                    StringBuilder hbuilder = new StringBuilder();

                    hbuilder.Append(t);
                    hbuilder.Append(",");
                    string version = "2008";
                    if (model.MadVersion == MaddenFileVersion.Ver2004)
                        version = "2004";
                    else if (model.MadVersion == MaddenFileVersion.Ver2005)
                        version = "2005";
                    else if (model.MadVersion == MaddenFileVersion.Ver2006)
                        version = "2006";
                    else if (model.MadVersion == MaddenFileVersion.Ver2007)
                        version = "2007";
                    else if (model.MadVersion == MaddenFileVersion.Ver2008)
                        version = "2008";
                    else if (model.MadVersion == MaddenFileVersion.Ver2019)
                        version = "2019";
                    hbuilder.Append(version);
                    hbuilder.Append(",");
                    hbuilder.Append("No");
                    hbuilder.Append(",");
                    writer.WriteLine(hbuilder.ToString());
                    //writer.Flush();
                    hbuilder.Clear();

                    if (!model.TableModels.ContainsKey(t))
                    {
                        model.ProcessTable(model.TableNames[tablename]);
                    }

                    TableModel table = model.TableModels[t];

                    List<TdbFieldProperties> props = table.GetFieldList();      //team db format maybe?
                    foreach (string field in fields_export[t])
                    {
                        foreach (TdbFieldProperties tdb in props)
                        {
                            if (field == tdb.Name)
                            {
                                // These are already fixed for big endian
                                string fieldname = tdb.Name;
                                hbuilder.Append(fieldname);
                                hbuilder.Append(",");
                            }
                        }
                    }
                    writer.WriteLine(hbuilder.ToString());

                    if (Descriptions_Checkbox.Checked)
                    {
                        if (model.TableDefs.ContainsKey(model.MadVersion))
                        {
                            Dictionary<string, tabledefs> currenttable = model.TableDefs[model.MadVersion];
                            if (currenttable.ContainsKey(table.Name))
                            {
                                tabledefs defs = currenttable[table.Name];
                                StringBuilder descbuilder = new StringBuilder();

                                for (int c = 0; c < fields_export[t].Count; c++)
                                {
                                    descbuilder.Append(defs.FieldDefs[fields_export[t][c]]);
                                    descbuilder.Append(",");
                                }

                                writer.WriteLine(descbuilder.ToString());
                            }
                        }
                    }

                    // Setting Filters
                    int teamID = -1;
                    int positionID = -1;
                    int archetypeID = -1;   //11-11-23
                    int collegeID = -1; //11-11-23
                    int DevTraitID = -1;    //11-11-23
                    int YearsProID = -1;    //11-11-23
                    int ProBowlID = -1; //11-11-23


                    if (filterTeamCheckbox.Checked)
                    {
                        //Get the team id for the team selected in the combobox
                        teamID = ((TeamRecord)(filterTeamCombo.SelectedItem)).TeamId;
                    }

                    if (filterPositionCheckbox.Checked)
                    {
                        //Get the position id for the position selected in the combobox
                        positionID = filterPositionCombo.SelectedIndex;
                    }

                    if (ArchetypeCheckBox.Checked)  //11-11-23
                    {
                        //Get the archetype id for the position selected in the combobox 11-11-23
                        archetypeID = filterArchetypeComboBox.SelectedIndex;
                    }

                    if (CollegeCheckBox.Checked)    //11-11-23
                    {
                        //Get the archetype id for the position selected in the combobox 11-11-23
                        collegeID = FilterCollegeComboBox.SelectedIndex;
                    }

                    if (DevTraitCheckBox.Checked)    //11-11-23
                    {
                        //Get the archetype id for the position selected in the combobox 11-11-23
                        DevTraitID = DevTraitComboBox.SelectedIndex;
                    }

                    if (YearsProCheckBox.Checked)    //11-11-23
                    {
                        //Get the archetype id for the position selected in the combobox 11-11-23
                        YearsProID = FilterYearsProcomboBox.SelectedIndex;
                    }

                    if (ProBowlCheckBox.Checked)    //11-11-23
                    {
                        //Get the archetype id for the position selected in the combobox 11-11-23
                        ProBowlID = ProBowlComboBox.SelectedIndex;
                    }

                    foreach (TableRecordModel rec in table.GetRecords()) // 11-11-23
                    {
                        if (rec == null || rec.Deleted)
                            continue;
                        else if (table.Name == "PLAY")
                        {
                            // Use logical OR to check any of the conditions
                            if ((teamID != -1 && teamID != rec.GetIntField("TGID")) ||
                                (positionID != -1 && positionID != rec.GetIntField("PPOS")) ||
                                (collegeID != -1 && collegeID != rec.GetIntField("PCOL")) ||
                                (DevTraitID != -1 && DevTraitID != rec.GetIntField("PROL")) ||
                                (YearsProID != -1 && YearsProID != rec.GetIntField("PYRP")) ||
                                (ProBowlID != -1 && ProBowlID != rec.GetIntField("PFPB")) ||
                                (archetypeID != -1 && archetypeID != rec.GetIntField("PLTY")))
                            {
                                continue;
                            }

                            // Rest of your code for processing the records
                        }
                    


                    StringBuilder builder = new StringBuilder();

                        foreach (string field in fields_export[t])
                        {
                            foreach (TdbFieldProperties tdb in props)
                            {
                                if (field == tdb.Name)
                                {
                                    if (tdb.FieldType == TdbFieldType.tdbString)
                                    {
                                        string res = rec.GetStringField(tdb.Name);
                                        res = res.Replace(",", " ");
                                        builder.Append(res);
                                    }
                                    else if (tdb.FieldType == TdbFieldType.tdbVarChar)
                                    {
                                        string res = "N/A";
                                        builder.Append(res);
                                    }
                                    else if (tdb.FieldType == TdbFieldType.tdbFloat)
                                    {

                                        builder.Append(rec.GetFloatField(tdb.Name).ToString("G", culture));
                                    }
                                    else
                                    {
                                        int test = rec.GetIntField(tdb.Name);
                                        builder.Append(test);
                                    }
                                    builder.Append(",");
                                }
                            }
                        }

                        writer.WriteLine(builder.ToString());

                    }
                    writer.Flush();
                    writer.Close();
                }

                catch (IOException err)
                {
                    err = err;
                    MessageBox.Show("Error opening file\r\n\r\n Check that the file is not already opened", "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            int teamID = -1;
            int positionID = -1;
            int archetypeID = -1;   //11-11-23
            int collegeID = -1;     //11-11-23
            int DevTraitID = -1;    //11-11-23
            int YearsProID = -1;    //11-11-23
            int ProBowlID = -1;     //11-11-23

            if (filterTeamCheckbox.Checked)
            {
                //Get the team id for the team selected in the combobox
                teamID = ((TeamRecord)(filterTeamCombo.SelectedItem)).TeamId;
            }

            if (filterPositionCheckbox.Checked)
            {
                //Get the position id for the position selected in the combobox
                positionID = filterPositionCombo.SelectedIndex;
            }

            if (ArchetypeCheckBox.Checked)    //11-11-23
            {
                //Get the position id for the position selected in the combobox
                archetypeID = filterArchetypeComboBox.SelectedIndex;
            }

            if (CollegeCheckBox.Checked)    //11-11-23
            {
                //Get the position id for the position selected in the combobox
                collegeID = FilterCollegeComboBox.SelectedIndex;
            }

            if (DevTraitCheckBox.Checked)    //11-11-23
            {
                //Get the position id for the position selected in the combobox
                DevTraitID = DevTraitComboBox.SelectedIndex;
            }

            if (YearsProCheckBox.Checked)    //11-11-23
            {
                //Get the position id for the position selected in the combobox
                YearsProID = FilterYearsProcomboBox.SelectedIndex;
            }

            if (ProBowlCheckBox.Checked)    //11-11-23
            {
                //Get the position id for the position selected in the combobox
                ProBowlID = ProBowlComboBox.SelectedIndex;
            }

            List<PlayerRecord> playerList = new List<PlayerRecord>();

            foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
            {
                if (record.Deleted)
                {
                    continue;
                }

                PlayerRecord playerRecord = (PlayerRecord)record;

                if (teamID != -1 && playerRecord.TeamId != teamID)
                {
                    continue;
                }

                if (positionID != -1 && playerRecord.PositionId != positionID)
                {
                    continue;
                }

                if (archetypeID != -1 && playerRecord.PlayerType != archetypeID)    //11-11-23
                {
                    continue;
                }

                if (collegeID != -1 && playerRecord.CollegeId != collegeID)    //11-11-23
                {
                    continue;
                }

                if (DevTraitID != -1 && playerRecord.DevTrait != DevTraitID)    //11-11-23
                {
                    continue;
                }

                if (YearsProID != -1 && playerRecord.YearsPro != YearsProID)    //11-11-23
                {
                    continue;
                }

                if (ProBowlID != -1 && playerRecord.ProBowlFilter != ProBowlID)    //11-11-23
                {
                    continue;
                }

                if (filterDraftClassCheckbox.Checked && playerRecord.YearsPro != 0)
                {
                    continue;
                }
                if (playerRecord.FirstName == "New" && playerRecord.LastName == "Player")
                    continue;

                //This player needs to be added to our list for export
                playerList.Add(playerRecord);
            }

            //Bring up a save dialog
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
                        StreamWriter wText = new StreamWriter(myStream);

                        //Output the headers first
                        StringBuilder hbuilder = new StringBuilder();
                        hbuilder.Append("Position,");
                        hbuilder.Append("First Name,");
                        hbuilder.Append("Last Name,");
                        // If Draft Class use College instead
                        if (filterDraftClassCheckbox.Checked)
                            hbuilder.Append("College,");
                        else
                            hbuilder.Append("Team,");


                        hbuilder.Append("Age,");
                        hbuilder.Append("Height,");
                        hbuilder.Append("Weight,");
                        hbuilder.Append("Tendency,");
                        hbuilder.Append("OVR,");

                        if (MainSkillsOnly_Checkbox.Checked)
                        {
                            switch (positionID)
                            {
                                case (int)MaddenPositions.QB:
                                    hbuilder.Append("THA,");
                                    hbuilder.Append("THP,");
                                    hbuilder.Append("AWR,");
                                    hbuilder.Append("SPD,");
                                    hbuilder.Append("AGI,");
                                    hbuilder.Append("BTK,");
                                    break;
                            }
                        }

                        else
                        {
                            hbuilder.Append("Speed,");
                            hbuilder.Append("Strength,");
                            hbuilder.Append("Awareness,");
                            hbuilder.Append("Agility,");
                            hbuilder.Append("Acceleration,");
                            hbuilder.Append("Catching,");
                            hbuilder.Append("Carrying,");
                            hbuilder.Append("Jumping,");
                            hbuilder.Append("Break Tackle,");
                            hbuilder.Append("Tackle,");
                            hbuilder.Append("Throw Power,");
                            hbuilder.Append("Throw Accuracy,");
                            hbuilder.Append("Pass Blocking,");
                            hbuilder.Append("Run Blocking,");
                            hbuilder.Append("Kick Power,");
                            hbuilder.Append("Kick Accuracy,");
                            hbuilder.Append("Kick Return,");
                            hbuilder.Append("Stamina,");
                            hbuilder.Append("Injury,");
                            hbuilder.Append("Toughness,");
                            hbuilder.Append("Importance,");
                            hbuilder.Append("Morale");
                        }

                        wText.WriteLine(hbuilder.ToString());

                        foreach (PlayerRecord rec in playerList)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append(Enum.GetNames(typeof(MaddenPositions))[rec.PositionId].ToString());
                            builder.Append(",");
                            builder.Append(rec.FirstName);
                            builder.Append(",");
                            builder.Append(rec.LastName);
                            builder.Append(",");
                            // If Draft Class use College Name instead
                            if (rec.YearsPro == 0 && filterDraftClassCheckbox.Checked)
                                builder.Append(collegenames[rec.CollegeId]);
                            else
                                builder.Append(model.TeamModel.GetTeamNameFromTeamId(rec.TeamId));
                            builder.Append(",");
                            builder.Append(rec.Age);
                            builder.Append(",");
                            builder.Append((rec.Height / 12) + "' " + (rec.Height % 12) + "\"");
                            builder.Append(",");
                            builder.Append(rec.Weight + 160);
                            builder.Append(",");

                            if (rec.Tendency == 2)
                                builder.Append("BAL");
                            else
                            {
                                switch (positionID)
                                {
                                    case (int)MaddenPositions.QB:
                                        {
                                            if (rec.Tendency == 0)
                                                builder.Append("POC");
                                            else builder.Append("SCR");
                                            break;
                                        }
                                    case (int)MaddenPositions.HB:
                                        {
                                            if (rec.Tendency == 0)
                                                builder.Append("POW");
                                            else builder.Append("SPD");
                                            break;
                                        }
                                    case (int)MaddenPositions.FB:
                                    case (int)MaddenPositions.TE:
                                        {
                                            if (rec.Tendency == 0)
                                                builder.Append("BLK");
                                            else builder.Append("REC");
                                            break;
                                        }
                                    case (int)MaddenPositions.WR:
                                        {
                                            if (rec.Tendency == 0)
                                                builder.Append("POS");
                                            else builder.Append("SPD");
                                            break;
                                        }
                                    case (int)MaddenPositions.LT:
                                    case (int)MaddenPositions.LG:
                                    case (int)MaddenPositions.C:
                                    case (int)MaddenPositions.RG:
                                    case (int)MaddenPositions.RT:
                                        {
                                            if (rec.Tendency == 0)
                                                builder.Append("RUN");
                                            else builder.Append("PAS");
                                            break;
                                        }
                                }
                            }

                            builder.Append(",");

                            builder.Append(rec.Overall);
                            builder.Append(",");

                            if (MainSkillsOnly_Checkbox.Checked)
                            {
                                switch (positionID)
                                {
                                    case (int)MaddenPositions.QB:
                                        builder.Append(rec.ThrowAccuracy);
                                        builder.Append(",");
                                        builder.Append(rec.ThrowPower);
                                        builder.Append(",");
                                        builder.Append(rec.Awareness);
                                        builder.Append(",");
                                        builder.Append(rec.Speed);
                                        builder.Append(",");
                                        builder.Append(rec.Agility);
                                        builder.Append(",");
                                        builder.Append(rec.BreakTackle);
                                        builder.Append(",");
                                        break;
                                }
                            }

                            else
                            {
                                builder.Append(rec.Speed);
                                builder.Append(",");
                                builder.Append(rec.Strength);
                                builder.Append(",");
                                builder.Append(rec.Awareness);
                                builder.Append(",");
                                builder.Append(rec.Agility);
                                builder.Append(",");
                                builder.Append(rec.Acceleration);
                                builder.Append(",");
                                builder.Append(rec.Catching);
                                builder.Append(",");
                                builder.Append(rec.Carrying);
                                builder.Append(",");
                                builder.Append(rec.Jumping);
                                builder.Append(",");
                                builder.Append(rec.BreakTackle);
                                builder.Append(",");
                                builder.Append(rec.Tackle);
                                builder.Append(",");
                                builder.Append(rec.ThrowPower);
                                builder.Append(",");
                                builder.Append(rec.ThrowAccuracy);
                                builder.Append(",");
                                builder.Append(rec.PassBlocking);
                                builder.Append(",");
                                builder.Append(rec.RunBlocking);
                                builder.Append(",");
                                builder.Append(rec.KickPower);
                                builder.Append(",");
                                builder.Append(rec.KickAccuracy);
                                builder.Append(",");
                                builder.Append(rec.KickReturn);
                                builder.Append(",");
                                builder.Append(rec.Stamina);
                                builder.Append(",");
                                builder.Append(rec.Injury);
                                builder.Append(",");
                                builder.Append(rec.Toughness);
                                builder.Append(",");
                                builder.Append(rec.Importance);
                                builder.Append(",");
                                builder.Append(rec.Morale);
                            }

                            wText.WriteLine(builder.ToString());
                            wText.Flush();
                        }

                        myStream.Close();
                    }

                }
                catch (IOException err)
                {
                    err = err;
                    MessageBox.Show("Error opening file\r\n\r\n Check that the file is not already opened", "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Cursor = Cursors.Default;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddExportTables_Button_Click(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;

                foreach (ListViewItem i in AvailTables_ListView.SelectedItems)
                {
                    string name = i.Text;

                    if (!tables_export.Contains(name))
                        tables_export.Add(name);
                }
                InitExportTables();

                isInitializing = false;
            }
        }

        private void RemoveExportTables_Button_Click(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;

                foreach (ListViewItem i in ExportTables_ListView.SelectedItems)
                {
                    string name = i.Text;
                    if (tables_export.Contains(name))
                        tables_export.Remove(name);
                }

                InitExportTables();

                isInitializing = false;
            }
        }

        private void filterTeamCombo_SelectedIndexChanged(object sender, EventArgs e)   
        {

        }

        private void filterPositionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void filterArchetypeComboBox_SelectedIndexChanged (object sender, EventArgs e) //add here 11-11-23
        {

        }

        private void FilterCollegeComboBox_SelectedIndexChanged(object sender, EventArgs e) //add here 11-11-23
        {

        }

        private void DevtraitComboBox_SelectedIndexChanged(object sender, EventArgs e) //add here 11-11-23
        {

        }

        private void FilterYearsPro_SelectedIndexChanged(object sender, EventArgs e) //add here 11-11-23
        {

        }

        private void ProBowlComboBox_SelectedIndexChanged(object sender, EventArgs e) //add here 11-11-23
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ExportTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ExportTables_ListView.SelectedItems.Count == 0)
                return;
            if (!isInitializing)
            {
                isInitializing = true;
                string tablename = ExportTables_ListView.SelectedItems[0].Text;
                InitAvailableFields(tablename);
                InitExportFields(tablename);
                isInitializing = false;
            }
        }

        private void AddFields_Button_Click(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;
                string tablename = ExportTables_ListView.SelectedItems[0].Text;
                List<string> newfields = new List<string>();
                foreach (ListViewItem i in AvailFields_ListView.SelectedItems)
                    newfields.Add(i.Text);
                if (fields_export.ContainsKey(tablename))
                {
                    List<string> existing = new List<string>(fields_export[tablename]);
                    foreach (string s in newfields)
                        if (!existing.Contains(s))
                            existing.Add(s);
                    fields_export[tablename] = existing;
                }
                else fields_export.Add(tablename, newfields);

                InitExportFields(tablename);
                isInitializing = false;
            }
        }

        private void RemoveFields_Button_Click(object sender, EventArgs e)
        {
            isInitializing = true;
            string tablename = ExportTables_ListView.SelectedItems[0].Text;
            List<string> exportlist = new List<string>();
            List<string> avail = fields_avail[tablename];

            foreach (ListViewItem x in ExportFields_ListView.SelectedItems)
                exportlist.Add(x.Text);
            foreach (string s in exportlist)
                fields_export[tablename].Remove(s);

            InitExportFields(tablename);

            isInitializing = false;
        }

        private void ImportCSV_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            Stream myStream = null;
            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = fileDialog.OpenFile()) != null)
                    {
                        StreamReader sr = new StreamReader(myStream);

                        #region Table/Version info
                        string csvtableinfo = sr.ReadLine();
                        string[] csvtable = csvtableinfo.Split(',');
                        string tablename = csvtable[0];
                        currenttablename = csvtable[0];

                        int ver = Convert.ToInt32(csvtable[1]);
                        bool hasdesc = false;
                        if (csvtable[2].ToUpper().Contains("Y"))
                            hasdesc = true;
                        if (ver == 2004)
                            csvVersion = MaddenFileVersion.Ver2004;
                        else if (ver == 2005)
                            csvVersion = MaddenFileVersion.Ver2005;
                        else if (ver == 2006)
                            csvVersion = MaddenFileVersion.Ver2006;
                        else if (ver == 2007)
                            csvVersion = MaddenFileVersion.Ver2007;
                        else if (ver == 2019)
                            csvVersion = MaddenFileVersion.Ver2019;
                        else csvVersion = MaddenFileVersion.Ver2008;
                        #endregion

                        #region Fields
                        import_fields_avail.Clear();
                        string fieldline = sr.ReadLine();
                        string[] csvfield = fieldline.Split(',');
                        for (int c = 0; c < csvfield.Length; c++)
                        {
                            if (csvfield[c] != "")
                                import_fields_avail.Add(c, csvfield[c]);
                        }
                        #endregion

                        #region Descriptions
                        // We dont need the descripitons for importing, it's just a reference for the roster maker
                        // so just read the line and dont do anything with it.
                        linenumber = 2;
                        if (hasdesc)
                        {
                            sr.ReadLine();
                            linenumber = 3;
                        }
                        #endregion

                        #region Records
                        CSVRecords.Clear();

                        while (!sr.EndOfStream)
                        {
                            List<string> rec_line = new List<string>();
                            string csvrecline = sr.ReadLine();
                            if (csvrecline == "")
                                continue;
                            else
                            {
                                string[] csvrec = csvrecline.Split(',');
                                foreach (string s in csvrec)
                                    rec_line.Add(s);
                                CSVRecords.Add(rec_line);
                            }
                        }

                        sr.Close();
                        //Done                        
                        #endregion

                        ImportTableName_Textbox.Text = tablename;
                        ColumnHeader header = new ColumnHeader();
                        header.Text = "Tables";
                        header.Name = "Tables";
                        WrongFields_ListView.Items.Clear();
                        WrongFields_ListView.Columns.Add(header);
                        WrongFields_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        WrongFields_ListView.Sorting = SortOrder.Ascending;

                        ColumnHeader header2 = new ColumnHeader();
                        header2.Text = "Tables";
                        header2.Name = "Tables";
                        ImportAvailFields_ListView.Items.Clear();
                        ImportAvailFields_ListView.Columns.Add(header2);
                        ImportAvailFields_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        ImportAvailFields_ListView.Sorting = SortOrder.Ascending;

                        ColumnHeader header3 = new ColumnHeader();
                        header3.Text = "Tables";
                        header3.Name = "Tables";
                        ImportSelected_ListView.Items.Clear();
                        ImportSelected_ListView.Columns.Add(header3);
                        ImportSelected_ListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        ImportSelected_ListView.Sorting = SortOrder.Ascending;

                        List<string> possible = new List<string>();
                        List<TdbFieldProperties> fplist = model.TableModels[tablename].GetFieldList();
                        foreach (TdbFieldProperties fp in fplist)
                            possible.Add(fp.Name);

                        for (int c = import_fields_avail.Count - 1; c >= 0; c--)
                        {
                            string fieldname = import_fields_avail[c];
                            if (!possible.Contains(fieldname))
                            {
                                WrongFields_ListView.Items.Add(fieldname);
                                import_fields_avail.Remove(c);
                            }
                            else ImportAvailFields_ListView.Items.Add(import_fields_avail[c]);
                        }

                        import_fields.Clear();
                        foreach (KeyValuePair<int, string> kvp in import_fields_avail)
                            import_fields.Add(kvp.Value);
                        //import_fields.Sort();

                        foreach (string fld in import_fields)
                            ImportSelected_ListView.Items.Add(fld);

                        ImportFieldsCount_Textbox.Text = import_fields_avail.Count.ToString();
                        NotImportableCount_Textbox.Text = WrongFields_ListView.Items.Count.ToString();
                    }
                }
                catch (IOException err)
                {
                    err = err;
                    MessageBox.Show("Error opening file\r\n\r\n Check that the file is not already opened", "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (myStream != null)
                myStream.Close();
        }

        private void ProcessRecords_Button_Click(object sender, EventArgs e)
        {
            currentrec = -1;

            foreach (List<string> record in CSVRecords)
            {
                TableRecordModel tablerecord = null;
                linenumber++;
                currentrec++;
                bool fail = false;


                if (currenttablename == "PLAY" && UpdateRecs_Checkbox.Checked)
                {
                    // Updating existing record if possible
                    int pgidkey = -1;
                    foreach (KeyValuePair<int, string> kvp in import_fields_avail)
                    {
                        if (kvp.Value == "PGID")
                        {
                            pgidkey = Convert.ToInt32(record[kvp.Key]);
                            break;
                        }
                    }
                    if (pgidkey != -1)
                    {
                        foreach (TableRecordModel trm in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                        {
                            if (trm.Deleted)
                                continue;
                            PlayerRecord player = (PlayerRecord)trm;
                            if (player.PlayerId == pgidkey)
                            {
                                tablerecord = trm;
                                break;
                            }
                        }
                    }
                }

                // Not updating PLAY table, so start replacing                    
                if (tablerecord == null)
                {
                    if (currentrec > model.TableModels[currenttablename].capacity - 1)
                        fail = true;
                    else if (currentrec > model.TableModels[currenttablename].RecordCount - 1)
                        tablerecord = model.TableModels[currenttablename].CreateNewRecord(true);
                    else tablerecord = model.TableModels[currenttablename].GetRecord(currentrec);
                }

                List<TdbFieldProperties> fplist = model.TableModels[currenttablename].GetFieldList();

                if (fail)
                    errors.Add("Line number " + linenumber.ToString() + " Exceeded Capacity");

                foreach (KeyValuePair<int, string> import in import_fields_avail)
                {
                    foreach (TdbFieldProperties fp in fplist)
                    {
                        if (fp.Name == import.Value)
                        {
                            try
                            {
                                tablerecord.SetFieldCSV(fp.Name, record[import.Key]);
                            }
                            catch
                            {
                                errors.Add("Line number " + linenumber.ToString() + " Field " + fp.Name + " " + import.Value);
                            }
                        }
                    }
                }
            }

            // delete unwanted
            if (DeleteCurrentRecs_Checkbox.Checked)
            {
                for (int c = currentrec; c < model.TableModels[currenttablename].RecordCount - 1; c++)
                {
                    TableRecordModel rec = model.TableModels[currenttablename].GetRecord(currentrec);
                    if (rec.Deleted)
                        continue;
                    rec.SetDeleteFlag(true);
                }
            }

            ColumnHeader head = new ColumnHeader();
            head.Text = "Errors";
            head.Name = "Errors";
            ImportErrors_Listview.Columns.Add(head);
            ImportErrors_Listview.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ImportErrors_Listview.Items.Clear();
            if (errors.Count > 0)
            {
                foreach (string error in errors)
                    ImportErrors_Listview.Items.Add(new ListViewItem(error));
            }
            else ImportErrors_Listview.Items.Add(new ListViewItem(linenumber.ToString() + " Lines processed.  No errors"));
        }

        private void DeleteCurrentRecs_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                if (DeleteCurrentRecs_Checkbox.Checked)
                {
                    UpdateRecs_Checkbox.Checked = false;
                    UpdateRecs_Checkbox.Enabled = false;
                }
                else
                {
                    UpdateRecs_Checkbox.Enabled = true;
                }
            }

            InitProcessButton();
        }

        private void UpdateRecs_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                if (UpdateRecs_Checkbox.Checked)
                {
                    DeleteCurrentRecs_Checkbox.Checked = false;
                    DeleteCurrentRecs_Checkbox.Enabled = false;
                }
                else
                {
                    DeleteCurrentRecs_Checkbox.Enabled = true;
                }
            }

            InitProcessButton();
        }

        private void LoadDraftClass_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Filter = "Madden Draft Class (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;
            dialog.ShowDialog();

            string filename = dialog.FileName;
            if (filename == "")
                return;

            try
            {
                if (!model.DraftClassModel.ReadDraftClass(filename))
                {
                    MessageBox.Show("Not a valid Madden Draft Class", "Not a Draft Class", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {

            }

        }

        private void ExportDraftClass_Button_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            Stream myStream = null;

            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    if ((myStream = fileDialog.OpenFile()) != null)
                    {
                        StreamWriter wText = new StreamWriter(myStream);

                        model.DraftClassModel.ExportCSVHeaders(wText, DraftClassDescriptions_Checkbox.Checked);

                        foreach (DraftPlayer player in model.DraftClassModel.draftclassplayers)
                        {
                            StringBuilder build = player.ExportDraftClassPlayerCSV(model.DraftClassModel.RatingDefs, model, DraftClassDescriptions_Checkbox.Checked);
                            wText.WriteLine(build.ToString());
                        }

                        wText.Close();
                    }
                }
                catch (IOException err)
                {
                    MessageBox.Show("Error opening file\r\n\r\n Check that the file is not already opened", "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = Cursors.Default;
            }
        }

        private void jsontocsv_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Select a JSON file"
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Save the CSV file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string jsonFilePath = openFileDialog.FileName;
                string csvFilePath = saveFileDialog.FileName;

                string jsonText = File.ReadAllText(jsonFilePath);
                JObject jsonData = JObject.Parse(jsonText);
                JObject playerMap = (JObject)jsonData["characterVisualsPlayerMap"];

                List<string> headers = new List<string> { "Player_ID" };
                List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

                foreach (var player in playerMap)
                {
                    string playerId = player.Key;
                    JObject playerData = (JObject)player.Value;

                    Dictionary<string, string> row = new Dictionary<string, string> { { "Player_ID", playerId } };

                    ProcessObject(playerData, row, headers, "");

                    rows.Add(row);
                }

                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine(string.Join(",", headers));
                    foreach (var row in rows)
                    {
                        writer.WriteLine(string.Join(",", headers.ConvertAll(header => row.ContainsKey(header) ? row[header] : "")));
                    }
                }

                MessageBox.Show("CSV export completed successfully.");
            }
            else
            {
                MessageBox.Show("Operation cancelled.");
            }
        }

        private static void ProcessObject(JObject obj, Dictionary<string, string> row, List<string> headers, string prefix)
        {
            foreach (var prop in obj.Properties())
            {
                string key = prop.Name;
                string fullKey = prefix + key;

                if (prop.Value.Type == JTokenType.Object)
                {
                    ProcessObject((JObject)prop.Value, row, headers, fullKey + "_");
                }
                else if (prop.Value.Type == JTokenType.Array)
                {
                    JArray array = (JArray)prop.Value;
                    for (int i = 0; i < array.Count; i++)
                    {
                        ProcessObject((JObject)array[i], row, headers, fullKey + "_" + i + "_");
                    }
                }
                else
                {
                    if (!headers.Contains(fullKey))
                    {
                        headers.Add(fullKey);
                    }

                    row[fullKey] = prop.Value.ToString();
                }
            }
        }

        private void csvtojson_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Select a CSV file"
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Save the JSON file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string csvFilePath = openFileDialog.FileName;
                string jsonFilePath = saveFileDialog.FileName;

                JObject jsonData = new JObject();
                JObject playerMap = new JObject();
                jsonData["characterVisualsPlayerMap"] = playerMap;

                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(new System.Globalization.CultureInfo("en-US")) { Delimiter = "," }))
                {
                    var records = csv.GetRecords<dynamic>();
                    foreach (var record in records)
                    {
                        var row = record as IDictionary<string, object>;
                        JObject playerData = new JObject();
                        string playerId = row["Player_ID"].ToString();
                        playerMap[playerId] = playerData;

                        foreach (var cell in row)
                        {
                            if (cell.Key != "Player_ID")
                            {
                                string[] path = cell.Key.Split('_');
                                SetValueAtPath(playerData, path, 0, cell.Value.ToString());
                            }
                        }
                    }
                }

                File.WriteAllText(jsonFilePath, jsonData.ToString(Newtonsoft.Json.Formatting.None));

                MessageBox.Show("JSON export completed successfully.");
            }
            else
            {
                MessageBox.Show("Operation cancelled.");
            }
        }

        private static void SetValueAtPath(JObject obj, string[] path, int index, string value)
        {
            if (string.IsNullOrEmpty(value)) return; // Skip empty values

            string key = path[index];

            if (index == path.Length - 1)
            {
                obj[key] = value;
            }
            else
            {
                if (int.TryParse(path[index + 1], out int arrayIndex))
                {
                    if (!obj.ContainsKey(key))
                    {
                        obj[key] = new JArray();
                    }

                    JArray array = (JArray)obj[key];
                    while (array.Count <= arrayIndex)
                    {
                        array.Add(new JObject());
                    }

                    SetValueAtPath((JObject)array[arrayIndex], path, index + 2, value);
                }
                else
                {
                    if (!obj.ContainsKey(key))
                    {
                        obj[key] = new JObject();
                    }

                    SetValueAtPath((JObject)obj[key], path, index + 1, value);
                }
            }
        }

        private void MoveTempSavesbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(sourceFolder))
                {
                    MessageBox.Show("Saves folder found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Check if temprosterstorage folder exists
                    if (Directory.Exists(tempRosterStorageFolder))
                    {
                        // Move contents of temprosterstorage to saves folder
                        foreach (string file in Directory.GetFiles(tempRosterStorageFolder))
                        {
                            File.Move(file, Path.Combine(savesFolder, Path.GetFileName(file)));
                        }

                        // Delete temprosterstorage folder
                        Directory.Delete(tempRosterStorageFolder, true);
                        MessageBox.Show("Rosters restored to Madden 24 saves folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Create temprosterstorage folder if it doesn't exist
                        if (!Directory.Exists(tempRosterStorageFolder))
                        {
                            Directory.CreateDirectory(tempRosterStorageFolder);
                        }

                        // Move files starting with "ROSTER-" to temprosterstorage folder
                        foreach (string file in Directory.GetFiles(savesFolder, "ROSTER-*"))
                        {
                            File.Move(file, Path.Combine(tempRosterStorageFolder, Path.GetFileName(file)));
                        }

                        MessageBox.Show("Rosters moved to temporary folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Saves folder not found. Program exiting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReturnTempSavesbutton_Click(object sender, EventArgs e)        //new 12-23-23
        {
            try
            {
                if (Directory.Exists(sourceFolder))
                {
                    MessageBox.Show("Saves folder found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Check if temprosterstorage folder exists
                    if (Directory.Exists(tempRosterStorageFolder))
                    {
                        // Move contents of temprosterstorage to saves folder
                        foreach (string file in Directory.GetFiles(tempRosterStorageFolder))
                        {
                            File.Move(file, Path.Combine(savesFolder, Path.GetFileName(file)));
                        }

                        // Delete temprosterstorage folder
                        Directory.Delete(tempRosterStorageFolder, true);
                        MessageBox.Show("Rosters restored to Madden 24 saves folder.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Temporary folder not found. No operation performed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    MessageBox.Show("Operation completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Saves folder not found. Program exiting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void M24PGIDConverterbutton_Click(object sender, EventArgs e)       //New 1-13-24
        {
            try
            {
                if (model.MadVersion != MaddenFileVersion.Ver2019)
                {
                    List<int> numberList = new List<int>
            {
                112, 116, 251, 498, 505, 506, 521, 727, 857, 890, 915, 927, 963, 964,
                1073, 1088, 1114, 1159, 1285, 1331, 1375, 1426, 1435, 1599, 1713, 1732,
                1751, 1796, 2002, 2003, 2016, 2017, 2020, 2029, 2039, 2040, 2055, 2060,
                2071, 2076, 2082, 2084, 2091, 2097, 2098, 2099, 2103, 2109, 2122, 2124,
                2136, 2148, 2169, 2200, 2202, 2206, 2254, 2261, 2262, 2276, 2278, 2279,
                2280, 2293, 2301, 2308, 2312, 2313, 2318, 2335, 2346, 2365, 2366, 2377, 
                2386, 2392, 2398, 2401, 2402, 2403, 2405, 2406, 2420, 2431, 2435, 2436, 
                2437, 2440, 2442, 2443, 2448, 2449, 2450, 2454, 2456, 2457, 2477, 2480, 2481, 2485, 2487, 2488, 2491, 2496, 2497, 2498, 2499, 2500, 2503, 2505, 2506, 2507, 2511, 2512, 2513, 2520, 2522, 2523, 2527, 2528, 2529, 2532, 2533, 2534, 2537, 2538, 2539, 2540, 2543, 2544, 2545, 2546, 
                2547, 2548, 2549, 2552, 2554, 2559, 2560, 2562, 2563, 2564, 2567, 2575, 2577, 2579, 2580, 2585, 2587, 2593, 2594, 2599, 2604, 2605, 2612, 2614, 2616, 2620, 2629, 2632, 2634, 2640, 2656, 2658, 2661, 2670, 2671, 2680, 2688, 2691, 2694, 2700, 2703, 2714, 2725, 2753, 2757, 2759, 
                2764, 2769, 2772, 2780, 2796, 2798, 2803, 2807, 2808, 2813, 2816, 2817, 2832, 2848, 2882, 2888, 2893, 2898, 2904, 9713, 9766, 9800, 9804, 9833, 9835, 9837, 9839, 9840, 9857, 9858, 9869, 9870, 9927, 10435, 10439, 10452, 10539, 10551, 10577, 10583, 10597, 10613, 10615, 10619, 
                10642, 10653, 10665, 10688, 10690, 10694, 10697, 10698, 10703, 10734, 10737, 10743, 10815, 10823, 10826, 10828, 10829, 10831, 10837, 10843, 10845, 10847, 10852, 10857, 10863, 10866, 10875, 10876, 10939, 10983, 10984, 11008, 11079, 11110, 11132, 11211, 11214, 11215, 11217, 11222, 
                11244, 11269, 11276, 11284, 11291, 11295, 11297, 11298, 11306, 11436, 11437, 11440, 11453, 11484, 11489, 11508, 11518, 11541, 11550, 11555, 11567, 11577, 11594, 11598, 11601, 11616, 11630, 11673, 11695, 11775, 11783, 11791, 11802, 11807, 11809, 11813, 11814, 11818, 11827, 11836, 
                11842, 11882, 11892, 11903, 11905, 11930, 11932, 11938, 11941, 11943, 11945, 12091, 12280, 12315, 12322, 12330, 12336, 12337, 12339, 12344, 12348, 12349, 12350, 12351, 12354, 12361, 12362, 12374, 12384, 12387, 12388, 12389, 12390, 12402, 12403, 12405, 12410, 12416, 12418, 12420, 
                12423, 12426, 12428, 12433, 12435, 12436, 12437, 12448, 12451, 12453, 12455, 12470, 12472, 12489, 12490, 12493, 12498, 12499, 12503, 12504, 12512, 12520, 12521, 12523, 12524, 12525, 12526, 12527, 12528, 12529, 12530, 12531, 12532, 12533, 12534, 12535, 12536, 12537, 12538, 12539, 
                12540, 12541, 12542, 12544, 12545, 12547, 12548, 12549, 12550, 12551, 12552, 12554, 12555, 12556, 12557, 12558, 12560, 12561, 12562, 12563, 12564, 12569, 12570, 12571, 12572, 12573, 12574, 12575, 12577, 12578, 12579, 12580, 12581, 12582, 12583, 12585, 12586, 12587, 12589, 12591, 
                12593, 12594, 12595, 12596, 12598, 12599, 12600, 12601, 12602, 12604, 12605, 12607, 12609, 12611, 12612, 12613, 12614, 12616, 12617, 12618, 12619, 12622, 12623, 12625, 12626, 12627, 12632, 12633, 12634, 12635, 12636, 12639, 12641, 12644, 12645, 12646, 12647, 12650, 12651, 12652, 
                12653, 12660, 12662, 12663, 12664, 12665, 12673, 12681, 12683, 12687, 12690, 12692, 12693, 12696, 12698, 12700, 12701, 12702, 12703, 12705, 12708, 12709, 12711, 12712, 12713, 12714, 12718, 12719, 12720, 12721, 12724, 12735, 12739, 12743, 12748, 12750, 12753, 12761, 12763, 12768, 
                12775, 12777, 12778, 12779, 12780, 12781, 12784, 12785, 12786, 12795, 12798, 12799, 12803, 12806, 12807, 12809, 12810, 12814, 12816, 12818, 12821, 12823, 12824, 12828, 12829, 12830, 12832, 12833, 12834, 12835, 12836, 12837, 12838, 12841, 12846, 12847, 12851, 12852, 12857, 12859, 
                12861, 12863, 12872, 12873, 12874, 12885, 12895, 12897, 12902, 12903, 12905, 12908, 12922, 12930, 12931, 12932, 12933, 12935, 12936, 12938, 12939, 12942, 12945, 12948, 12950, 12951, 12953, 12959, 12960, 12961, 12964, 12965, 12966, 12967, 12968, 12969, 12973, 12975, 12979, 12981, 
                12983, 12985, 12988, 12993, 12995, 12997, 12998, 13003, 13004, 13007, 13013, 13021, 13023, 13025, 13026, 13028, 13029, 13034, 13037, 13038, 13039, 13056, 13059, 13065, 13079, 13080, 13081, 13082, 13083, 13084, 13085, 13086, 13087, 13088, 13090, 13091, 13092, 13093, 13094, 13095, 
                13096, 13097, 13098, 13099, 13100, 13101, 13102, 13103, 13104, 13105, 13106, 13108, 13109, 13110, 13111, 13112, 13113, 13114, 13115, 13116, 13117, 13118, 13119, 13120, 13121, 13123, 13124, 13125, 13127, 13128, 13129, 13130, 13132, 13133, 13135, 13136, 13137, 13139, 13140, 13143, 
                13145, 13147, 13148, 13149, 13152, 13153, 13154, 13156, 13157, 13158, 13159, 13160, 13162, 13163, 13165, 13166, 13167, 13168, 13169, 13170, 13171, 13172, 13173, 13177, 13178, 13179, 13180, 13181, 13182, 13184, 13185, 13186, 13187, 13188, 13189, 13190, 13191, 13192, 13193, 13194, 
                13195, 13196, 13197, 13198, 13199, 13200, 13201, 13202, 13203, 13207, 13208, 13210, 13211, 13212, 13214, 13215, 13216, 13217, 13218, 13222, 13223, 13224, 13226, 13227, 13232, 13234, 13237, 13239, 13241, 13244, 13245, 13248, 13249, 13250, 13253, 13254, 13255, 13256, 13257, 13258, 
                13259, 13260, 13261, 13262, 13263, 13266, 13267, 13268, 13269, 13271, 13272, 13273, 13274, 13275, 13277, 13278, 13279, 13280, 13282, 13284, 13286, 13287, 13291, 13294, 13297, 13299, 13300, 13307, 13308, 13312, 13314, 13316, 13317, 13318, 13322, 13323, 13327, 13328, 13332, 13333, 
                13334, 13336, 13337, 13341, 13342, 13344, 13347, 13348, 13352, 13356, 13361, 13362, 13363, 13364, 13365, 13366, 13374, 13375, 13376, 13377, 13380, 13382, 13383, 13387, 13390, 13391, 13392, 13396, 13399, 13402, 13405, 13406, 13408, 13409, 13410, 13411, 13412, 13414, 13424, 13425, 
                13426, 13428, 13434, 13435, 13436, 13437, 13439, 13440, 13441, 13442, 13446, 13448, 13454, 13457, 13458, 13459, 13461, 13463, 13465, 13466, 13467, 13468, 13469, 13470, 13471, 13472, 13473, 13478, 13479, 13480, 13481, 13482, 13483, 13484, 13486, 13487, 13490, 13498, 13502, 13504, 
                13507, 13508, 13512, 13522, 13527, 13529, 13532, 13543, 13548, 13550, 13552, 13554, 13563, 13564, 13568, 13571, 13585, 13588, 13590, 13601, 13610, 13617, 13623, 13628, 13643, 13645, 13657, 13664, 13670, 13677, 13679, 13683, 13684, 13690, 13691, 13694, 13701, 13703, 13707, 13710, 
                13711, 13712, 13713, 13723, 13729, 13731, 13740, 13743, 13759, 13765, 13777, 13781, 13782, 13790, 13793, 13804, 13805, 13808, 13814, 13818, 13819, 13820, 13822, 13824, 13830, 13834, 13835, 13837, 13839, 13842, 13845, 13846, 13847, 13848, 13850, 13851, 13853, 13855, 13857, 13858, 
                13859, 13860, 13861, 13864, 13870, 13872, 13875, 13878, 13879, 13881, 13882, 13893, 13899, 13901, 13903, 13910, 13911, 13913, 13917, 13922, 14732, 14858, 15130, 17450, 17537, 17538, 17539, 17540, 17541, 17542, 17543, 17544, 17545, 17547, 17548, 17550, 17551, 17552, 17553, 17554, 
                17555, 17556, 17557, 17562, 17565, 17566, 17567, 17568, 17570, 17573, 17580, 17582, 17585, 17586, 17592, 17593, 17595, 17596, 17600, 17604, 17606, 17616, 17620, 17628, 17637, 17643, 17644, 17645, 17651, 17653, 17654, 17655, 17656, 17657, 17658, 17659, 17661, 17662, 17663, 17665, 
                17667, 17668, 17670, 17672, 17673, 17674, 17675, 17676, 17677, 17678, 17679, 17681, 17682, 17683, 17684, 17686, 17687, 17688, 17689, 17690, 17692, 17694, 17700, 17704, 17705, 17706, 17709, 17711, 17713, 17714, 17716, 17720, 17724, 17728, 17731, 17733, 17734, 17738, 17739, 17740, 
                17741, 17742, 17746, 17747, 17751, 17752, 17754, 17755, 17756, 17758, 17759, 17765, 17774, 17776, 17777, 17784, 17789, 17790, 17791, 17794, 17795, 17800, 17803, 17817, 17818, 17825, 17827, 17828, 17829, 17834, 17837, 17838, 17845, 17847, 17850, 17852, 17854, 17855, 17858, 17859, 
                17860, 17868, 17871, 17872, 17893, 17897, 17900, 17912, 17923, 17950, 17960, 17967, 18001, 18009, 19982, 19983, 20051, 20162, 20218, 20305, 20328, 20329, 20330, 20331, 20334, 20335, 20337, 20338, 20340, 20341, 20344, 20347, 20348, 20350, 20351, 20352, 20353, 20355, 20356, 20357, 
                20358, 20359, 20360, 20361, 20362, 20364, 20365, 20366, 20367, 20368, 20369, 20371, 20372, 20373, 20374, 20376, 20377, 20380, 20381, 20382, 20384, 20385, 20388, 20390, 20392, 20393, 20394, 20395, 20397, 20398, 20399, 20400, 20402, 20403, 20405, 20406, 20408, 20409, 20410, 20411, 
                20413, 20415, 20417, 20418, 20419, 20421, 20422, 20423, 20424, 20425, 20427, 20428, 20430, 20433, 20434, 20435, 20436, 20437, 20439, 20440, 20441, 20444, 20445, 20448, 20449, 20451, 20452, 20453, 20454, 20456, 20457, 20458, 20459, 20460, 20461, 20462, 20463, 20466, 20469, 20470, 
                20471, 20473, 20475, 20476, 20479, 20480, 20481, 20484, 20486, 20487, 20488, 20489, 20490, 20491, 20492, 20493, 20494, 20495, 20496, 20497, 20498, 20499, 20501, 20503, 20506, 20507, 20508, 20509, 20510, 20511, 20512, 20515, 20518, 20519, 20520, 20522, 20523, 20526, 20529, 20530, 
                20531, 20532, 20533, 20534, 20535, 20537, 20538, 20539, 20541, 20542, 20545, 20547, 20548, 20549, 20550, 20551, 20552, 20553, 20554, 20555, 20557, 20560, 20561, 20563, 20564, 20565, 20566, 20567, 20569, 20571, 20572, 20574, 20575, 20578, 20579, 20581, 20582, 20583, 20584, 20588, 
                20590, 20591, 20592, 20596, 20597, 20599, 20600, 20601, 20602, 20603, 20604, 20605, 20607, 20608, 20611, 20612, 20613, 20616, 20617, 20620, 20621, 20622, 20623, 20627, 20629, 20631, 20640, 20641, 20642, 20643, 20649, 20652, 20653, 20655, 20658, 20659, 20660, 20661, 20662, 20663, 20664, 20665, 20666, 20669, 20670, 20672, 20676, 20677, 20678, 20681, 20682, 20683, 20684, 20685, 20687, 20688, 20689,
                20692, 20693, 20694, 20695, 20696, 20698, 20699, 20700, 20701, 20703, 20704, 20706, 20713, 20715, 20716, 20719, 20723, 20724, 20727, 20728, 20729, 20730, 20731, 20732, 20733, 20734, 20735, 20736, 20737, 20739, 20740, 20742, 20743, 20747, 20748, 20749, 20752, 20753, 20754, 20755, 20765, 20771, 20772, 20774, 20778, 20781, 20782, 20789, 20790, 20791, 20794, 20795, 20798, 20799, 20800, 20801, 20802, 
                20806, 20809, 20810, 20813, 20816, 20817, 20818, 20819, 20821, 20823, 20826, 20828, 20830, 20832, 20833, 20834, 20835, 20836, 20837, 20838, 20839, 20841, 20842, 20843, 20844, 20845, 20846, 20847, 20848, 20849, 20851, 20852, 20853, 20854, 20856, 20858, 20859, 20861, 20862, 20863, 20864, 20867, 20868, 20869, 20873, 20876, 20877, 20879, 20880, 20881, 20882, 20883, 20884, 20885, 20887, 20890, 20892, 
                20894, 20896, 20897, 20900, 20901, 20902, 20906, 20907, 20909, 20912, 20913, 20914, 20915, 20916, 20917, 20918, 20919, 20920, 20921, 20922, 20924, 20925, 20926, 20927, 20928, 20930, 20931, 20932, 20933, 20934, 20935, 20937, 20938, 20940, 20941, 20942, 20943, 20945, 20946, 20947, 20948, 20949, 20950, 20951, 20952, 20953, 20954, 20955, 20957, 20960, 20961, 20962, 20963, 20964, 20965, 20966, 20968, 
                20969, 20970, 20971, 20972, 20973, 20974, 20975, 20976, 20977, 20978, 20979, 20981, 20982, 20983, 20984, 20985, 20986, 20988, 20989, 20990, 20991, 20994, 20995, 20997, 20998, 20999, 21000, 21001, 21002, 21003, 21004, 21005, 21006, 21008, 21009, 21010, 21011, 21012, 21013, 21014, 21016, 21017, 21018, 21019, 21020, 21021, 21022, 21023, 21029, 21030, 21033, 21034, 21035, 21036, 21039, 21040, 21042, 21043, 21044, 21045, 21046, 21047, 21048, 
                21049, 21050, 21051, 21056, 21058, 21059, 21063, 21064, 21065, 21066, 21067, 21069, 21070, 21071, 21072, 21073, 21074, 21076, 21077, 21080, 21081, 21082, 21085, 21086, 21088, 21089, 21090, 21092, 21093, 21094, 21095, 21096, 21097, 21098, 21099, 21100, 21101, 21102, 21103, 21104, 21105, 21106, 21107, 21108, 21109, 21110, 21111, 21112, 21113, 21114, 21115, 21116, 21118, 21119, 21120, 21121, 21122, 21124, 21125, 21126, 21127, 21129, 21130, 
                21131, 21132, 21133, 21134, 21136, 21137, 21139, 21140, 21141, 21142, 21143, 21144, 21145, 21146, 21148, 21149, 21150, 21151, 21152, 21153, 21154, 21155, 21156, 21157, 21158, 21159, 21160, 21161, 21162, 21163, 21164, 21165, 21166, 21167, 21168, 21169, 21170, 21171, 21175, 21177, 21179, 21184, 21186, 21187, 21189, 21192, 21196, 21200, 21205, 21209, 21210, 21211, 21213, 21214, 21216, 21218, 21223, 21225, 21227, 21228, 21231, 21232, 21233, 
                21234, 21235, 21238, 21239, 21240, 21241, 21242, 21243, 21244, 21246, 21247, 21252, 21253, 21255, 21257, 21258, 21259, 21260, 21263, 21264, 21267, 21268, 21269, 21270, 21272, 21273, 21274, 21275, 21276, 21282, 21283, 21284, 21285, 21286, 21287, 21288, 21289, 21291, 21292, 21293, 21295, 21296, 21301, 21302, 21303, 21304, 21306, 21307, 21309, 21312, 21313, 21316, 21319, 21321, 21322, 21324, 21325, 21327, 21328, 21329, 21330, 21332, 21335,
                21337, 21338, 21339, 21341, 21343, 21344, 21346, 21347, 21348, 21349, 21350, 21351, 21353, 21354, 21355, 21357, 21358, 21360, 21364, 21365, 21367, 21370, 21371, 21372, 21373, 21375, 21376, 21378, 21379, 21382, 21386, 21388, 21389, 21390, 21392, 21393, 21394, 21395, 21396, 21397, 21398, 21399, 21400, 21401, 21402, 21403, 21404, 21405, 21406, 21407, 21408, 21409, 21411, 21412, 21414, 21415, 21416, 21417, 21419, 21420, 21421, 21422, 21423, 21424, 21425, 21426, 21427, 21431, 21432, 21433, 21434, 21435, 21436, 21437, 21439, 21440, 21441, 21442, 21443, 21444, 21445, 21447, 21448, 21449, 21450, 21451, 21452, 21453, 21454, 21456, 21457, 21459, 21460, 21461, 21462, 21463, 21464, 21465, 21466, 21469, 21470, 21471, 
                21472, 21473, 21474, 21476, 21477, 21478, 21479, 21480, 21481, 21482, 21483, 21486, 21487, 21488, 21490, 21493, 21495, 21496, 21497, 21498, 21499, 21500, 21501, 21503, 21505, 21506, 21508, 21509, 21510, 21511, 21512, 21513, 21515, 21516, 21517, 21518, 21519, 21520, 21521, 21522, 21523, 21524, 21525, 21527, 21528, 21529, 21530, 21532, 21533, 21534, 21535, 21536, 21537, 21538, 21539, 21540, 21541, 21542, 21543, 21544, 21545, 21546, 21547, 21548, 21550, 21551, 21552, 21553, 21554, 21555, 21556, 21558, 21559, 21560, 21561, 21562, 21563, 21564, 21565, 21567, 21568, 21569, 21570, 21571, 21572, 21573, 21574, 21575, 21576, 21579, 21580, 21581, 21582, 21584, 21585, 21586, 21587, 21589, 21594, 21595, 21596, 21599, 21600, 21601, 21602, 21603, 21604, 21605, 21606, 21607, 21609, 21611, 21612, 21613, 21614, 21615, 21616, 21617, 21618, 21619, 21621, 21622, 21623, 21624, 21625, 21627, 21628, 21629, 21630, 21631, 21632, 21633, 21634, 21635, 21636, 21637, 21640, 21641, 21642, 21643, 21644, 21645, 21647, 21648, 21649, 21650, 21652, 21653, 21654, 21655, 21656, 21657, 21658, 21660, 21661, 21662, 21663, 21664, 21667, 21668, 21669, 21670, 21671, 21673, 21674, 21676, 21677, 21679, 21680, 21681, 21682, 21683, 21684, 21685, 21686, 21687, 21688, 21689, 21690, 21691, 21692, 21693, 21694, 21696, 21697, 21699, 21700, 21701, 21702, 21704, 21705, 21706, 21707, 21708, 21709, 21710, 21719, 21721, 21722, 21723, 21724, 21726, 21729, 21730, 21732, 21736, 21739, 21740, 21742, 21743, 21750, 21751, 21753, 21754, 21756, 21757, 21758, 21760, 21761, 21762, 21763, 21764, 21765, 21767, 21769, 21770, 21773, 21774, 21775, 21776, 21777, 21781, 21782, 21783, 21786, 21787, 21788, 21790, 21792, 21793, 21794, 21795, 21797, 21798, 21807, 21808, 21809, 21810, 21814, 21821, 21822, 21826, 21828, 21829, 21831, 21832, 21835, 21841, 21842, 21843, 21844, 21846, 21848, 21851, 21852, 21854, 21859, 21860, 21862, 21864, 21867, 21868, 21869, 21870, 21871, 21873, 21874, 21875, 21879, 21880, 21882, 21883, 21884, 21889, 21890, 21891, 21893, 21896, 21898, 21899, 21900, 
                21901, 21903, 21904, 21905, 21907, 21909, 21910, 21913, 21914, 21915, 21916, 21917, 21918, 21919, 21920, 21921, 21922, 21923, 21924, 21925, 21926, 21927, 21928, 21929, 21930, 21931, 21932, 21933, 21934, 21935, 21937, 21941, 21946, 21949, 21950, 21951, 21953, 21954, 21957, 21958, 21959, 21961, 21962, 21963, 21965, 21966, 21967, 21971, 21972, 21973, 21974, 21975, 21976, 21977, 21978, 21979, 21980, 21981, 21982, 21983, 21984, 21985, 21986, 21987, 21988, 21989, 21990, 21992, 21993, 21994, 21995, 21996, 21997, 21998, 21999, 22000, 22001, 22002, 22003, 22004, 22005, 22006, 22007, 22008, 22009, 22010, 22011, 22012, 22013, 22014, 22015, 22016, 22017, 22018, 22019, 22045, 22046, 22047, 22048, 22049, 22050, 22051, 22052, 22053, 22054, 22055, 22056, 22057, 22058, 22059, 22061, 22062, 22063, 22067, 22068, 22069, 22070, 22071, 22072, 22073, 22074, 22075, 22076, 22084, 22085, 22086, 22087, 22088, 22089, 22090, 22091, 22092, 22093, 22094, 22095, 22096, 22098, 22099, 22101, 22102, 22103, 22104, 22105, 22106, 22107, 22109, 22110, 22111, 22112, 22113, 22114, 22115, 22116, 22117, 22118, 22120, 22123, 22124, 22125, 22126, 22128, 22131, 22132, 22133, 22134, 22135, 22136, 22137, 22138, 22139, 22140, 22141, 22142, 22143, 22144, 22145, 22147, 22149, 22150, 22151, 22152, 22153, 22154, 22158, 22159, 22162, 22163, 22164, 22165, 22166, 22167, 22168, 22169, 22172, 22173, 22175, 22179, 22180, 22181, 22182, 22183, 22184, 22185, 22186, 22187, 22188, 22190, 22191, 22192, 22193, 22194, 22195, 22201, 22202, 22203, 22204, 22206, 22207, 22208, 22209, 22211, 22212, 22215, 22217, 22218, 22220, 22221, 22223, 22224, 22225, 22226, 22227, 22228, 22229, 22230, 22231, 22232, 22233, 22234, 22235, 22236, 22237, 22240, 22241, 22242, 22243, 22244, 22245, 22247, 22249, 22250, 22251, 22252, 22253, 22254, 22255, 22256, 22257, 22258, 22259, 22260, 22261, 22262, 22263, 22264, 22265, 22266, 22269, 22271, 22273, 22274, 22275, 22276, 22277, 22278, 22280, 22282, 22283, 22284, 22285, 22287, 22289, 22290, 22291, 22293, 22297, 22298, 22299, 22302, 22303, 22305, 
                22306, 22307, 22310, 22311, 22312, 22313, 22314, 22315, 22316, 22318, 22319, 22320, 22323, 22324, 22325, 22326, 22328, 22330, 22331, 22332, 22334, 22336, 22337, 22338, 22340, 22341, 22344, 22345, 22347, 22348, 22349, 22350, 22351, 22354, 22358, 22361, 22362, 22363, 22364, 22365, 22368, 22370, 22371, 22372, 22373, 22374, 22375, 22376, 22377, 22380, 22382, 22383, 22384, 22385, 22388, 22392, 22393, 22395, 22396, 22397, 22398, 22399, 22400, 22401, 22402, 22403, 22405, 22406, 22407, 22409, 22410, 22411, 22412, 22415, 22417, 22418, 22421, 22422, 22423, 22425, 22426, 22427, 22433, 22434, 22435, 22437, 22438, 22439, 22440, 22441, 22442, 22443, 22445, 22448, 22449, 22450, 22452, 22453, 22454, 22455, 22456, 22458, 22460, 22461, 22463, 22464, 22465, 22466, 22467, 22468, 22471, 22473, 22479, 22480, 22481, 22482, 22483, 22484, 22485, 22486, 22487, 22488, 22489, 22490, 22491, 22492, 22493, 22494, 22495, 22496, 22497, 22498, 22499, 22500, 22501, 22502, 22503, 22504, 22505, 22506, 22507, 22508, 22509, 22510, 22513, 22516, 22517, 22518, 22521, 22522, 22523, 22525, 22529, 22530, 22534, 22537, 22538, 22540, 22544, 22545, 22546, 22548, 22556, 22557, 22558, 22559, 22560, 22561, 22562, 22564, 22565, 22566, 22567, 22568, 22569, 22570, 22572, 22573, 22574, 22575, 22576, 22578, 22582, 22583, 22584, 22587, 22588, 22589, 22590, 22591, 22592, 22593, 22594, 22595, 22596, 22598, 22599, 22602, 22603, 22604, 22605, 22606, 22607, 22608, 22609, 22610, 22612, 22613, 22614, 22615, 22616, 22618, 22619, 22620, 22621, 22622, 22623, 22624, 22626, 22627, 22628, 22630, 22631, 22632, 22633, 22634, 22635, 22636, 22637, 22638, 22639, 22640, 22641, 22642, 22643, 22644, 22645, 22646, 22647, 22648, 22649, 22657, 22658, 22659, 22661, 22662, 22663, 22664, 22665, 22666, 22667, 22668, 22669, 22670, 22671, 22672, 22673, 22675, 22676, 22677, 22678, 22679, 22680, 22681, 22682, 22683, 22685, 22686, 22687, 22688, 22689, 22690, 22691, 22692, 22693, 22694, 22695, 22696, 22697, 22698, 22699, 22700, 22701, 22702, 22703, 22704, 22705, 22706, 22707, 22708, 22709, 
                22711, 22712, 22713, 22714, 22715, 22716, 22717, 22718, 22722, 22727, 22728, 22731, 22732, 22733, 22735, 22736, 22737, 22738, 22739, 22740, 22741, 22742, 22744, 22747, 22748, 22749, 22755, 22757, 22760, 22763, 22764, 22765, 22766, 22767, 22768, 22769, 22770, 22771, 22772, 22773, 22774, 22776, 22778, 22779, 22780, 22781, 22783, 22786, 22797, 22800, 22801, 22806, 22807, 22810, 22811, 22819, 22821, 22823, 22824, 22825, 22826, 22827, 22828, 22831, 22832, 22834, 22835, 22840, 22842, 22843, 22844, 22845, 22848, 22849, 22850, 22851, 22852, 22853, 22854, 22855, 22856, 22857, 22858, 22859, 22860, 22861, 22863, 22866, 22868, 22869, 22870, 22871, 22872, 22874, 22875, 22876, 22877, 22878, 22879, 22881, 22882, 22884, 22885, 22886, 22890, 22891, 22893, 22895, 22900, 22901, 22902, 22907, 22908, 22909, 22914, 22915, 22916, 22917, 22918, 22919, 22920, 22921, 22922, 22923, 22924, 22925, 22926, 22927, 22928, 22929, 22930, 22931, 22933, 22934, 22935, 22936, 22937, 22938, 22939, 22941, 22942, 22943, 22947, 22951, 22952, 22955, 22956, 22959, 22960, 22961, 22962, 22963, 22964, 22967, 22971, 22972, 22973, 22974, 22975, 22976, 22977, 22978, 22979, 22981, 22982, 22983, 22984, 22986, 22987, 22996, 22997, 22998, 22999, 23000, 23001, 23003, 23005, 23006, 23008, 23012, 23015, 23023, 23024, 23025, 23026, 23028, 23029, 23030, 23031, 23032, 23033, 23034, 23035, 23036, 23038, 23039, 23040, 23042, 23043, 23044, 23045, 23046, 23047, 23049, 23050, 23051, 23052, 23053, 23054, 23055, 23057, 23058, 23059, 23060, 23061, 23062, 23063, 23065, 23066, 23067, 23070, 23071, 23076, 23077, 23080, 23081, 23083, 23085, 23086, 23087, 23090, 23093, 23094, 23099, 23105, 23106, 23107, 23108, 23110, 23111, 23112, 23115, 23116, 23117, 23118, 23120, 23121, 23122, 23123, 23125, 23127, 23128, 23132, 23134, 23137, 23138, 23140, 23141, 23142, 23143, 23144, 23145, 23149, 23152, 23262, 23263, 23264, 23265, 23266, 23267, 23268, 23269, 23270, 23271, 23272, 23273, 23274, 23275, 23276, 23277, 23278, 23279, 23280, 23281, 23282, 23283, 23284, 23285, 23286, 23287, 23288, 
                23289, 23290, 23291, 23292, 23293, 23294, 23295, 23297, 23298, 23299, 23300, 23301, 23302, 23303, 23304, 23305, 23306, 23307, 23308, 23309, 23310, 23311, 23312, 23313, 23314, 23315, 23316, 23317, 23318, 23319, 23320, 23321, 23322, 23323, 23328, 23329, 23330, 24421, 24422, 24423, 24424, 24425, 24426, 24427, 24428, 24430, 24431, 24432, 24433, 24434, 24435, 24436, 26391, 26668, 27021, 27154, 27284, 27996, 28142, 28229, 28325, 28329, 28334, 28363, 28431, 28503, 28816, 28887, 29351, 29352, 29353, 29354, 29355, 29356, 29357, 29358, 29359, 29360, 29361, 29362, 29363, 29364, 29365, 29366, 29367
            };

                    int playerIndex = 0;

                    foreach (TableRecordModel record in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                    {
                        if (record.Deleted)
                            continue;

                        PlayerRecord playerRecord = (PlayerRecord)record;

                        if (playerIndex < numberList.Count)
                    
                            // Assign a number from the list to the player
                            playerRecord.PlayerId = numberList[playerIndex];
                        playerRecord.NFLID = numberList[playerIndex];

                        // Increment playerIndex for the next player
                        playerIndex++;

                    }
                    MessageBox.Show("Operation completed successfully.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"Error: {ex.Message}"); 
            }
        }



        private void PortingInStructionsbutton_Click(object sender, EventArgs e)        //new 12-23-23
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "PortingInstructions.txt");

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

        private void PlayersToDraftClassButton_Click(object sender, EventArgs e)    //new 12-23-23
        {
            try
            {
                string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "playersToDraftClass.exe");

                // Check if the application executable exists
                if (File.Exists(applicationPath))
                {
                    // Open the application
                    Process.Start(applicationPath);
                }
                else
                {
                    MessageBox.Show("playersToDraftClass.exe not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateDepthChart_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Select your PLAY table CSV file"
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Save the DCHT CSV file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string playCsvPath = openFileDialog.FileName;
                string dchtCsvPath = saveFileDialog.FileName;

                try
                {
                    var playRecords = new List<PlayRecord>();
                    using (var reader = new StreamReader(playCsvPath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
                    {
                        playRecords = csv.GetRecords<PlayRecord>().ToList();
                    }

                    var sortedRecords = playRecords.OrderBy(r => r.TGID).ThenBy(r => r.PPOS).ThenByDescending(r => r.POVR).ToList();

                    var dchtRecords = new List<DCHTRecord>();
                    var groupedByTeam = sortedRecords.GroupBy(r => r.TGID);

                    foreach (var teamGroup in groupedByTeam)
                    {
                        var groupedByPosition = teamGroup.GroupBy(r => r.PPOS);

                        foreach (var positionGroup in groupedByPosition)
                        {
                            int ddepForPosition = 0;
                            int depthLimitForPosition = GetDepthLimit(positionGroup.Key);

                            foreach (var record in positionGroup)
                            {
                                if (ddepForPosition >= depthLimitForPosition) break;

                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = record.PGID,
                                    TGID = record.TGID,
                                    PPOS = record.PPOS,
                                    ddep = ddepForPosition
                                });

                                ddepForPosition++;
                            }
                        }

                        // Handle special cases
                        var pkrtSortedPositions = teamGroup.OrderByDescending(r => r.PKRT).ThenByDescending(r => r.POVR).ToList();  // Kick Return
                        var depthLimitForSpecial = GetDepthLimit(21);

                        for (int ddepForSpecial = 0; ddepForSpecial < depthLimitForSpecial; ddepForSpecial++)
                        {
                            if (ddepForSpecial < pkrtSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord  //Kick Returner
                                {
                                    PGID = pkrtSortedPositions[ddepForSpecial].PGID,
                                    TGID = pkrtSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 21,
                                    ddep = ddepForSpecial
                                });

                                dchtRecords.Add(new DCHTRecord  //Punt Returner
                                {
                                    PGID = pkrtSortedPositions[ddepForSpecial].PGID,
                                    TGID = pkrtSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 22,
                                    ddep = ddepForSpecial
                                });
                            }

                            var ppos19Players = teamGroup.Where(r => r.PPOS == 19 || r.PPOS == 20).ToList();    //Kickoff Specialist        adjusted 1-6-24
                            if (ddepForSpecial < ppos19Players.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = ppos19Players[ddepForSpecial].PGID,
                                    TGID = ppos19Players[ddepForSpecial].TGID,
                                    PPOS = 23,
                                    ddep = ddepForSpecial
                                });
                            }

                            var ppos7Players = teamGroup.Where(r => r.PPOS == 7).ToList();  //Long Snapper
                            if (ddepForSpecial < ppos7Players.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = ppos7Players[ddepForSpecial].PGID,
                                    TGID = ppos7Players[ddepForSpecial].TGID,
                                    PPOS = 24,
                                    ddep = ddepForSpecial
                                });
                            }

                            var TDBSortedPositions = teamGroup.Where(r => r.PPOS == 1).OrderByDescending(r => r.PCTH).ToList();   // 3rd Down RB
                            if (ddepForSpecial < TDBSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = TDBSortedPositions[ddepForSpecial].PGID,
                                    TGID = TDBSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 25,
                                    ddep = ddepForSpecial
                                });
                            }

                            var phbSortedPositions = teamGroup.Where(r => r.PPOS == 1).OrderByDescending(r => r.PLTR).ToList();   //Power RB
                            if (ddepForSpecial < phbSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = phbSortedPositions[ddepForSpecial].PGID,
                                    TGID = phbSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 26,
                                    ddep = ddepForSpecial
                                });
                            }

                            var swrSortedPositions = teamGroup.Where(r => r.PPOS == 3).OrderByDescending(r => r.SRRN).ToList();   //Slot WR
                            if (ddepForSpecial < swrSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = swrSortedPositions[ddepForSpecial].PGID,
                                    TGID = swrSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 27,
                                    ddep = ddepForSpecial
                                });
                            }

                            var rleSortedPositions = teamGroup.Where(r => r.PPOS == 10).OrderByDescending(r => r.PFMS).ToList();   //Rush LE
                            if (ddepForSpecial < rleSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = rleSortedPositions[ddepForSpecial].PGID,
                                    TGID = rleSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 28,
                                    ddep = ddepForSpecial
                                });
                            }

                            var rreSortedPositions = teamGroup.Where(r => r.PPOS == 11).OrderByDescending(r => r.PFMS).ToList();   //Rush RE
                            if (ddepForSpecial < rreSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = rreSortedPositions[ddepForSpecial].PGID,
                                    TGID = rreSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 29,
                                    ddep = ddepForSpecial
                                });
                            }

                            var rdtSortedPositions = teamGroup.Where(r => r.PPOS == 12).OrderByDescending(r => r.PFMS).ToList();   //Rush DT
                            if (ddepForSpecial < rdtSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = rdtSortedPositions[ddepForSpecial].PGID,
                                    TGID = rdtSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 30,
                                    ddep = ddepForSpecial
                                });
                            }

                            var SLBSortedPositions = teamGroup.Where(r => r.PPOS == 13 || r.PPOS == 14 || r.PPOS == 15 || r.PLTY == 43).OrderByDescending(r => r.POVR).ToList();    //Sub LB
                            if (ddepForSpecial < SLBSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = SLBSortedPositions[ddepForSpecial].PGID,
                                    TGID = SLBSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 31,
                                    ddep = ddepForSpecial
                                });
                            }

                            var scbSortedPositions = teamGroup.Where(r => r.PPOS == 16).OrderByDescending(r => r.PTAK).ToList();   // Nickel DB
                            if (ddepForSpecial < rdtSortedPositions.Count)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = scbSortedPositions[ddepForSpecial].PGID,
                                    TGID = scbSortedPositions[ddepForSpecial].TGID,
                                    PPOS = 32,
                                    ddep = ddepForSpecial
                                });
                            }


                            var qb3SortedPositions = teamGroup.Where(r => r.PPOS > 0 && r.PPOS <= 20).OrderByDescending(r => r.PTHP).ToList();  // new 1-13-24
                            if (qb3SortedPositions.Count > 0)
                            {
                                dchtRecords.Add(new DCHTRecord
                                {
                                    PGID = qb3SortedPositions[ddepForSpecial].PGID,
                                    TGID = qb3SortedPositions[ddepForSpecial].TGID,
                                    PPOS = 0,
                                    ddep = 2
                                });
                            }
                        }

                        var specialPpos19Players = teamGroup.Where(r => r.PPOS == 19).OrderByDescending(r => r.POVR).ToList();
                        var specialPpos20Players = teamGroup.Where(r => r.PPOS == 20).OrderByDescending(r => r.POVR).ToList();

                        if (specialPpos19Players.Count > 0)
                        {
                            dchtRecords.Add(new DCHTRecord
                            {
                                PGID = specialPpos19Players[0].PGID,
                                TGID = specialPpos19Players[0].TGID,
                                PPOS = 20,
                                ddep = 1
                            });
                        }

                        if (specialPpos20Players.Count > 0)
                        {
                            dchtRecords.Add(new DCHTRecord
                            {
                                PGID = specialPpos20Players[0].PGID,
                                TGID = specialPpos20Players[0].TGID,
                                PPOS = 19,
                                ddep = 1
                            });
                        }
                    }

                    using (var writer = new StreamWriter(dchtCsvPath))
                    {
                        // Write custom header line
                        writer.WriteLine("DCHT,2008,No");
                        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
                        {
                            // Write records
                            csv.WriteRecords(dchtRecords);
                        }
                    }

                    MessageBox.Show("Depth Chart has been generated.");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"An error occurred while accessing the file: {ex.Message}. Please make sure the file is not in use by another process and try again.", "File Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Operation cancelled.");
            }
        }

        // Existing classes and methods...

        public class PlayRecord
        {
            public int PGID { get; set; }   // Player ID
            public int TGID { get; set; }   // Team ID
            public int PPOS { get; set; }   // Position
            public int POVR { get; set; }   // Overall
            public int PKRT { get; set; }   // Kick Return
            public int PLTR { get; set; }   // Trucking
            public int PCTH { get; set; }   // Catching
            public int SRRN { get; set; }   // Short Route Running
            public int PFMS { get; set; }   // Pass Rush
            public int PLMC { get; set; }   // Pass Coverage
            public int PTAK { get; set; }   // Tackling
            public int PLTY { get; set; }   //Player Type
            public int PTHP { get; set; }   //Throw Power
        }

        public class DCHTRecord
        {
            public int ddep { get; set; }
            public int PGID { get; set; }
            public int PPOS { get; set; }
            public int TGID { get; set; }
        }

        private int GetDepthLimit(int position)
        {
            // You can implement logic to determine the depth limit for each position
            // For example, return 3 for most positions and 2 for special cases like PKRT, PPOS 19, and PPOS 7.
            // You may need to adjust this based on your specific requirements.
            return position == 3 || position == 16 ? 6 : position == 1 ? 4 : 3;
        }
    

    private void ExportLeagueVisualsCSV_Button_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            Stream myStream = null;

            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;

            Dictionary<int, string> genericHeadMapping = new Dictionary<int, string>
{
                { 1, "gen_1_B_B_005" },
                { 2, "gen_1_B_G_01" },
                { 3, "gen_1_B_N_0012" },
                { 4, "gen_1_B_N_007" },
                { 5, "gen_1_B_N_01" },
                { 6, "gen_1_B_N_010" },
                { 7, "gen_1_B_N_011" },
                { 8, "gen_1_B_N_02" },
                { 9, "gen_1_B_N_03" },
                { 10, "gen_1_B_N_04" },
                { 11, "gen_1_B_S_001" },
                { 12, "gen_1_B_S_002" },
                { 13, "gen_1_BM_MB_005" },
                { 14, "gen_1_H_B_009" },
                { 15, "gen_1_H_BD_01" },
                { 16, "gen_1_H_BD_02" },
                { 17, "gen_1_H_N_010" },
                { 18, "gen_1_H_N_015" },
                { 19, "gen_1_H_S_003" },
                { 20, "gen_1_H_S_008" },
                { 21, "gen_1_M_N_02" },
                { 22, "gen_1_M_S_011" },
                { 23, "gen_2_B_B_004" },
                { 24, "gen_2_B_B_005" },
                { 25, "gen_2_B_B_006" },
                { 26, "gen_2_B_BD_03" },
                { 27, "gen_2_B_N_0010" },
                { 28, "gen_2_B_N_0011" },
                { 29, "gen_2_B_N_0012" },
                { 30, "gen_2_B_N_0013" },
                { 31, "gen_2_B_N_0014" },
                { 32, "gen_2_B_N_0015" },
                { 33, "gen_2_B_N_0016" },
                { 34, "gen_2_B_N_0017" },
                { 35, "gen_2_B_N_0018" },
                { 36, "gen_2_B_N_006" },
                { 37, "gen_2_B_N_009" },
                { 38, "gen_2_B_N_01" },
                { 39, "gen_2_B_N_02" },
                { 40, "gen_2_B_N_03" },
                { 41, "gen_2_B_S_001" },
                { 42, "gen_2_B_S_005" },
                { 43, "gen_2_B_S_006" },
                { 44, "gen_2_B_S_007" },
                { 45, "gen_2_B_S_008" },
                { 46, "gen_2_B_S_009" },
                { 47, "gen_2_BMH_G_009" },
                { 48, "gen_2_BMH_S_002" },
                { 49, "gen_2_BMH_S_003" },
                { 50, "gen_2_BMH_S_004" },
                { 51, "gen_2_BMH_S_010" },
                { 52, "gen_2_H_B_002" },
                { 53, "gen_2_H_B_004" },
                { 54, "gen_2_H_B_005" },
                { 55, "gen_2_H_B_010" },
                { 56, "gen_2_H_BD_03" },
                { 57, "gen_2_H_G_01" },
                { 58, "gen_2_H_G_02" },
                { 59, "gen_2_H_GM_004" },
                { 60, "gen_2_H_MB_012" },
                { 61, "gen_2_H_N_006" },
                { 62, "gen_2_H_N_007" },
                { 63, "gen_2_H_N_009" },
                { 64, "gen_2_M_B_01" },
                { 65, "gen_2_M_B_02" },
                { 66, "gen_2_M_N_007" },
                { 67, "gen_2_M_N_04" },
                { 68, "gen_2_M_N_05" },
                { 69, "gen_2_M_N_22" },
                { 70, "gen_2_M_S_001" },
                { 71, "gen_2_M_S_011" },
                { 72, "gen_2_T_N_01" },
                { 73, "gen_2_T_N_02" },
                { 74, "gen_2_T_S_007" },
                { 75, "gen_2_T_S_01" },
                { 76, "gen_3_B_B_001" },
                { 77, "gen_3_B_B_009" },
                { 78, "gen_3_B_B_01" },
                { 79, "gen_3_B_BD_02" },
                { 80, "gen_3_B_N_004" },
                { 81, "gen_3_B_N_006" },
                { 82, "gen_3_B_N_007" },
                { 83, "gen_3_B_N_01" },
                { 84, "gen_3_B_N_02" },
                { 85, "gen_3_B_S_010" },
                { 86, "gen_3_B_S_011" },
                { 87, "gen_3_BMT_S_003" },
                { 88, "gen_3_H_B_008" },
                { 89, "gen_3_H_B_01" },
                { 90, "gen_3_H_BD_01" },
                { 91, "gen_3_H_MS_01" },
                { 92, "gen_3_H_N_01" },
                { 93, "gen_3_H_S_001" },
                { 94, "gen_3_M_N_01" },
                { 95, "gen_3_M_N_21" },
                { 96, "gen_3_T_N_004" },
                { 97, "gen_3_T_N_01" },
                { 98, "gen_4_B_B_004" },
                { 99, "gen_4_B_BM_01" },
                { 100, "gen_4_B_G_003" },
                { 101, "gen_4_B_G_004" },
                { 102, "gen_4_B_M_01" },
                { 103, "gen_4_B_MS_01" },
                { 104, "gen_4_B_MS_02" },
                { 105, "gen_4_B_MS_03" },
                { 106, "gen_4_B_N_002" },
                { 107, "gen_4_B_N_003" },
                { 108, "gen_4_B_N_004" },
                { 109, "gen_4_B_N_005" },
                { 110, "gen_4_B_N_01" },
                { 111, "gen_4_B_N_02" },
                { 112, "gen_4_BHM_S_01" },
                { 113, "gen_4_BM_B_001" },
                { 114, "gen_4_H_BD_01" },
                { 115, "gen_4_H_N_001" },
                { 116, "gen_4_H_N_003" },
                { 117, "gen_4_H_N_004" },
                { 118, "gen_4_H_N_005" },
                { 119, "gen_4_M_BD_02" },
                { 120, "gen_4_M_M_005" },
                { 121, "gen_4_M_N_01" },
                { 122, "gen_4_T_G_01" },
                { 123, "gen_4_T_N_006" },
                { 124, "gen_4_T_N_007" },
                { 125, "gen_4_T_S_01" },
                { 126, "gen_4_T_S_02" },
                { 127, "gen_5_B_BD_02" },
                { 128, "gen_5_B_BD_03" },
                { 129, "gen_5_B_G_01" },
                { 130, "gen_5_B_G_02" },
                { 131, "gen_5_B_MS_02" },
                { 132, "gen_5_B_N_01" },
                { 133, "gen_5_B_N_02" },
                { 134, "gen_5_B_N_03" },
                { 135, "gen_5_B_S_002" },
                { 136, "gen_5_B_S_003" },
                { 137, "gen_5_B_S_008" },
                { 138, "gen_5_BHM_M_002" },
                { 139, "gen_5_BHTM_GS_001" },
                { 140, "gen_5_BTM_N_004" },
                { 141, "gen_5_H_BD_01" },
                { 142, "gen_5_H_BD_02" },
                { 143, "gen_5_H_G_01" },
                { 144, "gen_5_H_MG_006" },
                { 145, "gen_5_H_N_01" },
                { 146, "gen_5_M_B_001" },
                { 147, "gen_5_M_BD_02" },
                { 148, "gen_5_M_M_005" },
                { 149, "gen_5_M_MB_005" },
                { 150, "gen_5_M_N_009" },
                { 151, "gen_5_M_N_01" },
                { 152, "gen_5_M_N_02" },
                { 153, "gen_5_T_G_01" },
                { 154, "gen_5_T_GM_003" },
                { 155, "gen_6_B_G_005" },
                { 156, "gen_6_B_G_01" },
                { 157, "gen_6_B_G_03" },
                { 158, "gen_6_B_G_04" },
                { 159, "gen_6_B_G_05" },
                { 160, "gen_6_B_G_06" },
                { 161, "gen_6_B_MS_01" },
                { 162, "gen_6_B_N_0010" },
                { 163, "gen_6_B_N_0011" },
                { 164, "gen_6_B_N_003" },
                { 165, "gen_6_B_N_004" },
                { 166, "gen_6_B_N_005" },
                { 167, "gen_6_B_N_006" },
                { 168, "gen_6_B_N_007" },
                { 169, "gen_6_B_N_008" },
                { 170, "gen_6_B_N_009" },
                { 171, "gen_6_B_N_01" },
                { 172, "gen_6_B_N_02" },
                { 173, "gen_6_B_N_03" },
                { 174, "gen_6_B_S_01" },
                { 175, "gen_6_B_S_02" },
                { 176, "gen_6_BHM_B_01" },
                { 177, "gen_6_BHM_BM_004" },
                { 178, "gen_6_BHM_G_002" },
                { 179, "gen_6_BM_MG_008" },
                { 180, "gen_6_BM_N_009" },
                { 181, "gen_6_BM_S_01" },
                { 182, "gen_6_BMH_G_010" },
                { 183, "gen_6_BMH_N_003" },
                { 184, "gen_6_BTM_BM_001" },
                { 185, "gen_6_H_B_002" },
                { 186, "gen_6_H_BD_01" },
                { 187, "gen_6_H_BM_02" },
                { 188, "gen_6_H_G_01" },
                { 189, "gen_6_H_G_012" },
                { 190, "gen_6_H_MS_01" },
                { 191, "gen_6_H_N_001" },
                { 192, "gen_6_H_N_006" },
                { 193, "gen_6_H_N_01" },
                { 194, "gen_6_H_N_20" },
                { 195, "gen_6_M_G_01" },
                { 196, "gen_6_M_MB_011" },
                { 197, "gen_6_M_N_009" },
                { 198, "gen_6_M_N_01" },
                { 199, "gen_6_M_N_02" },
                { 200, "gen_6_N_MG_009" },
                { 201, "gen_6_T_G_005" },
                { 202, "gen_6_T_MG_006" },
                { 203, "gen_6_T_N_009" },
                { 204, "gen_7_B_B_001" },
                { 205, "gen_7_B_B_002" },
                { 206, "gen_7_B_B_005" },
                { 207, "gen_7_B_B_009" },
                { 208, "gen_7_B_G_0010" },
                { 209, "gen_7_B_G_004" },
                { 210, "gen_7_B_G_005" },
                { 211, "gen_7_B_G_007" },
                { 212, "gen_7_B_G_008" },
                { 213, "gen_7_B_G_009" },
                { 214, "gen_7_B_N_0010" },
                { 215, "gen_7_B_N_0011" },
                { 216, "gen_7_B_N_0012" },
                { 217, "gen_7_B_N_0013" },
                { 218, "gen_7_B_N_0014" },
                { 219, "gen_7_B_N_003" },
                { 220, "gen_7_B_N_007" },
                { 221, "gen_7_B_N_009" },
                { 222, "gen_7_B_N_011" },
                { 223, "gen_7_B_N_012" },
                { 224, "gen_7_B_N_013" },
                { 225, "gen_7_B_N_014" },
                { 226, "gen_7_B_N_015" },
                { 227, "gen_7_B_N_016" },
                { 228, "gen_7_B_N_017" },
                { 229, "gen_7_B_N_018" },
                { 230, "gen_7_B_N_019" },
                { 231, "gen_7_B_N_06" },
                { 232, "gen_7_B_N_07" },
                { 233, "gen_7_B_N_08" },
                { 234, "gen_7_B_N_09" },
                { 235, "gen_7_B_S_004" },
                { 236, "gen_7_B_S_005" },
                { 237, "gen_7_BHM_MG_001" },
                { 238, "gen_7_BHM_MG_007" },
                { 239, "gen_7_BHM_MG_022" },
                { 240, "gen_7_BHM_N_002" },
                { 241, "gen_7_BHM_N_004" },
                { 242, "gen_7_BHM_S_010" },
                { 243, "gen_7_BMH_B_006" },
                { 244, "gen_7_BMH_GS_005" },
                { 245, "gen_7_BMH_GS_011" },
                { 246, "gen_7_BMH_MB_019" },
                { 247, "gen_7_BMH_MG_001" },
                { 248, "gen_7_BMH_MG_003" },
                { 249, "gen_7_BMH_MG_008" },
                { 250, "gen_7_BMH_MG_020" },
                { 251, "gen_7_BMH_S_008" },
                { 252, "gen_7_BMT_BM_012" },
                { 253, "gen_7_BMT_M_009" },
                { 254, "gen_7_BMT_MG_002" },
                { 255, "gen_7_BMTH_N_003" },
                { 256, "gen_7_BTM_B_021" },
                { 257, "gen_7_BTM_MB_023" },
                { 258, "gen_7_BTM_MG_006" },
                { 259, "gen_7_BTM_N_007" },
                { 260, "gen_7_H_B_010" },
                { 261, "gen_7_H_BD_03" },
                { 262, "gen_7_H_BMS_013" },
                { 263, "gen_7_H_G_001" },
                { 264, "gen_7_H_G_004" },
                { 265, "gen_7_H_G_006" },
                { 266, "gen_7_H_G_008" },
                { 267, "gen_7_H_G_009" },
                { 268, "gen_7_H_G_02" },
                { 269, "gen_7_H_N_006" },
                { 270, "gen_7_H_N_01" },
                { 271, "gen_7_M_B_01" },
                { 272, "gen_7_M_BD_01" },
                { 273, "gen_7_M_BD_02" },
                { 274, "gen_7_M_BM_014" },
                { 275, "gen_7_M_G_004" },
                { 276, "gen_7_M_G_005" },
                { 277, "gen_7_M_GS_015" },
                { 278, "gen_7_M_MB_009" },
                { 279, "gen_7_M_MB_010" },
                { 280, "gen_7_M_MG_025" },
                { 281, "gen_7_M_N_001" },
                { 282, "gen_7_M_N_005" },
                { 283, "gen_7_M_N_006" },
                { 284, "gen_7_M_N_024" },
                { 285, "gen_7_T_B_006" },
                { 286, "gen_7_T_G_004" },
                { 287, "gen_7_T_G_007" },
                { 288, "gen_7_T_N_001" },
                { 289, "gen_7_T_N_009" },
                { 290, "gen_7_T_S_004" },
                { 400, "gen_1_MorphHead" },
                { 401, "gen_2_MorphHead" },
                { 402, "gen_3_MorphHead" },
                { 403, "gen_4_MorphHead" },
                { 404, "gen_5_MorphHead" },
                { 405, "gen_6_MorphHead" },
                { 406, "gen_7_MorphHead" },
                { 407, "gen_8_MorphHead" },
                { 450, "gen_1_T_S_004" },
                { 451, "gen_4_M_N_001" },
                { 452, "gen_4_M_N_002" },
                { 453, "gen_4_H_N_002" },
                { 454, "gen_5_H_S_010" },
                { 455, "gen_7_M_S_004" }
       };

            Dictionary<int, string> jerseyStyleMapping = new Dictionary<int, string>
{
             { 0, "Gear_JerseyStyle_SleeveTight" },
             { 1, "Gear_JerseyStyle_SleeveStandard" },
             { 2, "Gear_JerseyStyle_SleeveLong" }
};


            Dictionary<int, string> SocksStyleMapping = new Dictionary<int, string>
{
             { 0, "Gear_Socks_Mid" },
             { 1, "Gear_Socks_Low" },
             { 2, "Gear_Socks_High" },
             { 3, "Gear_Socks_Under"}
};
            Dictionary<int, string> HandWarmersStyleMapping = new Dictionary<int, string>
{
             { 0, "Handwarmer_None" },
             { 1, "Handwarmer_Standard" },
             { 2, "Handwarmer_Standard" }
};
            Dictionary<int, string> HandWarmersModStyleMapping = new Dictionary<int, string>
{
             { 0, "HandwarmerStyle_None " },
             { 1, "HandwarmerStyle_Front" },
             { 2, "HandwarmerStyle_Back" }
};
            Dictionary<int, string> MouthPieceStyleMapping = new Dictionary<int, string>
{
             { 0, "GearMouthpiece_None " },
             { 1, "GearMouthpiece_PacifierDual_White" },
             { 2, "GearMouthpiece_PacifierDual_Black" },
             { 3, "GearMouthpiece_PacifierDual_TeamColor" },
             { 4, "GearMouthpiece_PacifierDual_SecondaryColor" }
};
            Dictionary<int, string> NeckPadStyleMapping = new Dictionary<int, string>
{
             { 0, "GearNeckPad_None " },
             { 1, "GearNeckPad_VintageNeckRoll" },
             { 2, "GearNeckPad_ButterflyNeckRoll" },
             { 3, "GearNeckPad_VintageSingleNeckRoll" },
             { 4, "GearNeckPad_CowboyCollarNeckRoll" }
};
            Dictionary<int, string> TowelPositionStyleMapping = new Dictionary<int, string>
{
             { 0, "Towel_None " },
             { 1, "Towel_North" },
             { 2, "Towel_NorthEast" },
             { 3, "Towel_East" },
             { 4, "Towel_SouthEast" },
             { 5, "Towel_South" },
             { 6, "Towel_SouthWest" },
             { 7, "Towel_West" },
             { 8, "Towel_NorthWest" }
};
            Dictionary<int, string> UnderShirtStyleMapping = new Dictionary<int, string>
{
            { 0, "Undershirt_Tucked_in" },
            { 1, "Undershirt_Untucked" },
            { 2, "Undershirt_Rolled_up" },
            { 3, "Undershirt_Compression_ShortSleeve_Nike_White" },
            { 4, "Undershirt_Compression_ShortSleeve_Nike_Red" },
            { 5, "Undershirt_Compression_ShortSleeve_Nike_Pink" },
            { 6, "Undershirt_Compression_ShortSleeve_Nike_Navy" },
            { 7, "Undershirt_Compression_ShortSleeve_Nike_Green" },
            { 8, "Undershirt_Compression_ShortSleeve_Nike_Gold" },
            { 9, "Undershirt_Compression_ShortSleeve_Nike_Black" },
            { 10, "Undershirt_Compression_ShortSleeve_Nike_Bleige" },
            { 11, "Undershirt_Compression_LongSleeve_Nike_Bleige" },
            { 12, "Undershirt_Compression_LongSleeve_Nike_Black" },
            { 13, "Undershirt_Compression_LongSleeve_Nike_Gold" },
            { 14, "Undershirt_Compression_LongSleeve_Nike_Green" },
            { 15, "Undershirt_Compression_LongSleeve_Nike_Navy" },
            { 16, "Undershirt_Compression_LongSleeve_Nike_Pink" },
            { 17, "Undershirt_Compression_LongSleeve_Nike_Red" },
            { 18, "Undershirt_Compression_LongSleeve_Nike_White" },
            { 19, "Undershirt_Compression_LongSleeve_Miami_80s_Blue" },
            { 20, "Undershirt_Hoodie_Sleeveless_Nike_White" },
            { 21, "Undershirt_Hoodie_Sleeveless_Nike_Red" },
            { 22, "Undershirt_Hoodie_Sleeveless_Nike_Pink" },
            { 23, "Undershirt_Hoodie_Sleeveless_Nike_Navy" },
            { 24, "Undershirt_Hoodie_Sleeveless_Nike_Green" },
            { 25, "Undershirt_Hoodie_Sleeveless_Nike_Gold" },
            { 26, "Undershirt_Hoodie_Sleeveless_Nike_Black" },
            { 27, "Undershirt_Hoodie_Sleeveless_Nike_Bleige" },
            { 28, "Undershirt_Hoodie_Sleeveless_Brotherhood_Orange" },
            { 29, "Undershirt_Hoodie_Sleeveless_Brotherhood_Black" }
};
            Dictionary<int, string> VisorStyleMapping = new Dictionary<int, string>
{
            { 0, "GearVisor_None" },
            { 1, "GearVisor_visorDark" },
            { 2, "GearVisor_visorDarkLight" },
            { 3, "GearVisor_visorClear" },
            { 4, "GearVisor_visorOakley_clear" },
            { 5, "GearVisor_visorOakley_Prizm" },
            { 6, "GearVisor_visorOakley_DarkLight" },
            { 7, "GearVisor_visorOakley_Dark" },
            { 8, "v_NTS_Visor_RED" }
};
            Dictionary<int, string> FacePaintStyleMapping = new Dictionary<int, string>
{
            { 0, "FaceMarks_None" },
            { 1, "FaceMarks_NoseTape" },
            { 2, "FaceMarks_EyePaint" },
            { 3, "FaceMarks_EyeTape" },
            { 4, "FaceMarks_EyePaint2" },
            { 5, "FaceMarks_NoseTapeEyePaint" },
            { 6, "FaceMarks_NoseEyeTape" },
            { 7, "FaceMarks_EyePaintCross" },
            { 8, "FaceMarks_EyePaint3" },
            { 9, "FaceMarks_EyeTapeLeft" },
            { 10,"FaceMarks_EyeTapeRight" }
};
            Dictionary<int, string> HelmetStyleMapping = new Dictionary<int, string>
{
            { 0, "GearHelmet_Standard" },
            { 1, "GearHelmet_Schutt" },
            { 2, "GearHelmet_AirXP" },
            { 3, "GearHelmet_VicisZero1" },
            { 4, "GearHelmet_standardBrady" },
            { 5, "GearHelmet_SchuttVeng" },
            { 6, "GearHelmet_Revolution" },
            { 7, "GearHelmet_RevolutionSpeed" },
            { 8, "GearHelmet_Speed_Flex" },
            { 9, "GearHelmet_Riddell360" },
            { 10, "GearHelmet_X2E" },
            { 11, "GearHelmet_RiddellTK" },
            { 12, "GearHelmet_XenithEpic" },
            { 13, "GearHelmet_VengeanceZ10" },
            { 14, "GearHelmet_SchuttF7" },
            { 15, "GearHelmet_XenithShadow" },
            { 16, "GearHelmet_VicisZero2" },
            { 17, "GearHelmet_VicisZero2Trench" },
            { 18, "GearHelmet_Axiom" },
            { 19, "GearHelmet_None" }
};
            Dictionary<int, string> FaceMaskStyleMapping = new Dictionary<int, string>
{
            { 0, "GearFaceMask_2Bar" },
            { 1, "GearFaceMask_2BarSingle" },
            { 2, "GearFaceMask_3Bar" },
            { 4, "GearFaceMask_3BarQB" },
            { 6, "GearFaceMask_3BarRB" },
            { 7, "GearFaceMask_3BarRBJagged" },
            { 5, "GearFaceMask_3BarRBSingle" },
            { 3, "GearFaceMask_3BarSingle" },
            { 8, "GearFaceMask_BullRB" },
            { 66, "GearFaceMask_None" },
            { 108, "GearFaceMask_F7FullcageRobot" },
            { 110, "GearFaceMask_F7Kicker" },
            { 109, "GearFaceMask_F7RobotRB2" },
            { 14, "GearFaceMask_FullCage2" },
            { 12, "GearFaceMask_FullCage" },
            { 11, "GearFaceMask_FullCageRobot" },
            { 13, "GearFaceMask_FULLCAGESINGLE" },
            { 15, "GearFaceMask_HalfCage" },
            { 16, "GearFaceMask_HalfCage2" },
            { 17, "GearFaceMask_Kicker" },
            { 21, "GearFaceMask_REVO3Bar" },
            { 23, "GearFaceMask_REVO3BarLb" },
            { 22, "GearFaceMask_REVO3BarRB" },
            { 27, "GearFaceMask_revofullcage" },
            { 28, "GearFaceMask_revofullcage2" },
            { 29, "GearFaceMask_REVOfullcage3" },
            { 26, "GearFaceMask_RevoHalfCage" },
            { 30, "GearFaceMask_revoKicker" },
            { 19, "GearFaceMask_RevoNormal" },
            { 20, "GearFaceMask_RevoNormal2" },
            { 24, "GearFaceMask_RevoRobot" },
            { 25, "GearFaceMask_RevoRobot2" },
            { 31, "GearFaceMask_revospeed2bar" },
            { 32, "GearFaceMask_revospeed2barSingle" },
            { 33, "GearFaceMask_Revospeed3bar" },
            { 36, "GearFaceMask_revoSpeed3barLb" },
            { 37, "GearFaceMask_revospeed3BarLBStraight" },
            { 34, "GearFaceMask_revoSpeed3barSingle" },
            { 35, "GearFaceMask_revospeed3barstraight" },
            { 43, "GearFaceMask_RevospeedCage" },
            { 41, "GearFaceMask_revoSpeedFullCage" },
            { 42, "GearFaceMask_revoSpeedFullCage2" },
            { 44, "GearFaceMask_RevoSpeedGrid" },
            { 45, "GearFaceMask_revospeedKicker" },
            { 38, "GearFaceMask_revoSpeedRobot" },
            { 39, "GearFaceMask_revoSpeedRobot2" },
            { 40, "GearFaceMask_revoSpeedRobotRB" },
            { 105, "GearFaceMask_RevospeedHalfCage" },
            { 143, "GearFaceMask_Revospeed_RC" },
            { 90, "GearFaceMask_Revospeed2BarWR" },
            { 86, "GearFaceMask_Revospeed3BarQB" },
            { 89, "GearFaceMask_Revospeed808" },
            { 91, "GearFaceMask_RevospeedFullcageHook" },
            { 64, "GearFaceMask_Riddell3603BarLB" },
            { 65, "GearFaceMask_Riddell360FullCage" },
            { 62, "GearFaceMask_Riddell360Robot" },
            { 63, "GearFaceMask_Riddell360Robot2" },
            { 9, "GearFaceMask_Robot" },
            { 10, "GearFaceMask_RobotRB" },
            { 99, "GearFaceMask_F72Bar" },
            { 103, "GearFaceMask_F73BarRB" },
            { 100, "GearFaceMask_F73Bar" },
            { 104, "GearFaceMask_F7FullCage" },
            { 101, "GearFaceMask_F7Robot" },
            { 102, "GearFaceMask_F7RobotRB" },
            { 140, "GearFaceMask_Speedflex_2_Bar_WR" },
            { 112, "GearFaceMask_Speedflex3Bar" },
            { 123, "GearFaceMask_Speedflex3BarJagged" },
            { 122, "GearFaceMask_SpeedFlex3BarLBGrid" },
            { 124, "GearFaceMask_Speedflex3BarRBJagged" },
            { 115, "GearFaceMask_Speedflex3BarSingle" },
            { 111, "GearFaceMask_SpeedFlex808" },
            { 120, "GearFaceMask_SpeedFlexBulldog" },
            { 119, "GearFaceMask_SpeedFlexCageHook" },
            { 118, "GearFaceMask_SpeedflexFullcageHook" },
            { 117, "GearFaceMask_SpeedflexFullcageRobot" },
            { 116, "GearFaceMask_SpeedflexHalfCage" },
            { 144, "GearFaceMask_SpeedflexHalfCage2" },
            { 141, "GearFaceMask_Speedflex_3_Bar_LB_Jewel" },
            { 113, "GearFaceMask_SpeedFlexKicker" },
            { 121, "GearFaceMask_SpeedflexRBBull" },
            { 142, "GearFaceMask_SpeedflexRobot808" },
            { 114, "GearFaceMask_SpeedFlexRobotCage" },
            { 146, "GearFaceMask_Speedflex_Robot_Z" },
            { 56, "GearFaceMask_Speedflex2Bar" },
            { 81, "GearFaceMask_Speedflex2BarQB" },
            { 82, "GearFaceMask_Speedflex2BarSingle" },
            { 60, "GearFaceMask_Speedflex3BarLB" },
            { 85, "GearFaceMask_Speedflex3BarQB" },
            { 59, "GearFaceMask_Speedflex3BarRB" },
            { 84, "GearFaceMask_SpeedFlex3BarRBSingle" },
            { 61, "GearFaceMask_SpeedflexFullcage" },
            { 80, "GearFaceMask_SpeedflexCage" },
            { 57, "GearFaceMask_SpeedflexRobot" },
            { 58, "GearFaceMask_SpeedflexRobotRB" },
            { 83, "GearFaceMask_SpeedflexRobotRBJagged" },
            { 131, "GearFaceMask_2BarNose" },
            { 128, "GearFaceMask_2BarQBBull" },
            { 127, "GearFaceMask_2BarQBSingle" },
            { 132, "GearFaceMask_3BarNose" },
            { 126, "GearFaceMask_3BarQBSingle" },
            { 129, "GearFaceMask_FlatHalfCage" },
            { 130, "GearFaceMask_HalfCage3" },
            { 92, "GearFaceMask_Standard2BarWR" },
            { 18, "GearFaceMask_StdBulldog" },
            { 54, "GearFaceMask_Vengeance3BarRB" },
            { 107, "GearFaceMask_Vengeance2BarSingle" },
            { 106, "GearFaceMask_Vengeance3Bar" },
            { 98, "GearFaceMask_VengeanceKicker" },
            { 87, "GearFaceMask_VengeanceFullCageBulldog" },
            { 94, "GearFaceMask_VengeanceZ102Bar" },
            { 95, "GearFaceMask_VengeanceZ103BarLB" },
            { 96, "GearFaceMask_VengeanceZ10Cage" },
            { 93, "GearFaceMask_VengeanceZ10Robot" },
            { 88, "GearFaceMask_VengeanceZ10RobotRB" },
            { 55, "GearFaceMask_VengeanceFullCage" },
            { 51, "GearFaceMask_VengeanceQB" },
            { 52, "GearFaceMask_VengeanceRobot" },
            { 53, "GearFaceMask_VengeanceRobotRB" },
            { 139, "GearFaceMask_Vicis_Kicker" },
            { 147, "GearFaceMask_VicisZero2BAR" },
            { 152, "GearFaceMask_VicisZero23BARLB" },
            { 151, "GearFaceMask_VicisZero23BARRB" },
            { 155, "GearFaceMask_VicisZero2Kicker" },
            { 148, "GearFaceMask_VicisZero2Robot" },
            { 153, "GearFaceMask_VicisZero2RobotLB" },
            { 149, "GearFaceMask_VicisZero2RobotRB" },
            { 150, "GearFaceMask_VicisZero2RobotRB2" },
            { 154, "GearFaceMask_VicisZero2Trench" },
            { 74, "GearFaceMask_VicisZero12Bar" },
            { 78, "GearFaceMask_VicisZero13Bar" },
            { 73, "GearFaceMask_VicisZero13BarLB" },
            { 76, "GearFaceMask_VicisZero13BarRB" },
            { 77, "GearFaceMask_VicisZero1BullRB" },
            { 75, "GearFaceMask_VicisZero1Fullcage" },
            { 71, "GearFaceMask_VicisZero1Robot" },
            { 72, "GearFaceMask_VicisZero1RobotRB" },
            { 134, "GearFaceMask_Vintage2BarBull" },
            { 137, "GearFaceMask_Vintage2BarNose" },
            { 136, "GearFaceMask_Vintage2BarQBSingle" },
            { 135, "GearFaceMask_VintageHalfCage" },
            { 138, "GearFaceMask_VintageHalfCage2" },
            { 133, "GearFaceMask_VintageKicker" },
            { 70, "GearFaceMask_VintageLong" },
            { 67, "GearFaceMask_VintageOneBar" },
            { 69, "GearFaceMask_VintageStandard" },
            { 68, "GearFaceMask_VintageTwoBar" },
            { 46, "GearFaceMask_Xenith2Bar" },
            { 48, "GearFaceMask_Xenith3barrb" },
            { 50, "GearFaceMask_XenithFullcage" },
            { 125, "GearFaceMask_XenithKicker" },
            { 145, "GearFaceMask_Xenith_Prowl" },
            { 47, "GearFaceMask_XenithRobot" },
            { 49, "GearFaceMask_XenithRobotrb" },
            { 79, "GearFaceMask_XenithPredator" },
            { 97, "GearFaceMask_XenithPrism" },
            { 156, "GearFaceMask_Axiom2barsingle" },
            { 157, "GearFaceMask_Axiom3BarJagged" },
            { 158, "GearFaceMask_Axiom3BarLBJagged" },
            { 159, "GearFaceMask_Axiom3BarLBSingle" },
            { 160, "GearFaceMask_Axiom3BarSingle" }
};
            Dictionary<int, string> KneePadStyleMapping = new Dictionary<int, string>
{
            { 0, "KneePad_None" },
            { 1, "KneePad_Nike" },
            { 2, "KneePad_Regular" }
};
            Dictionary<int, string> LeftArmSleeveStyleMapping = new Dictionary<int, string>
{
            { 0, "GearArmSleeve_none" },
            { 5, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_White" },
            { 6, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_Black" },
            { 7, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_TeamColor" },
            { 21, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 1, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_White" },
            { 2, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_Black" },
            { 3, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_TeamColor" },
            { 4, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 8, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_White" }, 
            { 9, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_Black" }, 
            { 10, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_TeamColor" }, 
            { 22, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_Secondary" },   
            { 11, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_White" },
            { 12, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_Black" },
            { 13, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_TeamColor" },
            { 14, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 15, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_White" },
            { 16, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_Black" },
            { 17, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_TeamColor" },
            { 18, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 19, "GearArmSleeve_Quarter_armTape_normal_OffWhite" },
            { 31, "GearArmSleeve_Quarter_armTape_normal_Black" },
            { 32, "GearArmSleeve_Quarter_armTape_normal_TeamColor" },
            { 33, "GearArmSleeve_Quarter_armTape_normal_SecondaryColor" },
            { 20, "GearArmSleeve_Undershirt_armTape_normal_OffWhite" },    //GearArmSleeve_Baggy_White
            { 34, "GearArmSleeve_Undershirt_armTape_normal_Black" },    //GearArmSleeve_Baggy_Black
            { 35, "GearArmSleeve_Undershirt_armTape_normal_TeamColor" },    //GearArmSleeve_Baggy_TeamColor
            { 36, "GearArmSleeve_Undershirt_armTape_normal_SecondaryColor" },   //GearArmSleeve_Baggy_SecondaryColor
            { 37, "GearArmSleeve_Baggy_White" },
            { 38, "GearArmSleeve_Baggy_Black" },
            { 39, "GearArmSleeve_Baggy_TeamColor" },
            { 40, "GearArmSleeve_Baggy_SecondaryColor" }
};
            Dictionary<int, string> RightArmSleeveStyleMapping = new Dictionary<int, string>
{
            { 0, "GearArmSleeve_none" },
            { 5, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_White" },
            { 6, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_Black" },
            { 7, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_TeamColor" },
            { 21, "GearArmSleeve_Half_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 1, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_White" },
            { 2, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_Black" },
            { 3, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_TeamColor" },
            { 4, "GearArmSleeve_Full_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 8, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_White" },      //"GearArmSleeve_Undershirt_armTape_normal_OffWhite" 
            { 9, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_Black" },     //"GearArmSleeve_Undershirt_armTape_normal_Black"
            { 10, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_TeamColor" },        //"GearArmSleeve_Undershirt_armTape_normal_TeamColor" 
            { 22, "GearArmSleeve_Undershirt_sleeveLongUnderarmor_normal_Secondary" },       //"GearArmSleeve_Undershirt_armTape_normal_SecondaryColor"
            { 11, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_White" },
            { 12, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_Black" },
            { 13, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_TeamColor" },
            { 14, "GearArmSleeve_Quarter_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 15, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_White" },
            { 16, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_Black" },
            { 17, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_TeamColor" },
            { 18, "GearArmSleeve_Shooter_sleeveLongUnderarmor_normal_SecondaryColor" },
            { 19, "GearArmSleeve_Quarter_armTape_normal_OffWhite" },
            { 31, "GearArmSleeve_Quarter_armTape_normal_Black" },
            { 32, "GearArmSleeve_Quarter_armTape_normal_TeamColor" },
            { 33, "GearArmSleeve_Quarter_armTape_normal_SecondaryColor" },
            { 20, "GearArmSleeve_Undershirt_armTape_normal_OffWhite" },    //GearArmSleeve_Baggy_White
            { 34, "GearArmSleeve_Undershirt_armTape_normal_Black" },    //GearArmSleeve_Baggy_Black
            { 35, "GearArmSleeve_Undershirt_armTape_normal_TeamColor" },    //GearArmSleeve_Baggy_TeamColor
            { 36, "GearArmSleeve_Undershirt_armTape_normal_SecondaryColor" },   //GearArmSleeve_Baggy_SecondaryColor
            { 37, "GearArmSleeve_Baggy_White" },
            { 38, "GearArmSleeve_Baggy_Black" },
            { 39, "GearArmSleeve_Baggy_TeamColor" },
            { 40, "GearArmSleeve_Baggy_SecondaryColor" }
};
            Dictionary<int, string> LeftElbowGearStyleMapping = new Dictionary<int, string>
{
            { 0, "ElbowGear_none" },
            { 1, "ElbowGear_elbowpad_White" },
            { 2, "ElbowGear_elbowpad_Black" },
            { 3, "ElbowGear_elbowpad_TeamColor" },
            { 4, "ElbowGear_elbowpad_Stripe_White" },
            { 5, "ElbowGear_elbowpad_Stripe_Black" },
            { 6, "ElbowGear_elbowpadRubber_Black" },
            { 7, "ElbowGear_elbowSweatbandFull_White" },
            { 8, "ElbowGear_elbowSweatbandMedium_White" },
            { 9, "ElbowGear_elbowSweatbandThin_White" },
            { 10, "ElbowGear_elbowSweatbandFull_Black" },
            { 11, "ElbowGear_elbowSweatbandMedium_Black" },
            { 12, "ElbowGear_elbowSweatbandThin_Black" },
            { 13, "ElbowGear_elbowSweatbandFull_TeamColor" },
            { 14, "ElbowGear_elbowSweatbandMedium_TeamColor" },
            { 15, "ElbowGear_elbowSweatbandThin_TeamColor" },
            { 16, "ElbowGear_elbowBrace_TeamColor" },
            { 18, "ElbowGear_elbowSweatbandFull_SecondaryColor" },
            { 19, "ElbowGear_elbowSweatbandMedium_SecondaryColor" },
            { 20, "ElbowGear_elbowSweatbandThin_SecondaryColor" },
            { 21, "ElbowGear_VintageBeastWrap_White" },
            { 22, "ElbowGear_armBraceSmall" }
};
            Dictionary<int, string> RightElbowGearStyleMapping = new Dictionary<int, string>
{
            { 0, "ElbowGear_none" },
            { 1, "ElbowGear_elbowpad_White" },
            { 2, "ElbowGear_elbowpad_Black" },
            { 3, "ElbowGear_elbowpad_TeamColor" },
            { 4, "ElbowGear_elbowpad_Stripe_White" },
            { 5, "ElbowGear_elbowpad_Stripe_Black" },
            { 6, "ElbowGear_elbowpadRubber_Black" },
            { 7, "ElbowGear_elbowSweatbandFull_White" },
            { 8, "ElbowGear_elbowSweatbandMedium_White" },
            { 9, "ElbowGear_elbowSweatbandThin_White" },
            { 10, "ElbowGear_elbowSweatbandFull_Black" },
            { 11, "ElbowGear_elbowSweatbandMedium_Black" },
            { 12, "ElbowGear_elbowSweatbandThin_Black" },
            { 13, "ElbowGear_elbowSweatbandFull_TeamColor" },
            { 14, "ElbowGear_elbowSweatbandMedium_TeamColor" },
            { 15, "ElbowGear_elbowSweatbandThin_TeamColor" },
            { 16, "ElbowGear_elbowBrace_TeamColor" },
            { 18, "ElbowGear_elbowSweatbandFull_SecondaryColor" },
            { 19, "ElbowGear_elbowSweatbandMedium_SecondaryColor" },
            { 20, "ElbowGear_elbowSweatbandThin_SecondaryColor" },
            { 21, "ElbowGear_VintageBeastWrap_White" },
            { 22, "ElbowGear_armBraceSmall" }
};
            Dictionary<int, string> LeftHandStyleMapping = new Dictionary<int, string>
{
            { 0, "GearHand_None" },
            { 1, "GearHand_glove_NikeVaporJet_White" },
            { 2, "GearHand_glove_NikeVaporJet_Black" },
            { 3, "GearHand_glove_NikeVaporJet_TeamColor" },
            { 4, "GearHand_glove_Adizero11_White" },
            { 5, "GearHand_glove_Adizero11_Black" },
            { 6, "GearHand_glove_Adizero11_TeamColor" },
            { 7, "GearHand_glove_NikeSuperBad3_White" },
            { 8, "GearHand_glove_NikeSuperBad3_Black" },
            { 9, "GearHand_glove_NikeSuperBad3_TeamColor" },
            { 10, "GearHand_glove_NikeVaporKnit_White" },
            { 11, "GearHand_glove_NikeVaporKnit_Black" },
            { 12, "GearHand_glove_NikeVaporKnit_TeamColor" },
            { 13, "GearHand_glove_NikeHyperBeast_White" },
            { 14, "GearHand_glove_NikeHyperBeast_Black" },
            { 15, "GearHand_tapedHandFinger_White" },
            { 16, "GearHand_tapedHandFinger_Black" },
            { 17, "GearHand_tapedHandFinger_TeamColor" },
            { 18, "GearHand_tapedHandNormal_White" },
            { 19, "GearHand_tapedHandMax_White" },
            { 20, "GearHand_tapedHandCombo_White" },
            { 21, "GearHand_glove_UnderArmourSpotlight2019_TeamColor" },
            { 22, "GearHand_glove_UnderArmourF6_TeamColor" },
            { 23, "GearHand_glove_NikeVaporJet_SecondaryColor" },
            { 24, "GearHand_glove_Adizero11_SecondaryColor" },
            { 25, "GearHand_glove_NikeSuperBad3_SecondaryColor" }, 
            { 26, "GearHand_glove_NikeVaporKnit_SecondaryColor" },
            { 27, "GearHand_glove_UnderArmourSpotlight2019_SecondaryColor" },
            { 28, "GearHand_glove_UnderArmourF6_SecondaryColor" },
            { 29, "GearHand_glove_NikeVaporJet4_Black" },
            { 30, "GearHand_glove_NikeVaporJet4_White" },
            { 31, "GearHand_glove_NikeVaporJet4_TeamColor" },
            { 32, "GearHand_glove_NikeVaporJet4_SecondaryColor" },
            { 33, "GearHand_glove_NikeVaporJet5_Black" },
            { 34, "GearHand_glove_NikeVaporJet5_White" },
            { 35, "GearHand_glove_NikeVaporJet5_TeamColor" },
            { 36, "GearHand_glove_NikeVaporJet5_SecondaryColor" },
            { 37, "GearHand_glove_NikeSuperbad5_Black" },       //edited due to glitch 1-4-23 
            { 38, "GearHand_glove_NikeSuperbad5_White" },       //edited due to glitch 1-4-23
            { 39, "GearHand_glove_NikeSuperbad5_TeamColor" },       //edited due to glitch 1-4-23
            { 40, "GearHand_glove_NikeSuperbad5_SecondaryColor" },      //edited due to glitch 1-4-23 
            { 41, "GearHand_glove_NikeVaporKnit2_Black" },
            { 42, "GearHand_glove_NikeVaporKnit2_White" },
            { 43, "GearHand_glove_NikeVaporKnit2_TeamColor" },
            { 44, "GearHand_glove_NikeVaporKnit2_SecondaryColor" },
            { 45, "GearHand_glove_GenericCutter_Black" },
            { 46, "GearHand_glove_GenericCutter_White" },
            { 47, "GearHand_glove_GenericCutter_TeamColor" },
            { 48, "GearHand_glove_GenericCutter_SecondaryColor" },
            { 49, "GearHand_glove_UnderArmourSpotlight2019_White" },
            { 50, "GearHand_glove_UnderArmourSpotlight2019_Black" },
            { 51, "GearHand_glove_UnderArmourF6_White" },
            { 52, "GearHand_glove_UnderArmourF6_Black" },
            { 53, "GearHand_glove_JordanVaporJet5_Black" },
            { 54, "GearHand_glove_JordanVaporJet5_TeamColor" },
            { 55, "GearHand_glove_JordanVaporJet5_SecondaryColor" },
            { 56, "GearHand_glove_JordanVaporJet5_White" },
            { 57, "GearHand_NoneHandAmputee_teamColor" },
            { 58, "GearHand_glove_NikeVaporJet6_Black" },
            { 59, "GearHand_glove_NikeVaporJet6_TeamColor" },
            { 60, "GearHand_glove_NikeVaporJet6_SecondaryColor" },
            { 61, "GearHand_glove_NikeVaporJet6_White" },
            { 62, "GearHand_glove_NikeVaporKnit3_TeamColor" },
            { 63, "GearHand_glove_NikeVaporKnit3_SecondaryColor" },
            { 64, "GearHand_glove_NikeVaporKnit3_Black" },
            { 65, "GearHand_glove_NikeVaporKnit3_White" },
            { 68, "GearHand_glove_NikeVaporJet6_TeamColor" },
            { 69, "GearHand_glove_NikeVaporJet6_SecondaryColor" },
            { 70, "GearHand_glove_NikeVaporJet6_White" },
            { 71, "GearHand_glove_NikeVaporJet6_Black" },
            { 72, "GearHand_glove_AdidasFreak_TeamColor" },
            { 73, "GearHand_glove_AdidasFreak_SecondaryColor" },
            { 74, "GearHand_glove_AdidasFreak_Black" },
            { 75, "GearHand_glove_AdidasFreak_White" },
            { 76, "GearHand_glove_NikeVaporJet7_White" },
            { 77, "GearHand_glove_NikeVaporJet7_Black" },
            { 78, "GearHand_glove_NikeVaporJet7_TeamColor" },
            { 79, "GearHand_glove_NikeVaporJet7_SecondaryColor" },
            { 80, "GearHand_glove_NikeSuperbad6_White" },
            { 81, "GearHand_glove_NikeSuperbad6_Black" },
            { 82, "GearHand_glove_NikeSuperbad6_TeamColor" },
            { 83, "GearHand_glove_NikeSuperbad6_SecondaryColor" },
            { 84, "GearHand_glove_JordanSuperbad6_White" },
            { 85, "GearHand_glove_JordanSuperbad6_Black" },
            { 86, "GearHand_glove_JordanSuperbad6_TeamColor" },
            { 87, "GearHand_glove_JordanSuperbad6_SecondaryColor" },
            { 88, "GearHand_glove_JordanVaporJet7_White" },
            { 89, "GearHand_glove_JordanVaporJet7_Black" },
            { 90, "GearHand_glove_JordanVaporJet7_TeamColor" },
            { 91, "GearHand_glove_JordanVaporJet7_SecondaryColor" },
            { 92, "GearHand_NoneHandAmputee_teamColor" },
};
            Dictionary<int, string> RightHandStyleMapping = new Dictionary<int, string>
{
            { 0, "GearHand_None" },
            { 1, "GearHand_glove_NikeVaporJet_White" },
            { 2, "GearHand_glove_NikeVaporJet_Black" },
            { 3, "GearHand_glove_NikeVaporJet_TeamColor" },
            { 4, "GearHand_glove_Adizero11_White" },
            { 5, "GearHand_glove_Adizero11_Black" },
            { 6, "GearHand_glove_Adizero11_TeamColor" },
            { 7, "GearHand_glove_NikeSuperBad3_White" },
            { 8, "GearHand_glove_NikeSuperBad3_Black" },
            { 9, "GearHand_glove_NikeSuperBad3_TeamColor" },
            { 10, "GearHand_glove_NikeVaporKnit_White" },
            { 11, "GearHand_glove_NikeVaporKnit_Black" },
            { 12, "GearHand_glove_NikeVaporKnit_TeamColor" },
            { 13, "GearHand_glove_NikeHyperBeast_White" },
            { 14, "GearHand_glove_NikeHyperBeast_Black" },
            { 15, "GearHand_tapedHandFinger_White" },
            { 16, "GearHand_tapedHandFinger_Black" },
            { 17, "GearHand_tapedHandFinger_TeamColor" },
            { 18, "GearHand_tapedHandNormal_White" },
            { 19, "GearHand_tapedHandMax_White" },
            { 20, "GearHand_tapedHandCombo_White" },
            { 21, "GearHand_glove_UnderArmourSpotlight2019_TeamColor" },
            { 22, "GearHand_glove_UnderArmourF6_TeamColor" },
            { 23, "GearHand_glove_NikeVaporJet_SecondaryColor" },
            { 24, "GearHand_glove_Adizero11_SecondaryColor" },
            { 25, "GearHand_glove_NikeSuperBad3_SecondaryColor" },
            { 26, "GearHand_glove_NikeVaporKnit_SecondaryColor" },
            { 27, "GearHand_glove_UnderArmourSpotlight2019_SecondaryColor" },
            { 28, "GearHand_glove_UnderArmourF6_SecondaryColor" },
            { 29, "GearHand_glove_NikeVaporJet4_Black" },
            { 30, "GearHand_glove_NikeVaporJet4_White" },
            { 31, "GearHand_glove_NikeVaporJet4_TeamColor" },
            { 32, "GearHand_glove_NikeVaporJet4_SecondaryColor" },
            { 33, "GearHand_glove_NikeVaporJet5_Black" },
            { 34, "GearHand_glove_NikeVaporJet5_White" },
            { 35, "GearHand_glove_NikeVaporJet5_TeamColor" },
            { 36, "GearHand_glove_NikeVaporJet5_SecondaryColor" },
            { 37, "GearHand_glove_NikeSuperbad5_Black" },       //edited due to glitch 1-4-23
            { 38, "GearHand_glove_NikeSuperbad5_White" },       //edited due to glitch 1-4-23
            { 39, "GearHand_glove_NikeSuperbad5_TeamColor" },       //edited due to glitch 1-4-23
            { 40, "GearHand_glove_NikeSuperbad5_SecondaryColor" },      //edited due to glitch 1-4-23
            { 41, "GearHand_glove_NikeVaporKnit2_Black" },
            { 42, "GearHand_glove_NikeVaporKnit2_White" },
            { 43, "GearHand_glove_NikeVaporKnit2_TeamColor" },
            { 44, "GearHand_glove_NikeVaporKnit2_SecondaryColor" },
            { 45, "GearHand_glove_GenericCutter_Black" },
            { 46, "GearHand_glove_GenericCutter_White" },
            { 47, "GearHand_glove_GenericCutter_TeamColor" },
            { 48, "GearHand_glove_GenericCutter_SecondaryColor" },
            { 49, "GearHand_glove_UnderArmourSpotlight2019_White" },
            { 50, "GearHand_glove_UnderArmourSpotlight2019_Black" },
            { 51, "GearHand_glove_UnderArmourF6_White" },
            { 52, "GearHand_glove_UnderArmourF6_Black" },
            { 53, "GearHand_glove_JordanVaporJet5_Black" },
            { 54, "GearHand_glove_JordanVaporJet5_TeamColor" },
            { 55, "GearHand_glove_JordanVaporJet5_SecondaryColor" },
            { 56, "GearHand_glove_JordanVaporJet5_White" },
            { 57, "GearHand_NoneHandAmputee_teamColor" },
            { 58, "GearHand_glove_NikeVaporJet6_Black" },
            { 59, "GearHand_glove_NikeVaporJet6_TeamColor" },
            { 60, "GearHand_glove_NikeVaporJet6_SecondaryColor" },
            { 61, "GearHand_glove_NikeVaporJet6_White" },
            { 62, "GearHand_glove_NikeVaporKnit3_TeamColor" },
            { 63, "GearHand_glove_NikeVaporKnit3_SecondaryColor" },
            { 64, "GearHand_glove_NikeVaporKnit3_Black" },
            { 65, "GearHand_glove_NikeVaporKnit3_White" },
            { 68, "GearHand_glove_NikeVaporJet6_TeamColor" },
            { 69, "GearHand_glove_NikeVaporJet6_SecondaryColor" },
            { 70, "GearHand_glove_NikeVaporJet6_White" },
            { 71, "GearHand_glove_NikeVaporJet6_Black" },
            { 72, "GearHand_glove_AdidasFreak_TeamColor" },
            { 73, "GearHand_glove_AdidasFreak_SecondaryColor" },
            { 74, "GearHand_glove_AdidasFreak_Black" },
            { 75, "GearHand_glove_AdidasFreak_White" },
            { 76, "GearHand_glove_NikeVaporJet7_White" },
            { 77, "GearHand_glove_NikeVaporJet7_Black" },
            { 78, "GearHand_glove_NikeVaporJet7_TeamColor" },
            { 79, "GearHand_glove_NikeVaporJet7_SecondaryColor" },
            { 80, "GearHand_glove_NikeSuperbad6_White" },
            { 81, "GearHand_glove_NikeSuperbad6_Black" },
            { 82, "GearHand_glove_NikeSuperbad6_TeamColor" },
            { 83, "GearHand_glove_NikeSuperbad6_SecondaryColor" },
            { 84, "GearHand_glove_JordanSuperbad6_White" },
            { 85, "GearHand_glove_JordanSuperbad6_Black" },
            { 86, "GearHand_glove_JordanSuperbad6_TeamColor" },
            { 87, "GearHand_glove_JordanSuperbad6_SecondaryColor" },
            { 88, "GearHand_glove_JordanVaporJet7_White" },
            { 89, "GearHand_glove_JordanVaporJet7_Black" },
            { 90, "GearHand_glove_JordanVaporJet7_TeamColor" },
            { 91, "GearHand_glove_JordanVaporJet7_SecondaryColor" },
            { 92, "GearHand_NoneHandAmputee_teamColor" },
};
            Dictionary<int, string> LeftFootwearStyleMapping = new Dictionary<int, string>
{
            { 0, "GearFootwear_shoe_mid_AdidasFreak" },
            { 1, "GearFootwear_shoe_mid_AdidasAdizeroPrimeKnit" },
            { 2, "GearFootwear_shoe_Mid_NikeAlphaPro34TD" },
            { 3, "GearFootwear_shoe_Low_NikeVaporUntouchable" },
            { 4, "GearFootwear_shoe_Low_NikeVaporCarbonEliteTD" },
            { 5, "GearFootwear_shoe_Low_UnderArmourSpotlight2018" },
            { 6, "GearFootwear_shoe_Low_NikeAlphaMenaceElite" },
            { 7, "GearFootwear_shoe_Low_NikeVaporUntouchable2" },
            { 8, "GearFootwear_shoeLowVintage_nike" },
            { 9, "GearFootwear_shoeLowVintage_pony" },
            { 10, "GearFootwear_shoe_Mid_NikeLunarBeast" },
            { 11, "GearFootwear_shoe_Mid_NikeCodeEliteProShark" },
            { 12, "GearFootwear_shoe_high_UnderArmourHighlight2019" },
            { 13, "GearFootwear_shoe_mid_UnderArmourC1N2019" },
            { 14, "GearFootwear_shoe_mid_AirJordanRetro199" },
            { 15, "GearFootwear_shoe_High_NikeForceSavageEliteTDW" },
            { 16, "GearFootwear_shoe_mid_NikeAlphaMenacePro" },
            { 17, "GearFootwear_shoe_mid_NikeForceSavagePro" },
            { 18, "GearFootwear_shoe_low_NikeVaporSpeed3" },
            { 19, "GearFootwear_shoe_low_NikeVaporUntouchablePro3" },
            { 20, "GearFootwear_shoe_mid_NikeFieldGeneral" },
            { 21, "GearFootwear_shoe_low_NikeVaporUntouchablePro" },
            { 22, "GearFootwear_shoe_high_NikeForceSavageElite99Club" },
            { 23, "GearFootwear_shoe_low_Jordan5_99Club" },
            { 24, "GearFootwear_shoe_mid_NikeAlphaMenacePro2" },
            { 25, "GearFootwear_shoe_mid_NikeForceSavagePro2" },
            { 26, "GearFootwear_shoe_high_AdidasFreakUltra22" },
            { 27, "GearFootwear_shoe_mid_AirJordanRetro1" },
            { 28, "GearFootwear_shoe_mid_UnderArmourTB12" },
            { 29, "GearFootwear_shoe_high_NikeForceSavageElite2" },
            { 30, "GearFootwear_shoe_low_NikeVaporEdge" },
            { 31, "GearFootwear_shoe_high_NikeAlphaMenaceElite2" },
            { 32, "GearFootwear_shoe_high_NikeVaporUntouchablePro3" },
            { 33, "GearFootwear_shoe_low_AirJordanXI" },
            { 34, "GearFootwear_shoe_mid_AirJordanX" },
            { 35, "GearFootwear_shoe_mid_AdidasNasty" },
            { 36, "GearFootwear_shoe_low_Adidas_AdizeroElectric" },
            { 37, "GearFootwear_shoe_low_Adidas_AdizeroElectricPlus" },
            { 38, "GearFootwear_shoe_low_AdidasAdizone11Turbo" },
            { 39, "GearFootwear_shoe_mid_NikeAlphaMenacePro299Club" },
            { 40, "GearFootwear_shoe_low_Jordan7" },
            { 41, "GearFootwear_shoe_mid_Jordan5" },
            { 42, "GearFootwear_shoe_low_NikeVaporEdge_zebra" },
            { 43, "GearFootwear_shoe_low_NikeAlphaMenaceElite3" },
            { 44, "GearFootwear_shoe_mid_NikeAlphaMenacePro3WDP" },
            { 46, "GearFootwear_shoe_mid_Jordan7" },
            { 47, "GearFootwear_shoe_low_AirJordanRetro1" },
            { 48, "GearFootwear_shoe_low_AdidasAdizero_LTA58" },
            { 49, "GearFootwear_shoe_mid_NikeVaporEdgeDunk" },
            { 50, "GearFootwear_shoe_low_NikeEquinox" },
            { 51, "GearFootwear_shoe_low_AdidasFreak22" },
            { 52, "GearFootwear_shoe_mid_AdidasFreakUltraCleat" },
            { 53, "GearFootwear_shoe_low_NikeAirMax90PlayLikeMad" }
};
            Dictionary<int, string> RightFootwearStyleMapping = new Dictionary<int, string>
{
            { 0, "GearFootwear_shoe_mid_AdidasFreak" },
            { 1, "GearFootwear_shoe_mid_AdidasAdizeroPrimeKnit" },
            { 2, "GearFootwear_shoe_Mid_NikeAlphaPro34TD" },
            { 3, "GearFootwear_shoe_Low_NikeVaporUntouchable" },
            { 4, "GearFootwear_shoe_Low_NikeVaporCarbonEliteTD" },
            { 5, "GearFootwear_shoe_Low_UnderArmourSpotlight2018" },
            { 6, "GearFootwear_shoe_Low_NikeAlphaMenaceElite" },
            { 7, "GearFootwear_shoe_Low_NikeVaporUntouchable2" },
            { 8, "GearFootwear_shoeLowVintage_nike" },
            { 9, "GearFootwear_shoeLowVintage_pony" },
            { 10, "GearFootwear_shoe_Mid_NikeLunarBeast" },
            { 11, "GearFootwear_shoe_Mid_NikeCodeEliteProShark" },
            { 12, "GearFootwear_shoe_high_UnderArmourHighlight2019" },
            { 13, "GearFootwear_shoe_mid_UnderArmourC1N2019" },
            { 14, "GearFootwear_shoe_mid_AirJordanRetro199" },
            { 15, "GearFootwear_shoe_High_NikeForceSavageEliteTDW" },
            { 16, "GearFootwear_shoe_mid_NikeAlphaMenacePro" },
            { 17, "GearFootwear_shoe_mid_NikeForceSavagePro" },
            { 18, "GearFootwear_shoe_low_NikeVaporSpeed3" },
            { 19, "GearFootwear_shoe_low_NikeVaporUntouchablePro3" },
            { 20, "GearFootwear_shoe_mid_NikeFieldGeneral" },
            { 21, "GearFootwear_shoe_low_NikeVaporUntouchablePro" },
            { 22, "GearFootwear_shoe_high_NikeForceSavageElite99Club" },
            { 23, "GearFootwear_shoe_low_Jordan5_99Club" },
            { 24, "GearFootwear_shoe_mid_NikeAlphaMenacePro2" },
            { 25, "GearFootwear_shoe_mid_NikeForceSavagePro2" },
            { 26, "GearFootwear_shoe_high_AdidasFreakUltra22" },
            { 27, "GearFootwear_shoe_mid_AirJordanRetro1" },
            { 28, "GearFootwear_shoe_mid_UnderArmourTB12" },
            { 29, "GearFootwear_shoe_high_NikeForceSavageElite2" },
            { 30, "GearFootwear_shoe_low_NikeVaporEdge" },
            { 31, "GearFootwear_shoe_high_NikeAlphaMenaceElite2" },
            { 32, "GearFootwear_shoe_high_NikeVaporUntouchablePro3" },
            { 33, "GearFootwear_shoe_low_AirJordanXI" },
            { 34, "GearFootwear_shoe_mid_AirJordanX" },
            { 35, "GearFootwear_shoe_mid_AdidasNasty" },
            { 36, "GearFootwear_shoe_low_Adidas_AdizeroElectric" },
            { 37, "GearFootwear_shoe_low_Adidas_AdizeroElectricPlus" },
            { 38, "GearFootwear_shoe_low_AdidasAdizone11Turbo" },
            { 39, "GearFootwear_shoe_mid_NikeAlphaMenacePro299Club" },
            { 40, "GearFootwear_shoe_low_Jordan7" },
            { 41, "GearFootwear_shoe_mid_Jordan5" },
            { 42, "GearFootwear_shoe_low_NikeVaporEdge_zebra" },
            { 43, "GearFootwear_shoe_low_NikeAlphaMenaceElite3" },
            { 44, "GearFootwear_shoe_mid_NikeAlphaMenacePro3WDP" },
            { 46, "GearFootwear_shoe_mid_Jordan7" },
            { 47, "GearFootwear_shoe_low_AirJordanRetro1" },
            { 48, "GearFootwear_shoe_low_AdidasAdizero_LTA58" },
            { 49, "GearFootwear_shoe_mid_NikeVaporEdgeDunk" },
            { 50, "GearFootwear_shoe_low_NikeEquinox" },
            { 51, "GearFootwear_shoe_low_AdidasFreak22" },
            { 52, "GearFootwear_shoe_mid_AdidasFreakUltraCleat" },
            { 53, "GearFootwear_shoe_low_NikeAirMax90PlayLikeMad" }
};

            Dictionary<int, string> GearLeftSpatsStyleMapping = new Dictionary<int, string>
{
            { 0, "GearSpats_none" },
            { 1, "GearSpats_spatThin_White" },
            { 2, "GearSpats_spatBulky_White" },
            { 3, "GearSpats_spatThin_Black" },
            { 4, "GearSpats_spatBulky_Black" },
            { 5, "GearSpats_spatThin_TeamColor" },
            { 6, "GearSpats_spatBulky_TeamColor" },
            { 7, "GearSpats_spatThin_SecondaryColor" },
            { 8, "GearSpats_spatBulky_SecondaryColor" },
};



            Dictionary<int, string> GearRightSpatsStyleMapping = new Dictionary<int, string>
{
            { 0, "GearSpats_none" },
            { 1, "GearSpats_spatThin_White" },
            { 2, "GearSpats_spatBulky_White" },
            { 3, "GearSpats_spatThin_Black" },
            { 4, "GearSpats_spatBulky_Black" },
            { 5, "GearSpats_spatThin_TeamColor" },
            { 6, "GearSpats_spatBulky_TeamColor" },
            { 7, "GearSpats_spatThin_SecondaryColor" },
            { 8, "GearSpats_spatBulky_SecondaryColor" },
};
            Dictionary<int, int> SkinToneMapping = new Dictionary<int, int>
{
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
            { 6, 0 },
            { 7, 0 },
            { 8, 0 },
            { 9, 0 },
            { 10, 0 },
            { 11, 0 },
            { 12, 0 },
            { 13, 0 },
            { 14, 0 },
            { 15, 0 },
            { 16, 0 },
            { 17, 0 },
            { 18, 0 },
            { 19, 0 },
            { 20, 0 },
            { 21, 0 },
            { 22, 0 },
            { 400, 0 },
            { 450, 0 },
            { 23, 1 },
            { 24, 1 },
            { 25, 1 },
            { 26, 1 },
            { 27, 1 },
            { 28, 1 },
            { 29, 1 },
            { 30, 1 },
            { 31, 1 },
            { 32, 1 },
            { 33, 1 },
            { 34, 1 },
            { 35, 1 },
            { 36, 1 },
            { 37, 1 },
            { 38, 1 },
            { 39, 1 },
            { 40, 1 },
            { 41, 1 },
            { 42, 1 },
            { 43, 1 },
            { 44, 1 },
            { 45, 1 },
            { 46, 1 },
            { 47, 1 },
            { 48, 1 },
            { 49, 1 },
            { 50, 1 },
            { 51, 1 },
            { 52, 1 },
            { 53, 1 },
            { 54, 1 },
            { 55, 1 },
            { 56, 1 },
            { 57, 1 },
            { 58, 1 },
            { 59, 1 },
            { 60, 1 },
            { 61, 1 },
            { 62, 1 },
            { 63, 1 },
            { 64, 1 },
            { 65, 1 },
            { 66, 1 },
            { 67, 1 },
            { 68, 1 },
            { 69, 1 },
            { 70, 1 },
            { 71, 1 },
            { 401, 1 },
            { 72, 1 },
            { 73, 1 },
            { 74, 1 },
            { 75, 1 },
            { 76, 2 },
            { 77, 2 },
            { 78, 2 },
            { 79, 2 },
            { 80, 2 },
            { 81, 2 },
            { 82, 2 },
            { 83, 2 },
            { 84, 2 },
            { 85, 2 },
            { 86, 2 },
            { 87, 2 },
            { 88, 2 },
            { 89, 2 },
            { 90, 2 },
            { 91, 2 },
            { 92, 2 },
            { 93, 2 },
            { 94, 2 },
            { 95, 2 },
            { 402, 2 },
            { 96, 2 },
            { 97, 2 },
            { 98, 3 },
            { 99, 3 },
            { 100, 3 },
            { 101, 3 },
            { 102, 3 },
            { 103, 3 },
            { 104, 3 },
            { 105, 3 },
            { 106, 3 },
            { 107, 3 },
            { 108, 3 },
            { 109, 3 },
            { 110, 3 },
            { 111, 3 },
            { 112, 3 },
            { 113, 3 },
            { 114, 3 },
            { 115, 3 },
            { 453, 3 },
            { 116, 3 },
            { 117, 3 },
            { 118, 3 },
            { 119, 3 },
            { 120, 3 },
            { 451, 3 },
            { 452, 3 },
            { 121, 3 },
            { 403, 3 },
            { 122, 3 },
            { 123, 3 },
            { 124, 3 },
            { 125, 3 },
            { 126, 3 },
            { 127, 4 },
            { 128, 4 },
            { 129, 4 },
            { 130, 4 },
            { 131, 4 },
            { 132, 4 },
            { 133, 4 },
            { 134, 4 },
            { 135, 4 },
            { 136, 4 },
            { 137, 4 },
            { 138, 4 },
            { 139, 4 },
            { 140, 4 },
            { 141, 4 },
            { 142, 4 },
            { 143, 4 },
            { 144, 4 },
            { 145, 4 },
            { 454, 4 },
            { 146, 4 },
            { 147, 4 },
            { 148, 4 },
            { 149, 4 },
            { 150, 4 },
            { 151, 4 },
            { 152, 4 },
            { 404, 4 },
            { 153, 4 },
            { 154, 4 },
            { 155, 5 },
            { 156, 5 },
            { 157, 5 },
            { 158, 5 },
            { 159, 5 },
            { 160, 5 },
            { 161, 5 },
            { 162, 5 },
            { 163, 5 },
            { 164, 5 },
            { 165, 5 },
            { 166, 5 },
            { 167, 5 },
            { 168, 5 },
            { 169, 5 },
            { 170, 5 },
            { 171, 5 },
            { 172, 5 },
            { 173, 5 },
            { 174, 5 },
            { 175, 5 },
            { 176, 5 },
            { 177, 5 },
            { 178, 5 },
            { 179, 5 },
            { 180, 5 },
            { 181, 5 },
            { 182, 5 },
            { 183, 5 },
            { 184, 5 },
            { 185, 5 },
            { 186, 5 },
            { 187, 5 },
            { 188, 5 },
            { 189, 5 },
            { 190, 5 },
            { 191, 5 },
            { 192, 5 },
            { 193, 5 },
            { 194, 5 },
            { 195, 5 },
            { 196, 5 },
            { 197, 5 },
            { 198, 5 },
            { 199, 5 },
            { 405, 5 },
            { 200, 5 },
            { 201, 5 },
            { 202, 5 },
            { 203, 5 },
            { 204, 6 },
            { 205, 6 },
            { 206, 6 },
            { 207, 6 },
            { 208, 6 },
            { 209, 6 },
            { 210, 6 },
            { 211, 6 },
            { 212, 6 },
            { 213, 6 },
            { 214, 6 },
            { 215, 6 },
            { 216, 6 },
            { 217, 6 },
            { 218, 6 },
            { 219, 6 },
            { 220, 6 },
            { 221, 6 },
            { 222, 6 },
            { 223, 6 },
            { 224, 6 },
            { 225, 6 },
            { 226, 6 },
            { 227, 6 },
            { 228, 6 },
            { 229, 6 },
            { 230, 6 },
            { 231, 6 },
            { 232, 6 },
            { 233, 6 },
            { 234, 6 },
            { 235, 6 },
            { 236, 6 },
            { 237, 6 },
            { 238, 6 },
            { 239, 6 },
            { 240, 6 },
            { 241, 6 },
            { 242, 6 },
            { 243, 6 },
            { 244, 6 },
            { 245, 6 },
            { 246, 6 },
            { 247, 6 },
            { 248, 6 },
            { 249, 6 },
            { 250, 6 },
            { 251, 6 },
            { 252, 6 },
            { 253, 6 },
            { 254, 6 },
            { 255, 6 },
            { 256, 6 },
            { 257, 6 },
            { 258, 6 },
            { 259, 6 },
            { 260, 6 },
            { 261, 6 },
            { 262, 6 },
            { 263, 6 },
            { 264, 6 },
            { 265, 6 },
            { 266, 6 },
            { 267, 6 },
            { 268, 6 },
            { 269, 6 },
            { 270, 6 },
            { 271, 6 },
            { 272, 6 },
            { 273, 6 },
            { 274, 6 },
            { 275, 6 },
            { 276, 6 },
            { 277, 6 },
            { 278, 6 },
            { 279, 6 },
            { 280, 6 },
            { 281, 6 },
            { 282, 6 },
            { 283, 6 },
            { 284, 6 },
            { 455, 6 },
            { 406, 6 },
            { 285, 6 },
            { 286, 6 },
            { 287, 6 },
            { 288, 6 },
            { 289, 6 },
            { 290, 6 },
            { 407, 6 }
};
            Dictionary<int, string> GearLeftWristStyleMapping = new Dictionary<int, string>
{
            { 0, "GearWrist_none" },
            { 1, "GearWrist_wristBandNormal_White" },
            { 2, "GearWrist_wristBandNormal_Black" },
            { 3, "GearWrist_wristBandNormal_TeamColor" },
            { 4, "GearWrist_wristBandDouble_White" },
            { 5, "GearWrist_wristBandDouble_Black" },
            { 6, "GearWrist_wristBandDouble_TeamColor" },
            { 7, "GearWrist_wristBandCoach_White" },
            { 8, "GearWrist_wristBandCoach_Black" },
            { 9, "GearWrist_wristBandCoach_TeamColor" },
            { 10, "GearWrist_wristTapedLite_White" },
            { 11, "GearWrist_wristTapedNormal_White" },
            { 12, "GearWrist_wristTapedMax_White" },
            { 13, "GearWrist_gloveTapedLarge_White" },
            { 14, "GearWrist_gloveTapedNormal_White" },
            { 15, "GearWrist_gloveTapedLarge_Black" },
            { 16, "GearWrist_gloveTapedNormal_Black" },
            { 17, "GearWrist_gloveWristBrace_Black" },
            { 18, "GearWrist_wristBandNormal_SecondaryColor" },
            { 19, "GearWrist_wristBandDouble_SecondaryColor" },
            { 20, "GearWrist_wristTapedLite_Black" },
            { 21, "GearWrist_wristTapedNormal_Black" },
            { 22, "GearWrist_wristTapedMax_Black" },
            { 23, "GearWrist_wristTapedLite_TeamColor" },
            { 24, "GearWrist_wristTapedNormal_TeamColor" },
            { 25, "GearWrist_wristTapedMax_TeamColor" },
            { 26, "GearWrist_wristTapedLite_SecondaryColor" },
            { 27, "GearWrist_wristTapedNormal_SecondaryColor" },
            { 28, "GearWrist_wristTapedMax_SecondaryColor" }
};
            Dictionary<int, string> GearRightWristStyleMapping = new Dictionary<int, string>
{
            { 0, "GearWrist_None" },
            { 1, "GearWrist_wristBandNormal_White" },
            { 2, "GearWrist_wristBandNormal_Black" },
            { 3, "GearWrist_wristBandNormal_TeamColor" },
            { 4, "GearWrist_wristBandDouble_White" },
            { 5, "GearWrist_wristBandDouble_Black" },
            { 6, "GearWrist_wristBandDouble_TeamColor" },
            { 7, "GearWrist_wristBandCoach_White" },
            { 8, "GearWrist_wristBandCoach_Black" },
            { 9, "GearWrist_wristBandCoach_TeamColor" },
            { 10, "GearWrist_wristTapedLite_White" },
            { 11, "GearWrist_wristTapedNormal_White" },
            { 12, "GearWrist_wristTapedMax_White" },
            { 13, "GearWrist_gloveTapedLarge_White" },
            { 14, "GearWrist_gloveTapedNormal_White" },
            { 15, "GearWrist_gloveTapedLarge_Black" },
            { 16, "GearWrist_gloveTapedNormal_Black" },
            { 17, "GearWrist_gloveWristBrace_Black" },
            { 18, "GearWrist_wristBandNormal_SecondaryColor" },
            { 19, "GearWrist_wristBandDouble_SecondaryColor" },
            { 20, "GearWrist_wristTapedLite_Black" },
            { 21, "GearWrist_wristTapedNormal_Black" },
            { 22, "GearWrist_wristTapedMax_Black" },
            { 23, "GearWrist_wristTapedLite_TeamColor" },
            { 24, "GearWrist_wristTapedNormal_TeamColor" },
            { 25, "GearWrist_wristTapedMax_TeamColor" },
            { 26, "GearWrist_wristTapedLite_SecondaryColor" },
            { 27, "GearWrist_wristTapedNormal_SecondaryColor" },
            { 28, "GearWrist_wristTapedMax_SecondaryColor" }
};
            Dictionary<int, string> GearThighStyleMapping = new Dictionary<int, string>
{
            { 0, "ThighPad_Regular" },
            { 1, "ThighPad_Nike" }
};

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = fileDialog.OpenFile()) != null)
                    {
                        TableModel playerTableModel = model.TableModels[EditorModel.PLAYER_TABLE];
                        List<TdbFieldProperties> props = playerTableModel.GetFieldList();
                        StringBuilder hbuilder = new StringBuilder();
                        StreamWriter writer = new StreamWriter(myStream);

                        // Customize the header
                        hbuilder.Append("Player_ID,assetName,genericHeadName,jerseyNumber,lastName,containerId,genericHead,heightInches,loadouts_0_loadoutType,loadouts_0_loadoutCategory,loadouts_0_loadoutElements_0_slotType,loadouts_0_loadoutElements_0_blends_0_barycentricBlend,loadouts_0_loadoutElements_0_blends_0_baseBlend,loadouts_0_loadoutElements_1_slotType,loadouts_0_loadoutElements_1_blends_0_barycentricBlend,loadouts_0_loadoutElements_1_blends_0_baseBlend,loadouts_0_loadoutElements_2_slotType," +
                            "loadouts_0_loadoutElements_2_blends_0_barycentricBlend,loadouts_0_loadoutElements_2_blends_0_baseBlend,loadouts_0_loadoutElements_3_slotType,loadouts_0_loadoutElements_3_blends_0_barycentricBlend,loadouts_0_loadoutElements_3_blends_0_baseBlend,loadouts_0_loadoutElements_4_slotType,loadouts_0_loadoutElements_4_blends_0_barycentricBlend,loadouts_0_loadoutElements_4_blends_0_baseBlend,loadouts_0_loadoutElements_5_slotType,loadouts_0_loadoutElements_5_blends_0_barycentricBlend,loadouts_0_loadoutElements_5_blends_0_baseBlend," +
                            "loadouts_0_loadoutElements_6_slotType,loadouts_0_loadoutElements_6_blends_0_barycentricBlend,loadouts_0_loadoutElements_6_blends_0_baseBlend,loadouts_1_loadoutType,loadouts_1_loadoutCategory,loadouts_1_loadoutElements_0_slotType,loadouts_1_loadoutElements_0_itemAssetName,loadouts_1_loadoutElements_1_slotType,loadouts_1_loadoutElements_1_itemAssetName,loadouts_1_loadoutElements_2_slotType,loadouts_1_loadoutElements_2_itemAssetName,loadouts_1_loadoutElements_3_slotType,loadouts_1_loadoutElements_3_itemAssetName,loadouts_1_loadoutElements_4_slotType,loadouts_1_loadoutElements_4_itemAssetName,loadouts_1_loadoutElements_5_slotType," +
                            "loadouts_1_loadoutElements_5_itemAssetName,loadouts_1_loadoutElements_6_slotType,loadouts_1_loadoutElements_6_itemAssetName,loadouts_1_loadoutElements_7_slotType,loadouts_1_loadoutElements_7_itemAssetName,loadouts_1_loadoutElements_8_slotType,loadouts_1_loadoutElements_8_itemAssetName,loadouts_1_loadoutElements_9_slotType,loadouts_1_loadoutElements_9_itemAssetName,loadouts_1_loadoutElements_10_slotType,loadouts_1_loadoutElements_10_itemAssetName,loadouts_1_loadoutElements_11_slotType,loadouts_1_loadoutElements_11_itemAssetName,loadouts_1_loadoutElements_12_slotType,loadouts_1_loadoutElements_12_itemAssetName," +
                            "loadouts_1_loadoutElements_13_slotType,loadouts_1_loadoutElements_13_itemAssetName,loadouts_1_loadoutElements_14_slotType,loadouts_1_loadoutElements_14_itemAssetName,loadouts_1_loadoutElements_15_slotType,loadouts_1_loadoutElements_15_itemAssetName,loadouts_1_loadoutElements_16_slotType,loadouts_1_loadoutElements_16_itemAssetName,loadouts_1_loadoutElements_17_slotType,loadouts_1_loadoutElements_17_itemAssetName,loadouts_1_loadoutElements_18_slotType,loadouts_1_loadoutElements_18_itemAssetName,loadouts_1_loadoutElements_19_slotType,loadouts_1_loadoutElements_19_itemAssetName,loadouts_1_loadoutElements_20_slotType,loadouts_1_loadoutElements_20_itemAssetName," +
                            "loadouts_1_loadoutElements_21_slotType,loadouts_1_loadoutElements_21_itemAssetName,loadouts_1_loadoutElements_22_slotType,loadouts_1_loadoutElements_22_itemAssetName,skinTone,skinToneScale,weightPounds,loadouts_1_loadoutElements_23_slotType,loadouts_1_loadoutElements_23_itemAssetName,loadouts_1_loadoutElements_24_slotType,loadouts_1_loadoutElements_24_itemAssetName,loadouts_1_loadoutElements_25_slotType,loadouts_1_loadoutElements_25_itemAssetName,loadouts_1_loadoutElements_26_slotType,loadouts_1_loadoutElements_26_itemAssetName," +
                            "loadouts_1_loadoutElements_27_slotType,loadouts_1_loadoutElements_27_itemAssetName,loadouts_1_loadoutElements_27_blends_0_barycentricBlend,loadouts_1_loadoutElements_27_blends_0_baseBlend,loadouts_1_loadoutElements_26_blends_0_barycentricBlend,loadouts_1_loadoutElements_26_blends_0_baseBlend,loadouts_1_loadoutElements_25_blends_0_barycentricBlend,loadouts_1_loadoutElements_25_blends_0_baseBlend,loadouts_1_loadoutElements_24_blends_0_barycentricBlend,loadouts_1_loadoutElements_24_blends_0_baseBlend,loadouts_1_loadoutElements_23_blends_0_barycentricBlend,loadouts_1_loadoutElements_23_blends_0_baseBlend,loadouts_1_loadoutElements_22_blends_0_barycentricBlend,loadouts_1_loadoutElements_22_blends_0_baseBlend,");
                        writer.WriteLine(hbuilder.ToString());
                        writer.Flush();

                        foreach (TableRecordModel record in playerTableModel.GetRecords())
                        {
                            // Clear the StringBuilder and build the data line with player data for each record
                            StringBuilder builder = new StringBuilder();
                            // Append the player ID to the data line
                            builder.Append(record.GetIntField("PGID")); // Assuming "PlayerID" is the actual field name
                            builder.Append(",");
                            // Player Assets
                            builder.Append(record.GetStringField("PEPS"));
                            builder.Append(",");
                            int numericGenericHead = record.GetIntField("PGHE");
                            if (genericHeadMapping.TryGetValue(numericGenericHead, out string stringGenericHead))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringGenericHead);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("gen_1_B_B_005");
                            }

                            builder.Append(",");
                            // JerseyNumbers
                            builder.Append(record.GetIntField("PJEN"));
                            builder.Append(",");
                            // Last Name
                            builder.Append(record.GetStringField("PLNA"));
                            builder.Append(",");
                            // Container ID which is PGID
                            builder.Append(record.GetIntField("PGID")); 
                            builder.Append(",");
                            // Face ID
                            builder.Append(record.GetIntField("PGHE")); 
                            builder.Append(",");
                            // Height
                            builder.Append(record.GetIntField("PHGT")); 
                            builder.Append(",");
                            // loadouts_0_loadoutType
                            builder.Append("Base");
                            builder.Append(",");
                            // loadouts_0_loadoutCategory
                            builder.Append("Base");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_slotType
                            builder.Append("ArmSize");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend
                            builder.Append(record.GetFloatField("BSAT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend
                            builder.Append(record.GetFloatField("BSAA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("CalfBlend");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend
                            builder.Append(record.GetFloatField("BSCT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend
                            builder.Append(record.GetFloatField("BSCA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("Chest");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend (Pad Size)
                            builder.Append(record.GetFloatField("BSPT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend (Pad Height)
                            builder.Append(record.GetFloatField("BSPA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("Feet");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend (Pad Size)
                            builder.Append(record.GetFloatField("BSFT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend (Pad Height)
                            builder.Append(record.GetFloatField("BSFA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("Glute");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend (Pad Size)
                            builder.Append(record.GetFloatField("BSBT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend (Pad Height)
                            builder.Append(record.GetFloatField("BSBA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("Gut");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend (Pad Size)
                            builder.Append(record.GetFloatField("BSGT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend (Pad Height)
                            builder.Append(record.GetFloatField("BSGA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("Thighs");
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_barycentricBlend (Pad Size)
                            builder.Append(record.GetFloatField("BSTT"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend (Pad Height)
                            builder.Append(record.GetFloatField("BSTA"));
                            builder.Append(",");
                            // loadouts_0_loadoutElements_1_slotType
                            builder.Append("PlayerOnField");
                            builder.Append(",");
                            builder.Append("GearOnly");
                            builder.Append(",");
                            builder.Append("JerseyStyle");
                            builder.Append(",");
                            int numericJerseyStyle = record.GetIntField("PJER");
                            if (jerseyStyleMapping.TryGetValue(numericJerseyStyle, out string stringJerseyStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringJerseyStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("Gear_JerseyStyle_SleeveTight");
                            }
                            builder.Append(",");
                            builder.Append("Shoulderpads");
                            builder.Append(",");
                            builder.Append("Small_Pads");
                            builder.Append(",");
                            builder.Append("GearSocks");
                            builder.Append(",");
                            int numericSocksStyle = record.GetIntField("PSKH");
                            if (SocksStyleMapping.TryGetValue(numericSocksStyle, out string stringSocksStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringSocksStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("Gear_Socks_Mid");
                            }
                            builder.Append(",");
                            builder.Append("Handwarmer");
                            builder.Append(",");
                            int numericHandWarmersStyle = record.GetIntField("PLHW");
                            if (HandWarmersStyleMapping.TryGetValue(numericHandWarmersStyle, out string stringHandWarmersStyle))        //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringHandWarmersStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("Handwarmer_None");
                            }
                            builder.Append(",");
                            builder.Append("HandwarmerMod");
                            builder.Append(",");
                            int numericHandWarmersModStyle = record.GetIntField("PLHW");
                            if (HandWarmersModStyleMapping.TryGetValue(numericHandWarmersModStyle, out string stringHandWarmersModStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringHandWarmersModStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("HandwarmerStyle_None ");
                            }
                            builder.Append(",");
                            builder.Append("Mouthpiece");
                            builder.Append(",");
                            int numericMouthPieceStyle = record.GetIntField("PMPC");
                            if (MouthPieceStyleMapping.TryGetValue(numericMouthPieceStyle, out string stringMouthPieceStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringMouthPieceStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearMouthpiece_None ");
                            }
                            builder.Append(",");
                            builder.Append("Neckpad");
                            builder.Append(",");
                            int numericNeckPadStyle = record.GetIntField("PNEK");
                            if (NeckPadStyleMapping.TryGetValue(numericNeckPadStyle, out string stringNeckPadStyle))        //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringNeckPadStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearNeckPad_None ");
                            }
                            builder.Append(",");
                            builder.Append("Towel");
                            builder.Append(",");
                            int numericTowelPositionStyle = record.GetIntField("PLTL");
                            if (TowelPositionStyleMapping.TryGetValue(numericTowelPositionStyle, out string stringTowelPositionStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringTowelPositionStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("Towel_None");
                            }
                            builder.Append(",");
                            builder.Append("Undershirt");
                            builder.Append(",");
                            int numericUnderShirtStyle = record.GetIntField("PJST");

                            if (UnderShirtStyleMapping.TryGetValue(numericUnderShirtStyle, out string stringUnderShirtStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringUnderShirtStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("Undershirt_Tucked_in");
                            }

                            builder.Append(",");
                            builder.Append("Visor");
                            builder.Append(",");
                            int numericVisorStyle = record.GetIntField("PVIS");

                            if (VisorStyleMapping.TryGetValue(numericVisorStyle, out string stringVisorStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringVisorStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearVisor_None");
                            }

                            builder.Append(",");
                            builder.Append("FacePaint");
                            builder.Append(",");
                            int numericFacePaintStyle = record.GetIntField("PEYE");

                            if (FacePaintStyleMapping.TryGetValue(numericFacePaintStyle, out string stringFacePaintStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringFacePaintStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("FaceMarks_None");
                            }

                            builder.Append(",");
                            builder.Append("GearHelmet");
                            builder.Append(",");
                            int numericHelmetStyle = record.GetIntField("PHLM");

                            if (HelmetStyleMapping.TryGetValue(numericHelmetStyle, out string stringHelmetStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringHelmetStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearHelmet_Standard");
                            }

                            builder.Append(",");
                            builder.Append("Facemask");
                            builder.Append(",");
                            int numericFaceMaskStyle = record.GetIntField("PFMK");

                            if (FaceMaskStyleMapping.TryGetValue(numericFaceMaskStyle, out string stringFaceMaskStyle))     //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringFaceMaskStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearFaceMask_2Bar");
                            }
                            builder.Append(",");
                            builder.Append("KneeItem");
                            builder.Append(",");
                            int numericKneePadStyle = record.GetIntField("PLKN");

                            if (KneePadStyleMapping.TryGetValue(numericKneePadStyle, out string stringKneePadStyle))        //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringKneePadStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("KneePad_None");
                            }
                            builder.Append(",");
                            builder.Append("LeftArm");
                            builder.Append(",");
                            int numericLeftArmSleeveStyle = record.GetIntField("PGSL");

                            if (LeftArmSleeveStyleMapping.TryGetValue(numericLeftArmSleeveStyle, out string stringLeftArmSleeveStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringLeftArmSleeveStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearArmSleeve_None");
                            }
                            builder.Append(",");
                            builder.Append("RightArm");
                            builder.Append(",");
                            int numericRightArmSleeveStyle = record.GetIntField("PMOR");

                            if (RightArmSleeveStyleMapping.TryGetValue(numericRightArmSleeveStyle, out string stringRightArmSleeveStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringRightArmSleeveStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearArmSleeve_None");
                            }
                            builder.Append(",");
                            builder.Append("LeftElbowGear");
                            builder.Append(",");
                            int numericLeftElbowGearStyle = record.GetIntField("PLEL");

                            if (LeftElbowGearStyleMapping.TryGetValue(numericLeftElbowGearStyle, out string stringLeftElbowGearStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringLeftElbowGearStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("ElbowGear_None");
                            }
                            builder.Append(",");
                            builder.Append("RightElbowGear");
                            builder.Append(",");
                            int numericRightElbowGearStyle = record.GetIntField("PREL");

                            if (RightElbowGearStyleMapping.TryGetValue(numericRightElbowGearStyle, out string stringRightElbowGearStyle))       //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringRightElbowGearStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("ElbowGear_None");
                            }
                            builder.Append(",");
                            builder.Append("LeftHandgear");
                            builder.Append(",");
                            int numericLeftHandStyle = record.GetIntField("PLHA");

                            if (LeftHandStyleMapping.TryGetValue(numericLeftHandStyle, out string stringLeftHandStyle))     //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringLeftHandStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearHand_None");
                            }
                            builder.Append(",");
                            builder.Append("RightHandgear");
                            builder.Append(",");
                            int numericRightHandStyle = record.GetIntField("PRHA");

                            if (RightHandStyleMapping.TryGetValue(numericRightHandStyle, out string stringRightHandStyle))      //done 12=10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringRightHandStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearHand_None");
                            }
                            builder.Append(",");
                            builder.Append("LeftShoe");
                            builder.Append(",");
                            int numericLeftFootwearStyle = record.GetIntField("PLSH");

                            if (LeftFootwearStyleMapping.TryGetValue(numericLeftFootwearStyle, out string stringLeftFootwearStyle))     //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringLeftFootwearStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearFootwear_shoe_mid_AdidasFreak");
                            }
                            builder.Append(",");
                            builder.Append("RightShoe");
                            builder.Append(",");
                            int numericRightFootwearStyle = record.GetIntField("PRSH");

                            if (RightFootwearStyleMapping.TryGetValue(numericRightFootwearStyle, out string stringRightFootwearStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringRightFootwearStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearFootwear_shoe_mid_AdidasFreak");
                            }
                            builder.Append(",");
                            builder.Append("LeftSpat");
                            builder.Append(",");
                            int numericGearLeftSpatsStyle = record.GetIntField("PSPL");

                            if (GearLeftSpatsStyleMapping.TryGetValue(numericGearLeftSpatsStyle, out string stringGearLeftSpatsStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringGearLeftSpatsStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearSpats_none");
                            }
                            builder.Append(",");
                            int numericSkinToneStyle = record.GetIntField("PGHE");

                            if (SkinToneMapping.TryGetValue(numericSkinToneStyle, out int stringSkinToneStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringSkinToneStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("0");
                            }
                            builder.Append(",");
                            builder.Append("-8355712");
                            builder.Append(",");
                            builder.Append(record.GetIntField("PWGT")+ 160);
                            builder.Append(",");
                            builder.Append("RightSpat");
                            builder.Append(",");
                            int numericGearRightSpatsStyle = record.GetIntField("PSPL");

                            if (GearRightSpatsStyleMapping.TryGetValue(numericGearRightSpatsStyle, out string stringGearRightSpatsStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringGearRightSpatsStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearSpats_none");
                            }
                            builder.Append(",");
                            builder.Append("LeftWristgear");
                            builder.Append(",");
                            int numericGearLeftWristStyle = record.GetIntField("PLWR");

                            if (GearLeftWristStyleMapping.TryGetValue(numericGearLeftWristStyle, out string stringGearLeftWristStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringGearLeftWristStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearWrist_None");
                            }
                            builder.Append(",");
                            builder.Append("RightWristgear");
                            builder.Append(",");
                            int numericGearRightWristStyle = record.GetIntField("PRWR");

                            if (GearRightWristStyleMapping.TryGetValue(numericGearRightWristStyle, out string stringGearRightWristStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringGearRightWristStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("GearWrist_none");
                            }
                            builder.Append(",");
                            builder.Append("ThighGear");
                            builder.Append(",");
                            int numericGearThighStyle = record.GetIntField("PLTH");

                            if (GearThighStyleMapping.TryGetValue(numericGearLeftWristStyle, out string stringGearThighStyle))      //done 12-10-23
                            {
                                // Append the string value to the data line
                                builder.Append(stringGearThighStyle);
                            }
                            else
                            {
                                // Handle the case where there is no mapping for the numeric value
                                builder.Append("ThighPad_Regular");
                            }
                            builder.Append(",");
                            int flakJacketValue = record.GetIntField("PFLA");       //Big Test trying to combine 2 into 1
                            int backPlateValue = record.GetIntField("PLBP");

                            if (flakJacketValue == 1 && backPlateValue == 1)
                            {
                                // Both Flak Jacket and Back Plate are present, so use Flak Jacket
                                builder.Append("FlakJacket");
                            }
                            else if (backPlateValue == 1)
                            {
                                // Only Back Plate is present
                                builder.Append("BackPlate");
                            }
                            else if (flakJacketValue == 1)
                            {
                                // Only Flak Jacket is present
                                builder.Append("FlakJacket");
                            }
                            // If none are present, you may choose to do nothing or handle it differently

                            // Continue appending other parts of your string using the gearBuilder
                            builder.Append(",");

                            // Now, gearBuilder.ToString() contains the entire string with the selected gear
                            string finalString = builder.ToString();

                            int flakJacketvalue = record.GetIntField("PFLA");       //Big Test trying to combine 2 into 1
                            int backPlatevalue = record.GetIntField("PLBP");

                            if (flakJacketValue == 1 && backPlateValue == 1)
                            {
                                // Both Flak Jacket and Back Plate are present, so use Flak Jacket
                                builder.Append("Flakjacket_On");
                            }
                            else if (backPlateValue == 1)
                            {
                                // Only Back Plate is present
                                builder.Append("Backplate_Standard");
                            }
                            else if (flakJacketValue == 1)
                            {
                                // Only Flak Jacket is present
                                builder.Append("Flakjacket_On");
                            }
                            // If none are present, you may choose to do nothing or handle it differently

                            // Continue appending other parts of your string using the gearBuilder
                            builder.Append(",");

                            // Now, gearBuilder.ToString() contains the entire string with the selected gear
                            string finalstring = builder.ToString();

                            if (flakJacketValue == 1 && backPlateValue == 1)
                            {
                            builder.Append(record.GetFloatField("BSWT"));
                            }
                            else if (backPlateValue == 1)
                            {
                            builder.Append(record.GetFloatField("BSWT"));
                            }
                            else if (flakJacketValue == 1)
                            {
                                // Only Flak Jacket is present
                             builder.Append(record.GetFloatField("BSWT"));
                            }
                            builder.Append(",");
                            // loadouts_0_loadoutElements_0_blends_0_baseBlend (Pad Height)
                            if (flakJacketValue == 1 && backPlateValue == 1)
                            {
                                builder.Append(record.GetFloatField("BSWA"));
                            }
                            else if (backPlateValue == 1)
                            {
                                builder.Append(record.GetFloatField("BSWA"));
                            }
                            else if (flakJacketValue == 1)
                            {
                                // Only Flak Jacket is present
                                builder.Append(record.GetFloatField("BSWA"));
                            }
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");
                            builder.Append("");
                            builder.Append(",");

                            // Write the data line to the file
                            writer.WriteLine(builder.ToString());
                            writer.Flush();
                        }

                        writer.Close();
                    }
                    MessageBox.Show("Generated Visuals Successfully completed.");
                }
                catch (IOException)
                {
                    MessageBox.Show("Error opening file\r\n\r\n Check that the file is not already opened", "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




        private void CreateDraftClass_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            Stream myStream = null;
            dialog.Filter = "Draft Class CSV file (*.csv)|*.csv|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = null;
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    if ((myStream = dialog.OpenFile()) != null)
                    {
                        sr = new StreamReader(myStream);
                        string csvtableinfo = sr.ReadLine();
                        string[] csvtable = csvtableinfo.Split(',');
                        if (csvtable[0] != "DRAFT")
                        {
                            MessageBox.Show("Not a valid Madden Draft Class", "Not a Draft Class", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sr.Close();
                            return;
                        }
                        string csvfieldline = sr.ReadLine();
                        string[] csvfields = csvfieldline.Split(',');

                        if (csvtable[2].ToUpper().Contains("Y"))
                        {
                            sr.ReadLine();
                        }

                        List<string> records = new List<string>();
                        while (!sr.EndOfStream)
                        {
                            string csvrecordline = sr.ReadLine();
                            records.Add(csvrecordline);
                        }

                        model.DraftClassModel.ImportCSVDraftClass(records, csvfields);
                    }
                }
                catch
                {

                }

                if (sr != null)
                    sr.Close();

                this.Cursor = Cursors.Default;
            }

            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.RestoreDirectory = true;
            savedialog.Filter = "Madden Draft Class (*.*)|*.*";
            savedialog.ShowDialog();

            string filename = savedialog.FileName;
            if (filename == "")
                return;

            model.DraftClassModel.SaveDraftClass(filename, FB_Draft, ExportVersion.SelectedIndex);
        }

        private void CreateCommentID_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> CommentIDs = new Dictionary<string, int>();

            OpenFileDialog dialog = new OpenFileDialog();
            Stream myStream = null;
            dialog.Filter = "All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = null;
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    if ((myStream = dialog.OpenFile()) != null)
                    {
                        sr = new StreamReader(myStream);

                        while (!sr.EndOfStream)
                        {
                            bool stop = false;
                            string line = sr.ReadLine();
                            if (line.Contains("SurnameToAudioStore"))
                            {
                                while (!stop)
                                {
                                    string line0 = sr.ReadLine();
                                    if (!line0.Contains("SurnameToAudioID"))
                                        stop = true;

                                    int id = 0;
                                    string name = "";

                                    string line1 = sr.ReadLine();
                                    if (line1.Contains("value="))
                                    {
                                        line1 = line1.Replace("\"", "");
                                        string[] ids = line1.Split(' ');
                                        foreach (string s in ids)
                                        {
                                            if (s.Contains("value="))
                                            {
                                                id = Convert.ToInt32(s.Replace("value=", ""));
                                                break;
                                            }
                                        }
                                    }

                                    string line2 = sr.ReadLine();
                                    if (line2.Contains("Surname"))
                                    {
                                        line2 = line2.Replace("\"", "");
                                        string[] ids = line2.Split(' ');
                                        foreach (string s in ids)
                                        {
                                            if (s.Contains("value="))
                                            {
                                                name = s.Replace("value=", "");
                                                break;
                                            }
                                        }
                                    }

                                    string line3 = sr.ReadLine(); // </record>

                                    if (name != "" && !CommentIDs.ContainsKey(name))
                                        CommentIDs.Add(name, id);
                                }
                            }

                        }


                    }

                }
                catch
                {

                }

                foreach (PlayerRecord rec in model.TableModels[EditorModel.PLAYER_TABLE].GetRecords())
                {
                    if (rec.LastName != "" && rec.LastName != "Player" && !CommentIDs.ContainsKey(rec.LastName))
                        CommentIDs.Add(rec.LastName, rec.PlayerComment);
                }


                SaveFileDialog savedialog = new SaveFileDialog();
                string direct = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                direct += "\\Madden NFL 19\\settings";
                if (Directory.Exists(direct))
                    dialog.InitialDirectory = direct;
                else savedialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                savedialog.ShowDialog();

                string filename = savedialog.FileName;
                if (filename == "")
                    return;

                StreamWriter wText = new StreamWriter(filename);

                foreach (KeyValuePair<string, int> id in CommentIDs)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(id.Key);
                    builder.Append(",");
                    builder.Append(id.Value.ToString());

                    wText.WriteLine(builder.ToString());
                }

                wText.Flush();
                wText.Close();


            }
        }


    }
}






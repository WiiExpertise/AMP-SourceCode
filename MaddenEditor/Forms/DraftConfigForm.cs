/******************************************************************************
 * Madden 2005 Editor
 * Copyright (C) 2005 MaddenWishlist.com
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
using System.IO;
using System.Windows.Forms;
using MaddenEditor.Core;

namespace MaddenEditor.Forms
{
    public partial class DraftConfigForm : Form
    {
        EditorModel model;
        ScoutingForm scoutingForm;
        DraftModel draftModel;
        int secs;
        int humanId;
        string customclass = null;
        bool continueLoading = true;
        bool isInitializing = false;
        int ResumeRound = 0;
        int ResumePick = 0;

        public DraftConfigForm(EditorModel em)
        {
            model = em;
            InitializeComponent();
        }

        private void DraftConfigForm_Load(object sender, EventArgs e)
        {
            List<string> teamNames = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                teamNames.Add(model.TeamModel.GetTeamRecord(i).Name);
            }
            teamNames.Sort();
            teamChooser.Items.AddRange(teamNames.ToArray());
            seconds.Value = 0;
            minutes.Value = 5;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            secs = (int)minutes.Value * 60 + (int)seconds.Value;
            humanId = model.TeamModel.GetTeamIdFromTeamName((string)teamChooser.SelectedItem);

            if (draftClass.Checked)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.DefaultExt = "mdc";
                ofd.Filter = "Madden Draft Class (*.mdc)|*.mdc";
                ofd.Multiselect = false;
                ofd.ShowDialog();

                if (ofd.FileName.Length > 0)
                {
                    customclass = ofd.FileName;
                }
            }

            draftModel = new DraftModel(model);

            if (customclass != null)
            {
                string response = draftModel.MDCVerify(customclass);

                if (response != null)
                {
                    MessageBox.Show("Error reading custom draft class file.  " + response, "Error");
                    this.Cursor = Cursors.Arrow;
                    startButton.Enabled = true;
                    customclass = null;
                    return;
                }
            }

            progressBar.Visible = true;
            //backgrounWorker.RunWorkerAsync();
            bg1();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void teamChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!teamChooser.SelectedItem.ToString().Equals(""))
            {
                startButton.Enabled = true;
            }
        }

        public int promptFix(string analysis, int recommendation)
        {
            if (recommendation == 0)
            {
                DialogResult dr = MessageBox.Show("This class is not a good candidate for auto-repair.\n\nIt is recommended that you do NOT use this draft class, and answer \"Cancel\" below.  However, if you know that this is a Madden generated draft class, you may use auto-repair.\n\nChoose \"YES\" to auto-repair and continue.\nChoose \"No\" to continue without auto-repairing.\nChoose \"Cancel\" to exit the draft. (Recommended)\n\n---------------------------------\n\nDraft Class Analysis:\n" + analysis, "Draft Class Analysis", MessageBoxButtons.YesNoCancel);

                if (dr != DialogResult.Cancel)
                {
                    DialogResult dr2 = MessageBox.Show("Are you sure you want to continue?  Using this class could adversely affect your franchise.", "Confirm", MessageBoxButtons.YesNo);

                    if (dr2 == DialogResult.Yes)
                    {
                        if (dr == DialogResult.Yes)
                            return 1;
                        else if (dr == DialogResult.No)
                            return 0;
                    }
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("This class can probably be auto-repaired, to give the players more realistic ratings.\n\nIt is recommended that you do this, and answer \"YES\" below.\n\nChoose \"YES\" to auto-repair and continue. (Recommended)\nChoose \"No\" to continue without auto-repairing.\nChoose \"Cancel\" to exit the draft.\n\n---------------------------------\n\nDraft Class Analysis:\n" + analysis, "Draft Class Analysis", MessageBoxButtons.YesNoCancel);

                if (dr == DialogResult.Yes)
                {
                    return 1;
                }
                else if (dr == DialogResult.No)
                {
                    dr = MessageBox.Show("Are you sure you want to continue?  Using this draft class could adversely affect your franchise.", "Confirm", MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        return 0;
                    }
                }
            }

            continueLoading = false;
            return -1;
        }

        //		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        private void bg1()
        {
            // This method will run on a thread other than the UI thread.
            // Be sure not to manipulate any Windows Forms controls created
            // on the UI thread from this method.
            if (autoSave.Checked)
            {
                string fileName = model.GetFileName();
                string backupFile = fileName.Substring(0, fileName.LastIndexOf('.')) + "-mfebak";
                string newFile;

                if (overwrite.Checked)
                {
                    File.Delete(backupFile + "0.fra");
                }

                int index = 0;
                while (true)
                {
                    newFile = backupFile + index + ".fra";
                    if (!File.Exists(newFile))
                    {
                        break;
                    }
                    index++;
                }

                File.Copy(fileName, newFile);
            }

            //ReportProgress(15);
            draftModel.InitCoachScouting();

            draftModel.ResumePick = (int)ResumeSelection_Updown.Value;
            draftModel.ResumeRound = (int)ResumeRound_Updown.Value;
            draftModel.ResumeIndex = (int)ResumePick_Updown.Value;

            draftModel.InitializeDraft(humanId, this, customclass);

            if (continueLoading)
            {
                //ReportProgress(55);
                // Fix this for people who want to stay with Madden's Original order
                //draftModel.FixDraftOrder();
                //ReportProgress(75);
                draftModel.InitializeScouting();

                //ReportProgress(100);
            }
            if (continueLoading)
            {
                scoutingForm = new ScoutingForm(model, humanId, secs, draftModel, true);
                scoutingForm.Show();
            }

            this.Cursor = Cursors.Default;
            this.Close();
        }

        private void ResumeRound_Updown_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;
                ResumeSelection_Updown.Value = (int)(ResumeRound_Updown.Value - 1) * 32 + (int)ResumePick_Updown.Value;
                isInitializing = false;
            }
        }

        private void ResumePick_Updown_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;
                ResumeSelection_Updown.Value = (int)(ResumeRound_Updown.Value - 1) * 32 + (int)ResumePick_Updown.Value;
                isInitializing = false;
            }
        }

        private void ResumeSelection_Updown_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                isInitializing = true;
                ResumeRound_Updown.Value = (int)Math.Ceiling(ResumeSelection_Updown.Value / 32);
                int round = (int)Math.Ceiling(ResumeSelection_Updown.Value / 32) - 1;
                ResumePick_Updown.Value = ResumeSelection_Updown.Value - (round * 32);

                isInitializing = false;
            }
        }

        /*
		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            if (continueLoading)
            {
                scoutingForm = new ScoutingForm(model, humanId, secs, draftModel, true);
                scoutingForm.Show();
            }

			this.Cursor = Cursors.Default;
			this.Close();
		}

		public void ReportProgress(int percentage)
		{
			backgroundWorker.ReportProgress(percentage);
		}

 */

    }
}
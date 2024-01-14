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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using MaddenEditor.Core;
using MaddenEditor.Core.Record;

namespace MaddenEditor.Forms
{
    public partial class DepthChartEditorControl : UserControl, IEditorForm
    {
        private bool isInitialising = false;
        private EditorModel model = null;
        private DepthChartEditingModel depthEditingModel = null;
        public Overall playeroverall = new Overall();

        public DepthChartEditorControl()
        {
            isInitialising = true;
            InitializeComponent();
            isInitialising = false;
        }

        #region IEditorForm Members

        public MaddenEditor.Core.EditorModel Model
        {
            set { this.model = value; }
        }

        public void InitialiseUI()
        {
            depthEditingModel = new DepthChartEditingModel(model);
            isInitialising = true;
            foreach (TeamRecord team in model.TeamModel.GetTeams())
            {
                teamCombo.Items.Add(team);
            }

            for (int p = 0; p < 33; p++) // Always use Madden 19 positions
            {
                string pos = Enum.GetName(typeof(MaddenPositions2019), p);
                positionCombo.Items.Add(pos);
            }

            playeroverall.InitRatings19(); // Initialize for Madden 19

            positionCombo.Text = positionCombo.Items[0].ToString();
            teamCombo.SelectedIndex = 0;

            isInitialising = false;

            LoadDepthChart();

            if (availablePlayerDatagrid.Rows.Count > 0)
                availablePlayerDatagrid.Rows[0].Selected = true;
            if (depthChartDataGrid.Rows.Count > 0)
                this.depthChartDataGrid.Rows[0].Selected = true;
        }

        public void CleanUI()
        {
            teamCombo.Items.Clear();
        }

        #endregion

        /// <summary>
        /// This function should really be in the DepthChartEditingModel
        /// </summary>
        private void LoadDepthChart()
        {
            this.Cursor = Cursors.WaitCursor;
            isInitialising = true;
            int teamId = ((TeamRecord)teamCombo.SelectedItem).TeamId;
            int positionId = positionCombo.SelectedIndex;

            SortedList<int, DepthPlayerValueObject> depthList = depthEditingModel.GetPlayers(teamId, positionId);
            List<PlayerRecord> teamPlayers = depthEditingModel.GetAllPlayersOnTeamByOvr(teamId, positionId);

            depthChartDataGrid.Rows.Clear();

            foreach (DepthPlayerValueObject valObject in depthList.Values)
            {
                double overall = playeroverall.GetOverall19(valObject.playerObject, positionCombo.SelectedIndex, -1);
                DataGridViewRow row = valObject.playerObject.GetDataRow(positionId, (int)overall);
                // Now add our DepthChartRecord row on
                DataGridViewTextBoxCell depthCell = new DataGridViewTextBoxCell();
                depthCell.Value = valObject.depthObject;
                row.Cells.Add(depthCell);

                depthChartDataGrid.Rows.Add(row);
            }
            // Add the number of blank rows required for appropriate positions
            AddBlankRowsRequired(positionId);

            // Load the team's players in the availableDataGrid
            availablePlayerDatagrid.Rows.Clear();
            foreach (PlayerRecord record in teamPlayers)
            {
                double overall = playeroverall.GetOverall19(record, positionId, -1);
                availablePlayerDatagrid.Rows.Add(record.GetDataRow(positionId, (int)overall));
            }

            // Sort by OVR
            availablePlayerDatagrid.Sort(availablePlayerDatagrid.Columns["TeamOverall"], ListSortDirection.Descending);

            teamDepthChartLabel.Text = teamCombo.Text + " Depth Chart (" + positionCombo.Text + ")";

            if (availablePlayerDatagrid.Rows.Count > 0)
            {
                if (availablePlayerDatagrid.SelectedRows.Count == 0)
                    availablePlayerDatagrid.Rows[0].Selected = true;
            }
            if (depthChartDataGrid.Rows.Count > 0)
            {
                if (depthChartDataGrid.SelectedRows.Count == 0)
                    this.depthChartDataGrid.Rows[0].Selected = true;
            }
            this.Cursor = Cursors.Default;
            isInitialising = false;
        }

        private void AddBlankRowsRequired(int positionid)
        {
            int currentCount = depthChartDataGrid.Rows.Count;
            int requiredCount = 3;
            switch (positionid)
            {
                case (int)MaddenPositions2019.QB:
                case (int)MaddenPositions2019.FB:
                case (int)MaddenPositions2019.TE:
                case (int)MaddenPositions2019.LT:
                case (int)MaddenPositions2019.LG:
                case (int)MaddenPositions2019.C:
                case (int)MaddenPositions2019.RG:
                case (int)MaddenPositions2019.RT:
                case (int)MaddenPositions2019.LE:
                case (int)MaddenPositions2019.RE:
                case (int)MaddenPositions2019.LOLB:
                case (int)MaddenPositions2019.ROLB:
                case (int)MaddenPositions2019.FS:
                case (int)MaddenPositions2019.SS:
                case (int)MaddenPositions2019.K:
                case (int)MaddenPositions2019.P:
                case (int)MaddenPositions2019.KOS:
                case (int)MaddenPositions2019.LS:
                case (int)MaddenPositions2019.TDB:
                case (int)MaddenPositions2019.SWR:
                case (int)MaddenPositions2019.RLE:
                case (int)MaddenPositions2019.RRE:
                case (int)MaddenPositions2019.RDT:
                case (int)MaddenPositions2019.SLB:
                case (int)MaddenPositions2019.SCB:
                    // Must have 3 positions
                    requiredCount = 3;
                    break;
                case (int)MaddenPositions2019.HB:
                case (int)MaddenPositions2019.DT:
                case (int)MaddenPositions2019.KR:
                case (int)MaddenPositions2019.PR:
                    // Must have 4 positions
                    requiredCount = 4;
                    break;
                case (int)MaddenPositions2019.WR:
                    // Must have 6 positions
                    requiredCount = 6;
                    break;
                case (int)MaddenPositions2019.CB:
                    // Must have 5 positions
                    requiredCount = 5;
                    break;
                case (int)MaddenPositions2019.PHB:
                    requiredCount = 2;
                    break;
                default:
                    break;
            }


            while (currentCount++ < requiredCount)
            {
                depthChartDataGrid.Rows.Add();
            }
        }

        private void teamCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                LoadDepthChart();
            }
        }

        private void positionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitialising)
            {
                LoadDepthChart();
            }
        }

        private void availablePlayerDatagrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                availablePlayerDatagrid.ClearSelection();
                availablePlayerDatagrid.Rows[e.RowIndex].Selected = true;
            }
        }

        private void availablePlayerDatagrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                availablePlayerDatagrid.ClearSelection();
                availablePlayerDatagrid.Rows[e.RowIndex].Selected = true;
            }
        }

        private void depthChartDataGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                depthChartDataGrid.ClearSelection();
                depthChartDataGrid.Rows[e.RowIndex].Selected = true;
            }
        }

        private void depthChartDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                depthChartDataGrid.ClearSelection();
                depthChartDataGrid.Rows[e.RowIndex].Selected = true;
            }
        }

        private void depthOrderDownButton_Click(object sender, EventArgs e)
        {
            if (depthChartDataGrid.SelectedRows.Count == 1)
            {
                //We have to find the two DepthChart objects to exchange positions
                if (depthChartDataGrid.Rows.Count == 1)
                {
                    //We can't transfer down if there are only 1 row 
                    return;
                }
                if (depthChartDataGrid.SelectedRows[0].Index == depthChartDataGrid.Rows.Count - 1)
                {
                    //We can't transfer the bottom row down
                    return;
                }
                int selIndex = depthChartDataGrid.SelectedRows[0].Index;
                //There are at least 2 rows and we arent at the bottom so
                DepthChartRecord selectedRecord = (DepthChartRecord)depthChartDataGrid.Rows[depthChartDataGrid.SelectedRows[0].Index].Cells[4].Value;
                DepthChartRecord destinationRecord = (DepthChartRecord)depthChartDataGrid.Rows[depthChartDataGrid.SelectedRows[0].Index + 1].Cells[4].Value;

                //Now don't swap to/from a null row
                if (selectedRecord == null || destinationRecord == null)
                {
                    return;
                }

                int tempDepthOrd = destinationRecord.DepthOrder;
                destinationRecord.DepthOrder = selectedRecord.DepthOrder;
                selectedRecord.DepthOrder = tempDepthOrd;

                LoadDepthChart();

                //Now select the one we just moved
                depthChartDataGrid.ClearSelection();
                depthChartDataGrid.Rows[selIndex + 1].Selected = true;
            }
        }

        private void depthOrderUpButton_Click(object sender, EventArgs e)
        {
            if (depthChartDataGrid.SelectedRows.Count == 1)
            {
                //We have to find the two DepthChart objects to exchange positions
                if (depthChartDataGrid.Rows.Count == 1)
                {
                    //We can't transfer up if there are only 1 row 
                    return;
                }
                if (depthChartDataGrid.SelectedRows[0].Index == 0)
                {
                    //We can't transfer the top row up
                    return;
                }
                int selIndex = depthChartDataGrid.SelectedRows[0].Index;
                //There are at least 2 rows and we arent at the top so
                DepthChartRecord selectedRecord = (DepthChartRecord)depthChartDataGrid.Rows[depthChartDataGrid.SelectedRows[0].Index].Cells[4].Value;
                DepthChartRecord destinationRecord = (DepthChartRecord)depthChartDataGrid.Rows[depthChartDataGrid.SelectedRows[0].Index - 1].Cells[4].Value;

                //Now don't swap to/from a null row
                if (selectedRecord == null || destinationRecord == null)
                {
                    return;
                }

                int tempDepthOrd = destinationRecord.DepthOrder;
                destinationRecord.DepthOrder = selectedRecord.DepthOrder;
                selectedRecord.DepthOrder = tempDepthOrd;

                LoadDepthChart();

                //Now select the one we just moved
                depthChartDataGrid.ClearSelection();
                depthChartDataGrid.Rows[selIndex - 1].Selected = true;
            }
        }

        private void transferButton_Click(object sender, EventArgs e)
        {
            if (availablePlayerDatagrid.SelectedRows.Count == 1 && depthChartDataGrid.SelectedRows.Count == 1)
            {
                //First check that the player we are bringing up into the depth chart isnt already there
                foreach (DataGridViewRow row in depthChartDataGrid.Rows)
                {
                    if (row.Cells[3].Value == availablePlayerDatagrid.SelectedRows[0].Cells[3].Value)
                    {
                        //Trying to transfer a player already there
                        return;
                    }
                }
                if (depthChartDataGrid.SelectedRows[0].Cells[4].Value == null)
                {
                    //We are transfering to a blank depthchart record we need to create a record for it
                    DepthChartRecord newRecord = (DepthChartRecord)model.TableModels[EditorModel.DEPTH_CHART_TABLE].CreateNewRecord(true);
                    PlayerRecord playerRecord = (PlayerRecord)availablePlayerDatagrid.SelectedRows[0].Cells[3].Value;

                    newRecord.PlayerId = playerRecord.PlayerId;
                    newRecord.TeamId = playerRecord.TeamId;
                    newRecord.DepthOrder = depthChartDataGrid.SelectedRows[0].Index;
                    newRecord.PositionId = positionCombo.SelectedIndex;

                    LoadDepthChart();
                    return;
                }
                else
                {
                    //We are swapping two players
                    DepthChartRecord depthRecord = (DepthChartRecord)depthChartDataGrid.SelectedRows[0].Cells[4].Value;

                    PlayerRecord playerRecord = (PlayerRecord)availablePlayerDatagrid.SelectedRows[0].Cells[3].Value;

                    depthRecord.PlayerId = playerRecord.PlayerId;

                    //Load up the screen again
                    LoadDepthChart();
                    return;
                }
            }
        }

        private void GenerateDepthChart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate the depth chart automatically?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Get the selected position from the combo box
                int selectedPosition = positionCombo.SelectedIndex;

                // Filter players based on the selected position
                var filteredPlayers = availablePlayerDatagrid.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => ((PlayerRecord)row.Cells[3].Value).PositionId == selectedPosition)
                    .ToList();

                // Sort filtered players by overall in descending order
                var sortedPlayers = filteredPlayers
                    .OrderByDescending(row => ((PlayerRecord)row.Cells[3].Value).Overall)
                    .ToList();

                // Update the depth chart based on the sorted players
                for (int i = 0; i < depthChartDataGrid.Rows.Count && i < sortedPlayers.Count; i++)
                {
                    var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;
                    var playerRecord = (PlayerRecord)sortedPlayers[i].Cells[3].Value;

                    if (depthRecord == null)
                    {
                        // Create a new depth chart record if it's a blank row
                        depthRecord = (DepthChartRecord)model.TableModels[EditorModel.DEPTH_CHART_TABLE].CreateNewRecord(true);
                        depthRecord.TeamId = playerRecord.TeamId;
                        depthRecord.PositionId = selectedPosition;
                    }

                    // Update the depth order based on the loop index
                    depthRecord.DepthOrder = i;

                    // Update the player ID in the depth chart record
                    depthRecord.PlayerId = playerRecord.PlayerId;
                }

                // Reload the depth chart after updating
                LoadDepthChart();
            }
        }

        private void GenerateDepthChartBPA_Click(object sender, EventArgs e)
{
    if (MessageBox.Show("Are you sure you want to generate the depth chart automatically?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
    {
        // Get all available players and sort them by TeamOverall in descending order
        var allPlayers = availablePlayerDatagrid.Rows
            .Cast<DataGridViewRow>()
            .OrderByDescending(row => (int)row.Cells["TeamOverall"].Value)
            .ToList();

        // Update the depth chart based on the sorted players
        for (int i = 0; i < depthChartDataGrid.Rows.Count && i < allPlayers.Count; i++)
        {
            var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;
            var playerRecord = (PlayerRecord)allPlayers[i].Cells[3].Value;

            if (depthRecord == null)
            {
                // Create a new depth chart record if it's a blank row
                depthRecord = (DepthChartRecord)model.TableModels[EditorModel.DEPTH_CHART_TABLE].CreateNewRecord(true);
                depthRecord.TeamId = playerRecord.TeamId;
                depthRecord.PositionId = playerRecord.PositionId; // Set PositionId based on the player's position
            }

            // Update the depth order based on the loop index
            depthRecord.DepthOrder = i;

            // Update the player ID in the depth chart record
            depthRecord.PlayerId = playerRecord.PlayerId;
        }

        // Reload the depth chart after updating
        LoadDepthChart();
    }
}




        private void GenerateTeamDepthChart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate the depth chart automatically?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int positionIndex = 0; positionIndex < positionCombo.Items.Count; positionIndex++)
                {
                    // Set the selected position in the ComboBox
                    positionCombo.SelectedIndex = positionIndex;

                    // Get the selected position from the combo box
                    int selectedPosition = positionCombo.SelectedIndex;
                    string selectedPositionName = Enum.GetName(typeof(MaddenPositions2019), selectedPosition);

                    // Filter players based on the selected position
                    var filteredPlayers = availablePlayerDatagrid.Rows
                        .Cast<DataGridViewRow>()
                        .Where(row => ((PlayerRecord)row.Cells[3].Value).PositionId == selectedPosition)
                        .ToList();

                    // Sort filtered players by overall in descending order
                    var sortedPlayers = filteredPlayers
                        .OrderByDescending(row => (int)row.Cells["TeamOverall"].Value)
                        .ToList();

                    // Update the depth chart based on the sorted players
                    for (int i = 0; i < depthChartDataGrid.Rows.Count && i < sortedPlayers.Count; i++)
                    {
                        var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;
                        var playerRecord = (PlayerRecord)sortedPlayers[i].Cells[3].Value;

                        if (depthRecord == null)
                        {
                            // Create a new depth chart record if it's a blank row
                            depthRecord = (DepthChartRecord)model.TableModels[EditorModel.DEPTH_CHART_TABLE].CreateNewRecord(true);
                            depthRecord.TeamId = playerRecord.TeamId;
                            depthRecord.PositionId = selectedPosition;
                            depthRecord.DepthOrder = i;
                        }

                        // Update the player ID in the depth chart record
                        depthRecord.PlayerId = playerRecord.PlayerId;
                    }
                }

                // Reload the depth chart after updating
                LoadDepthChart();
            }
        }


        private void GenerateAllDepthChart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate the depth chart automatically?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int teamIndex = 0; teamIndex < teamCombo.Items.Count; teamIndex++)
                {
                    // Set the selected team in the ComboBox
                    teamCombo.SelectedIndex = teamIndex;

                    for (int positionIndex = 0; positionIndex < positionCombo.Items.Count; positionIndex++)
                    {
                        // Set the selected position in the ComboBox
                        positionCombo.SelectedIndex = positionIndex;

                        // Get the selected team and position
                        int selectedTeam = teamCombo.SelectedIndex;
                        int selectedPosition = positionCombo.SelectedIndex;

                        // Filter players based on the selected team and position
                        var filteredPlayers = availablePlayerDatagrid.Rows
                            .Cast<DataGridViewRow>()
                            .Where(row => ((PlayerRecord)row.Cells[3].Value).TeamId == selectedTeam && ((PlayerRecord)row.Cells[3].Value).PositionId == selectedPosition)
                            .ToList();

                        // Sort filtered players by overall in descending order
                        var sortedPlayers = filteredPlayers
                            .OrderByDescending(row => (int)row.Cells["TeamOverall"].Value)
                            .ToList();

                        // Update the depth chart based on the sorted players
                        for (int i = 0; i < depthChartDataGrid.Rows.Count && i < sortedPlayers.Count; i++)
                        {
                            var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;
                            var playerRecord = (PlayerRecord)sortedPlayers[i].Cells[3].Value;

                            if (depthRecord == null)
                            {
                                // Create a new depth chart record if it's a blank row
                                depthRecord = (DepthChartRecord)model.TableModels[EditorModel.DEPTH_CHART_TABLE].CreateNewRecord(true);
                                depthRecord.TeamId = selectedTeam;
                                depthRecord.PositionId = selectedPosition;
                                depthRecord.DepthOrder = i;
                            }

                            // Update the player ID in the depth chart record
                            depthRecord.PlayerId = playerRecord.PlayerId;
                        }
                    }
                }

                // Reload the depth chart after updating
                LoadDepthChart();
            }
        }

        private void EraseTeamDepthChart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to erase the team's depth chart?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int positionIndex = 0; positionIndex < positionCombo.Items.Count; positionIndex++)
                {
                    // Set the selected position in the ComboBox
                    positionCombo.SelectedIndex = positionIndex;

                    // Get the selected position from the combo box
                    int selectedPosition = positionCombo.SelectedIndex;

                    // Iterate through each row in the depth chart
                    for (int i = 0; i < depthChartDataGrid.Rows.Count; i++)
                    {
                        var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;

                        // Check if the depth record belongs to the selected position
                        if (depthRecord != null && depthRecord.PositionId == selectedPosition)
                        {
                            // Mark the record for deletion
                            depthRecord.SetDeleteFlag(true);

                            // Move the other records up
                            int index = depthRecord.DepthOrder + 1;
                            while (index < depthChartDataGrid.Rows.Count)
                            {
                                if (depthChartDataGrid.Rows[index].Cells[4].Value == null)
                                {
                                    // Found an empty row
                                    break;
                                }

                                // Move this one up
                                ((DepthChartRecord)depthChartDataGrid.Rows[index].Cells[4].Value).DepthOrder--;
                                index++;
                            }
                        }
                    }
                }

                // Reload the depth chart after updating
                LoadDepthChart();
            }
        }

        private void EraseAllDepthChart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to erase all depth chart entries?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int teamIndex = 0; teamIndex < teamCombo.Items.Count; teamIndex++)
                {
                    // Set the selected team in the ComboBox
                    teamCombo.SelectedIndex = teamIndex;

                    for (int positionIndex = 0; positionIndex < positionCombo.Items.Count; positionIndex++)
                    {
                        // Set the selected position in the ComboBox
                        positionCombo.SelectedIndex = positionIndex;

                        // Get the selected team and position
                        int selectedTeam = teamCombo.SelectedIndex;
                        int selectedPosition = positionCombo.SelectedIndex;

                        // Iterate through each row in the depth chart
                        for (int i = 0; i < depthChartDataGrid.Rows.Count; i++)
                        {
                            var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;

                            // Check if the depth record belongs to the selected team and position
                            if (depthRecord != null && depthRecord.TeamId == selectedTeam && depthRecord.PositionId == selectedPosition)
                            {
                                // Mark the record for deletion
                                depthRecord.SetDeleteFlag(true);

                                // Move the other records up
                                int index = depthRecord.DepthOrder + 1;
                                while (index < depthChartDataGrid.Rows.Count)
                                {
                                    if (depthChartDataGrid.Rows[index].Cells[4].Value == null)
                                    {
                                        // Found an empty row
                                        break;
                                    }

                                    // Move this one up
                                    ((DepthChartRecord)depthChartDataGrid.Rows[index].Cells[4].Value).DepthOrder--;
                                    index++;
                                }
                            }
                        }
                    }
                }

                // Reload the depth chart after updating
                LoadDepthChart();
            }
        }

        private void ClearDepthChart_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to erase the team's depth chart?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Get the selected position from the combo box
                int selectedPosition = positionCombo.SelectedIndex;

                // Iterate through each row in the depth chart
                for (int i = 0; i < depthChartDataGrid.Rows.Count; i++)
                {
                    var depthRecord = (DepthChartRecord)depthChartDataGrid.Rows[i].Cells[4].Value;

                    // Check if the depth record is not null and belongs to the selected position
                    if (depthRecord != null && depthRecord.PositionId == selectedPosition)
                    {
                        // Mark the record for deletion
                        depthRecord.SetDeleteFlag(true);

                        // Move the other records up
                        int index = depthRecord.DepthOrder + 1;
                        while (index < depthChartDataGrid.Rows.Count)
                        {
                            if (depthChartDataGrid.Rows[index].Cells[4].Value == null)
                            {
                                // Found an empty row
                                break;
                            }

                            // Move this one up
                            ((DepthChartRecord)depthChartDataGrid.Rows[index].Cells[4].Value).DepthOrder--;
                            index++;
                        }
                    }
                }

                // Reload the depth chart after updating
                LoadDepthChart();
            }
        }

        private void eraseButton_Click(object sender, EventArgs e)
        {
            if (depthChartDataGrid.SelectedRows.Count == 1)
            {
                if (depthChartDataGrid.SelectedRows[0].Cells[4].Value == null)
                {
                    //Don't delete an empy row
                    return;
                }
                else
                {
                    DepthChartRecord record = (DepthChartRecord)depthChartDataGrid.SelectedRows[0].Cells[4].Value;
                    record.SetDeleteFlag(true);
                    //move the other records up
                    int index = record.DepthOrder + 1;
                    while (index < depthChartDataGrid.Rows.Count)
                    {
                        if (depthChartDataGrid.Rows[index].Cells[4].Value == null)
                        {
                            //Found an empty
                            break;
                        }
                        //move this one up
                        ((DepthChartRecord)depthChartDataGrid.Rows[index].Cells[4].Value).DepthOrder--;
                        index++;
                    }
                    LoadDepthChart();
                    return;
                }
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }
    }
}

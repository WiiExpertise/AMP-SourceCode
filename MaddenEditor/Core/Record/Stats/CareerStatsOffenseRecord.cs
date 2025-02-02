/******************************************************************************
 * MaddenAmp
 * Copyright (C) 2014 Stingray68
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
 *****************************************************************************/

namespace MaddenEditor.Core.Record.Stats
{
    public class CareerStatsOffenseRecord : TableRecordModel
    {
        //QB Stats
        public const string PASS_ATT = "caat";
        public const string COMEBACKS = "cacb";             //  2007-2008 
        public const string PASS_COMP = "cacm";
        public const string FIRST_DOWNS = "cafd";           //  2007-2008 
        public const string PASS_INT = "cain";
        public const string PASS_LONG = "calN";
        public const string PASS_SACKED = "casa";
        public const string PASS_YDS = "caya";
        public const string PASS_TDS = "catd";

        // WR Stats
        public const string RECEIVING_RECS = "ccca";
        public const string RECEIVING_DROPS = "ccdr";
        public const string RECEIVING_TDS = "cctd";
        public const string RECEIVING_YARDS = "ccya";
        public const string RECEIVING_YAC = "ccyc";
        public const string RECEIVING_LONG = "ccrL";

        //RB Stats
        public const string RUSHING_TDS = "cutd";
        public const string RUSHING_LONG = "culN";
        public const string FUMBLES = "cufu";
        public const string RUSHING_ATTEMPTS = "cuat";
        public const string RUSHING_YARDS = "cuya";
        public const string RUSHING_YAC = "cuyh";
        public const string RUSHING_20 = "cu2y";
        public const string RUSHING_BT = "cubt";

        public const string PLAYER_ID = "PGID";



        public CareerStatsOffenseRecord(int record, TableModel tableModel, EditorModel EditorModel)
            : base(record, tableModel, EditorModel)
        {

        }

        public int Pass_att
        {
            get
            {
                return GetIntField(PASS_ATT);
            }
            set
            {
                SetField(PASS_ATT, value);
            }
        }

        public int Comebacks
        {
            get { return GetIntField(COMEBACKS); }
            set { SetField(COMEBACKS, value); }
        }

        public int Pass_comp
        {
            get
            {
                return GetIntField(PASS_COMP);
            }
            set
            {
                SetField(PASS_COMP, value);
            }
        }

        public int FirstDowns
        {
            get { return GetIntField(FIRST_DOWNS); }
            set { SetField(FIRST_DOWNS, value); }
        }

        public int Pass_int
        {
            get
            {
                return GetIntField(PASS_INT);
            }
            set
            {
                SetField(PASS_INT, value);
            }
        }

        public int Pass_long
        {
            get
            {
                return GetIntField(PASS_LONG);
            }
            set
            {
                SetField(PASS_LONG, value);
            }
        }

        public int Pass_sacked
        {
            get
            {
                return GetIntField(PASS_SACKED);
            }
            set
            {
                SetField(PASS_SACKED, value);
            }
        }

        public int Pass_yds
        {
            get
            {
                return GetIntField(PASS_YDS);
            }
            set
            {
                SetField(PASS_YDS, value);
            }
        }

        public int Pass_tds
        {
            get
            {
                return GetIntField(PASS_TDS);
            }
            set
            {
                SetField(PASS_TDS, value);
            }
        }

        public int Receiving_recs
        {
            get
            {
                return GetIntField(RECEIVING_RECS);
            }
            set
            {
                SetField(RECEIVING_RECS, value);
            }
        }

        public int Receiving_drops
        {
            get
            {
                return GetIntField(RECEIVING_DROPS);
            }
            set
            {
                SetField(RECEIVING_DROPS, value);
            }
        }

        public int Receiving_tds
        {
            get
            {
                return GetIntField(RECEIVING_TDS);
            }
            set
            {
                SetField(RECEIVING_TDS, value);
            }
        }

        public int Receiving_yards
        {
            get
            {
                return GetIntField(RECEIVING_YARDS);
            }
            set
            {
                SetField(RECEIVING_YARDS, value);
            }
        }

        public int Receiving_yac
        {
            get
            {
                return GetIntField(RECEIVING_YAC);
            }
            set
            {
                SetField(RECEIVING_YAC, value);
            }
        }

        public int Receiving_long
        {
            get
            {
                return GetIntField(RECEIVING_LONG);
            }
            set
            {
                SetField(RECEIVING_LONG, value);
            }
        }

        public int Fumbles
        {
            get
            {
                return GetIntField(FUMBLES);
            }
            set
            {
                SetField(FUMBLES, value);
            }
        }

        public int RushingAttempts
        {
            get
            {
                return GetIntField(RUSHING_ATTEMPTS);
            }
            set
            {
                SetField(RUSHING_ATTEMPTS, value);
            }
        }

        public int RushingYards
        {
            get
            {
                return GetIntField(RUSHING_YARDS);
            }
            set
            {
                SetField(RUSHING_YARDS, value);
            }
        }

        public int Rushing_tds
        {
            get
            {
                return GetIntField(RUSHING_TDS);
            }
            set
            {
                SetField(RUSHING_TDS, value);
            }
        }

        public int Rushing_long
        {
            get
            {
                return GetIntField(RUSHING_LONG);
            }
            set
            {
                SetField(RUSHING_LONG, value);
            }
        }

        public int Rushing_yac
        {
            get
            {
                return GetIntField(RUSHING_YAC);
            }
            set
            {
                SetField(RUSHING_YAC, value);
            }
        }

        public int Rushing_20
        {
            get
            {
                return GetIntField(RUSHING_20);
            }
            set
            {
                SetField(RUSHING_20, value);
            }
        }

        public int Rushing_bt
        {
            get
            {
                return GetIntField(RUSHING_BT);
            }
            set
            {
                SetField(RUSHING_BT, value);
            }
        }

        public int PlayerId
        {
            get
            {
                return GetIntField(PLAYER_ID);
            }
            set
            {
                SetField(PLAYER_ID, value);
            }
        }

    }
}

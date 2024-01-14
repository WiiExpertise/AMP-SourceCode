/******************************************************************************
 * MaddenAmp
 * Copyright (C) 2015 Stingray68
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
using System;
using System.Collections.Generic;
using MaddenEditor.Core.Record;

namespace MaddenEditor.Core
{
    public class ovrdef
    {
        #region Members

        public Dictionary<int, double> Ratings;
        public double totalweight = 0;
        // entries as found in table PORC in dbtemplates or streameddata in 2019
        public double RateHigh = 0;
        public double RateLow = 35;
        public double PACW = 0;
        public double PAGW = 0;
        public double PAWW = 0;
        public double PBKW = 0;
        public double PBSW = 0;
        public double PBTW = 0;
        public double PCAW = 0;
        public double PCIW = 0;
        public double PCNW = 0;
        public double PCRW = 0;
        public double PDRW = 0;
        public double PHTW = 0;
        public double PIJW = 0;
        public double PINW = 0;
        public double PJUW = 0;
        public double PKAW = 0;
        public double PKPW = 0;
        public double PKRW = 0;             //  kick return not used in any OVR for normal positions
        public double PLBW = 0;
        public double PLEW = 0;
        public double PLFW = 0;
        public double PLIW = 0;
        public double PLJW = 0;
        public double PLKW = 0;
        public double PLPW = 0;
        public double PLRW = 0;
        public double PLSW = 0;
        public double PLTW = 0;
        public double PTAD = 0;
        public double PBSK = 0;
        public double PTAM = 0;
        public double PTAS = 0;
        public double PBCV = 0;
        public double PPLA = 0;
        public double PLPE = 0;
        public double PTUP = 0;
        public double PTOR = 0;
        public int PLTY = 0;                // Player type
        public double PLUW = 0;
        public double PLWW = 0;
        public double PMCW = 0;
        public double PMRW = 0;
        public double PPBW = 0;
        public double PPEW = 0;
        public double PPMW = 0;
        public int PPOS = 0;
        public double PPRW = 0;
        public double PPWA = 0;
        public double PPWW = 0;
        public double PRBW = 0;
        public double PRDH = 0;
        public double PRDL = 0;
        public double PRLW = 0;
        public double PRRW = 0;
        public double PRSW = 0;
        public double PSAW = 0;
        public double PSEW = 0;
        public double PSHW = 0;
        public double PSTW = 0;
        public double PTAW = 0;
        public double PTCW = 0;
        public double PTDW = 0;
        public double PTMW = 0;
        public double PTOW = 0;
        public double PTPW = 0;
        public double PTRW = 0;
        public double PTSW = 0;
        public double PUPW = 0;
        public double PYSW = 0;
        public double PZCW = 0;
        public double SRRW = 0;
        #endregion

        public ovrdef()
        {
            Ratings = new Dictionary<int, double>();
            RateHigh = 0;
            RateLow = 35;
            PACW = 0;
            PAGW = 0;
            PAWW = 0;
            PBTW = 0;
            PCAW = 0;
            PCRW = 0;
            PINW = 0;
            PJUW = 0;
            PKAW = 0;
            PKPW = 0;
            PKRW = 0;
            PPBW = 0;
            PRBW = 0;
            PSEW = 0;
            PSTW = 0;
            PTAW = 0;
            PTCW = 0;
            PTPW = 0;

            PPOS = 0;
            totalweight = 0;
            SetRatings();
        }

        public ovrdef(int pos, int type)
        {

            Ratings = new Dictionary<int, double>();
            this.PPOS = pos;
            this.PLTY = type;


            if (pos == 0)
            #region QB
            {
                if (type == 0)
                {
                    //field general
                    this.RateLow = 43;
                    this.RateHigh = 95;
                    this.PAWW = 16;
                    this.PTAD = 12;
                    this.PTAM = 18;
                    this.PPLA = 3;
                    this.PTAS = 18;
                    this.PTOR = 3;
                    this.PTUP = 5;
                    this.PTPW = 25;

                }
                else if (type == 1)
                {
                    //strong arm
                    this.RateLow = 43;
                    this.RateHigh = 100;
                    this.PAWW = 12;
                    this.PBSK = 3;
                    this.PTAD = 18;
                    this.PTAM = 22;
                    this.PPLA = 2;
                    this.PTUP = 10;
                    this.PTPW = 33;

                }
                else if (type == 2)
                {
                    //improviser
                    this.RateLow = 43;
                    this.RateHigh = 94;
                    this.PAWW = 15;
                    this.PAGW = 2;
                    this.PACW = 1;
                    this.PBSK = 8;
                    this.PTAD = 15;
                    this.PLEW = 1;
                    this.PTAM = 4;
                    this.PTAS = 8;
                    this.PSEW = 2;
                    this.PTOR = 12;
                    this.PTUP = 15;
                    this.PTPW = 17;
                }
                else if (type == 3)
                {
                    //scrambler
                    this.RateLow = 43;
                    this.RateHigh = 99;
                    this.PBCV = 3;
                    this.PAWW = 7;
                    this.PAGW = 2;
                    this.PACW = 3;
                    this.PCRW = 2;
                    this.PBSK = 10;
                    this.PLEW = 1;
                    this.PTAM = 13;
                    this.PTAS = 14;
                    this.PSEW = 6;
                    this.PTOR = 10;
                    this.PTUP = 7;
                    this.PTPW = 22;
                }
            }
            #endregion

            else if (pos == 1)
            #region HB
            {
                if (type == 4)
                {
                    // power
                    this.RateLow = 50;
                    this.RateHigh = 95;
                    this.PBCV = 9;
                    this.PAWW = 11;
                    this.PAGW = 4;
                    this.PACW = 9;
                    this.PCRW = 13;
                    this.PBTW = 12;
                    this.PSEW = 9;
                    this.PSTW = 10;
                    this.PLSW = 10;
                    this.PLTW = 13;
                }
                else if (type == 5)
                {
                    // elusive
                    this.RateLow = 50;
                    this.RateHigh = 96;
                    this.PBCV = 10;
                    this.PAWW = 11;
                    this.PAGW = 12;
                    this.PACW = 13;
                    this.PCRW = 11;
                    this.PBTW = 5;
                    this.PLEW = 8;
                    this.PLJW = 9;
                    this.PLPW = 8;
                    this.PSEW = 13;
                }
                else if (type == 6)
                {
                    //receiving
                    this.RateLow = 52;
                    this.RateHigh = 90;
                    this.PBCV = 5;
                    this.PAWW = 11;
                    this.PAGW = 8;
                    this.PACW = 11;
                    this.PCRW = 6;
                    this.PLEW = 6;
                    this.PCIW = 8;
                    this.PCAW = 13;
                    this.PLJW = 5;
                    this.PSHW = 1;
                    this.PSEW = 11;
                    this.SRRW = 10;
                    this.PLPW = 5;
                }

            }
            #endregion

            else if (pos == 2)
            #region FB
            {
                if (type == 7)
                {
                    //blocking
                    this.RateLow = 33;
                    this.RateHigh = 86;
                    this.PAWW = 10;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PCRW = 5;
                    this.PBTW = 6;
                    this.PCIW = 2;
                    this.PCAW = 2;
                    this.PLIW = 7;
                    this.PLKW = 18;
                    this.PPWW = 1;
                    this.PPBW = 4;
                    this.PYSW = 1;
                    this.PRBW = 10;
                    this.SRRW = 5;
                    this.PLRW = 5;
                    this.PLWW = 3;
                    this.PSEW = 1;
                    this.PSTW = 9;
                    this.PLTW = 7;
                }
                else if (type == 8)
                {
                    //utility
                    this.RateLow = 33;
                    this.RateHigh = 86;
                    this.PBCV = 6;
                    this.PAWW = 10;
                    this.PAGW = 6;
                    this.PACW = 5;
                    this.PCRW = 7;
                    this.PBTW = 4;
                    this.PLEW = 2;
                    this.PCIW = 2;
                    this.PCAW = 8;
                    this.PLIW = 11;
                    this.PLKW = 5;
                    this.PPBW = 2;
                    this.PRBW = 3;
                    this.SRRW = 9;
                    this.PSEW = 5;
                    this.PSTW = 6;
                    this.PLSW = 4;
                    this.PLTW = 5;
                }
            }
            #endregion

            else if (pos == 3)
            #region WR
            {
                if (type == 9)
                {
                    //Deep Threat
                    this.RateLow = 39;
                    this.RateHigh = 96;
                    this.PAWW = 9;
                    this.PAGW = 8;
                    this.PACW = 12;
                    this.PLEW = 2;
                    this.PCAW = 12;
                    this.PDRW = 15;
                    this.PJUW = 1;
                    this.PLJW = 1;
                    this.PSHW = 9;
                    this.PMRW = 6;
                    this.PLPW = 1;
                    this.PSEW = 12;
                    this.PRLW = 4;
                    this.PCIW = 8;
                }
                else if (type == 10)
                {
                    //Route Runner
                    this.RateLow = 39;
                    this.RateHigh = 96;
                    this.PBCV = 7;
                    this.PAWW = 6;
                    this.PAGW = 10;
                    this.PACW = 10;
                    this.PBTW = 4;
                    this.PCIW = 3;
                    this.PLEW = 4;
                    this.PCAW = 5;
                    this.PDRW = 6;
                    this.PJUW = 2;
                    this.PLJW = 10;
                    this.PMRW = 12;
                    this.SRRW = 5;
                    this.PSEW = 10;
                    this.PRLW = 3;
                    this.PLPW = 3;
                }
                else if (type == 11)
                {
                    //Physical
                    this.RateLow = 39;
                    this.RateHigh = 96;
                    this.PAWW = 5;
                    this.PAGW = 3;
                    this.PACW = 6;
                    this.PBTW = 3;
                    this.PCAW = 10;
                    this.PCIW = 16;
                    this.PJUW = 9;
                    this.PMRW = 3;
                    this.PSHW = 12;
                    this.SRRW = 7;
                    this.PSEW = 4;
                    this.PRLW = 15;
                    this.PSTW = 4;
                    this.PLSW = 2;
                    this.PLTW = 1;
                }
                else if (type == 12)
                {
                    //Slot
                    this.RateLow = 38;
                    this.RateHigh = 97;
                    this.PAWW = 12;
                    this.PAGW = 12;
                    this.PACW = 10;
                    this.PCRW = 1;
                    this.PBTW = 1;
                    this.PLEW = 2;
                    this.PCIW = 14;
                    this.PCAW = 13;
                    this.PDRW = 1;
                    this.PLJW = 1;
                    this.PMRW = 7;
                    this.SRRW = 19;
                    this.PLPW = 1;
                    this.PSEW = 6;
                }
            }
            #endregion

            else if (pos == 4)
            #region TE
            {
                if (type == 13)
                {
                    //Blocking TE
                    this.RateLow = 28;
                    this.RateHigh = 82;
                    this.PAWW = 9;
                    this.PBTW = 2;
                    this.PCAW = 3;
                    this.PCIW = 3;
                    this.PLIW = 9;
                    this.PLKW = 8;
                    this.PLRW = 9;
                    this.PLSW = 2;
                    this.PLTW = 1;
                    this.PLWW = 9;
                    this.PMRW = 4;
                    this.PPBW = 8;
                    this.PPWW = 6;
                    this.PRBW = 10;
                    this.PSTW = 6;
                    this.PYSW = 6;
                    this.SRRW = 5;
                }
                else if (type == 14)
                {
                    //Vertical Threat TE
                    this.RateLow = 26;
                    this.RateHigh = 87;
                    this.PACW = 7;
                    this.PAGW = 4;
                    this.PAWW = 9;
                    this.PBTW = 3;
                    this.PCAW = 11;
                    this.PCIW = 8;
                    this.PDRW = 5;
                    this.PJUW = 3;
                    this.PLBW = 2;
                    this.PLEW = 2;
                    this.PLRW = 3;
                    this.PLSW = 2;
                    this.PLTW = 1;
                    this.PLWW = 3;
                    this.PMRW = 9;
                    this.PPBW = 2;
                    this.PPWW = 1;
                    this.PRBW = 3;
                    this.PRLW = 3;
                    this.PSEW = 7;
                    this.PSHW = 4;
                    this.PYSW = 1;
                    this.SRRW = 7;
                }
                else if (type == 15)
                {
                    //Possession TE
                    this.RateLow = 26;
                    this.RateHigh = 87;
                    this.PACW = 7;
                    this.PAGW = 5;
                    this.PAWW = 9;
                    this.PBTW = 3;
                    this.PCAW = 11;
                    this.PCIW = 8;
                    this.PJUW = 3;
                    this.PDRW = 5;
                    this.PLEW = 1;
                    this.PLRW = 3;
                    this.PLSW = 2;
                    this.PLTW = 1;
                    this.PLWW = 3;
                    this.PMRW = 9;
                    this.PPBW = 2;
                    this.PPWW = 1;
                    this.PRBW = 3;
                    this.PRLW = 4;
                    this.PSEW = 7;
                    this.PSHW = 3;
                    this.PYSW = 1;
                    this.SRRW = 7;
                    this.PBCV = 2;
                }
            }
            #endregion

            else if (pos == 5)
            #region LT
            {
                if (type == 19)
                {
                    //Pass Prot. T
                    this.RateLow = 32;
                    this.RateHigh = 96;
                    this.PACW = 2;
                    this.PAGW = 2;
                    this.PAWW = 14;
                    this.PLIW = 2;
                    this.PPBW = 10;
                    this.PPWW = 32;
                    this.PSEW = 2;
                    this.PSTW = 4;
                    this.PYSW = 32;

                }
                else if (type == 20)
                {
                    //Power T
                    this.RateLow = 30;
                    this.RateHigh = 96;
                    this.PACW = 2;
                    this.PAGW = 2;
                    this.PAWW = 10;
                    this.PLIW = 4;
                    this.PLKW = 2;
                    this.PLRW = 25;
                    this.PPBW = 4;
                    this.PRBW = 10;
                    this.PSEW = 2;
                    this.PSTW = 15;
                    this.PYSW = 24;

                }
                else if (type == 21)
                {
                    //Agile T
                    this.RateLow = 30;
                    this.RateHigh = 97;
                    this.PACW = 2;
                    this.PAGW = 3;
                    this.PAWW = 10;
                    this.PLIW = 3;
                    this.PLKW = 2;
                    this.PLWW = 24;
                    this.PPBW = 6;
                    this.PPWW = 25;
                    this.PRBW = 8;
                    this.PSEW = 3;
                    this.PSTW = 14;

                }
            }
            #endregion

            else if (pos == 6)
            #region LG
            {
                if (type == 22)
                {
                    //Pass Prot. G
                    this.RateLow = 32;
                    this.RateHigh = 95;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PAWW = 15;
                    this.PLIW = 5;
                    this.PPBW = 10;
                    this.PPWW = 30;
                    this.PSTW = 4;
                    this.PYSW = 30;
                    this.PSEW = 2;

                }
                else if (type == 23)
                {
                    //Power G
                    this.RateLow = 32;
                    this.RateHigh = 96;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PAWW = 11;
                    this.PLIW = 7;
                    this.PLKW = 8;
                    this.PLRW = 19;
                    this.PPBW = 2;
                    this.PRBW = 10;
                    this.PSTW = 15;
                    this.PYSW = 22;
                    this.PSEW = 2;

                }
                else if (type == 24)
                {
                    //Agile G
                    this.RateLow = 32;
                    this.RateHigh = 98;
                    this.PAGW = 3;
                    this.PACW = 2;
                    this.PAWW = 10;
                    this.PLIW = 6;
                    this.PLKW = 8;
                    this.PLWW = 20;
                    this.PPBW = 4;
                    this.PPWW = 23;
                    this.PSTW = 14;
                    this.PRBW = 8;
                    this.PSEW = 2;

                }
            }
            #endregion

            else if (pos == 7)
            #region C
            {
                this.RateLow = 34;
                this.RateHigh = 94;

                if (type == 16)
                {
                    //Pass Prot. C
                    this.RateLow = 34;
                    this.RateHigh = 94;
                    this.PAWW = 21;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PLIW = 4;
                    this.PPWW = 29;
                    this.PPBW = 10;
                    this.PYSW = 29;
                    this.PSTW = 3;

                }
                else if (type == 17)
                {
                    //Power C
                    this.RateLow = 34;
                    this.RateHigh = 95;
                    this.PAWW = 15;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PLIW = 6;
                    this.PLKW = 6;
                    this.PPBW = 2;
                    this.PYSW = 20;
                    this.PRBW = 10;
                    this.PLRW = 21;
                    this.PSEW = 2;
                    this.PSTW = 14;

                }
                else if (type == 18)
                {
                    //Agile C
                    this.RateLow = 34;
                    this.RateHigh = 97;
                    this.PAWW = 15;
                    this.PAGW = 3;
                    this.PACW = 2;
                    this.PLIW = 5;
                    this.PLKW = 6;
                    this.PPWW = 20;
                    this.PPBW = 4;
                    this.PRBW = 8;
                    this.PLWW = 21;
                    this.PSTW = 13;
                    this.PSEW = 3;

                }
            }
            #endregion

            else if (pos == 8)
            #region RG
            {
                if (type == 22)
                {
                    //Pass Prot. G
                    this.RateLow = 32;
                    this.RateHigh = 95;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PAWW = 15;
                    this.PLIW = 5;
                    this.PPBW = 10;
                    this.PPWW = 30;
                    this.PSTW = 4;
                    this.PYSW = 30;
                    this.PSEW = 2;

                }
                else if (type == 23)
                {
                    //Power G
                    this.RateLow = 32;
                    this.RateHigh = 96;
                    this.PAGW = 2;
                    this.PACW = 2;
                    this.PAWW = 11;
                    this.PLIW = 7;
                    this.PLKW = 8;
                    this.PLRW = 19;
                    this.PPBW = 2;
                    this.PRBW = 10;
                    this.PSTW = 15;
                    this.PYSW = 22;
                    this.PSEW = 2;

                }
                else if (type == 24)
                {
                    //Agile G
                    this.RateLow = 32;
                    this.RateHigh = 98;
                    this.PAGW = 3;
                    this.PACW = 2;
                    this.PAWW = 10;
                    this.PLIW = 6;
                    this.PLKW = 8;
                    this.PLWW = 20;
                    this.PPBW = 4;
                    this.PPWW = 23;
                    this.PSTW = 14;
                    this.PRBW = 8;
                    this.PSEW = 2;

                }
            }
            #endregion

            else if (pos == 9)
            #region RT
            {
                if (type == 19)
                {
                    //Pass Prot. T
                    this.RateLow = 32;
                    this.RateHigh = 96;
                    this.PACW = 2;
                    this.PAGW = 2;
                    this.PAWW = 14;
                    this.PLIW = 2;
                    this.PPBW = 10;
                    this.PPWW = 32;
                    this.PSEW = 2;
                    this.PSTW = 4;
                    this.PYSW = 32;
                }
                else if (type == 20)
                {
                    //Power T
                    this.RateLow = 30;
                    this.RateHigh = 96;
                    this.PACW = 2;
                    this.PAGW = 2;
                    this.PAWW = 10;
                    this.PLIW = 4;
                    this.PLKW = 2;
                    this.PLRW = 25;
                    this.PPBW = 4;
                    this.PRBW = 10;
                    this.PSEW = 2;
                    this.PSTW = 15;
                    this.PYSW = 24;
                }
                else if (type == 21)
                {
                    //Agile T
                    this.RateLow = 30;
                    this.RateHigh = 97;
                    this.PACW = 2;
                    this.PAGW = 3;
                    this.PAWW = 10;
                    this.PLIW = 3;
                    this.PLKW = 2;
                    this.PLWW = 24;
                    this.PPBW = 6;
                    this.PPWW = 25;
                    this.PRBW = 8;
                    this.PSEW = 3;
                    this.PSTW = 14;
                }
            }
            #endregion

            else if (pos == 10)
            #region LE
            {
                if (type == 25)
                {
                    //speed rush
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PACW = 14;
                    this.PAGW = 9;
                    this.PAWW = 12;
                    this.PLFW = 23;
                    this.PLUW = 7;
                    this.PPRW = 2;
                    this.PSEW = 10;
                    this.PSTW = 5;
                    this.PTCW = 15;
                    this.PHTW = 2;
                    this.PPMW = 1;
                }
                else if (type == 26)
                {
                    //power rush
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PACW = 10;
                    this.PAGW = 6;
                    this.PAWW = 12;
                    this.PLUW = 7;
                    this.PPMW = 23;
                    this.PPRW = 2;
                    this.PSEW = 7;
                    this.PSTW = 15;
                    this.PTCW = 15;
                    this.PHTW = 2;
                    this.PLFW = 1;
                }
                else if (type == 27)
                {
                    //run stopper
                    this.RateLow = 36;
                    this.RateHigh = 94;
                    this.PACW = 8;
                    this.PAGW = 5;
                    this.PAWW = 6;
                    this.PBSW = 18;
                    this.PLFW = 2;
                    this.PLUW = 9;
                    this.PPMW = 2;
                    this.PPRW = 12;
                    this.PSEW = 5;
                    this.PSTW = 12;
                    this.PTCW = 18;
                    this.PHTW = 3;
                }
            }
            #endregion

            else if (pos == 11)
            #region RE
            {
                if (type == 25)
                {
                    //speed rush
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PACW = 14;
                    this.PAGW = 9;
                    this.PAWW = 12;
                    this.PLFW = 23;
                    this.PLUW = 7;
                    this.PPRW = 2;
                    this.PSEW = 10;
                    this.PSTW = 5;
                    this.PTCW = 15;
                    this.PHTW = 2;
                    this.PPMW = 1;
                }
                else if (type == 26)
                {
                    //power rush
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PACW = 10;
                    this.PAGW = 6;
                    this.PAWW = 12;
                    this.PLUW = 7;
                    this.PPMW = 23;
                    this.PPRW = 2;
                    this.PSEW = 7;
                    this.PSTW = 15;
                    this.PTCW = 15;
                    this.PHTW = 2;
                    this.PLFW = 1;
                }
                else if (type == 27)
                {
                    //run stopper
                    this.RateLow = 36;
                    this.RateHigh = 94;
                    this.PACW = 8;
                    this.PAGW = 5;
                    this.PAWW = 6;
                    this.PBSW = 18;
                    this.PLFW = 2;
                    this.PLUW = 9;
                    this.PPMW = 2;
                    this.PPRW = 12;
                    this.PSEW = 5;
                    this.PSTW = 12;
                    this.PTCW = 18;
                    this.PHTW = 3;
                }
            }
            #endregion

            else if (pos == 12)
            #region DT
            {
                if (type == 28)
                {
                    //run stop
                    this.RateLow = 35;
                    this.RateHigh = 95;
                    this.PACW = 6;
                    this.PAGW = 3;
                    this.PAWW = 4;
                    this.PBSW = 22;
                    this.PLUW = 8;
                    this.PPRW = 12;
                    this.PSEW = 4;
                    this.PSTW = 21;
                    this.PTCW = 17;
                    this.PHTW = 3;

                }
                else if (type == 29)
                {
                    //speed rush
                    this.RateLow = 35;
                    this.RateHigh = 95;
                    this.PACW = 8;
                    this.PAGW = 4;
                    this.PAWW = 13;
                    this.PBSW = 4;
                    this.PLFW = 22;
                    this.PLUW = 7;
                    this.PPRW = 3;
                    this.PSEW = 6;
                    this.PSTW = 17;
                    this.PTCW = 13;
                    this.PHTW = 2;
                    this.PPMW = 1;
                }
                else if (type == 30)
                {
                    //power rush
                    this.RateLow = 35;
                    this.RateHigh = 94;
                    this.PACW = 6;
                    this.PAGW = 2;
                    this.PAWW = 12;
                    this.PBSW = 6;
                    this.PLFW = 1;
                    this.PLUW = 5;
                    this.PPMW = 22;
                    this.PPRW = 6;
                    this.PSEW = 5;
                    this.PSTW = 19;
                    this.PTCW = 13;
                    this.PHTW = 3;
                }
            }
            #endregion

            else if (pos == 13)
            #region LOLB
            {
                if (type == 31)
                {
                    //speed rush
                    this.RateLow = 37;
                    this.RateHigh = 92;
                    this.PACW = 14;
                    this.PAGW = 6;
                    this.PAWW = 11;
                    this.PLEW = 1;
                    this.PLFW = 25;
                    this.PMCW = 1;
                    this.PPMW = 1;
                    this.PPRW = 2;
                    this.PLUW = 10;
                    this.PSEW = 9;
                    this.PSTW = 6;
                    this.PTCW = 10;
                    this.PZCW = 4;
                }
                else if (type == 32)
                {
                    //power rush
                    this.RateLow = 37;
                    this.RateHigh = 92;
                    this.PACW = 10;
                    this.PAGW = 2;
                    this.PAWW = 12;
                    this.PHTW = 2;
                    this.PLFW = 1;
                    this.PMCW = 1;
                    this.PPMW = 25;
                    this.PPRW = 3;
                    this.PLUW = 12;
                    this.PSEW = 5;
                    this.PSTW = 8;
                    this.PTCW = 15;
                    this.PZCW = 4;
                }
                else if (type == 33)
                {
                    //pass coverage
                    this.RateLow = 37;
                    this.RateHigh = 89;
                    this.PACW = 11;
                    this.PAGW = 3;
                    this.PAWW = 15;
                    this.PLEW = 4;
                    this.PLUW = 11;
                    this.PMCW = 7;
                    this.PPRW = 7;
                    this.PSEW = 9;
                    this.PSTW = 2;
                    this.PTCW = 14;
                    this.PZCW = 17;

                }
                else if (type == 34)
                {
                    //run stop
                    this.RateLow = 36;
                    this.RateHigh = 91;
                    this.PACW = 9;
                    this.PAGW = 2;
                    this.PAWW = 4;
                    this.PBSW = 16;
                    this.PLEW = 2;
                    this.PHTW = 3;
                    this.PMCW = 2;
                    this.PPRW = 9;
                    this.PLUW = 16;
                    this.PSEW = 6;
                    this.PSTW = 10;
                    this.PTCW = 16;
                    this.PZCW = 5;
                }
            }
            #endregion

            else if (pos == 14)
            #region MLB
            {
                if (type == 35)
                {
                    // field general
                    this.RateLow = 39;
                    this.RateHigh = 91;
                    this.PACW = 9;
                    this.PAGW = 4;
                    this.PAWW = 9;
                    this.PBSW = 7;
                    this.PLEW = 3;
                    this.PMCW = 4;
                    this.PPRW = 9;
                    this.PLUW = 14;
                    this.PSEW = 5;
                    this.PTCW = 18;
                    this.PSTW = 8;
                    this.PZCW = 10;
                }
                else if (type == 36)
                {
                    //pass coverage
                    this.RateLow = 39;
                    this.RateHigh = 92;
                    this.PACW = 10;
                    this.PAGW = 6;
                    this.PAWW = 10;
                    this.PLEW = 4;
                    this.PMCW = 9;
                    this.PPRW = 4;
                    this.PLUW = 12;
                    this.PSEW = 7;
                    this.PTCW = 15;
                    this.PZCW = 19;
                    this.PSTW = 4;
                }
                else if (type == 37)
                {
                    this.RateLow = 40;

                    //run stop
                    this.RateLow = 39;
                    this.RateHigh = 92;
                    this.PACW = 8;
                    this.PAGW = 4;
                    this.PAWW = 5;
                    this.PBSW = 10;
                    this.PLEW = 2;
                    this.PHTW = 2;
                    this.PMCW = 2;
                    this.PPRW = 12;
                    this.PLUW = 15;
                    this.PSEW = 4;
                    this.PTCW = 20;
                    this.PSTW = 11;
                    this.PZCW = 5;
                }
            }
            #endregion

            else if (pos == 15)
            #region ROLB
            {

                if (type == 31)
                {
                    //speed rush
                    this.RateLow = 37;
                    this.RateHigh = 92;
                    this.PACW = 14;
                    this.PAGW = 6;
                    this.PAWW = 11;
                    this.PLEW = 1;
                    this.PLFW = 25;
                    this.PMCW = 1;
                    this.PPMW = 1;
                    this.PPRW = 2;
                    this.PLUW = 10;
                    this.PSEW = 9;
                    this.PSTW = 6;
                    this.PTCW = 10;
                    this.PZCW = 4;
                }
                else if (type == 32)
                {
                    //power rush
                    this.RateLow = 37;
                    this.RateHigh = 92;
                    this.PACW = 10;
                    this.PAGW = 2;
                    this.PAWW = 12;
                    this.PHTW = 2;
                    this.PLFW = 1;
                    this.PMCW = 1;
                    this.PPMW = 25;
                    this.PPRW = 3;
                    this.PLUW = 12;
                    this.PSEW = 5;
                    this.PSTW = 8;
                    this.PTCW = 15;
                    this.PZCW = 4;
                }
                else if (type == 33)
                {
                    //pass coverage
                    this.RateLow = 37;
                    this.RateHigh = 89;
                    this.PACW = 11;
                    this.PAGW = 3;
                    this.PAWW = 15;
                    this.PLEW = 4;
                    this.PLUW = 11;
                    this.PMCW = 7;
                    this.PPRW = 7;
                    this.PSEW = 9;
                    this.PSTW = 2;
                    this.PTCW = 14;
                    this.PZCW = 17;

                }
                else if (type == 34)
                {
                    //run stop
                    this.RateLow = 36;
                    this.RateHigh = 91;
                    this.PACW = 9;
                    this.PAGW = 2;
                    this.PAWW = 4;
                    this.PBSW = 16;
                    this.PLEW = 2;
                    this.PHTW = 3;
                    this.PMCW = 2;
                    this.PPRW = 9;
                    this.PLUW = 16;
                    this.PSEW = 6;
                    this.PSTW = 10;
                    this.PTCW = 16;
                    this.PZCW = 5;
                }
            }
            #endregion

            else if (pos == 16)
            #region CB
            {
                if (type == 38)
                {
                    //man to man
                    this.RateLow = 38;
                    this.RateHigh = 95;
                    this.PAWW = 11;
                    this.PAGW = 3;
                    this.PACW = 18;
                    this.PLEW = 5;
                    this.PCAW = 2;
                    this.PJUW = 2;
                    this.PMCW = 22;
                    this.PPRW = 5;
                    this.PLPE = 7;
                    this.PSEW = 20;
                    this.PTCW = 3;
                    this.PZCW = 2;
                }
                else if (type == 39)
                {
                    //slot
                    this.RateLow = 12;
                    this.RateHigh = 99;
                    this.PAWW = 8;
                    this.PAGW = 5;
                    this.PACW = 14;
                    this.PBSW = 2;
                    this.PLEW = 8;
                    this.PCAW = 2;
                    this.PJUW = 2;
                    this.PMCW = 14;
                    this.PPRW = 9;
                    this.PLUW = 4;
                    this.PLPE = 3;
                    this.PSEW = 12;
                    this.PTCW = 12;
                    this.PZCW = 5;
                }
                else if (type == 40)
                {
                    //zone
                    this.RateLow = 38;
                    this.RateHigh = 95;
                    this.PAWW = 11;
                    this.PAGW = 3;
                    this.PACW = 20;
                    this.PLEW = 4;
                    this.PCAW = 2;
                    this.PJUW = 2;
                    this.PMCW = 4;
                    this.PPRW = 7; ;
                    this.PLPE = 4;
                    this.PSEW = 18;
                    this.PTCW = 5;
                    this.PZCW = 20;
                }
            }
            #endregion

            else if (pos == 17)
            #region FS
            {
                this.RateLow = 38;
                this.RateHigh = 95;

                if (type == 41)
                {
                    //zone
                    this.RateLow = 35;
                    this.RateHigh = 94;
                    this.PAWW = 20;
                    this.PAGW = 6;
                    this.PACW = 10;
                    this.PLEW = 4;
                    this.PJUW = 3;
                    this.PPRW = 3;
                    this.PLUW = 7;
                    this.PSEW = 14;
                    this.PTCW = 9;
                    this.PSTW = 2;
                    this.PZCW = 22;
                }
                else if (type == 42)
                {
                    //hybrid
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PAWW = 14;
                    this.PAGW = 7;
                    this.PACW = 11;
                    this.PLEW = 4;
                    this.PJUW = 3;
                    this.PMCW = 14;
                    this.PPRW = 6;
                    this.PLUW = 4;
                    this.PLPE = 3;
                    this.PSEW = 14;
                    this.PTCW = 11;
                    this.PSTW = 2;
                    this.PZCW = 7;
                }
                else if (type == 43)
                {
                    //run support
                    this.RateLow = 35;
                    this.RateHigh = 91;
                    this.PAWW = 8;
                    this.PAGW = 6;
                    this.PACW = 8;
                    this.PBSW = 2;
                    this.PLEW = 3;
                    this.PHTW = 5;
                    this.PJUW = 3;
                    this.PMCW = 5;
                    this.PPRW = 16;
                    this.PLUW = 9;
                    this.PLPE = 1;
                    this.PSEW = 9;
                    this.PTCW = 15;
                    this.PSTW = 2;
                    this.PZCW = 8;
                }
            }
            #endregion

            else if (pos == 18)
            #region SS
            {
                this.RateLow = 38;
                this.RateHigh = 95;

                if (type == 41)
                {
                    //zone
                    this.RateLow = 35;
                    this.RateHigh = 94;
                    this.PACW = 10;
                    this.PAGW = 8;
                    this.PAWW = 11;
                    this.PJUW = 3;
                    this.PLUW = 6;
                    this.PMCW = 0;
                    this.PPEW = 0;
                    this.PPRW = 11;
                    this.PSEW = 14;
                    this.PSTW = 2;
                    this.PTCW = 11;
                    this.PZCW = 22;
                }
                else if (type == 42)
                {
                    //hybrid
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PAWW = 14;
                    this.PAGW = 7;
                    this.PACW = 11;
                    this.PLEW = 4;
                    this.PJUW = 3;
                    this.PMCW = 14;
                    this.PPRW = 6;
                    this.PLUW = 4;
                    this.PLPE = 3;
                    this.PSEW = 14;
                    this.PTCW = 11;
                    this.PSTW = 2;
                    this.PZCW = 7;
                }
                else if (type == 43)
                {
                    //run support
                    this.RateLow = 35;
                    this.RateHigh = 91;
                    this.PAWW = 8;
                    this.PAGW = 6;
                    this.PACW = 8;
                    this.PBSW = 2;
                    this.PLEW = 3;
                    this.PHTW = 5;
                    this.PJUW = 3;
                    this.PMCW = 5;
                    this.PPRW = 16;
                    this.PLUW = 9;
                    this.PLPE = 1;
                    this.PSEW = 9;
                    this.PTCW = 15;
                    this.PSTW = 2;
                    this.PZCW = 8;
                }
            }
            #endregion

            else if (pos == 19)
            #region K
            {
                if (type == 44)
                {
                    //accurate kicker
                    this.RateLow = 12;
                    this.RateHigh = 99;
                    this.PKPW = 16;
                    this.PAWW = 35;
                    this.PKAW = 49;

                }
                else if (type == 45)
                {
                    //power kicker
                    this.RateLow = 12;
                    this.RateHigh = 99;
                    this.PAWW = 41;
                    this.PKPW = 38;
                    this.PKAW = 21;

                }
            }
            #endregion

            else if (pos == 20)
            #region P
            {
                if (type == 44)
                {
                    //accurate kicker
                    this.RateLow = 10;
                    this.RateHigh = 99;
                    this.PKPW = 16;
                    this.PAWW = 35;
                    this.PKAW = 49;
                }
                else if (type == 45)
                {
                    //power kicker
                    this.RateLow = 10;
                    this.RateHigh = 99;
                    this.PAWW = 41;
                    this.PKPW = 38;
                    this.PKAW = 21;
                }
            }
            #endregion

            else if (pos == 21)
            #region KR
            {
                this.RateLow = 0;
                this.RateHigh = 99;

                if (type == 12)
                {
                    this.PACW = 8;
                    this.PKRW = 80;
                    this.PSEW = 12;
                }
            }
            #endregion

            else if (pos == 22)
            #region PR
            {
                this.RateLow = 0;
                this.RateHigh = 99;
                if (type == 12)
                {
                    this.PACW = 12;
                    this.PCAW = 5;
                    this.PKRW = 75;
                    this.PSEW = 12;
                }
            }
            #endregion

            else if (pos == 23)
            #region KOS
            {
                this.RateLow = 12;
                this.RateHigh = 99;

                if (type == 45)
                {
                    this.PKAW = 20;
                    this.PKPW = 80;
                }
            }
            #endregion

            else if (pos == 24)
            #region LS
            {
                this.RateLow = 35;
                this.RateHigh = 88;

                if (type == 18)
                {
                    this.PAWW = 5;
                    this.PPBW = 50;
                    this.PSTW = 5;

                }
            }
            #endregion

            else if (pos == 25)
            #region TDB
            {
                if (type == 6)
                {
                    //receiving
                    this.RateLow = 52;
                    this.RateHigh = 89;
                    this.PACW = 12;
                    this.PAGW = 8;
                    this.PAWW = 9;
                    this.PCAW = 14;
                    this.PCIW = 8;
                    this.PCRW = 7;
                    this.PLBW = 5;
                    this.PLEW = 4;
                    this.PLJW = 5;
                    this.PLPW = 5;
                    this.PSEW = 12;
                    this.SRRW = 10;
                }
            }
            #endregion

            else if (pos == 26)
            #region PHB
            {
                if (type == 4)
                {
                    // power
                    this.RateLow = 50;
                    this.RateHigh = 95;
                    this.PACW = 9;
                    this.PAGW = 5;
                    this.PAWW = 8;
                    this.PBTW = 12;
                    this.PCRW = 13;
                    this.PLBW = 10;
                    this.PLSW = 11;
                    this.PLTW = 13;
                    this.PSEW = 9;
                    this.PSTW = 10;
                }
            }
            #endregion

            else if (pos == 27)
            #region Slot WR
            {
                if (type == 12)
                {
                    this.RateLow = 35;
                    this.RateHigh = 95;
                    this.PACW = 12;
                    this.PAGW = 7;
                    this.PAWW = 10;
                    this.PBTW = 1;
                    this.PCAW = 12;
                    this.PCIW = 13;
                    this.PCRW = 1;
                    this.PDRW = 3;
                    this.PLBW = 1;
                    this.PLEW = 2;
                    this.PLJW = 1;
                    this.PLPW = 1;
                    this.PMRW = 9;
                    this.PRBW = 1;
                    this.PSEW = 10;
                    this.SRRW = 16;
                }
            }
            #endregion

            else if (pos == 28)
            #region RLE
            {
                if (type == 31)
                {
                    this.RateLow = 37;
                    this.RateHigh = 91;

                    this.PACW = 13;
                    this.PAGW = 7;
                    this.PAWW = 7;
                    this.PLFW = 18;
                    this.PLUW = 10;
                    this.PPMW = 14;
                    this.PPRW = 7;
                    this.PSEW = 8;
                    this.PSTW = 6;
                    this.PTCW = 10;
                }
            }
            #endregion

            else if (pos == 29)
            #region RRE
            {
                if (type == 31)
                {
                    this.RateLow = 37;
                    this.RateHigh = 91;

                    this.PACW = 13;
                    this.PAGW = 7;
                    this.PAWW = 7;
                    this.PLFW = 18;
                    this.PLUW = 10;
                    this.PPMW = 14;
                    this.PPRW = 7;
                    this.PSEW = 8;
                    this.PSTW = 6;
                    this.PTCW = 10;
                }
            }
            #endregion

            else if (pos == 30)
            #region RDT 
            {
                if (type == 26)
                {
                    this.RateLow = 36;
                    this.RateHigh = 92;

                    this.PACW = 10;
                    this.PAGW = 6;
                    this.PAWW = 7;
                    this.PHTW = 3;
                    this.PLFW = 5;
                    this.PLUW = 8;
                    this.PPMW = 18;
                    this.PPRW = 7;
                    this.PSEW = 7;
                    this.PSTW = 15;
                    this.PTCW = 16;
                }
            }
            #endregion

            else if (pos == 31)
            #region SLB
            {
                if (type == 33)
                {
                    this.RateLow = 36;
                    this.RateHigh = 93;
                    this.PACW = 11;
                    this.PAGW = 4;
                    this.PAWW = 9;
                    this.PBSW = 3;
                    this.PLUW = 11;
                    this.PMCW = 6;
                    this.PPRW = 9;
                    this.PSEW = 9;
                    this.PSTW = 2;
                    this.PTCW = 14;
                    this.PZCW = 15;
                }
            }
            #endregion

            else if (pos == 32)
            #region SCB
            {
                if (type == 39)
                {
                    this.RateLow = 37;
                    this.RateHigh = 92;
                    this.PACW = 14;
                    this.PAGW = 10;
                    this.PAWW = 9;
                    this.PBSW = 2;
                    this.PCAW = 2;
                    this.PJUW = 4;
                    this.PLUW = 4;
                    this.PMCW = 14;
                    this.PPEW = 3;
                    this.PPRW = 9;
                    this.PSEW = 12;
                    this.PTCW = 12;
                    this.PZCW = 5;
                }
            }
            #endregion

            SetTotalWeight((int)MaddenFileVersion.Ver2019);
            SetRatings();
        }

        public ovrdef(int pos)
        {
            Ratings = new Dictionary<int, double>();
            this.PPOS = pos;
            this.PINW = .5;

            switch (pos)
            {
                case 0:
                    {
                        this.RateHigh = 89;
                        this.PTAW = 4;
                        this.PSEW = 1.5;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PTPW = 3.5;
                        this.PBTW = .5;
                        this.PAWW = 3;
                        break;
                    }
                case 1:
                    {
                        this.RateHigh = 90;
                        this.PCAW = 1;
                        this.PPBW = .5;
                        this.PACW = 1;
                        this.PSEW = 2.5;
                        this.PAGW = 2.5;
                        this.PINW = .5;
                        this.PCRW = 1.5;
                        this.PBTW = 2.5;
                        this.PSTW = .5;
                        this.PAWW = 1.5;
                        break;
                    }
                case 2:
                    {
                        this.RateHigh = 73;
                        this.PCAW = 3;
                        this.PPBW = .5;
                        this.PRBW = 4;
                        this.PACW = 1;
                        this.PSEW = 1;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PCRW = 1;
                        this.PBTW = 1;
                        this.PSTW = 1;
                        this.PAWW = 1.5;
                        break;
                    }
                case 3:
                    {
                        this.RateHigh = 93;
                        this.PCAW = 3;
                        this.PACW = 1.5;
                        this.PSEW = 1.5;
                        this.PAGW = 1.5;
                        this.PINW = .5;
                        this.PBTW = .5;
                        this.PSTW = .5;
                        this.PJUW = 1;
                        this.PAWW = 1.5;
                        break;
                    }
                case 4:
                    {
                        this.RateHigh = 77;
                        this.PCAW = 2;
                        this.PPBW = .5;
                        this.PRBW = 2;
                        this.PACW = .5;
                        this.PSEW = 1;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PBTW = .5;
                        this.PSTW = 1;
                        this.PAWW = 1;
                        break;
                    }
                case 5:
                case 9:
                    {
                        this.RateHigh = 92;
                        this.PPBW = 3;
                        this.PRBW = 2.5;
                        this.PACW = .5;
                        this.PSEW = .5;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PSTW = 2;
                        this.PAWW = 2;
                        break;
                    }
                case 6:
                case 7:
                case 8:
                    {
                        this.RateHigh = 88;
                        this.PPBW = 2;
                        this.PRBW = 3;
                        this.PACW = 1;
                        this.PSEW = 1;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PSTW = 2;
                        this.PAWW = 2;
                        break;
                    }
                case 10:
                case 11:
                    {
                        this.RateHigh = 84;
                        this.PACW = 1;
                        this.PTCW = 1.5;
                        this.PSEW = 1;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PSTW = 1;
                        this.PAWW = .5;
                        break;
                    }
                case 12:
                    {
                        this.RateHigh = 86;
                        this.PACW = 1.5;
                        this.PTCW = 2.5;
                        this.PSEW = 1;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PSTW = 3;
                        this.PAWW = 2;
                        break;
                    }
                case 13:
                case 15:
                    {
                        this.RateHigh = 86;
                        this.PCAW = .5;
                        this.PACW = .5;
                        this.PTCW = 2;
                        this.PSEW = 1.5;
                        this.PAGW = .5;
                        this.PSTW = 1;
                        this.PAWW = 1.5;
                        break;
                    }
                case 14:
                    {
                        this.RateHigh = 90;
                        this.PACW = 1;
                        this.PTCW = 3;
                        this.PSEW = .5;
                        this.PAGW = 1;
                        this.PINW = .5;
                        this.PSTW = 2;
                        this.PAWW = 3;
                        break;
                    }
                case 16:
                    {
                        this.RateHigh = 88.5;
                        this.PCAW = 2;
                        this.PACW = 1.5;
                        this.PTCW = 1;
                        this.PSEW = 2.5;
                        this.PAGW = 1;
                        this.PINW = .5;
                        this.PSTW = .5;
                        this.PJUW = 1;
                        this.PAWW = 2.5;
                        break;
                    }
                case 17:
                    {
                        this.RateHigh = 85;
                        this.PCAW = 2;
                        this.PACW = 1.5;
                        this.PTCW = 1.5;
                        this.PSEW = 2;
                        this.PAGW = .5;
                        this.PINW = .5;
                        this.PSTW = .5;
                        this.PJUW = 1;
                        this.PAWW = 3;
                        break;
                    }
                case 18:
                    {
                        this.RateHigh = 84;
                        this.PCAW = 2;
                        this.PACW = 1;
                        this.PTCW = 2;
                        this.PSEW = 2;
                        this.PAGW = 1;
                        this.PINW = 0;
                        this.PSTW = 1;
                        this.PJUW = .5;
                        this.PAWW = 3;
                        break;
                    }
                case 19:
                    {
                        this.RateHigh = 93;
                        this.RateLow = 60;
                        this.PKAW = 3.5;
                        this.PKPW = 3;
                        this.PINW = 0;
                        this.PAWW = .5;
                        break;
                    }
                case 20:
                    {
                        this.RateHigh = 92.5;
                        this.RateLow = 60;
                        this.PINW = 0;
                        this.PKAW = 3;
                        this.PKPW = 3.5;
                        this.PAWW = .5;
                        break;
                    }

                default:
                    break;
            }

            SetTotalWeight((int)MaddenFileVersion.Ver2008);
            SetRatings();
        }

        public ovrdef(OverallRecord play)
        {
            Ratings = new Dictionary<int, double>();
            RateHigh = play.RatingHigh;
            RateLow = play.RatingLow;
            PPOS = play.Position;

            PACW = play.Acceleration;
            PAGW = play.Agility;
            PAWW = play.Awareness;
            PBTW = play.BreakTackle;
            PCAW = play.Catch;
            PCRW = play.Carry;
            PINW = play.Injury;
            PJUW = play.Jump;
            PKAW = play.KickAccuracy;
            PKPW = play.KickPower;
            PKRW = play.KickReturn;
            PPBW = play.PassBlock;
            PRBW = play.RunBlock;
            PSEW = play.Speed;
            PSTW = play.Strength;
            PTAW = play.ThrowAccuracy;
            PTCW = play.Tackle;
            PTPW = play.ThrowPower;

            SetTotalWeight((int)MaddenFileVersion.Ver2008);
            SetRatings();
        }

        public void SetTotalWeight(int ver)
        {
            if (ver <= (int)MaddenFileVersion.Ver2008)
            {
                totalweight = PCAW + PKAW + PTAW + PPBW + PRBW + PACW + PTCW +
                PSEW + PAGW + PKPW + PTPW + PCRW + PKRW + PBTW + PSTW + PJUW + PAWW;
            }
            else
            {
                totalweight = PACW + PAGW + PAWW + PBKW + PBSW + PBTW + PCAW + PCIW + PCNW + PCRW
                    + PDRW + PHTW + PIJW + PINW + PJUW + PKAW + PKPW + PKRW + PLBW + PLEW
                    + PLFW + PLIW + PLJW + PLPW + PLRW + PLSW + PLTW + PLUW + PLWW + PMCW + PMRW
                    + PPBW + PPEW + PPMW + PPRW + PPWA + PPWW + PRBW + PRDH + PRDL + PRLW + PRRW
                    + PRSW + PSAW + PSEW + PSHW + PSTW + PTAW + PTCW + PTDW + PTMW + PTOW + PTPW
                    + PTRW + PTSW + PUPW + PYSW + PZCW + SRRW;
            }
        }

        public void SetRatings()
        {
            Ratings.Clear();
            Ratings.Add(0, this.PSTW); //strength
            Ratings.Add(1, this.PAGW); //agiilty
            Ratings.Add(2, this.PSEW); //speed
            Ratings.Add(3, this.PACW); //acceleration
            Ratings.Add(4, this.PAWW); //awareness
            Ratings.Add(5, this.PCAW); //catch
            Ratings.Add(6, this.PCRW); //carry
            Ratings.Add(7, this.PTPW); //throw power
            Ratings.Add(8, this.PTAW); //throw accuracy
            Ratings.Add(9, this.PKPW); //kick power
            Ratings.Add(10, this.PKAW); //kick accuracy
            Ratings.Add(11, this.PBTW); //breake tackle
            Ratings.Add(12, this.PTCW); //tackle
            Ratings.Add(13, this.PPBW); //pass block
            Ratings.Add(14, this.PRBW); //run block
            Ratings.Add(15, this.PJUW); //jump
            Ratings.Add(16, this.PKRW); //kick return
            Ratings.Add(18, this.PINW); //injury
            Ratings.Add(19, this.PBKW); // NA
            Ratings.Add(20, this.PPWA); // NA
            Ratings.Add(21, this.PTDW); // NA
            Ratings.Add(22, this.PTMW); // NA
            Ratings.Add(23, this.PTRW); // NA
            Ratings.Add(24, this.PTSW); // NA
            Ratings.Add(25, this.PUPW); // NA
            Ratings.Add(26, this.PLTW); // trucking
            Ratings.Add(27, this.PLEW); // Change of Direction
            Ratings.Add(28, this.PLJW); // juke
            Ratings.Add(29, this.PLPW); // spin move
            Ratings.Add(30, this.PLSW); // stiff arm
            Ratings.Add(31, this.PLBW); // vision
            Ratings.Add(32, this.SRRW); // short route
            Ratings.Add(33, this.PBTW); // NA
            Ratings.Add(34, this.PCIW); // catch in traffic
            Ratings.Add(35, this.PRLW); // spec catch
            Ratings.Add(36, this.PLIW); // impact block
            Ratings.Add(37, this.PLKW); // lead block
            Ratings.Add(38, this.PLRW); // run block strength
            Ratings.Add(39, this.PLWW); // run block footwork
            Ratings.Add(40, this.PPWW); // pass block footwork
            Ratings.Add(41, this.PYSW); // pass block strength
            Ratings.Add(42, this.PDRW); // deep route
            Ratings.Add(43, this.PMRW); // medium route
            Ratings.Add(44, this.PSHW); // release for WR
            Ratings.Add(45, this.PBSW); // block shedding
            Ratings.Add(46, this.PHTW); // Hit power
            Ratings.Add(47, this.PLFW); // Finesse Moves
            Ratings.Add(48, this.PPMW); // Power Moves
            Ratings.Add(49, this.PPRW); // play recognition
            Ratings.Add(50, this.PLUW); // pursuit
            Ratings.Add(51, this.PMCW); // man cover
            Ratings.Add(52, this.PZCW); // zone cover
            Ratings.Add(53, this.PKRW); // kick return
            Ratings.Add(54, this.PTAD); // throw deep
            Ratings.Add(55, this.PBSK); // break sack
            Ratings.Add(56, this.PTAM); // throw medium
            Ratings.Add(57, this.PTAS); // throw short
            Ratings.Add(58, this.PBCV); // ball carrier vision
            Ratings.Add(59, this.PPLA); // play action
            Ratings.Add(60, this.PLPE); // Press Coverage
            Ratings.Add(61, this.PTUP); // throw under pressure
            Ratings.Add(62, this.PTOR); // throw on run
        }

        public double GetPerc(int trait, double rate)
        {
            if (this.Ratings[trait] == 0)
                return 0;
            double median = (this.RateHigh + this.RateLow) / 2;
            double point = 100 / (this.RateHigh - this.RateLow);
            double perc = point * this.Ratings[trait] / totalweight;
            double result = (rate - median) * perc;

            return result;
        }

        public double GetOverall(PlayerRecord rec)
        {
            double total = 50;

            double str = GetPerc((int)Rating.STR, rec.GetRating((int)Rating.STR));
            double agi = GetPerc((int)Rating.AGI, rec.GetRating((int)Rating.AGI));
            double spd = GetPerc((int)Rating.SPD, rec.GetRating((int)Rating.SPD));
            double acc = GetPerc((int)Rating.ACC, rec.GetRating((int)Rating.ACC));
            double awr = GetPerc((int)Rating.AWR, rec.GetRating((int)Rating.AWR));
            double cth = GetPerc((int)Rating.CTH, rec.GetRating((int)Rating.CTH));
            double car = GetPerc((int)Rating.CAR, rec.GetRating((int)Rating.CAR));
            double thp = GetPerc((int)Rating.THP, rec.GetRating((int)Rating.THP));
            double tha = GetPerc((int)Rating.THA, rec.GetRating((int)Rating.THA));
            double kpw = GetPerc((int)Rating.KPW, rec.GetRating((int)Rating.KPW));
            double kac = GetPerc((int)Rating.KAC, rec.GetRating((int)Rating.KAC));
            double btk = GetPerc((int)Rating.BTK, rec.GetRating((int)Rating.BTK));
            double tak = GetPerc((int)Rating.TAK, rec.GetRating((int)Rating.TAK));
            double pbk = GetPerc((int)Rating.PBK, rec.GetRating((int)Rating.PBK));
            double rbk = GetPerc((int)Rating.RBK, rec.GetRating((int)Rating.RBK));
            double jmp = GetPerc((int)Rating.JMP, rec.GetRating((int)Rating.JMP));

            double physical = str + agi + spd + acc + thp + jmp + kpw;
            //physical = Math.Truncate(physical);


            total += physical + awr + cth + car + btk + tak + pbk + rbk + kac + tha;
            //double round = total - Math.Truncate(total);
            //total = Math.Truncate(total);
            //if (round > .50)
            //    total++;
            return total;
        }

        public double GetOverall19(PlayerRecord rec)
        {
            double total = 50;

            double str = GetPerc((int)Rating.STR, rec.GetRating((int)Rating.STR));
            double agi = GetPerc((int)Rating.AGI, rec.GetRating((int)Rating.AGI));
            double spd = GetPerc((int)Rating.SPD, rec.GetRating((int)Rating.SPD));
            double acc = GetPerc((int)Rating.ACC, rec.GetRating((int)Rating.ACC));
            double awr = GetPerc((int)Rating.AWR, rec.GetRating((int)Rating.AWR));
            double cth = GetPerc((int)Rating.CTH, rec.GetRating((int)Rating.CTH));
            double car = GetPerc((int)Rating.CAR, rec.GetRating((int)Rating.CAR));
            double thp = GetPerc((int)Rating.THP, rec.GetRating((int)Rating.THP));
            double tha = GetPerc((int)Rating.THA, rec.GetRating((int)Rating.THA));
            double kpw = GetPerc((int)Rating.KPW, rec.GetRating((int)Rating.KPW));
            double kac = GetPerc((int)Rating.KAC, rec.GetRating((int)Rating.KAC));
            double btk = 0;
            // s68 have to reverse this fieldname for big endian database
            if (rec.ContainsIntField("TKBP") || rec.ContainsIntField("PBKT"))
                btk = GetPerc((int)Rating.BTK, rec.GetRating((int)Rating.BKT));
            else btk = GetPerc((int)Rating.BTK, rec.GetRating((int)Rating.BTK));
            double tak = GetPerc((int)Rating.TAK, rec.GetRating((int)Rating.TAK));
            double pbk = GetPerc((int)Rating.PBK, rec.GetRating((int)Rating.PBK));
            double rbk = GetPerc((int)Rating.RBK, rec.GetRating((int)Rating.RBK));
            double jmp = GetPerc((int)Rating.JMP, rec.GetRating((int)Rating.JMP));
            double bks = GetPerc((int)Rating.BKS, rec.GetRating((int)Rating.BKS));
            double pwa = GetPerc((int)Rating.PWA, rec.GetRating((int)Rating.PWA));
            double thd = GetPerc((int)Rating.THD, rec.GetRating((int)Rating.THD));
            double thm = GetPerc((int)Rating.THM, rec.GetRating((int)Rating.THM));
            double tor = GetPerc((int)Rating.TOR, rec.GetRating((int)Rating.TOR));
            double ths = GetPerc((int)Rating.THS, rec.GetRating((int)Rating.THS));
            double tup = GetPerc((int)Rating.TUP, rec.GetRating((int)Rating.TUP));
            double tru = GetPerc((int)Rating.TRU, rec.GetRating((int)Rating.TRU));
            double elu = GetPerc((int)Rating.ELU, rec.GetRating((int)Rating.ELU));
            double juk = GetPerc((int)Rating.JUK, rec.GetRating((int)Rating.JUK));
            double spn = GetPerc((int)Rating.SPN, rec.GetRating((int)Rating.SPN));
            double sfa = GetPerc((int)Rating.SFA, rec.GetRating((int)Rating.SFA));
            double vis = GetPerc((int)Rating.VIS, rec.GetRating((int)Rating.VIS));
            double srr = GetPerc((int)Rating.SRR, rec.GetRating((int)Rating.SRR));
            double cit = GetPerc((int)Rating.CIT, rec.GetRating((int)Rating.CIT));
            double spc = GetPerc((int)Rating.SPC, rec.GetRating((int)Rating.SPC));
            double ibk = GetPerc((int)Rating.IBK, rec.GetRating((int)Rating.IBK));
            double lbk = GetPerc((int)Rating.LBK, rec.GetRating((int)Rating.LBK));
            double rbs = GetPerc((int)Rating.RBS, rec.GetRating((int)Rating.RBS));
            double rbf = GetPerc((int)Rating.RBF, rec.GetRating((int)Rating.RBF));
            double pbf = GetPerc((int)Rating.PBF, rec.GetRating((int)Rating.PBF));
            double pbs = GetPerc((int)Rating.PBS, rec.GetRating((int)Rating.PBS));
            double drr = GetPerc((int)Rating.DRR, rec.GetRating((int)Rating.DRR));
            double mrr = GetPerc((int)Rating.MRR, rec.GetRating((int)Rating.MRR));
            double rel = GetPerc((int)Rating.REL, rec.GetRating((int)Rating.REL));
            double shd = GetPerc((int)Rating.SHD, rec.GetRating((int)Rating.SHD));
            double hit = GetPerc((int)Rating.HIT, rec.GetRating((int)Rating.HIT));
            double fin = GetPerc((int)Rating.FNM, rec.GetRating((int)Rating.FNM));
            double pow = GetPerc((int)Rating.PWM, rec.GetRating((int)Rating.PWM));
            double plr = GetPerc((int)Rating.PLR, rec.GetRating((int)Rating.PLR));
            double pur = GetPerc((int)Rating.PUR, rec.GetRating((int)Rating.PUR));
            double man = GetPerc((int)Rating.MAN, rec.GetRating((int)Rating.MAN));
            double zon = GetPerc((int)Rating.ZON, rec.GetRating((int)Rating.ZON));
            double krr = GetPerc((int)Rating.KRR, rec.GetRating((int)Rating.KRR));

            total += str + agi + spd + acc + thp + jmp + kpw + awr + cth + car + btk + tak + pbk + rbk + kac + tha;
            total += bks + pwa + thd + thm + tor + ths + tup + tru + elu + juk + spn + sfa + vis + srr + cit + spc;
            total += ibk + lbk + rbs + rbf + pbf + pbs + drr + mrr + rel;
            total += shd + hit + fin + pow + plr + pur + man + zon + krr;

            //double round = total - Math.Truncate(total);
            total = Math.Truncate(total);
            //if (round > .50)
            //    total++;
            return total;
        }

    }



    public class Overall
    {
        public Dictionary<int, ovrdef> Table;
        public List<ovrdef> OVR19;

        public Overall()
        {
            Table = new Dictionary<int, ovrdef>();
            OVR19 = new List<ovrdef>();
        }

        public void InitRatings(MGMT man)
        {
            if (man.db_misc_model != null)
            {
                Table.Clear();
                foreach (TableRecordModel rec in man.db_misc_model.TableModels[EditorModel.PLAYER_OVERALL_CALC].GetRecords())
                {
                    OverallRecord ovr = (OverallRecord)rec;
                    Table.Add(ovr.Position, new ovrdef(ovr));
                }
            }
            else
            {
                for (int p = 0; p <= 20; p++)
                {
                    Table.Add(p, new ovrdef(p));
                }
            }
        }

        public void InitRatings19()
        {
            OVR19.Clear();

            for (int pos = 0; pos <= 33; pos++)
            {
                int start = 0;
                int end = 0;
                #region Get start/end types for each position
                switch (pos)
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
                    case (int)MaddenPositions.KR:
                        {
                            start = 12;
                            end = 12;
                        }
                        break;
                    case (int)MaddenPositions.PR:
                        {
                            start = 12;
                            end = 12;
                        }
                        break;
                    case (int)MaddenPositions.KOS:
                        {
                            start = 45;
                            end = 45;
                        }
                        break;
                    case (int)MaddenPositions.LS:
                        {
                            start = 18;
                            end = 18;
                        }
                        break;
                    case (int)MaddenPositions.TDB:
                        {
                            start = 6;
                            end = 6;
                        }
                        break;
                    case (int)MaddenPositions2019.PHB:
                        {
                            start = 4;
                            end = 4;
                        }
                        break;
                    case (int)MaddenPositions2019.SWR:
                        {
                            start = 12;
                            end = 12;
                        }
                        break;
                    case (int)MaddenPositions2019.RLE:
                        {
                            start = 31;
                            end = 31;
                        }
                        break;
                    case (int)MaddenPositions2019.RRE:
                        {
                            start = 31;
                            end = 31;
                        }
                        break;
                    case (int)MaddenPositions2019.RDT:
                        {
                            start = 26;
                            end = 26;
                        }
                        break;
                    case (int)MaddenPositions2019.SLB:
                        {
                            start = 33;
                            end = 33;
                        }
                        break;
                    case (int)MaddenPositions2019.SCB:
                        {
                            start = 39;
                            end = 39;
                        }
                        break;
                }
                #endregion

                for (int type = start; type <= end; type++)
                    OVR19.Add(new ovrdef(pos, type));
            }
        }

        public double GetOverall(PlayerRecord rec)
        {
            return Table[rec.PositionId].GetOverall(rec);
        }

        public double GetOverall19(PlayerRecord rec, int pos, int type)
        {
            if (rec.PositionId == pos && type == -1)
                return (double)rec.Overall;

            double ovr = 99;

            foreach (ovrdef overdef in OVR19)
            {
                if (type == -1)
                {
                    if (overdef.PPOS == pos)
                    {
                        double check = overdef.GetOverall19(rec);
                        if (check < ovr)
                            ovr = check;
                    }
                }
                else
                {
                    if (overdef.PPOS == pos && overdef.PLTY == type)
                        return overdef.GetOverall19(rec);
                }
            }

            if (type == -1)
                return ovr;
            else return 0;
        }

        public double GetRatingOVR(PlayerRecord rec, int rating)
        {
            return Table[rec.PositionId].GetPerc(rating, rec.GetRating(rating));
        }
        public double GetRatingOVR19(PlayerRecord rec, int rating)
        {
            return Table[rec.PlayerType].GetPerc(rating, rec.GetRating(rating));
        }

        internal object GetOverall19(PlayerRecord currentPlayerRecord, int positionId, object type)
        {
            throw new NotImplementedException();
        }
    }
}

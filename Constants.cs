/*
This file is part of BK_Globalizer.

    BK_Globalizer is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    BK_Globalizer is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKGlobalize.Classes
{
    static class Constants
    {
        public const int JALCMD = 3;
        public const int LUICMD = 15;
        public const int CMDLENGTH = 4;

        public static class Register
        {
            public const int AT = 1;
            public const int V0 = 2;
            public const int V1 = 3;
        }

        public static class LevelID
        {
            public const int CC = 0;
            public const int MMM = 1;
            public const int GV = 2;
            public const int TTC = 3;
            public const int MM = 4;
            public const int BGS = 5;
            public const int RBB = 6;
            public const int FP = 7;
            public const int SM = 8;
            public const int CS = 9;
            public const int GL = 10;
            public const int FB = 11;
            public const int CCW = 12;
        }

        public static class LevelAsmAddress
        {
            public const int CC = 0xFA3FD0;
            public const int MMM = 0xFA5F50;
            public const int GV = 0xFA9150;
            public const int TTC = 0xFAE860;
            public const int MM = 0xFB24A0;
            public const int BGS = 0xFB44E0;
            public const int RBB = 0xFB9A30;
            public const int FP = 0xFBEBE0;
            public const int SM = 0xFC4810;
            public const int CS = 0xFC6F20;
            public const int GL = 0xFC9150;
            public const int FB = 0xFD0420;
            public const int CCW = 0xFD6190;

            public static int[] laa = { CC, MMM, GV, TTC, MM, BGS, RBB, FP, SM, CS, GL, FB, CCW };
        }

        public static class AsmGlobalsFuncsOffsets
        {
            public static int[] cc = { 0x9BA98, 0x9BAC0, 0x9BA70, 0x9BA50 };
            public static int[] mmm = { 0x9B9D0, 0x9BA00, 0x9B9A0, 0x9BA30 };
            public static int[] gv = { 0x9BB3C, 0x9BB74, 0x9BB04 };
            public static int[] ttc = { 0xABBE8, 0x9BC08, 0x9BBC8 };
            public static int[] mm = { };
            public static int[] bgs = { };
            public static int[] rbb = { };
            public static int[] fp = { 0x9BE58, 0x9BE90, 0x9BE20 };
            public static int[] sm = { };
            public static int[] cs = { };
            public static int[] gl = { 0x9BCA8, 0x9BCC8, 0x9BC88, 0x9BC68 };
            public static int[] fb = { };
            public static int[] ccw = { };
            public static int[][] afo = { cc, mmm, gv, ttc, mm, bgs, rbb, fp, sm, cs, gl, fb, ccw };
        }

        public static class VariableSpaceConstants
        {
            public static byte[] cc = { 0x00, 0x0D, 0x46, 0xA1, 0x87, 0x1B, 0xA4, 0x3E, 0x00, 0x00, 0xE1, 0x1D, 0xFF, 0x62, 0xC2, 0xB8 };
            public static byte[] mmm = { 0x00, 0x14, 0x60, 0x61, 0xEC, 0x79, 0xD7, 0xB4, 0x00, 0x01, 0xBA, 0xD4, 0x94, 0x94, 0xD8, 0x65 };
            public static byte[] gv = { 0x00, 0x27, 0x45, 0x30, 0xAA, 0x18, 0xBB, 0xF3, 0x00, 0x03, 0x03, 0x3E, 0x93, 0x4A, 0x83, 0xF1 };
            public static byte[] ttc = { 0x00, 0x16, 0xD2, 0xFD, 0xFB, 0x70, 0xB0, 0x1D, 0x00, 0x04, 0xF1, 0x4B, 0xD2, 0x2F, 0xFF, 0xD8 };
            public static byte[] mm = { 0x00, 0x0C, 0x74, 0x0C, 0xCD, 0x24, 0x9C, 0xB3, 0x00, 0x00, 0xD5, 0x72, 0xD3, 0x81, 0xB7, 0x2F };
            public static byte[] bgs = { 0x00, 0x28, 0x2B, 0x61, 0xCC, 0xDA, 0xEE, 0xA0, 0x00, 0x02, 0xDF, 0xB6, 0xAD, 0x7F, 0xF2, 0xF3 };
            public static byte[] rbb = { 0x00, 0x25, 0xF8, 0x32, 0xED, 0xD2, 0x84, 0x39, 0x00, 0x03, 0x80, 0xF0, 0xDE, 0xFE, 0xF6, 0x92 };
            public static byte[] fp = { 0x00, 0x2A, 0x7A, 0xA5, 0x88, 0x88, 0xDE, 0xDD, 0x00, 0x05, 0x20, 0x10, 0xD8, 0x84, 0xD8, 0xAC, 
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0xC4, 0x58, 0x40, 0x00, 0x46, 0x0B, 0x1C, 0x00, 0xC2, 0x5C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC4, 0x1C, 0x40, 0x00,
                                        0x43, 0xE9, 0x00, 0x00, 0xC2, 0xDE, 0x00, 0x00 };
            public static byte[] sm = { 0x00, 0x0F, 0xE2, 0xC1, 0x8C, 0x09, 0x92, 0xD1, 0x00, 0x01, 0xEC, 0x98, 0xAD, 0x01, 0x9D, 0x3C };
            public static byte[] cs = { 0x00, 0x1C, 0x80, 0xC5, 0xF6, 0x52, 0xA2, 0x6A, 0x00, 0x04, 0x33, 0x5E, 0xF5, 0x1C, 0xDB, 0x53 };
            public static byte[] gl = { 0x00, 0x2F, 0xEB, 0xBA, 0xE9, 0x49, 0xC2, 0x51, 0x00, 0x06, 0x72, 0xE7, 0xCF, 0xAD, 0xA2, 0x90 };
            public static byte[] fb = { 0x00, 0x2B, 0xA0, 0xF7, 0xD8, 0xB3, 0x8D, 0x25, 0x00, 0x05, 0x48, 0xB7, 0xDD, 0xC3, 0xA7, 0x24 };
            public static byte[] ccw = { 0x00, 0x21, 0x3B, 0x1A, 0xB6, 0x54, 0xDD, 0xE8, 0x00, 0x04, 0x0A, 0x16, 0xC3, 0xA6, 0x88, 0x32 };

            public static byte[][] vsc = { cc, mmm, gv, ttc, mm, bgs, rbb, fp, sm, cs, gl, fb, ccw}; 
        }

        public static class LevelObjectsAddress
        {
            public const int CC = 0xFA5D96;
            public const int MMM = 0xFA8CE6;
            public const int GV = 0xFAE27E;
            public const int TTC = 0xFB1AEB;
            public const int MM = 0xFB42D9;
            public const int BGS = 0xFB9610;
            public const int RBB = 0xFBE5E2;
            public const int FP = 0xFC3FEF;
            public const int SM = 0xFC6C0F;
            public const int CS = 0xFC8AFC;
            public const int GL = 0xFCF698;
            public const int FB = 0xFD5A60;
            public const int CCW = 0xFDA2FF;
            public const int END = 0xFDAA10;

            public static int[] loa = { CC, MMM, GV, TTC, MM, BGS, RBB, FP, SM, CS, GL, FB, CCW, END };
        }

        public static class LoadOps
        {
            public const int LDL = 26;
            public const int LDR = 27; 
            public const int LB = 32;
            public const int LH = 33;
            public const int LWL = 34;
            public const int LW = 35;
            public const int LBU = 36;
            public const int LHU = 37;
            public const int LWR = 38;
            public const int LWU = 39;
            public const int LL = 48;
            public const int LWC1 = 49;
            public const int LLD = 52;
            public const int LDC1 = 53;
            public const int LD = 55;
        }

        public static class ImmediateOps
        {
            public const int ADDIU = 9;
            public const int ORI = 13;
        }

        public static class StoreOps
        {
            public const int SB = 40;
            public const int SH = 41;
            public const int SWL = 42;
            public const int SW = 43;
            public const int SDL = 44;
            public const int SDR = 45;
            public const int SWR = 46;
            public const int SC = 56;
            public const int SWC1 = 57;
            public const int SCD = 60;
            public const int SDC1 = 61;
            public const int SD = 63;

        }
        public static class ObjVarSpace
        {
            public const int CC = 0x90;
            public const int MMM = 0x70;
            public const int GV = 0xF0;
            public const int TTC = 0x50;
            public const int MM = 0x20;
            public const int BGS = 0x20;
            public const int RBB = 0x60;
            public const int FP = 0x6E0;
            public const int SM = 0x10;
            public const int CS = 0x10;
            public const int GL = 0x120;
            public const int FB = 0x1F0;
            public const int CCW = 0x20;
        }

        // Subroutines offsets for loading objects into RAM list.
        public static class AsmSubOffsets
        {
            public const uint CC = 0x19B0;
            public const uint MMM = 0x2CF0;
            public const uint GV = 0x8D64;
            public const uint TTC = 0x26D0;
            public const uint MM = 0x24C0;
            public const uint BGS = 0x8DF0;
            public const uint RBB = 0x858;
            public const uint FP = 0xAF34;
            public const uint SM = 0x420;
            public const uint CS = 0x60F0;
            public const uint GL = 0x3CD4;
            public const uint FB = 0;
            public const uint CCW = 0x777C;
        }

    }
}

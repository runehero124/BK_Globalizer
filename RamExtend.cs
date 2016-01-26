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
using BKGlobalize.Classes;

namespace BKGlobalize
{
    public static class RamExtend
    {
        public static void MoveBackgroundRAM(byte[] F37F90, byte[] F9CAE0, uint decompressedRam, uint compressedRam)
        {
            uint upperCompressed = compressedRam >> 16;
            uint lowerCompressed = (compressedRam & 0x0000FFFF);

            uint upperDecompressed = decompressedRam >> 16;
            uint lowerDecompressed = (decompressedRam & 0x0000FFFF);

            int fileOffset = 0x7520;

            // Hook to Background loading routine.
            ASM.WriteCommand(F37F90, 0x83124, 0x0C0DAAAC);                                   // JAL 8036AAB0

            // Custom Background Loader Subroutine.
            ASM.WriteCommand(F9CAE0, fileOffset, 0x3C088038);                                // LUI	    T0, 0x8038
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x25083CC4);                           // AddIU	T0, T0, 0x3CC4
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x8D080000);                           // LW	    T0, 0x0000(T0)			        ; RAM Offset of File Lookup Table

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x000448C0);                           // SLL	    T1, A0, 3			            ; Multiply Background Id by 8 to get position in lookup table
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x01285021);                           // ADDU	T2, T1, T0			            ; Ram Pointer to Background ID ROM Offset (from 101C50)
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x8D490000);                           // LW	    T1, 0x0000 (T2)			        ; Address from 101C50 of Background
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x8D480008);                           // LW	    T0, 0x0008 (T2)			        ; Address from 101C50 of next file
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x01093022);                           // SUB	    A2, T0, T1			            ; Compressed Size

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x3C0A8038);                           // LUI	    T2, 0x8038
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x354A3CCC);                           // ORI  	T2, T2, 0x3CCC
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x8D4A0000);                           // LW	    T2, 0x0000 (T2)			        ; Store 0x101C50 to REG T2

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x3C040000 + upperCompressed);         // LUI	    A0, upperCompressed
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x34840000 + lowerCompressed);         // ORI	    A0, A0, lowerCompressed		    ; Where to store compressed data in RAM

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x0C09017C);                           // JAL	    0x802405F0			            ; Subroutine call: Load compressed data
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x012A2821);                           // ADDU	A1, T1, T2			            ; Finally get ROM address

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x3C040000 + upperCompressed);         // LUI	    A0, upperCompressed
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x34840000 + lowerCompressed);         // ORI  	A0, A0, lowerCompressed		    ; Where compressed data is stored (RAM)

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x3C050000 + upperDecompressed);       // LUI	    A1, upperDecompressed

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x0C08F828);                           // JAL	    0x8023E0A0			            ; Subroutine Call: Decompress data
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x34A50000 + lowerDecompressed);       // ORI	    A1, A1, lowerDecompressed       ; Write decompressed data to decompressedRam address

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x3C020000 + upperDecompressed);       // LUI	    V0, upperDecompressed
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x34420000 + lowerDecompressed);       // ORI  	V0, V0, lowerDecompressed	    ; Pointer to the decompressed data

            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x080C282F);                           // J	    0x8030A0BC			            ; Return to caller
            ASM.WriteCommand(F9CAE0, fileOffset += 4, 0x00000000);                           // NOP
        }

        static void MoveSetupsRAM()
        {

        }
    }
}

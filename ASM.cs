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
    static class ASM
    {
        private static class Masks
        {
            public const uint Opcode = 0xFC000000;
            public const uint SourceReg = 0x3E00000;
            public const uint DestReg = 0x1F0000;
            public const uint Base = 0x3E00000;
            public const uint Upper = 0xFFFF0000;
            public const uint Immediate = 0x0000FFFF;
            public const uint JalImmediate = 0x3FFFFFF;
        }

        public static void WriteCommand(byte[] file, int offset, uint command)
        {
            byte[] commandArray = BitConverter.GetBytes(command);
            
            if (BitConverter.IsLittleEndian)
                Array.Reverse(commandArray);

                file[offset] = commandArray[0];
                file[offset + 1] = commandArray[1];
                file[offset + 2] = commandArray[2];
                file[offset + 3] = commandArray[3];
        }

        public static uint GetObjectsRamAddress(byte[] objectsFile, int position)
        {
            byte[] tempArray = new byte[4];
            Array.Copy(objectsFile, position, tempArray, 0, 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempArray);

            uint ramAddress = BitConverter.ToUInt32(tempArray, 0);
            return ramAddress;
        }

        public static uint GetJalImmediate(uint cmd)
        {
            uint immediate = (uint)((cmd & Masks.JalImmediate) * 4);
            return immediate;
        }

        public static uint BuildNewJalCommand(uint jalAddress)
        {
            jalAddress = (uint)((jalAddress & Masks.JalImmediate) / 4);
            jalAddress += (Constants.JALCMD << 26);
            return jalAddress;
        }

        public static byte GetOpcode(uint command)
        {
            command = (command & Masks.Opcode);
            byte op = (byte)(command >> 26);
            return op;
        }

        public static uint GetImmediate(uint command)
        {
            uint immediate = (command & Masks.Immediate);
            return immediate;
        }

        public static byte GetBaseReg(uint command)
        {
            byte baseReg = GetSourceReg(command);
            return baseReg;
        }

        public static byte GetSourceReg(uint command)
        {
            command = (command & Masks.SourceReg);
            byte srcReg = (byte)(command >> 21);
            return srcReg;
        }

        public static byte GetDestReg(uint command)
        {
            command = (command & Masks.DestReg);
            byte dstReg = (byte)(command >> 16);
            return dstReg;
        }

        public static uint GetCommand(byte[] file, int pos)
        {
            uint command = 0;
            byte[] cmdBuffer = new byte[4];
            
            Array.Copy(file, pos, cmdBuffer, 0, 4);
            command = (uint)((cmdBuffer[0] << 24) + (cmdBuffer[1] << 16) 
                + (cmdBuffer[2] << 8) + (cmdBuffer[3]));

            return command;
        }

        public static uint GetCommandAddress(uint luiCmd, uint pairCmd)
        {
            uint cmdAddress = luiCmd << 16;
            cmdAddress = cmdAddress + (pairCmd & Masks.Immediate);

            // Adjust for sign-extended addresses.
            if ((cmdAddress & 0xFFFF) >= 0x8000)
                cmdAddress -= 0x10000;

            return cmdAddress;
        }
        
    }
}

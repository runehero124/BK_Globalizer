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
//using System.Text;
using System.Collections;
//using System.IO;
using BKGlobalize.Classes;
using C = BKGlobalize.Classes.Constants;
using GE = BKGlobalize.GECompress;

namespace BKGlobalize
{
    public class Globalizer
    {
        // Fields.
        private uint _baseRam = 0;
        private uint _baseRom = 0;
        private const uint _BASERAM = 0x803863F0;
        private uint _upperRAM;
        private byte[] _F37F90;
        private byte[] _rom;
        private Array _levelIDArray = Enum.GetValues(typeof(_levelID));
        private uint[] _subOffsets = { C.AsmSubOffsets.CC, C.AsmSubOffsets.MMM, C.AsmSubOffsets.GV,
            C.AsmSubOffsets.TTC, C.AsmSubOffsets.MM, C.AsmSubOffsets.BGS, C.AsmSubOffsets.RBB,
            C.AsmSubOffsets.FP, C.AsmSubOffsets.SM, C.AsmSubOffsets.CS, C.AsmSubOffsets.GL,
            C.AsmSubOffsets.FB, C.AsmSubOffsets.CCW };
        
        private List<byte> _loadOpsList = new List<byte> { C.LoadOps.LB, C.LoadOps.LH, C.LoadOps.LW, 
            C.LoadOps.LBU, C.LoadOps.LHU, C.LoadOps.LWU, C.LoadOps.LWC1, C.LoadOps.LDC1, C.LoadOps.LD,
            C.LoadOps.LDL, C.LoadOps.LDR, C.LoadOps.LL, C.LoadOps.LLD, C.LoadOps.LWL, C.LoadOps.LWR,
            C.LoadOps.LWU};
        private List<byte> _storeOpsList = new List<byte> { C.StoreOps.SB, C.StoreOps.SC, C.StoreOps.SCD,
            C.StoreOps.SD, C.StoreOps.SDC1, C.StoreOps.SDL, C.StoreOps.SDR, C.StoreOps.SH, C.StoreOps.SW,
            C.StoreOps.SWC1, C.StoreOps.SWL, C.StoreOps.SWR };
        private List<byte> _immediateOpsList = new List<byte> { C.ImmediateOps.ADDIU, C.ImmediateOps.ORI };

        private Queue _asmFiles = new Queue();
        private Queue _objectsFiles = new Queue();
        private Queue _variableSpaceArrays = new Queue();

        private List<byte[]> _fileBuffer = new List<byte[]>();
        private ArrayList _subOffArray = new ArrayList();

        private GE.GECompression _decompressor = new GE.GECompression();

        // Constructor.
        public Globalizer(uint baseRam, uint baseRom, byte[] romFile)
        {
            _decompressor.SetGame(GE.GECompression.BANJOKAZOOIE);
            this._baseRam = baseRam;
            this._baseRom = baseRom;

            foreach (int id in _levelIDArray)
            {
                // Instantiate Level File Objects
                _asmFiles.Enqueue(new AsmFile(id, C.LevelAsmAddress.laa[id], C.LevelObjectsAddress.loa[id]));
                _objectsFiles.Enqueue(new ObjectsFile(id, C.LevelObjectsAddress.loa[id], C.LevelObjectsAddress.loa[id + 1]));

                // Instantiate Object Variable Arrays.
                _variableSpaceArrays.Enqueue(C.VariableSpaceConstants.vsc[id]);
            }

            // Copy romFile contents to class _rom field.
            _rom = new byte[romFile.Length];

            for (int i = 0; i < romFile.Length; i++)
            {
                _rom[i] = romFile[i];
            }

            // Setup buffer for F37F90 file.
            var globalsAsm = new AsmFile(1, 0xF37F90, 0xFA3FD0);
            byte[] temp = DecompressFile(globalsAsm);
            _F37F90 = new byte[temp.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                _F37F90[i] = temp[i];
            }
        }

        // Enums.
        private enum _levelID { CC, MMM, GV, TTC, MM, BGS, RBB, FP, SM, CS, GL, FB, CCW }
 
        private void CopyArray(byte[] source, byte[] dest)
        {
            for (int i = 0; i < dest.Length; i++)
            {
                dest[i] = source[i];
            }
        }

        // Methods.
        public void BeginUpdating(out byte[] f37f90, out byte[] assets)
        {

            // Iterate through each levels' asm and objects files.
            while (_asmFiles.Count > 0)
            {
                UpdateAsmFile((AsmFile)_asmFiles.Dequeue());
                UpdateObjectsFile((ObjectsFile)_objectsFiles.Dequeue());
            }

            // Writes everything to a file.
            WriteFile(out assets);

            // Update pointers in 0xF37F90.
            UpdateGlobalsPointers(_F37F90);

            // Fill F37F90 output buffer.
            f37f90 = new byte[_F37F90.Length];
            CopyArray(_F37F90, f37f90);

            // Null non-constant array fields.
            _F37F90 = null;
            _rom = null;
            RomFile.TotalOffset = 0;
        }

        private void UpdateGlobalsPointers(byte[] buffer)
        {
            uint upperRam = (uint)((_baseRam & 0xFFFF0000) >> 16),
                lowerRam = (uint)(_baseRam & 0x0000FFFF),
                upperRom = (uint)((_baseRom & 0xFFFF0000) >> 16),
                lowerRom = (uint)(_baseRom & 0x0000FFFF),
                upperLength = 0x0007,
                lowerLength = 0x54B0;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\F37F90.bin";

            int cmdAddress = 0x3C8D0;

            // Clear space for writing ASM commands.
            for (int i = 0x3C894; i <= 0x3C960; i++)
                buffer[i] = 0;

            // Load ROM file.
            ASM.WriteCommand(buffer, 0x3C894, (0x3C06 << 16) + upperLength);
            ASM.WriteCommand(buffer, 0x3C898, (0x34C6 << 16) + lowerLength);
            ASM.WriteCommand(buffer, 0x3C89C, (0x3C04 << 16) + upperRam);
            ASM.WriteCommand(buffer, 0x3C8A0, (0x3484 << 16) + lowerRam);
            ASM.WriteCommand(buffer, 0x3C8A4, (0x3C05 << 16) + upperRom);
            ASM.WriteCommand(buffer, 0x3C8A8, 0x0C09017C);
            ASM.WriteCommand(buffer, 0x3C8AC, (0x34A5 << 16) + lowerRom);
            

            // Fix hardcoded checksum for Bottles Spiral Mountain.
            ASM.WriteCommand(buffer, 0x3C8B0, 0x3C04803F);
            ASM.WriteCommand(buffer, 0x3C8B4, 0x3484FE00);
            ASM.WriteCommand(buffer, 0x3C8B8, 0x3C0500E7);
            ASM.WriteCommand(buffer, 0x3C8BC, 0x34A59996);
            ASM.WriteCommand(buffer, 0x3C8C0, 0xAC850000);
            ASM.WriteCommand(buffer, 0x3C8C4, 0x3C05ADC4);
            ASM.WriteCommand(buffer, 0x3C8C8, 0x34A5AE5A);
            ASM.WriteCommand(buffer, 0x3C8CC, 0xAC850004);

            // Write JAL commands to level routines.
            foreach (uint i in _subOffArray)
            {                
                uint jalAddress  = (i & 0x03FFFFFF) / 4;
                ASM.WriteCommand(buffer, cmdAddress, (0x03 << 26) + jalAddress);
                cmdAddress += 8;
            }
        }

        private void WriteFile(out byte[] objectsBuffer)
        {
            objectsBuffer = new byte[_fileBuffer.SelectMany(byteArr => byteArr).ToArray().Length];
            Array.Copy(_fileBuffer.SelectMany(byteArr => byteArr).ToArray(), objectsBuffer, objectsBuffer.Length);
        }

        private void UpdateObjectsFile(ObjectsFile fileObj)
        {
            byte[] objectsFile = DecompressFile(fileObj);
            byte[] reservedSpace = new byte[fileObj.ReservedLength];
            byte[] reservedSpaceConstants = (byte[])_variableSpaceArrays.Dequeue();

            // Scan for RAM addresses.
            for (int i = 0; i < objectsFile.Length; i += 4)
            {
                uint ramAddress = ASM.GetObjectsRamAddress(objectsFile, i);

                if (ramAddress < _BASERAM)
                    continue;

                if (ramAddress > _upperRAM)
                    continue;

                if (!((ramAddress & 0xFF000000) == 0x80000000))
                    continue;

                // Update RAM address.
                WriteObjectsAddress(objectsFile, ramAddress, i);
            }

            RomFile.UpdateTotalsOffset(objectsFile.Length, reservedSpace.Length);

            Array.Copy(reservedSpaceConstants, reservedSpace, reservedSpaceConstants.Length); 
            
            // Update file buffer.
            _fileBuffer.Add(objectsFile);
            _fileBuffer.Add(reservedSpace);
        }

        private void UpdateAsmFile(AsmFile fileObj)
        {
            // Local Variables
            var objectsFile = (ObjectsFile)_objectsFiles.Peek();


            // Update current offset.
            AsmFile.CurrentFileOffset = RomFile.TotalOffset;
            _subOffArray.Add((uint)(RomFile.TotalOffset + _subOffsets[fileObj.Id] + _baseRam));

            // Create a local byte[] and pass it decompressed file info.
            byte[] asmFile = DecompressFile(fileObj);

            // Get _upperRAM value
            byte[] objFile = DecompressFile(objectsFile);
            _upperRAM = (uint)(_BASERAM + asmFile.Length + objFile.Length + objectsFile.ReservedLength);

            // Update JAL and LUI commands.
            ProcessCommands(asmFile);

            // Update F39F90 JALs
            UpdateF37F90(fileObj);

            RomFile.UpdateTotalsOffset(asmFile.Length, 0);

            // Update file buffer.
            _fileBuffer.Add(asmFile);
        }

        private void UpdateF37F90(AsmFile fileObj)
        {
            uint jr = 0x03E00008;
            uint cmd;

            if (fileObj.Id == (int)_levelID.CC)
            {
                cmd = ASM.GetCommand(_F37F90, 0x5DA20);
                UpdateJAL(_F37F90, cmd, 0x5DA20);
            }


            for (int j = 0; j < C.AsmGlobalsFuncsOffsets.afo[fileObj.Id].Length; j++)
            {
                int pos = C.AsmGlobalsFuncsOffsets.afo[fileObj.Id][j];

                // Step thru all commands in subroutine and check for JALs. 
                while (ASM.GetCommand(_F37F90, pos) != jr)
                {
                    cmd = ASM.GetCommand(_F37F90, pos);
                    if (ASM.GetOpcode(cmd) == C.JALCMD)
                        UpdateJAL(_F37F90, cmd, pos);
                            
                        pos += 4;
                    }
                }
        }

        private void WriteObjectsAddress(byte[] file, uint ramAddress, int position)
        {
            // Convert to new address.
            uint offset = (uint)(ramAddress - _BASERAM);
            uint newAddress = (uint)(_baseRam + RomFile.CurrentFileOffset + offset);

            byte[] AddressArray = BitConverter.GetBytes(newAddress);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(AddressArray);

            file[position] = AddressArray[0];
            file[position + 1] = AddressArray[1];
            file[position + 2] = AddressArray[2];
            file[position + 3] = AddressArray[3];
        }

        private byte[] DecompressFile(RomFile fileObj)
        {
            byte[] file = (byte[])_decompressor.DecompressRomFile((byte[])_rom, fileObj.Address, fileObj.Length);
            return file;
        }

        private void ProcessCommands(byte[] file)
        {
            // Local variables.
            int numCommands = file.Length / C.CMDLENGTH;
            uint cmd;
            uint opcode;

            // Search file for all instances of a LUI command. 
            for (int i = 0; i < numCommands; i++)
            {
                int position = i * 4;

                // If LUI command, find pairs and update addresses.
                cmd = ASM.GetCommand(file, position);
                opcode = ASM.GetOpcode(cmd);

                switch (opcode)
                {
                    case C.LUICMD:
                        UpdateLui(file, cmd, position);
                        break;
                    case C.JALCMD:
                        UpdateJAL(file, cmd, position);
                        break;
                }
            }
        }

        private void UpdateJAL(byte[] file, uint jalCmd, int position)
        {
            uint jalImmediate = ASM.GetJalImmediate(jalCmd);
            uint fullAddress = jalImmediate + 0x80000000;

            // Convert to new address.
            uint offset = (uint)(fullAddress - _BASERAM);
            uint newAddress = (uint)(_baseRam + RomFile.CurrentFileOffset + offset);

            uint newJal = ASM.BuildNewJalCommand(newAddress);

            byte[] JalArray = BitConverter.GetBytes(newJal);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(JalArray);

            if ((jalImmediate >= (_BASERAM - 0x80000000)) && (jalImmediate <= (_upperRAM - 0x80000000)))
            {
                file[position] = JalArray[0];
                file[position + 1] = JalArray[1];
                file[position + 2] = JalArray[2];
                file[position + 3] = JalArray[3];
            }
        }

        private void UpdateLui(byte[] file, uint luiCmd, int position)
        {
            int luiAddress = position;
            uint cmd;
            byte opcode;
            byte luiReg = ASM.GetDestReg(luiCmd);

            while (true)
            {
                position += C.CMDLENGTH;

                if (position == file.Length)
                    break;

                cmd = ASM.GetCommand(file, position);
                opcode = ASM.GetOpcode(cmd);

                // If LUI register not overwritten by other LUI command.
                if (!((opcode == C.LUICMD) && (ASM.GetDestReg(cmd) == luiReg)))
                {
                    // If LUI reg is AT.
                    if (C.Register.AT == luiReg)
                    {
                        // If paired command is not "load" or "store".
                        if (!(_loadOpsList.Contains(opcode) || _storeOpsList.Contains(opcode)))
                            continue;

                        // If register AT isn't used, exit. 
                        if (luiReg != ASM.GetBaseReg(cmd))
                            continue;

                        WriteLuiAddress(file, luiCmd, luiAddress, cmd, position);

                        break;
                    }
                    else // If LUI reg is anything other than AT.
                    {
                        // If LUI != dest reg.
                        if ((luiReg != ASM.GetDestReg(cmd)) && (ASM.GetDestReg(cmd) != C.Register.V0) && (ASM.GetDestReg(cmd) != C.Register.V1))
                            continue;

                        // If LUI != src reg.
                        if (luiReg != ASM.GetSourceReg(cmd))
                            continue;

                        // If paired command is an ORI.
                        if (opcode == C.ImmediateOps.ORI)
                            break;

                        // If not an ADDIU command or load command.
                        if ((opcode != C.ImmediateOps.ADDIU) && (!(_loadOpsList.Contains(opcode))) && (!(_storeOpsList.Contains(opcode))))
                            continue;

                        WriteLuiAddress(file, luiCmd, luiAddress, cmd, position);

                        break;
                    }

                }
                else
                {
                    break;
                }
            }
        }
     
        private void WriteLuiAddress(byte[] file, uint luiCmd, int luiAddress, uint pairCmd, int pairAddress)
        {
            uint address = ASM.GetCommandAddress(luiCmd, pairCmd);
            uint offset;
            uint newAddress;
            byte[] addressArray;

            // Convert to new address.
            offset = (uint)(address - _BASERAM);
            newAddress = (uint)(_baseRam + RomFile.CurrentFileOffset + offset);

            // Adjust upward for sign extension.
            if ((newAddress & 0xFFFF) > 0x8000)
                newAddress += 0x10000;

            addressArray = BitConverter.GetBytes(newAddress);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(addressArray);

            if ((address >= _BASERAM) && (address <= _upperRAM))
            {
                // Update the LUI immediate.
                file[luiAddress + 2] = addressArray[0];
                file[luiAddress + 3] = addressArray[1];
                
                // Update the paired command immediate.
                file[pairAddress + 2] = addressArray[2];
                file[pairAddress + 3] = addressArray[3];
            }
        }
    }
}

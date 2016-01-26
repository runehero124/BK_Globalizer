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
using C = BKGlobalize.Classes.Constants;

namespace BKGlobalize.Classes
{
    abstract class RomFile
    {
        private static int count = 0;
        protected static int totalOffset = 0;

        // Private Fields. 
        private int address;
        private int length;
        protected int[] _levelObjectVarAllocArray = { C.ObjVarSpace.CC, C.ObjVarSpace.MMM, C.ObjVarSpace.GV, C.ObjVarSpace.TTC, C.ObjVarSpace.MM, 
            C.ObjVarSpace.BGS, C.ObjVarSpace.RBB, C.ObjVarSpace.FP, C.ObjVarSpace.SM, C.ObjVarSpace.CS, C.ObjVarSpace.GL, C.ObjVarSpace.FB, C.ObjVarSpace.CCW };

        // Public Properties
        public static int CurrentFileOffset { get; set; }


        public static int TotalOffset 
        {
            get
            {
                return totalOffset;
            }
            set
            {
                totalOffset = value;
            }
        }

        public int Address
        {
            get { return address; }
        }
        
        public int Length 
        {
            get { return length; }
        }
        
        public static int Count
        {
            get { return count; }
        }

        // Constructor.
        public RomFile(int id, int startAddress, int endAddress)
        {

            this.address = startAddress;
            this.length = endAddress - startAddress;
            
            count++;
        }

        public static void UpdateTotalsOffset(int unpackedSize, int reservedSize)
        {
            totalOffset += (unpackedSize + reservedSize);
        }
    }
}

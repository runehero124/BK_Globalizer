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
    class ObjectsFile : RomFile
    {
        private static int count = 0;
        private int reservedLength;

        public int ReservedLength
        {
            get
            {
                return reservedLength;
            }
        }
         
        public static new int Count
        {
            get
            {
                return count;
            }
        } 

        public ObjectsFile(int id, int startAddress, int endAddress)
            : base(id, startAddress, endAddress)
        {
            this.reservedLength = _levelObjectVarAllocArray[id];

            count++;
        }
    }
}

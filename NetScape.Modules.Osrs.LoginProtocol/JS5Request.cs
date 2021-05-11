using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.Osrs.LoginProtocol
{
    public class JS5Request
    {
        public int Index { get; }
        public int File { get; }
        public bool Priority { get; }

        public JS5Request(int index, int file, bool priority)
        {
            Index = index;
            File = file;
            Priority = priority;
        }
    }
}

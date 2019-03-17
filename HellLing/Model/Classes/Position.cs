using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model.Classes
{
    /// <summary>
    /// Позиция лексемы в языке
    /// </summary>
    class Position
    {
        public Position(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; private set; }
        public int End { get; private set; }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }
}

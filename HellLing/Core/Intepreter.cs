using HellLing.Model.Intertreter;
using HellLing.Model.STree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Core
{
    class Intepreter
    {
        static List<Var> Vars;

        public static void Run(Tree tree)
        {
            Vars = new List<Var>();
        }
    }
}

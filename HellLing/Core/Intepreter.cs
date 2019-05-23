using HellLing.Model.Enums;
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
        static Stack<string> CurrentFunc;
        static Tree tree;

        public static void Run(Tree abstract_tree)
        {
            Vars = new List<Var>();
            CurrentFunc = new Stack<string>();
            tree = abstract_tree;
        }

        public void FindGlobalVars()
        {
            foreach (var branch in tree.Branches)
            {
                if (branch.Node.Purpose == EPurpose.Declare)
                {
                    Vars.Add(new Var(branch.Node.Type, branch.Node.State, 0.ToString(), false));
                }
            }
        }

        public Assign()
        {

        }
    }
}

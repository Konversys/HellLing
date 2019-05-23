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
        static List<string> Func;
        static Stack<string> Calls;
        static Stack<string> Args;
        static Stack<List<Var>> LocalVarsStack;
        static Tree tree;

        public static void Run(Tree abstract_tree, string func)
        {
            Func = new List<string>();
            Args = new Stack<string>();
            Calls = new Stack<string>();
            LocalVarsStack = new Stack<List<Var>>();
            tree = abstract_tree;
            List<Var> localVars = new List<Var>();
            LocalVarsStack.Push(localVars);
            FindGlobalElements(localVars);
            StartFunc(func);
            LocalVarsStack.Pop();
        }

        static void FindGlobalElements(List<Var> localVars)
        {
            foreach (var branch in tree.Branches)
            {
                Node node = branch.Node;
                if (node.Purpose == EPurpose.Declare)
                {
                    localVars.Add(new Var(node.Type, node.State, 0.ToString(), false));
                }
                if (node.Element == EElement.Func && node.Purpose == EPurpose.None)
                {
                    Func.Add(node.State);
                }
            }
        }

        static void StartFunc(string func)
        {
            List<Var> localVars = new List<Var>();
            LocalVarsStack.Push(localVars);
            Calls.Push(func);
            Tree tree = GetTreeFunc(func);
            foreach (var branch in tree.Branches)
            {
                if (branch.Node.Element == EElement.For)
                {
                    Console.WriteLine("GAV");
                }
                else
                {
                    switch (branch.Node.Purpose)
                    {
                        case EPurpose.Declare:
                            if (Args.Count() > 0)
                            {
                                localVars.Add(new Var(branch.Node.Type, branch.Node.State, Args.Pop(), branch.Node.IsArg));
                            }
                            else
                            {
                                localVars.Add(new Var(branch.Node.Type, branch.Node.State, 0.ToString(), branch.Node.IsArg));
                            }
                            break;
                        case EPurpose.Call:
                            foreach (var item in branch.Branches)
                            {
                                Args.Push(GetVar(item.Node.State).Value);
                            }
                            List<Var> temp = LocalVarsStack.Pop();
                            StartFunc(branch.Node.State);
                            LocalVarsStack.Push(temp);
                            break;
                        case EPurpose.Expression:
                            Assign(branch);
                            break;
                    }
                }
            }
            Calls.Pop();
            LocalVarsStack.Pop();
        }

        static void Assign(Tree tree)
        {
            Var assigner = GetVar(tree.Node.State);
            foreach (var branch in tree.Branches)
            {
                if (branch.Node.Element == EElement.Var)
                {
                    if (branch.Node.Type == EType.Int || branch.Node.Type == EType.Double)
                    {
                        assigner.Assign(branch.Node.State);
                    }
                    if (branch.Node.Type == EType.None)
                    {
                        assigner.Assign(GetVar(branch.Node.State).Value);
                    }
                }
            }
        }

        static Tree GetTreeFunc(string func)
        {
            foreach (var branch in tree.Branches)
            {
                if (branch.Node.Element == EElement.Func && branch.Node.Purpose == EPurpose.None && branch.Node.State == func)
                {
                    return branch;
                }
            }
            return null;
        }

        static Var GetVar(string var)
        {
            foreach (var area in LocalVarsStack.ToList())
            {
                if (area.Where(x => x.Title == var).Count() == 1)
                {
                    return area.Where(x => x.Title == var).First();
                }
            }
            return null;
        }
    }
}

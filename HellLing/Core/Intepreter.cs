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
        static EPurpose CurOperation;
        static string CurVal;

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
            CallFunc(func);
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

        static void CallFunc(string func)
        {
            if (func == "print")
            {
                CallXPrint();
                return;
            }
            List<Var> localVars = new List<Var>();
            LocalVarsStack.Push(localVars);
            Calls.Push(func);
            Tree tree = GetTreeFunc(func);
            foreach (var branch in tree.Branches)
            {
                if (branch.Node.Element == EElement.For)
                {
                    CallFor(branch);
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
                            CallFunc(branch.Node.State);
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

        private static void CallXPrint()
        {
            while (Args.Count() > 0)
            {
                Console.WriteLine("STOOPED INTERPRETER SAY: " + Args.Pop());
            }
        }

        static void CallFor(Tree tree)
        {
            List<Var> localVars = new List<Var>();
            LocalVarsStack.Push(localVars);
            Calls.Push("For");
            foreach (var branch in tree.Branches.Take(2))
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
                        CallFunc(branch.Node.State);
                        LocalVarsStack.Push(temp);
                        break;
                    case EPurpose.Expression:
                        Assign(branch);
                        break;
                }
            }
            Var iterator = localVars.First();
            Tree expression = tree.Branches[2];
            Var expL = iterator;
            EPurpose sign = expression.Branches[1].Node.Purpose;
            Var expR;
            if (expression.Branches[2].Node.Type == EType.None)
            {
                expR = GetVar(expression.Branches[2].Node.State);
            }
            else
            {
                expR = new Var(expression.Branches[2].Node.Type, null, expression.Branches[2].Node.State);
            }
            Node procedure = tree.Branches[3].Node;
            while (CompareBool(expL.Value, sign, expR.Value))
            {
                foreach (var branch in tree.Branches.Skip(4))
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
                            CallFunc(branch.Node.State);
                            LocalVarsStack.Push(temp);
                            break;
                        case EPurpose.Expression:
                            Assign(branch);
                            break;
                    }
                }
                Operation(iterator, null, procedure.Purpose);
            }
            Calls.Pop();
            LocalVarsStack.Pop();
        }

        static string Assign(Tree tree)
        {
            Var assigner = GetVar(tree.Node.State);
            if (tree.Branches.Count() == 1)
            {
                if (tree.Branches[0].Node.Element == EElement.Var)
                {
                    if (tree.Branches[0].Node.Type == EType.Int || tree.Branches[0].Node.Type == EType.Double)
                    {
                        return assigner.Assign(tree.Branches[0].Node.State);
                    }
                    if (tree.Branches[0].Node.Type == EType.None)
                    {
                        return assigner.Assign(GetVar(tree.Branches[0].Node.State).Value);
                    }
                }
            }
            else if (tree.Branches.Count() > 1)
            {
                foreach (var branch in tree.Branches)
                {
                    if (branch.Node.Element == EElement.Var)
                    {
                        Operation(assigner, GetVar(branch.Node.State).Value, branch.Node.Purpose);
                    }
                    else
                    {
                        Operation(assigner, branch.Node.Value, branch.Node.Purpose);
                    }
                }
                return assigner.Value;
            }
            return null;
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

        static bool CompareBool(string val1, EPurpose procedure, string val2)
        {
            switch (procedure)
            {
                case EPurpose.TGE:
                    return double.Parse(val1) >= double.Parse(val2);
                case EPurpose.TGT:
                    return double.Parse(val1) > double.Parse(val2);
                case EPurpose.TLE:
                    return double.Parse(val1) <= double.Parse(val2);
                case EPurpose.TLT:
                    return double.Parse(val1) < double.Parse(val2);
                case EPurpose.TNQ:
                    return val1 != val2;
                case EPurpose.TEQ:
                    return val1 == val2;
                default:
                    return false;
            }
        }

        static void Operation(Var var, string value = null, EPurpose purpose = EPurpose.None)
        {
            if (purpose == EPurpose.IncL || purpose == EPurpose.DecL || purpose == EPurpose.IncF || purpose == EPurpose.DecF)
            {
                switch (purpose)
                {
                    case EPurpose.IncF:
                        var.Assign((double.Parse(var.Value) + 1).ToString());
                        break;
                    case EPurpose.DecF:
                        var.Assign((double.Parse(var.Value) - 1).ToString());
                        break;
                    case EPurpose.IncL:
                        var.Assign((double.Parse(var.Value) + 1).ToString());
                        break;
                    case EPurpose.DecL:
                        var.Assign((double.Parse(var.Value) - 1).ToString());
                        break;
                    default:
                        break;
                }
                return;
            }
            if (purpose == EPurpose.None && value != null && CurOperation == EPurpose.None)
            {
                var.Assign(value);
                return;
            }
            if (value == null && purpose != EPurpose.None)
            {
                CurOperation = purpose;
            }
            if (value != null && purpose == EPurpose.None)
            {
                CurVal = value;
            }
            if (CurOperation != EPurpose.None && CurVal != null)
            {
                switch (CurOperation)
                {
                    case EPurpose.Add:
                        var.Assign((double.Parse(var.Value) + double.Parse(CurVal)).ToString());
                        break;
                    case EPurpose.Sub:
                        var.Assign((double.Parse(var.Value) - double.Parse(CurVal)).ToString());
                        break;
                    case EPurpose.Mul:
                        var.Assign((double.Parse(var.Value) * double.Parse(CurVal)).ToString());
                        break;
                    case EPurpose.Div:
                        var.Assign((double.Parse(var.Value) / double.Parse(CurVal)).ToString());
                        break;
                    default:
                        break;
                }
                CurOperation = EPurpose.None;
                CurVal = null;
                return;
            }
        }
    }
}

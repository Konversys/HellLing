using HellLing.Model.Classes;
using HellLing.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model.STree
{
    class Tree
    {
        public Node Node { get; private set; }
        public Tree Up { get; private set; }
        public List<Tree> Branches { get; private set; }

        private Tree(Tree up, Node node)
        {
            Branches = new List<Tree>();
            Up = up;
            Node = node;
        }
        public Tree(string name)
        {
            Node = Node.NewBase();

        }
        /// <summary>
        /// Левый потомок - сосед
        /// </summary>
        /// <param name="node"></param>
        public Tree AddBranch(Node node)
        {
            Tree tree = new Tree(this, node);
            Branches.Add(tree);
            return tree;
        }
        /// <summary>
        /// Правый потомок - следующий уровень вложенности
        /// </summary>
        /// <param name="node"></param>


        public Tree FindUpVar(Token token)
        {
            Tree current = this;
            while (current != null)
            {
                if (current.Node.Title == token.State && current.Node.Element == EElement.Var)
                {
                    return current;
                }
                foreach (var branch in current.Branches)
                {
                    if (branch != null && branch.Node.Title == token.State && branch.Node.Element == EElement.Var)
                    {
                        return branch;
                    }
                }
                current = current.Up;
            }
            return null;
        }
       
        public Tree FindUpArray(Token token)
        {
            Tree current = this;
            while (current != null)
            {
                if (current.Node.Title == token.State && current.Node.Element == EElement.Array)
                {
                    return current;
                }
                foreach (var branch in current.Branches)
                {
                    if (branch != null && branch.Node.Title == token.State && branch.Node.Element == EElement.Array)
                    {
                        return branch;
                    }
                }
                current = current.Up;
            }
            return null;
        }
        public Tree FindUpFunc(Token token)
        {
            Tree current = this;
            while (current != null)
            {
                if (current.Node.Title == token.State && current.Node.Element == EElement.Func)
                {
                    return current;
                }
                foreach (var branch in current.Branches)
                {
                    if (branch != null && branch.Node.Title == token.State && branch.Node.Element == EElement.Func)
                    {
                        return branch;
                    }
                }
                current = current.Up;
            }
            return null;
        }
       
        public Tree FindUpBase(Token token)
        {
            Tree current = this;
            while (current != null)
            {
                if (current.Node.Title == token.State && current.Node.Element == EElement.Base)
                {
                    return current;
                }
                foreach (var branch in current.Branches)
                {
                    if (branch != null && branch.Node.Title == token.State && branch.Node.Element == EElement.Base)
                    {
                        return branch;
                    }
                }
                current = current.Up;
            }
            return null;
        }
        public bool ContainsVar(Token token)
        {
            if (FindUpVar(token) == null)
            {
                return false;
            }
            return true;
        }
        public bool ContainsArray(Token token)
        {
            if (FindUpArray(token) == null)
            {
                return false;
            }
            return true;
        }
        public bool ContainsFunc(Token token)
        {
            if (token.State == "print")
            {
                return true;
            }
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Func))
            {
                current = current.Up;
            }
            if (current == null)
            {
                return false;
            }
            return true;
        }
        public bool ContainsBase(Token token)
        {
            if (FindUpBase(token) == null)
            {
                return false;
            }
            return true;
        }

        public EType GetTypeVar(Token token)
        {
            Tree tree = FindUpVar(token);
            if (tree == null)
            {
                return EType._null;
            }
            return tree.Node.Type;
        }
        public EType GetTypeArray(Token token)
        {
            Tree tree = FindUpArray(token);
            if (tree == null)
            {
                return EType._null;
            }
            return tree.Node.Type;
        }
        public EType GetTypeFunc(Token token)
        {
            Tree tree = FindUpFunc(token);
            if (tree == null)
            {
                return EType._null;
            }
            return tree.Node.Type;
        }

        public EType GetTypeBase(Token token)
        {
            Tree tree = FindUpBase(token);
            if (tree == null)
            {
                return EType._null;
            }
            return tree.Node.Type;
        }
    }
}

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
            Node = Node.Create(EElement.Base);
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
       
        public Tree FindUp(Token token, EElement element)
        {
            Tree current = this;
            while (current != null)
            {
                if (current.Node.State == token.State && current.Node.Element == element)
                {
                    return current;
                }
                foreach (var branch in current.Branches)
                {
                    if (branch != null && branch.Node.State == token.State && branch.Node.Element == element)
                    {
                        return branch;
                    }
                }
                current = current.Up;
            }
            return null;
        }

        public bool Contains(Token token, EElement element)
        {
            if (element == EElement.Func && token.State == "print")
            {
                return true;
            }
            if (FindUp(token, element) == null)
            {
                return false;
            }
            return true;
        }

        public EType GetType(Token token, EElement element)
        {
            Tree tree = FindUp(token, element);
            if (tree == null)
            {
                return EType._null;
            }
            return tree.Node.Type;
        }
    }
}

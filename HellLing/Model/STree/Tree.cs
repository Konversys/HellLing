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
        public Tree Left { get; private set; }
        public Tree Right { get; private set; }

        private Tree(Tree left, Tree right, Tree up, Node node)
        {
            Left = left;
            Right = right;
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
        public void SetLeft(Node node)
        {
            Left = new Tree(null, null, this, node);
        }
        /// <summary>
        /// Правый потомок - следующий уровень вложенности
        /// </summary>
        /// <param name="node"></param>
        public void SetRight(Node node)
        {
            Right = new Tree(null, null, this, node);
        }
        public Tree FindUpVar(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Var))
            {
                current = current.Up;
            }
            return current;
        }
        public Tree FindUpArray(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Array))
            {
                current = current.Up;
            }
            return current;
        }
        public Tree FindUpFunc(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Func))
            {
                current = current.Right;
            }
            return current;
        }
        public Tree FindUpNone(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.None))
            {
                current = current.Up;
            }
            return current;
        }
        public Tree FindUpNull(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement._null))
            {
                current = current.Up;
            }
            return current;
        }
        public Tree FindUpBase(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Base))
            {
                current = current.Up;
            }
            return current;
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
        public bool ContainsNone(Token token)
        {
            if (FindUpNone(token) == null)
            {
                return false;
            }
            return true;
        }

        public EType GetTypeVar(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Var))
            {
                current = current.Up;
            }
            if (current == null)
            {
                return EType._null;
            }
            return current.Node.Type;
        }
        public EType GetTypeArray(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Array))
            {
                current = current.Up;
            }
            if (current == null)
            {
                return EType._null;
            }
            return current.Node.Type;
        }
        public EType GetTypeFunc(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Element == EElement.Func))
            {
                current = current.Up;
            }
            while (current != null && !(current.Node.Title == token.State && current.Node.Element == EElement.Func))
            {
                current = current.Right;
            }
            if (current == null)
            {
                return EType._null;
            }
            return current.Node.Type;
        }

        public EType GetThisFunc(Token token)
        {
            Tree current = this;
            while (current != null && !(current.Node.Element == EElement.Func))
            {
                current = current.Up;
            }
            if (current == null)
            {
                return EType._null;
            }
            return current.Node.Type;
        }
    }
}

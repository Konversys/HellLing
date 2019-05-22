using HellLing.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model.STree
{
    class Node
    {
        /// <summary>
        /// Var, Array, Func, Const
        /// </summary>
        public string State { get; private set; }
        /// <summary>
        /// Var, Array, Const, Func, None
        /// </summary>
        public EElement Element { get; private set; }
        /// <summary>
        /// Var, Array, Func, Const
        /// </summary>
        public EType Type { get; private set; }
        /// <summary>
        /// Array
        /// </summary>
        public int Length { get; private set; }

        public Node() { }

        public Node(string state, EElement element, EType type, int length)
        {
            State = state;
            Element = element;
            Type = type;
            Length = length;
        }

        public static Node Create(EElement element, string state = null, EType type = EType.None, int length = 0)
        {
            return new Node(state, element, type, length);
        }
    }
}

/*
        /// <summary>
        /// Название функции для аргумента?
        /// </summary>
        public string Func { get; private set; }

        public static Node NewVar(string state, EType type, string func = null)
        {
            return new Node() { Element = EElement.Var, Type = type, State = state, Func = func };
        }
        public static Node NewArray(string state, EType type, int length, string func = null)
        {
            return new Node() { Element = EElement.Array, Type = type, State = state, Length = length, Func = func };
        }
        public static Node NewFunction(string state, EType returnType)
        {
            return new Node() { Element = EElement.Func, Type = returnType, State = state };
        }
        public static Node NewConst(EType type)
        {
            return new Node() { Element = EElement.Const, Type = type };
        }

        public static Node NewOperator(string state, EOperator oper)
        {
            return new Node() { Element = EElement., };
        }
 */

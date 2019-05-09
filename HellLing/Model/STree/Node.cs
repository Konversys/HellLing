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
        public string Title { get; private set; }
        /// <summary>
        /// Var, Array, Func, Const
        /// </summary>
        public EType Type { get; private set; }
        /// <summary>
        /// Var, Array, Const, Func, None
        /// </summary>
        public EElement Element { get; private set; }
        /// <summary>
        /// Array
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// Название функции для аргумента?
        /// </summary>
        public string Func { get; private set; }

        public static Node NewVar(string title, EType type, string func = null)
        {
            return new Node() { Element = EElement.Var, Type = type, Title = title, Func = func };
        }
        public static Node NewArray(string title, EType type, int length, string func = null)
        {
            return new Node() { Element = EElement.Array, Type = type, Title = title, Length = length, Func = func };
        }
        public static Node NewFunction(string title, EType returnType)
        {
            return new Node() { Element = EElement.Func, Type = returnType, Title = title };
        }
        public static Node NewConst(EType type)
        {
            return new Node() { Element = EElement.Const, Type = type };
        }
        public static Node NewNone()
        {
            return new Node() { Element = EElement.None };
        }
        public static Node NewBase()
        {
            return new Node() { Element = EElement.None };
        }
    }
}

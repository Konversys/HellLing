using HellLing.Model;
using HellLing.Model.Classes;
using HellLing.Model.Enums;
using HellLing.Model.STree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Exception = HellLing.Model.Classes.Exception;

namespace HellLing.Core
{
    class Analyzer
    {
        static Stack<Tree> save { get; set; }
        public static Tree tree { get; private set; }
        /// <summary>
        /// Список ошибок
        /// </summary>
        public static Errors errors { get; private set; }
        /// <summary>
        /// Список лексем
        /// </summary>
        public static List<Token> Tokens { get; set; }
        /// <summary>
        /// Каретка
        /// </summary>
        static int car { get; set; }
        static bool CurrentToken(Lexem lex)
        {
            if (Tokens[car].Lexem == lex)
                return true;
            return false;
        }
        /// <summary>
        /// Проверка на присутствие следующего токена
        /// </summary>
        /// <returns></returns> 
        static bool ShiftToken(int shift = 1)
        {
            if (car + shift >= Tokens.Count || car + shift < 0)
                return false;
            car += shift;
            return true;
        }
        static Token GetToken(int shift = 0)
        {
            if (car + shift >= Tokens.Count)
                return null;
            return Tokens[car + shift];
        }
        public static Errors Start(List<Token> tokens)
        {
            Tokens = tokens;
            car = 0;
            errors = new Errors();
            tree = new Tree("program");
            save = new Stack<Tree>();
            FC.SetData(tokens);
            Program();
            return errors;
        }

        static void Program()
        {
            while (!First(Lexem.TEOF))
            {
                if (FC.Method3(car))
                {
                    Method();
                }
                else if (FC.GlobalVar0(car))
                {
                    GlobalVar();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name} Ожидались глобальные переменные или метод");
                    ShiftToken();
                }
            }
        }
        static void Method()
        {
            ShiftToken(3);
            AddFunc(-1);
            save.Push(tree);
            ArgDeclar();
            ShiftToken(2);
            MethodBlock();
            tree = save.Pop();
            ShiftToken();
        }
        static void GlobalVar()
        {
            while ((FC.VarDeclar2(car) || FC.VarAssign2(car) || FC.ArrAssign5(car)) && !FC.Method3(car))
            {
                if (FC.VarDeclar2(car))
                {
                    VarList();
                }
                else if (FC.VarAssign2(car))
                {
                    VarAssign();
                }
                else if (FC.ArrAssign5(car))
                {
                    ArrAssign();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалась операция с переменными и массивами");
                }

                if (First(Lexem.TTZpt))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                    ShiftToken();
                }
            }
        }

        static void VarList()
        {
            do
            {
                ShiftToken(1);
                if (FC.VarAssign2(car))
                {
                    AddVar(1);
                    VarAssign();
                }
                else if (First(Lexem.TID, Lexem.TCLS))
                {
                    ShiftToken(2);
                    AddArray(-1);
                    Expression();
                    if (First(Lexem.TCRS))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ]");
                        ShiftToken();
                    }
                    ShiftToken();
                }
                else if (First(Lexem.TID))
                {
                    ShiftToken();
                    AddVar();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ID");
                    ShiftToken();
                }
            } while (First(Lexem.TZpt));
        }

        static void VarAssign()
        {
            if (FC.VarAssign2(car))
            {
                ShiftToken(2);
                Expression();
            }
            else
            {
                errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалось присвоение переменной");
                ShiftToken();
            }
        }

        static void ArrAssign()
        {
            if (FC.ArrAssign5(car))
            {
                ShiftToken(2);
                Expression();
                if (First(Lexem.TCRS))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалось ]");
                    ShiftToken();
                }
                if (First(Lexem.TSave))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалось =");
                    ShiftToken();
                }
                Expression();
            }
        }

        static void VarDeclar()
        {
            if (FC.VarDeclar2(car))
            {
                ShiftToken(2);
            }
            else
            {
                errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидался тип данных");
                ShiftToken();
            }
        }

        static void ArgDeclar()
        {
            do
            {
                if (FC.VarDeclar2(car))
                {
                    ShiftToken(2);
                    if (First(Lexem.TCLS))
                    {
                        AddArray();
                        ShiftToken();
                        Expression();
                        if (First(Lexem.TCRS))
                        {
                            ShiftToken();
                        }
                    }
                    else
                    {
                        AddVar();
                    }
                }
            } while (First(Lexem.TZpt) && ShiftToken());
        }

        static void MethodBlock()
        {
            do
            {
                if (FC.MethodCall2(car))
                {
                    MethodCall();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (FC.For1(car))
                {
                    For();
                    if (First(Lexem.TFRS))
                    {
                        break;
                    }
                }
                else if (FC.ArrAssign5(car))
                {
                    ArrAssign();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (FC.VarDeclar2(car))
                {
                    VarList();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (FC.Inc2(car))
                {
                    Inc();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (FC.Dec2(car))
                {
                    Dec();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (First(Lexem._int) || First(Lexem._double))
                {
                    VarList();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (First(Lexem._return))
                {
                    ShiftToken();
                    Expression();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                }
                else if (FC.VarAssign2(car))
                {
                    VarAssign();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                    }
                }
            } while (FC.MethodCall2(car) || FC.For1(car) || FC.ArrAssign5(car) || FC.Inc2(car) || FC.Dec2(car) ||
                    First(Lexem._int) || First(Lexem._double) || First(Lexem._return) || FC.VarAssign2(car));
        }

        static void MethodCall()
        {
            if (FC.MethodCall2(car))
            {
                ShiftToken(2);
                do
                {
                    if (First(Lexem.TRS))
                    {
                        break;
                    }
                    Expression();
                } while (First(Lexem.TZpt) && ShiftToken());
                if (First(Lexem.TRS))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется )");
                }
            }
        }

        static void For()
        {
            if (First(Lexem._for, Lexem.TLS))
            {
                ShiftToken(2);
            }
            AddFor();
            save.Push(tree);
            if (First(Lexem._int) || First(Lexem._double))
            {
                ShiftToken();
            }
            if (FC.VarAssign2(car))
            {
                VarAssign();
            }
            if (First(Lexem.TTZpt))
            {
                ShiftToken();
            }
            else
            {
                errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                ShiftToken();
            }
            Expression();
            if (First(Lexem.TTZpt))
            {
                ShiftToken();
            }
            if (FC.VarAssign2(car))
            {
                VarAssign();
            }
            else if (FC.Inc2(car))
            {
                Inc();
            }
            else if (FC.Dec2(car))
            {
                Dec();
            }
            if (First(Lexem.TRS, Lexem.TFLS))
            {
                ShiftToken(2);
            }
            MethodBlock();
            tree = save.Pop();
            if (First(Lexem.TFRS))
            {
                ShiftToken();
            }
        }

        static void Inc()
        {
            if (FC.Inc2(car))
            {
                ShiftToken(2);
            }
        }

        static void Dec()
        {
            if (FC.Dec2(car))
            {
                ShiftToken(2);
            }
        }

        static void Expression()
        {
            do
            {
                Addend();
            } while ((First(Lexem.TGE) || First(Lexem.TGT) || First(Lexem.TLE) || First(Lexem.TLT) || First(Lexem.TNQ) || First(Lexem.TEQ))
                && ShiftToken());
        }

        static void Addend()
        {
            do
            {
                Multiplier();
            } while ((First(Lexem.TPlus) || First(Lexem.TMinus)) && ShiftToken());
        }

        static void Multiplier()
        {
            do
            {
                AtomicExpression();
            } while ((First(Lexem.TMult) || First(Lexem.TDiv)) && ShiftToken());
        }

        static void AtomicExpression()
        {
            if (First(Lexem.TNot))
            {
                ShiftToken();
                Expression();
            }
            else if (First(Lexem.TID, Lexem.TCLS))
            {
                ShiftToken(2);
                Expression();
                if (First(Lexem.TCRS))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ]");
                    ShiftToken();
                }
            }
            else if (FC.Inc2(car))
            {
                Inc();
            }
            else if (FC.Inc2(car))
            {
                Dec();
            }
            else if (First(Lexem.TConstInt))
            {
                ShiftToken();
            }
            else if (First(Lexem.TConstDouble))
            {
                ShiftToken();
            }
            else if (First(Lexem.TConstChar))
            {
                ShiftToken();
            }
            else if (First(Lexem.TID))
            {
                ShiftToken();
            }
            else if (FC.MethodCall2(car))
            {
                MethodCall();
            }
            else
            {
                errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалось элементарное выражение");
                ShiftToken();
            }
        }

        #region Operation
        static bool First(Lexem lexem1, Lexem lexem2 = Lexem.TError, Lexem lexem3 = Lexem.TError, Lexem lexem4 = Lexem.TError)
        {
            if (lexem4 == Lexem.TError)
            {
                if (lexem3 == Lexem.TError)
                {
                    if (lexem2 == Lexem.TError)
                    {
                        if (GetToken(1) != null && GetToken(1).Lexem == lexem1)
                        {
                            return true;
                        }
                        return false;
                    }
                    else if (GetToken(2) != null && GetToken(1).Lexem == lexem1 && GetToken(2).Lexem == lexem2)
                    {
                        return true;
                    }
                    return false;
                }
                else if (GetToken(3) != null && GetToken(1).Lexem == lexem1 && GetToken(2).Lexem == lexem2 && GetToken(3).Lexem == lexem3)
                {
                    return true;
                }
            }
            else if (GetToken(4) != null && GetToken(1).Lexem == lexem1 && GetToken(2).Lexem == lexem2 && GetToken(3).Lexem == lexem3 && GetToken(4).Lexem == lexem4)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Tree operation
        public static bool AddFor()
        {
            tree.SetRight(Node.NewNone());
            tree = tree.Right;
            return true;
        }

        public static bool AddFunc(int shift = 0)
        {
            Token token = GetToken(shift);
            if (tree.FindUpFunc(token) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Функция {token.State} уже существует в этой области видимости");
                return false;
            }
            else
            {
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        tree.SetRight(Node.NewFunction(token.State, EType.Int));
                        break;
                    case Lexem._double:
                        tree.SetRight(Node.NewFunction(token.State, EType.Double));
                        break;
                    case Lexem._void:
                        tree.SetRight(Node.NewFunction(token.State, EType.Void));
                        break;
                    default:
                        return false;
                }
                tree = tree.Right;
                return true;
            }
        }
        public static bool AddVar(int shift = 0)
        {
            Token token = GetToken(shift);
            if (tree.FindUpVar(token) != null || tree.FindUpArray(token) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Переменная {token.State} уже существует в этой области видимости");
                return false;
            }
            else
            {
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        tree.SetLeft(Node.NewVar(token.State, EType.Int));
                        break;
                    case Lexem._double:
                        tree.SetLeft(Node.NewVar(token.State, EType.Double));
                        break;
                    default:
                        return false;
                }
                tree = tree.Left;
                return true;
            }
        }
        public static bool AddArray(int shift = 0)
        {
            Token token = GetToken(shift);
            int lenght = int.Parse(GetToken(shift + 2).Title);
            if (tree.FindUpVar(token) != null || tree.FindUpArray(token) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Массив {token.State} уже существует в этой области видимости");
                return false;
            }
            else
            {
                switch (GetToken(shift-1).Lexem)
                {
                    case Lexem._int:
                        tree.SetLeft(Node.NewArray(token.State, EType.Int, lenght, false));
                        break;
                    case Lexem._double:
                        tree.SetLeft(Node.NewArray(token.State, EType.Double, lenght, false));
                        break;
                    default:
                        return false;
                }
                tree = tree.Left;
                return true;
            }
        }
        #endregion
    }
}
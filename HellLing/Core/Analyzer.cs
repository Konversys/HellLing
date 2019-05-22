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
        /// <summary>
        /// Текущяя функция
        /// </summary>
        static string FuncName { get; set; }
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
            FuncName = null;
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
            FuncName = null;
            AddFunc(-1);
            ArgDeclar();
            FuncName = null;
            ShiftToken(2);
            MethodBlock();
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
                    if (First(Lexem.TConstInt) || First(Lexem.TID))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидался тип int");
                        ShiftToken();
                    }
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
                if (!tree.ContainsVar(GetToken(1)))
                {
                    errors.Add(GetToken(1), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Переменная не объявлена");
                }
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
                if (!tree.ContainsArray(GetToken(1)))
                {
                    errors.Add(GetToken(1), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Массив не объявлен");
                }
                ShiftToken(2);
                if (First(Lexem.TConstInt) || First(Lexem.TID))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидался тип int");
                    ShiftToken();
                }
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
                AddVar();
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
                        AddArray(0, true);
                        ShiftToken();
                        if (First(Lexem.TConstInt) || First(Lexem.TID))
                        {
                            ShiftToken();
                        }
                        else
                        {
                            errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидался тип int");
                            ShiftToken();
                        }
                        if (First(Lexem.TCRS))
                        {
                            ShiftToken();
                        }
                        else
                        {
                            errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалось ]");
                            ShiftToken();
                        }
                    }
                    else
                    {
                        AddVar(0, true);
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
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
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
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                    }
                }
                else if (FC.VarDeclar2(car))
                {
                    VarList();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                    }
                }
                else if (FC.Inc2(car))
                {
                    Inc();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                    }
                }
                else if (FC.Dec2(car))
                {
                    Dec();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
                    }
                }
                else if (First(Lexem._int) || First(Lexem._double))
                {
                    VarList();
                    if (First(Lexem.TTZpt))
                    {
                        ShiftToken();
                    }
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
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
                    else
                    {
                        errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ;");
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

        static EType MethodCall()
        {
            if (FC.MethodCall2(car))
            {
                EType type = tree.GetTypeFunc(GetToken(1));
                if (!tree.ContainsFunc(GetToken(1)))
                {
                    errors.Add(GetToken(1), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Функция не объявлена");
                }
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
                    return type;
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется )");
                }
            }
            return EType._null;
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
                if (FC.VarAssign2(car))
                {
                    ShiftToken(-1);
                    VarDeclar();
                    ShiftToken(-1);
                    VarAssign();
                }
            }
            else if (FC.VarAssign2(car))
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

        static EType Inc()
        {
            if (FC.Inc2(car))
            {
                if (First(Lexem.TID))
                {
                    ShiftToken(2);
                    return tree.GetTypeVar(GetToken(-1));
                }
                else
                {
                    ShiftToken(2);
                    return tree.GetTypeVar(GetToken());
                }
            }
            return EType._null;
        }

        static EType Dec()
        {
            if (FC.Dec2(car))
            {
                if (First(Lexem.TID))
                {
                    ShiftToken(2);
                    return tree.GetTypeVar(GetToken(-1));
                }
                else
                {
                    ShiftToken(2);
                    return tree.GetTypeVar(GetToken());
                }
            }
            return EType._null;
        }

        static EType Expression()
        {
            EType result = EType.None;
            EType type;
            int start = car;
            do
            {
                type = Addend();
                result = CastPlusMinusType(type, result);
            } while ((First(Lexem.TGE) || First(Lexem.TGT) || First(Lexem.TLE) || First(Lexem.TLT) || First(Lexem.TNQ) || First(Lexem.TEQ))
                && ShiftToken());
            if ((result == EType.None && type == EType._null) || (result == EType._null))
            {
                errors.Add(new Token(GetToken().Lexem, new Position(start + 1, car + 1), GetCarText(start + 1, car + 1), GetToken().Code), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Невозможно привести типы в данном выражении");
                return EType._null;
            }
            if (result != EType.None)
            {
                return result;
            }
            else
            {
                return type;
            }
        }

        static EType Addend()
        {
            EType result = EType.None;
            EType type;
            do
            {
                type = Multiplier();
                if (First(Lexem.TPlus))
                {
                    result = CastPlusMinusType(type, result);
                }
                else if (First(Lexem.TMinus))
                {
                    result = CastPlusMinusType(type, result);
                }
            } while ((First(Lexem.TPlus) || First(Lexem.TMinus)) && ShiftToken());
            if (result == EType.None)
            {
                return type;
            }
            return result;
        }

        static EType Multiplier()
        {
            EType result = EType.None;
            EType type;
            do
            {
                type = AtomicExpression();
                if (First(Lexem.TMult))
                {
                    result = CastMultType(type, result);
                }
                else if (First(Lexem.TDiv))
                {
                    result = CastDivType(type, result);
                }
            } while ((First(Lexem.TMult) || First(Lexem.TDiv)) && ShiftToken());
            if (result == EType.None)
            {
                return type;
            }
            return result;
        }

        static EType AtomicExpression()
        {
            if (First(Lexem.TNot))
            {
                ShiftToken();
                return Expression();
            }
            else if (First(Lexem.TID, Lexem.TCLS))
            {
                ShiftToken();
                EType type = tree.GetTypeArray(GetToken());
                ShiftToken();
                if (First(Lexem.TConstInt) || First(Lexem.TID))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидался тип int");
                    ShiftToken();
                }
                if (First(Lexem.TCRS))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Требуется ]");
                    ShiftToken();
                }
                return type;
            }
            else if (FC.Inc2(car))
            {
                return Inc();
            }
            else if (FC.Inc2(car))
            {
                return Dec();
            }
            else if (First(Lexem.TConstInt))
            {
                ShiftToken();
                return EType.Int;
            }
            else if (First(Lexem.TConstDouble))
            {
                ShiftToken();
                return EType.Double;
            }
            else if (First(Lexem.TConstChar))
            {
                ShiftToken();
                return EType.Char;
            }
            else if (First(Lexem.TID))
            {
                ShiftToken();
                if (!tree.ContainsVar(GetToken(0)))
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Переменная не объявлена");
                    return EType._null;
                }
                return tree.GetTypeVar(GetToken());
            }
            else if (FC.MethodCall2(car))
            {
                return MethodCall();
            }
            else
            {
                errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Ожидалось элементарное выражение");
                ShiftToken();
            }
            return EType._null;
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
        static string GetCarText(int start, int end)
        {
            string result = "";
            for (int i = start; i < end; i++)
            {
                result += Tokens[i].State;
            }
            return result;
        }
        #endregion

        #region Tree operation
        public static bool AddFor()
        {
            tree.SetRight(Node.NewFor());
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
                FuncName = token.State;
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        tree.AddBranch(Node.Create(EElement.Func, token.State, EType.Int));
                        break;
                    case Lexem._double:
                        tree.AddBranch(Node.Create(EElement.Func, token.State, EType.Double));
                        break;
                    case Lexem._void:
                        tree.AddBranch(Node.Create(EElement.Func, token.State, EType.Void));
                        break;
                    default:
                        return false;
                }
                tree = tree.;
                return true;
            }
        }
        public static bool AddVar(int shift = 0, bool isFunc = false)
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
                        if (isFunc)
                        {
                            tree.SetLeft(Node.NewVar(token.State, EType.Int, FuncName));
                        }
                        else
                        {
                            tree.SetLeft(Node.NewVar(token.State, EType.Int));
                        }
                        break;
                    case Lexem._double:
                        if (isFunc)
                        {
                            tree.SetLeft(Node.NewVar(token.State, EType.Double, FuncName));
                        }
                        else
                        {
                            tree.SetLeft(Node.NewVar(token.State, EType.Double));
                        }
                        break;
                    default:
                        return false;
                }
                tree = tree.Left;
                return true;
            }
        }
        public static bool AddArray(int shift = 0, bool isFunc = false)
        {
            Token token = GetToken(shift);
            int lenght = int.Parse(GetToken(shift + 2).State);
            try
            {
                lenght = int.Parse(GetToken(shift + 2).State);
            }
            catch (System.Exception)
            {
                lenght = 100;
            }
            if (tree.FindUpVar(token) != null || tree.FindUpArray(token) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Массив {token.State} уже существует в этой области видимости");
                return false;
            }
            else
            {
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        if (isFunc)
                        {
                            tree.SetLeft(Node.NewArray(token.State, EType.Int, lenght, FuncName));
                        }
                        else
                        {
                            tree.SetLeft(Node.NewArray(token.State, EType.Int, lenght));
                        }
                        break;
                    case Lexem._double:
                        if (isFunc)
                        {
                            tree.SetLeft(Node.NewArray(token.State, EType.Double, lenght, FuncName));
                        }
                        else
                        {
                            tree.SetLeft(Node.NewArray(token.State, EType.Double, lenght));
                        }
                        break;
                    default:
                        return false;
                }
                tree = tree.Left;
                return true;
            }
        }
        #endregion

        #region Cast
        static EType CastMultType(EType hold, EType actual)
        {
            if (actual == EType.None)
            {
                return hold;
            }
            if (hold == EType._null || actual == EType._null)
            {
                return EType._null;
            }
            switch (hold)
            {
                case EType.Int:
                    if (actual == EType.Int || actual == EType.Char)
                    {
                        return EType.Int;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Double:
                    if (actual == EType.Int || actual == EType.Double || actual == EType.Char)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Char:
                    if (actual == EType.Int || actual == EType.Char)
                    {
                        return EType.Int;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                default:
                    return EType._null;
            }
        }
        static EType CastDivType(EType hold, EType actual)
        {
            if (actual == EType.None)
            {
                return hold;
            }
            if (hold == EType._null || actual == EType._null)
            {
                return EType._null;
            }
            switch (hold)
            {
                case EType.Int:
                    if (actual == EType.Int || actual == EType.Char || actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Double:
                    if (actual == EType.Int || actual == EType.Double || actual == EType.Char)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Char:
                    if (actual == EType.Int || actual == EType.Char || actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                default:
                    return EType._null;
            }
        }
        static EType CastPlusMinusType(EType hold, EType actual)
        {
            if (actual == EType.None)
            {
                return hold;
            }
            if (hold == EType._null || actual == EType._null)
            {
                return EType._null;
            }
            switch (hold)
            {
                case EType.Int:
                    if (actual == EType.Int)
                    {
                        return EType.Int;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else if (actual == EType.Char)
                    {
                        return EType.Int;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Double:
                    if (actual == EType.Int)
                    {
                        return EType.Double;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else if (actual == EType.Char)
                    {
                        return EType.Double;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Char:
                    if (actual == EType.Int)
                    {
                        return EType.Int;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Double;
                    }
                    else if (actual == EType.Char)
                    {
                        return EType.Char;
                    }
                    else
                    {
                        return EType._null;
                    }
                default:
                    return EType._null;
            }
        }
        static EType CastLogicType(EType hold, EType actual)
        {
            if (actual == EType.None)
            {
                return hold;
            }
            if (hold == EType._null || actual == EType._null)
            {
                return EType._null;
            }
            switch (hold)
            {
                case EType.Int:
                    if (actual == EType.Int)
                    {
                        return EType.Char;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Char;
                    }
                    else if (actual == EType.Char)
                    {
                        return EType.Char;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Double:
                    if (actual == EType.Int)
                    {
                        return EType.Char;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Char;
                    }
                    else if (actual == EType.Char)
                    {
                        return EType.Char;
                    }
                    else
                    {
                        return EType._null;
                    }
                case EType.Char:
                    if (actual == EType.Int)
                    {
                        return EType.Char;
                    }
                    else if (actual == EType.Double)
                    {
                        return EType.Char;
                    }
                    else if (actual == EType.Char)
                    {
                        return EType.Char;
                    }
                    else
                    {
                        return EType._null;
                    }
                default:
                    return EType._null;
            }
        }
        #endregion
    }
}
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
            tree = new Tree("base");
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
            tree = AddFunc(-1);
            ArgDeclar();
            FuncName = null;
            ShiftToken(2);
            MethodBlock();
            tree = tree.Up;
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
                if (!tree.Contains(GetToken(1), EElement.Var))
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
                if (!tree.Contains(GetToken(1), EElement.Array))
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
                    //VarAssign();
                    if (First(Lexem.TID))
                    {
                        ShiftToken(2);
                        Expression(GetToken(-1).State);
                    }
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
                EType type = tree.GetType(GetToken(1), EElement.Func);
                if (!tree.Contains(GetToken(1), EElement.Func))
                {
                    errors.Add(GetToken(1), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Функция не объявлена");
                }
                tree = tree.AddBranch(Node.Create(EElement.Func, EPurpose.Call, GetToken(1).State));
                ShiftToken(2);
                do
                {
                    if (First(Lexem.TRS))
                    {
                        break;
                    }
                    Expression(GetToken(1).State);
                } while (First(Lexem.TZpt) && ShiftToken());
                tree = tree.Up;
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
            tree = tree.AddBranch(Node.Create(EElement.For));
            if (First(Lexem._int) || First(Lexem._double))
            {
                ShiftToken();
                if (FC.VarAssign2(car))
                {
                    ShiftToken(-1);
                    VarDeclar();
                    ShiftToken(1);
                    //VarAssign();
                    Expression();
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
                if (First(Lexem.TID))
                {
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.IncL, GetToken(1).State));
                }
                else
                {
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.IncF, GetToken(1).State));
                }
                Inc();
            }
            else if (FC.Dec2(car))
            {
                if (First(Lexem.TID))
                {
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.DecL, GetToken(1).State));
                }
                else
                {
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.DecF, GetToken(1).State));
                }
                Dec();
            }
            if (First(Lexem.TRS, Lexem.TFLS))
            {
                ShiftToken(2);
            }
            MethodBlock();
            tree = tree.Up;
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
                    return tree.GetType(GetToken(-1), EElement.Var);
                }
                else
                {
                    ShiftToken(2);
                    return tree.GetType(GetToken(), EElement.Var);
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
                    return tree.GetType(GetToken(-1), EElement.Var);
                }
                else
                {
                    ShiftToken(2);
                    return tree.GetType(GetToken(), EElement.Var);
                }
            }
            return EType._null;
        }

        static EType Expression(string state = null)
        {

            EType result = EType.None;
            EType type;
            int start = car;
            if (state == null)
            {
                tree = tree.AddBranch(Node.Create(EElement.None, EPurpose.Expression, tree.Branches.Last().Node.State));
            }
            else
            {
                tree = tree.AddBranch(Node.Create(EElement.None, EPurpose.Expression, state));
            }
            do
            {
                type = Addend();
                result = CastPlusMinusType(type, result);
                switch (GetToken(1).Lexem)
                {
                    case Lexem.TGE:
                        tree.AddBranch(Node.Create(EElement.None, EPurpose.TGE));
                        break;
                    case Lexem.TGT:
                        tree.AddBranch(Node.Create(EElement.None, EPurpose.TGT));
                        break;
                    case Lexem.TLE:
                        tree.AddBranch(Node.Create(EElement.None, EPurpose.TLE));
                        break;
                    case Lexem.TLT:
                        tree.AddBranch(Node.Create(EElement.None, EPurpose.TLT));
                        break;
                    case Lexem.TNQ:
                        tree.AddBranch(Node.Create(EElement.None, EPurpose.TNQ));
                        break;
                    case Lexem.TEQ:
                        tree.AddBranch(Node.Create(EElement.None, EPurpose.TEQ));
                        break;
                    default:
                        break;
                }
            } while ((First(Lexem.TGE) || First(Lexem.TGT) || First(Lexem.TLE) || First(Lexem.TLT) || First(Lexem.TNQ) || First(Lexem.TEQ))
                && ShiftToken());
            if ((result == EType.None && type == EType._null) || (result == EType._null))
            {
                errors.Add(new Token(GetToken().Lexem, new Position(start + 1, car + 1), GetCarText(start + 1, car + 1), GetToken().Code), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Невозможно привести типы в данном выражении");
                tree = tree.Up;
                return EType._null;
            }
            if (result != EType.None)
            {
                tree = tree.Up;
                return result;
            }
            else
            {
                tree = tree.Up;
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
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.Add));
                }
                else if (First(Lexem.TMinus))
                {
                    result = CastPlusMinusType(type, result);
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.Sub));
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
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.Mul));
                }
                else if (First(Lexem.TDiv))
                {
                    result = CastDivType(type, result);
                    tree.AddBranch(Node.Create(EElement.None, EPurpose.Div));
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
                EType type = tree.GetType(GetToken(), EElement.Array);
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
                if (First(Lexem.TID))
                {
                    tree.AddBranch(Node.Create(EElement.Var, EPurpose.IncL, GetToken(1).State));
                }
                else
                {
                    tree.AddBranch(Node.Create(EElement.Var, EPurpose.IncF, GetToken(2).State));
                }
                return Inc();
            }
            else if (FC.Dec2(car))
            {
                if (First(Lexem.TID))
                {
                    tree.AddBranch(Node.Create(EElement.Var, EPurpose.DecL, GetToken(1).State));
                }
                else
                {
                    tree.AddBranch(Node.Create(EElement.Var, EPurpose.DecF, GetToken(2).State));
                }
                return Dec();
            }
            else if (First(Lexem.TConstInt))
            {
                ShiftToken();
                tree.AddBranch(Node.Create(EElement.Var, EPurpose.None, GetToken().State, EType.Int));
                return EType.Int;
            }
            else if (First(Lexem.TConstDouble))
            {
                ShiftToken();
                tree.AddBranch(Node.Create(EElement.Var, EPurpose.None, GetToken().State, EType.Double));
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
                if (!tree.Contains(GetToken(0), EElement.Var))
                {
                    errors.Add(GetToken(), car, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}: Переменная не объявлена");
                    return EType._null;
                }
                tree.AddBranch(Node.Create(EElement.Var, EPurpose.None, GetToken().State));
                return tree.GetType(GetToken(), EElement.Var);
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

        public static Tree AddFunc(int shift = 0)
        {
            Token token = GetToken(shift);
            if (tree.FindUp(token, EElement.Func) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Функция {token.State} уже существует в этой области видимости");
                return null;
            }
            else
            {
                FuncName = token.State;
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        return tree.AddBranch(Node.Create(EElement.Func, EPurpose.None, token.State, EType.Int));
                    case Lexem._double:
                        return tree.AddBranch(Node.Create(EElement.Func, EPurpose.None, token.State, EType.Double));
                    case Lexem._void:
                        return tree.AddBranch(Node.Create(EElement.Func, EPurpose.None, token.State, EType.Void));
                    default:
                        return null;
                }
            }
        }
        public static bool AddVar(int shift = 0, bool isFunc = false)
        {
            Token token = GetToken(shift);
            if (tree.FindUp(token, EElement.Var) != null || tree.FindUp(token, EElement.Array) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Переменная {token.State} уже существует в этой области видимости");
                return false;
            }
            else
            {
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        tree.AddBranch(Node.Create(EElement.Var, EPurpose.Declare, token.State, EType.Int, null, isFunc));
                        break;
                    case Lexem._double:
                        tree.AddBranch(Node.Create(EElement.Var, EPurpose.Declare, token.State, EType.Double, null, isFunc));
                        break;
                    default:
                        return false;
                }
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
            if (tree.FindUp(token, EElement.Var) != null || tree.FindUp(token, EElement.Array) != null)
            {
                errors.Add(token, car, $"{MethodInfo.GetCurrentMethod().Name} - Массив {token.State} уже существует в этой области видимости");
                return false;
            }
            else
            {
                switch (GetToken(shift - 1).Lexem)
                {
                    case Lexem._int:
                        tree.AddBranch(Node.Create(EElement.Array, EPurpose.Declare, token.State, EType.Int, null, isFunc, lenght));
                        break;
                    case Lexem._double:
                        tree.AddBranch(Node.Create(EElement.Array, EPurpose.Declare, token.State, EType.Double, null, isFunc, lenght));
                        break;
                    default:
                        return false;
                }
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
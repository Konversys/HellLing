using HellLing.Model;
using HellLing.Model.Classes;
using HellLing.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exception = HellLing.Model.Classes.Exception;

namespace HellLing.Core
{
    class Analyzer
    {
        /// <summary>
        /// Список ошибок
        /// </summary>
        static Errors errors { get; set; }
        /// <summary>
        /// Список лексем
        /// </summary>
        static List<Token> Tokens { get; set; }
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
            FC.SetData(tokens);
            Program();
            return errors;
        }

        static void Program()
        {
            if (FC.Method(car))
            {
                Method();
            }
            else if (FC.GlobalVar(car))
            {
                GlobalVar();
            }
            else
            {
                errors.Add(GetToken(), car, "Ожидались глобальные переменные или метод");
            }
        }
        static void Method()
        {
            ShiftToken(3);
            ArgDeclar();
            ShiftToken(2);
            MethodBlock();
            ShiftToken();
        }
        static void GlobalVar()
        {
            while (FC.VarDeclar(car) || FC.VarAssign(car) || FC.ArrAssign(car))
            {
                if (FC.VarDeclar(car))
                {
                    VarList();
                }
                else if (FC.VarAssign(car))
                {
                    VarAssign();
                }
                else if (FC.ArrAssign(car))
                {
                    ArrAssign();
                }
                else
                {
                    errors.Add(GetToken(), car, "Ожидалась операция с переменными и массивами");
                }

                if (First(Lexem.TTZpt))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, "Требуется ;");
                    ShiftToken();
                }
            }
        }

        static void VarList()
        {
            do
            {
                ShiftToken(1);
                if (FC.VarAssign(car))
                {
                    VarAssign();
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
                        errors.Add(GetToken(), car, "Требуется ]");
                        ShiftToken();
                    }
                    ShiftToken();
                }
                else if (First(Lexem.TID, Lexem.TZpt))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, "Требуется ID");
                    ShiftToken();
                }
            } while (First(Lexem.TZpt));
        }

        static void VarAssign()
        {
            if (FC.VarAssign(car))
            {
                ShiftToken(2);
                Expression();
            }
            else
            {
                errors.Add(GetToken(), car, "Ожидалось присвоение переменной");
                ShiftToken();
            }
        }

        static void ArrAssign()
        {
            if (FC.ArrAssign(car))
            {
                ShiftToken(2);
                Expression();
                if (First(Lexem.TCRS))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, "Ожидалось ]");
                    ShiftToken();
                }
                if (First(Lexem.TSave))
                {
                    ShiftToken();
                }
                else
                {
                    errors.Add(GetToken(), car, "Ожидалось =");
                    ShiftToken();
                }
            }
        }

        static void VarDeclar()
        {
            if (FC.VarDeclar(car))
            {
                
            }
            else
            {
                errors.Add(GetToken(), car, "Ожидался тип данных");
            }
            ShiftToken(2);
        }

        static void ArgDeclar()
        {
            do
            {
                if (FC.VarDeclar(car))
                {
                    ShiftToken(2);
                    if (First(Lexem.TCLS))
                    {
                        ShiftToken();
                        Expression();
                        if (First(Lexem.TCRS))
                        {
                            ShiftToken();
                        }
                    }
                }
            } while (First(Lexem.TZpt) && ShiftToken());
        }

        static void MethodBlock()
        {
            if (FC.ArrAssign(car))
            {
                ShiftToken(5);
                Expression();
            }
            else if (FC.MethodCall(car))
            {
                MethodCall();
            }
            else if (FC.For(car))
            {
                For();
            }
            else if (FC.VarDeclar(car))
            {
                VarList();
            }
            else if (First(Lexem._return))
            {
                ShiftToken();
                Expression();
            }
        }

        static void MethodCall()
        {
            if (FC.MethodCall(car))
            {
                ShiftToken(2);
                do
                {
                    Expression();
                } while (First(Lexem.TZpt) && ShiftToken());
                if (First(Lexem.TRS))
                {
                    ShiftToken();
                }
            }
        }

        static void For()
        {
            if (First(Lexem._for, Lexem.TLS))
            {
                ShiftToken(2);
            }
            if (First(Lexem._int) || First(Lexem._double))
            {
                ShiftToken();
            }
            if (FC.VarAssign(car))
            {
                VarAssign();
            }
            if (First(Lexem.TTZpt))
            {
                ShiftToken();
            }
            Expression();
            if (First(Lexem.TTZpt))
            {
                ShiftToken();
            }
            VarAssign();
            if (First(Lexem.TRS, Lexem.TFLS))
            {
                ShiftToken(2);
            }
            MethodBlock();
            if (First(Lexem.TFRS))
            {
                ShiftToken();
            }
        }

        static void Inc()
        {
            if (FC.Inc(car))
            {
                ShiftToken(2);
            }
        }

        static void Dec()
        {
            if (FC.Dec(car))
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
            else if (First(Lexem.TInc))
            {
                Inc();
            }
            else if (First(Lexem.TDec))
            {
                Dec();
            }
            else if (First(Lexem.TConstInt))
            {

            }
            else if (First(Lexem.TConstDouble))
            {

            }
            else if (First(Lexem.TConstChar))
            {

            }
            else if (FC.MethodCall(car))
            {
                MethodCall();
            }
            else
            {
                errors.Add(GetToken(), car, "Ожидалось элементарное выражение");
                ShiftToken();
            }
        }

        #region
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
    }
}
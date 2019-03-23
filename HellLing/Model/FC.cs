using HellLing.Model.Classes;
using HellLing.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Model
{
    class FC
    {
        public static bool Method3(int car)
        {
            Car = car;
            if (First(Lexem._int) || First(Lexem._double) || First(Lexem._void))
            {
                if (FTok(Lexem.TID, 2) && FTok(Lexem.TLS, 3))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool VarAssign2(int car)
        {
            Car = car;
            if (First(Lexem.TID, Lexem.TSave) && GetToken(3) != null)
            {
                return true;
            }
            return false;
        }
        public static bool ArrAssign5(int car)
        {
            Car = car;
            if (First(Lexem.TID, Lexem.TCLS, Lexem.TConstInt) || First(Lexem.TID, Lexem.TCLS, Lexem.TID) &&
                FTok(Lexem.TCRS, 4) && FTok(Lexem.TSave, 5) && GetToken(6) != null)
            {
                return true;
            }
            return false;
        }
        public static bool VarDeclar2(int car)
        {
            Car = car;
            if ((First(Lexem._int) || First(Lexem._double)) && FTok(Lexem.TID, 2))
            {
                return true;
            }
            return false;
        }
        public static bool MethodCall2(int car)
        {
            Car = car;
            if (First(Lexem.TID, Lexem.TLS))
            {
                return true;
            }
            return false;

        }
        public static bool GlobalVar0(int car)
        {
            if (VarAssign2(car) || ArrAssign5(car) || VarDeclar2(car))
            {
                return true;
            }
            return false;

        }
        public static bool For1(int car)
        {
            Car = car;
            if (First(Lexem._for))
            {
                return true;
            }
            return false;
        }
        public static bool Inc2(int car)
        {
            Car = car;
            if (First(Lexem.TInc, Lexem.TID) || First(Lexem.TID, Lexem.TInc))
            {
                return true;
            }
            return false;
        }
        public static bool Dec2(int car)
        {
            Car = car;
            if (First(Lexem.TDec, Lexem.TID) || First(Lexem.TID, Lexem.TDec))
            {
                return true;
            }
            return false;
        }

        // Operand data
        public static void SetData(List<Token> tokens)
        {
            Tokens = tokens;
        }
        static bool FTok(Lexem lexem, int shift = 1)
        {
            if (GetToken(shift) != null && GetToken(shift).Lexem == lexem)
                return true;
            return false;
        }        
        static List<Token> Tokens { get; set; }
        static int Car { get; set; }
        static Token GetToken(int shift = 0)
        {
            if (Car + shift >= Tokens.Count)
                return null;
            return Tokens[Car + shift];
        }
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
    }
}

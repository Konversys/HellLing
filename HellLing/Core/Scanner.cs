using HellLing.Model.Classes;
using HellLing.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellLing.Core
{
    /// <summary>
    /// Лексический сканер. Считывает лексемы
    /// </summary>
    class Scanner
    {
        static SortedList<string, short> progressIdentityLex;
        static Dictionary<string, short> keyWords;
        static List<Token> report;
        static readonly int maxlen = 32;
        static List<string> vsLexem;
        static List<string> tempLexem;
        static short code = 0;

        public static List<Token> Scan(string text, bool isTakeIng = false)
        {
            List<Token> result = Parse(text, isTakeIng);
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Lexem == Lexem.TChar)
                {
                    result[i].State = result[i].State.Replace("'", "");
                }
            }
            return result;
        }

        static List<Token> Parse(string text, bool isTakeIng = false)
        {
            text = text.Replace("\r", "").Replace("\n", "");
            int start = 0;
            int end;
            report = new List<Token>();
            InitLexemList();
            ProgressIdentityLex();
            string typ = "null";
            char ch;
            for (int pos = 0; pos < text.Length + 1; pos++)
            {
                if (text.Length == pos)
                {
                    Token token = new Token();
                    if (code != 0)
                    {
                        end = start;
                        code = CheckKeyWords(text, start, pos);
                        token.Set((Lexem)code, new Position(start, pos), text);
                    }
                    else
                    {
                        end = start;
                        code = CheckKeyWords(text, start, pos);
                        token.Set((Lexem)code, new Position(start, pos), text);
                    }
                    if (typ != "Com" && typ != "Ch")
                    {
                        for (int i = start; i < pos; i++)
                            if (Ident.OPart(text[i]) != 0)
                            {
                                code = Ident.OPart(text[i]);
                                report.Add(new Token((Lexem)code, new Position(i, i + 1), text));
                            }
                    }
                    else
                        report.Add(new Token((Lexem)code, new Position(start, end), text));
                    report.Add(new Token((Lexem)300, new Position(start, end), text));
                    if (!isTakeIng)
                    {
                        report.Insert(0, new Token(Lexem.TSOF, new Position(start, end), text));
                        return report.Where(x => x.Lexem != Lexem.TIgn).ToList();
                    }
                    report.Insert(0, new Token(Lexem.TSOF, new Position(start, end), text));
                    return report;
                }
                ch = text[pos];
                foreach (var item in vsLexem)
                {
                    if (tempLexem.Contains(item))
                    {
                        int tm = RebuildKey(ch, item);
                        if (tm == 0 && tempLexem.Contains(item))
                        {
                            tempLexem.Remove(item);
                        }
                        if (tempLexem.Count == 1)
                        {
                            foreach (var key in progressIdentityLex)
                                if (key.Value != 0)
                                {
                                    code = key.Value;
                                }
                            typ = tempLexem[0];
                        }
                        if (tempLexem.Count == 0)
                        {
                            Token token = new Token();
                            if (code == 0)
                            {
                                token.Set((Lexem)403, new Position(start, pos), text);
                                if (report.Last().Code != 403)
                                {
                                    end = start;
                                    report.Add(new Token((Lexem)code, new Position(start, end), text));
                                    start = pos;
                                }
                            }
                            else
                            {
                                end = pos;
                                code = CheckKeyWords(text, start, end);
                                token.Set((Lexem)403, new Position(start, pos), text);
                                if (item == "OPart")
                                    for (int i = start; i < end; i++)
                                    {
                                        code = Ident.OPart(text[i]);
                                        report.Add(new Token((Lexem)code, new Position(i, i + 1), text));
                                    }

                                else
                                    report.Add(new Token((Lexem)code, new Position(start, end), text));
                                start = end;
                                code = 0;
                                pos--;
                            }
                            InitLexemList();
                            break;
                        }
                    }
                }
            }
            if (!isTakeIng)
            {
                return report.Where(x => x.Lexem != Lexem.TIgn).ToList();
            }
            return report;
        }
        static short RebuildKey(char ch, string name)
        {
            progressIdentityLex.TryGetValue(name, out short key);
            key = Ident.Mainframe(name, ch, key);
            progressIdentityLex.Remove(name);
            progressIdentityLex.Add(name, key);
            progressIdentityLex.TryGetValue(name, out key);
            return key;
        }
        static void InitLexemList()
        {
            if (keyWords == null)
            {
                keyWords = new Dictionary<string, short>
                {
                    { "int", Ident.D(Lexem._int) },
                    { "double", Ident.D(Lexem._double) },
                    { "Main", Ident.D(Lexem._main) },
                    { "for", Ident.D(Lexem._for) },
                    { "const", Ident.D(Lexem._const) },
                    { "void", Ident.D(Lexem._void) },
                    { "return", Ident.D(Lexem._return) },
                };
            }
            if (vsLexem == null)
            {
                vsLexem = new List<string>
                {
                    "ID",
                    "Ign",
                    "Com",
                    "COEQ",
                    "DTypes",
                    "Ch",
                    "OPart",
                    "Inc",
                    "Dec"
                };
            }
            tempLexem = new List<string>();
            tempLexem.Clear();
            foreach (var item in vsLexem)
            {
                tempLexem.Add(item);
            }
        }
        static void ProgressIdentityLex()
        {
            progressIdentityLex = new SortedList<string, short>();
            foreach (var item in vsLexem)
            {
                progressIdentityLex.Add(item, 0);
            }
        }
        static short CheckKeyWords(string text, int start, int end)
        {
            if (code == Ident.D(Lexem.TConstInt))
            {
                if (text.Substring(start, end - start).Length > maxlen)
                    return Ident.D(Lexem.TError);
            }
            if (code == Ident.D(Lexem.TID))
            {
                string id = text.Substring(start, end - start);
                foreach (var item in keyWords)
                {
                    if (item.Key == id)
                    {
                        return item.Value;
                    }
                }
            }
            return code;
        }
    }
    class Ident
    {
        public static short Mainframe(string typ, char ch, short key)
        {
            switch (typ)
            {
                case "ID":
                    return ID(ch, key);
                case "Ign":
                    return Ign(ch);
                case "Com":
                    return Com(ch, key);
                case "COEQ":
                    return COEQ(ch, key);
                case "DTypes":
                    return DTypes(ch, key);
                case "Ch":
                    return Ch(ch, key);
                case "OPart":
                    return OPart(ch);
                case "Inc":
                    return Inc(ch, key);
                case "Dec":
                    return Dec(ch, key);
                default:
                    return D(Lexem.TError);
            }
        }
        static short ID(char ch, short key)
        {
            if (Char.IsLetter(ch))
            {
                return D(Lexem.TID);
            }
            else if (Char.IsDigit(ch) && key != 0)
            {
                return D(Lexem.TID);
            }
            else
            {
                return 0;
            }
        }
        static short Ign(char ch)
        {
            if (ch == '\n' || ch == '\t' || ch == ' ')
            {
                return D(Lexem.TIgn);
            }
            return 0;
        }
        static short Com(char ch, short key)
        {
            if (ch == '/' && key == -1)
            {
                return D(Lexem.TCom);
            }
            else if (key == D(Lexem.TCom) && ch != '\n')
            {
                return D(Lexem.TCom);
            }
            else if (ch == '/')
            {
                return -1;
            }
            return 0;
        }
        static short COEQ(char ch, short key)
        {
            switch (ch)
            {
                case '<':
                    return D(Lexem.TLT);
                case '>':
                    return D(Lexem.TGT);
                case '!':
                    return D(Lexem.TNot);
                case '=':
                    if (key == D(Lexem.TLT))
                    {
                        return D(Lexem.TLE);
                    }
                    if (key == D(Lexem.TGT))
                    {
                        return D(Lexem.TGE);
                    }
                    if (key == D(Lexem.TNot))
                    {
                        return D(Lexem.TNQ);
                    }
                    if (key == D(Lexem.TSave))
                    {
                        return D(Lexem.TEQ);
                    }
                    else
                    {
                        return D(Lexem.TSave);
                    }
                default:
                    return 0;
            }
        }
        static short DTypes(char ch, short key)
        {
            if (ch == '.' && key == D(Lexem.TConstInt))
            {
                return -1;
            }
            else if (Char.IsDigit(ch) && key == -1)
            {
                return D(Lexem.TConstDouble);
            }
            else if (Char.IsDigit(ch) && key == D(Lexem.TConstDouble))
            {
                return D(Lexem.TConstDouble);
            }
            else if (Char.IsDigit(ch))
            {
                return D(Lexem.TConstInt);
            }
            else
                return 0;
        }
        static short Ch(char ch, short key)
        {
            if (key == -1)
            {
                return -2;
            }
            else if (ch == '\'' && key == -2)
            {
                return D(Lexem.TChar);
            }
            else if (ch == '\'' && key == 0)
            {
                return -1;
            }
            return 0;
        }
        public static short OPart(char ch)
        {
            switch (ch)
            {
                case '*':
                    return D(Lexem.TMult);
                case '/':
                    return D(Lexem.TDiv);
                case ';':
                    return D(Lexem.TTZpt);
                case '.':
                    return D(Lexem.TDot);
                case ',':
                    return D(Lexem.TZpt);
                case '(':
                    return D(Lexem.TLS);
                case ')':
                    return D(Lexem.TRS);
                case '{':
                    return D(Lexem.TFLS);
                case '}':
                    return D(Lexem.TFRS);
                case '[':
                    return D(Lexem.TCLS);
                case ']':
                    return D(Lexem.TCRS);
                default:
                    return 0;
            }
        }
        static short Inc(char ch, short key)
        {
            if (ch == '+' && key == 0)
            {
                return D(Lexem.TPlus);
            }
            else if (ch == '+' && key == D(Lexem.TPlus))
            {
                return D(Lexem.TInc);
            }
            else
                return 0;
        }
        static short Dec(char ch, short key)
        {
            if (ch == '-' && key == 0)
            {
                return D(Lexem.TMinus);
            }
            else if (ch == '-' && key == D(Lexem.TMinus))
            {
                return D(Lexem.TDec);
            }
            else
                return 0;
        }
        public static short D(Lexem item)
        {
            return (short)item;
        }
    }
}

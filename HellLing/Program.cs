using HellLing.Core;
using HellLing.External;
using HellLing.Model.Classes;
using System;
using System.Collections.Generic;
using Exception = HellLing.Model.Classes.Exception;

namespace HellLing
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = FileControl.Read();
            List<Token> tokens = Scanner.Scan(text, false);
            Console.WriteLine(GetStringScanner(tokens));
            Console.ReadKey();
        }
        static string GetStringScanner(List<Token> tokens)
        {
            string result = "";
            int count = 0;
            foreach (var item in tokens)
            {
                result += String.Format("{0:d2}: {1}\n", count++, item.ToString());
            }
            result += "\n======================================================\n";
            FileControl.Write(result);
            return result;
        }
        static string GetStringAnalyzer(List<Exception> tokens)
        {
            if (tokens.Count == 0)
            {
                return "\n====================  Ошибок нет  ====================\n";
            }
            string result = "";
            foreach (var item in tokens)
            {
                result += String.Format("{0}\n", item.ToString());
            }
            result += "\n======================================================\n";
            return result;
        }
    }
}

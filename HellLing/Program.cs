using HellLing.Core;
using HellLing.External;
using HellLing.Model.Classes;
using HellLing.Model.STree;
using System;
using System.Collections.Generic;
using Exception = HellLing.Model.Classes.Exception;

namespace HellLing
{
    /// <summary>
    /// Главная программа для языка C++
    /// Типы: int double
    /// Операции: Унарные и банарные арифметические, сравнения
    /// Операторы: Присваивание и for простейшей структуры (цикл по одной переменной с заданным шагом)
    /// Операнды: Простые переменные, элементы одномерных массивов
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string text = FileControl.Read();
            List<Token> tokens = Scanner.Scan(text, false);
            Console.WriteLine(GetStringScanner(tokens));
            Errors errors = Analyzer.Start(tokens);
            Console.WriteLine(GetStringAnalyzer(errors));
            Tree tree = Analyzer.tree;
            errors.PrintErrorCode(); 
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
        static string GetStringAnalyzer(Errors errors)
        {
            if (errors.GetErrors().Count == 0)
            {
                return "\n====================  Ошибок нет  ====================\n";
            }
            string result = "";
            foreach (var item in errors.GetErrors())
            {
                result += String.Format("{0}\n", item);
            }
            result += "\n======================================================\n";
            return result;
        }
    }
}

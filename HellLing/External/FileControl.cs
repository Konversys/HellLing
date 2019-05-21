using System.IO;

namespace HellLing.External
{
    /// <summary>
    /// Интерфейс файловой системы
    /// </summary>
    class FileControl
    {
        /// <summary>
        /// Считать файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static string Read(string path = @"D:\ASTU\8s\ТЯПиМТ\HellLing\HellLing\Data\input.txt")
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string text = sr.ReadToEnd();
                sr.Close();
                return text;
            }
        }
        /// <summary>
        /// Записать файл
        /// </summary>
        /// <param name="text">Текст для записи</param>
        /// <param name="path">Путь к файлу</param>
        public static void Write(string text, string path = @"D:\ASTU\7s\МАвТФЯ\HellLing\HellLing\Data\output.txt")
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.Write(text);
                sw.Close();
            }
        }
    }
}

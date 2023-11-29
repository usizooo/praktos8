using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lab_8
{
    // Создадим отдельный класс для хранения рекордов, чтобы можно было его серелиализовать и десериализовать
    public class Records
    {
        // Рекорды будем хранить в словаре. Ключ - имя игрока, значение - рекорд
        public Dictionary<string, Record> recordsDictionary { get; set; }
        public Records(Dictionary<string, Record> recordsDictionary) => this.recordsDictionary = recordsDictionary;
    }

    public static class TableOfRecords
    {
        private static Records records = new Records(new Dictionary<string, Record>());

        // Добавление нового рекорда
        public static void AddRecord(Record record)
        {
            // Если человек уже сохранём, то только лишь обновляем данные
            if (records.recordsDictionary.ContainsKey(record.Name))
            {
                records.recordsDictionary[record.Name].RecordSymbolsPerMinute = record.RecordSymbolsPerMinute;
                records.recordsDictionary[record.Name].RecordSymbolsPerSecond = record.RecordSymbolsPerSecond;
            }
            // Если ещё нет, то создаём новый рекорд
            else
            {
                records.recordsDictionary.Add(record.Name, record);
            }
        }
        
        // Вывод на экран списка рекордов
        public static void PrintRecords()
        {
            Console.WriteLine("Таблица рекордов:");
            Console.WriteLine(new string('-', Console.WindowWidth));
            foreach (var record in records.recordsDictionary)
            {
                Console.WriteLine(record.Value);
            }
        }

        // Сериализация (указываем путь к файлу)
        public static void Serialization(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(JsonConvert.SerializeObject(records));
            }
        }
        // Десериализация (указываем путь к файлу)
        public static void Deserialization(string path)
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                string jsonData = streamReader.ReadToEnd();
                if (jsonData == string.Empty)
                {
                    return;
                }
                records = JsonConvert.DeserializeObject<Records>(jsonData) 
                    ?? throw new NullReferenceException("Данные файла повреждены повреждены.");
            }
        }
    }
}
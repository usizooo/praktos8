using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lab_8
{
    public class Records
    {
        public Dictionary<string, Record> recordsDictionary { get; set; }
        public Records(Dictionary<string, Record> recordsDictionary) => this.recordsDictionary = recordsDictionary;
    }

    public static class TableOfRecords
    {
        private static Records records = new Records(new Dictionary<string, Record>());

        public static void AddRecord(Record record)
        {
            if (records.recordsDictionary.ContainsKey(record.Name))
            {
                records.recordsDictionary[record.Name].RecordSymbolsPerMinute = record.RecordSymbolsPerMinute;
                records.recordsDictionary[record.Name].RecordSymbolsPerSecond = record.RecordSymbolsPerSecond;
            }
            else
            {
                records.recordsDictionary.Add(record.Name, record);
            }
        }
   
        public static void PrintRecords()
        {
            Console.WriteLine("Таблица рекордов:");
            Console.WriteLine(new string('-', Console.WindowWidth));
            List<Record> _records = records.recordsDictionary.Select(x => x.Value).ToList();
            _records.Sort(new RecordComparer());
            _records.Reverse();
            _records.ForEach(x => Console.WriteLine(x));
        }

        public static void Serialization(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(JsonConvert.SerializeObject(records));
            }
        }
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
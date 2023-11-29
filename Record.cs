using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Lab_8
{
    // Класс, который хранит в себе рекорды отдельно взятого игрока
    public class Record
    {
        public readonly string Name;

        private int recordSymbolsPerMinute = 0;
        // Создаём сложное свойство: проверяем при обновлении значения,
        // больше ли оно предыдущего, чтобы сохранять только лучшие результаты
        public int RecordSymbolsPerMinute 
        {
            get => recordSymbolsPerMinute;
            set
            {
                if (value > recordSymbolsPerMinute)
                {
                    recordSymbolsPerMinute = value;
                }
            }
        }

        private float recordSymbolsPerSecond = 0;
        // Создаём сложное свойство: проверяем при обновлении значения,
        // больше ли оно предыдущего, чтобы сохранять только лучшие результаты
        public float RecordSymbolsPerSecond
        {
            get => recordSymbolsPerSecond;
            set
            {
                if (value > recordSymbolsPerSecond)
                {
                    recordSymbolsPerSecond = value;
                }
            }
        }

        public Record(string name) => Name = name;

        // Помечаем этот конструктор как такой, который вызывается при десериализации
        [JsonConstructor]
        public Record(string name, int recordSymbolsPerMinute, float recordSymbolsPerSecond) : this(name)
        {
            RecordSymbolsPerMinute = recordSymbolsPerMinute;
            RecordSymbolsPerSecond = recordSymbolsPerSecond;
        }

        // Перегружаем метод ToString() для вывода данных
        public override string ToString()
        {
            return $"{Name, -25}" +
                $"{RecordSymbolsPerMinute, 5} символов в минуту\t" +
                $"{RecordSymbolsPerSecond, 5} символов в секунду";
        }
    }
}

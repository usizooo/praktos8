using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Lab_8
{
    // Основной класс отвечающий за работу игры
    public static class Game
    {
        // Кол-во секунд, которое даётся на прохождение уровня
        public static int TotalSeconds { get; private set; }

        // Главный метод - запуск игры
        public static void Run(string path, int totalSeconds, List<string> levels)
        {
            // Десериализуем данные рекордов
            TableOfRecords.Deserialization(path);
            // Устанавливаем время на прохождение уровня
            TotalSeconds = totalSeconds;
            // Инициализируем уровни
            Levels.InitLevels(levels);

            int currentLevel = 0;
            bool window = true;

            while (window)
            {
                // Просим ввести имя игрока
                var name = InitNewPlayer();
                // Запускаем уровень с нужным текстом от лица опредлённого игрока
                Play(Levels.Texts[currentLevel], name);
                // После увеличиваем уровень на единицу. Если он становится больше
                // или равен кол-во всех уровней, тогда обнуляем
                currentLevel = currentLevel + 1 >= Levels.Texts.Count ? 0 : currentLevel + 1;
                // Выводим на экран таблицу рекордлв
                TableOfRecords.PrintRecords();

                // Считываем нажатие по клавиатуре
                ConsoleKeyInfo action = Console.ReadKey();
                switch (action.Key)
                {
                    // Если Enter, то продолжаем игру
                    case ConsoleKey.Enter:
                        break;
                    default:
                        window = false;
                        break;
                }

                Console.Clear();
            }

            // После выхода из игры сериализуем данные в файл
            // ВАЖНО: при закрытие программы по крестику, данные НЕ сериализуются
            TableOfRecords.Serialization(path);
        }

        // Ввод имени игрока
        private static string InitNewPlayer()
        {
            Console.WriteLine("Введите имя для таблицы рекордов:");
            string name = Console.ReadLine() 
                ?? throw new ArgumentException("Ошибка при введении имени");
            return name;
        }

        // Запуск уровня
        private static void Play(string text, string name)
        {
            Console.Clear();
            Console.WriteLine(text);
            // Переменная отвечающая за прогресс игрока в тексте
            int currentSymbol = 0;
            // Устанавливаем и запускаем таймер
            Stopwatch timer = new Stopwatch();
            timer.Start();
            // ВАЖНО: locker нужен для того, чтобы при использовании разделяемого ресурса
            // (в нашем случае это консоль) у разных потоков не было конфликтов.
            // Мы блокируем этот объект пока выполняется один поток, второй в этот момент ничего не делает.
            // Таким образом мы разрешаем конфликт доступа к разделяемым ресурсам.
            object locker = new object();
            bool isPlay = true;

            // В отдельном потоке запустим цикл, который будет следить за нажатием клавиш пользователем.
            // Нам нужен другой поток для того, чтобы таймер тикал и (что важно) выводился на экран
            // вне зависимости от того, нажимает на клавиши в данный момент пользователь или нет.
            Thread levelRendering = new Thread(_ =>
            {
                while (timer.Elapsed.Seconds < TotalSeconds && currentSymbol < text.Length && isPlay)
                {
                    lock(locker)
                    {
                        // Ставим курсор туда, где сейчас по тексту находится игрок
                        Console.SetCursorPosition(currentSymbol % Console.BufferWidth, currentSymbol / Console.BufferWidth);

                        // Считываем нажатие
                        ConsoleKeyInfo symbol = Console.ReadKey(true);

                        // Если Enter, то заканчиваем уровень
                        if (symbol.Key == ConsoleKey.Enter)
                        {
                            isPlay = false;
                            break;
                        }

                        // Ставим курсор туда, где сейчас по тексту находится игрок
                        Console.SetCursorPosition(currentSymbol % Console.BufferWidth, currentSymbol / Console.BufferWidth);

                        // Проверяем правильно ли игрок ввёл символ
                        if (symbol.KeyChar == text[currentSymbol])
                        {
                            // Если да, то красим синим и продвигаемся по уровню
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(symbol.KeyChar);
                            currentSymbol++;
                        }
                        else
                        {
                            // Если нет, то красим в красный и НЕ продвигаемся по уровню
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(text[currentSymbol]);
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                isPlay = false;
            });

            // Запускаем поток
            levelRendering.Start();

            // В текущем потоке отрисовываем таймер и надпись про Enter
            while (timer.Elapsed.Seconds < TotalSeconds && isPlay)
            {
                lock (locker)
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine($"{timer.Elapsed.Minutes}:{TotalSeconds - timer.Elapsed.Seconds}");
                    Console.SetCursorPosition(0, 8);
                    Console.Write("Нажмите Enter, когда будете готовы");
                }
            }

            // Сливаем обратно с текущим поток выделенный поток
            levelRendering.Join();

            // Пишем "Стоп!" и останавливаем выполнение программы на 2 секунды
            Console.SetCursorPosition(0, 10);
            Console.Write("Стоп!");
            Thread.Sleep(2000);

            // Добавляем новый рекорд в таблицу
            TableOfRecords.AddRecord(new Record(name, currentSymbol, 0));

            // Очищаем буфер консоли от лишних символов оставшихся после игрока
            while (Console.KeyAvailable)
                Console.ReadKey(true); // skips previous input chars

            Console.Clear();
        }
    }
}
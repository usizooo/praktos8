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
    public static class Game
    {
        public static int TotalSeconds { get; private set; }

        public static void Run(string path, int totalSeconds, List<string> levels)
        {
            TableOfRecords.Deserialization(path);
            TotalSeconds = totalSeconds;
            Levels.InitLevels(levels);

            int currentLevel = 0;
            bool window = true;

            while (window)
            {
                var name = InitNewPlayer();
                Play(Levels.Texts[currentLevel], name);
                currentLevel = currentLevel + 1 >= Levels.Texts.Count ? 0 : currentLevel + 1;
                TableOfRecords.PrintRecords();
                Console.WriteLine("Нажмите Enter, чтобы продолжить");

                ConsoleKeyInfo action = Console.ReadKey();
                switch (action.Key)
                {
                    case ConsoleKey.Enter:
                        break;
                    default:
                        window = false;
                        break;
                }

                Console.Clear();
            }

            TableOfRecords.Serialization(path);
        }

        private static string InitNewPlayer()
        {
            Console.WriteLine("Введите имя для таблицы рекордов:");
            string name = Console.ReadLine() 
                ?? throw new ArgumentException("Ошибка при введении имени");
            return name;
        }

        private static void Play(string text, string name)
        {
            Console.Clear();
            Console.WriteLine(text);
            int currentSymbol = 0;
            Stopwatch timer = new Stopwatch();
            
            object locker = new object();
            bool isPlay = true;

            Thread levelRendering = new Thread(_ =>
            {
                while (timer.ElapsedMilliseconds / 1000 < TotalSeconds && isPlay)
                {
                    lock (locker)
                    {
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine($"Осталось: {TotalSeconds - timer.Elapsed.Seconds} {"сек.", -6}");
                        Console.SetCursorPosition(0, 8);
                        Console.Write("Нажмите Enter, когда будете готовы");
                    }
                }
                isPlay = false;
            });

            levelRendering.Start();
            timer.Start();


            while (timer.ElapsedMilliseconds / 1000 < TotalSeconds && currentSymbol < text.Length && isPlay)
            {
                lock(locker)
                {
                    Console.SetCursorPosition(currentSymbol % Console.BufferWidth, currentSymbol / Console.BufferWidth);
                    Console.CursorVisible = true;
                    
                    ConsoleKeyInfo symbol = default(ConsoleKeyInfo);
                    while (Console.KeyAvailable)
                        symbol = Console.ReadKey(true);

                    if (symbol.Key == ConsoleKey.Enter)
                    {
                        isPlay = false;
                        break;
                    }

                    Console.CursorVisible = false;

                    Console.SetCursorPosition(currentSymbol % Console.BufferWidth, currentSymbol / Console.BufferWidth);

                    if (symbol.KeyChar == text[currentSymbol])
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(symbol.KeyChar);
                        currentSymbol++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(text[currentSymbol]);
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            isPlay = false;

            levelRendering.Join();

            Console.SetCursorPosition(0, 10);
            Console.Write($"{"Стоп!", -30}");
            Thread.Sleep(2000);

            TableOfRecords.AddRecord(new Record
                (
                    name, 
                    timer.Elapsed.Minutes < 1 ? currentSymbol : (float) currentSymbol / timer.Elapsed.Minutes, 
                    (float) currentSymbol / timer.Elapsed.Seconds)
                );

            while (Console.KeyAvailable)
                Console.ReadKey(true); 

            Console.Clear();
        }
    }
}
using System;
using System.IO;


namespace PM_02_520_Bushak
{
    class Program
    {
        static int[,] costs;
        static int[,] solution;
        static int[] supply;
        static int[] demand;
        static int numSources;
        static int numDestinations;

        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в программу для решения транспортной задачи!");

            while (true)
            {
                // Вывод доступных команд
                Console.WriteLine("\nДоступные команды:");
                Console.WriteLine("1. ввод - ввести исходные данные вручную");
                Console.WriteLine("2. файл - загрузить исходные данные из файла");
                Console.WriteLine("3. рассчитать - рассчитать опорный план");
                Console.WriteLine("4. сохранить - сохранить опорный план в файл");
                Console.WriteLine("5. помощь - ознакомиться с командами программы");
                Console.WriteLine("6. выход - выйти из программы");

                // Ввод команды от пользователя
                Console.Write("\nВведите команду: ");
                var command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "ввод":
                        InputDataManually();
                        break;
                    case "файл":
                        InputDataFromFile();
                        break;
                    case "рассчитать":
                        CalculateNorthWestCorner();
                        break;
                    case "сохранить":
                        SaveToFile();
                        break;
                    case "помощь":
                        ShowHelp();
                        break;
                    case "выход":
                        return;
                    default:
                        Console.WriteLine("Некорректная команда. Попробуйте еще раз.");
                        break;
                }
            }
        }

        // Ввод исходных данных вручную
        static void InputDataManually()
        {
            numSources = ReadInteger("Введите количество заводов: ");
            numDestinations = ReadInteger("Введите количество поликлиник: ");

            supply = ReadArray("Введите запасы для каждого завода:", numSources);
            demand = ReadArray("Введите потребности для каждой поликлиники:", numDestinations);

            Console.WriteLine("Введите стоимости перевозок:");
            costs = ReadMatrix(numSources, numDestinations);
        }

        // Чтение целого числа
        static int ReadInteger(string prompt)
        {
            int value;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out value));
            return value;
        }

        // Чтение неотрицательного целого числа
        static int ReadNonNegativeInteger(string prompt)
        {
            int value;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out value) || value < 0);
            return value;
        }

        // Чтение массива целых чисел
        static int[] ReadArray(string prompt, int length)
        {
            int[] array = new int[length];
            Console.WriteLine(prompt);
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadNonNegativeInteger($"Ввод для {i + 1}-го элемента: ");
            }
            return array;
        }

        // Чтение матрицы стоимостей
        static int[,] ReadMatrix(int rows, int cols)
        {
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = ReadNonNegativeInteger($"Завод {i + 1} -> Поликлиника {j + 1}: ");
                }
            }
            return matrix;
        }

        // Загрузка исходных данных из файла
        static void InputDataFromFile()
        {
            Console.WriteLine("Пример вида пути к файлу: C:\\data.txt");
            Console.Write("Введите путь к файлу: ");
            var path = Console.ReadLine();

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    numSources = int.Parse(sr.ReadLine());
                    numDestinations = int.Parse(sr.ReadLine());

                    supply = ReadIntArray(sr, numSources);
                    demand = ReadIntArray(sr, numDestinations);

                    costs = new int[numSources, numDestinations];
                    for (int i = 0; i < numSources; i++)
                    {
                        var line = sr.ReadLine().Split(' ');
                        for (int j = 0; j < numDestinations; j++)
                        {
                            costs[i, j] = int.Parse(line[j]);
                        }
                    }
                }
                Console.WriteLine("Данные успешно загружены из файла.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        // Чтение массива целых чисел из файла
        static int[] ReadIntArray(StreamReader sr, int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = int.Parse(sr.ReadLine());
            }
            return array;
        }

        // Расчет опорного плана методом северо-западного угла
        static void CalculateNorthWestCorner()
        {
            if (costs == null)
            {
                Console.WriteLine("Исходные данные не введены.");
                return;
            }

            solution = new int[numSources, numDestinations];

            int i = 0, j = 0;
            while (i < numSources && j < numDestinations)
            {
                int min = Math.Min(supply[i], demand[j]);
                solution[i, j] = min;
                supply[i] -= min;
                demand[j] -= min;

                if (supply[i] == 0) i++;
                if (demand[j] == 0) j++;
            }

            Console.WriteLine("Опорный план:");
            PrintMatrix(solution);
            Console.WriteLine($"Общая стоимость грузоперевозки: {CalculateTotalCost()}");
        }

        // Вычисление общей стоимости грузоперевозки
        static int CalculateTotalCost()
        {
            int totalCost = 0;
            for (int i = 0; i < numSources; i++)
            {
                for (int j = 0; j < numDestinations; j++)
                {
                    totalCost += solution[i, j] * costs[i, j];
                }
            }
            return totalCost;
        }

        // Вывод матрицы на консоль
        static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j],5}");
                }
                Console.WriteLine();
            }
        }

        // Сохранение опорного плана в файл
        static void SaveToFile()
        {
            if (solution == null)
            {
                Console.WriteLine("Опорный план не рассчитан.");
                return;
            }

            Console.WriteLine("Пример вида пути к файлу для сохранения: C:\\result.txt");
            Console.Write("Введите путь для сохранения файла: ");
            var path = Console.ReadLine();

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("Опорный план:");
                    WriteMatrixToFile(sw, solution);
                    sw.WriteLine($"Общая стоимость грузоперевозки: {CalculateTotalCost()}");
                }
                Console.WriteLine("Опорный план успешно сохранен в файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        // Запись матрицы в файл
        static void WriteMatrixToFile(StreamWriter sw, int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    sw.Write($"{matrix[i, j],5}");
                }
                sw.WriteLine();
            }
        }

        // Вывод справки по командам
        static void ShowHelp()
        {
            Console.WriteLine("\nКоманды программы:");
            Console.WriteLine("1. ввод - ввести исходные данные вручную");
            Console.WriteLine("2. файл - загрузить исходные данные из файла");
            Console.WriteLine("3. рассчитать - рассчитать опорный план");
            Console.WriteLine("4. сохранить - сохранить опорный план в файл");
            Console.WriteLine("5. помощь - ознакомиться с командами программы");
            Console.WriteLine("6. выход - выйти из программы");
        }
    }
}



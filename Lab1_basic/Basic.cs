using System.Diagnostics;

namespace Lab1_basic
{
    internal class Basic
    {
        static void Main(string[] args)
        {
            string fileA = "file_A.txt";
            string fileB = "file_B.txt";
            string fileC = "file_C.txt";

            // генерація даних в file_A.txt
            GenerateData(fileA);
            
            var stopwatch = Stopwatch.StartNew();
            // сортування
            NaturalMergeSort(fileA, fileB, fileC);
            
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00} хвилин, {1:00} секунд, {2:000} мілісекунд",
                ts.Minutes, ts.Seconds, ts.Milliseconds);
            
            Console.WriteLine("Сортування завершено за: " + elapsedTime);
        }

        static void GenerateData(string fileName)
        {
            Random rand = new Random();
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < 100250000; i++)
                {
                    int number = rand.Next(100_000_000); // около 10 байт
                    writer.WriteLine(number);
                }
            }
        }

        static void NaturalMergeSort(string fileA, string fileB, string fileC)
        {
            int numRuns;

            do
            {
                // Розбиття fileA на серії і їх запис в fileB та fileC
                numRuns = SplitRuns(fileA, fileB, fileC);

                // Злиття серій із fileB і fileC в fileA
                MergeRuns(fileA, fileB, fileC);

            } while (numRuns > 1);
        }

        static int SplitRuns(string inputFile, string outputFile1, string outputFile2)
        {
            using (StreamReader reader = new StreamReader(inputFile))
            using (StreamWriter writer1 = new StreamWriter(outputFile1))
            using (StreamWriter writer2 = new StreamWriter(outputFile2))
            {
                int numRuns = 0;
                StreamWriter currentWriter = writer1;
                int? prevValue = null;
                string? line;
                bool firstElement = true;
                while ((line = reader.ReadLine()) != null)
                {
                    int value = int.Parse(line.Trim());
                    if (!firstElement && value < prevValue)
                    {
                        // Початок нової серії
                        numRuns++;
                        // Перехід на інший файл для запису
                        currentWriter = currentWriter == writer1 ? writer2 : writer1;
                    }
                    currentWriter.WriteLine(value);
                    prevValue = value;
                    firstElement = false;
                }
                numRuns++; // Врахування останньої серії
                return numRuns;
            }
        }

        static void MergeRuns(string outputFile, string inputFile1, string inputFile2)
        {
            using (StreamReader reader1 = new StreamReader(inputFile1))
            using (StreamReader reader2 = new StreamReader(inputFile2))
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                bool endOfFile1 = false, endOfFile2 = false;
                int? nextValue1 = null, nextValue2 = null;

                while (!endOfFile1 || !endOfFile2)
                {
                    // Читання наступної серії з кожного файлу
                    Queue<int> run1 = ReadNextRun(reader1, ref endOfFile1, ref nextValue1);
                    Queue<int> run2 = ReadNextRun(reader2, ref endOfFile2, ref nextValue2);
                    
                    while (run1.Count > 0 && run2.Count > 0)
                    {
                        if (run1.Peek() <= run2.Peek())
                        {
                            writer.WriteLine(run1.Dequeue());
                        }
                        else
                        {
                            writer.WriteLine(run2.Dequeue());
                        }
                    }
                    while (run1.Count > 0)
                    {
                        writer.WriteLine(run1.Dequeue());
                    }
                    while (run2.Count > 0)
                    {
                        writer.WriteLine(run2.Dequeue());
                    }
                }
            }
        }

        static Queue<int> ReadNextRun(StreamReader reader, ref bool endOfFile, ref int? nextValue)
        {
            Queue<int> run = new Queue<int>();

            int currentValue;

            string? line;
            if (nextValue != null)
            {
                currentValue = nextValue.Value;
                nextValue = null;
            }
            else
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    endOfFile = true;
                    return run;
                }
                if (!int.TryParse(line.Trim(), out currentValue))
                {
                    // Обробка некоректних даних
                    return run;
                }
            }
            run.Enqueue(currentValue);

            int prevValue = currentValue;
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line.Trim(), out int value))
                {
                    // Обробка некоректних даних
                    continue;
                }
                if (value < prevValue)
                {
                    // Збереження значення для наступної серії
                    nextValue = value;
                    return run;
                }
                run.Enqueue(value);
                prevValue = value;
            }
            endOfFile = true;
            return run;
        }

    }
}

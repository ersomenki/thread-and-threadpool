using System;
using System.Collections.Generic;
using System.Threading;

class ThreadExamples
{
    // 1. Обработка массива с Thread
    static void ProcessArrayMulti(int[] array, int startIndex, int endIndex)
    {
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} обрабатывает элементы {startIndex} - {endIndex - 1}");

        for (int i = startIndex; i < endIndex; i++)
        {
            array[i] *= 2;
        }
    }

    public static void ThreadMultiArrProcess()
    {
        int[] numbers = { 1,2,3,4,5,6,7,8,9,10 };
        int middle = numbers.Length / 2;

        Thread t1 = new Thread(() => ProcessArrayMulti(numbers, 0, middle));
        Thread t2 = new Thread(() => ProcessArrayMulti(numbers, middle, numbers.Length));

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        Console.WriteLine("Результат умножения массива:");
        Console.WriteLine(string.Join(", ", numbers));
    }

    // 2. Суммирование с ThreadPool
    public static void ThreadPoolSumProcess()
    {
        int[] numbers = { 1,2,3,4,5,6,7,8,9,10 };
        int sum = 0;
        int completedTasks = 0;
        object locker = new object();
        ManualResetEvent doneEvent = new ManualResetEvent(false);

        foreach (int number in numbers)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}");

                lock (locker)
                {
                    sum += number;
                    completedTasks++;

                    if (completedTasks == numbers.Length)
                        doneEvent.Set();
                }
            });
        }

        doneEvent.WaitOne();
        Console.WriteLine($"Сумма элементов массива: {sum}");
    }

    // 3. Поиск максимума с Thread
    static void FindMax(int[] array, int start, int end, ref int globalMax, object locker)
    {
        int localMax = array[start];

        for (int i = start; i < end; i++)
        {
            if (array[i] > localMax)
                localMax = array[i];
        }

        lock (locker)
        {
            if (localMax > globalMax)
                globalMax = localMax;
        }

        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} нашёл локальный максимум: {localMax}");
    }

    public static void ThreadMaxArrProcess()
    {
        int[] numbers = { 3, 17, 5, 22, 9, 11, 8, 30, 15 };
        int middle = numbers.Length / 2;

        int max = int.MinValue;
        object locker = new object();

        Thread t1 = new Thread(() => FindMax(numbers, 0, middle, ref max, locker));
        Thread t2 = new Thread(() => FindMax(numbers, middle, numbers.Length, ref max, locker));

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        Console.WriteLine($"Максимальный элемент массива: {max}");
    }

    // 4. Фильтрация с ThreadPool
    public static void ThreadPoolFilterMoreThenFiveProcess()
    {
        int[] numbers = { 1, 8, 3, 10, 5, 6, 2, 9, 7, 4 };
        List<int> filteredNumbers = new List<int>();

        int completedTasks = 0;
        object locker = new object();
        ManualResetEvent doneEvent = new ManualResetEvent(false);

        foreach (int number in numbers)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}");

                if (number > 5)
                {
                    lock (filteredNumbers)
                    {
                        filteredNumbers.Add(number);
                    }
                }

                lock (locker)
                {
                    completedTasks++;
                    if (completedTasks == numbers.Length)
                        doneEvent.Set();
                }
            });
        }

        doneEvent.WaitOne();

        Console.WriteLine("Числа больше 5:");
        Console.WriteLine(string.Join(", ", filteredNumbers));
    }
}

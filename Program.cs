class Program
{
    static void Main()
    {
        ThreadExamples.ThreadMultiArrProcess();
        Console.WriteLine();

        ThreadExamples.ThreadPoolSumProcess();
        Console.WriteLine();

        ThreadExamples.ThreadMaxArrProcess();
        Console.WriteLine();

        ThreadExamples.ThreadPoolFilterMoreThenFiveProcess();
    }
}

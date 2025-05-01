using System.Net.Sockets;

namespace Task1
{
    class Program
    {
        static int goldMine = 100;
        static object locker = new object();
        static bool collapse = false;

        static void Main()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            List<Task> unitTasks = new List<Task>();

            for (int i = 0; i < 5; i++)
            {
                int unitId = i + 1;
                Task task = Task.Run(() => UnitWork(unitId, token));
                unitTasks.Add(task);
            }

            Thread.Sleep(5000);
            Console.WriteLine("\nMine collapse! Units come back to camp...\n");
            collapse = true;

            Thread.Sleep(5000);
            collapse = false;
            Console.WriteLine("\nMine is safe! Units come back to work...\n");

            foreach (Task task in unitTasks)
            {
                task.Wait();
            }

            Console.WriteLine("\nMining is finished!");
            cancellationTokenSource.Cancel();
            Console.ReadKey();
        }

        static void UnitWork(int id, CancellationToken token)
        {
            Random rnd = new Random();

            while (!token.IsCancellationRequested)
            {
                if (collapse)
                {
                    Console.WriteLine($"[Unit {id}] Collapse! Wait...");
                    Thread.Sleep(5000);
                    continue;
                }

                Thread.Sleep(rnd.Next(500, 1500));

                int mined = 0;

                lock (locker)
                {
                    if (goldMine <= 0)
                        return;

                    mined = rnd.Next(1, 5);
                    if (goldMine - mined < 0)
                        mined = goldMine;

                    goldMine -= mined;

                    Console.WriteLine($"[Unit {id}] Mined {mined} of gold. Last: {goldMine}");

                    if (goldMine <= 0)
                        return;
                }
            }
        }
    }
}


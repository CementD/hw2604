namespace Task2
{
    class Program
    {
        static int[] teams = new int[3];
        static object locker = new object();
        static bool ceasefire = false;
        

        static void Main()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < teams.Length; i++)
            {
                int teamId = i;
                teams[teamId] = new Random().Next(10, 20);
                tasks.Add(Task.Run(() => Fight(teamId, token)));
            }

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(7000);
                    ceasefire = true;
                    Console.WriteLine("\nCeasefire for 3 seconds...\n");
                    Thread.Sleep(3000);
                    ceasefire = false;
                    Console.WriteLine("\nFigth was restarted!\n");
                }
            });

            Thread.Sleep(35000);
            cancellationTokenSource.Cancel();

            Console.WriteLine("\nEnd of the game. Result:\n");

            foreach (Task t in tasks)
            {
                t.Wait();
            }

            for (int i = 0; i < teams.Length; i++)
            {
                Console.WriteLine($"Team {i + 1}: {teams[i]} fighters");
            }

            Console.ReadKey();
        }

        static void Fight(int teamId, CancellationToken token)
        {
            Random rnd = new Random();

            while (!token.IsCancellationRequested)
            {
                if (ceasefire)
                {
                    Thread.Sleep(500);
                    continue;
                }

                Thread.Sleep(rnd.Next(1000, 2000));

                lock (locker)
                {
                    int recruits = rnd.Next(1, 5);
                    teams[teamId] += recruits;

                    int targetId;
                    do
                    {
                        targetId = rnd.Next(0, teams.Length);
                    } while (targetId == teamId);

                    int damage = rnd.Next(1, 5);
                    teams[targetId] -= damage;
                    if (teams[targetId] < 0)
                        teams[targetId] = 0;

                    Console.WriteLine($"[Team {teamId + 1}] +{recruits} fighters | Atack -> Team {targetId + 1}, -{damage}. Status: [{string.Join(", ", teams)}]");
                }
            }
        }
    }
}

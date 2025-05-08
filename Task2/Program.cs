namespace Task2
{
    class Program
    {
        static int[] teams = new int[3];
        static object locker = new object();
        static bool ceasefire = false;

        static async void Main()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            List<Task> tasks = new List<Task>();

            Random rnd = new Random();
            for (int i = 0; i < teams.Length; i++)
            {
                int teamId = i;
                teams[teamId] = rnd.Next(10, 20);
                tasks.Add(FightAsync(teamId, token));
            }

            await Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(7000, token);
                    ceasefire = true;
                    Console.WriteLine("\nCeasefire for 3 seconds...\n");
                    await Task.Delay(3000, token);
                    ceasefire = false;
                    Console.WriteLine("\nFight was restarted!\n");
                }
            });

            await Task.Delay(35000);
            cancellationTokenSource.Cancel();

            Console.WriteLine("\nEnd of the game. Result:\n");

            await Task.WhenAll(tasks);

            for (int i = 0; i < teams.Length; i++)
            {
                Console.WriteLine($"Team {i + 1}: {teams[i]} fighters");
            }

            Console.ReadKey();
        }

        static async Task FightAsync(int teamId, CancellationToken token)
        {
            Random rnd = new Random();

            while (!token.IsCancellationRequested)
            {
                if (ceasefire)
                {
                    await Task.Delay(500, token);
                    continue;
                }

                await Task.Delay(rnd.Next(1000, 2000), token);

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

                    Console.WriteLine($"[Team {teamId + 1}] +{recruits} fighters | Attack -> Team {targetId + 1}, -{damage}. Status: [{string.Join(", ", teams)}]");
                }
            }
        }
    }
}

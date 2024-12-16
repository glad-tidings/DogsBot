using System.Text.Json;

namespace Dogs
{
    static class Program
    {
        private static ProxyType[]? proxies;
        static List<DogsQuery>? LoadQuery()
        {
            try
            {
                var contents = File.ReadAllText(@"data.txt");
                return JsonSerializer.Deserialize<List<DogsQuery>>(contents);
            }
            catch { }

            return null;
        }

        static ProxyType[]? LoadProxy()
        {
            try
            {
                var contents = File.ReadAllText(@"proxy.txt");
                return JsonSerializer.Deserialize<ProxyType[]>(contents);
            }
            catch { }

            return null;
        }

        static void Main()
        {
            Console.WriteLine("  ____                  ____   ___ _____ \r\n |  _ \\  ___   __ _ ___| __ ) / _ \\_   _|\r\n | | | |/ _ \\ / _` / __|  _ \\| | | || |  \r\n | |_| | (_) | (_| \\__ \\ |_) | |_| || |  \r\n |____/ \\___/ \\__, |___/____/ \\___/ |_|  \r\n              |___/                      ");
            Console.WriteLine();
            Console.WriteLine("Github: https://github.com/glad-tidings/DogsBot");
            Console.WriteLine();
            Console.Write("Select an option:\n1. Run bot\n2. Create session\n> ");
            string? opt = Console.ReadLine();

            var DogsQueries = LoadQuery();
            proxies = LoadProxy();

            if (opt != null)
            {
                if (opt == "1")
                {
                    foreach (var Query in DogsQueries ?? [])
                    {
                        var BotThread = new Thread(() => DogsThread(Query)); BotThread.Start();
                        Thread.Sleep(60000);
                    }
                }
                else
                {
                    foreach (var Query in DogsQueries ?? [])
                    {
                        if (!File.Exists(@$"sessions\{Query.Name}.session"))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})");
                            TelegramMiniApp.WebView vw = new(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "");
                            if (vw.Save_Session().Result)
                                Console.WriteLine("Session created");
                            else
                                Console.WriteLine("Create session failed");
                        }
                    }

                    Environment.Exit(0);
                }
            }

            Console.ReadLine();
        }

        public async static void DogsThread(DogsQuery Query)
        {
            while (true)
            {
                var RND = new Random();

                try
                {
                    var Bot = new DogsBot(Query, proxies ?? []);
                    if (!Bot.HasError)
                    {
                        Log.Show("Dogs", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White);
                        Log.Show("Dogs", Query.Name, $"login successfully.", ConsoleColor.Green);
                        var Sync = await Bot.DogsRewards();
                        if (Sync != null)
                        {
                            Log.Show("Dogs", Query.Name, $"synced successfully. B<{Sync.Total}>", ConsoleColor.Blue);

                            if (Query.DailyReward)
                            {
                                var calendar = await Bot.DogsCalendar();
                                foreach (var calen in calendar.Where(x => x.IsChecked == false & x.IsCurrent == true & x.IsAvailable == true))
                                {
                                    bool check = await Bot.DogsCalendarCheck(calen.ID);
                                    if (check)
                                        Log.Show("Dogs", Query.Name, $"daily reward claimed", ConsoleColor.Green);
                                    else
                                        Log.Show("Dogs", Query.Name, $"claim daily reward failed", ConsoleColor.Red);
                                }
                            }
                        }
                        else
                            Log.Show("Dogs", Query.Name, $"synced failed", ConsoleColor.Red);

                        Sync = await Bot.DogsRewards();
                        if (Sync != null)
                            Log.Show("Dogs", Query.Name, $"B<{Sync.Total}>", ConsoleColor.Blue);
                    }
                    else
                        Log.Show("Dogs", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red);
                }
                catch (Exception ex)
                {
                    Log.Show("Dogs", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red);
                }

                int syncRND = 0;
                if (DateTime.Now.Hour < 8)
                    syncRND = RND.Next(Query.NightSleep[0], Query.NightSleep[1]);
                else
                    syncRND = RND.Next(Query.DaySleep[0], Query.DaySleep[1]);
                Log.Show("Dogs", Query.Name, $"sync sleep '{Convert.ToInt32(syncRND / 3600d)}h {Convert.ToInt32(syncRND % 3600 / 60d)}m {syncRND % 60}s'", ConsoleColor.Yellow);
                Thread.Sleep(syncRND * 1000);
            }
        }
    }
}
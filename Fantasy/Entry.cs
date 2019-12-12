using System;
using DataTransferLibrary;
using System.Collections.Generic;
using DataTransferLibrary.Models.Player;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Fantasy
{
    public static class Entry
    {


        public static void CreateNewUser(string userName, string email)
        {
            var dataAccess = new DataAccess();
            var userExists = dataAccess.CreateNewUser(userName, email);

            if (userExists)
            {
                Console.WriteLine($"User Name {userName} already exists");
            }
            else
            {
                Console.WriteLine($"User {userName} has benn created");
            }

        }

        public static bool UserExists( string userName)
        {
            var dataAccess = new DataAccess();
            return dataAccess.DoesUserExist(userName);
        }

        public static void LoadDatabaseFromApi(int benchMarkSeason)
        {

            var cInitial = InitialLoadPlayerData(100);

            var p = new DataAccess();
            p.LoadInitialPlayerSeasonAverages(InitialLoadBenchmarkSeasonAverages(benchMarkSeason));
            p.LoadInitialPlayer(InitialLoadPlayerData(100));

        }

        public static List<Player> LoadPlayerData()
        {

            var p = new DataAccess();
            var indexes = p.GetPlayerIndexes();

            List<Player> playerList = new List<Player>();

            foreach (PlayerIndex m in indexes)
            {
                var c = new Player(m);

                playerList.Add(c);

            }

            List<Player> SortedPlayerList = playerList.OrderByDescending(o => o.Index).ToList();

          //  SortedPlayerList.ForEach(i => Console.WriteLine($"{i.Id} {i.First_Name} {i.Last_Name} {i.Index}"));

            return SortedPlayerList;
        }


        public static List<Team> LoadTeamData(List<Player> playerList)
          {
            var p = new DataAccess();
            var teamData = p.GetTeam();

            List<Team> teamList = new List<Team>();

            var distincUserTeams = teamData
                               .Select(m => new { m.UserName, m.Email, m.TeamName })
                               .Distinct()
                               .ToList();

            foreach (var user in distincUserTeams)
            {

                List<Player> playerMatches = new List<Player>();

                foreach (var kv in teamData)
                {
                    bool playerMatchesTeam = user.TeamName == kv.TeamName ? true : false;
                    if (playerMatchesTeam)
                    {
                        var player = playerList.FirstOrDefault(x => x.Id == kv.PlayerId);
                        if (player != null)
                        {
                            playerMatches.Add(player);
                        }
                    }
                }

                teamList.Add(new Team(new User(user.UserName, user.Email), user.TeamName, playerMatches));

            }

            return teamList;
        }


  
 
        public static void CalculateBestPlayerStats(List<Player> players, List<Team> teams, string startDate, string endDate)

        {
            List<int> plList = new List<int>();

            foreach (Player pl in players)
            {
                plList.Add(pl.Id);
            }

            var gameData = LoadGameData(2018, plList, startDate, endDate);

            List<PlayerStat> AllGamesByPlayer = new List<PlayerStat>();

            foreach (DataTransferLibrary.Models.Game.Datum d in gameData.SelectMany(g => g.data))
            {
                AllGamesByPlayer.Add(new PlayerStat(d.player.id, d.pts, d.blk, d.stl, d.ast, d.reb, d.turnover, d.fg_pct, d.fg3_pct, d.ft_pct));
            }

            var gamesByPlayer =
            from player in AllGamesByPlayer
            group player by player.player_id;

            //only the best game of the week
            var bestGameByPlayer = gamesByPlayer.SelectMany(a => a.Where(b => b.WeeklyIndex == a.Max(c => c.WeeklyIndex)));


            List<PlayerStat> BestGamesByPlayer = new List<PlayerStat>();

            foreach (PlayerStat pls in bestGameByPlayer)
            {
                BestGamesByPlayer.Add(new PlayerStat(pls.player_id, pls.pts, pls.blk, pls.stl, pls.ast, pls.reb, pls.turnover, pls.fg_pct, pls.fg3_pct, pls.ft_pct));
            }

            //assing calculated indexes to teams and calculate team totals
            foreach (Team team in teams)
            {
                team.CalculatePlayerIndexes(BestGamesByPlayer);
            }
        }



        public static List<DataTransferLibrary.Models.Game.RootObject> LoadGameData(int season, List<int> pList, string start_date, string end_date)
        {
            Console.WriteLine($"Loading game data from API... This might take a few seconds...");

            int? totalPages = 2;
            List<DataTransferLibrary.Models.Game.RootObject> PlayerData = new List<DataTransferLibrary.Models.Game.RootObject>();
   
            var chunkedLists = pList.ChunkBy(25);

            string gameUrl = $"https://www.balldontlie.io/api/v1/stats/?seasons[]={season}&start_date={start_date}&end_date={end_date}";

            StringBuilder tSB = new StringBuilder(gameUrl);

            foreach (List<int> cList in chunkedLists)
            {
                foreach (int pl in cList)
                {
                    var playerId = pl;
                    tSB.Insert(gameUrl.LastIndexOf("&start_date"), $"&player_ids[]={playerId}");
                }

                for (int i = 1; i < totalPages; i++)
                {
                    var p = new Request();

                    string webServiceUrl = tSB.ToString();
                    PlayerData.Add(JsonConvert.DeserializeObject<DataTransferLibrary.Models.Game.RootObject>(p.WebRequestContent(webServiceUrl)));

                    if (i == 1)
                    {
                        totalPages = PlayerData[0].meta.total_pages;
                    }

                }

                tSB = new StringBuilder(gameUrl);

            }
            Console.WriteLine($"Loading game data has finished...");

            return PlayerData;
        }


        public static List<DataTransferLibrary.Models.Player.RootObject> InitialLoadPlayerData(int per_page)
        {
            int totalPages = 2;
            List<DataTransferLibrary.Models.Player.RootObject> PlayerData = new List<DataTransferLibrary.Models.Player.RootObject>();

            for (int i = 1; i < totalPages; i++)
            {
                var p = new Request();
                string webServiceUrl = $"https://www.balldontlie.io/api/v1/players?per_page={per_page}&page={i}";

                PlayerData.Add(JsonConvert.DeserializeObject<DataTransferLibrary.Models.Player.RootObject>(p.WebRequestContent(webServiceUrl)));

                if (i == 1)
                {
                    totalPages = PlayerData[0].meta.total_pages;
                }

            }
            return PlayerData;
        }



        public static List<DataTransferLibrary.Models.PlayerSeasonAverages.RootObject> InitialLoadBenchmarkSeasonAverages(int benchmarkSeason)
        {
            List<DataTransferLibrary.Models.PlayerSeasonAverages.RootObject> PlayerData = new List<DataTransferLibrary.Models.PlayerSeasonAverages.RootObject>();

            var p = new Request();

            StringBuilder tSB = new StringBuilder($"https://www.balldontlie.io/api/v1/season_averages?season={benchmarkSeason}");

            for (int i = 1; i < 550; i++)
            {
                tSB.Append($"&player_ids[]={i}");
            }

            string webServiceUrl = tSB.ToString();
            PlayerData.Add(JsonConvert.DeserializeObject<DataTransferLibrary.Models.PlayerSeasonAverages.RootObject>(p.WebRequestContent(webServiceUrl)));

            foreach (DataTransferLibrary.Models.PlayerSeasonAverages.Datum pp in PlayerData.SelectMany(g => g.data))
            {
                pp.initialIndex = pp.pts + pp.blk * 2 + pp.stl * 2 + pp.ast * 2 + pp.reb - (pp.turnover * 2) + pp.pts * (1 + (pp.fg_pct / 100 + pp.fg3_pct / 100 + pp.ft_pct / 100));
            }
            return PlayerData;
        }


    }
}

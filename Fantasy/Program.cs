namespace Fantasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Program
    {
        internal static void Main(string[] args)
        {

            // If not loaded already, loads all data and calculates Initial player index from season averages:
            // Entry.LoadDatabaseFromApi(2017);


            void StartProgram()
            {

                    string dataDateTime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
                    string dataDatetimeMinusWeek = DateTime.Now.AddYears(-1).AddDays(-7).ToString("yyyy-MM-dd");

                    string dataYear = dataDateTime.Substring(0, 4);
                    string initialIndexYear = DateTime.Now.AddYears(-2).ToString("yyyy");

                    Console.WriteLine($"Info: Assemble a team of max 8 players with max initial index of 200. Your players can bring your team index points based on best games played throughout the week. Index will be calculated from {dataDatetimeMinusWeek} till {dataDateTime} and initial index points assigned from previous season averages. Them team with the greatest sum of index points wins.");

                    Console.WriteLine("Enter your username:");
                    string userName = Console.ReadLine();
                    if (Entry.UserExists(userName))
                    {
                        StartTeamFlow(userName, dataDatetimeMinusWeek, dataDateTime);
                    }
                    else
                    {
                        Console.WriteLine($"User {userName} does not exist");
                        Console.WriteLine($"Type CREATE for creating a new user. Type ENTERNEW for entering a new username.");
                        string input = Console.ReadLine();
                        if (input == "ENTERNEW")
                        {
                            StartProgram();
                        }
                        else if (input == "CREATE")
                        {
                            Console.WriteLine($"Create new user. Enter username:");
                            string newUserName = Console.ReadLine();
                            Console.WriteLine($"Create new user. Enter email adress:");
                            string newUserEmail = Console.ReadLine();
                            Entry.CreateNewUser(newUserName, newUserEmail);
                            StartProgram();
                        }
                        else
                        {
                            Console.WriteLine($"Wrong input. Expected ENTERNEW or CREATE");
                            StartProgram();
                        }
                    }

            }


            StartProgram();


            void StartTeamFlow(string userName, string dataDatetimeMinusWeek, string dataDateTime)
            {
                var playerList = Entry.LoadPlayerData();
                var teamPlayerList = Entry.LoadTeamData(playerList);
                var gameData = Entry.LoadStats(playerList, teamPlayerList, dataDatetimeMinusWeek, dataDateTime);

                bool userHasTeamsPlayers = false;

                List<Team> userPlayerTeams = new List<Team>();

                foreach (Team team in teamPlayerList)
                {
                    if (team.FantasyUser.UserName == userName)
                    {
                        userPlayerTeams.Add(team);
                        userHasTeamsPlayers = true;
                    }
                }


                if (!userHasTeamsPlayers)
                {
                    Console.WriteLine("User does not have any teams. Enter a Team Name you would like to create:");
                    string teamName = Console.ReadLine();
                    new Team(new User(userName, ""), teamName);
                    teamPlayerList = Entry.LoadTeamData(playerList);
                    userPlayerTeams.Clear();
                    foreach (Team team in teamPlayerList)
                    {
                        if (team.FantasyUser.UserName == userName)
                        {
                            userPlayerTeams.Add(team);
                            userHasTeamsPlayers = true;
                        }
                    }

                }

                ShowTeamData(userName, teamPlayerList, dataDateTime, dataDatetimeMinusWeek, gameData);

            }



            void ShowTeamData(string userName, List<Team> teams, string dateTime, string dateTimeMinusWeek, List<DataTransferLibrary.Models.Game.RootObject> gameData)
            {
                var playerList = Entry.LoadPlayerData();
                var teamList = Entry.LoadTeamData(playerList);

                Entry.CalculateBestStats(teams, gameData);

                bool userHasTeams = false;

                Console.WriteLine("Listing all teams:");
                foreach (Team t in teams.OrderByDescending(x => x.TeamTotalIndex))
                {
                    Console.WriteLine($"Team Name: {t.TeamName} TotalIndex: {t.TeamTotalIndex}");
                }

                Console.WriteLine("Listing user teams:");
                foreach (Team t in teams)
                {
                    if (t.FantasyUser.UserName == userName)
                    { 
                        Console.WriteLine($"Team Name: {t.TeamName} TotalIndex: {t.TeamTotalIndex}");

                        foreach (Player p in t.Players)
                        {
                            Console.WriteLine($" Id: {p.Id} Name: {p.First_Name} {p.Last_Name}  InitialIndex: {p.InitialIndex} Current Index: {p.Index}");
                        }
                    }
                }


                Console.WriteLine("If you would like to add or remove players from your team type ADD or REMOVE");
                string addOrRemove = Console.ReadLine();
                if (addOrRemove == "ADD" | addOrRemove == "REMOVE")
                {
                    EditTeams(userName,teams, playerList, addOrRemove, dateTime,dateTimeMinusWeek, gameData);
                }

            }


            void EditTeams(string userName, List<Team> teams, List<Player> players, string addOrRemove, string dateTime, string dateTimeMinusWeek, List<DataTransferLibrary.Models.Game.RootObject> gameData)
            {
                Console.WriteLine($"Enter Team name you would like to edit:");
                string teamName = Console.ReadLine();
                var selectedTeam = teams.Where(a => a.TeamName == teamName).ToList();

                string playerId;


                if (addOrRemove == "ADD")
                {
                    foreach (Player p in players)
                    {
                        Console.WriteLine($" Id: {p.Id} Name: {p.First_Name} {p.Last_Name}  InitialIndex: {p.InitialIndex}");
                    }
                    Console.WriteLine($"Enter Player Id, you would like to add:");
                    playerId = Console.ReadLine();
                    selectedTeam[0].AddPlayer(Convert.ToInt32(playerId), selectedTeam[0], players);
                    ShowTeamData(userName, teams, dateTime, dateTimeMinusWeek, gameData);
                }
                else if (addOrRemove == "REMOVE")
                {
                    Console.WriteLine($"Enter Player Id, you would like to remove:");
                    playerId = Console.ReadLine().ToString();
                    selectedTeam[0].RemovePlayer(Convert.ToInt32(playerId), selectedTeam[0]);
                    ShowTeamData(userName, teams, dateTime, dateTimeMinusWeek, gameData);
                }

            }
            Console.ReadLine();
        }
    }
}

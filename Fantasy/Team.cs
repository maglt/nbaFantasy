using System;
using System.Collections.Generic;
using System.Linq;
using DataTransferLibrary;


namespace Fantasy
{
    public class Team
    {
        public User FantasyUser { get; set; }
        public string TeamName { get; set; }
        public List<Player> Players { get; set; }
        public decimal TeamTotalIndex { get; set; }

        public Team(User fantasyUser, string teamName)
        {
            this.FantasyUser = fantasyUser;
            this.TeamName = teamName;
            Players = new List<Player>();
            var dataAccess = new DataAccess();
            var doesTeamAlreadyExist = dataAccess.CreateTeamDb(this.TeamName,fantasyUser.UserName);
            if (doesTeamAlreadyExist)
                {
                Console.WriteLine($"Team {teamName} already exists");
                }
            else
                {
                Console.WriteLine($"Team {teamName} has been creater for user {fantasyUser.UserName}");
                }
        }

        public Team(User fantasyUser, string teamName, List<Player> Players)
        {
            this.FantasyUser = fantasyUser;
            this.TeamName = teamName;
            this.Players = Players;
     
        }


        public void AddPlayer(int playerID, Team fTeam, List<Player> lPlayer, bool initialSelection = false)
        {
            decimal maxInSelSum = 200;
            decimal curInSelSum = 0;
            int playerCount = 8;
            bool addPlayer = true;

            var pb = lPlayer.Where(u => u.Id == playerID).ToList();

            if (fTeam.Players.Count < playerCount)
            {
                if (initialSelection)
                {
                    curInSelSum = fTeam.Players.Sum(item => item.InitialIndex);

                    if (curInSelSum + pb[0].InitialIndex > maxInSelSum)

                    {
                        addPlayer = false;
                        Console.WriteLine($"You don't have enough money for this player. you have {maxInSelSum - curInSelSum} $ left");
                    }

                }

                if (addPlayer)
                {
                    
                    var dataAccess = new DataAccess();
                    var playerExists = dataAccess.AddPlayerDb(this.TeamName, pb[0].Id);
                    if (playerExists)
                    {
                        Console.WriteLine($"Player {pb[0].First_Name} {pb[0].Last_Name} already exists on {this.TeamName}");
                    }
                    else
                    {
                        fTeam.AddPlayerToList(pb[0]);
                        Console.WriteLine($"Player {pb[0].First_Name} {pb[0].Last_Name} has been added to {this.TeamName}");
                        CalculateTeamTotal();
                    }

                }

            }

            else
            {
                Console.WriteLine($"You can only have 8 players on your team.");
            }
        }




        public void RemovePlayer(int playerID, Team fTeam)
        {
            var dataAccess = new DataAccess();
            var playerExists = dataAccess.RemovePlayerDb(this.TeamName, playerID);
            if (playerExists)
            {
                Console.WriteLine($"Player {playerID} does not exists on {this.TeamName}");
            }
            else
            {
                Players.Remove(Players.FirstOrDefault(o => o.Id == playerID));
                Console.WriteLine($"Player {playerID} has been removed from {this.TeamName}");
                CalculateTeamTotal();
            }

        }



        public void CalculateTeamTotal()
        {
             TeamTotalIndex = Players.Sum(item => item.Index);
        }

        public void CalculatePlayerIndexes(List<PlayerStat> BestGamesByPlayer)
        {
            foreach (var kv in BestGamesByPlayer)
            {
                var player = Players.FirstOrDefault(x => x.Id == kv.player_id);
                if (player != null & kv.WeeklyIndex != null)
                    player.Index = (decimal)kv.WeeklyIndex;
            }

            CalculateTeamTotal();
        }


        public void AddPlayerToList(Player p)
        {
            Players.Add(p);
        }


    }
}

using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;
using DataTransferLibrary.Models.Player;
using DataTransferLibrary.Models.TeamPlayer;
using System.Linq;
using DataTransferLibrary;




namespace DataTransferLibrary
{

   
    public class DataAccess
    {

        string connectionString = "Data Source=B8301455\\SQLEXPRESS;Initial Catalog=NbaFantasy;Integrated Security=SSPI";

        //veikia
        public IEnumerable<PlayerIndex> GetPlayerIndexes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var PlayerIndexes = connection.Query<PlayerIndex> ("SELECT p.id, p.First_Name,p.Last_Name,p.Team,pa.PlayerIndex as [Index] FROM dbo.PlayerSeasonAverages pa INNER JOIN dbo.Player p ON p.Id = pa.player_id");

                return PlayerIndexes;
            }
        }


        public bool DoesUserExist(string userName)
        {
            bool userExists = true;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var existingId = connection.Query<int?>("SELECT [Id] FROM [dbo].[User] WHERE userName = @userName", new { userName });

                if (existingId.IsCountZero())
                   userExists = false;
            }
            return userExists;
        }



        public List<TeamPlayer> GetTeam()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var teamData = connection.Query<TeamPlayer>("SELECT u.UserName, u.Email, t.Name [TeamName], p.Id [PlayerId] FROM [dbo].[User] u INNER JOIN [dbo].[UserTeam] ut ON u.Id = ut.UserId INNER JOIN [dbo].[Team] t ON ut.TeamId = t.Id LEFT JOIN [dbo].[TeamPlayer] tp ON tp.TeamID = t.Id LEFT JOIN [dbo].[Player] p ON p.Id = tp.PlayerId LEFT JOIN [dbo].[PlayerSeasonAverages] psa ON psa.player_id = p.Id");

                return teamData.ToList();
            }
        }




        public void LoadInitialPlayerSeasonAverages(List<DataTransferLibrary.Models.PlayerSeasonAverages.RootObject> PlayerSeasonAverages)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute("TRUNCATE TABLE [dbo].[PlayerSeasonAverages]");
                connection.Execute( @"INSERT INTO [dbo].[PlayerSeasonAverages]([player_id],[games_played] ,[season] ,[min] ,[fgm] ,[fga] ,[fg3m] ,[fg3a] ,[ftm] ,[fta] ,[oreb] ,[dreb] ,[reb] ,[ast] ,[stl],[blk]  ,[turnover] ,[pf] ,[pts] ,[fg_pct] ,[fg3_pct],[ft_pct] , [PlayerIndex]) VALUES (@player_id, @games_played , @season , @min , @fgm , @fga , @fg3m , @fg3a , @ftm , @fta , @oreb , @dreb , @reb , @ast  , @stl , @blk , @turnover , @pf , @pts , @fg_pct , @fg3_pct, @ft_pct, @initialIndex)", PlayerSeasonAverages[0].data);
            }
        }


        public bool CreateNewUser(string userName, string email)
        {
            bool userExists = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var existingId = connection.Query<int?>("SELECT [Id] FROM [dbo].[User] WHERE userName = @userName", new { userName });

                if (existingId.IsCountZero())
                    connection.Execute(@"INSERT INTO [dbo].[User] ([UserName],[Email]) VALUES (@userName, @email)", new { userName, email });
                else
                    userExists = true;
            }
            return userExists;
        }



        public bool AddPlayerDb(string teamName, int playerId)
        {
            bool playerExists = false;
           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var teamId = connection.Query<int?>("SELECT [Id] FROM [dbo].[Team] WHERE Name = @teamName", new { teamName });
                var existingId = connection.Query<int?>("SELECT [PlayerId] FROM [NbaFantasy].[dbo].[TeamPlayer] WHERE teamId = @teamId AND playerId = @playerId",new { teamId, playerId });
                
                if (existingId.IsCountZero())
           
                connection.Execute(@"INSERT INTO [dbo].[TeamPlayer] ([TeamId],[PlayerId]) VALUES (@teamId, @playerId)", new { teamId, playerId });     
                else
                playerExists = true;

            }

            return playerExists;
        }



        public bool RemovePlayerDb(string teamName, int playerId)
        {
            bool playerDoesNotExist = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var teamId = connection.Query<int?>("SELECT [Id] FROM [dbo].[Team] WHERE Name = @teamName", new { teamName });
                var existingId = connection.Query<int?>("SELECT [PlayerId] FROM [NbaFantasy].[dbo].[TeamPlayer] WHERE teamId = @teamId AND playerId = @playerId", new { teamId, playerId });

                if (existingId.IsCountZero())
                    playerDoesNotExist = true;
                else
                    connection.Execute(@"DELETE FROM [dbo].[TeamPlayer] WHERE teamId = @teamId and playerId =  @playerId", new { teamId, playerId });
            }
            return playerDoesNotExist;

        }



        public bool CreateTeamDb(string teamName, string userName)
        {
            bool teamAlreadyExist = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var existingId = connection.Query<int?>("SELECT [Id] FROM [dbo].[Team] WHERE Name = @teamName", new {teamName});

                //  connection.Execute(@"INSERT INTO [dbo].[Team] ([Name]) VALUES (@teamName)", new {teamName});
                if (existingId.IsCountZero())
                {
                    connection.Execute(@"INSERT INTO [dbo].[Team] ([Name]) VALUES (@teamName)", new { teamName });
                    var userId = connection.Query<int?>("SELECT [Id] FROM [dbo].[User] WHERE UserName = @userName", new { userName });
                    var teamId = connection.Query<int?>("SELECT [Id] FROM [dbo].[Team] WHERE Name =  @teamName", new { teamName });
                    connection.Execute(@"INSERT INTO [dbo].[UserTeam] ([UserId],[TeamId]) VALUES (@userid,@teamId)", new { userId, teamId });
                }
                else
                    teamAlreadyExist = true;
            }
            return teamAlreadyExist;
        }

        public void LoadInitialPlayer(List<DataTransferLibrary.Models.Player.RootObject> Player)
        {


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute("TRUNCATE TABLE [dbo].[Player]");
                for (int i = 0; i < Player.Count; i++)
                {

                    for (int p = 0; p < Player[i].data.Count; p++)

                        connection.Execute(@"INSERT INTO [dbo].[Player]([Id],[First_Name],[Height_Feet],[Height_Inches],[Last_Name],[Position],[Team],[Weight_pounds]) VALUES (@id, @first_name , @height_feet , @height_inches , @last_name , @position, @team, @weight_pounds)", new
                        {
                            id = Player[i].data[p].id,
                            first_name = Player[i].data[p].first_name,
                            height_feet = Player[i].data[p].height_feet,
                            height_inches = Player[i].data[p].height_inches,
                            last_name = Player[i].data[p].last_name,
                            position = Player[i].data[p].position,
                            team = Player[i].data[p].team.full_name,
                            weight_pounds = Player[i].data[p].weight_pounds
                        });

                }
            }



        }

        }

    }







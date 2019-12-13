using DataTransferLibrary.Models.Player;
using System;

namespace Fantasy
{
    public class Player : PlayerIndex
    {
        public decimal InitialIndex { get; set; }
        public Player (PlayerIndex i)
        {
            this.Id = i.Id;
            this.First_Name = i.First_Name;
            this.Last_Name = i.Last_Name;
            this.Index = 0;
            this.Team = i.Team;
            this.InitialIndex = i.Index; 
        }



    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferLibrary.Models.Game
{
    public class Game
    {
        public int? id { get; set; }
        public DateTime? date { get; set; }
        public int? home_team_id { get; set; }
        public int? home_team_score { get; set; }
        public int? season { get; set; }
        public int? visitor_team_id { get; set; }
        public int? visitor_team_score { get; set; }
    }
}

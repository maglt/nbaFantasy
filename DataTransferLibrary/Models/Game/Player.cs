using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferLibrary.Models.Game
{
    public class Player
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string position { get; set; }
        public int? team_id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferLibrary.Models.Player
{
    public class Meta
    {
        public int total_pages { get; set; }
        public int current_page { get; set; }
        public int next_page { get; set; }
        public int per_page { get; set; }
        public int total_count { get; set; }
    }

}

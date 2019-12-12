using System;
using System.Collections.Generic;
using System.Text;
using DataTransferLibrary;

namespace DataTransferLibrary.Models.Player
{
    public class Datum
    {
            public int id { get; set; }
            public string first_name { get; set; }
            public string height_feet { get; set; }
            public string height_inches { get; set; }
            public string last_name { get; set; }
            public string position { get; set; }
            public Team team { get; set; }
            public string weight_pounds { get; set; }
    }
}

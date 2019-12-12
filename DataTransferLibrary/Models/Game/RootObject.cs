using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace DataTransferLibrary.Models.Game
{
    public class RootObject
    {
        public List<Datum> data { get; set; }
        public Meta meta { get; set; }

    }
}

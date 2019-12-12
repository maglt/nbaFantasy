using System.Collections.Generic;
using Newtonsoft.Json;


namespace DataTransferLibrary.Models.Player
{
    public class RootObject
    {
            public List<Datum> data { get; set; }
            public Meta meta { get; set; }

    }
}

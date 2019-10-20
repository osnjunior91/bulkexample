using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkExample
{
    public class Result
    {
        [JsonProperty(PropertyName = "data")]
        public List<Partida> Partidas { get; set; }
        [JsonProperty(PropertyName = "meta")]
        public MetaDados Dados { get; set; }
    }

    public class MetaDados
    {
        [JsonProperty(PropertyName = "total_pages")]
        public int TotalPages { get; set; }
        [JsonProperty(PropertyName = "current_page")]
        public int CurrentPage { get; set; }
    }
}

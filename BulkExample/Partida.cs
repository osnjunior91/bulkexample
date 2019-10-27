using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkExample
{
    [Table("PARTIDA")]
    public class Partida : ICloneable
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int ID { get; set; }
        [Column("DATA_PARTIDA")] 
        [JsonProperty(PropertyName = "date")]
        public DateTime DataPartida { get; set; }
        [Column("PONTOS_CASA")]
        [JsonProperty(PropertyName = "home_team_score")]
        public int PontosCasa { get; set; }
        [Column("PONTOS_FORA")]
        [JsonProperty(PropertyName = "visitor_team_score")]
        public int PontosFora { get; set; }
        [Column("TEMPORADA")]
        [JsonProperty(PropertyName = "season")]
        public string Temporada { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

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

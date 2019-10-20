using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkExample
{
    [Table("PARTIDA")]
    public class Partida
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
    }
}

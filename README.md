# Improving Entity Framework Performance with Bulk Operations

A practical comparison of standard Entity Framework Core operations and **EFCore.BulkExtensions** for inserting and deleting large datasets in SQL Server.

> **Historical example (2019):** This project accompanies an article originally published on November 4, 2019. Its code, dependencies, and benchmark results reflect the versions and environment used at that time. Performance may differ significantly with modern EF Core versions, batching behavior, database configuration, and hardware.

## Overview

Modern applications frequently need to process large volumes of data efficiently. Inserting or deleting thousands of records through an ORM can become expensive, particularly when each entity must be tracked and processed individually.

SQL Server supports bulk data operations for efficiently loading large datasets from sources such as CSV and text files. The [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) library brings a similar approach to EF Core, providing APIs such as `BulkInsert` and `BulkDelete`.

This sample uses NBA game data retrieved from an API to compare the execution time of regular EF Core operations against bulk operations.

## Technologies

- C# / .NET Core console application
- Entity Framework Core
- SQL Server
- [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions)
- Newtonsoft.Json
- [NBA API](https://rapidapi.com/theapiguy/api/free-nba) (the API referenced in the original article; availability may have changed)

## Database setup

Create the table used to store NBA game data:

```sql
CREATE TABLE [dbo].[PARTIDA] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [DATA_PARTIDA] DATETIME NOT NULL,
    [PONTOS_CASA] INT NOT NULL,
    [PONTOS_FORA] INT NOT NULL,
    [TEMPORADA] VARCHAR(15) NOT NULL,
    CONSTRAINT [PK_PARTIDA] PRIMARY KEY CLUSTERED ([ID] ASC)
);
```

## Data model

The `Partida` entity maps NBA API fields to the SQL Server table. `Result` and `MetaDados` represent the API response and pagination metadata.

```csharp
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

        public object Clone() => MemberwiseClone();
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
```

## Standard Entity Framework operations

The following methods add or remove entities using the regular EF Core change tracker and `SaveChanges()`.

```csharp
public double SalvaPartidasEntity(List<Partida> partidas)
{
    var sw = Stopwatch.StartNew();
    Partidas.AddRange(partidas);
    SaveChanges();
    sw.Stop();
    return sw.Elapsed.TotalSeconds;
}

public double RemoverTodosEntity(List<Partida> partidas)
{
    var sw = Stopwatch.StartNew();
    Partidas.RemoveRange(partidas);
    SaveChanges();
    sw.Stop();
    return sw.Elapsed.TotalSeconds;
}
```

## Bulk operations

The bulk versions use `BulkInsert` and `BulkDelete` to perform set-oriented operations more efficiently for large datasets.

```csharp
public double SalvaPartidasBulk(List<Partida> partidasBulk)
{
    var sw = Stopwatch.StartNew();
    this.BulkInsert(partidasBulk);
    sw.Stop();
    return sw.Elapsed.TotalSeconds;
}

public double RemoverTodosBulk(List<Partida> partidasBulk)
{
    var sw = Stopwatch.StartNew();
    this.BulkDelete(partidasBulk);
    sw.Stop();
    return sw.Elapsed.TotalSeconds;
}
```

Each method returns the elapsed execution time in seconds. The original project contains the console application that runs these comparisons.

## Benchmark results

The original experiment compared insertion and deletion operations across different dataset sizes, including 1,000 records. In that environment, bulk operations were substantially faster than the standard EF Core methods.

### Insert performance

![Original insertion benchmark chart](https://media.licdn.com/dms/image/v2/C4D12AQHt3iVG2KyTyw/article-inline_image-shrink_400_744/article-inline_image-shrink_400_744/0/1572314906839?e=1793232000&v=beta&t=I2SOPNLR9n9RW6aR2FmmOcaynpw-IcYFBnOo2c6dpE0)

### Delete performance

![Original deletion benchmark chart](https://media.licdn.com/dms/image/v2/C4D12AQFOhQOl9peLLA/article-inline_image-shrink_400_744/article-inline_image-shrink_400_744/0/1572315319255?e=1793232000&v=beta&t=BCAbr3l-HQoXh4apLKvhj57WuUZXz5YGDgqE4SIl_iE)

> **Note:** The charts above are linked to LinkedIn-hosted images and may expire. For a durable GitHub README, commit the original chart images into an `images/` folder and replace these links with relative paths.

## What to consider in production

- **Benchmark fairly:** Use the same database state, row counts, hardware, and comparable transaction settings for each operation.
- **Account for EF Core improvements:** Modern EF Core versions support improved batching and set-based deletion with `ExecuteDelete`; this historical comparison should not be interpreted as a current benchmark.
- **Check behavior differences:** Bulk APIs may bypass EF Core change tracking and may handle generated keys, relationships, concurrency, and transactions differently.
- **Measure at your scale:** Bulk operations can add overhead for small datasets. Profile your actual workload before choosing an approach.
- **Review dependencies:** Verify library compatibility and licensing requirements for your target EF Core version.

## Conclusion

In the original 2019 experiment, **EFCore.BulkExtensions** provided a simple way to improve insertion and deletion performance for large datasets. Even the 1,000-record test showed a noticeable benefit in that environment.

Bulk operations remain worth evaluating for data-heavy workflows, but their benefits should be validated against current EF Core capabilities and real application requirements.

## Source and author

**Oswaldo da Silva Nicacio Junior**  
[LinkedIn profile](https://www.linkedin.com/in/oswaldodasilvanicaciojunior/) · [Original article (Portuguese, November 4, 2019)](https://www.linkedin.com/pulse/entity-framework-melhorando-o-desempenho-com-ef-nicacio-junior/) · [Source repository](https://github.com/osnjunior91/bulkexample)

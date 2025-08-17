using System;
using Cassandra.Mapping.Attributes; // For mapping attributes

namespace core8_nuxt_cassandra.Entities
{

[Table("users", Keyspace = "core8")] // Maps to a Cassandra table
public class Product {

        [Column("id")]
        public Guid Id { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("descriptions")]
        public string Descriptions { get; set; }

        [Column("qty")]
        public int? Qty { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

        [Column("costprice")]
        public decimal CostPrice { get; set; }

        [Column("sellprice")]
        public decimal SellPrice { get; set; }

        [Column("saleprice")]
        public decimal SalePrice { get; set; }

        [Column("productpicture")]
        public string ProductPicture { get; set; }

        [Column("alertstocks")]
        public int? AlertStocks { get; set; }

        [Column("criticalstocks")]
        public int? CriticalStocks { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }    
}
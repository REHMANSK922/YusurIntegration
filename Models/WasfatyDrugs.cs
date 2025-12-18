using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YusurIntegration.Models
{
    public class WasfatyDrugs
    {
        [Key]
        [Column("drugId")]
        public int DrugId { get; set; }

        [Column("version")]
        public int? Version { get; set; }

        [Column("status")]
        [StringLength(12)]
        public string? Status { get; set; }

        [Column("isInfiniteDivisible")]
        public bool? IsInfiniteDivisible { get; set; }

        [Column("strength")]
        [StringLength(150)]
        public string? Strength { get; set; }

        [Column("genericName")]
        [StringLength(500)]
        public string? GenericName { get; set; }

        [Column("granularUnit")]
        public int? GranularUnit { get; set; }

        [Column("updatedDate")]
        [StringLength(12)]
        public string? UpdatedDate { get; set; }

        [Column("source")]
        [StringLength(500)]
        public string? Source { get; set; }

        [Column("packageType")]
        [StringLength(500)]
        public string? PackageType { get; set; }

        [Column("division")]
        [StringLength(500)]
        public string? Division { get; set; }

        [Column("regOwner")]
        [StringLength(500)]
        public string? RegOwner { get; set; }

        [Column("discontinueDate")]
        [StringLength(12)]
        public string? DiscontinueDate { get; set; }

        [Column("doseStrengthUnitId")]
        public int? DoseStrengthUnitId { get; set; }

        [Column("isHighAlert")]
        public bool? IsHighAlert { get; set; }

        [Column("price")]
        public double? Price { get; set; }

        [Column("suggestedRouteOfAdmin")]
        [StringLength(500)]
        public string? SuggestedRouteOfAdmin { get; set; }

        [Column("isAlternative")]
        public bool? IsAlternative { get; set; }

        [Column("barcode")]
        [StringLength(50)]
        public string? Barcode { get; set; }

        [Column("category2")]
        [StringLength(500)]
        public string? Category2 { get; set; }

        [Column("mappingScientificCode")]
        [StringLength(500)]
        public string? MappingScientificCode { get; set; }

        [Column("routeOfAdmin")]
        [StringLength(500)]
        public string? RouteOfAdmin { get; set; }

        [Column("dosageOrderForm")]
        [StringLength(500)]
        public string? DosageOrderForm { get; set; }

        [Column("category1")]
        [StringLength(500)]
        public string? Category1 { get; set; }

        [Column("dosageForm")]
        [StringLength(500)]
        public string? DosageForm { get; set; }

        [Column("dosageFormCode")]
        [StringLength(500)]
        public string? DosageFormCode { get; set; }

        [Column("verbSecondLanguageDescription")]
        [StringLength(500)]
        public string? VerbSecondLanguageDescription { get; set; }

        [Column("volume")]
        public double? Volume { get; set; }

        [Column("isControlled")]
        [StringLength(500)]
        public string? IsControlled { get; set; }

        [Column("packageVolume")]
        public double? PackageVolume { get; set; }

        [Column("volumeUnitId")]
        public int? VolumeUnitId { get; set; }

        [Column("divisibleFactor")]
        public double? DivisibleFactor { get; set; }

        [Column("roaCode")]
        [StringLength(500)]
        public string? RoaCode { get; set; }

        [Column("doseStrengthUnit")]
        [StringLength(500)]
        public string? DoseStrengthUnit { get; set; }

        [Column("maxRefill")]
        public int? MaxRefill { get; set; }

        [Column("region")]
        [StringLength(500)]
        public string? Region { get; set; }

        [Column("verbDescription")]
        [StringLength(500)]
        public string? VerbDescription { get; set; }

        [Column("unitTypeId")]
        public int? UnitTypeId { get; set; }

        [Column("drugCode")]
        [StringLength(500)]
        public string? DrugCode { get; set; }

        [Column("volumeUnit")]
        [StringLength(500)]
        public string? VolumeUnit { get; set; }

        [Column("packageSize")]
        [StringLength(500)]
        public string? PackageSize { get; set; }

        [Column("unitType")]
        [StringLength(500)]
        public string? UnitType { get; set; }

        [Column("tradeName")]
        [StringLength(500)]
        public string? TradeName { get; set; }

        [Column("doseStrength")]
        public double? DoseStrength { get; set; }

        [Column("ingredients")]
        [StringLength(500)]
        public string? Ingredients { get; set; }

        [Column("company")]
        [StringLength(500)]
        public string? Company { get; set; }

        [Column("atcCode")]
        [StringLength(500)]
        public string? AtcCode { get; set; }

        [Column("isHazardous")]
        public bool? IsHazardous { get; set; }

        [Column("dosageUnit")]
        [StringLength(500)]
        public string? DosageUnit { get; set; }

        [Column("dosageUnitId")]
        public int? DosageUnitId { get; set; }

        [Column("isDivisible")]
        public bool? IsDivisible { get; set; }

        [Column("genericCode")]
        [StringLength(500)]
        public string? GenericCode { get; set; }

        [Column("isLasa")]
        public bool? IsLasa { get; set; }

        [Column("isDelivery")]
        public bool? IsDelivery { get; set; }

        [Column("isDivisibleFactor")]
        public bool? IsDivisibleFactor { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("publishDate")]
        public DateTime? PublishDate { get; set; }

        [Column("id")]
        [StringLength(500)]
        public string? Id { get; set; }

        [Column("listId")]
        [StringLength(500)]
        public string? ListId { get; set; }

        [Column("itemId")]
        public int? ItemId { get; set; }
    }
}

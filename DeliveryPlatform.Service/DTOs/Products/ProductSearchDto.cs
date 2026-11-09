using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Service.DTOs.Products
{
    public class ProductSearchDto : IValidatableObject
    {
        public string? Search { get; set; }

        [Range(1, int.MaxValue)]
        public int? StoreId { get; set; }

         [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxPrice { get; set; }

        public bool? InStock { get; set; }

        public string? SortBy { get; set; } = "name";

        public bool SortDescending { get; set; }
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1,100)]
        public int PageSize { get; set; } = 10;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (MinPrice.HasValue &&
                MaxPrice.HasValue &&
                MinPrice > MaxPrice)
            {
                yield return new ValidationResult(
                    "MinPrice cannot be greater than MaxPrice.",
                    new[] { nameof(MinPrice), nameof(MaxPrice) });
            }

            if (!string.IsNullOrWhiteSpace(SortBy) &&
                SortBy.ToLower() != "name" &&
                SortBy.ToLower() != "price")
            {
                yield return new ValidationResult(
                    "SortBy must be name or price.",
                    new[] { nameof(SortBy) });
            }
        }



    }
}
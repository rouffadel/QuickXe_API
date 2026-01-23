using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DAL.DAO;
public class Country
{
    public int CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }
    public string? CurrencyName { get; set; }
    public string? CurrencyCode { get; set; }
    public double? BuyRate { get; set; }
    public double? SellRate { get; set; }
    public bool? CurrencyAvailable { get; set; }
    public string? CurrencySymbol { get; set; }

    [ForeignKey("ApplicationUser")]
    public string? TenantId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

}

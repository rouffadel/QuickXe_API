namespace DAL.DAO;
public class Country
{
    public int CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? CountryCode { get; set; }
    public string? CurrencyName { get; set; }
    public double? BuyRate { get; set; }
    public double? SellRate { get; set; }
    public string? TenantId { get; set; }
}

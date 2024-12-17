namespace ProductStorage.Models;
public class Product
{
    public int Id { get; set; }
    public string BarCode { get; set; }
    public string Name { get; set; }
    public int? ReminderAmount { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
}

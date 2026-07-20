namespace CarMarket.Api.Models;

public class CarImage
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Orden { get; set; }

    public int CarId { get; set; }
    public Car? Car { get; set; }
}

using CarMarket.Api.Models.Enums;

namespace CarMarket.Api.Models;

public class Car
{
    public int Id { get; set; }

    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;

    public int Anio { get; set; }
    public int Kilometraje { get; set; }
    public decimal Precio { get; set; }

    public EstadoVehiculo Estado { get; set; } = EstadoVehiculo.Disponible;

    public string Color { get; set; } = string.Empty;
    public string Motor { get; set; } = string.Empty;
    public string Combustible { get; set; } = string.Empty;
    public string Transmision { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }

    public List<CarImage> Imagenes { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;
using CarMarket.Api.Models.Enums;

namespace CarMarket.Api.Dtos;

public class CarListItemDto
{
    public int Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
    public decimal Precio { get; set; }
    public EstadoVehiculo Estado { get; set; }
    public string? ImagenPrincipal { get; set; }
    public int CantidadImagenes { get; set; }
}

public class CarDetailDto : CarListItemDto
{
    public string Color { get; set; } = string.Empty;
    public string Motor { get; set; } = string.Empty;
    public string Combustible { get; set; } = string.Empty;
    public string Transmision { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Imagenes { get; set; } = new();
}

public class CarUpsertDto
{
    [Required, MaxLength(80)] public string Marca { get; set; } = string.Empty;
    [Required, MaxLength(80)] public string Modelo { get; set; } = string.Empty;
    [Required, MaxLength(120)] public string Version { get; set; } = string.Empty;

    [Range(1950, 2100)] public int Anio { get; set; }
    [Range(0, 2000000)] public int Kilometraje { get; set; }
    [Range(0, 999999999)] public decimal Precio { get; set; }

    public EstadoVehiculo Estado { get; set; } = EstadoVehiculo.Disponible;

    [MaxLength(60)] public string Color { get; set; } = string.Empty;
    [MaxLength(120)] public string Motor { get; set; } = string.Empty;
    [MaxLength(60)] public string Combustible { get; set; } = string.Empty;
    [MaxLength(60)] public string Transmision { get; set; } = string.Empty;
    [MaxLength(4000)] public string Descripcion { get; set; } = string.Empty;

    /// <summary>Hasta 15 URLs de imágenes, en el orden en que deben mostrarse.</summary>
    [MaxLength(15)]
    public List<string> Imagenes { get; set; } = new();
}

/// <summary>Parámetros de listado público: búsqueda, filtro y orden.</summary>
public class CarQueryParams
{
    public string? Q { get; set; }
    public EstadoVehiculo? Estado { get; set; }
    public string? OrderBy { get; set; } // precio_asc | precio_desc | anio_asc | anio_desc | km_asc | km_desc | recientes
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class LoginDto
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraUtc { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

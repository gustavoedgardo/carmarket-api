using CarMarket.Api.Data;
using CarMarket.Api.Dtos;
using CarMarket.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarMarket.Api.Controllers;

[ApiController]
[Route("api/cars")]
public class CarsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CarsController(AppDbContext db) => _db = db;

    // ---------- PÚBLICO ----------

    /// <summary>Lista pública de autos, con búsqueda, filtro por estado y orden.</summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<CarListItemDto>> GetAll([FromQuery] CarQueryParams p)
    {
        var query = _db.Cars.Include(c => c.Imagenes).AsQueryable();

        if (!string.IsNullOrWhiteSpace(p.Q))
        {
            var q = p.Q.Trim().ToLower();
            query = query.Where(c =>
                c.Marca.ToLower().Contains(q) ||
                c.Modelo.ToLower().Contains(q) ||
                c.Version.ToLower().Contains(q));
        }

        if (p.Estado.HasValue)
            query = query.Where(c => c.Estado == p.Estado.Value);

        query = p.OrderBy switch
        {
            "precio_asc" => query.OrderBy(c => c.Precio),
            "precio_desc" => query.OrderByDescending(c => c.Precio),
            "anio_asc" => query.OrderBy(c => c.Anio),
            "anio_desc" => query.OrderByDescending(c => c.Anio),
            "km_asc" => query.OrderBy(c => c.Kilometraje),
            "km_desc" => query.OrderByDescending(c => c.Kilometraje),
            _ => query.OrderByDescending(c => c.FechaCreacion),
        };

        var total = query.Count();
        var items = query
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(c => new CarListItemDto
            {
                Id = c.Id,
                Marca = c.Marca,
                Modelo = c.Modelo,
                Version = c.Version,
                Anio = c.Anio,
                Kilometraje = c.Kilometraje,
                Precio = c.Precio,
                Estado = c.Estado,
                ImagenPrincipal = c.Imagenes.OrderBy(i => i.Orden).Select(i => i.Url).FirstOrDefault(),
                CantidadImagenes = c.Imagenes.Count
            })
            .ToList();

        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(items);
    }

    /// <summary>Detalle público de un auto, con hasta 15 imágenes.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public ActionResult<CarDetailDto> GetById(int id)
    {
        var c = _db.Cars.Include(x => x.Imagenes).FirstOrDefault(x => x.Id == id);
        if (c is null) return NotFound();

        var dto = new CarDetailDto
        {
            Id = c.Id,
            Marca = c.Marca,
            Modelo = c.Modelo,
            Version = c.Version,
            Anio = c.Anio,
            Kilometraje = c.Kilometraje,
            Precio = c.Precio,
            Estado = c.Estado,
            Color = c.Color,
            Motor = c.Motor,
            Combustible = c.Combustible,
            Transmision = c.Transmision,
            Descripcion = c.Descripcion,
            Imagenes = c.Imagenes.OrderBy(i => i.Orden).Select(i => i.Url).Take(15).ToList(),
            CantidadImagenes = c.Imagenes.Count
        };
        dto.ImagenPrincipal = dto.Imagenes.FirstOrDefault();

        return Ok(dto);
    }

    // ---------- ADMIN (requiere JWT con rol Admin) ----------

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<CarDetailDto> Create(CarUpsertDto dto)
    {
        var car = MapToEntity(new Car(), dto);
        _db.Cars.Add(car);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = car.Id }, car.Id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, CarUpsertDto dto)
    {
        var car = _db.Cars.Include(c => c.Imagenes).FirstOrDefault(c => c.Id == id);
        if (car is null) return NotFound();

        MapToEntity(car, dto);
        car.FechaActualizacion = DateTime.UtcNow;

        // Reemplaza imágenes con la lista nueva (orden = posición en la lista)
        _db.CarImages.RemoveRange(car.Imagenes);
        car.Imagenes = dto.Imagenes.Take(15)
            .Select((url, idx) => new CarImage { Url = url, Orden = idx })
            .ToList();

        _db.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var car = _db.Cars.Find(id);
        if (car is null) return NotFound();

        _db.Cars.Remove(car);
        _db.SaveChanges();
        return NoContent();
    }

    private static Car MapToEntity(Car car, CarUpsertDto dto)
    {
        car.Marca = dto.Marca;
        car.Modelo = dto.Modelo;
        car.Version = dto.Version;
        car.Anio = dto.Anio;
        car.Kilometraje = dto.Kilometraje;
        car.Precio = dto.Precio;
        car.Estado = dto.Estado;
        car.Color = dto.Color;
        car.Motor = dto.Motor;
        car.Combustible = dto.Combustible;
        car.Transmision = dto.Transmision;
        car.Descripcion = dto.Descripcion;

        if (car.Id == 0 && dto.Imagenes.Any())
        {
            car.Imagenes = dto.Imagenes.Take(15)
                .Select((url, idx) => new CarImage { Url = url, Orden = idx })
                .ToList();
        }

        return car;
    }
}

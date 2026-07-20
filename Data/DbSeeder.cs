using CarMarket.Api.Models;
using CarMarket.Api.Models.Enums;

namespace CarMarket.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.AdminUsers.Any())
        {
            db.AdminUsers.Add(new AdminUser
            {
                Email = "admin@concesionaria.com",
                Nombre = "Administrador",
                // Contraseña de arranque: admin123 (¡cambiarla en producción!)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
            });
        }

        if (!db.Cars.Any())
        {
            db.Cars.AddRange(
                new Car
                {
                    Marca = "Toyota", Modelo = "Corolla", Version = "XEI 2.0 CVT",
                    Anio = 2022, Kilometraje = 32000, Precio = 24500000, Estado = EstadoVehiculo.Disponible,
                    Color = "Gris Plata", Motor = "2.0L 4 cilindros", Combustible = "Nafta", Transmision = "Automática CVT",
                    Descripcion = "Corolla XEI en excelente estado, único dueño, service oficial al día.",
                    Imagenes = new List<CarImage> {
                        new() { Url = "https://picsum.photos/seed/car-1-0/900/600", Orden = 0 },
                        new() { Url = "https://picsum.photos/seed/car-1-1/900/600", Orden = 1 },
                        new() { Url = "https://picsum.photos/seed/car-1-2/900/600", Orden = 2 },
                    }
                },
                new Car
                {
                    Marca = "Volkswagen", Modelo = "Amarok", Version = "V6 Extreme 4x4",
                    Anio = 2021, Kilometraje = 58000, Precio = 42900000, Estado = EstadoVehiculo.Reservado,
                    Color = "Negro Profundo", Motor = "3.0L V6 TDI", Combustible = "Diésel", Transmision = "Automática 8 vel.",
                    Descripcion = "Amarok tope de gama, cuero, techo eléctrico, barra antivuelco.",
                    Imagenes = new List<CarImage> {
                        new() { Url = "https://picsum.photos/seed/car-2-0/900/600", Orden = 0 },
                        new() { Url = "https://picsum.photos/seed/car-2-1/900/600", Orden = 1 },
                    }
                }
            );
        }

        db.SaveChanges();
    }
}

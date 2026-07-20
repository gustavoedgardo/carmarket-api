using CarMarket.Api.Data;
using CarMarket.Api.Dtos;
using CarMarket.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarMarket.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    /// <summary>Login de administrador. Devuelve un JWT válido por 8 horas.</summary>
    [HttpPost("login")]
    public ActionResult<LoginResponseDto> Login(LoginDto dto)
    {
        var user = _db.AdminUsers.FirstOrDefault(u => u.Email == dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Email o contraseña incorrectos." });

        var (token, expira) = _jwt.GenerarToken(user);

        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiraUtc = expira,
            Nombre = user.Nombre,
            Email = user.Email
        });
    }
}

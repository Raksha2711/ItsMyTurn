using Dapper;
using Microsoft.AspNetCore.Mvc;
using srv.slots.application.DTOs;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DapperContext _context;

    public UsersController(DapperContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var query = @"
            INSERT INTO users
            (user_id, full_name, mobile, email, password_hash)
            VALUES
            (UUID(), @FullName, @Mobile, @Email, @Password)";

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(query, dto);

        return Ok("User Created Successfully");
    }
}
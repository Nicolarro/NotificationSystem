using System.ComponentModel;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Application.Interfaces;
using TakeHomeChallenge.Domain.Entities;
using TakeHomeChallenge.Infrastructure;

namespace TakeHomeChallenge.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AutoMapper.IMapper _autoMapper;

    public UserController(IUserService userService, AutoMapper.IMapper autoMapper)
    {
        _userService = userService;
        _autoMapper = autoMapper;
    }



    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Description("GetUsers")]
    [HttpGet]
    public async Task<ActionResult<ICollection<User>>> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        if (users == null)
        {
            return NotFound();
        }
        return Ok(users);
    }



    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Description("GetUserByID")]
    [HttpGet("{id:int}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }



    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Description("CreteUser")]
    [HttpPost(Name = "CreteUser")]
    public async Task<ActionResult<User>> CreateUser([FromBody] UserDTO userDTO)
    {

        var user = _autoMapper.Map<User>(userDTO);
        var newUser = await _userService.CreateUser(user);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(newUser);
    }
}

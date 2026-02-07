using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TakeHomeChallenge.Application.DTOs;
using TakeHomeChallenge.Application.Interfaces;

namespace TakeHomeChallenge.API.Controllers;

/// <summary>
/// Manages user operations including CRUD and Pokemon assignments.
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets all registered users.
    /// </summary>
    /// <returns>A list of all users with their Pokemon IDs.</returns>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ICollection<UserResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ICollection<UserResponseDTO>>> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Gets a specific user by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user with the specified ID.</returns>
    /// <response code="200">Returns the user.</response>
    /// <response code="404">User not found.</response>
    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDTO>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="userDto">The user data to create.</param>
    /// <returns>The newly created user.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid input data.</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDTO>> CreateUser([FromBody] CreateUserDTO userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdUser = await _userService.CreateUserAsync(userDto);
        if (createdUser is null)
        {
            return BadRequest("Could not create user.");
        }

        return CreatedAtRoute("GetUserById", new { id = createdUser.Id }, createdUser);
    }

    /// <summary>
    /// Updates an existing user. Only provided fields will be modified.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="userDto">The fields to update.</param>
    /// <response code="204">User updated successfully.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="404">User not found.</response>
    [AllowAnonymous]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _userService.UpdateUserAsync(id, userDto);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="404">User not found.</response>
    [AllowAnonymous]        
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

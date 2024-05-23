using Microsoft.AspNetCore.Mvc;

namespace LoyaltyProgram.Users;

using Models;

[Route("/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly Dictionary<int, LoyaltyProgramUser> RegisteredUsers = new();

    [HttpGet]
    [Route("{userId:int}", Name = "GetUserById")]
    public ActionResult<LoyaltyProgramUser> GetUserById(int userId) =>
        RegisteredUsers.ContainsKey(userId) ? Ok(RegisteredUsers[userId]) : NotFound();

    [HttpPost("")]
    public ActionResult<LoyaltyProgramUser> CreateUser([FromBody]LoyaltyProgramUser user)
    {
        if (user is null) return BadRequest();
        var newUser = RegisterUser(user);
        return CreatedAtRoute(nameof(GetUserById), new { userId = newUser.Id }, newUser.Id);
    }

    [HttpPut("{userId:int}")]
    public LoyaltyProgramUser UpdateUser(int userId, [FromBody] LoyaltyProgramUser user) =>
        RegisteredUsers[userId] = user;

    private LoyaltyProgramUser RegisterUser(LoyaltyProgramUser user)
    {
        var userId = RegisteredUsers.Count;
        return RegisteredUsers[userId] = user with { Id = userId };
    }
}

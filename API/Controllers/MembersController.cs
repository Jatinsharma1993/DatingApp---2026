
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository) : BaseApiController
{   
    [HttpGet] // /api/users
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
    {
        return  Ok(await memberRepository.GetMembersAsync());
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(string id)
    {
        var user =  await memberRepository.GetMemberByIdAsync(id);

        if(user == null) return NotFound();

        return user;
    }

    [HttpGet("{id}/photos")] 
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhoto(string id)
    {
        return Ok(await memberRepository.GetPhotoForMemberAsync(id));
    }
}


using API.DTOs;
using API.Entities;
using API.Extensions;
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
        return Ok(await memberRepository.GetMembersAsync());
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(string id)
    {
        var user = await memberRepository.GetMemberByIdAsync(id);

        if (user == null) return NotFound();

        return user;
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhoto(string id)
    {
        return Ok(await memberRepository.GetPhotoForMemberAsync(id));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
        var memberId = User.GetMemberId();

        //  var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // if (memberId == null) return BadRequest("No id in token");

        var member = await memberRepository.GetMemberForUpdate(memberId);

        if (member == null) return BadRequest("No member found to be update");

        member.UserName = memberUpdateDto.UserName ?? member.UserName;
        member.Description = memberUpdateDto.Description ?? member.Description;
        member.City = memberUpdateDto.City ?? member.City;
        member.Country = memberUpdateDto.Country ?? member.Country;

        member.User.UserName = memberUpdateDto.UserName ?? member.User.UserName;

        memberRepository.Update(member); //Optional

        if (await memberRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to Update member");

    }
}

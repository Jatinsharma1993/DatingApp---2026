
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository , IPhotoService photoService) : BaseApiController
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

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm]IFormFile file)
    {
       var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

       if(member == null) return BadRequest("No member found!");

       var result = await photoService.UploadPhotoAsync(file);

       if(result.Error != null) return BadRequest(result.Error.Message);

       var photo = new Photo
       {
           Url = result.SecureUrl.AbsoluteUri,
           PublicId = result.PublicId,
           MemberId = User.GetMemberId()
       };

       if(member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        member.Photos.Add(photo);

        if(await memberRepository.SaveAllAsync()) return photo;

        return BadRequest("Problem uploading photo");

    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

        if(member == null) return BadRequest("Cannot get hold of member");

        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

        if(member.ImageUrl == photo?.Url || photo == null)
        {
            return BadRequest("Cannot set this image as main image");
        }

        member.ImageUrl = photo.Url;
        member.User.ImageUrl = photo.Url;

        if(await memberRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting main photo");

    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

        if(member == null) return BadRequest("Cannot get hold of member");

        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

        if(photo == null || photo.Url == member.ImageUrl) 
        {
            return BadRequest("This photo cannot be deleted");
        }

        if(photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
            
        }

         member.Photos.Remove(photo);

        if(await memberRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");

    }
}

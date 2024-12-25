using System;
using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
        //fill the age
        .ForMember(d => d.Age,
                    o => o.MapFrom(
                        s => s.DateOfBirth.CalculateAge()))
        //fill the photo url from photos (d for distination o for option s for source)
        .ForMember(d => d.PhotoUrl,
                    o => o.MapFrom(
                        s => s.Photos.FirstOrDefault(
                            x => x.IsMain)!
                            .Url));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<string, DateOnly>().ConstructUsing(s => DateOnly.Parse(s));

        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl,
                    o => o.MapFrom(
                            s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.RecipientPhotoUrl,
                    o => o.MapFrom(
                            s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));

        //To get the right UTC time in case getting from DB
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }

}

using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;

namespace DatingAppAPI.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(m=>m.PhotoUrl, opts=>
                opts.MapFrom(a=>a.Photos.FirstOrDefault(p=>p.IsMain).Url));
        CreateMap<Photo, PhotoDto>();
    }
}
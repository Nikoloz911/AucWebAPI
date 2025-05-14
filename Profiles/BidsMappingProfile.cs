using AucWebAPI.DTOs.BidsDTOs;
using AucWebAPI.Models;
using AutoMapper;

namespace AucWebAPI.Profiles;
public class BidsMappingProfile : Profile
{
    public BidsMappingProfile() {

        CreateMap<AddBidDTO, Bid>()
          .ForMember(dest => dest.BidTime, opt => opt.Ignore()); 

        CreateMap<Bid, AddBidResponseDTO>();
        CreateMap<Bid, GetBidByIdDTO>();
    }
}

using AucWebAPI.DTOs.AuctionDTOs;
using AucWebAPI.Models;
using AutoMapper;

namespace AucWebAPI.Profiles;
public class AuctionMappingProfile : Profile
{
    public AuctionMappingProfile()
    {
        CreateMap<AddAuctionDTO, Auction>()
            .ForMember(dest => dest.Status, opt => opt.Ignore()); 

        CreateMap<Auction, AddAuctionResponseDTO>();
        CreateMap<Auction, GetAllAuctionsDTO>();
        CreateMap<Auction, GetAuctionsFilterDTO>();
        CreateMap<Auction, GetAuctionsByIdDTO>();
        CreateMap<UpdateAuctionDTO, Auction>()
            .ForMember(dest => dest.Status, opt => opt.Ignore()); 

        CreateMap<Auction, UpdateAuctionResponseDTO>();
        CreateMap<Auction, GetActiveAuctionsDTO>();
        CreateMap<Auction, GetUsersAuctionsDTO>();
        CreateMap<Bid, GetBidDTO>();
    }
}

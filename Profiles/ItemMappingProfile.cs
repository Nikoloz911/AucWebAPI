using AucWebAPI.DTOs.ItemDTOs;
using AucWebAPI.Models;
using AutoMapper;

namespace AucWebAPI.Profiles;
public class ItemMappingProfile : Profile
{
    public ItemMappingProfile()
    {
        CreateMap<AddItemDTO, Item>()
     .ForMember(dest => dest.Condition, opt => opt.Ignore());
        CreateMap<Item, AddItemResponseDTO>();
        CreateMap<UpdateItemDTO, Item>()
    .ForMember(dest => dest.Condition, opt => opt.Ignore()); 

        CreateMap<Item, UpdateItemResponseDTO>();

    }
}
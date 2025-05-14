using AucWebAPI.Data;
using AucWebAPI.Services.Interfaces;
using AutoMapper;
namespace AucWebAPI.Services.Implementations;
public class BidsService : IBidsService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public BidsService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
}

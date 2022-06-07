namespace GrannysKitchen.API.Helpers;

using AutoMapper;
using GrannysKitchen.Models.RequestModels;
using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.ResponseModels;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterRequest, ChefUsers>();
        CreateMap<RegisterRequest, Users>();
        CreateMap<ChefUsers, AuthenticateResponse>();
        CreateMap<Users, AuthenticateResponse>();
    }
}



using AutoMapper;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Models.dto;

namespace core8_nuxt_cassandra.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<UserRegister, User>();
            CreateMap<UserLogin, User>();
            CreateMap<UserUpdate, User>();
            CreateMap<UserPasswordUpdate, User>();
            CreateMap<Product, ProductModel>();
            CreateMap<ProductModel, Product>();

        }
    }
    

}
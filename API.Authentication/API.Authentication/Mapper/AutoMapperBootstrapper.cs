using API.Authentication.DTO.Users;
using AutoMapper;
using Common.Encoding.Hash;
using Data.Authentication.Models;
using System;

namespace API.Authentication.Mapper
{
    /// <summary>
    /// Automapper bootstrapper
    /// </summary>
    public class AutoMapperBootstrapper : Profile 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AutoMapperBootstrapper()
        {
            CreateMap<User, UserDTO>();

            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password.ToSHA256()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(x => x.Email, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.CreateDate, opt => opt.Ignore());
        }
    }
}

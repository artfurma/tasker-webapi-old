using AutoMapper;
using TaskerWebAPI.DTO;
using TaskerWebAPI.Models;

namespace TaskerWebAPI.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<User, UserDTO>();
			CreateMap<UserDTO, User>();
		}
	}
}
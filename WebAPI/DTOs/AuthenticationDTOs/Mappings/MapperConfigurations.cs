using Application.UseCases.Authentication.Commands;
using AutoMapper;

namespace WebAPI.DTOs.AuthenticationDTOs.Mappings {
	public class MapperConfigurations : Profile {
		public MapperConfigurations() {
			CreateMap<RegisterDTO, RegisterCommand>();
		}
	}
}

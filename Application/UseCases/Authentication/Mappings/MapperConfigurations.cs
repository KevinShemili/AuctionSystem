using Application.UseCases.Authentication.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Authentication.Mappings {
	public class MapperConfigurations : Profile {
		public MapperConfigurations() {
			CreateMap<RegisterCommand, User>();
		}
	}
}

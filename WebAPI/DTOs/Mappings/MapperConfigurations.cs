using Application.UseCases.Auctions.Commands;
using Application.UseCases.Authentication.Commands;
using AutoMapper;
using WebAPI.DTOs.AuctionDTOs;
using WebAPI.DTOs.AuthenticationDTOs;

namespace WebAPI.DTOs.Mappings {
	public class MapperConfigurations : Profile {
		public MapperConfigurations() {
			CreateMap<RegisterDTO, RegisterCommand>();
			CreateMap<SignInDTO, SignInCommand>();
			CreateMap<TokensDTO, RefreshTokenCommand>();
			CreateMap<CreateAuctionDTO, CreateAuctionCommand>();
		}
	}
}

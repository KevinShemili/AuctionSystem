using Application.UseCases.Auctions.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.UseCases.Auctions.Mappings {
	public class MapperConfigurations : Profile {
		public MapperConfigurations() {
			CreateMap<CreateAuctionCommand, Auction>()
				.ForMember(dest => dest.Images, opt => opt.Ignore());
		}
	}
}

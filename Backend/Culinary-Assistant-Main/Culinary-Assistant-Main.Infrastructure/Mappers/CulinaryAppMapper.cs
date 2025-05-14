using AutoMapper;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Feedback;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.DTO.ReceiptCollection;
using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Mappers
{
	public class CulinaryAppMapper : Profile
	{
		private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

		public CulinaryAppMapper()
		{
			MapReceipts();
		}


		private void MapReceipts()
		{
			CreateMap<Receipt, FullReceiptOutDTO>()
				.ForMember(fr => fr.Title, opt => opt.MapFrom(r => r.Title.Value))
				.ForMember(fr => fr.Description, opt => opt.MapFrom(r => r.Description.Value));

			CreateMap<Receipt, ShortReceiptOutDTO>()
				.ForMember(sr => sr.Title, opt => opt.MapFrom(r => r.Title.Value))
				.ForMember(fr => fr.Description, opt => opt.MapFrom(r => r.Description.Value));
		}
	}
}

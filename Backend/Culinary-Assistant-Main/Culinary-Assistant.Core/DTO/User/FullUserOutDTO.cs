﻿using Culinary_Assistant.Core.DTO.Receipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.User
{
	public class FullUserOutDTO
	{
		public Guid Id { get; set; }
		public string Login { get; set; }
		public long? Phone { get; set; }
		public string? Email { get; set; }
		public string PresignedPictureUrl { get; set; }
		public List<ShortReceiptOutDTO> Receipts { get; set; }
	}
}

using Culinary_Assistant.Core.DTO.User;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Receipt
{
	public class FullReceiptOutDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string PresignedPictureUrl { get; set; }
	}
}

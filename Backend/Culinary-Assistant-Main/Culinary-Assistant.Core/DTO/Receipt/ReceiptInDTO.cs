﻿using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.DTO.Receipt
{
	public record ReceiptInDTO(string Title, string Description,  FilePath? MainPictureUrl);
}

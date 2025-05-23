﻿
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Services.Files;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Culinary_Assistant_Main.Services.Receipts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Startups
{
    public static class ServicesStartup
	{
		public static IServiceCollection AddCustomServices(this IServiceCollection services)
		{
			services.AddScoped<IFileService, FileService>();
			services.AddScoped<IFileMessagesProducerService, FileMessagesProducerService>();
			services.AddScoped<IReceiptsService, ReceiptsService>();
			return services;
		}
	}
}

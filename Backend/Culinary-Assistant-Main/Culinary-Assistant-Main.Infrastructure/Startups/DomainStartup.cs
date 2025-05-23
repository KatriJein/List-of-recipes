﻿using Core.Base.Interfaces;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant_Main.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Infrastructure.Startups
{
    public static class DomainStartup
	{
		public static IServiceCollection AddDomain(this IServiceCollection services)
		{
			services.AddScoped<IReceiptsRepository, ReceiptsRepository>();
			return services;
		}
	}
}

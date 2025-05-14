using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Filters;
using Culinary_Assistant.Core.ServicesResponses;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Domain.Repositories;
using Culinary_Assistant.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using System.Text.Json;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Const;
using Minio;
using Culinary_Assistant.Core.Redis;
using Culinary_Assistant_Main.Services.Files;

namespace Culinary_Assistant_Main.Services.Receipts
{
	public class ReceiptsService(IReceiptsRepository receiptsRepository, IFileService fileService, ILogger logger) :
		BaseService<Receipt, ReceiptInDTO, UpdateReceiptDTO>(receiptsRepository, logger), IReceiptsService
	{
		private readonly IFileService _fileService = fileService;

		private readonly Dictionary<ReceiptSortOption?, Func<Receipt, double>> _orderByExpressions = new()
		{
			{ ReceiptSortOption.ByCalories, (Receipt receipt) => receipt.Nutrients.Calories },
		};

		public async Task<Result<EntitiesResponseWithCountAndPages<Receipt>>> GetAllAsync(ReceiptsFilter receiptsFilter, CancellationToken cancellationToken = default, List<Guid>? collectionReceiptsIds = null)
		{
			List<Guid> requiredReceiptsIds = [Guid.Empty];
			var filteredReceipts = await DoReceiptsFilteringAsync(requiredReceiptsIds, collectionReceiptsIds, receiptsFilter, cancellationToken);
			var sortedReceipts = DoSorting(filteredReceipts, receiptsFilter.SortOption, _orderByExpressions, receiptsFilter.IsAscendingSorting);
			var response = ApplyPaginationToEntities(sortedReceipts, receiptsFilter);
			await _fileService.SetPresignedUrlsForReceiptsAsync(BucketConstants.ReceiptsImagesBucketName, response.Data);
			return Result.Success(response);
		}

		public async override Task<Receipt?> GetByGuidAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var receipt = await base.GetByGuidAsync(id, cancellationToken);
			if (receipt != null)
				await _fileService.SetPresignedUrlsForReceiptsAsync(BucketConstants.ReceiptsImagesBucketName, [receipt]);
			return receipt;
		}

		public override async Task<Result> NotBulkUpdateAsync(Guid entityId, UpdateReceiptDTO updateRequest)
		{
			var results = Miscellaneous.CreateResultList(3);
			var existingReceipt = await GetByGuidAsync(entityId);
			if (existingReceipt == null) return Result.Failure("Сущности для обновления не существует");
			results[0] = existingReceipt.SetTitle(updateRequest.Title ?? existingReceipt.Title.Value);
			results[1] = existingReceipt.SetDescription(updateRequest.Description ?? existingReceipt.Description.Value);
			if (updateRequest.MainPictureUrl != null)
				results[2] = existingReceipt.SetPicture(updateRequest.MainPictureUrl);
			if (!results.All(r => r.IsSuccess)) return Miscellaneous.ResultFailureWithAllFailuresFromResultList(results);
			existingReceipt.ActualizeUpdatedAtField();
			var res = await base.NotBulkUpdateAsync(entityId, updateRequest);
			return res;
		}

		public override async Task<Result<Guid>> CreateAsync(ReceiptInDTO entityCreateRequest, bool autoSave = true)
		{
			var receiptResult = Receipt.Create(entityCreateRequest);
			if (!receiptResult.IsSuccess)
				return Result.Failure<Guid>(receiptResult.Error);
			var nutrientsResult = receiptResult.Value.SetNutrients(20, 15, 20, 30);
			if (!nutrientsResult.IsSuccess)
				return Result.Failure<Guid>(nutrientsResult.Error);
			var res = await AddToRepositoryAsync(receiptResult);
			return res;
		}

		public override async Task<Result<string>> NotBulkDeleteAsync(Guid entityId)
		{
			var entity = await GetByGuidAsync(entityId);
			if (entity != null)
				await _fileService.RemoveFilesAsync(BucketConstants.ReceiptsImagesBucketName, [entity.MainPictureUrl]);
			return await base.NotBulkDeleteAsync(entityId);
		}


		private async Task<List<Receipt>> DoReceiptsFilteringAsync(List<Guid> requiredIds, List<Guid>? collectionReceiptsIds, ReceiptsFilter receiptsFilter, CancellationToken cancellationToken)
		{
			var tags = new HashSet<Tag>(receiptsFilter.Tags ?? []);
			var categories = new HashSet<Category>(receiptsFilter.Categories ?? []);
			var difficulties = new HashSet<CookingDifficulty>(receiptsFilter.CookingDifficulties ?? []);
			var hadEmpty = requiredIds.Count > 0 && requiredIds[0] == Guid.Empty;
			var searchForReceiptsInCollectionHashSet = new HashSet<Guid>(collectionReceiptsIds ?? []);
			var searchForIdsHashSet = new HashSet<Guid>(requiredIds);
			var data = await _repository.GetAll()
				.Where(r => collectionReceiptsIds == null || searchForReceiptsInCollectionHashSet.Contains(r.Id))
				.Where(r => hadEmpty || searchForIdsHashSet.Contains(r.Id))
				.Where(r => r.Title.Value.ToLower().Contains(receiptsFilter.SearchByTitle.ToLower()))
				.OrderByDescending(r => r.UpdatedAt)
				.ToListAsync(cancellationToken);
			return data;
		}
	}
}

using Core.Base;
using CSharpFunctionalExtensions;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.DTO.Receipt;
using Culinary_Assistant.Core.Enums;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Domain.Models
{
    public class Receipt : Core.Base.Entity<Guid>
	{
		public Text Title { get; private set; }
		public Text Description { get; private set; }
		public Nutrients Nutrients { get; private set; }
		public string MainPictureUrl { get; private set; }
		public string PicturesUrls { get; private set; }
		public DateTime UpdatedAt { get; private set; }

		[NotMapped]
		public string PresignedPictureUrl { get; set; }
		[NotMapped]
		public List<FilePath> PresignedPicturesUrls { get; set; }

		public static Result<Receipt> Create(ReceiptInDTO receiptInDTO)
		{
			var results = Miscellaneous.CreateResultList(3);
			var receipt = new Receipt();
			results[0] = receipt.SetTitle(receiptInDTO.Title);
			results[1] = receipt.SetDescription(receiptInDTO.Description);
			results[2] = receipt.SetPicture(receiptInDTO.MainPictureUrl);
			if (results.Any(r => r.IsFailure))
				return Result.Failure<Receipt>(Miscellaneous.ResultFailureWithAllFailuresFromResultList(results).Error);
			receipt.ActualizeUpdatedAtField();
			return Result.Success(receipt);
		}

		public Result SetTitle(string title)
		{
			var titleResult = Text.Create(title, 100);
			if (titleResult.IsFailure) return Result.Failure(titleResult.Error);
			Title = titleResult.Value;
			return Result.Success();
		}

		public Result SetDescription(string description)
		{
			var descriptionResult = Text.Create(description, 1000);
			if (descriptionResult.IsFailure) return Result.Failure(descriptionResult.Error);
			Description = descriptionResult.Value;
			return Result.Success();
		}



		public Result SetPicture(FilePath? pictureUrl)
		{
			if (pictureUrl == null)
			{
				MainPictureUrl = "none";
				PicturesUrls = "none";
				return Result.Success();
			}
			MainPictureUrl = pictureUrl.Url;
			PicturesUrls = JsonSerializer.Serialize(new List<FilePath>() { new(MainPictureUrl)});
			return Result.Success();
		}

		public Result SetNutrients(double calories, double proteins, double fats, double carbohydrates)
		{
			var nutrientsResult =  Nutrients.Create(calories, proteins, fats, carbohydrates);
			if (nutrientsResult.IsSuccess)
			{
				Nutrients = nutrientsResult.Value;
				return Result.Success();
			}
			return Result.Failure(nutrientsResult.Error);
		}

		/// <summary>
		/// Установить Id рецепта. Лучше не использовать, необходим только для тестирования!
		/// </summary>
		public void SetReceiptId(Guid id)
		{
			SetId(id);
		}

		public void ActualizeUpdatedAtField()
		{
			UpdatedAt = DateTime.UtcNow;
		}
	}

}

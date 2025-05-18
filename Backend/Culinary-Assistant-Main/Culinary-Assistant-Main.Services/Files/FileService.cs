using Amazon.S3;
using Amazon.S3.Model;
using Culinary_Assistant.Core.Const;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant.Core.Utils;
using Culinary_Assistant_Main.Domain.Models;
using Culinary_Assistant_Main.Services.RabbitMQ.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Files
{
	public class FileService(IOptions<AmazonS3Options> amazonOptions, ILogger logger) : IFileService
	{
		private readonly IAmazonS3 _amazonClient = new AmazonS3Client(new AmazonS3Config() { ServiceURL = amazonOptions.Value.ServiceURL });
		private readonly ILogger _logger = logger;

		public async Task<List<FilePath>> InitiateFilesUploadingAndSendLinksAsync(string bucketName, string entityName, FilesDTO filesDTO)
		{
			var uniqueFileNames = filesDTO.Files.Select(f => FileUtils.GenerateUniqueNameForFileName(f.FileName)).ToList();
			await UploadFilesAsync(filesDTO.Files, bucketName, uniqueFileNames);
			return uniqueFileNames.Select(fileName => new FilePath(fileName)).ToList();
		}

		public async Task RemoveFilesAsync(string bucketName, List<string> pictureUrls)
		{
			var fileTasks = new Task[pictureUrls.Count];
			for (var i = 0; i <  pictureUrls.Count; i++)
			{
				var pictureUrl = pictureUrls[i];
				fileTasks[i] = Task.Run(async () =>
				{
					var deleteRequest = new DeleteObjectRequest() { BucketName = bucketName, Key = pictureUrl };
					try
					{
						await _amazonClient.DeleteObjectAsync(deleteRequest);
					}
					catch (Exception e)
					{
						_logger.Error("Ошибка при удалении объекта {@obj}: {@error}", pictureUrl, e.Message);
					}
				});
			}
			await Task.WhenAll(fileTasks);
		}

		private async Task UploadFilesAsync(List<IFormFile> files, string bucketName, List<string> fileNames)
		{
			var fileTasks = new Task[files.Count];
			for (var i = 0; i < files.Count; i++)
			{
				var file = files[i];
				var fileName = fileNames[i];
				fileTasks[i] = Task.Run(async () =>
				{
					using var memoryStream = new MemoryStream();
					await file.CopyToAsync(memoryStream);
					memoryStream.Position = 0;
					var putRequest = new PutObjectRequest() { BucketName = bucketName, Key = fileName, InputStream = memoryStream, ContentType = file.ContentType };
					try
					{
						await _amazonClient.PutObjectAsync(putRequest);
					}
					catch (Exception e)
					{
						_logger.Error("Ошибка при загрузке объекта {@obj}: {@error}", fileName, e.Message);
					}
				});
			}
			await Task.WhenAll(fileTasks);
		}

		public async Task SetPresignedUrlsForReceiptsAsync(string bucketName, List<Receipt> receipts)
		{
			foreach (var receipt in receipts)
			{
				var getPresignedURLRequest = new GetPreSignedUrlRequest() { BucketName = bucketName, Expires = DateTime.UtcNow.AddHours(12), Key = receipt.MainPictureUrl };
				try
				{
					receipt.PresignedPictureUrl = await _amazonClient.GetPreSignedURLAsync(getPresignedURLRequest);
				}
				catch (Exception e)
				{
					_logger.Error("Ошибка при генерации ссылки на объект {@obj}: {@error}", receipt.MainPictureUrl, e.Message);
					receipt.PresignedPictureUrl = "none";
				}
			}
		}
	}
}

using Culinary_Assistant.Core.DTO;
using Culinary_Assistant.Core.Shared.Serializable;
using Culinary_Assistant_Main.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Files
{
	public interface IFileService
	{
		Task<List<FilePath>> InitiateFilesUploadingAndSendLinksAsync(string bucketName, string entityName, FilesDTO filesDTO);
		Task RemoveFilesAsync(string bucketName, List<string> pictureUrls);
		Task SetPresignedUrlsForReceiptsAsync(string bucketName, List<Receipt> receipts);
	}
}

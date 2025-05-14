using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Const
{
	public static class BucketConstants
	{
		public const string ReceiptsImagesBucketName = "culinar-unique334-pictures";
		public const string UserProfilePicturesBucketName = "userpictures";

		public const int PresignedUrlExpiryTimeInSeconds = 3600;
	}
}

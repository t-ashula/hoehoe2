using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{

	public interface IMultimediaShareService
	{
		string Upload(ref string filePath, ref string message, long reply_to);
		bool CheckValidExtension(string ext);
		string GetFileOpenDialogFilter();
		UploadFileType GetFileType(string ext);
		bool IsSupportedFileType(UploadFileType type);
		bool CheckValidFilesize(string ext, long fileSize);
		bool Configuration(string key, object value);
	}
}

namespace Tween
{
    public interface IMultimediaShareService
    {
        string Upload(ref string filePath, ref string message, long reply_to);

        bool CheckValidExtension(string ext);

        string GetFileOpenDialogFilter();

        Tween.MyCommon.UploadFileType GetFileType(string ext);

        bool IsSupportedFileType(Tween.MyCommon.UploadFileType type);

        bool CheckValidFilesize(string ext, long fileSize);

        bool Configuration(string key, object value);
    }
}
using System.IO;
using System.Drawing;

class FileImageSource : IImageSource
{
	string basePath;

	public FileImageSource(string basePath)
	{
		this.basePath = basePath;
	}

	public Image GetImage(string imageId)
	{
		string path = Path.Combine(basePath, imageId);
		// TODO: Error handling (e.g. return some default image)
		return Bitmap.FromFile(path);
	}
}

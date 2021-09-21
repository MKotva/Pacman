using System.Drawing;

public interface IImageSource
{
	Image GetImage(string imageId);
}

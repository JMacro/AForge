namespace AForge.Imaging
{
	internal static class ExceptionMessage
	{
		public const string ColorHistogramException = "Cannot access color histogram since the last processed image was grayscale.";

		public const string GrayHistogramException = "Cannot access gray histogram since the last processed image was color.";
	}
}

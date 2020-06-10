using System;

namespace AForge.Imaging
{
	public class InvalidImagePropertiesException : ArgumentException
	{
		public InvalidImagePropertiesException()
		{
		}

		public InvalidImagePropertiesException(string message)
			: base(message)
		{
		}

		public InvalidImagePropertiesException(string message, string paramName)
			: base(message, paramName)
		{
		}
	}
}

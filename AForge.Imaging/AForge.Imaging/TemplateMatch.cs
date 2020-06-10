using System.Drawing;

namespace AForge.Imaging
{
	public class TemplateMatch
	{
		private Rectangle rect;

		private float similarity;

		public Rectangle Rectangle => rect;

		public float Similarity => similarity;

		public TemplateMatch(Rectangle rect, float similarity)
		{
			this.rect = rect;
			this.similarity = similarity;
		}
	}
}

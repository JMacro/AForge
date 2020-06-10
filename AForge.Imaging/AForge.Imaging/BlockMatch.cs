namespace AForge.Imaging
{
	public class BlockMatch
	{
		private IntPoint sourcePoint;

		private IntPoint matchPoint;

		private float similarity;

		public IntPoint SourcePoint => sourcePoint;

		public IntPoint MatchPoint => matchPoint;

		public float Similarity => similarity;

		public BlockMatch(IntPoint sourcePoint, IntPoint matchPoint, float similarity)
		{
			this.sourcePoint = sourcePoint;
			this.matchPoint = matchPoint;
			this.similarity = similarity;
		}
	}
}

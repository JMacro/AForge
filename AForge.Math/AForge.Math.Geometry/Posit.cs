using System;

namespace AForge.Math.Geometry
{
	public class Posit
	{
		private const float stop_epsilon = 0.0001f;

		private float focalLength;

		private Vector3[] modelPoints;

		private Matrix3x3 modelVectors;

		private Matrix3x3 modelPseudoInverse;

		public Vector3[] Model => (Vector3[])modelPoints.Clone();

		public float FocalLength
		{
			get
			{
				return focalLength;
			}
			set
			{
				focalLength = value;
			}
		}

		public Posit(Vector3[] model, float focalLength)
		{
			if (model.Length != 4)
			{
				throw new ArgumentException("The model must have 4 points.");
			}
			this.focalLength = focalLength;
			modelPoints = (Vector3[])model.Clone();
			modelVectors = Matrix3x3.CreateFromRows(model[1] - model[0], model[2] - model[0], model[3] - model[0]);
			modelPseudoInverse = modelVectors.PseudoInverse();
		}

		public void EstimatePose(Point[] points, out Matrix3x3 rotation, out Vector3 translation)
		{
			if (points.Length != 4)
			{
				throw new ArgumentException("4 points must be be given for pose estimation.");
			}
			float num = 0f;
			float num2 = 1f;
			Vector3 vector = new Vector3(points[0].X);
			Vector3 vector2 = new Vector3(points[0].Y);
			Vector3 vector3 = new Vector3(points[1].X, points[2].X, points[3].X);
			Vector3 vector4 = new Vector3(points[1].Y, points[2].Y, points[3].Y);
			int i = 0;
			Vector3 vector5 = default(Vector3);
			Vector3 vector6 = default(Vector3);
			Vector3 vector7 = default(Vector3);
			Vector3 vector8 = default(Vector3);
			Vector3 vector9 = default(Vector3);
			Vector3 vector10 = new Vector3(1f);
			for (; i < 100; i++)
			{
				vector8 = vector3 * vector10 - vector;
				vector9 = vector4 * vector10 - vector2;
				vector5 = modelPseudoInverse * vector8;
				vector6 = modelPseudoInverse * vector9;
				float num3 = vector5.Normalize();
				float num4 = vector6.Normalize();
				num2 = (num3 + num4) / 2f;
				vector7 = Vector3.Cross(vector5, vector6);
				num = focalLength / num2;
				Vector3 vector11 = vector10;
				vector10 = modelVectors * vector7 / num + 1f;
				if ((vector10 - vector11).Abs().Max < 0.0001f)
				{
					break;
				}
			}
			rotation = Matrix3x3.CreateFromRows(vector5, vector6, vector7);
			Vector3 vector12 = rotation * modelPoints[0];
			translation = new Vector3(points[0].X / num2 - vector12.X, points[0].Y / num2 - vector12.Y, focalLength / num2);
		}
	}
}

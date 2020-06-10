using System;

namespace AForge.Math.Geometry
{
	public class CoplanarPosit
	{
		private const float ErrorLimit = 2f;

		private float focalLength;

		private Vector3[] modelPoints;

		private Matrix3x3 modelVectors;

		private Matrix3x3 modelPseudoInverse;

		private Vector3 modelNormal;

		private Matrix3x3 alternateRotation = default(Matrix3x3);

		private Vector3 alternateTranslation = default(Vector3);

		private float alternatePoseError;

		private Matrix3x3 bestRotation = default(Matrix3x3);

		private Vector3 bestTranslation = default(Vector3);

		private float bestPoseError;

		public Matrix3x3 BestEstimatedRotation => bestRotation;

		public Vector3 BestEstimatedTranslation => bestTranslation;

		public float BestEstimationError => bestPoseError;

		public Matrix3x3 AlternateEstimatedRotation => alternateRotation;

		public Vector3 AlternateEstimatedTranslation => alternateTranslation;

		public float AlternateEstimationError => alternatePoseError;

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

		public CoplanarPosit(Vector3[] model, float focalLength)
		{
			if (model.Length != 4)
			{
				throw new ArgumentException("The model must have 4 points.");
			}
			this.focalLength = focalLength;
			modelPoints = (Vector3[])model.Clone();
			modelVectors = Matrix3x3.CreateFromRows(model[1] - model[0], model[2] - model[0], model[3] - model[0]);
			modelVectors.SVD(out Matrix3x3 u, out Vector3 e, out Matrix3x3 v);
			modelPseudoInverse = v * Matrix3x3.CreateDiagonal(e.Inverse()) * u.Transpose();
			modelNormal = v.GetColumn(e.MinIndex);
		}

		public void EstimatePose(Point[] points, out Matrix3x3 rotation, out Vector3 translation)
		{
			if (points.Length != 4)
			{
				throw new ArgumentException("4 points must be be given for pose estimation.");
			}
			POS(points, new Vector3(1f), out Matrix3x3 rotation2, out Matrix3x3 rotation3, out Vector3 translation2, out Vector3 translation3);
			float num = Iterate(points, ref rotation2, ref translation2);
			float num2 = Iterate(points, ref rotation3, ref translation3);
			if (num < num2)
			{
				bestRotation = rotation2;
				bestTranslation = translation2;
				bestPoseError = num;
				alternateRotation = rotation3;
				alternateTranslation = translation3;
				alternatePoseError = num2;
			}
			else
			{
				bestRotation = rotation3;
				bestTranslation = translation3;
				bestPoseError = num2;
				alternateRotation = rotation2;
				alternateTranslation = translation2;
				alternatePoseError = num;
			}
			rotation = bestRotation;
			translation = bestTranslation;
		}

		private float Iterate(Point[] points, ref Matrix3x3 rotation, ref Vector3 translation)
		{
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 100; i++)
			{
				Vector3 eps = modelVectors * rotation.GetRow(2) / translation.Z + 1f;
				POS(points, eps, out Matrix3x3 rotation2, out Matrix3x3 rotation3, out Vector3 translation2, out Vector3 translation3);
				float error = GetError(points, rotation2, translation2);
				float error2 = GetError(points, rotation3, translation3);
				if (error < error2)
				{
					rotation = rotation2;
					translation = translation2;
					num2 = error;
				}
				else
				{
					rotation = rotation3;
					translation = translation3;
					num2 = error2;
				}
				if (num2 <= 2f || num2 > num)
				{
					break;
				}
				num = num2;
			}
			return num2;
		}

		private void POS(Point[] imagePoints, Vector3 eps, out Matrix3x3 rotation1, out Matrix3x3 rotation2, out Vector3 translation1, out Vector3 translation2)
		{
			Vector3 vector = new Vector3(imagePoints[1].X, imagePoints[2].X, imagePoints[3].X);
			Vector3 vector2 = new Vector3(imagePoints[1].Y, imagePoints[2].Y, imagePoints[3].Y);
			Vector3 vector3 = vector * eps - imagePoints[0].X;
			Vector3 vector4 = vector2 * eps - imagePoints[0].Y;
			Vector3 vector5 = modelPseudoInverse * vector3;
			Vector3 vector6 = modelPseudoInverse * vector4;
			Vector3 vector7 = default(Vector3);
			Vector3 vector8 = default(Vector3);
			Vector3 vector9 = default(Vector3);
			float num = vector6.Square - vector5.Square;
			float num2 = Vector3.Dot(vector5, vector6);
			float num3 = 0f;
			float num4 = 0f;
			if (num == 0f)
			{
				num4 = (float)(-System.Math.PI / 2.0 * (double)System.Math.Sign(num2));
				num3 = (float)System.Math.Sqrt(System.Math.Abs(2f * num2));
			}
			else
			{
				num3 = (float)System.Math.Sqrt(System.Math.Sqrt(num * num + 4f * num2 * num2));
				num4 = (float)System.Math.Atan(-2f * num2 / num);
				if (num < 0f)
				{
					num4 += (float)System.Math.PI;
				}
				num4 /= 2f;
			}
			float factor = (float)((double)num3 * System.Math.Cos(num4));
			float factor2 = (float)((double)num3 * System.Math.Sin(num4));
			vector7 = vector5 + modelNormal * factor;
			vector8 = vector6 + modelNormal * factor2;
			float num5 = vector7.Normalize();
			float num6 = vector8.Normalize();
			vector9 = Vector3.Cross(vector7, vector8);
			rotation1 = Matrix3x3.CreateFromRows(vector7, vector8, vector9);
			float num7 = (num5 + num6) / 2f;
			Vector3 vector10 = rotation1 * modelPoints[0];
			translation1 = new Vector3(imagePoints[0].X / num7 - vector10.X, imagePoints[0].Y / num7 - vector10.Y, focalLength / num7);
			vector7 = vector5 - modelNormal * factor;
			vector8 = vector6 - modelNormal * factor2;
			num5 = vector7.Normalize();
			num6 = vector8.Normalize();
			vector9 = Vector3.Cross(vector7, vector8);
			rotation2 = Matrix3x3.CreateFromRows(vector7, vector8, vector9);
			num7 = (num5 + num6) / 2f;
			vector10 = rotation2 * modelPoints[0];
			translation2 = new Vector3(imagePoints[0].X / num7 - vector10.X, imagePoints[0].Y / num7 - vector10.Y, focalLength / num7);
		}

		private float GetError(Point[] imagePoints, Matrix3x3 rotation, Vector3 translation)
		{
			Vector3 vector = rotation * modelPoints[0] + translation;
			vector.X = vector.X * focalLength / vector.Z;
			vector.Y = vector.Y * focalLength / vector.Z;
			Vector3 vector2 = rotation * modelPoints[1] + translation;
			vector2.X = vector2.X * focalLength / vector2.Z;
			vector2.Y = vector2.Y * focalLength / vector2.Z;
			Vector3 vector3 = rotation * modelPoints[2] + translation;
			vector3.X = vector3.X * focalLength / vector3.Z;
			vector3.Y = vector3.Y * focalLength / vector3.Z;
			Vector3 vector4 = rotation * modelPoints[3] + translation;
			vector4.X = vector4.X * focalLength / vector4.Z;
			vector4.Y = vector4.Y * focalLength / vector4.Z;
			Point[] array = new Point[4]
			{
				new Point(vector.X, vector.Y),
				new Point(vector2.X, vector2.Y),
				new Point(vector3.X, vector3.Y),
				new Point(vector4.X, vector4.Y)
			};
			float angleBetweenVectors = GeometryTools.GetAngleBetweenVectors(imagePoints[0], imagePoints[1], imagePoints[3]);
			float angleBetweenVectors2 = GeometryTools.GetAngleBetweenVectors(imagePoints[1], imagePoints[2], imagePoints[0]);
			float angleBetweenVectors3 = GeometryTools.GetAngleBetweenVectors(imagePoints[2], imagePoints[3], imagePoints[1]);
			float angleBetweenVectors4 = GeometryTools.GetAngleBetweenVectors(imagePoints[3], imagePoints[0], imagePoints[2]);
			float angleBetweenVectors5 = GeometryTools.GetAngleBetweenVectors(array[0], array[1], array[3]);
			float angleBetweenVectors6 = GeometryTools.GetAngleBetweenVectors(array[1], array[2], array[0]);
			float angleBetweenVectors7 = GeometryTools.GetAngleBetweenVectors(array[2], array[3], array[1]);
			float angleBetweenVectors8 = GeometryTools.GetAngleBetweenVectors(array[3], array[0], array[2]);
			return (System.Math.Abs(angleBetweenVectors - angleBetweenVectors5) + System.Math.Abs(angleBetweenVectors2 - angleBetweenVectors6) + System.Math.Abs(angleBetweenVectors3 - angleBetweenVectors7) + System.Math.Abs(angleBetweenVectors4 - angleBetweenVectors8)) / 4f;
		}
	}
}

using System;

namespace AForge.Math
{
	[Serializable]
	public struct Matrix3x3
	{
		public float V00;

		public float V01;

		public float V02;

		public float V10;

		public float V11;

		public float V12;

		public float V20;

		public float V21;

		public float V22;

		public static Matrix3x3 Identity
		{
			get
			{
				Matrix3x3 result = default(Matrix3x3);
				result.V00 = (result.V11 = (result.V22 = 1f));
				return result;
			}
		}

		public float Determinant => V00 * V11 * V22 + V01 * V12 * V20 + V02 * V10 * V21 - V00 * V12 * V21 - V01 * V10 * V22 - V02 * V11 * V20;

		public float[] ToArray()
		{
			return new float[9]
			{
				V00,
				V01,
				V02,
				V10,
				V11,
				V12,
				V20,
				V21,
				V22
			};
		}

		public static Matrix3x3 CreateRotationY(float radians)
		{
			Matrix3x3 result = default(Matrix3x3);
			float v = (float)System.Math.Cos(radians);
			float num = (float)System.Math.Sin(radians);
			result.V00 = (result.V22 = v);
			result.V02 = num;
			result.V20 = 0f - num;
			result.V11 = 1f;
			return result;
		}

		public static Matrix3x3 CreateRotationX(float radians)
		{
			Matrix3x3 result = default(Matrix3x3);
			float v = (float)System.Math.Cos(radians);
			float num = (float)System.Math.Sin(radians);
			result.V11 = (result.V22 = v);
			result.V12 = 0f - num;
			result.V21 = num;
			result.V00 = 1f;
			return result;
		}

		public static Matrix3x3 CreateRotationZ(float radians)
		{
			Matrix3x3 result = default(Matrix3x3);
			float v = (float)System.Math.Cos(radians);
			float num = (float)System.Math.Sin(radians);
			result.V00 = (result.V11 = v);
			result.V01 = 0f - num;
			result.V10 = num;
			result.V22 = 1f;
			return result;
		}

		public static Matrix3x3 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			return CreateRotationY(yaw) * CreateRotationX(pitch) * CreateRotationZ(roll);
		}

		public void ExtractYawPitchRoll(out float yaw, out float pitch, out float roll)
		{
			yaw = (float)System.Math.Atan2(V02, V22);
			pitch = (float)System.Math.Asin(0f - V12);
			roll = (float)System.Math.Atan2(V10, V11);
		}

		public static Matrix3x3 CreateFromRows(Vector3 row0, Vector3 row1, Vector3 row2)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = row0.X;
			result.V01 = row0.Y;
			result.V02 = row0.Z;
			result.V10 = row1.X;
			result.V11 = row1.Y;
			result.V12 = row1.Z;
			result.V20 = row2.X;
			result.V21 = row2.Y;
			result.V22 = row2.Z;
			return result;
		}

		public static Matrix3x3 CreateFromColumns(Vector3 column0, Vector3 column1, Vector3 column2)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = column0.X;
			result.V10 = column0.Y;
			result.V20 = column0.Z;
			result.V01 = column1.X;
			result.V11 = column1.Y;
			result.V21 = column1.Z;
			result.V02 = column2.X;
			result.V12 = column2.Y;
			result.V22 = column2.Z;
			return result;
		}

		public static Matrix3x3 CreateDiagonal(Vector3 vector)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = vector.X;
			result.V11 = vector.Y;
			result.V22 = vector.Z;
			return result;
		}

		public Vector3 GetRow(int index)
		{
			switch (index)
			{
			default:
				throw new ArgumentException("Invalid row index was specified.", "index");
			case 2:
				return new Vector3(V20, V21, V22);
			case 1:
				return new Vector3(V10, V11, V12);
			case 0:
				return new Vector3(V00, V01, V02);
			}
		}

		public Vector3 GetColumn(int index)
		{
			switch (index)
			{
			default:
				throw new ArgumentException("Invalid column index was specified.", "index");
			case 2:
				return new Vector3(V02, V12, V22);
			case 1:
				return new Vector3(V01, V11, V21);
			case 0:
				return new Vector3(V00, V10, V20);
			}
		}

		public static Matrix3x3 operator *(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = matrix1.V00 * matrix2.V00 + matrix1.V01 * matrix2.V10 + matrix1.V02 * matrix2.V20;
			result.V01 = matrix1.V00 * matrix2.V01 + matrix1.V01 * matrix2.V11 + matrix1.V02 * matrix2.V21;
			result.V02 = matrix1.V00 * matrix2.V02 + matrix1.V01 * matrix2.V12 + matrix1.V02 * matrix2.V22;
			result.V10 = matrix1.V10 * matrix2.V00 + matrix1.V11 * matrix2.V10 + matrix1.V12 * matrix2.V20;
			result.V11 = matrix1.V10 * matrix2.V01 + matrix1.V11 * matrix2.V11 + matrix1.V12 * matrix2.V21;
			result.V12 = matrix1.V10 * matrix2.V02 + matrix1.V11 * matrix2.V12 + matrix1.V12 * matrix2.V22;
			result.V20 = matrix1.V20 * matrix2.V00 + matrix1.V21 * matrix2.V10 + matrix1.V22 * matrix2.V20;
			result.V21 = matrix1.V20 * matrix2.V01 + matrix1.V21 * matrix2.V11 + matrix1.V22 * matrix2.V21;
			result.V22 = matrix1.V20 * matrix2.V02 + matrix1.V21 * matrix2.V12 + matrix1.V22 * matrix2.V22;
			return result;
		}

		public static Matrix3x3 Multiply(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			return matrix1 * matrix2;
		}

		public static Matrix3x3 operator +(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = matrix1.V00 + matrix2.V00;
			result.V01 = matrix1.V01 + matrix2.V01;
			result.V02 = matrix1.V02 + matrix2.V02;
			result.V10 = matrix1.V10 + matrix2.V10;
			result.V11 = matrix1.V11 + matrix2.V11;
			result.V12 = matrix1.V12 + matrix2.V12;
			result.V20 = matrix1.V20 + matrix2.V20;
			result.V21 = matrix1.V21 + matrix2.V21;
			result.V22 = matrix1.V22 + matrix2.V22;
			return result;
		}

		public static Matrix3x3 Add(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			return matrix1 + matrix2;
		}

		public static Matrix3x3 operator -(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = matrix1.V00 - matrix2.V00;
			result.V01 = matrix1.V01 - matrix2.V01;
			result.V02 = matrix1.V02 - matrix2.V02;
			result.V10 = matrix1.V10 - matrix2.V10;
			result.V11 = matrix1.V11 - matrix2.V11;
			result.V12 = matrix1.V12 - matrix2.V12;
			result.V20 = matrix1.V20 - matrix2.V20;
			result.V21 = matrix1.V21 - matrix2.V21;
			result.V22 = matrix1.V22 - matrix2.V22;
			return result;
		}

		public static Matrix3x3 Subtract(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			return matrix1 - matrix2;
		}

		public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
		{
			return new Vector3(matrix.V00 * vector.X + matrix.V01 * vector.Y + matrix.V02 * vector.Z, matrix.V10 * vector.X + matrix.V11 * vector.Y + matrix.V12 * vector.Z, matrix.V20 * vector.X + matrix.V21 * vector.Y + matrix.V22 * vector.Z);
		}

		public static Vector3 Multiply(Matrix3x3 matrix, Vector3 vector)
		{
			return matrix * vector;
		}

		public static Matrix3x3 operator *(Matrix3x3 matrix, float factor)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = matrix.V00 * factor;
			result.V01 = matrix.V01 * factor;
			result.V02 = matrix.V02 * factor;
			result.V10 = matrix.V10 * factor;
			result.V11 = matrix.V11 * factor;
			result.V12 = matrix.V12 * factor;
			result.V20 = matrix.V20 * factor;
			result.V21 = matrix.V21 * factor;
			result.V22 = matrix.V22 * factor;
			return result;
		}

		public static Matrix3x3 Multiply(Matrix3x3 matrix, float factor)
		{
			return matrix * factor;
		}

		public static Matrix3x3 operator +(Matrix3x3 matrix, float value)
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = matrix.V00 + value;
			result.V01 = matrix.V01 + value;
			result.V02 = matrix.V02 + value;
			result.V10 = matrix.V10 + value;
			result.V11 = matrix.V11 + value;
			result.V12 = matrix.V12 + value;
			result.V20 = matrix.V20 + value;
			result.V21 = matrix.V21 + value;
			result.V22 = matrix.V22 + value;
			return result;
		}

		public static Matrix3x3 Add(Matrix3x3 matrix, float value)
		{
			return matrix + value;
		}

		public static bool operator ==(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			if (matrix1.V00 == matrix2.V00 && matrix1.V01 == matrix2.V01 && matrix1.V02 == matrix2.V02 && matrix1.V10 == matrix2.V10 && matrix1.V11 == matrix2.V11 && matrix1.V12 == matrix2.V12 && matrix1.V20 == matrix2.V20 && matrix1.V21 == matrix2.V21)
			{
				return matrix1.V22 == matrix2.V22;
			}
			return false;
		}

		public static bool operator !=(Matrix3x3 matrix1, Matrix3x3 matrix2)
		{
			if (matrix1.V00 == matrix2.V00 && matrix1.V01 == matrix2.V01 && matrix1.V02 == matrix2.V02 && matrix1.V10 == matrix2.V10 && matrix1.V11 == matrix2.V11 && matrix1.V12 == matrix2.V12 && matrix1.V20 == matrix2.V20 && matrix1.V21 == matrix2.V21)
			{
				return matrix1.V22 != matrix2.V22;
			}
			return true;
		}

		public bool Equals(Matrix3x3 matrix)
		{
			return this == matrix;
		}

		public override bool Equals(object obj)
		{
			if (obj is Matrix3x3)
			{
				return Equals((Matrix3x3)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return V00.GetHashCode() + V01.GetHashCode() + V02.GetHashCode() + V10.GetHashCode() + V11.GetHashCode() + V12.GetHashCode() + V20.GetHashCode() + V21.GetHashCode() + V22.GetHashCode();
		}

		public Matrix3x3 Transpose()
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = V00;
			result.V01 = V10;
			result.V02 = V20;
			result.V10 = V01;
			result.V11 = V11;
			result.V12 = V21;
			result.V20 = V02;
			result.V21 = V12;
			result.V22 = V22;
			return result;
		}

		public Matrix3x3 MultiplySelfByTranspose()
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = V00 * V00 + V01 * V01 + V02 * V02;
			result.V10 = (result.V01 = V00 * V10 + V01 * V11 + V02 * V12);
			result.V20 = (result.V02 = V00 * V20 + V01 * V21 + V02 * V22);
			result.V11 = V10 * V10 + V11 * V11 + V12 * V12;
			result.V21 = (result.V12 = V10 * V20 + V11 * V21 + V12 * V22);
			result.V22 = V20 * V20 + V21 * V21 + V22 * V22;
			return result;
		}

		public Matrix3x3 MultiplyTransposeBySelf()
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = V00 * V00 + V10 * V10 + V20 * V20;
			result.V10 = (result.V01 = V00 * V01 + V10 * V11 + V20 * V21);
			result.V20 = (result.V02 = V00 * V02 + V10 * V12 + V20 * V22);
			result.V11 = V01 * V01 + V11 * V11 + V21 * V21;
			result.V21 = (result.V12 = V01 * V02 + V11 * V12 + V21 * V22);
			result.V22 = V02 * V02 + V12 * V12 + V22 * V22;
			return result;
		}

		public Matrix3x3 Adjugate()
		{
			Matrix3x3 result = default(Matrix3x3);
			result.V00 = V11 * V22 - V12 * V21;
			result.V01 = 0f - (V01 * V22 - V02 * V21);
			result.V02 = V01 * V12 - V02 * V11;
			result.V10 = 0f - (V10 * V22 - V12 * V20);
			result.V11 = V00 * V22 - V02 * V20;
			result.V12 = 0f - (V00 * V12 - V02 * V10);
			result.V20 = V10 * V21 - V11 * V20;
			result.V21 = 0f - (V00 * V21 - V01 * V20);
			result.V22 = V00 * V11 - V01 * V10;
			return result;
		}

		public Matrix3x3 Inverse()
		{
			float determinant = Determinant;
			if (determinant == 0f)
			{
				throw new ArgumentException("Cannot calculate inverse of the matrix since it is singular.");
			}
			float num = 1f / determinant;
			Matrix3x3 result = Adjugate();
			result.V00 *= num;
			result.V01 *= num;
			result.V02 *= num;
			result.V10 *= num;
			result.V11 *= num;
			result.V12 *= num;
			result.V20 *= num;
			result.V21 *= num;
			result.V22 *= num;
			return result;
		}

		public Matrix3x3 PseudoInverse()
		{
			SVD(out Matrix3x3 u, out Vector3 e, out Matrix3x3 v);
			return v * CreateDiagonal(e.Inverse()) * u.Transpose();
		}

		public void SVD(out Matrix3x3 u, out Vector3 e, out Matrix3x3 v)
		{
			double[,] array = new double[3, 3]
			{
				{
					V00,
					V01,
					V02
				},
				{
					V10,
					V11,
					V12
				},
				{
					V20,
					V21,
					V22
				}
			};
			svd.svdcmp(array, out double[] w, out double[,] v2);
			u = default(Matrix3x3);
			u.V00 = (float)array[0, 0];
			u.V01 = (float)array[0, 1];
			u.V02 = (float)array[0, 2];
			u.V10 = (float)array[1, 0];
			u.V11 = (float)array[1, 1];
			u.V12 = (float)array[1, 2];
			u.V20 = (float)array[2, 0];
			u.V21 = (float)array[2, 1];
			u.V22 = (float)array[2, 2];
			v = default(Matrix3x3);
			v.V00 = (float)v2[0, 0];
			v.V01 = (float)v2[0, 1];
			v.V02 = (float)v2[0, 2];
			v.V10 = (float)v2[1, 0];
			v.V11 = (float)v2[1, 1];
			v.V12 = (float)v2[1, 2];
			v.V20 = (float)v2[2, 0];
			v.V21 = (float)v2[2, 1];
			v.V22 = (float)v2[2, 2];
			e = default(Vector3);
			e.X = (float)w[0];
			e.Y = (float)w[1];
			e.Z = (float)w[2];
		}
	}
}

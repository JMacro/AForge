using System;

namespace AForge.Math
{
	[Serializable]
	public struct Matrix4x4
	{
		public float V00;

		public float V01;

		public float V02;

		public float V03;

		public float V10;

		public float V11;

		public float V12;

		public float V13;

		public float V20;

		public float V21;

		public float V22;

		public float V23;

		public float V30;

		public float V31;

		public float V32;

		public float V33;

		public static Matrix4x4 Identity
		{
			get
			{
				Matrix4x4 result = default(Matrix4x4);
				result.V00 = (result.V11 = (result.V22 = (result.V33 = 1f)));
				return result;
			}
		}

		public float[] ToArray()
		{
			return new float[16]
			{
				V00,
				V01,
				V02,
				V03,
				V10,
				V11,
				V12,
				V13,
				V20,
				V21,
				V22,
				V23,
				V30,
				V31,
				V32,
				V33
			};
		}

		public static Matrix4x4 CreateRotationY(float radians)
		{
			Matrix4x4 identity = Identity;
			float v = (float)System.Math.Cos(radians);
			float num = (float)System.Math.Sin(radians);
			identity.V00 = (identity.V22 = v);
			identity.V02 = num;
			identity.V20 = 0f - num;
			return identity;
		}

		public static Matrix4x4 CreateRotationX(float radians)
		{
			Matrix4x4 identity = Identity;
			float v = (float)System.Math.Cos(radians);
			float num = (float)System.Math.Sin(radians);
			identity.V11 = (identity.V22 = v);
			identity.V12 = 0f - num;
			identity.V21 = num;
			return identity;
		}

		public static Matrix4x4 CreateRotationZ(float radians)
		{
			Matrix4x4 identity = Identity;
			float v = (float)System.Math.Cos(radians);
			float num = (float)System.Math.Sin(radians);
			identity.V00 = (identity.V11 = v);
			identity.V01 = 0f - num;
			identity.V10 = num;
			return identity;
		}

		public static Matrix4x4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			return CreateRotationY(yaw) * CreateRotationX(pitch) * CreateRotationZ(roll);
		}

		public void ExtractYawPitchRoll(out float yaw, out float pitch, out float roll)
		{
			yaw = (float)System.Math.Atan2(V02, V22);
			pitch = (float)System.Math.Asin(0f - V12);
			roll = (float)System.Math.Atan2(V10, V11);
		}

		public static Matrix4x4 CreateFromRotation(Matrix3x3 rotationMatrix)
		{
			Matrix4x4 identity = Identity;
			identity.V00 = rotationMatrix.V00;
			identity.V01 = rotationMatrix.V01;
			identity.V02 = rotationMatrix.V02;
			identity.V10 = rotationMatrix.V10;
			identity.V11 = rotationMatrix.V11;
			identity.V12 = rotationMatrix.V12;
			identity.V20 = rotationMatrix.V20;
			identity.V21 = rotationMatrix.V21;
			identity.V22 = rotationMatrix.V22;
			return identity;
		}

		public static Matrix4x4 CreateTranslation(Vector3 position)
		{
			Matrix4x4 identity = Identity;
			identity.V03 = position.X;
			identity.V13 = position.Y;
			identity.V23 = position.Z;
			return identity;
		}

		public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget)
		{
			Matrix4x4 result = default(Matrix4x4);
			Vector3 vector = cameraPosition - cameraTarget;
			vector.Normalize();
			Vector3 vector2 = Vector3.Cross(new Vector3(0f, 1f, 0f), vector);
			vector2.Normalize();
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			result.V00 = vector2.X;
			result.V01 = vector2.Y;
			result.V02 = vector2.Z;
			result.V10 = vector3.X;
			result.V11 = vector3.Y;
			result.V12 = vector3.Z;
			result.V20 = vector.X;
			result.V21 = vector.Y;
			result.V22 = vector.Z;
			result.V03 = 0f - Vector3.Dot(cameraPosition, vector2);
			result.V13 = 0f - Vector3.Dot(cameraPosition, vector3);
			result.V23 = 0f - Vector3.Dot(cameraPosition, vector);
			result.V33 = 1f;
			return result;
		}

		public static Matrix4x4 CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			if (nearPlaneDistance <= 0f || farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("Both near and far view planes' distances must be greater than zero.");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("Near plane must be closer than the far plane.");
			}
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = 2f * nearPlaneDistance / width;
			result.V11 = 2f * nearPlaneDistance / height;
			result.V22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			result.V32 = -1f;
			result.V23 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
			return result;
		}

		public static Matrix4x4 CreateFromRows(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = row0.X;
			result.V01 = row0.Y;
			result.V02 = row0.Z;
			result.V03 = row0.W;
			result.V10 = row1.X;
			result.V11 = row1.Y;
			result.V12 = row1.Z;
			result.V13 = row1.W;
			result.V20 = row2.X;
			result.V21 = row2.Y;
			result.V22 = row2.Z;
			result.V23 = row2.W;
			result.V30 = row3.X;
			result.V31 = row3.Y;
			result.V32 = row3.Z;
			result.V33 = row3.W;
			return result;
		}

		public static Matrix4x4 CreateFromColumns(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = column0.X;
			result.V10 = column0.Y;
			result.V20 = column0.Z;
			result.V30 = column0.W;
			result.V01 = column1.X;
			result.V11 = column1.Y;
			result.V21 = column1.Z;
			result.V31 = column1.W;
			result.V02 = column2.X;
			result.V12 = column2.Y;
			result.V22 = column2.Z;
			result.V32 = column2.W;
			result.V03 = column3.X;
			result.V13 = column3.Y;
			result.V23 = column3.Z;
			result.V33 = column3.W;
			return result;
		}

		public static Matrix4x4 CreateDiagonal(Vector4 vector)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = vector.X;
			result.V11 = vector.Y;
			result.V22 = vector.Z;
			result.V33 = vector.W;
			return result;
		}

		public Vector4 GetRow(int index)
		{
			switch (index)
			{
			default:
				throw new ArgumentException("Invalid row index was specified.", "index");
			case 3:
				return new Vector4(V30, V31, V32, V33);
			case 2:
				return new Vector4(V20, V21, V22, V23);
			case 1:
				return new Vector4(V10, V11, V12, V13);
			case 0:
				return new Vector4(V00, V01, V02, V03);
			}
		}

		public Vector4 GetColumn(int index)
		{
			switch (index)
			{
			default:
				throw new ArgumentException("Invalid column index was specified.", "index");
			case 3:
				return new Vector4(V03, V13, V23, V33);
			case 2:
				return new Vector4(V02, V12, V22, V32);
			case 1:
				return new Vector4(V01, V11, V21, V31);
			case 0:
				return new Vector4(V00, V10, V20, V30);
			}
		}

		public static Matrix4x4 operator *(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = matrix1.V00 * matrix2.V00 + matrix1.V01 * matrix2.V10 + matrix1.V02 * matrix2.V20 + matrix1.V03 * matrix2.V30;
			result.V01 = matrix1.V00 * matrix2.V01 + matrix1.V01 * matrix2.V11 + matrix1.V02 * matrix2.V21 + matrix1.V03 * matrix2.V31;
			result.V02 = matrix1.V00 * matrix2.V02 + matrix1.V01 * matrix2.V12 + matrix1.V02 * matrix2.V22 + matrix1.V03 * matrix2.V32;
			result.V03 = matrix1.V00 * matrix2.V03 + matrix1.V01 * matrix2.V13 + matrix1.V02 * matrix2.V23 + matrix1.V03 * matrix2.V33;
			result.V10 = matrix1.V10 * matrix2.V00 + matrix1.V11 * matrix2.V10 + matrix1.V12 * matrix2.V20 + matrix1.V13 * matrix2.V30;
			result.V11 = matrix1.V10 * matrix2.V01 + matrix1.V11 * matrix2.V11 + matrix1.V12 * matrix2.V21 + matrix1.V13 * matrix2.V31;
			result.V12 = matrix1.V10 * matrix2.V02 + matrix1.V11 * matrix2.V12 + matrix1.V12 * matrix2.V22 + matrix1.V13 * matrix2.V32;
			result.V13 = matrix1.V10 * matrix2.V03 + matrix1.V11 * matrix2.V13 + matrix1.V12 * matrix2.V23 + matrix1.V13 * matrix2.V33;
			result.V20 = matrix1.V20 * matrix2.V00 + matrix1.V21 * matrix2.V10 + matrix1.V22 * matrix2.V20 + matrix1.V23 * matrix2.V30;
			result.V21 = matrix1.V20 * matrix2.V01 + matrix1.V21 * matrix2.V11 + matrix1.V22 * matrix2.V21 + matrix1.V23 * matrix2.V31;
			result.V22 = matrix1.V20 * matrix2.V02 + matrix1.V21 * matrix2.V12 + matrix1.V22 * matrix2.V22 + matrix1.V23 * matrix2.V32;
			result.V23 = matrix1.V20 * matrix2.V03 + matrix1.V21 * matrix2.V13 + matrix1.V22 * matrix2.V23 + matrix1.V23 * matrix2.V33;
			result.V30 = matrix1.V30 * matrix2.V00 + matrix1.V31 * matrix2.V10 + matrix1.V32 * matrix2.V20 + matrix1.V33 * matrix2.V30;
			result.V31 = matrix1.V30 * matrix2.V01 + matrix1.V31 * matrix2.V11 + matrix1.V32 * matrix2.V21 + matrix1.V33 * matrix2.V31;
			result.V32 = matrix1.V30 * matrix2.V02 + matrix1.V31 * matrix2.V12 + matrix1.V32 * matrix2.V22 + matrix1.V33 * matrix2.V32;
			result.V33 = matrix1.V30 * matrix2.V03 + matrix1.V31 * matrix2.V13 + matrix1.V32 * matrix2.V23 + matrix1.V33 * matrix2.V33;
			return result;
		}

		public static Matrix4x4 Multiply(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			return matrix1 * matrix2;
		}

		public static Matrix4x4 operator +(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = matrix1.V00 + matrix2.V00;
			result.V01 = matrix1.V01 + matrix2.V01;
			result.V02 = matrix1.V02 + matrix2.V02;
			result.V03 = matrix1.V03 + matrix2.V03;
			result.V10 = matrix1.V10 + matrix2.V10;
			result.V11 = matrix1.V11 + matrix2.V11;
			result.V12 = matrix1.V12 + matrix2.V12;
			result.V13 = matrix1.V13 + matrix2.V13;
			result.V20 = matrix1.V20 + matrix2.V20;
			result.V21 = matrix1.V21 + matrix2.V21;
			result.V22 = matrix1.V22 + matrix2.V22;
			result.V23 = matrix1.V23 + matrix2.V23;
			result.V30 = matrix1.V30 + matrix2.V30;
			result.V31 = matrix1.V31 + matrix2.V31;
			result.V32 = matrix1.V32 + matrix2.V32;
			result.V33 = matrix1.V33 + matrix2.V33;
			return result;
		}

		public static Matrix4x4 Add(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			return matrix1 + matrix2;
		}

		public static Matrix4x4 operator -(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			Matrix4x4 result = default(Matrix4x4);
			result.V00 = matrix1.V00 - matrix2.V00;
			result.V01 = matrix1.V01 - matrix2.V01;
			result.V02 = matrix1.V02 - matrix2.V02;
			result.V03 = matrix1.V03 - matrix2.V03;
			result.V10 = matrix1.V10 - matrix2.V10;
			result.V11 = matrix1.V11 - matrix2.V11;
			result.V12 = matrix1.V12 - matrix2.V12;
			result.V13 = matrix1.V13 - matrix2.V13;
			result.V20 = matrix1.V20 - matrix2.V20;
			result.V21 = matrix1.V21 - matrix2.V21;
			result.V22 = matrix1.V22 - matrix2.V22;
			result.V23 = matrix1.V23 - matrix2.V23;
			result.V30 = matrix1.V30 - matrix2.V30;
			result.V31 = matrix1.V31 - matrix2.V31;
			result.V32 = matrix1.V32 - matrix2.V32;
			result.V33 = matrix1.V33 - matrix2.V33;
			return result;
		}

		public static Matrix4x4 Subtract(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			return matrix1 - matrix2;
		}

		public static Vector4 operator *(Matrix4x4 matrix, Vector4 vector)
		{
			return new Vector4(matrix.V00 * vector.X + matrix.V01 * vector.Y + matrix.V02 * vector.Z + matrix.V03 * vector.W, matrix.V10 * vector.X + matrix.V11 * vector.Y + matrix.V12 * vector.Z + matrix.V13 * vector.W, matrix.V20 * vector.X + matrix.V21 * vector.Y + matrix.V22 * vector.Z + matrix.V23 * vector.W, matrix.V30 * vector.X + matrix.V31 * vector.Y + matrix.V32 * vector.Z + matrix.V33 * vector.W);
		}

		public static Vector4 Multiply(Matrix4x4 matrix, Vector4 vector)
		{
			return matrix * vector;
		}

		public static bool operator ==(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			if (matrix1.V00 == matrix2.V00 && matrix1.V01 == matrix2.V01 && matrix1.V02 == matrix2.V02 && matrix1.V03 == matrix2.V03 && matrix1.V10 == matrix2.V10 && matrix1.V11 == matrix2.V11 && matrix1.V12 == matrix2.V12 && matrix1.V13 == matrix2.V13 && matrix1.V20 == matrix2.V20 && matrix1.V21 == matrix2.V21 && matrix1.V22 == matrix2.V22 && matrix1.V23 == matrix2.V23 && matrix1.V30 == matrix2.V30 && matrix1.V31 == matrix2.V31 && matrix1.V32 == matrix2.V32)
			{
				return matrix1.V33 == matrix2.V33;
			}
			return false;
		}

		public static bool operator !=(Matrix4x4 matrix1, Matrix4x4 matrix2)
		{
			if (matrix1.V00 == matrix2.V00 && matrix1.V01 == matrix2.V01 && matrix1.V02 == matrix2.V02 && matrix1.V03 == matrix2.V03 && matrix1.V10 == matrix2.V10 && matrix1.V11 == matrix2.V11 && matrix1.V12 == matrix2.V12 && matrix1.V13 == matrix2.V13 && matrix1.V20 == matrix2.V20 && matrix1.V21 == matrix2.V21 && matrix1.V22 == matrix2.V22 && matrix1.V23 == matrix2.V23 && matrix1.V30 == matrix2.V30 && matrix1.V31 == matrix2.V31 && matrix1.V32 == matrix2.V32)
			{
				return matrix1.V33 != matrix2.V33;
			}
			return true;
		}

		public bool Equals(Matrix4x4 matrix)
		{
			return this == matrix;
		}

		public override bool Equals(object obj)
		{
			if (obj is Matrix4x4)
			{
				return Equals((Matrix4x4)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return V00.GetHashCode() + V01.GetHashCode() + V02.GetHashCode() + V03.GetHashCode() + V10.GetHashCode() + V11.GetHashCode() + V12.GetHashCode() + V13.GetHashCode() + V20.GetHashCode() + V21.GetHashCode() + V22.GetHashCode() + V23.GetHashCode() + V30.GetHashCode() + V31.GetHashCode() + V32.GetHashCode() + V33.GetHashCode();
		}
	}
}

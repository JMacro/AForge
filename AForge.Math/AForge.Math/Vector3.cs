using System;
using System.Globalization;

namespace AForge.Math
{
	[Serializable]
	public struct Vector3
	{
		public float X;

		public float Y;

		public float Z;

		public float Max
		{
			get
			{
				if (!(X > Y))
				{
					if (!(Y > Z))
					{
						return Z;
					}
					return Y;
				}
				if (!(X > Z))
				{
					return Z;
				}
				return X;
			}
		}

		public float Min
		{
			get
			{
				if (!(X < Y))
				{
					if (!(Y < Z))
					{
						return Z;
					}
					return Y;
				}
				if (!(X < Z))
				{
					return Z;
				}
				return X;
			}
		}

		public int MaxIndex
		{
			get
			{
				if (!(X >= Y))
				{
					if (!(Y >= Z))
					{
						return 2;
					}
					return 1;
				}
				if (!(X >= Z))
				{
					return 2;
				}
				return 0;
			}
		}

		public int MinIndex
		{
			get
			{
				if (!(X <= Y))
				{
					if (!(Y <= Z))
					{
						return 2;
					}
					return 1;
				}
				if (!(X <= Z))
				{
					return 2;
				}
				return 0;
			}
		}

		public float Norm => (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);

		public float Square => X * X + Y * Y + Z * Z;

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(float value)
		{
			X = (Y = (Z = value));
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", X, Y, Z);
		}

		public float[] ToArray()
		{
			return new float[3]
			{
				X,
				Y,
				Z
			};
		}

		public static Vector3 operator +(Vector3 vector1, Vector3 vector2)
		{
			return new Vector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
		}

		public static Vector3 Add(Vector3 vector1, Vector3 vector2)
		{
			return vector1 + vector2;
		}

		public static Vector3 operator +(Vector3 vector, float value)
		{
			return new Vector3(vector.X + value, vector.Y + value, vector.Z + value);
		}

		public static Vector3 Add(Vector3 vector, float value)
		{
			return vector + value;
		}

		public static Vector3 operator -(Vector3 vector1, Vector3 vector2)
		{
			return new Vector3(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
		}

		public static Vector3 Subtract(Vector3 vector1, Vector3 vector2)
		{
			return vector1 - vector2;
		}

		public static Vector3 operator -(Vector3 vector, float value)
		{
			return new Vector3(vector.X - value, vector.Y - value, vector.Z - value);
		}

		public static Vector3 Subtract(Vector3 vector, float value)
		{
			return vector - value;
		}

		public static Vector3 operator *(Vector3 vector1, Vector3 vector2)
		{
			return new Vector3(vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z);
		}

		public static Vector3 Multiply(Vector3 vector1, Vector3 vector2)
		{
			return vector1 * vector2;
		}

		public static Vector3 operator *(Vector3 vector, float factor)
		{
			return new Vector3(vector.X * factor, vector.Y * factor, vector.Z * factor);
		}

		public static Vector3 Multiply(Vector3 vector, float factor)
		{
			return vector * factor;
		}

		public static Vector3 operator /(Vector3 vector1, Vector3 vector2)
		{
			return new Vector3(vector1.X / vector2.X, vector1.Y / vector2.Y, vector1.Z / vector2.Z);
		}

		public static Vector3 Divide(Vector3 vector1, Vector3 vector2)
		{
			return vector1 / vector2;
		}

		public static Vector3 operator /(Vector3 vector, float factor)
		{
			return new Vector3(vector.X / factor, vector.Y / factor, vector.Z / factor);
		}

		public static Vector3 Divide(Vector3 vector, float factor)
		{
			return vector / factor;
		}

		public static bool operator ==(Vector3 vector1, Vector3 vector2)
		{
			if (vector1.X == vector2.X && vector1.Y == vector2.Y)
			{
				return vector1.Z == vector2.Z;
			}
			return false;
		}

		public static bool operator !=(Vector3 vector1, Vector3 vector2)
		{
			if (vector1.X == vector2.X && vector1.Y == vector2.Y)
			{
				return vector1.Z != vector2.Z;
			}
			return true;
		}

		public bool Equals(Vector3 vector)
		{
			if (vector.X == X && vector.Y == Y)
			{
				return vector.Z == Z;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector3)
			{
				return Equals((Vector3)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
		}

		public float Normalize()
		{
			float num = (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
			float num2 = 1f / num;
			X *= num2;
			Y *= num2;
			Z *= num2;
			return num;
		}

		public Vector3 Inverse()
		{
			return new Vector3((X == 0f) ? 0f : (1f / X), (Y == 0f) ? 0f : (1f / Y), (Z == 0f) ? 0f : (1f / Z));
		}

		public Vector3 Abs()
		{
			return new Vector3(System.Math.Abs(X), System.Math.Abs(Y), System.Math.Abs(Z));
		}

		public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
		{
			return new Vector3(vector1.Y * vector2.Z - vector1.Z * vector2.Y, vector1.Z * vector2.X - vector1.X * vector2.Z, vector1.X * vector2.Y - vector1.Y * vector2.X);
		}

		public static float Dot(Vector3 vector1, Vector3 vector2)
		{
			return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
		}

		public Vector4 ToVector4()
		{
			return new Vector4(X, Y, Z, 1f);
		}
	}
}

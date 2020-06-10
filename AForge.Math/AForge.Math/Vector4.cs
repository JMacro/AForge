using System;
using System.Globalization;

namespace AForge.Math
{
	[Serializable]
	public struct Vector4
	{
		public float X;

		public float Y;

		public float Z;

		public float W;

		public float Max
		{
			get
			{
				float num = (X > Y) ? X : Y;
				float num2 = (Z > W) ? Z : W;
				if (!(num > num2))
				{
					return num2;
				}
				return num;
			}
		}

		public float Min
		{
			get
			{
				float num = (X < Y) ? X : Y;
				float num2 = (Z < W) ? Z : W;
				if (!(num < num2))
				{
					return num2;
				}
				return num;
			}
		}

		public int MaxIndex
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				int num3 = 0;
				int num4 = 0;
				if (X >= Y)
				{
					num = X;
					num3 = 0;
				}
				else
				{
					num = Y;
					num3 = 1;
				}
				if (Z >= W)
				{
					num2 = Z;
					num4 = 2;
				}
				else
				{
					num2 = W;
					num4 = 3;
				}
				if (!(num >= num2))
				{
					return num4;
				}
				return num3;
			}
		}

		public int MinIndex
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				int num3 = 0;
				int num4 = 0;
				if (X <= Y)
				{
					num = X;
					num3 = 0;
				}
				else
				{
					num = Y;
					num3 = 1;
				}
				if (Z <= W)
				{
					num2 = Z;
					num4 = 2;
				}
				else
				{
					num2 = W;
					num4 = 3;
				}
				if (!(num <= num2))
				{
					return num4;
				}
				return num3;
			}
		}

		public float Norm => (float)System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

		public float Square => X * X + Y * Y + Z * Z + W * W;

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(float value)
		{
			X = (Y = (Z = (W = value)));
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", X, Y, Z, W);
		}

		public float[] ToArray()
		{
			return new float[4]
			{
				X,
				Y,
				Z,
				W
			};
		}

		public static Vector4 operator +(Vector4 vector1, Vector4 vector2)
		{
			return new Vector4(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z, vector1.W + vector2.W);
		}

		public static Vector4 Add(Vector4 vector1, Vector4 vector2)
		{
			return vector1 + vector2;
		}

		public static Vector4 operator +(Vector4 vector, float value)
		{
			return new Vector4(vector.X + value, vector.Y + value, vector.Z + value, vector.W + value);
		}

		public static Vector4 Add(Vector4 vector, float value)
		{
			return vector + value;
		}

		public static Vector4 operator -(Vector4 vector1, Vector4 vector2)
		{
			return new Vector4(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z, vector1.W - vector2.W);
		}

		public static Vector4 Subtract(Vector4 vector1, Vector4 vector2)
		{
			return vector1 - vector2;
		}

		public static Vector4 operator -(Vector4 vector, float value)
		{
			return new Vector4(vector.X - value, vector.Y - value, vector.Z - value, vector.W - value);
		}

		public static Vector4 Subtract(Vector4 vector, float value)
		{
			return vector - value;
		}

		public static Vector4 operator *(Vector4 vector1, Vector4 vector2)
		{
			return new Vector4(vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z, vector1.W * vector2.W);
		}

		public static Vector4 Multiply(Vector4 vector1, Vector4 vector2)
		{
			return vector1 * vector2;
		}

		public static Vector4 operator *(Vector4 vector, float factor)
		{
			return new Vector4(vector.X * factor, vector.Y * factor, vector.Z * factor, vector.W * factor);
		}

		public static Vector4 Multiply(Vector4 vector, float factor)
		{
			return vector * factor;
		}

		public static Vector4 operator /(Vector4 vector1, Vector4 vector2)
		{
			return new Vector4(vector1.X / vector2.X, vector1.Y / vector2.Y, vector1.Z / vector2.Z, vector1.W / vector2.W);
		}

		public static Vector4 Divide(Vector4 vector1, Vector4 vector2)
		{
			return vector1 / vector2;
		}

		public static Vector4 operator /(Vector4 vector, float factor)
		{
			return new Vector4(vector.X / factor, vector.Y / factor, vector.Z / factor, vector.W / factor);
		}

		public static Vector4 Divide(Vector4 vector, float factor)
		{
			return vector / factor;
		}

		public static bool operator ==(Vector4 vector1, Vector4 vector2)
		{
			if (vector1.X == vector2.X && vector1.Y == vector2.Y && vector1.Z == vector2.Z)
			{
				return vector1.W == vector2.W;
			}
			return false;
		}

		public static bool operator !=(Vector4 vector1, Vector4 vector2)
		{
			if (vector1.X == vector2.X && vector1.Y == vector2.Y && vector1.Z == vector2.Z)
			{
				return vector1.W != vector2.W;
			}
			return true;
		}

		public bool Equals(Vector4 vector)
		{
			if (vector.X == X && vector.Y == Y && vector.Z == Z)
			{
				return vector.W == W;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector4)
			{
				return Equals((Vector4)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
		}

		public float Normalize()
		{
			float num = (float)System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
			float num2 = 1f / num;
			X *= num2;
			Y *= num2;
			Z *= num2;
			W *= num2;
			return num;
		}

		public Vector4 Inverse()
		{
			return new Vector4((X == 0f) ? 0f : (1f / X), (Y == 0f) ? 0f : (1f / Y), (Z == 0f) ? 0f : (1f / Z), (W == 0f) ? 0f : (1f / W));
		}

		public Vector4 Abs()
		{
			return new Vector4(System.Math.Abs(X), System.Math.Abs(Y), System.Math.Abs(Z), System.Math.Abs(W));
		}

		public static float Dot(Vector4 vector1, Vector4 vector2)
		{
			return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z + vector1.W * vector2.W;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(X / W, Y / W, Z / W);
		}
	}
}

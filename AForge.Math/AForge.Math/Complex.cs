using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace AForge.Math
{
	public struct Complex : ICloneable, ISerializable
	{
		public double Re;

		public double Im;

		public static readonly Complex Zero = new Complex(0.0, 0.0);

		public static readonly Complex One = new Complex(1.0, 0.0);

		public static readonly Complex I = new Complex(0.0, 1.0);

		public double Magnitude => System.Math.Sqrt(Re * Re + Im * Im);

		public double Phase => System.Math.Atan2(Im, Re);

		public double SquaredMagnitude => Re * Re + Im * Im;

		public Complex(double re, double im)
		{
			Re = re;
			Im = im;
		}

		public Complex(Complex c)
		{
			Re = c.Re;
			Im = c.Im;
		}

		public static Complex Add(Complex a, Complex b)
		{
			return new Complex(a.Re + b.Re, a.Im + b.Im);
		}

		public static Complex Add(Complex a, double s)
		{
			return new Complex(a.Re + s, a.Im);
		}

		public static void Add(Complex a, Complex b, ref Complex result)
		{
			result.Re = a.Re + b.Re;
			result.Im = a.Im + b.Im;
		}

		public static void Add(Complex a, double s, ref Complex result)
		{
			result.Re = a.Re + s;
			result.Im = a.Im;
		}

		public static Complex Subtract(Complex a, Complex b)
		{
			return new Complex(a.Re - b.Re, a.Im - b.Im);
		}

		public static Complex Subtract(Complex a, double s)
		{
			return new Complex(a.Re - s, a.Im);
		}

		public static Complex Subtract(double s, Complex a)
		{
			return new Complex(s - a.Re, a.Im);
		}

		public static void Subtract(Complex a, Complex b, ref Complex result)
		{
			result.Re = a.Re - b.Re;
			result.Im = a.Im - b.Im;
		}

		public static void Subtract(Complex a, double s, ref Complex result)
		{
			result.Re = a.Re - s;
			result.Im = a.Im;
		}

		public static void Subtract(double s, Complex a, ref Complex result)
		{
			result.Re = s - a.Re;
			result.Im = a.Im;
		}

		public static Complex Multiply(Complex a, Complex b)
		{
			double re = a.Re;
			double im = a.Im;
			double re2 = b.Re;
			double im2 = b.Im;
			return new Complex(re * re2 - im * im2, re * im2 + im * re2);
		}

		public static Complex Multiply(Complex a, double s)
		{
			return new Complex(a.Re * s, a.Im * s);
		}

		public static void Multiply(Complex a, Complex b, ref Complex result)
		{
			double re = a.Re;
			double im = a.Im;
			double re2 = b.Re;
			double im2 = b.Im;
			result.Re = re * re2 - im * im2;
			result.Im = re * im2 + im * re2;
		}

		public static void Multiply(Complex a, double s, ref Complex result)
		{
			result.Re = a.Re * s;
			result.Im = a.Im * s;
		}

		public static Complex Divide(Complex a, Complex b)
		{
			double re = a.Re;
			double im = a.Im;
			double re2 = b.Re;
			double im2 = b.Im;
			double num = re2 * re2 + im2 * im2;
			if (num == 0.0)
			{
				throw new DivideByZeroException("Can not divide by zero.");
			}
			double num2 = 1.0 / num;
			return new Complex((re * re2 + im * im2) * num2, (im * re2 - re * im2) * num2);
		}

		public static Complex Divide(Complex a, double s)
		{
			if (s == 0.0)
			{
				throw new DivideByZeroException("Can not divide by zero.");
			}
			return new Complex(a.Re / s, a.Im / s);
		}

		public static Complex Divide(double s, Complex a)
		{
			if (a.Re == 0.0 || a.Im == 0.0)
			{
				throw new DivideByZeroException("Can not divide by zero.");
			}
			return new Complex(s / a.Re, s / a.Im);
		}

		public static void Divide(Complex a, Complex b, ref Complex result)
		{
			double re = a.Re;
			double im = a.Im;
			double re2 = b.Re;
			double im2 = b.Im;
			double num = re2 * re2 + im2 * im2;
			if (num == 0.0)
			{
				throw new DivideByZeroException("Can not divide by zero.");
			}
			double num2 = 1.0 / num;
			result.Re = (re * re2 + im * im2) * num2;
			result.Im = (im * re2 - re * im2) * num2;
		}

		public static void Divide(Complex a, double s, ref Complex result)
		{
			if (s == 0.0)
			{
				throw new DivideByZeroException("Can not divide by zero.");
			}
			result.Re = a.Re / s;
			result.Im = a.Im / s;
		}

		public static void Divide(double s, Complex a, ref Complex result)
		{
			if (a.Re == 0.0 || a.Im == 0.0)
			{
				throw new DivideByZeroException("Can not divide by zero.");
			}
			result.Re = s / a.Re;
			result.Im = s / a.Im;
		}

		public static Complex Negate(Complex a)
		{
			return new Complex(0.0 - a.Re, 0.0 - a.Im);
		}

		public static bool ApproxEqual(Complex a, Complex b)
		{
			return ApproxEqual(a, b, 8.8817841970012523E-16);
		}

		public static bool ApproxEqual(Complex a, Complex b, double tolerance)
		{
			if (System.Math.Abs(a.Re - b.Re) <= tolerance)
			{
				return System.Math.Abs(a.Im - b.Im) <= tolerance;
			}
			return false;
		}

		public static Complex Parse(string s)
		{
			Regex regex = new Regex("\\((?<real>.*),(?<imaginary>.*)\\)", RegexOptions.None);
			Match match = regex.Match(s);
			if (match.Success)
			{
				return new Complex(double.Parse(match.Result("${real}")), double.Parse(match.Result("${imaginary}")));
			}
			throw new FormatException("String representation of the complex number is not correctly formatted.");
		}

		public static bool TryParse(string s, out Complex result)
		{
			try
			{
				Complex complex = result = Parse(s);
				return true;
			}
			catch (FormatException)
			{
				result = default(Complex);
				return false;
			}
		}

		public static Complex Sqrt(Complex a)
		{
			Complex zero = Zero;
			if (a.Re == 0.0 && a.Im == 0.0)
			{
				return zero;
			}
			if (a.Im == 0.0)
			{
				zero.Re = ((a.Re > 0.0) ? System.Math.Sqrt(a.Re) : System.Math.Sqrt(0.0 - a.Re));
				zero.Im = 0.0;
			}
			else
			{
				double magnitude = a.Magnitude;
				zero.Re = System.Math.Sqrt(0.5 * (magnitude + a.Re));
				zero.Im = System.Math.Sqrt(0.5 * (magnitude - a.Re));
				if (a.Im < 0.0)
				{
					zero.Im = 0.0 - zero.Im;
				}
			}
			return zero;
		}

		public static Complex Log(Complex a)
		{
			Complex zero = Zero;
			if (a.Re > 0.0 && a.Im == 0.0)
			{
				zero.Re = System.Math.Log(a.Re);
				zero.Im = 0.0;
			}
			else if (a.Re == 0.0)
			{
				if (a.Im > 0.0)
				{
					zero.Re = System.Math.Log(a.Im);
					zero.Im = System.Math.PI / 2.0;
				}
				else
				{
					zero.Re = System.Math.Log(0.0 - a.Im);
					zero.Im = -System.Math.PI / 2.0;
				}
			}
			else
			{
				zero.Re = System.Math.Log(a.Magnitude);
				zero.Im = System.Math.Atan2(a.Im, a.Re);
			}
			return zero;
		}

		public static Complex Exp(Complex a)
		{
			Complex zero = Zero;
			double num = System.Math.Exp(a.Re);
			zero.Re = num * System.Math.Cos(a.Im);
			zero.Im = num * System.Math.Sin(a.Im);
			return zero;
		}

		public static Complex Sin(Complex a)
		{
			Complex zero = Zero;
			if (a.Im == 0.0)
			{
				zero.Re = System.Math.Sin(a.Re);
				zero.Im = 0.0;
			}
			else
			{
				zero.Re = System.Math.Sin(a.Re) * System.Math.Cosh(a.Im);
				zero.Im = System.Math.Cos(a.Re) * System.Math.Sinh(a.Im);
			}
			return zero;
		}

		public static Complex Cos(Complex a)
		{
			Complex zero = Zero;
			if (a.Im == 0.0)
			{
				zero.Re = System.Math.Cos(a.Re);
				zero.Im = 0.0;
			}
			else
			{
				zero.Re = System.Math.Cos(a.Re) * System.Math.Cosh(a.Im);
				zero.Im = (0.0 - System.Math.Sin(a.Re)) * System.Math.Sinh(a.Im);
			}
			return zero;
		}

		public static Complex Tan(Complex a)
		{
			Complex zero = Zero;
			if (a.Im == 0.0)
			{
				zero.Re = System.Math.Tan(a.Re);
				zero.Im = 0.0;
			}
			else
			{
				double num = 2.0 * a.Re;
				double value = 2.0 * a.Im;
				double num2 = System.Math.Cos(num) + System.Math.Cosh(num);
				zero.Re = System.Math.Sin(num) / num2;
				zero.Im = System.Math.Sinh(value) / num2;
			}
			return zero;
		}

		public override int GetHashCode()
		{
			return Re.GetHashCode() ^ Im.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Complex))
			{
				return false;
			}
			return this == (Complex)obj;
		}

		public override string ToString()
		{
			return $"({Re}, {Im})";
		}

		public static bool operator ==(Complex u, Complex v)
		{
			if (u.Re == v.Re)
			{
				return u.Im == v.Im;
			}
			return false;
		}

		public static bool operator !=(Complex u, Complex v)
		{
			return !(u == v);
		}

		public static Complex operator -(Complex a)
		{
			return Negate(a);
		}

		public static Complex operator +(Complex a, Complex b)
		{
			return Add(a, b);
		}

		public static Complex operator +(Complex a, double s)
		{
			return Add(a, s);
		}

		public static Complex operator +(double s, Complex a)
		{
			return Add(a, s);
		}

		public static Complex operator -(Complex a, Complex b)
		{
			return Subtract(a, b);
		}

		public static Complex operator -(Complex a, double s)
		{
			return Subtract(a, s);
		}

		public static Complex operator -(double s, Complex a)
		{
			return Subtract(s, a);
		}

		public static Complex operator *(Complex a, Complex b)
		{
			return Multiply(a, b);
		}

		public static Complex operator *(double s, Complex a)
		{
			return Multiply(a, s);
		}

		public static Complex operator *(Complex a, double s)
		{
			return Multiply(a, s);
		}

		public static Complex operator /(Complex a, Complex b)
		{
			return Divide(a, b);
		}

		public static Complex operator /(Complex a, double s)
		{
			return Divide(a, s);
		}

		public static Complex operator /(double s, Complex a)
		{
			return Divide(s, a);
		}

		public static explicit operator Complex(float value)
		{
			return new Complex(value, 0.0);
		}

		public static explicit operator Complex(double value)
		{
			return new Complex(value, 0.0);
		}

		object ICloneable.Clone()
		{
			return new Complex(this);
		}

		public Complex Clone()
		{
			return new Complex(this);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Real", Re);
			info.AddValue("Imaginary", Im);
		}
	}
}

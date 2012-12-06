using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MHGameWork.TheWizards.CG.Math
{
    [Serializable]
    public struct Color4
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public Color4(int argb)
        {
            this.Alpha = (float)(argb >> 24 & (int)byte.MaxValue) / (float)byte.MaxValue;
            this.Red = (float)(argb >> 16 & (int)byte.MaxValue) / (float)byte.MaxValue;
            this.Green = (float)(argb >> 8 & (int)byte.MaxValue) / (float)byte.MaxValue;
            this.Blue = (float)(argb & (int)byte.MaxValue) / (float)byte.MaxValue;
        }

        public Color4(Vector4 color)
        {
            this.Alpha = color.W;
            this.Red = color.X;
            this.Green = color.Y;
            this.Blue = color.Z;
        }

        public Color4(Vector3 color)
        {
            this.Alpha = 1f;
            this.Red = color.X;
            this.Green = color.Y;
            this.Blue = color.Z;
        }

        //public Color4(Color3 color)
        //{
        //    this.Alpha = 1f;
        //    this.Red = color.Red;
        //    this.Green = color.Green;
        //    this.Blue = color.Blue;
        //}

        public Color4(Color color)
        {
            this.Alpha = (float)color.A / (float)byte.MaxValue;
            this.Red = (float)color.R / (float)byte.MaxValue;
            this.Green = (float)color.G / (float)byte.MaxValue;
            this.Blue = (float)color.B / (float)byte.MaxValue;
        }

        public Color4(float red, float green, float blue)
        {
            this.Alpha = 1f;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public Color4(float alpha, float red, float green, float blue)
        {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public static explicit operator int(Color4 value)
        {
            return value.ToArgb();
        }

        //public static explicit operator Color3(Color4 value)
        //{
        //    return new Color3(value.Red, value.Green, value.Blue);
        //}

        public static explicit operator Vector3(Color4 value)
        {
            return new Vector3(value.Red, value.Green, value.Blue);
        }

        public static explicit operator Vector4(Color4 value)
        {
            return new Vector4(value.Red, value.Green, value.Blue, value.Alpha);
        }

        public static implicit operator Color4(Color value)
        {
            return new Color4(value);
        }

        public static explicit operator Color4(Vector4 value)
        {
            Vector4 vector4 = value;
            Color4 color4;
            color4.Alpha = vector4.W;
            color4.Red = vector4.X;
            color4.Green = vector4.Y;
            color4.Blue = vector4.Z;
            return color4;
        }

        public static explicit operator Color4(Vector3 value)
        {
            Vector3 vector3 = value;
            Color4 color4;
            color4.Alpha = 1f;
            color4.Red = vector3.X;
            color4.Green = vector3.Y;
            color4.Blue = vector3.Z;
            return color4;
        }

        //public static explicit operator Color4(Color3 value)
        //{
        //    Color3 color3 = value;
        //    Color4 color4;
        //    color4.Alpha = 1f;
        //    color4.Red = color3.Red;
        //    color4.Green = color3.Green;
        //    color4.Blue = color3.Blue;
        //    return color4;
        //}

        public static explicit operator Color4(int value)
        {
            return new Color4(value);
        }

        public static explicit operator Color(Color4 value)
        {
            return value.ToColor();
        }

        public static Color4 operator +(Color4 left, Color4 right)
        {
            Color4 color4;
            color4.Alpha = left.Alpha + right.Alpha;
            color4.Red = left.Red + right.Red;
            color4.Green = left.Green + right.Green;
            color4.Blue = left.Blue + right.Blue;
            return color4;
        }

        public static Color4 operator -(Color4 value)
        {
            Color4 color4;
            color4.Alpha = 1f - value.Alpha;
            color4.Red = 1f - value.Red;
            color4.Green = 1f - value.Green;
            color4.Blue = 1f - value.Blue;
            return color4;
        }

        public static Color4 operator -(Color4 left, Color4 right)
        {
            Color4 color4;
            color4.Alpha = left.Alpha - right.Alpha;
            color4.Red = left.Red - right.Red;
            color4.Green = left.Green - right.Green;
            color4.Blue = left.Blue - right.Blue;
            return color4;
        }

        public static Color4 operator *(Color4 color1, Color4 color2)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha * color2.Alpha;
            color4.Red = color1.Red * color2.Red;
            color4.Green = color1.Green * color2.Green;
            color4.Blue = color1.Blue * color2.Blue;
            return color4;
        }

        public static Color4 operator *(float scale, Color4 value)
        {
            return value * scale;
        }

        public static Color4 operator *(Color4 value, float scale)
        {
            float num = value.Alpha;
            Color4 color4;
            color4.Alpha = num;
            color4.Red = value.Red * scale;
            color4.Green = value.Green * scale;
            color4.Blue = value.Blue * scale;
            return color4;
        }

        public static bool operator ==(Color4 left, Color4 right)
        {
            return Color4.Equals(ref left, ref right);
        }

        public static bool operator !=(Color4 left, Color4 right)
        {
            return !Color4.Equals(ref left, ref right);
        }

      
        //public Color3 ToColor3()
        //{
        //    return new Color3(this.Red, this.Green, this.Blue);
        //}

        public Color ToColor()
        {
            return Color.FromArgb((int)((double)this.Alpha * (double)byte.MaxValue), (int)((double)this.Red * (double)byte.MaxValue), (int)((double)this.Green * (double)byte.MaxValue), (int)((double)this.Blue * (double)byte.MaxValue));
        }

        public int ToArgb()
        {
            return (((int)(uint)((double)this.Alpha * (double)byte.MaxValue) * 256 + (int)(uint)((double)this.Red * (double)byte.MaxValue)) * 256 + (int)(uint)((double)this.Green * (double)byte.MaxValue)) * 256 + (int)(uint)((double)this.Blue * (double)byte.MaxValue);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.Red, this.Green, this.Blue);
        }

        public Vector4 ToVector4()
        {
            return new Vector4(this.Red, this.Green, this.Blue, this.Alpha);
        }

        public static void Add(ref Color4 color1, ref Color4 color2, out Color4 result)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha + color2.Alpha;
            color4.Red = color1.Red + color2.Red;
            color4.Green = color1.Green + color2.Green;
            color4.Blue = color1.Blue + color2.Blue;
            result = color4;
        }

        public static Color4 Add(Color4 color1, Color4 color2)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha + color2.Alpha;
            color4.Red = color1.Red + color2.Red;
            color4.Green = color1.Green + color2.Green;
            color4.Blue = color1.Blue + color2.Blue;
            return color4;
        }

        public static void Subtract(ref Color4 color1, ref Color4 color2, out Color4 result)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha - color2.Alpha;
            color4.Red = color1.Red - color2.Red;
            color4.Green = color1.Green - color2.Green;
            color4.Blue = color1.Blue - color2.Blue;
            result = color4;
        }

        public static Color4 Subtract(Color4 color1, Color4 color2)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha - color2.Alpha;
            color4.Red = color1.Red - color2.Red;
            color4.Green = color1.Green - color2.Green;
            color4.Blue = color1.Blue - color2.Blue;
            return color4;
        }

        public static void Modulate(ref Color4 color1, ref Color4 color2, out Color4 result)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha * color2.Alpha;
            color4.Red = color1.Red * color2.Red;
            color4.Green = color1.Green * color2.Green;
            color4.Blue = color1.Blue * color2.Blue;
            result = color4;
        }

        public static Color4 Modulate(Color4 color1, Color4 color2)
        {
            Color4 color4;
            color4.Alpha = color1.Alpha * color2.Alpha;
            color4.Red = color1.Red * color2.Red;
            color4.Green = color1.Green * color2.Green;
            color4.Blue = color1.Blue * color2.Blue;
            return color4;
        }

        public static void Lerp(ref Color4 color1, ref Color4 color2, float amount, out Color4 result)
        {
            Color4 color4;
            color4.Alpha = (color2.Alpha - color1.Alpha) * amount + color1.Alpha;
            color4.Red = (color2.Red - color1.Red) * amount + color1.Red;
            color4.Green = (color2.Green - color1.Green) * amount + color1.Green;
            color4.Blue = (color2.Blue - color1.Blue) * amount + color1.Blue;
            result = color4;
        }

        public static Color4 Lerp(Color4 color1, Color4 color2, float amount)
        {
            Color4 color4;
            color4.Alpha = (color2.Alpha - color1.Alpha) * amount + color1.Alpha;
            color4.Red = (color2.Red - color1.Red) * amount + color1.Red;
            color4.Green = (color2.Green - color1.Green) * amount + color1.Green;
            color4.Blue = (color2.Blue - color1.Blue) * amount + color1.Blue;
            return color4;
        }

        public static void Negate(ref Color4 color, out Color4 result)
        {
            Color4 color4;
            color4.Alpha = 1f - color.Alpha;
            color4.Red = 1f - color.Red;
            color4.Green = 1f - color.Green;
            color4.Blue = 1f - color.Blue;
            result = color4;
        }

        public static Color4 Negate(Color4 color)
        {
            Color4 color4;
            color4.Alpha = 1f - color.Alpha;
            color4.Red = 1f - color.Red;
            color4.Green = 1f - color.Green;
            color4.Blue = 1f - color.Blue;
            return color4;
        }

        public static void Scale(ref Color4 color, float scale, out Color4 result)
        {
            float num = color.Alpha;
            Color4 color4;
            color4.Alpha = num;
            color4.Red = color.Red * scale;
            color4.Green = color.Green * scale;
            color4.Blue = color.Blue * scale;
            result = color4;
        }

        public static Color4 Scale(Color4 color, float scale)
        {
            float num = color.Alpha;
            Color4 color4;
            color4.Alpha = num;
            color4.Red = color.Red * scale;
            color4.Green = color.Green * scale;
            color4.Blue = color.Blue * scale;
            return color4;
        }

        public static void AdjustContrast(ref Color4 color, float contrast, out Color4 result)
        {
            float num = color.Alpha;
            Color4 color4;
            color4.Alpha = num;
            color4.Red = (float)(((double)color.Red - 0.5) * (double)contrast + 0.5);
            color4.Green = (float)(((double)color.Green - 0.5) * (double)contrast + 0.5);
            color4.Blue = (float)(((double)color.Blue - 0.5) * (double)contrast + 0.5);
            result = color4;
        }

        public static Color4 AdjustContrast(Color4 color, float contrast)
        {
            float num = color.Alpha;
            Color4 color4;
            color4.Alpha = num;
            color4.Red = (float)(((double)color.Red - 0.5) * (double)contrast + 0.5);
            color4.Green = (float)(((double)color.Green - 0.5) * (double)contrast + 0.5);
            color4.Blue = (float)(((double)color.Blue - 0.5) * (double)contrast + 0.5);
            return color4;
        }

        public static void AdjustSaturation(ref Color4 color, float saturation, out Color4 result)
        {
            float num1 = (float)((double)color.Green * 0.715399980545044 + (double)color.Red * 0.212500005960464 + (double)color.Blue * 0.0720999985933304);
            float num2 = color.Alpha;
            Color4 color4;
            color4.Alpha = num2;
            color4.Red = (color.Red - num1) * saturation + num1;
            color4.Green = (color.Green - num1) * saturation + num1;
            color4.Blue = (color.Blue - num1) * saturation + num1;
            result = color4;
        }

        public static Color4 AdjustSaturation(Color4 color, float saturation)
        {
            float num1 = (float)((double)color.Green * 0.715399980545044 + (double)color.Red * 0.212500005960464 + (double)color.Blue * 0.0720999985933304);
            float num2 = color.Alpha;
            Color4 color4;
            color4.Alpha = num2;
            color4.Red = (color.Red - num1) * saturation + num1;
            color4.Green = (color.Green - num1) * saturation + num1;
            color4.Blue = (color.Blue - num1) * saturation + num1;
            return color4;
        }

        public override  string ToString()
        {
            object[] objArray = new object[4];
            float num1 = this.Alpha;
            objArray[0] = (object)num1.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num2 = this.Red;
            objArray[1] = (object)num2.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num3 = this.Green;
            objArray[2] = (object)num3.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            float num4 = this.Blue;
            objArray[3] = (object)num4.ToString((IFormatProvider)CultureInfo.CurrentCulture);
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "A:{0} R:{1} G:{2} B:{3}", objArray);
        }

        public override  int GetHashCode()
        {
            return this.Alpha.GetHashCode() + (this.Green.GetHashCode() + this.Blue.GetHashCode() + this.Red.GetHashCode());
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static bool Equals(ref Color4 value1, ref Color4 value2)
        {
            return (double)value1.Alpha == (double)value2.Alpha && (double)value1.Red == (double)value2.Red && ((double)value1.Green == (double)value2.Green && (double)value1.Blue == (double)value2.Blue);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool Equals(Color4 other)
        {
            return (double)this.Alpha == (double)other.Alpha && (double)this.Red == (double)other.Red && ((double)this.Green == (double)other.Green && (double)this.Blue == (double)other.Blue);
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public override  bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((Color4)obj);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElasticsearchDemo
{
	public static class Tools
	{
		private static Random random = new Random();

		#region 随机颜色
		/**
		 * 该方法基于 HSB（色相、饱和度、亮度）模型生成颜色，
		 * 通过调整色相值、固定饱和度和亮度值，可以得到更鲜艳、亮眼的颜色。
		 * 这种方法生成的颜色更加接近纯色，并且通过随机的色相值确保生成的颜色多样性。
		 */
		public static System.Drawing.Color GenerateRandomColor()
		{
			// 生成随机的 HSB（色相、饱和度、亮度）值
			double hue = random.NextDouble() * 360; // 随机生成色相值（0-360）
			double saturation = 0.8; // 设置固定的饱和度值（范围 0-1）
			double brightness = 0.8; // 设置固定的亮度值（范围 0-1）

			return HSBToRGB(hue, saturation, brightness);
		}

		private static System.Drawing.Color HSBToRGB(double hue, double saturation, double brightness)
		{
			double chroma = brightness * saturation;
			double huePrime = hue / 60.0;
			double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));
			double red, green, blue;

			if (huePrime >= 0 && huePrime < 1)
			{
				red = chroma;
				green = x;
				blue = 0;
			}
			else if (huePrime >= 1 && huePrime < 2)
			{
				red = x;
				green = chroma;
				blue = 0;
			}
			else if (huePrime >= 2 && huePrime < 3)
			{
				red = 0;
				green = chroma;
				blue = x;
			}
			else if (huePrime >= 3 && huePrime < 4)
			{
				red = 0;
				green = x;
				blue = chroma;
			}
			else if (huePrime >= 4 && huePrime < 5)
			{
				red = x;
				green = 0;
				blue = chroma;
			}
			else
			{
				red = chroma;
				green = 0;
				blue = x;
			}

			double lightness = brightness - chroma;
			red += lightness;
			green += lightness;
			blue += lightness;

			int redInt = Convert.ToInt32(red * 255);
			int greenInt = Convert.ToInt32(green * 255);
			int blueInt = Convert.ToInt32(blue * 255);

			return System.Drawing.Color.FromArgb(redInt, greenInt, blueInt);
		}
		#endregion


		//是否为邮箱
		public static bool IsValidEmail(string input)
		{
			// 邮箱地址的正则表达式模式
			string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

			// 使用正则表达式进行匹配
			bool isMatch = Regex.IsMatch(input, pattern);

			return isMatch;
		}

		//是否为数字
		public static bool IsNumeric(string input)
		{
			// 数字的正则表达式模式
			string pattern = @"^[0-9]+$";

			// 使用正则表达式进行匹配
			bool isMatch = Regex.IsMatch(input, pattern);

			return isMatch;
		}


	}
}

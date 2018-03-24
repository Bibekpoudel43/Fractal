using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_1
{
    class ToHSB
    {
        public float red, green, blue;

        public ToHSB()
        {
            red = green = blue = 0;

        }
        public static Color HSBtoRGB(float hue, float saturation, float brightness , Color[] cPic)
        {

            int r = 0, g = 0, b = 0;
            if (saturation == 0)
            {
                r = g = b = (int)(brightness * 255.0f + 0.5f);
            }
            else
            {
                float h = (hue - (float)Math.Floor(hue)) * 6.0f;
                float f = h - (float)Math.Floor(h);
                float p = brightness * (1.0f - saturation);
                float q = brightness * (1.0f - saturation * f);
                float t = brightness * (1.0f - (saturation * (1.0f - f)));
                switch ((int)h)
                {
                        case 0:
                            Color col0 = cPic[0];
                            r = (int)(brightness * (col0.R * 1.0f) + 0.5f);
                            g = (int)(t * (col0.G * 1.0f) + 0.5f);
                            b = (int)(p * (col0.B * 1.0f) + 0.5f);
                            break;
                        case 1:
                            Color col1 = cPic[1];
                            r = (int)(q * (col1.R * 1.0f) + 0.5f);
                            g = (int)(brightness * (col1.G * 1.0f) + 0.5f);
                            b = (int)(p * (col1.B * 1.0f) + 0.5f);
                            break;
                        case 2:
                            Color col2 = cPic[2];
                            r = (int)(p * (col2.R * 1.0f) + 0.5f);
                            g = (int)(brightness * (col2.G * 1.0f) + 0.5f);
                            b = (int)(t * (col2.B * 1.0f) + 0.5f);
                            break;
                        case 3:
                            Color col3 = cPic[3];
                            r = (int)(p * (col3.R * 1.0f) + 0.5f);
                            g = (int)(q * (col3.G * 1.0f) + 0.5f);
                            b = (int)(brightness * (col3.B * 1.0f) + 0.5f);
                            break;
                        case 4:
                            Color col4 = cPic[3];
                            r = (int)(t * (col4.R * 1.0f) + 0.5f);
                            g = (int)(p * (col4.G * 1.0f) + 0.5f);
                            b = (int)(brightness * (col4.B * 1.0f) + 0.5f);
                            break;
                        case 5:
                            Color col5 = cPic[5];
                            r = (int)(brightness * (col5.R * 1.0f) + 0.5f);
                            g = (int)(p * (col5.G * 1.0f) + 0.5f);
                            b = (int)(q * (col5.B * 1.0f) + 0.5f);
                            break;
                }
            }
            return Color.FromArgb(Convert.ToByte(255), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }
    }
}

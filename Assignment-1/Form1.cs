using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace Assignment_1
{
    public partial class Form : System.Windows.Forms.Form
    {

        private int MAX = 256;      // max iterations
        private double SX = -2.025; // start value real
        private double SY = -1.125; // start value imaginary
        private double EX = 0.6;    // end value real
        private double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Image picture;
        private Graphics g1;
        private Cursor c1, c2;
        private bool cndnColor;
        private ToHSB HSBcol = new ToHSB();
        Random rn = new Random();
        Color[] colorPic = new Color[6];
        private bool isLaunched = true;
        String[] settings = new String[4];


        private bool clicked = false;

        public Form()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

        }
        //fires when form is loaded
        private void Form_Load(object sender, EventArgs e)
        {
            init(); //load initial value
            storeColor();
            start(); //load mandelbrot on a pictureBox
        }
        
        //fires when user click on exit button
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteToFile();
            Close();
        }


        //takes the coordinate and write it to the file
        //state save
        private void WriteToFile()
        {
            StreamWriter sw = new StreamWriter(@"C:\Users\Bibek\Documents\Visual Studio 2015\Projects\Assignment-1\Fractal\Assignment-1\state\state.txt");
            sw.WriteLine(xstart);
            sw.WriteLine(ystart);
            sw.WriteLine(xende);
            sw.WriteLine(yende);
            sw.Close();
        }

        //read the coordinate from the file
        //state load
        private String[] ReadFromFile()
        {
            StreamReader sr = new StreamReader(@"C:\Users\Bibek\Documents\Visual Studio 2015\Projects\Assignment-1\Fractal\Assignment-1\state\state.txt");
            String line = "";
            String tempStrore = "";
            while ((line = sr.ReadLine()) != null)
            {
                tempStrore += (line + ",");
            }
            settings = tempStrore.Split(',');
            sr.Close();
            return settings;
        }

        //paint on the graphics of the picturebox
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImage(picture, 0, 0);

        }

        private void initvalues() // reset start values
        {
            string[] set = ReadFromFile();

            if(isLaunched)
            {
                isLaunched = false;
                xstart = Convert.ToDouble(set[0]);
                ystart = Convert.ToDouble(set[1]);
                xende = Convert.ToDouble(set[2]);
                yende = Convert.ToDouble(set[3]);
            } else
            {
                xstart = SX;
                ystart = SY;
                xende = EX;
                yende = EY;
                if ((float)((xende - xstart) / (yende - ystart)) != xy)
                    xstart = xende - (yende - ystart) * (double)xy;
            }
        
        }


        // Load when the form loads
        //all instances will be prepared
        public void init()
        {
            finished = false;
            c1 = Cursors.WaitCursor;
            c2 = Cursors.Cross;
            x1 = pictureBox.Size.Width;
            y1 = pictureBox.Size.Height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(picture);
            finished = true;
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        //for coloring the points 
        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }


        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f, c;
            Pen pn = null;
            Color col;

            action = false;
            pictureBox.Cursor = c1;
            statusBar.Text = ("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h;
                        //calling method of ToHSB class(passing value into)                   
                        col = ToHSB.HSBtoRGB(h, 0.8f, b, colorPic);
                        pn = new Pen(col);
                        //djm 
                        alt = h;
                    }
                    g1.DrawLine(pn, x, y, x + 1, y);
                }
            }
            statusBar.Text = ("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            pictureBox.Cursor = c2;
            action = true;
        }

        //fires when we press the mouse
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (action)
            {
                xs = e.X;
                ys = e.Y;
                rectangle = true;
            }
        }

        //fires when we drag the mouse 
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    
                }
                Graphics g = pictureBox.CreateGraphics();
                update(g);     
        }

        //fires when we release the pressed mouse 
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            clicked = false;
            int z, w;
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;



            }
        }

        //for drawing rectangle to the selected area
        public void update(Graphics g)
        {
            Pen pen = new Pen(Color.White);
            g.DrawImage(picture, 0, 0);
            if (rectangle)
            {

                if (xs < xe)
                {
                    if (ys < ye) g.DrawRectangle(pen, xs, ys, (xe - xs), (ye - ys));
                    else g.DrawRectangle(pen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g.DrawRectangle(pen, xe, ys, (xs - xe), (ye - ys));
                    else g.DrawRectangle(pen, xe, ye, (xs - xe), (ys - ye));
                }

            }

        }


        private void saveAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image    
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "JPeg|*.jpg|Bitmap|*.bmp|Gif|*.gif |Png|*.png";
            sf.Title = "Save an Image File";
            sf.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (sf.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                System.IO.FileStream fs =
                   (System.IO.FileStream)sf.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon 
                // File type selected in the dialog box.  
                // NOTE that the FilterIndex property is one-based.  
                switch (sf.FilterIndex)
                {
                    case 1:
                        picture.Save(fs, ImageFormat.Jpeg);
                        break;

                    case 2:
                        picture.Save(fs, ImageFormat.Bmp);
                        break;

                    case 3:
                        picture.Save(fs, ImageFormat.Gif);
                        break;

                    case 4:
                        picture.Save(fs, ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }


        //fires when the user click the change color submenu
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            storeColor();
            mandelbrot();
        }

        //generate random color and store in an array
        private void storeColor()
        {
            //default
            if (!cndnColor)
            {
                colorPic[0] = Color.FromArgb(255, 255, 255);
                colorPic[1] = Color.FromArgb(255, 255, 255);
                colorPic[2] = Color.FromArgb(255, 255, 255);
                colorPic[3] = Color.FromArgb(255, 255, 255);
                colorPic[4] = Color.FromArgb(255, 255, 255);
                colorPic[5] = Color.FromArgb(255, 255, 255);
                cndnColor = true;
            }
            else
            {
                colorPic[0] = Color.FromArgb(rn.Next(255), rn.Next(255), rn.Next(255));
                colorPic[1] = Color.FromArgb(rn.Next(255), rn.Next(255), rn.Next(255));
                colorPic[2] = Color.FromArgb(rn.Next(255), rn.Next(255), rn.Next(255));
                colorPic[3] = Color.FromArgb(rn.Next(255), rn.Next(255), rn.Next(255));
                colorPic[4] = Color.FromArgb(rn.Next(255), rn.Next(255), rn.Next(255));
                colorPic[5] = Color.FromArgb(rn.Next(255), rn.Next(255), rn.Next(255));
            }
        

            }
        }

    }

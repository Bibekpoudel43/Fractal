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
        String[] settings = new String[4];


        private bool clicked = false;

        public Form()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            init(); //load initial value
            storeColor();
            start(); //load mandelbrot on a pictureBox
        }

        //paint on the graphics of the picturebox
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.DrawImage(picture, 0, 0);

        }

  
        private void initvalues() // reset start values
        {
                xstart = SX;
                ystart = SY;
                xende = EX;
                yende = EY;
                if ((float)((xende - xstart) / (yende - ystart)) != xy)
                    xstart = xende - (yende - ystart) * (double)xy;  
        }

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

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (action)
            {
                xs = e.X;
                ys = e.Y;
                rectangle = true;
            }
        }

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
                FileStream fs =
                   (FileStream)sf.OpenFile();
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //takes the coordinate and write it to the file
        //state save function
        private void WriteToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text File|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = (FileStream)sfd.OpenFile();
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(xstart);
                sw.WriteLine(ystart);
                sw.WriteLine(xende);
                sw.WriteLine(yende);
                sw.WriteLine(colorPic[0].R);
                sw.WriteLine(colorPic[0].G);
                sw.WriteLine(colorPic[0].B);
                sw.WriteLine(colorPic[1].R);
                sw.WriteLine(colorPic[1].G);
                sw.WriteLine(colorPic[1].B);
                sw.WriteLine(colorPic[2].R);
                sw.WriteLine(colorPic[2].G);
                sw.WriteLine(colorPic[2].B);
                sw.WriteLine(colorPic[3].R);
                sw.WriteLine(colorPic[3].G);
                sw.WriteLine(colorPic[3].B);
                sw.WriteLine(colorPic[4].R);
                sw.WriteLine(colorPic[4].G);
                sw.WriteLine(colorPic[4].B);
                sw.WriteLine(colorPic[5].R);
                sw.WriteLine(colorPic[5].G);
                sw.WriteLine(colorPic[5].B);

                sw.Close();
            }

        }

        //read the value from the file
        //state load function
        private String[] ReadFromFile()
        {
            String line = "";
            String tempStrore = "";
            OpenFileDialog od = new OpenFileDialog();
            od.Title = "Open File";
            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            if (od.ShowDialog() == DialogResult.OK)
            {
               FileStream fs =(FileStream)od.OpenFile();
                StreamReader sr = new StreamReader(fs);
                    while ((line = sr.ReadLine()) != null)
                    {
                        tempStrore += (line + ",");
                    }
                    settings = tempStrore.Split(',');
                    sr.Close();
            }
            
            return settings;
        }

        //state save submenu
        //fires when the save state submenu is clicked
        private void saveState(object sender, EventArgs e)
        {
            WriteToFile();
        }

        //state load submenu
        //fires when the load state submenu is clicked
        private void loadState(object sender, EventArgs e)
        {
            string[] set = ReadFromFile();
                xstart = Convert.ToDouble(set[0]);
                ystart = Convert.ToDouble(set[1]);
                xende = Convert.ToDouble(set[2]);
                yende = Convert.ToDouble(set[3]);

            // Color
            colorPic[0] = Color.FromArgb(Convert.ToInt32(set[4]), Convert.ToInt32(set[5]), Convert.ToInt32(set[6]));
            colorPic[1] = Color.FromArgb(Convert.ToInt32(set[7]), Convert.ToInt32(set[8]), Convert.ToInt32(set[9]));
            colorPic[2] = Color.FromArgb(Convert.ToInt32(set[10]), Convert.ToInt32(set[11]), Convert.ToInt32(set[12]));
            colorPic[3] = Color.FromArgb(Convert.ToInt32(set[13]), Convert.ToInt32(set[14]), Convert.ToInt32(set[15]));
            colorPic[4] = Color.FromArgb(Convert.ToInt32(set[16]), Convert.ToInt32(set[17]), Convert.ToInt32(set[18]));
            colorPic[5] = Color.FromArgb(Convert.ToInt32(set[19]), Convert.ToInt32(set[20]), Convert.ToInt32(set[21]));
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        //color cycling function
        private void Color_Cycle(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void animateStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            storeColor();
            mandelbrot();
            pictureBox.Refresh();
        }

    }

}

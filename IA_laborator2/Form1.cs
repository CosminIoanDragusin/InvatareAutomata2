using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA_laborator2
{
    public partial class Form1 : Form
    {
        public class Pointc
        {
            public Pointc(Point P, Brush B,Rectangle R)
            {
                p = P;
                b = B;
                r = R;
            }

            public Point p { set; get; }
            public Brush b { set; get; }
            public Rectangle r { set; get; }
            public override String ToString() => $"({p}, {b},{r})";
        }
        private List<Pointc> puncte = new List<Pointc>();
        private List<Pointc> testarepuncte = new List<Pointc>();
        private List<Point> spectruPuncte = new List<Point>();
        private List<Pointc> puncteCentroizi = new List<Pointc>();
        private List<Brush> ColorList = new List<Brush>();
        private int origine_X;
        private int origine_Y;
        private int spectru = 300;
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            Random rand = new Random();

            //desenare axa 
            Graphics g;
            g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            AfisareSpectruAxe(g);
            using (StreamReader reader = new StreamReader("puncte.txt"))
            {

                while (!reader.EndOfStream)
                {
                    string CurrentLine = reader.ReadLine();
                    string[] bits = CurrentLine.Split(' ');
                    int x = int.Parse(bits[1]);
                    int y = int.Parse(bits[2]);
                    Point point = new Point(origine_X + x, origine_Y + y);
                    Rectangle r = new Rectangle(point, new Size(1, 1));
                    Pointc p = new Pointc(point, Brushes.Black, r);
                    puncte.Add(p);
                    g.FillRectangle(p.b, p.r) ;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g=this.CreateGraphics();
            origine_X = this.ClientRectangle.Width / 2;
            origine_Y = this.ClientRectangle.Height / 2;
            ColorList.Add(Brushes.Blue);
            ColorList.Add(Brushes.Red);
            ColorList.Add(Brushes.Green);
            ColorList.Add(Brushes.Yellow);
            ColorList.Add(Brushes.DarkViolet);
            ColorList.Add(Brushes.Cyan);
            ColorList.Add(Brushes.OrangeRed);
            ColorList.Add(Brushes.LawnGreen);
            ColorList.Add(Brushes.DarkGoldenrod);
            ColorList.Add(Brushes.Pink);
            int nrCentroizi = new Random().Next(2, 10);
            Random r = new Random();
            for (int i = 0; i < nrCentroizi; i++)
            {
                int x = r.Next(-300, 300);
                int y = r.Next(-300, 300);
                Point p = new Point(origine_X + x, origine_Y + y);
                Rectangle rect = new Rectangle(p, new Size(8, 8));
                puncteCentroizi.Add(new Pointc(p, ColorList[i], rect));
                g.FillRectangle(ColorList[i], rect);
            }
        }
        public void AfisareSpectruAxe(Graphics g)
        {
            origine_X = this.ClientRectangle.Width / 2;
            origine_Y = this.ClientRectangle.Height / 2;
            g.DrawLine(Pens.Black, new Point(origine_X, 0), new Point(origine_X, this.Bottom));
            g.DrawLine(Pens.Black, new Point(0, origine_Y), new Point(this.Right, origine_Y));
            g.FillEllipse(Brushes.Black, new Rectangle(new Point(origine_X - 2, origine_Y - 2), new Size(4, 4)));
            //delimitare spectru 
            Point point1 = new Point(origine_X + spectru, origine_Y - spectru);
            spectruPuncte.Add(point1);
            Point point2 = new Point(origine_X - spectru, origine_Y - spectru);
            spectruPuncte.Add(point2);
            Point point3 = new Point(origine_X - spectru, origine_Y + spectru);
            spectruPuncte.Add(point3);
            Point point4 = new Point(origine_X + spectru, origine_Y + spectru);
            spectruPuncte.Add(point4);
            spectruPuncte.Add(point1);

            Point[] points1 = spectruPuncte.ToArray();

            for (int i = 1; i < points1.Length; i++)
            {
                g.DrawLine(Pens.Red, points1[i - 1], points1[i]);
            }
        }
        public void AfisareCentroizi(Graphics g)
        {
            foreach (Pointc c in puncteCentroizi)
            {
                g.FillRectangle(c.b, c.r);
            }
        }
        public void AfisarePuncte(Graphics g)
        {
            foreach (Pointc p in puncte)
            {
                g.FillRectangle(p.b, p.r);
            }
        }
        public void GruparePuncte(Graphics g)
        {
            foreach (Pointc p in puncte)
            {
                Dictionary<Pointc, double> dist = new Dictionary<Pointc, double>();
                foreach (Pointc c in puncteCentroizi)
                {
                    double d = distantaEuclidiana(p.p.X, p.p.Y, c.p.X, c.p.Y);
                    dist.Add(c, d);
                }
                dist = dist.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                p.b = dist.First().Key.b;
                g.FillRectangle(p.b, p.r);
                dist.Clear();
            }
        }
        public double distantaEuclidiana(int px, int py, int cx, int cy)
        {
            double rez;
            rez= Math.Sqrt((Math.Pow((cx - px), 2) +Math.Pow((cy - py), 2)));
            return rez;
        }
        public List<Pointc> RecalculareCentroizi()
        {
            List<Pointc> l = new List<Pointc>();
            foreach (Pointc c in puncteCentroizi)
            {
                int newX = 0, newY = 0, index = 0;
                foreach (Pointc p in puncte)
                {
                   if (p.b==c.b)
                   {
                        newX += p.p.X;
                        newY += p.p.Y;
                        index++;
                   }
                }
                if (index != 0)
                {
                    newX = newX / index;
                    newY = newY / index;
                    Point punct = new Point(newX, newY);
                    Rectangle rect = new Rectangle(punct, new Size(8, 8));
                    Pointc centroid = new Pointc(punct, c.b, rect);
                    l.Add(centroid);
                }
                else l.Add(c);
            }
            return l;
        }
        private double CalculareDivergenta()
        {
            double div=0;
            foreach(Pointc c in puncteCentroizi)
            {
                foreach (Pointc p in puncte)
                {
                    if (p.b == c.b)
                    {
                        double dist = distantaEuclidiana(p.p.X, p.p.Y, c.p.X, c.p.Y);
                        div += dist;
                    }
                }
            }
            return div;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            g.Clear(BackColor);
            AfisareSpectruAxe(g);
            foreach (Pointc c in puncteCentroizi)
            {
                g.FillRectangle(c.b, c.r);
            }
            GruparePuncte(g);
            double divergenta = CalculareDivergenta();
            richTextBox1.Text = "E=" + divergenta + "\n";
            double divergentaNoua = 0;
            while (divergenta != divergentaNoua)
            {
                divergenta = CalculareDivergenta();
                g.Clear(BackColor);
                AfisareSpectruAxe(g);
                puncteCentroizi = RecalculareCentroizi();
                divergentaNoua = CalculareDivergenta();
                richTextBox1.Text += "E=" + divergentaNoua + "\n";
                foreach (Pointc c in puncteCentroizi)
                {
                    g.FillRectangle(c.b, c.r);
                }
                GruparePuncte(g);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            g.Clear(BackColor);
            for (int i=-300; i<=300;i=i+2)
            {
                for (int j=-300;j<=300;j=j+2)
                {
                    Point point = new Point(origine_X + i, origine_Y + j);
                    Rectangle r = new Rectangle(point, new Size(1, 1));
                    Pointc p = new Pointc(point, Brushes.Black, r);
                    testarepuncte.Add(p);
                }
            }
            AfisareSpectruAxe(g);
            foreach (Pointc c in puncteCentroizi)
            {
                g.FillRectangle(c.b, c.r);
            }
            foreach (Pointc p in testarepuncte)
            {
                Dictionary<Pointc, double> dist = new Dictionary<Pointc, double>();
                foreach (Pointc c in puncteCentroizi)
                {
                    double d = distantaEuclidiana(p.p.X, p.p.Y, c.p.X, c.p.Y);
                    dist.Add(c, d);
                }
                dist = dist.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                p.b = dist.First().Key.b;
                g.FillRectangle(p.b, p.r);
                dist.Clear();
            }
        
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

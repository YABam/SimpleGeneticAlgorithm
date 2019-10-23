using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		private bool isStart = false;
		private GeneOperation myOperator;
		private Population[] myPop;
		private int Generation = 0;

		double mul_X = 180;
		double mul_Y = 80;

		int x_Offset = 200;
		int y_Offset = 350;

		int curveCount = 200;

		private void Form1_Load(object sender, EventArgs e)
		{
			myOperator = new GeneOperation(0.1, 0.03, 20);
			myPop = myOperator.InitPopulation(2, -1, 100);//初始化种群
		}

		public Form1()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (isStart == false)
			{
				btnStart.Text = "pause";
				isStart = true;
				timer1.Enabled = true;
			}
			else
			{
				btnStart.Text = "start";
				isStart = false;
				timer1.Enabled = false;
			}
		}

		private int Trans_X(double x)
		{
			int result = 0;
			result = (int)(mul_X*x + x_Offset);
			return result;
		}

		private int Trans_Y(double y)
		{
			int result = 0;
			result = (int)(y_Offset - mul_Y*y);
			return result;
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Pen p = new Pen(Color.Black, 2);
			//绘制必要图形
			g.DrawLine(p, Trans_X(-2), Trans_Y(0), Trans_X(3), Trans_Y(0));
			g.DrawLine(p, Trans_X(0), Trans_Y(-5), Trans_X(0), Trans_Y(5));

			p.Color = Color.Blue;

			for (int i = 0; i < curveCount; i++)
			{
				g.DrawLine(p, Trans_X(-1+ i* ((2.0 - (-1.0)) / curveCount)), Trans_Y(myPop[0].CalFitness(-1 + i * ((2.0 - (-1.0)) / curveCount))), 
					Trans_X(-1 + (i+1) * ((2.0 - (-1.0)) / curveCount)), Trans_Y(myPop[0].CalFitness(-1 + (i+1) * ((2.0 - (-1.0)) / curveCount))));
			}
			//绘制交互图形
			p.Color = Color.Green;
			//g.DrawEllipse(p, 10, 10, 200, 200);
			//p.Width = 4;

			for (int i = 0; i < myPop.Length; i++)
			{
				g.DrawEllipse(p, Trans_X(myPop[i].Value)-2, Trans_Y(myPop[i].Fitness)-2, 4, 4);
				//g.DrawEllipse(p, 10, 10, 200, 200);
			}

			if (isStart == true)
			{
				myPop = myOperator.NextGeneration(myPop);
				Generation++;
				this.Text = Generation.ToString();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Invalidate();
		}
	}
}

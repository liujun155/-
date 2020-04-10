using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 散点凸包
{
   struct vector
    {
        public int x;
        public int y;
    }
    public partial class Form1 : Form
    {
        Graphics graphic;
        Color color;
        Pen pen;

        Boolean drawFlag = false;
        List<Point> allPoint = new List<Point>();
        List<Point> tmpList = new List<Point>();

        public Form1()
        {
            InitializeComponent();
            color = Color.FromArgb(255, 0, 0);
            pen = new Pen(color);
            graphic = this.drawPane.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (allPoint.Count > 0)
            {
                MessageBox.Show("请先清除画布。");
                return;
            }
            drawFlag = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            drawFlag = false;
        }

        private void drawPane_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawFlag)
            {
                allPoint.Add(new Point(e.X, e.Y));
                tmpList.Add(new Point(e.X, e.Y));
                graphic.FillEllipse(Brushes.Blue, e.X - 3, e.Y - 3, 6, 6);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (allPoint.Count < 3)
            {
                MessageBox.Show("至少三个点");
            }
            else if (allPoint.Count == 3)
            {
                drawList(allPoint);
            }
            else
            {
                List<Point> res = calcConvexHull(tmpList);
                drawList(res);
            }
        }
        private List<Point> calcConvexHull(List<Point> list)
        {
            drawFlag = false;
            List<Point> resPoint = new List<Point>();
            int minIndex = 0;
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].Y < list[minIndex].Y)
                {
                    minIndex = i;
                }
            }
            Point minPoint = list[minIndex];
            resPoint.Add(list[minIndex]);
            list.RemoveAt(minIndex);
            list.Sort(
                delegate(Point p1, Point p2)
                {
                    vector baseVec;
                    baseVec.x = 1;
                    baseVec.y = 0;

                    vector p1Vec;
                    p1Vec.x = p1.X - minPoint.X;
                    p1Vec.y = p1.Y - minPoint.Y;

                    vector p2Vec;
                    p2Vec.x = p2.X - minPoint.X;
                    p2Vec.y = p2.Y - minPoint.Y;

                    double up1 = p1Vec.x * baseVec.x;
                    double down1 = Math.Sqrt(p1Vec.x * p1Vec.x + p1Vec.y * p1Vec.y);

                    double up2 = p2Vec.x * baseVec.x;
                    double down2 = Math.Sqrt(p2Vec.x * p2Vec.x + p2Vec.y * p2Vec.y);


                    double cosP1 = up1 / down1;
                    double cosP2 = up2 / down2;

                    if (cosP1 > cosP2)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                );
            resPoint.Add(list[0]);
            resPoint.Add(list[1]);
            for (int i = 2; i < list.Count; i++)
            {
                Point basePt = resPoint[resPoint.Count - 2];
                vector v1;
                v1.x = list[i - 1].X - basePt.X;
                v1.y = list[i - 1].Y - basePt.Y;

                vector v2;
                v2.x = list[i].X - basePt.X;
                v2.y = list[i].Y - basePt.Y;

                if (v1.x * v2.y - v1.y * v2.x < 0)
                {
                    resPoint.RemoveAt(resPoint.Count - 1);
                    while (true)
                    {
                        Point basePt2 = resPoint[resPoint.Count - 2];
                        vector v12;
                        v12.x = resPoint[resPoint.Count - 1].X - basePt2.X;
                        v12.y = resPoint[resPoint.Count - 1].Y - basePt2.Y;
                        vector v22;
                        v22.x = list[i].X - basePt2.X;
                        v22.y = list[i].Y - basePt2.Y;
                        if (v12.x * v22.y - v12.y * v22.x < 0)
                        {
                            resPoint.RemoveAt(resPoint.Count - 1);
                        }
                        else
                        {
                            break;
                        }

                    }
                    resPoint.Add(list[i]);
                }
                else
                {
                    resPoint.Add(list[i]);
                }
            }
            return resPoint;
        }
        private void drawList(List<Point> list)
        {
            if (list == null)
            {
                MessageBox.Show("传入了一个null的list");
                return;
            }
            if (list != null && list.Count == 0)
            {
                MessageBox.Show("传入的list里面无数据");
                return;
            }
            if (list != null && list.Count == 1)
            {
                MessageBox.Show("一个点无法画线");
                return;
            }
            if (list != null && list.Count > 1)
            {
                int cnt = list.Count;
                for (int i = 0; i < cnt - 1; i++)
                {
                    graphic.DrawLine(pen, list[i], list[i + 1]);
                }
                graphic.DrawLine(pen, list[cnt - 1], list[0]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            drawFlag = false;
            allPoint.Clear();
            tmpList.Clear();
            drawPane.Refresh();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace AnimatedGame
{
    public partial class Form1 : Form
    {
        //Declare variables
        bool leftArrowDown, rightArrowDown, upArrowDown, downArrowDown, leftLook, rightLook, downLook;
        int xFrog, yFrog;
        const int FROGHEIGHT = 10, FROGWIDTH = 10;
        int rowHeight, columnWidth;
        int jumpWait = 200;
        int carOneLength = 50, carOneSpeed = 2, carTwoLength = 46, carTwoSpeed = 5, carThreeLength = 54, carThreeSpeed = -3, carFourLength = 46, carFourSpeed = 3;
        int logOneLength, logOneSpeed, logTwoLength, logTwoSpeed, logThreeLength, logThreeSpeed, logFourLength, logFourSpeed;

        //create brushes
        SolidBrush riverBrush = new SolidBrush(Color.Blue);
        SolidBrush roadBrush = new SolidBrush(Color.DarkSlateGray);

        //create new stopwatch and random number generator
        Stopwatch jumpWatch = new Stopwatch();
        Random rng = new Random();

        //create obstacle lists
        List<int> carRowOne = new List<int>();
        List<int> carRowTwo = new List<int>();
        List<int> carRowThree = new List<int>();
        List<int> carRowFour = new List<int>();
        List<int> logRowOne = new List<int>();
        List<int> logRowTwo = new List<int>();
        List<int> logRowThree = new List<int>();
        List<int> logRowFour = new List<int>();

        public Form1()
        {
            InitializeComponent();
            StartGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (leftArrowDown == true && xFrog > 0 && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                xFrog -= columnWidth;
                jumpWatch.Reset();
                jumpWatch.Start();
                leftLook = true;
            }
            if (rightArrowDown == true && xFrog < 15 * columnWidth && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                xFrog += columnWidth;
                jumpWatch.Reset();
                jumpWatch.Start();
                rightLook = true;
                leftLook = false;
            }
            if (upArrowDown == true && yFrog > 0 && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                yFrog -= rowHeight;
                jumpWatch.Reset();
                jumpWatch.Start();
                leftLook = false;
                rightLook = false;
                downLook = false;
            }
            if (downArrowDown == true && yFrog < rowHeight * 10 && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                yFrog += rowHeight;
                jumpWatch.Reset();
                jumpWatch.Start();
                downLook = true;
                leftLook = false;
                rightLook = false;
            }

            //update cars
            UpdateObstacles(carRowOne, carOneSpeed, carOneLength);
            UpdateObstacles(carRowTwo, carTwoSpeed, carTwoLength);
            UpdateObstacles(carRowThree, carThreeSpeed, carThreeLength);
            UpdateObstacles(carRowFour, carFourSpeed, carFourLength);
            //update logs
            UpdateObstacles(logRowOne, logOneSpeed, logOneLength);
            UpdateObstacles(logRowTwo, logTwoSpeed, logTwoLength);
            UpdateObstacles(logRowThree, logThreeSpeed, logThreeLength);
            UpdateObstacles(logRowFour, logFourSpeed, logFourLength);

            Rectangle frogRec = new Rectangle(xFrog, yFrog, FROGWIDTH, FROGHEIGHT);

            //check car collisions
            DetectCollisions(carRowOne, rowHeight * 9, carOneLength, rowHeight, frogRec);
            DetectCollisions(carRowTwo, rowHeight * 8, carTwoLength, rowHeight, frogRec);
            DetectCollisions(carRowThree, rowHeight * 7, carThreeLength, rowHeight, frogRec);
            DetectCollisions(carRowFour, rowHeight * 6, carFourLength, rowHeight, frogRec);
            //check log collisions
            DetectCollisions(logRowOne, rowHeight * 4, logOneLength, logOneSpeed, rowHeight, frogRec);
            DetectCollisions(logRowTwo, rowHeight * 3, logTwoLength, logTwoSpeed, rowHeight, frogRec);
            DetectCollisions(logRowThree, rowHeight * 2, logThreeLength, logThreeSpeed, rowHeight, frogRec);
            DetectCollisions(logRowFour, rowHeight, logFourLength, logFourSpeed, rowHeight, frogRec);

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            DrawBackground(e.Graphics);

            //draw frog
            if (leftLook == true) { e.Graphics.DrawImage(Properties.Resources.frogLeft, xFrog, yFrog); }
            else if (rightLook == true) { e.Graphics.DrawImage(Properties.Resources.frogRight, xFrog, yFrog); }
            else if (downLook == true) { e.Graphics.DrawImage(Properties.Resources.frogDown, xFrog, yFrog); }
            else { e.Graphics.DrawImage(Properties.Resources.frogUp, xFrog, yFrog); }
            //draw cars
            DrawObstacles(carRowOne, rowHeight * 9, e.Graphics, Properties.Resources.car1Right);
            DrawObstacles(carRowTwo, rowHeight * 8, e.Graphics, Properties.Resources.car2Right);
            DrawObstacles(carRowThree, rowHeight * 7, e.Graphics, Properties.Resources.car3Left);
            DrawObstacles(carRowFour, rowHeight * 6, e.Graphics, Properties.Resources.car2Right);
            //draw logs
            DrawObstacles(logRowOne, rowHeight * 4, e.Graphics);
            DrawObstacles(logRowTwo, rowHeight * 3, e.Graphics);
            DrawObstacles(logRowThree, rowHeight * 2, e.Graphics);
            DrawObstacles(logRowFour, rowHeight, e.Graphics);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
            }
        }

        public void StartGame()
        {
            //create columns and rows
            rowHeight = this.Height / 11;
            columnWidth = this.Width / 16;

            //set frog location
            yFrog = 10 * rowHeight;
            xFrog = 8 * columnWidth;
            //start frog jump delay
            jumpWatch.Start();

            //add values to lists
            carRowOne.AddRange(new int[] { -carOneLength, 3 * columnWidth, 6 * columnWidth, 9 * columnWidth, 12 * columnWidth, 15 * columnWidth });
            carRowTwo.AddRange(new int[] { 1 * columnWidth, 5 * columnWidth, 9 * columnWidth, 13 * columnWidth });
            carRowThree.AddRange(new int[] { 0, 3 * columnWidth, 4 * columnWidth, 7 * columnWidth, 10 * columnWidth, 11 * columnWidth, 14 * columnWidth, 15 * columnWidth });
            carRowFour.AddRange(new int[] { 0, 1 * columnWidth, 2 * columnWidth, 5 * columnWidth, 6 * columnWidth, 7 * columnWidth, 10 * columnWidth, 11 * columnWidth, 12 * columnWidth, 14 * columnWidth });
            logRowOne.AddRange(new int[] { });
            logRowTwo.AddRange(new int[] { });
            logRowThree.AddRange(new int[] { });
            logRowFour.AddRange(new int[] { });
        }

        public void DrawBackground(Graphics g)
        {
            g.Clear(Color.Green);
            g.FillRectangle(roadBrush, 0, 6 * rowHeight, this.Width, 4 * rowHeight);
            g.FillRectangle(riverBrush, 0, 1 * rowHeight, this.Width, 4 * rowHeight);
        }

        public void DrawObstacles(List<int> list, int y, Graphics g, Image image)
        {
            for (int i = 0; i <= list.Count() - 1; i++)
            {
                g.DrawImage(image, list[i], y);
            }
        }

        public void UpdateObstacles(List<int> list, int speed, int length)
        {
            if (speed > 0)
            {
                for (int i = 0; i <= list.Count() - 1; i++) { list[i] += speed; }
                for (int i = 0; i <= list.Count() - 1; i++) { if (list[i] >= this.Width) { list[i] = -length; } }
            }
            else
            {
                for (int i = 0; i <= list.Count() - 1; i++) { list[i] += speed; }
                for (int i = 0; i <= list.Count() - 1; i++) { if (list[i] <= -length) { list[i] = this.Width + length; } }
            }
        }

        public void DetectCollisions(List<int> list, int y, int length, int height, Rectangle frogRec)
        {
            for (int i = 0; i <= list.Count() - 1; i++)
            {
                Rectangle obstacleRec = new Rectangle(list[i], y, length, height);

                if (obstacleRec.IntersectsWith(frogRec))
                {
                    //death
                }
            }
        }

        public void DetectCollisions(List<int> list, int y, int height, int width, int speed, Rectangle frogRec)
        {
            for (int i = 0; i <= list.Count() - 1; i++)
            {
                Rectangle obstacleRec = new Rectangle(list[i], y, height, width);

                if (obstacleRec.IntersectsWith(frogRec))
                {
                    //moves frog log speed
                }
            }
        }
    }
}

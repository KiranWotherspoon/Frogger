using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Media;

namespace AnimatedGame
{
    public partial class GameScreen : UserControl
    {
        //Declare variables
        bool leftArrowDown, rightArrowDown, upArrowDown, downArrowDown, leftLook, rightLook, downLook, frogOnLog, frogDead, mosquitoHere, badMosquitoHere;
        bool frogSpotOne = false, frogSpotTwo = false, frogSpotThree = false, frogSpotFour = false, frogSpotFive = false, frogSpotSix = false;
        int xFrog, yFrog;
        const int FROGHEIGHT = 50, FROGWIDTH = 55;
        int rowHeight, columnWidth, jumpWait = 200, lives = 3, score = 0, time = 45;
        int carOneLength = 50, carOneSpeed = 2, carTwoLength = 46, carTwoSpeed = 5, carThreeLength = 54, carThreeSpeed = -3, carFourLength = 46, carFourSpeed = 3;
        int logLength = 78, logOneSpeed = -4, logTwoSpeed = 6, logThreeSpeed = -1, logFourSpeed = 5;
        int xMosquito, yMosquito, xBadMosquito, yBadMosquito;

        //create brushes and fonts
        SolidBrush riverBrush = new SolidBrush(Color.FromArgb(24, 13, 255));
        SolidBrush roadBrush = new SolidBrush(Color.FromArgb(127, 127, 127));
        SolidBrush grassBrush = new SolidBrush(Color.Green);
        SolidBrush livesBrush = new SolidBrush(Color.Red);
        SolidBrush wordBrush = new SolidBrush(Color.White);
        Font wordFont = new Font("Arial", 36, FontStyle.Bold);
        Font numberFont = new Font("Arial", 26, FontStyle.Bold);

        //create stopwatches
        Stopwatch jumpWatch = new Stopwatch();
        Stopwatch roundWatch = new Stopwatch();
        Stopwatch mosquitoWatch = new Stopwatch();
        Stopwatch badMosquitoWatch = new Stopwatch();
        Stopwatch speedUpWatch = new Stopwatch();

        //create sound players
        SoundPlayer splashPlayer = new SoundPlayer(Properties.Resources.splashSound);
        SoundPlayer hornPlayer = new SoundPlayer(Properties.Resources.hornSound);
        SoundPlayer musicPlayer = new SoundPlayer(Properties.Resources.backMusic);

        //create random numbre generator
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

        public GameScreen()
        {
            InitializeComponent();

            //create columns and rows
            rowHeight = this.Height / 11;
            columnWidth = this.Width / 16;

            //set everything to starting positions and values
            StartGame();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (leftArrowDown == true && xFrog > 0 && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                xFrog -= columnWidth;
                jumpWatch.Restart();
                leftLook = true;
            }
            if (rightArrowDown == true && xFrog < 15 * columnWidth && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                xFrog += columnWidth;
                jumpWatch.Restart();
                rightLook = true;
                leftLook = false;
            }
            if (upArrowDown == true && yFrog > 0 && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                yFrog -= rowHeight;
                jumpWatch.Restart();
                leftLook = false;
                rightLook = false;
                downLook = false;
            }
            if (downArrowDown == true && yFrog < rowHeight * 10 && jumpWatch.ElapsedMilliseconds > jumpWait)
            {
                yFrog += rowHeight;
                jumpWatch.Restart();
                downLook = true;
                leftLook = false;
                rightLook = false;
            }

            //create mosquito and manage mosquito
            if (rng.Next(1, 151) == 10 && mosquitoWatch.ElapsedMilliseconds >= 5000 && mosquitoHere == false)
            {
                yMosquito = rng.Next(1, 5) * rowHeight;
                xMosquito = rng.Next(0, 16) * columnWidth;
                mosquitoHere = true;
                mosquitoWatch.Restart();
            }
            if (mosquitoWatch.ElapsedMilliseconds >= 15000 && mosquitoHere == true) { mosquitoHere = false; }
            Rectangle mosquitoRec = new Rectangle(xMosquito, yMosquito, 80, 60);

            //create and manage bad mosquito
            if (rng.Next(1, 151) == 10 && badMosquitoWatch.ElapsedMilliseconds >= 5000 && badMosquitoHere == false)
            {
                xBadMosquito = rng.Next(0, 16) * columnWidth;
                yBadMosquito = rng.Next(1, 5) * rowHeight;
                badMosquitoHere = true;
                badMosquitoWatch.Restart();
            }
            if (badMosquitoWatch.ElapsedMilliseconds > 15000 && badMosquitoHere == true) { badMosquitoHere = false; }
            Rectangle badMosquitoRec = new Rectangle(xBadMosquito, yBadMosquito, 80, 60);

            //update cars
            UpdateObstacles(carRowOne, carOneSpeed, carOneLength);
            UpdateObstacles(carRowTwo, carTwoSpeed, carTwoLength);
            UpdateObstacles(carRowThree, carThreeSpeed, carThreeLength);
            UpdateObstacles(carRowFour, carFourSpeed, carFourLength);
            //update turtles
            UpdateObstacles(logRowOne, logOneSpeed, logLength);
            UpdateObstacles(logRowTwo, logTwoSpeed, logLength);
            UpdateObstacles(logRowThree, logThreeSpeed, logLength);
            UpdateObstacles(logRowFour, logFourSpeed, logLength);
            
            //create rectangle around frog
            Rectangle frogRec = new Rectangle(xFrog, yFrog, FROGWIDTH, FROGHEIGHT);

            //check mosquito collisions
            if (frogRec.IntersectsWith(mosquitoRec) && mosquitoHere == true)
            {
                lives++;
                mosquitoHere = false;
                mosquitoWatch.Restart();
            }
            if (frogRec.IntersectsWith(badMosquitoRec) && badMosquitoHere == true)
            {
                logOneSpeed -= 5;
                logTwoSpeed += 5;
                logThreeSpeed -= 5;
                logFourSpeed += 5;
                carOneSpeed += 5;
                carTwoSpeed += 5;
                carThreeSpeed -= 5;
                carFourSpeed += 5;

                badMosquitoHere = false;

                speedUpWatch.Restart();
                badMosquitoWatch.Restart();
            }
            if (speedUpWatch.ElapsedMilliseconds >= 3000)
            {
                logOneSpeed += 5;
                logTwoSpeed -= 5;
                logThreeSpeed += 5;
                logFourSpeed -= 5;
                carOneSpeed -= 5;
                carTwoSpeed -= 5;
                carThreeSpeed += 5;
                carFourSpeed -= 5;

                speedUpWatch.Stop();
                speedUpWatch.Reset();
            }

            //check car collisions
            DetectCollisions(carRowOne, rowHeight * 9, carOneLength, rowHeight, frogRec);
            DetectCollisions(carRowTwo, rowHeight * 8, carTwoLength, rowHeight, frogRec);
            DetectCollisions(carRowThree, rowHeight * 7, carThreeLength, rowHeight, frogRec);
            DetectCollisions(carRowFour, rowHeight * 6, carFourLength, rowHeight, frogRec);
            //check log collisions
            frogOnLog = false;
            DetectCollisions(logRowOne, rowHeight * 4, logLength, rowHeight, logOneSpeed, frogRec);
            DetectCollisions(logRowTwo, rowHeight * 3, logLength, rowHeight, logTwoSpeed, frogRec);
            DetectCollisions(logRowThree, rowHeight * 2, logLength,rowHeight, logThreeSpeed, frogRec);
            DetectCollisions(logRowFour, rowHeight, logLength, rowHeight, logFourSpeed, frogRec);
            //check to see if player is on one of the winning locations
            frogSpotOne = DetectWinCollisions(frogSpotOne, 0, frogRec);
            frogSpotTwo = DetectWinCollisions(frogSpotTwo, columnWidth * 3, frogRec);
            frogSpotThree = DetectWinCollisions(frogSpotThree, columnWidth * 6, frogRec);
            frogSpotFour = DetectWinCollisions(frogSpotFour, columnWidth * 9, frogRec);
            frogSpotFive = DetectWinCollisions(frogSpotFive, columnWidth * 12, frogRec);
            frogSpotSix = DetectWinCollisions(frogSpotSix, columnWidth * 15, frogRec);
            if (yFrog == 0) { Death(); } //if they aren't on any win spots and they are on the last row, kill them

            //check for other death conditions - in the river / time ran out
            if (0 < yFrog && yFrog <= rowHeight * 4 && frogOnLog == false) { splashPlayer.Play(); Death(); }
            if (time - (Convert.ToInt32(roundWatch.ElapsedMilliseconds) / 1000) <= 0 || xFrog <= -FROGWIDTH || xFrog >= this.Width) { Death(); }
            //check to see if all the win locations are filled with frogs
            if (frogSpotOne == true && frogSpotTwo == true && frogSpotThree == true && frogSpotFour == true && frogSpotFive == true && frogSpotSix == true)
            {
                GameWin();
            }

            Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            //draw the backround
            DrawBackground(e.Graphics);

            //draw turtles
            DrawObstacles(logRowOne, rowHeight * 4, e.Graphics, Properties.Resources.turtleIconLeft);
            DrawObstacles(logRowTwo, rowHeight * 3, e.Graphics, Properties.Resources.turtleIconRight);
            DrawObstacles(logRowThree, rowHeight * 2, e.Graphics, Properties.Resources.turtleIconLeft);
            DrawObstacles(logRowFour, rowHeight, e.Graphics, Properties.Resources.turtleIconRight);
            //draw frog
            if  (frogDead == false)
            {
                if (leftLook == true) { e.Graphics.DrawImage(Properties.Resources.frogLeft, xFrog, yFrog); }
                else if (rightLook == true) { e.Graphics.DrawImage(Properties.Resources.frogRight, xFrog, yFrog); }
                else if (downLook == true) { e.Graphics.DrawImage(Properties.Resources.frogDown, xFrog, yFrog); }
                else { e.Graphics.DrawImage(Properties.Resources.frogUp, xFrog, yFrog); }
            }
            //draw cars
            DrawObstacles(carRowOne, rowHeight * 9, e.Graphics, Properties.Resources.car1Right);
            DrawObstacles(carRowTwo, rowHeight * 8, e.Graphics, Properties.Resources.car2Right);
            DrawObstacles(carRowThree, rowHeight * 7, e.Graphics, Properties.Resources.car3Left);
            DrawObstacles(carRowFour, rowHeight * 6, e.Graphics, Properties.Resources.car2Right);

            //draw lives, remaining time and mosquitoes
            DrawLives(e.Graphics);
            DrawTime(e.Graphics, time - (Convert.ToInt32(roundWatch.ElapsedMilliseconds) / 1000));
            if(mosquitoHere == true) { e.Graphics.DrawImage(Properties.Resources.mosquitoIcon, xMosquito, yMosquito); }
            if (badMosquitoHere == true) { e.Graphics.DrawImage(Properties.Resources.mosquitoBadIcon, xBadMosquito, yBadMosquito); }

            //draw frogs on final row
            if (frogSpotOne == true) { DrawFrogs(e.Graphics, 0); }
            if (frogSpotTwo == true) { DrawFrogs(e.Graphics, columnWidth * 3); }
            if (frogSpotThree == true) { DrawFrogs(e.Graphics, columnWidth * 6); }
            if (frogSpotFour == true) { DrawFrogs(e.Graphics, columnWidth * 9); }
            if (frogSpotFive == true) { DrawFrogs(e.Graphics, columnWidth * 12); }
            if (frogSpotSix == true) { DrawFrogs(e.Graphics, columnWidth * 15); }

            //draw score
            DrawScore(e.Graphics);
        }

        public void DrawBackground(Graphics g)
        {
            //draw the grass, road, and water
            g.Clear(Color.Green);
            g.FillRectangle(roadBrush, 0, 6 * rowHeight, this.Width, 4 * rowHeight);
            g.FillRectangle(riverBrush, 0, rowHeight * 1, this.Width, 4 * rowHeight);
            for (int i = 1; i < 17; i += 3)
            {
                g.FillRectangle(riverBrush, columnWidth * i, 0, columnWidth * 2, rowHeight);
            }
        }

        public void DrawObstacles(List<int> list, int y, Graphics g, Image image)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                g.DrawImage(image, list[i], y);
            }
        }

        public void DrawLives(Graphics g)
        {
            for (int i = 0; i < lives; i++)
            {
                g.FillEllipse(livesBrush, columnWidth + i * rowHeight / 2, (rowHeight * 10) + (rowHeight / 4), rowHeight / 2, rowHeight / 2);
            }
        }

        public void DrawTime (Graphics g, int t)
        {
            g.DrawString(t.ToString("00"), numberFont, wordBrush, columnWidth / 4, (rowHeight * 10) + (rowHeight / 4));
        }

        public void DrawScore (Graphics g)
        {
            g.DrawString(score.ToString("0000"), numberFont, wordBrush, columnWidth * 29/2, rowHeight / 4);
        }

        public void DrawFrogs(Graphics g, int x)
        {
            g.DrawImage(Properties.Resources.frogDown, x, 0);
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                case Keys.Escape:
                    Cursor.Show();
                    Form f = this.FindForm();
                    f.Controls.Remove(this);
                    MainScreen ms = new MainScreen();
                    f.Controls.Add(ms);
                    ms.Focus();
                    musicPlayer.Stop();
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
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
            //set frog location to starting position (bottom middle)
            yFrog = 10 * rowHeight;
            xFrog = 8 * columnWidth;
            //start frog jump delay and round timer
            jumpWatch.Restart();
            roundWatch.Restart();
            mosquitoWatch.Restart();
            badMosquitoWatch.Restart();

            //clear all lists
            carRowOne.RemoveRange(0, carRowOne.Count);
            carRowTwo.RemoveRange(0, carRowTwo.Count);
            carRowThree.RemoveRange(0, carRowThree.Count);
            carRowFour.RemoveRange(0, carRowFour.Count);
            logRowOne.RemoveRange(0, logRowOne.Count);
            logRowTwo.RemoveRange(0, logRowTwo.Count);
            logRowThree.RemoveRange(0, logRowThree.Count);
            logRowFour.RemoveRange(0, logRowFour.Count);

            //add values to lists
            carRowOne.AddRange(new int[] { -carOneLength, 3 * columnWidth, 6 * columnWidth, 9 * columnWidth, 12 * columnWidth, 15 * columnWidth });
            carRowTwo.AddRange(new int[] { 1 * columnWidth, 5 * columnWidth, 9 * columnWidth, 13 * columnWidth });
            carRowThree.AddRange(new int[] { 0, 3 * columnWidth, 4 * columnWidth, 7 * columnWidth, 10 * columnWidth, 11 * columnWidth, 14 * columnWidth, 15 * columnWidth });
            carRowFour.AddRange(new int[] { 0, 1 * columnWidth, 2 * columnWidth, 5 * columnWidth, 6 * columnWidth, 7 * columnWidth, 10 * columnWidth, 11 * columnWidth, 12 * columnWidth, 14 * columnWidth });
            logRowOne.AddRange(new int[] {columnWidth * 2, columnWidth * 4, columnWidth * 5, columnWidth * 7, columnWidth * 8, columnWidth * 10, columnWidth * 15});
            logRowTwo.AddRange(new int[] { columnWidth * 1, columnWidth * 5, columnWidth * 8, columnWidth * 9, columnWidth * 10, columnWidth * 13, columnWidth * 15});
            logRowThree.AddRange(new int[] { 0, columnWidth * 2, columnWidth * 3, columnWidth * 6, columnWidth * 7, columnWidth * 10, columnWidth * 11, columnWidth * 12});
            logRowFour.AddRange(new int[] { 0, columnWidth * 4, columnWidth * 7, columnWidth * 8, columnWidth * 11, columnWidth * 13, columnWidth * 14});

            //start the background music
            musicPlayer.PlayLooping();

            Form1.service.startGame();
        }

        public void Death()
        {
            Graphics g = this.CreateGraphics();
            //pause the game and make sure the forg is drawn in the right position
            Refresh();
            gameTimer.Enabled = false;
            //take away a life and add to score based on the players current position
            lives -= 1;
            score += 500 - (50 * (yFrog / rowHeight));

            //set draw frog bool to true, then redraw screen without frog
            frogDead = true;
            Thread.Sleep(750);
            Refresh();

            //draw death icon at the frogs location
            g.DrawImage(Properties.Resources.deathIcon, xFrog, yFrog);

            //check to see if they have any remaining lives
            if (lives == 0) //if they don't
            {
                //draw game over and final score
                g.DrawString("GAME OVER\n Score: " + score, wordFont, wordBrush, columnWidth * 6, rowHeight * 4);
                Thread.Sleep(1500);

                //return to main menu
                Cursor.Show();
                Form f = this.FindForm();
                f.Controls.Remove(this);
                MainScreen ms = new MainScreen();
                f.Controls.Add(ms);
                Form1.service.endGame(score);
            }
            else //if they do
            {        
                Thread.Sleep(750);

                //reset frog and timer
                yFrog = 10 * rowHeight;
                xFrog = 8 * columnWidth;
                roundWatch.Restart();
      
                //make sure frog is drawn again and un-pause the game
                frogDead = false;
                gameTimer.Enabled = true;
            }
        }

        public void RoundWin ()
        {
            Graphics g = this.CreateGraphics();
            //pause the game
            gameTimer.Enabled = false;

            //add to the score based on time it took player to finish round and amount of lives left
            score += 1000 * lives * Convert.ToInt32(roundWatch.ElapsedMilliseconds) / 1000 / time;

            //reset frog and timer 
            yFrog = 10 * rowHeight;
            xFrog = 8 * columnWidth;
            roundWatch.Restart();

            //un-pause the game
            gameTimer.Enabled = true;
        }

        public void UpdateObstacles(List<int> list, int speed, int length)
        {
            if (speed > 0)
            {
                for (int i = 0; i < list.Count(); i++) { list[i] += speed; }
                for (int i = 0; i < list.Count(); i++) { if (list[i] >= this.Width) { list[i] = -length; } }
            }
            else
            {
                for (int i = 0; i < list.Count(); i++) { list[i] += speed; }
                for (int i = 0; i < list.Count(); i++) { if (list[i] <= -length) { list[i] = this.Width + length; } }
            }
        }

        public void DetectCollisions(List<int> list, int y, int length, int height, Rectangle frogRec)
        {
            bool frogHit = false;
            for (int i = 0; i < list.Count(); i++)
            {
                Rectangle obstacleRec = new Rectangle(list[i], y, length, height);

                if (obstacleRec.IntersectsWith(frogRec))
                {
                    frogHit = true;
                }
            }
            if (frogHit == true)
            {
                hornPlayer.Play();
                Death();
            }
        }

        public void DetectCollisions(List<int> list, int y, int height, int width, int speed, Rectangle frogRec)
        {
            bool moveFrog = false;
            for (int i = 0; i < list.Count(); i++)
            {
                Rectangle obstacleRec = new Rectangle(list[i], y, height, width);

                if (obstacleRec.IntersectsWith(frogRec))
                {
                    frogOnLog = true;
                    moveFrog = true;
                }                
            }
            if (moveFrog == true)
            {
                xFrog += speed;
            }
        }

        public bool DetectWinCollisions(bool frogSpot, int x, Rectangle frogRec)
        {
            Rectangle winRec = new Rectangle(x, 0, columnWidth, rowHeight);

            if (winRec.IntersectsWith(frogRec) && frogSpot == false)
            {
                frogSpot = true;
                RoundWin();
            }

            return frogSpot;
        }

        public void GameWin()
        {
            Graphics g = this.CreateGraphics();
            //pause the game
            gameTimer.Enabled = false;

            //draw keep going in the middle of the screen
            g.DrawString("Keep Going!", wordFont, wordBrush, columnWidth * 6, rowHeight * 4);
            Thread.Sleep(1500);

            //add to the score based on amount of lives remaining
            score += 2000 * lives;
            //reduce the amount of time player will have to complete a single round
            time -= 5;
            //reset all of the win locations
            frogSpotOne = false;
            frogSpotTwo = false;
            frogSpotThree = false;
            frogSpotFour = false;
            frogSpotFive = false;
            frogSpotSix = false;

            //reset and un-pause the game
            StartGame();
            gameTimer.Enabled = true;
        }
    }
}

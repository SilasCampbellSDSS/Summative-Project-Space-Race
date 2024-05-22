using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Summative_Project_Space_Race
{
    public partial class SpaceRacer : Form
    {
        //My record is fourteen points in one game
        //Global variables

        //Player variables
        Rectangle player1 = new Rectangle(163, 505, 15, 25);
        Rectangle player2 = new Rectangle(487, 505, 15, 25);

        int playerSpeed = 10;
        int meteorSize = 0;

        //Lists
        List<Rectangle> meteorList = new List<Rectangle>();
        List<int> meteorSizes = new List<int>();
        List<int> meteorSpeeds = new List<int>();

        int player1Score = 0;
        int player2Score = 0;
        int time = 1000;

        Random randGen = new Random();
        Random randSize = new Random();
        Random randomSpeeds = new Random();

        int randValue = 0;

        bool upPressed = false;
        bool downPressed = false;
        bool wPressed = false;
        bool sPressed = false;

        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        public SpaceRacer()
        {
            InitializeComponent();
        }

        private void SpaceRacer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    upPressed = true;
                    break;
                case Keys.Down:
                    downPressed = true;
                    break;
                case Keys.W:
                    wPressed = true;
                    break;
                case Keys.S:
                    sPressed = true;
                    break;
                case Keys.Space:
                    gameTimer.Enabled = true;
                    InitializeGame();
                    break;
                case Keys.Escape:
                    if (gameTimer.Enabled == false)
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void SpaceRacer_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    upPressed = false;
                    break;
                case Keys.Down:
                    downPressed = false;
                    break;
                case Keys.W:
                    wPressed = false;
                    break;
                case Keys.S:
                    sPressed = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //Making a game timer
            time--;
            timeLabel.Text = $"{time}";

            MeteorMovement();

            MeteorRandomGeneration();

            OutOfBounds();

            ScoringMethod();

            PlayerCollisions();

            WinMethod();

            PlayerMovement();

            Refresh();
        }

        public void InitializeGame()
        {
            winLabel.Visible = false;
            titleLabel.Visible = false;
            p2ScoreLabel.Text = "0";
            p1ScoreLabel.Text = "0";

            meteorList.Clear();
            meteorSizes.Clear();
            meteorSpeeds.Clear();

            time = 1000;
            player1Score = 0;
            player2Score = 0;
            player1.Y = 505;
            player2.Y = 505;
        }
        public void MeteorMovement()
        {
            //For generating sideways moving meteors continuously
            for (int i = 0; i < meteorList.Count(); i++)
            {
                int x = meteorList[i].X + meteorSpeeds[i];
                meteorList[i] = new Rectangle(x, meteorList[i].Y, meteorSizes[i], meteorSizes[i]);
            }
        }
        public void MeteorRandomGeneration()
        {
            randValue = randGen.Next(1, 100);

            //Making meteors and their sizes and speeds random
            if (randValue <= 15)
            {
                randValue = randGen.Next(30, this.Height);
                Rectangle meteor = new Rectangle(0, randValue, meteorSize, meteorSize);
                meteorList.Add(meteor);

                meteorSizes.Add(randSize.Next(3, 10));
                meteorSpeeds.Add(randomSpeeds.Next(4, 17));
            }

            if (randValue > 15 && randValue <= 30)
            {
                randValue = randGen.Next(30, this.Height);
                Rectangle meteor = new Rectangle(650, randValue, meteorSize, meteorSize);
                meteorList.Add(meteor);

                meteorSizes.Add(randSize.Next(3, 10));
                meteorSpeeds.Add(randomSpeeds.Next(-17, -4));
            }
        }
        public void OutOfBounds()
        {
            //Removing meteor if it goes out of bounds
            for (int i = 0; i < meteorList.Count(); i++)
            {
                if (meteorList[i].X > 650 || meteorList[i].Y > 495)
                {
                    meteorList.RemoveAt(i);
                    meteorSpeeds.RemoveAt(i);
                    meteorSizes.RemoveAt(i);
                }
            }
        }
        public void ScoringMethod()
        {
            //Making the players get a point when they reach the end and then bringing them back down
            if (player1.Y < 1)
            {
                SoundPlayer pointPlayer = new SoundPlayer(Properties.Resources.point);
                pointPlayer.Play();
                
                player1.Y = 505;
                player1Score++;
                p1ScoreLabel.Text = $"{player1Score}";
            }

            if (player2.Y < 1)
            {
                SoundPlayer pointPlayer = new SoundPlayer(Properties.Resources.point);
                pointPlayer.Play();

                player2.Y = 505;
                player2Score++;
                p2ScoreLabel.Text = $"{player2Score}";
            }
        }
        public void PlayerCollisions()
        {
            //Making it so that if player 1 or 2 intersects with a meteor they go back down
            for (int i = 0; i < meteorList.Count(); i++)
            {
                if (meteorList[i].IntersectsWith(player1))
                {
                    SoundPlayer collisionPlayer = new SoundPlayer(Properties.Resources.collision);
                    collisionPlayer.Play();
                    
                    player1.Y = 505;
                    meteorList.RemoveAt(i);
                    meteorSpeeds.RemoveAt(i);
                    meteorSizes.RemoveAt(i);
                }

                if (meteorList[i].IntersectsWith(player2))
                {
                    SoundPlayer collisionPlayer = new SoundPlayer(Properties.Resources.collision);
                    collisionPlayer.Play();

                    player2.Y = 505;
                    meteorList.RemoveAt(i);
                    meteorSpeeds.RemoveAt(i);
                    meteorSizes.RemoveAt(i);
                }
            }
        }
        public void WinMethod()
        {
            //Deciding who wins at the end of the game
            if (time == 0)
            {
                gameTimer.Stop();
                SoundPlayer gongPLayer = new SoundPlayer(Properties.Resources.gong);
                gongPLayer.Play();

                winLabel.Visible = true;
                titleLabel.Visible = true;

                winLabel.Text = "PRESS SPACE TO RESTART \n\n PRESS ESC TO EXIT";

                if (player1Score > player2Score)
                {
                    winLabel.Text = "PLAYER 1 WINS";
                }
                else if (player2Score > player1Score)
                {
                    winLabel.Text = "PLAYER 2 WINS";
                }
                else
                {
                    winLabel.Text = "TIE GAME";
                }
            }
        }
        public void PlayerMovement()
        {
            //Player movement controls
            if (upPressed == true && player2.Y > 0)
            {
                player2.Y = player2.Y - playerSpeed;
            }

            if (downPressed == true && player2.Y < 500)
            {
                player2.Y = player2.Y + playerSpeed;
            }

            if (wPressed == true && player1.Y > 0)
            {
                player1.Y = player1.Y - playerSpeed;
            }

            if (sPressed == true && player1.Y < 500)
            {
                player1.Y = player1.Y + playerSpeed;
            }
        }
        private void SpaceRacer_Paint(object sender, PaintEventArgs e)
        {
            //Drawing players and meteors
            if (gameTimer.Enabled == true)
            {
                e.Graphics.FillRectangle(blueBrush, player1);
                e.Graphics.FillRectangle(redBrush, player2);

                for (int i = 0; i < meteorList.Count(); i++)
                {
                    e.Graphics.FillEllipse(whiteBrush, meteorList[i]);
                }
            }
        }
    }
}

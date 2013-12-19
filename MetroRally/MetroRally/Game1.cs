using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MetroRally.Entities;

namespace MetroRally
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Car car;
        Texture2D carTexture, GameOverScreen, healthScreen;
        Motion motion;

        private ScrollingBackground myBackground;
        private int velocity=60;

        public int lifes;
        string life;
        SpriteFont Font1;
        Vector2 FontPos;

        Obstacles obstacle;

        bool isGameOver, isHealthScreenShown;
        private int score;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            if (Motion.IsSupported)
                motion = new Motion();

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferHeight = 1280;
            graphics.PreferredBackBufferWidth = 768;
            graphics.IsFullScreen = true;
           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            car = new Car();
            if (motion != null)
            {
                motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);
                motion.Start();
            }
    
            lifes = 5;
            isHealthScreenShown = false;

            base.Initialize();
            life = "Your score is:";
        }

        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            car.Position.X = (int)(e.SensorReading.Gravity.X * 300 + 200);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            carTexture = this.Content.Load<Texture2D>(".\\Textures\\car");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            obstacle = new Obstacles();
            Texture2D obstacle1 = this.Content.Load<Texture2D>(".\\Textures\\obstacle1");

            obstacle.Load(GraphicsDevice, obstacle1);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            myBackground = new ScrollingBackground();
            Texture2D background = this.Content.Load<Texture2D>(".\\Textures\\background");


            myBackground.Load(GraphicsDevice, background);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameOverScreen = Content.Load<Texture2D>(".\\Textures\\gameOver");
            isGameOver = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            healthScreen = Content.Load<Texture2D>(".\\Textures\\healthScreen");
            isGameOver = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Arial");
            FontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 0);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Rectangle carRect = new Rectangle((int)car.Position.X, (int)car.Position.Y, carTexture.Width, carTexture.Height);
            Rectangle obsRect = new Rectangle((int)obstacle.screenpos.X, (int)obstacle.screenpos.Y, carTexture.Width, carTexture.Height);
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                
                if (isGameOver)
                {
                    isGameOver = false;
                    lifes = 5;
                    velocity = 60;
                }
                else
                    this.Exit();
            }
            TouchCollection currentTouch = TouchPanel.GetState();

            if (carRect.Intersects(obsRect))
            {

                System.Diagnostics.Debug.WriteLine("Collision");
                if (obstacle.updateCollision == true)
                {
                    lifes--;
                    obstacle.updateCollision = false;
                    obstacle.screenpos.Y = 0;
                }
                
                isHealthScreenShown = true;
                
                if(lifes == 0)
                    isGameOver = true;
            }

            if (isGameOver == false)
            {
                foreach (TouchLocation location in currentTouch)
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            if (isHealthScreenShown == true)
                            {
                                isHealthScreenShown = false;
                            }
                            //car.Position.X = location.Position.X;
                            //car.Position.Y = location.Position.Y;
                            break;
                        case TouchLocationState.Moved:
                            if (motion == null)
                            {
                                if (Math.Abs(car.Position.X - (location.Position.X - carTexture.Width / 2)) > 130)
                                {
                                    break;
                                }
                                else
                                {
                                    car.Position.X = location.Position.X - carTexture.Width / 2;
                                    break;
                                }

                                //car.Position.Y = location.Position.Y;  
                                
                            }
                            break;
                    }
                }
                if (isHealthScreenShown == false)
                {
                    score++;
                    velocity += 1;
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    myBackground.Update(elapsed * velocity);

                    float elapsedObstacle = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    obstacle.Update(elapsed * velocity);
                }
            }
            else
            {
                isGameOver = true;
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (isGameOver == false)
            {
                //background
                spriteBatch.Begin();
                myBackground.Draw(spriteBatch);
                spriteBatch.End();

                //obstacle (opponents car)
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                obstacle.Draw(spriteBatch);
                spriteBatch.End();

                //player car
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatch.Draw(carTexture, car.Position, Color.White);
                spriteBatch.End();

                //health
                spriteBatch.Begin();
                spriteBatch.DrawString(Font1, lifes.ToString(), FontPos, Color.White);
                spriteBatch.End();

                if (isHealthScreenShown == true)
                {
                    //Vector2 FontOrigin = Font1.MeasureString(lifes);
                    spriteBatch.Begin();
                    spriteBatch.Draw(healthScreen, Vector2.Zero, Color.White);
                    spriteBatch.End();

                    
                   // spriteBatch.DrawString(Font1, lifes.ToString(), FontPos, Color.White);
                    
                }
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(GameOverScreen, Vector2.Zero, Color.White);
                spriteBatch.End();

                spriteBatch.Begin();
                spriteBatch.DrawString(Font1, life, Vector2.Zero, Color.White);
                spriteBatch.DrawString(Font1, score.ToString(), FontPos, Color.White);
               
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}

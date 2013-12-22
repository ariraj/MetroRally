using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;


namespace MetroRally
{
    /// <summary>
    /// Implemented by Pauli Korhonen
    /// </summary>
    

    public class Obstacles
    {
        public Texture2D obstacleTexture;
        public bool updateCollision;
        private int screenheight;
        private Vector2 origin;
        public Vector2 screenpos;
        private Random rand;
        private Color color;


        public void Load(GraphicsDevice device, Texture2D obsTexture)
        {
            obstacleTexture = obsTexture;
            screenheight = device.Viewport.Height;
            int screenwidth = device.Viewport.Width;
            // Set the origin so that we're drawing from the 
            // center of the top edge.
            origin = new Vector2(0, 0);
            // Set the screen position to the center of the screen.
            rand = new Random();
            int randomInt = rand.Next(10, 450);
            color = new Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));

            updateCollision = true;

            //screenpos = new Vector2(screenwidth / 5, 0);
            screenpos = new Vector2(randomInt, 0);
            // Offset to draw the second texture, when necessary.
           // texturesize = new Vector2(0, obstacleTexture.Height);

        }

       
        // Obstacles.Update
        public void Update(float deltaY)
        {
            screenpos.Y += deltaY;
            if (screenpos.Y > screenheight)
            {
                screenpos.Y = screenpos.Y % obstacleTexture.Height;
                rand = new Random();
                int randomInt = rand.Next(10, 450);
                System.Diagnostics.Debug.WriteLine(randomInt);
                screenpos.X = randomInt;
                color = new Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));

                updateCollision = true;
            }
            else
                updateCollision = true;
        }
        // Obstacle.Draw
        public void Draw(SpriteBatch batch)
        {
            // Draw the texture, if it is still onscreen.
            if (screenpos.Y < screenheight)
            {
                batch.Draw(obstacleTexture, screenpos, null,
                     color, 0, origin, 1, SpriteEffects.None, 0f);
            }
            // Draw the texture a second time, behind the first,
            // to create the scrolling illusion.
           // batch.Draw(obstacleTexture, screenpos - texturesize, null,
             //    Color.White, 0, origin, 1, SpriteEffects.None, 0f);
        }
    }
}

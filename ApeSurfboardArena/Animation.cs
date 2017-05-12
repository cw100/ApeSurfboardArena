using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApeSurfboardArena
{
    public class Animation
    {
        public Rectangle hitBox;
        public Rectangle bigHitBox;
        public bool reversed = false;
        public float scale = 1f;
        public bool active = true;
        public SpriteEffects flip;
        public Texture2D spriteSheet;
        float time;
        float frameTime = 1;
        public int frameIndex;
        int totalFrames = 1;
        public int frameHeight;
        public int frameWidth;
        //ENCAPSULATION of position values
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Vector2 origin;
        Rectangle source;
        public float angle;
        public Color color;
        public bool isPaused = false;
        public float offset = 1;
        private Color[] colorData;
        //ENCAPSULATION of colorData array
        public Color[] ColorData
        {
            get
            {
                //Creates empty Color array based on texture area
                colorData = new Color[frameWidth * frameHeight];
                //Gets Color data of current frame
                spriteSheet.GetData<Color>(0, source, colorData, 0, frameWidth * frameHeight);
                return colorData; 
            }

        }
        public bool runOnce = false;
        public Matrix transformation;

        

        //Constructor creates animation with angle and color (EXAMPLE OF POLYMOPHISM)
        public Animation(Texture2D spriteSheet, int totalFrames, float animationLength, Vector2 position, float angle, Color color)
        {
            //Active by default
            active = true;
            //Sets texture
            this.spriteSheet = spriteSheet;
            //Gets all frame heights and widths
            frameHeight = spriteSheet.Height;
            frameWidth = spriteSheet.Width / totalFrames;
          
            //Sets amount of frames in animation
            this.totalFrames = totalFrames;
            //Calculates time of each frame
            this.frameTime = animationLength / totalFrames;
            //Default position
            this.position = position;
            //Default angle
            this.angle = angle;
            //Default color
            this.color = color;
            //Creates hitbox for use in collision with vectors or points
            hitBox = new Rectangle((int)X - (int)(frameWidth * scale / 2), (int)Y - (int)(frameHeight * scale / 2),
                (int)(frameWidth * scale), (int)(frameHeight * scale));
        }
        //Constructor creates single frame animation with angle and color (EXAMPLE OF POLYMOPHISM)
        public Animation(Texture2D spriteSheet, Vector2 position, float angle, Color color)
        {
            //Single frame animation
            this.totalFrames = 1;
            this.frameTime = 1;
            //Active by default
            active = true;
            //Sets texture
            this.spriteSheet = spriteSheet;
            //Gets all frame heights and widths
            frameHeight = spriteSheet.Height;
            frameWidth = spriteSheet.Width / totalFrames;          
            //Default position
            this.position = position;
            //Default angle
            this.angle = angle;
            //Default color
            this.color = color;
            //Creates hitbox for use in collision with vectors or points
            hitBox = new Rectangle((int)X - (int)(frameWidth * scale / 2), (int)Y - (int)(frameHeight * scale / 2),
                (int)(frameWidth * scale), (int)(frameHeight * scale));
        }
        //Constructor creates animation (EXAMPLE OF POLYMOPHISM)
        public Animation(Texture2D spriteSheet, int totalFrames, float animationLength, Vector2 position)
        {
            //Active by default
            active = true;
            //Sets texture
            this.spriteSheet = spriteSheet;
            //Gets all frame heights and widths
            frameHeight = spriteSheet.Height;
            frameWidth = spriteSheet.Width / totalFrames;          
            //Sets amount of frames in animation
            this.totalFrames = totalFrames;
            //Calculates time of each frame
            this.frameTime = animationLength / totalFrames;
            //Default position
            this.position = position;
            //Default angle
            angle = 0f;
            //Default color
            color = Color.White;
            //Creates hitbox for use in collision with vectors or points
            hitBox = new Rectangle((int)X - (int)(frameWidth * scale / 2), (int)Y - (int)(frameHeight * scale / 2),
                (int)(frameWidth * scale), (int)(frameHeight * scale));
        }
        //Constructor creates single frame animation (EXAMPLE OF POLYMOPHISM)
        public Animation(Texture2D spriteSheet, Vector2 position)
        {
            //Single frame animation
            totalFrames = 1;
            frameTime = 1;
            //Active by default
            active = true;
            //Sets texture
            this.spriteSheet = spriteSheet;
            //Gets all frame heights and widths
            frameHeight = spriteSheet.Height;
            frameWidth = spriteSheet.Width / totalFrames;
            //Default position
            this.position = position;
            //Default angle
            angle = 0f;
            //Default color
            color = Color.White;
            //Creates hitbox for use in collision with vectors or points
            hitBox = new Rectangle((int)X - (int)(frameWidth * scale / 2), (int)Y - (int)(frameHeight * scale / 2),
                (int)(frameWidth * scale), (int)(frameHeight * scale));
        }
        public Animation(Texture2D spriteSheet)
        {
            position = new Vector2(0, 0);
            //Single frame animation
            totalFrames = 1;
            frameTime = 1;
            //Active by default
            active = true;
            //Sets texture
            this.spriteSheet = spriteSheet;
            //Gets all frame heights and widths
            frameHeight = spriteSheet.Height;
            frameWidth = spriteSheet.Width / totalFrames;
            //Default position
            this.position = position;
            //Default angle
            angle = 0f;
            //Default color
            color = Color.White;
            //Creates hitbox for use in collision with vectors or points
            hitBox = new Rectangle((int)X - (int)(frameWidth * scale / 2), (int)Y - (int)(frameHeight * scale / 2),
                (int)(frameWidth * scale), (int)(frameHeight * scale));
        }
        //Creates color data array for use in pixel perfect collision

        //Animation game update logic
        public virtual void Update(GameTime gameTime)
        {
            //Only updates while active
            if (active)
            {
                //Only advances frame if not paused
                if (!isPaused)
                {
                    //Times each frame
                    time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Advances frame after elapsed frame time
                    while (time > frameTime)
                    {

                        //Checks if animation should be reversed
                        if (!reversed)
                        {
                            //Advances frame number
                            frameIndex++;
                            //Checks if animation is at it's end
                            if (frameIndex >= totalFrames)
                            {
                                //Sets animation back to start
                                frameIndex = 0;
                                //Checks if animation should be play only once
                                if (runOnce)
                                {
                                    //Sets to last frame
                                    frameIndex = totalFrames - 1;
                                    //Pause animation
                                    isPaused = true;
                                }
                            }
                        }
                        else
                        {
                            //Reverses frame number
                            frameIndex--;
                            //Checks if animation is at it's start
                            if (frameIndex < 1)
                            {
                                //Sets animation back to end
                                frameIndex = totalFrames - 1;

                                //Checks if animation should be play only once
                                if (runOnce)
                                {
                                    //Sets to last frame
                                    frameIndex = 0;
                                    //Pause animation
                                    isPaused = true;
                                }
                            }
                        }
                        //Reset frame time
                        time = 0f;
                    }
                }
                //Sets current frame rectangle based on frame number
                source = new Rectangle(frameIndex * frameWidth, 0, frameWidth, frameHeight);
                //Sets origin to animation center
                origin = new Vector2((frameWidth / 2.0f), (frameHeight / 2.0f));
                //Creates transformation matrix, for use in pixel perfect collision
                transformation =
                 Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                 Matrix.CreateScale(scale) *
                 Matrix.CreateRotationZ(angle) *
                 Matrix.CreateTranslation(new Vector3(position, 0.0f));

                //Updateshitbox for use in collision with vectors or points
                hitBox = new Rectangle((int)X - (int)(frameWidth * scale / 2), (int)Y - (int)(frameHeight * scale / 2), (int)(frameWidth * scale), (int)(frameHeight * scale));
            }
          
        }

        //Animation draw logic
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Only draws if active
            if (active)
            {
                //Draws current frame of sprite texture
                spriteBatch.Draw(spriteSheet, position, source, color, angle,
                  origin, scale, flip, 0f);
            }
        }
    }
}

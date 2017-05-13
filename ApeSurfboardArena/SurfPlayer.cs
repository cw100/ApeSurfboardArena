using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;

namespace ApeSurfboardArena
{
    class SurfPlayer:Player
    {
        public Animation animation;
        public Body body;
        public int hitPoints;
        public float direction;
        public Color color;
        public PlayerIndex playerIndex;
        Texture2D hitBar; Texture2D redBar;
        public SurfPlayer(Animation animation, Body body, Texture2D hitBar, Texture2D redBar, PlayerIndex playerIndex)
        {
            this.hitBar = hitBar;
            this.redBar = redBar;
            this.playerIndex = playerIndex;
            this.animation = animation;
            this.body = body;
            hitPoints = 100;
            if (playerIndex == PlayerIndex.One)
            {
                color = Color.White;
            }
            else
            {
                color = Color.Yellow;
            }
        }
        public void Move()
        {
            Vector2 force = new Vector2(0, 0);
        
                force = new Vector2(0, -GamePad.GetState(playerIndex).ThumbSticks.Left.Y * 50);

              

            


          
            body.ApplyLinearImpulse(force);
            body.LinearVelocity = new Vector2(body.LinearVelocity.X,MathHelper.Clamp(body.LinearVelocity.Y, -4, 4));
        }
        public override void Update(GameTime gameTime)
        {
            Move();
            animation.Position = ConvertUnits.ToDisplayUnits( body.Position ) + new Vector2(0,-150*animation.scale);
            
            animation.angle = body.Rotation;
            animation.color = color;
            animation.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle healthRectangle = new Rectangle((int)animation.X - redBar.Width / 2,
                                  (int)animation.Y - (int)(animation.frameHeight * animation.scale) / 2 + 10,
                                  (int)(redBar.Width),
                                  redBar.Height);

            spriteBatch.Draw(redBar, healthRectangle, Color.Red);
            float healthPercentage = (float)hitPoints / 100;
            float visibleWidth = (float)hitBar.Width * healthPercentage;

            healthRectangle = new Rectangle((int)animation.X - redBar.Width / 2,
                                           (int)animation.Y - (int)(animation.frameHeight * animation.scale) / 2 + 10,
                                           (int)(visibleWidth),
                                           hitBar.Height);

            spriteBatch.Draw(hitBar, healthRectangle, Color.Green);
            animation.Draw(spriteBatch);
        }

    }
}

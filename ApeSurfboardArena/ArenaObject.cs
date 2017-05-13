using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApeSurfboardArena
{
    public class ArenaObject
    {
        Animation animation;
       public bool active;
        public Body body;
        public ArenaObject(Animation animation, Body body)
        {
            this.animation = animation;
            this.body = body;
            active = true;
        }
        public void Update(GameTime gameTime )
        {
            animation.Position = ConvertUnits.ToDisplayUnits(body.Position);
            animation.Update(gameTime);
            animation.angle = body.Rotation;
        }
        public void Draw(SpriteBatch spriteBatch)

        {
            if (active)
            {
                animation.Draw(spriteBatch);
            }
        }

    }
}

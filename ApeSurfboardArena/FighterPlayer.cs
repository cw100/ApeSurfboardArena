using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApeSurfboardArena
{
    class FighterPlayer:Player
    {
        Body feetSensor;
        Animation currentAnimation;
        Vector2 position;
        Vector2 velocity;
        public Body body;
        PlayerIndex playerIndex;
        enum MoveState
        {
            idle,
            running,
            walking
        }
        enum JumpState
        {
            onGround,
            jumping
        }
        JumpState jumpState;
        MoveState moveState;
        public FighterPlayer(Dictionary<string, Animation> animations, Body body,Body feetSensor, PlayerIndex playerIndex)
        {
            jumpState = JumpState.jumping;
            this.feetSensor = feetSensor;
            this.playerIndex = playerIndex;

            this.currentAnimation = animations["idle"];
            this.body = body;
            moveState = MoveState.idle;
            
            feetSensor.OnCollision += new OnCollisionEventHandler(GroundCollison);
           

        }
        
        private bool GroundCollison(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            jumpState = JumpState.onGround;
         
            return true;
        }
        public void Punch()
        {
            Body punchSensor = BodyFactory.CreateRectangle(Game1.world, ConvertUnits.ToSimUnits(150),
                      ConvertUnits.ToSimUnits(251), 10.0f);
        }
        public void Move(GameTime gameTime)
        {
            Vector2 force = new Vector2(0, 0);
            if (body.LinearVelocity.X < 7 && GamePad.GetState(playerIndex).ThumbSticks.Left.X > 0)
            {
                force = new Vector2(GamePad.GetState(playerIndex).ThumbSticks.Left.X * 75, 0);

          

            }
           if (body.LinearVelocity.X > -7 && GamePad.GetState(playerIndex).ThumbSticks.Left.X < 0)
                {
                    force = new Vector2(GamePad.GetState(playerIndex).ThumbSticks.Left.X * 75, 0);

                    

                }



            body.ApplyLinearImpulse(force);
            body.LinearVelocity = new Vector2(MathHelper.Clamp(body.LinearVelocity.X,-7,7), body.LinearVelocity.Y);


        }
        public void Jump(GameTime gameTime)
        {
      
        if(GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed&& jumpState != JumpState.jumping)
            {
                Vector2 force= new Vector2(0, -20000);
                body.ApplyForce(force);
                jumpState = JumpState.jumping;
              

            }
        
        
    }
       public void DirectionChanger()
        {
            if(body.LinearVelocity.X > 0.05f)
            {
                currentAnimation.flip = SpriteEffects.None;
            }
            if (body.LinearVelocity.X < -0.05f)
            {
                currentAnimation.flip = SpriteEffects.FlipHorizontally;
            }
        }
        public override void Update(GameTime gameTime)
        {
          
           Move(gameTime);
            Jump(gameTime);
            DirectionChanger();
            currentAnimation.Position = ConvertUnits.ToDisplayUnits(body.Position) + new Vector2(0, -20);
            body.Rotation = 0;
            feetSensor.Position = body.Position;
            feetSensor.Rotation = body.Rotation;
            feetSensor.Awake = true;
            currentAnimation.scale = 0.66f;
            currentAnimation.angle = body.Rotation;
            currentAnimation.Update(gameTime);
           
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            currentAnimation.Draw(spriteBatch);
        }
    }
}

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
    public class FighterPlayer:Player
    {
        Dictionary<string, Animation> animations;
        public int hitPoints;
        Body punchSensor;
        Dictionary<string, Animation> attackAnimations;
        Body feetSensor;
        Animation attackAnimation;
        Animation currentAnimation;
        SpriteEffects flip;
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
        enum AttackState
        {
            none,
            punching
        }

        
        enum JumpState
        {
            onGround,
            jumping
        }
        AttackState attackState;
        JumpState jumpState;
        MoveState moveState;
        public FighterPlayer(Dictionary<string, Animation> animations, Dictionary<string, Animation> attackAnimations, Body body,Body feetSensor, PlayerIndex playerIndex)
        {
            hitPoints = 100;
            this.animations = animations;
            this.attackAnimations = attackAnimations;
            jumpState = JumpState.onGround;
            this.feetSensor = feetSensor;
            this.playerIndex = playerIndex;

            this.currentAnimation = animations["idle"];
            this.body = body;
            moveState = MoveState.idle;
            attackState = AttackState.none;
            feetSensor.OnCollision += new OnCollisionEventHandler(GroundCollison);
           

        }
        
        private bool GroundCollison(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            jumpState = JumpState.onGround;
            animations["jump"].frameIndex = 0;
            animations["jump"].isPaused = false;
            return true;
        }
        public void Punch()
        {
            if (GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed && jumpState == JumpState.onGround&& attackState== AttackState.none)
            {
                attackState = AttackState.punching;
                body.LinearVelocity = new Vector2(0, body.LinearVelocity.Y);

                
            }
            if(attackState == AttackState.punching)
            {
                if (animations["punch"].frameIndex == 3)
                {
                    if (attackAnimation == null)
                    {
                        attackAnimation = attackAnimations["punch"];
                        
                         punchSensor = BodyFactory.CreateRectangle(Game1.world, ConvertUnits.ToSimUnits(attackAnimation.frameWidth* 0.825),
                              ConvertUnits.ToSimUnits(attackAnimation.frameWidth* 0.825), 10.0f);
                        punchSensor.Awake = true;
                        if (flip == SpriteEffects.None)

                        {
                            punchSensor.Position =new Vector2(body.Position.X, body.Position.Y) + ConvertUnits.ToSimUnits (new Vector2(30, -20));
                        }
                        else
                        {

                            punchSensor.Position = new Vector2(body.Position.X, body.Position.Y) + ConvertUnits.ToSimUnits(new Vector2(-30, -20));
                        }
                        punchSensor.IsSensor = true;
                        punchSensor.BodyType = BodyType.Dynamic;
                        punchSensor.IgnoreCollisionWith(body);
                        punchSensor.OnCollision += new OnCollisionEventHandler(PunchDamage);
                        attackAnimation.Position = ConvertUnits.ToDisplayUnits(punchSensor.Position);
                    }
                }
                else
                {
                    if (punchSensor != null)
                    {
                       
                        punchSensor.Awake = false;
                        punchSensor.Dispose();
                    }
                    attackAnimation = null;
                }
                if (animations["punch"].isPaused )
                {
                    attackState = AttackState.none;
                    animations["punch"].isPaused = false;
                    animations["punch"].frameIndex = 0;
                }
            }
            
        }

        private bool PunchDamage(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            foreach (FighterPlayer player in Game1.fighterPlayers)
            {
                if (player.body.Equals(fixtureB.Body))
                {
                    player.hitPoints -= 10;
                    if (flip == SpriteEffects.None)
                    {
                        player.body.ApplyLinearImpulse(new Vector2(400,- 400));
                    }
                    else
                    {
                        player.body.ApplyLinearImpulse(new Vector2(-400, -400));
                     }
                }
               
            }
            return true;
        }
        public void StateSwitcher()
        {
            if (body.LinearVelocity.X > 4 || body.LinearVelocity.X < -4)
            {
                moveState = MoveState.running;
            }
            if (body.LinearVelocity.X < 4 && body.LinearVelocity.X > -4)
            {
                if (body.LinearVelocity.X < 0.01 && body.LinearVelocity.X > -0.01)
                {

                    moveState = MoveState.idle;
                }
                else
                {
                    moveState = MoveState.walking;
                }
            }
            if (jumpState != JumpState.jumping && attackState == AttackState.none)
            {
                switch (moveState)
                {
                    case MoveState.idle:
                        currentAnimation = animations["idle"];
                        break;
                    case MoveState.running:
                        currentAnimation = animations["run"];
                        break;
                    case MoveState.walking:
                        currentAnimation = animations["walk"];
                        break;


                    default:
                        break;
                }
            }
            else
            {
                if (jumpState == JumpState.jumping && attackState == AttackState.none)
                {
                    currentAnimation = animations["jump"];
                }
                else
                {
                    switch (attackState)
                    {
                        case AttackState.punching:
                            currentAnimation = animations["punch"];
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        public void Move(GameTime gameTime)
        {
            if (attackState ==AttackState.none)
            {
                Vector2 force = new Vector2(0, 0);
                if (body.LinearVelocity.X < 7 && GamePad.GetState(playerIndex).ThumbSticks.Left.X > 0)
                {
                    force = new Vector2(GamePad.GetState(playerIndex).ThumbSticks.Left.X * 75, 0);

                    flip = SpriteEffects.None;

                }
                if (body.LinearVelocity.X > -7 && GamePad.GetState(playerIndex).ThumbSticks.Left.X < 0)
                {
                    force = new Vector2(GamePad.GetState(playerIndex).ThumbSticks.Left.X * 75, 0);

                    flip = SpriteEffects.FlipHorizontally;

                }



                body.ApplyLinearImpulse(force);
                body.LinearVelocity = new Vector2(MathHelper.Clamp(body.LinearVelocity.X, -7, 7), body.LinearVelocity.Y);

            }
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
       
        public override void Update(GameTime gameTime)
        {
          
           Move(gameTime);
            Jump(gameTime);
            Punch();
            StateSwitcher();
            if (attackAnimation != null)
            {
                attackAnimation.flip = flip;
                attackAnimation.scale = 0.825f;
                attackAnimation.Update(gameTime);
            }
                currentAnimation.flip = flip;
            currentAnimation.Position = ConvertUnits.ToDisplayUnits(body.Position) + new Vector2(0, -10);
            body.Rotation = 0;
            feetSensor.Position = body.Position;
            feetSensor.Rotation = body.Rotation;
            feetSensor.Awake = true;
            currentAnimation.scale = 0.825f;
            currentAnimation.angle = body.Rotation;
            currentAnimation.Update(gameTime);
           
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            currentAnimation.Draw(spriteBatch);
        }
        public  void DrawAttack(SpriteBatch spriteBatch)
        {

            if (attackAnimation != null)
            {
                attackAnimation.Draw(spriteBatch);
            }
        }
    }
}

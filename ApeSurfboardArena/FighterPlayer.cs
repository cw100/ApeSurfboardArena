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
        Color firstColor;
        public bool active;
        public Body body;
        public Texture2D redBar;
        public PlayerIndex playerIndex;
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
            jumping,
            falling
        }
        ButtonState lastState;
        AttackState attackState;
        JumpState jumpState;
        MoveState moveState;
        float timeSinceLastJump;
        Texture2D healthtexture;
        public FighterPlayer(Dictionary<string, Animation> animations, Dictionary<string, Animation> attackAnimations, Texture2D healthtexture,Texture2D redBar, Body body,Body feetSensor, PlayerIndex playerIndex)
        {
            this.redBar = redBar;
            active = true;
            timeSinceLastJump = 0;
            hitPoints = 100;
            this.animations = animations;
            this.attackAnimations = attackAnimations;
            jumpState = JumpState.onGround;
            this.feetSensor = feetSensor;
            this.playerIndex = playerIndex;
            lastState = ButtonState.Released;
            this.currentAnimation = animations["idle"];
            this.body = body;
            moveState = MoveState.idle;
            attackState = AttackState.none;
            feetSensor.OnCollision += new OnCollisionEventHandler(GroundCollison);

            this.healthtexture = healthtexture;
            if (playerIndex == PlayerIndex.One)
            {
                firstColor = Color.White;
            }
            else
            {
                firstColor = Color.Yellow;
            }
            color = firstColor;

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
            if (GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed && attackState== AttackState.none&&active)
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
        public Color color = Color.White;
        private bool PunchDamage(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {

            foreach (FighterPlayer player in Game1.fighterPlayers)
            {
                if (player.body.Equals(fixtureB.Body))
                {
                    player.hitPoints -= 10;
                    player.timeSinceDamage = 0;
                    player.color = Color.Red;
                    if (flip == SpriteEffects.None)
                    {
                        player.body.ApplyLinearImpulse(new Vector2(200,- 200));
                    }
                    else
                    {
                        player.body.ApplyLinearImpulse(new Vector2(-200, -200));
                     }
                }
               
            }
            if(fixtureB.CollisionCategories == Category.All)
            {
                if (flip == SpriteEffects.None)
                {                    
                    fixtureB.Body.ApplyLinearImpulse(new Vector2(200, -300));
                }
                    else
                    {
                    fixtureB.Body.ApplyLinearImpulse(new Vector2(-200, -300));
                }
            }
            return true;
        }
        public void StateSwitcher()
        {
            if (active)
            {
                if (body.LinearVelocity.X > 4 || body.LinearVelocity.X < -4)
                {
                    moveState = MoveState.running;
                }
                if (body.LinearVelocity.X < 4 && body.LinearVelocity.X > -4)
                {
                    if (body.LinearVelocity.X < 0.02 && body.LinearVelocity.X > -0.02)
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
            else
            {
                currentAnimation = animations["death"];
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

            timeSinceLastJump += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed&& lastState  == ButtonState.Released&& jumpState == JumpState.onGround&& timeSinceLastJump>0.75f)
            {
                body.ApplyLinearImpulse(new Vector2(0, -300));
                jumpState = JumpState.jumping;
           
                timeSinceLastJump = 0;
            }
            
                lastState = GamePad.GetState(playerIndex).Buttons.A;

        }
        public float timeSinceDamage;
        
        public override void Update(GameTime gameTime)
        {
            timeSinceDamage +=(float) gameTime.ElapsedGameTime.TotalSeconds;
            
            if (hitPoints <=0)
            {
                active = false;
            }
            if (active)
            {
                Move(gameTime);
                Jump(gameTime);
               
            }
            Punch();
            StateSwitcher();
            if (attackAnimation != null)
            {
                attackAnimation.flip = flip;
                attackAnimation.scale = 0.825f;
                attackAnimation.Update(gameTime);
            }
                currentAnimation.flip = flip;
            currentAnimation.Position = ConvertUnits.ToDisplayUnits(body.Position) + new Vector2(0, -5);
            body.Rotation = 0;
            feetSensor.Position = body.Position ;
            feetSensor.Rotation = body.Rotation;
            feetSensor.Awake = true;
      
            var currentNode = feetSensor.ContactList;
         
            while ((currentNode != null) && (currentNode.Contact.IsTouching != true))
            {
                

                currentNode = currentNode.Next;
            }
            if ((currentNode == null))
            {

                body.Friction = 0f;
                if ((body.ContactList != null))
                {
                   
                        body.ContactList.Contact.Friction = 0f;
                    
                }
                if (jumpState != JumpState.jumping)
                {
                    jumpState = JumpState.falling;
                }
            }
            else
            {

                body.ContactList.Contact.Friction = 0.6f;
                    body.Friction = 0.8f;



            }
            currentAnimation.scale = 0.825f;
            currentAnimation.angle = body.Rotation;

            if (timeSinceDamage > 0.25f)
            {
                color = firstColor;
            }
            if(!active)
            {
                color = firstColor;
            }
            currentAnimation.color = color;
            currentAnimation.Update(gameTime);
           
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle healthRectangle = new Rectangle((int)currentAnimation.X- redBar.Width/2,
                                          (int)currentAnimation.Y  - (int)(currentAnimation.frameHeight * currentAnimation.scale) / 2 + 10,
                                          (int)(redBar.Width),
                                          redBar.Height);

            spriteBatch.Draw(redBar, healthRectangle, Color.Red);
            float healthPercentage =(float) hitPoints / 100;
            float visibleWidth = (float)healthtexture.Width * healthPercentage;

            healthRectangle = new Rectangle((int)currentAnimation.X - redBar.Width / 2,
                                           (int)currentAnimation.Y -(int)( currentAnimation.frameHeight * currentAnimation.scale) / 2 +10,
                                           (int)(visibleWidth),
                                           healthtexture.Height);

            spriteBatch.Draw(healthtexture, healthRectangle, Color.Green);


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

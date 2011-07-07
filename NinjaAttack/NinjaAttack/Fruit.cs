using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NinjaAttack
{
    public class Fruit : GameObject
    {
        private int startTime;
        private Animation appleDie;
        private Animator animator;
        private Texture2D block;

        public Fruit(NinjaGame game, Vector2 startPos, Vector2 initialVelocity, int start)
        {
            this.Game = game;
            this.IsAlive = true;
            this.Position = startPos;
            this.Velocity = initialVelocity;
            startTime = start;
            LoadContent();
        }

        private void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("apple");
            block = Game.Content.Load<Texture2D>("black");
            appleDie = new Animation(Game.Content.Load<Texture2D>("appledice"), 0.02f, false, "die");
            animator = new Animator(appleDie);
        }

        public void Draw(SpriteBatch sprite, GameTime time)
        {
            if (!this.IsAlive)
            {
                animator.Draw(sprite, this.Position, time);                
                return;
            }
            sprite.Draw(Texture, Position, null, Color.White, 0, new Vector2(25.0f, 25.0f), 1, SpriteEffects.None, 0);
        }

        public void Update(GameTime time)
        {
            if (!this.IsAlive)
            {
                animator.Play(appleDie, SpriteEffects.None);
            }
            Velocity -= (float)time.ElapsedGameTime.TotalSeconds * Game.gravity;

            
            Position += Velocity * (float)time.ElapsedGameTime.TotalSeconds;
            Vector2 xVec = new Vector2(Velocity.X,0);
            Position += xVec;
        }
    }
}

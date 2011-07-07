using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NinjaAttack
{
    public class NinjaPlayer
    {
        private NinjaGame mGame;

        private Vector2 realHandPosition = new Vector2(400, 300);
        private Texture2D handTexture;
        private Texture2D block;

        private float handSensitivity = 1.5f;

        public NinjaPlayer(NinjaGame game)
        {
            mGame = game;
            LoadContent();
        }

        private void LoadContent()
        {
            handTexture = mGame.Content.Load<Texture2D>("sword");
            block = mGame.Content.Load<Texture2D>("black");
        }

        public float Sensitivity
        {
            get { return handSensitivity; }
            set { handSensitivity = value; }
        }

        public Rectangle Hand
        {
            get { return new Rectangle((int)realHandPosition.X - 50, (int)realHandPosition.Y - 50, handTexture.Width, handTexture.Height); }
        }

        public void UpdateHand(Vector2 hand)
        {

            //first, check to see if the ninja's hand is in the correct spot
            if (hand.X >= 200 && hand.X <= 600 && hand.Y <= 300)
            {
                //the y is easy, just multiply the current hand by y
                realHandPosition.Y = hand.Y * 2;
                //then, since the x is offset by 200, you have to subtract that much and multiply by 2
                realHandPosition.X = (hand.X - 200) * 2;
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(handTexture, realHandPosition, null, Color.White, 0.0f, new Vector2(50.0f, 50.0f), 1, SpriteEffects.None, 0);
        }
    }
}

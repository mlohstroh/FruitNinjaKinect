using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NinjaAttack
{
    public abstract class GameObject
    {
        private Texture2D mTexture;

        public Texture2D Texture
        {
            get { return mTexture; }
            set { mTexture = value; }
        }

        private Vector2 mPosition;

        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        private Vector2 mVelocity;

        public Vector2 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        private NinjaGame mGame;

        public NinjaGame Game
        {
            get { return mGame; }
            set { mGame = value; }
        }

        private bool mIsAlive;

        public bool IsAlive
        {
            get { return mIsAlive; }
            set { mIsAlive = value; }
        }

        public Rectangle BoundingRectangle
        {
            get { return new Rectangle((int)mPosition.X - 25, (int)mPosition.Y - 25, mTexture.Width, mTexture.Height); }
        }
    }
}

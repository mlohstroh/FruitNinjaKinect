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
    }
}

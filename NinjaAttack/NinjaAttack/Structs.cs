using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace NinjaAttack
{
    public struct FruitData
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public FruitData(Vector2 pos, Vector2 vel)
        {
            this.Position = pos;
            this.Velocity = vel;
        }
    }
}

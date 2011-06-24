using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.Audio;

namespace NinjaAttack
{
    
    public class NinjaGame
    {
        private Runtime kinectDevice;
        private ContentManager mContent;

        public NinjaGame(IServiceProvider services)
        {
            mContent = new ContentManager(services);
        }

        public ContentManager Content
        {
            get { return mContent; }
            set { mContent = value; }
        }
    }
}

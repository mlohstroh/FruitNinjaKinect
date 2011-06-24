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
        private NinjaPlayer player;
        private Texture2D backGround;

        public NinjaGame(IServiceProvider services)
        {
            mContent = new ContentManager(services, "Content");
            LoadContent();
        }

        public ContentManager Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        private void LoadContent()
        {
            kinectDevice = new Runtime();
            kinectDevice.Initialize(RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking);
            
            kinectDevice.SkeletonEngine.TransformSmooth = true;

            kinectDevice.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectDevice_SkeletonFrameReady);

            player = new NinjaPlayer(this);

            backGround = Content.Load<Texture2D>("wood_paneling");

        }

        void kinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
        }

        public void Draw(SpriteBatch sprite, GameTime time)
        {
            //draw background first
            sprite.Draw(backGround, new Rectangle(0, 0, 800, 600), Color.White);

            //then draw needed objects
            player.Draw(sprite);
            
        }

        public void Update(GameTime time)
        {
        }
    }
}

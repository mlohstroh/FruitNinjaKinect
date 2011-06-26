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
        private Texture2D kinectRGBVideo;
        private GraphicsDevice mDevice;

        //easy to change
        public Vector2 gravity = new Vector2(0, -175.0f);

        

        private List<GameObject> mObjects = new List<GameObject>();

        private List<FruitData> fruitToAddNextUpdate = new List<FruitData>();

        public NinjaGame(IServiceProvider services, GraphicsDevice device)
        {
            mContent = new ContentManager(services, "Content");
            mDevice = device;
            LoadContent();
            fruitToAddNextUpdate.Add(new FruitData(new Vector2(200, 600), new Vector2(2 , -400)));
        }



        public ContentManager Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        private void LoadContent()
        {
            kinectDevice = new Runtime();
            kinectDevice.Initialize(RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            
            kinectDevice.SkeletonEngine.TransformSmooth = true;
            kinectDevice.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);

            kinectDevice.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectDevice_SkeletonFrameReady);
            kinectDevice.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinectDevice_VideoFrameReady);

            player = new NinjaPlayer(this);

            backGround = Content.Load<Texture2D>("wood_paneling");
            
        }

        void kinectDevice_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage p = e.ImageFrame.Image;

            Color[] color = new Color[p.Height * p.Width];
            kinectRGBVideo = new Texture2D(mDevice, p.Width, p.Height);

            int index = 0;
            for (int y = 0; y < p.Height; y++)
            {
                for (int x = 0; x < p.Width; x++, index += 4)
                {
                    color[y * p.Width + x] = new Color(p.Bits[index + 2], p.Bits[index + 1], p.Bits[index + 0]);
                }
            }
            kinectRGBVideo.SetData<Color>(color);
        }

        void kinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            foreach (SkeletonData skeleton in e.SkeletonFrame.Skeletons)
            {
                //make sure he's just the first players
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in skeleton.Joints)
                    {
                        if (joint.ID == JointID.HandRight)
                        {
                            //player.UpdateHand(joint.Position);
                            float depthX, depthY;
                            kinectDevice.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);

                            depthX = Math.Max(0, Math.Min(depthX * 320, 320));
                            depthY = Math.Max(0, Math.Min(depthY * 240, 240));  


                            int colorX, colorY;
                            kinectDevice.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, new ImageViewArea(), (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

                            player.UpdateHand(new Vector2((float)(800 * colorX / 640.0), (float)(600 * colorY / 480.0)));
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sprite, GameTime time)
        {
            //draw background first
            sprite.Draw(backGround, new Rectangle(0, 0, 800, 600), Color.White);
            if (kinectRGBVideo != null)
            {
                sprite.Draw(kinectRGBVideo, new Rectangle(0, 0, 160, 120), Color.White);
            }
            //then draw needed objects

            foreach (GameObject obj in mObjects)
            {
                if (obj.GetType() == typeof(Fruit))
                {
                    Fruit fruit = (Fruit)obj;
                    fruit.Draw(sprite ,time);
                }
            }

            player.Draw(sprite);
            
        }

        public void Update(GameTime time)
        {
            
            foreach (FruitData data in this.fruitToAddNextUpdate)
            {
                this.mObjects.Add(new Fruit(this, data.Position, data.Velocity, time.TotalGameTime.Milliseconds));
            }
            //then clear
            fruitToAddNextUpdate.Clear();
            foreach (GameObject obj in mObjects)
            {
                if (obj.GetType() == typeof(Fruit))
                {
                    Fruit fruit = (Fruit)obj;
                    fruit.Update(time);
                }
            }
        }
    }
}

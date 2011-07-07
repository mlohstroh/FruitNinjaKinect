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
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using System.IO;

namespace NinjaAttack
{
    
    public class NinjaGame
    {
        private int elapsedMili = 0;

        private SpriteFont font;

        private const string RecognizerId = "SR_MS_en-US_Kinect_10.0";

        //private int gameWave;

        private int playerScore = 0;

        private Runtime kinectDevice;
        private ContentManager mContent;
        private NinjaPlayer player;
        private Texture2D backGround;
        private Texture2D kinectRGBVideo;        
        private GraphicsDevice mDevice;

        //Kinect Speech stuff
        private KinectAudioSource kinectAudio;
        private RecognizerInfo ri;
        private SpeechRecognitionEngine sre;
        private Choices audioChoices;
        private GrammarBuilder grammerBuilder;
        private Grammar grammer;

        //easy to change
        public Vector2 gravity = new Vector2(0, -175.0f);

        private bool canThrowMoreFruit = true;

        private List<GameObject> mObjects = new List<GameObject>();

        private List<FruitData> fruitToAddNextUpdate = new List<FruitData>();
        private List<Fruit> fruitToRemoveNextUpdate = new List<Fruit>();
        private NinjaAttack startInstance;

        public NinjaGame(IServiceProvider services, GraphicsDevice device, NinjaAttack starter)
        {
            mContent = new ContentManager(services, "Content");
            mDevice = device;
            LoadContent();
            //gameWave = 1;
            startInstance = starter;
        }

        public ContentManager Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        private void LoadContent()
        {
            kinectDevice = new Runtime();
            kinectDevice.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);

            kinectDevice.SkeletonEngine.TransformSmooth = true;
            kinectDevice.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            kinectDevice.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

            kinectDevice.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectDevice_SkeletonFrameReady);
            kinectDevice.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinectDevice_VideoFrameReady);            

            kinectAudio = new KinectAudioSource();

            kinectAudio.FeatureMode = true;
            kinectAudio.AutomaticGainControl = false;
            kinectAudio.SystemMode = SystemMode.OptibeamArrayOnly;

            ri = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();
            sre = new SpeechRecognitionEngine(ri.Id);
            audioChoices = new Choices();
            audioChoices.Add("stop");
            audioChoices.Add("start");
            audioChoices.Add("kinect close");
            audioChoices.Add("increase sensitivity");
            audioChoices.Add("decrease sensitivity");
            audioChoices.Add("reset hand");
            grammerBuilder = new GrammarBuilder();
            grammerBuilder.Culture = ri.Culture;
            grammerBuilder.Append(audioChoices);
            grammer = new Grammar(grammerBuilder);

            sre.LoadGrammar(grammer);

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);

            sre.SetInputToAudioStream(kinectAudio.Start(), new SpeechAudioFormatInfo( EncodingFormat.Pcm, 16000,16,1,32000,2, null));
            sre.RecognizeAsync(RecognizeMode.Multiple);

            player = new NinjaPlayer(this);

            backGround = Content.Load<Texture2D>("wood_paneling");
            font = Content.Load<SpriteFont>("font");

        }

        public void StopKinect()
        {            
            sre.RecognizeAsyncStop();
            kinectDevice.Uninitialize();
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            
            switch (e.Result.Text)
            {
                case "start":
                    this.canThrowMoreFruit = true;
                    break;
                case "stop":
                    this.canThrowMoreFruit = false;
                    break;
                case "kinect close":
                    //this.startInstance.Exit();
                    break;
           }
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

                            float depthX, depthY;
                            kinectDevice.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);

                            depthX = Math.Max(0, Math.Min(depthX * 320, 320));
                            depthY = Math.Max(0, Math.Min(depthY * 240, 240));


                            int colorX, colorY;
                            kinectDevice.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, new ImageViewArea(), (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

                            player.UpdateHand(new Vector2((float)(800 * (colorX) / 640.0), (float)(600 * (colorY) / 480.0)));
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
            sprite.DrawString(font, "Player Score: " + playerScore.ToString(), new Vector2(0, 560), Color.DarkCyan);
        }

        public void Update(GameTime time)
        {
            elapsedMili += time.ElapsedGameTime.Milliseconds;

            //throw a fruit a second
            if (elapsedMili >= 100 && canThrowMoreFruit)
            {
                AddFruit();
                //then reset time
                elapsedMili = 0;
            }
            foreach (Fruit fruit in fruitToRemoveNextUpdate)
            {
                mObjects.Remove(fruit);
            }
            
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
                    if (CheckForCollision(fruit))
                    {
                        fruit.IsAlive = false;
                        fruitToRemoveNextUpdate.Add(fruit);
                        playerScore += 5;
                    }
                    fruit.Update(time);
                    if (fruit.Position.Y >= 600)
                    {
                        fruitToRemoveNextUpdate.Add(fruit);
                    }
                }
                
            }
        }

        private void AddFruit()
        {
            Random randomChooser = new Random(DateTime.Now.Millisecond);
            int xPos = randomChooser.Next(0, 800); 
            int xVel = randomChooser.Next(-3,4);
            int yVel = randomChooser.Next(-500, -350);

            fruitToAddNextUpdate.Add(new FruitData(new Vector2(xPos,600), new Vector2(xVel, yVel)));
        }

        private bool CheckForCollision(Fruit fruit)
        {
            if (player.Hand.Intersects(fruit.BoundingRectangle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

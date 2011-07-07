//*************************************
//  Author: Mark Lohstroh
//*************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace NinjaAttack
{
    public class Animator
    {
        private Animation mAnimation;
               

        /// <summary>
        /// Gets or sets the animation
        /// </summary>
        public Animation Animation
        {
            get { return mAnimation; }
            set { mAnimation = value; }
        }

        /// <summary>
        /// Basic constructor
        /// <remarks>You will need to add your animation later</remarks>
        /// </summary>
        public Animator()
        {
            //TODO: Set this up later                        
        }
        
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <remarks>Using this should be the norm for an animator</remarks>
        /// <param name="anim">The animation to play</param>
        public Animator(Animation anim)
        {
            //Set up the rectangle
            mFrameRect = new Rectangle(0, 0, anim.FrameWidth, anim.FrameHeight);
            Animation = anim;
        }
        
        private int mCurrentFrame;

        /// <summary>
        /// Gets or sets the current frame
        /// <remarks>Used in advancing the frame</remarks>
        /// </summary>
        public int CurrentFrame
        {
            get { return mCurrentFrame; }
            set { mCurrentFrame = value; }
        }

        private Rectangle mFrameRect;

        /// <summary>
        /// Gets or set the rectangle that will be drawn in the texture
        /// </summary>
        public Rectangle FrameRect
        {
            get { return mFrameRect; }
            set { mFrameRect = value; }
        }
        
        private float mElaspedTime;

        private SpriteEffects mEffects;

        /// <summary>
        /// Plays a certain animation
        /// </summary>
        /// <param name="anim">The animation to play</param>
        /// <param name="effects">The sprite effect to use</param>
        public void Play(Animation anim,SpriteEffects effects)
        {
            if (Animation.Name == anim.Name)
            {
                //Don't do anything
                return;
            }
            //Reset the current animation
            mEffects = effects;
            Animation = anim;
            mFrameRect.X = 0;
            mElaspedTime = 0;
            mCurrentFrame = 0;
        }

        /// <summary>
        /// Draws the animation
        /// </summary>
        /// <param name="spritebatch">So it can draw</param>
        /// <param name="newvec">The vector position to draw at</param>
        /// <param name="time">The current game time</param>
        public void Draw(SpriteBatch spritebatch, Vector2 newvec, GameTime time)
        {            
            //TODO: Fix the fliker problem when restarting the animation


            //Calculates the rectangle at the vector and the frame height and width
            Rectangle recttodraw = new Rectangle((int)newvec.X, (int)newvec.Y, Animation.FrameWidth, Animation.FrameHeight);
            //Get the total elapsed time
            mElaspedTime += (float)time.ElapsedGameTime.TotalSeconds;

            //Check frame time
            if (mElaspedTime >= Animation.FrameTime)
            {
                //Advance and draw
                //The - 1 is needed because of the OBOB in the rectangle                
                if (mCurrentFrame >= Animation.FrameCount - 1)
                {
                    if (Animation.IsLooping)
                    {
                        //Restart the animation                    
                        mFrameRect.X = 0;
                        mElaspedTime = 0.0f;
                        mCurrentFrame = 0;
                    }
                    else //Show last frame
                        spritebatch.Draw(Animation.Texture, recttodraw, mFrameRect, Color.White, 0.0f, new Vector2(25.0f, 25.0f), mEffects, 0);
                        return;
                }
                else
                {
                    //Advance frame and reset the time
                    mFrameRect.X += Animation.FrameWidth;
                    mCurrentFrame++;
                    mElaspedTime = 0.0f;
                }
            }            
            //Draw a specific part of the texture at a certain vector on the window
            spritebatch.Draw(Animation.Texture, recttodraw, mFrameRect, Color.White, 0.0f, new Vector2(25.0f, 25.0f), mEffects, 0);
        }
    }
}

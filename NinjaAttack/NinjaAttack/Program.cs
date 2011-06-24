using System;

namespace NinjaAttack
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NinjaAttack game = new NinjaAttack())
            {
                game.Run();
            }
        }
    }
#endif
}


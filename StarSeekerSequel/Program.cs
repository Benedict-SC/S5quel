using System;

namespace StarSeekerSequel
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new StarSequel())
                game.Run();
        }
    }
}

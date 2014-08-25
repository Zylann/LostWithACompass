using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    /// <summary>
    /// This program is a Ludum Dare 48 compo. Don't expect well written code :'D
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Universe!");

            //try
            //{
            Assets.root = "Assets";
            LD30.Game game = new LD30.Game();

            Application app = new Application(game);
            app.Start();
            //}
            //catch(Exception e)
            //{
            //	Console.WriteLine("Fatal exception: " + e.Message);
            //}

            //Console.WriteLine("Press any key to dismiss...");
            //Console.Read();

            Console.WriteLine("Bye Universe!");
        }
    }
}

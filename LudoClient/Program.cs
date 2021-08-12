using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameClient;

namespace LudoClient
{
    static class Program
    {
        static GClient GameClient;
        static Form1 login;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GameClient = new GClient(9999);
            GameClient.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1(GameClient));
            login = new Form1(GameClient);
            Application.Run(login);
        }
    }
}

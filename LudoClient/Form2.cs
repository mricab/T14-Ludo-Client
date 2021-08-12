using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameClient;

namespace LudoClient
{
    public partial class Form2 : Form, IGClient2
    {
        GClient GameClient;
        Form1 login;
        static Form3 match;
        public Form2(GClient GameClient, Form1 login)
        {
            InitializeComponent();
            this.GameClient = GameClient;
            GameClient.RegisterObserver2(this);
            this.login = login;
            match = new Form3(GameClient, this);
        }

        // Control Events
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            GameClient.Send("logout");
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            StartGameButton.Visible = false;
            StartGameAButton.Visible = true;
            StartGameBButton.Visible = true;
            EnableJoinGameButton.Visible = false;
            JoinGameButton.Visible = false;
            JoinGameTextField.Visible = false;
            CancelStartGameButton.Visible = true;
        }

        private void StartGameAButton_Click(object sender, EventArgs e)
        {
            Invoke(LaunchGameHandler, this, "A");
            Invoke(CloseFormHandler, this);
            GameClient.StartGame("A");
        }

        private void StartGameBButton_Click(object sender, EventArgs e)
        {
            Invoke(LaunchGameHandler, this, "B");
            Invoke(CloseFormHandler, this);
            GameClient.StartGame("B");
        }

        private void EnableJoinGameButton_Click(object sender, EventArgs e)
        {
            StartGameButton.Visible = false;
            StartGameAButton.Visible = false;
            StartGameBButton.Visible = false;
            EnableJoinGameButton.Visible = false;
            JoinGameButton.Visible = true;
            JoinGameTextField.Visible = true;
            CancelStartGameButton.Visible = true;
        }

        private void JoinGameButton_Click(object sender, EventArgs e)
        {
            int id;
            int.TryParse(JoinGameTextField.Text, out id);
            Invoke(JoinGameHandler, this, id);
            Invoke(CloseFormHandler, this);
            GameClient.JoinGame(id);
        }

        private void CancelStartGameButton_Click(object sender, EventArgs e)
        {
            StartGameButton.Visible = true;
            StartGameAButton.Visible = false;
            StartGameBButton.Visible = false;
            EnableJoinGameButton.Visible = true;
            JoinGameButton.Visible = false;
            JoinGameTextField.Visible = false;
            CancelStartGameButton.Visible = false;
        }

        // IGCLient2
        void IGClient2.OnGLogout(GLogoutEvent e)
        {
            Invoke(LaunchLoginFormHandler, this);
            Invoke(CloseFormHandler, this);            
        }

        // Form Control
        private delegate void CloseFormDelegate(Form2 form);                // Close Form
        CloseFormDelegate CloseFormHandler = CloseForm;
        private static void CloseForm(Form2 form)
        {
            form.Hide();
        }

        private delegate void LaunchLoginFormDelegate(Form2 form);          // Back to login
        LaunchLoginFormDelegate LaunchLoginFormHandler = LaunchLoginForm;
        private static void LaunchLoginForm(Form2 form)
        {
            //Form1 login = new Form1(form.GameClient);
            form.login.Show();
        }

        private delegate void LaunchGameDelegate(Form2 form, String game);  // New Game
        LaunchGameDelegate LaunchGameHandler = LaunchGameForm;
        private static void LaunchGameForm(Form2 form, String game)
        {
            //Form3 match = new Form3(form.GameClient, game);
            match.Show();
        }

        private delegate void JoinGameDelegate(Form2 form, int gameId); // Join Game
        JoinGameDelegate JoinGameHandler = JoinGameForm;
        private static void JoinGameForm(Form2 form, int gameId)
        {
            //Form3 match = new Form3(form.GameClient, gameId);
            match.Show();
        }

    }
}

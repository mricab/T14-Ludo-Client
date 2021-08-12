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
    public partial class Form1 : Form, IGClient
    {
        GClient GameClient;
        static Form2 menu;

        public bool State
        {
            get
            {
                return UsernameTextbox.Enabled 
                    && PasswordTextbox.Enabled 
                    && LoginButton.Enabled
                    && RegisterButton.Enabled;
            }
            set {
                UsernameTextbox.Enabled = value;
                PasswordTextbox.Enabled = value;
                LoginButton.Enabled = value;
                RegisterButton.Enabled = value;
            }
        }

        public Form1(GClient GameClient)
        {
            InitializeComponent();
            this.GameClient = GameClient;            
            GameClient.RegisterObserver(this);
            if(GameClient.IsConnected()) { State = true; }
            menu = new Form2(GameClient, this);
        }

        // Control events
        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoginErrorLabel.Enabled = false;

            if (ValidateLoginData(UsernameTextbox.Text, PasswordTextbox.Text))
            {
                GameClient.Login(UsernameTextbox.Text, PasswordTextbox.Text);
            }
            else
            {
                LoginErrorLabel.Enabled = true;
                LoginErrorLabel.Text = "Nombre de usuario o password inválido(s)";
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            LoginErrorLabel.Enabled = false;

            if (ValidateRegisterData(UsernameTextbox.Text, PasswordTextbox.Text))
            {
                GameClient.Send("register", new string[] { UsernameTextbox.Text, PasswordTextbox.Text });
            }
            else
            {
                LoginErrorLabel.Enabled = true;
                LoginErrorLabel.Text = "El nombre de usuario o password es muy corto";
            }
        }

        private void RegisterLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginButton.Visible = false;
            RegisterButton.Visible = true;
            RegisterLink.Visible = false;
            LoginLink.Visible = true;
        }

        private void LoginLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginButton.Visible = true;
            RegisterButton.Visible = false;
            RegisterLink.Visible = true;
            LoginLink.Visible = false;
        }

        // IGCLient
        void IGClient.OnGConnected(GConnectedEvent e)
        {
            Invoke(SetLoginStateHandler, this, true);
        }

        void IGClient.OnGDisconnected(GDisconnectedEvent e)
        {
            Invoke(SetLoginStateHandler, this, false);
        }

        void IGClient.OnGLogin(GLoginEvent e)
        {
            Invoke(LaunchMenuFormHandler, this);
            Invoke(CloseFormHandler, this);
        }

        void IGClient.OnGLoginFailure(GLoginFailureEvent e)
        {
            Invoke(SetErrorHandler, this, e.data.message);
        }

        // Form control
        private delegate void SetLoginStateDelegate(Form1 form, bool state);
        SetLoginStateDelegate SetLoginStateHandler = SetLoginState;
        private static void SetLoginState(Form1 form, bool state)
        {
            form.State = state;
        }

        private delegate void LaunchMenuFormDelegate(Form1 form);
        LaunchMenuFormDelegate LaunchMenuFormHandler = LaunchMenuForm;
        private static void LaunchMenuForm(Form1 form)
        {
            menu.Show();
        }

        private delegate void CloseFormDelegate(Form1 form);
        CloseFormDelegate CloseFormHandler = CloseForm;
        private static void CloseForm(Form1 form)
        {
            form.Hide();
        }

        private delegate void SetErrorDelegate(Form1 form, string message);
        SetErrorDelegate SetErrorHandler = SetError;
        private static void SetError(Form1 form, string message)
        {
            form.LoginErrorLabel.Enabled = true;
            form.LoginErrorLabel.Text = message;
        }

        // Validations
        private bool ValidateLoginData(String username, String password)
        {
            if(username.Length>2 && password.Length>3) { return true; }
            else { return false; }
        }

        private bool ValidateRegisterData(String username, String password)
        {
            if (username.Length > 2 && password.Length > 3) { return true; }
            else { return false; }
        }


    }
}

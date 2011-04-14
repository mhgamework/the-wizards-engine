using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.ClientMain
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public struct LoginInfo
        {
            public string Username;
            public string Password;

            public LoginInfo(string username, string password)
            {
                Username = username;
                Password = password;
            }
        }

        public LoginInfo ShowLogin(string defaultUsername, string defaultPassword)
        {
            txtUsername.Text = defaultUsername;
            txtPassword.Text = defaultPassword;
            ShowDialog();

            return new LoginInfo(txtUsername.Text, txtPassword.Text);
        }
    }
}
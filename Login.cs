using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Password_Hash_lib;

namespace AdvancedMusicPlayer
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        static PasswordManager pwdManager = new PasswordManager();

        public bool loginResult;

        private void loginButton_Click(object sender, EventArgs e)
        {
            string userid = usernameBox.Text;

            string password = passwordBox.Text;

            if (UserStorage.users.Count > 0)
            {
                User user = MusicPlayer.users.GetUser(userid);

                if (user != null)
                {
                    bool result = pwdManager.IsPasswordMatch(password, user.Salt, user.PasswordHash);

                    if (result == true)
                    {
                        loginResult = true;
                    }
                    else
                    {
                        loginResult = false;
                    }
                }
                else
                {
                    loginResult = false;
                }
            }
        }

        private void createAccountButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(usernameBox.Text) && !string.IsNullOrEmpty(passwordBox.Text))
            {
                loginResult = false;

                string userid = usernameBox.Text;

                if (MusicPlayer.users.GetUser(userid) != null)
                {
                    MessageBox.Show("That username already exists");
                    return;
                }

                string password = passwordBox.Text;

                string salt = null;

                string passwordHash = pwdManager.GeneratePasswordHash(password, out salt);

                User user = new User
                {
                    UserID = userid,
                    PasswordHash = passwordHash,
                    Salt = salt
                };

                MusicPlayer.users.AddUser(user);

                usernameBox.Clear();
                passwordBox.Clear();
            }
        }
    }
}

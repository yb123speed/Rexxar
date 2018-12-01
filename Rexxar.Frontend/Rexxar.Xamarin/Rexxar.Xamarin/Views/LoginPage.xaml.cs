using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Rexxar.Xamarin.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        void LoginBtn_Clicked(object sender, System.EventArgs e)
        {
            var uname = (username as Entry).Text;
            var pwd = (password as Entry).Text;

        }
    }
}

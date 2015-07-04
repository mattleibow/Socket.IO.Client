using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using SocketIO;
using SocketIO.Client;

using AppCompatActivity = Android.Support.V7.App.AppCompatActivity;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace SocketIOClientJavaSample
{
    [Activity(MainLauncher = true, Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity
    {
        private Socket socket;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Resources.GetString(Resource.String.ApplicationName);

            var login = new LoginFragment();
            FragmentManager.BeginTransaction()
                           .Add(Resource.Id.frameLayout, login)
                           .Commit();
        }

        public void Login(string username)
        {
            if (socket != null)
            {
                socket.Close();
            }

            socket = IO.Socket("http://chat.socket.io/");
            socket.Connect();

            // Tell the server your username
            socket.Emit("add user", username);

            var chat = new ChatFragment(username, socket);
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.frameLayout, chat)
                           .Commit();
        }
    }
}

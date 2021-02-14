﻿//TODO: Do all tests for Release (New System, Run with Minecraft, Web hooks)
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace _2B2T_Queue_Notifier
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Color BkHv = Color.FromRgb(67, 76, 94);
        private readonly Color BkLv = Color.FromRgb(45, 51, 63);
        private string Chat = "Position in queue: ";

        private readonly IniFile Config =
            new IniFile(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\connorcode\2B2T-Queue-Notifier\settings.ini");

        private DispatcherTimer DispatcherTimer;
        private bool DoWebHook;
        private int EqFr;
        private bool HookLogin;
        private bool HookLogout;
        private bool HookPosition;
        private int IndexCache;
        private bool IsIn = true;
        private bool IsLogin = true;
        private bool MntLogin;
        private bool MntLogout;
        private bool MntPosition;
        private string Path = Environment.ExpandEnvironmentVariables(@"%AppData%\.minecraft\logs\latest.log");
        private readonly Color Tcf = Color.FromRgb(191, 97, 106);
        private readonly Color Tcl = Color.FromRgb(163, 190, 140);
        private readonly Color Tcm = Color.FromRgb(235, 203, 139);
        private int TickDelay = 10;
        private int Timeout = 30;
        private string WebHook;
        private string WhoMnt;

        public MainWindow()
        {
            InitializeComponent();
            if (!Config.KeyExists("setup"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\connorcode\2B2T-Queue-Notifier\");
                Config.Write("setup", "true");
                Config.Write("timeout", "30");
                Config.Write("tickdelay", "10");
                Config.Write("chat", "Position in queue: ");
                Config.Write("logpath", @"%AppData%\.minecraft\logs\latest.log");
                Config.Write("dowebhook", "false");
                Config.Write("hooklogin", "true");
                Config.Write("hooklogout", "true");
                Config.Write("hookpoz", "true");
                Config.Write("hookuri", "");
                Config.Write("mntlogin", "false");
                Config.Write("mntlogout", "false");
                Config.Write("mntpoz", "false");
                Config.Write("whomnt", "@everyone");
            }
            else
            {
                UpdateVars();
            }
        }

        private void UpdateVars()
        {
            try
            {
                Chat = Config.Read("chat");
                Timeout = int.Parse(Config.Read("timeout"));
                try { DoWebHook = bool.Parse(Config.Read("tickdelay")); } catch { DoWebHook = false; }
                TickDelay = int.Parse(Config.Read("tickdelay"));
                Path = Environment.ExpandEnvironmentVariables(Config.Read("logpath"));
                try { DoWebHook = bool.Parse(Config.Read("dowebhook")); } catch { DoWebHook = false; }
                try { HookPosition = bool.Parse(Config.Read("hookpoz")); } catch { HookPosition = false; }
                try { HookLogin = bool.Parse(Config.Read("hooklogin")); } catch { HookLogin = false; }
                try { HookLogout = bool.Parse(Config.Read("hooklogout")); } catch { HookLogout = false; }
                try { MntLogin = bool.Parse(Config.Read("mntlogin")); } catch { MntLogin = false; }
                try { MntLogout = bool.Parse(Config.Read("mntlogout")); } catch { MntLogout = false; }
                try { MntPosition = bool.Parse(Config.Read("mntpoz")); } catch { MntPosition = false; }
                WhoMnt = Config.Read("whomnt");
                WebHook = Config.Read("hookuri");
            } catch
            {
                MessageBox.Show("ERR reading Config File...");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\connorcode\2B2T-Queue-Notifier\");
                Config.Write("setup", "true");
                Config.Write("timeout", "30");
                Config.Write("tickdelay", "10");
                Config.Write("chat", "Position in queue: ");
                Config.Write("logpath", @"%AppData%\.minecraft\logs\latest.log");
                Config.Write("dowebhook", "false");
                Config.Write("hooklogin", "true");
                Config.Write("hooklogout", "true");
                Config.Write("hookpoz", "true");
                Config.Write("hookuri", "");
                Config.Write("mntlogin", "false");
                Config.Write("mntlogout", "false");
                Config.Write("mntpoz", "false");
                Config.Write("whomnt", "@everyone");
            }
        }

        private void start_MouseEnter(object sender, MouseEventArgs e)
        {
            Start.Fill = new SolidColorBrush(BkHv);
        }

        private void start_MouseLeave(object sender, MouseEventArgs e)
        {
            Start.Fill = new SolidColorBrush(BkLv);
        }

        private void start_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var Win2 = new Settings();
            Win2.Show();
        }

        private void WebhookError(bool success)
        {
            HookErr.Visibility = !success ? Visibility.Visible : Visibility.Hidden;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateVars();

                var Full = DataGet.GetIndex(Path, Chat);
                var LastChatEvent = DataGet.ChatTime(Path);
                var Index = Full[0];
                if (Index != IndexCache && Full[1] > DataGet.NowTime() - Timeout)
                {
                    EqFr = 0;
                    MainTime.Text = Index.ToString();
                    if (Index > 500)
                    {
                        MainTime.Foreground = new SolidColorBrush(Tcf);
                        if (HookPosition)
                            WebhookError(DataGet.DiscordWebHook(WebHook, Index.ToString(), IndexCache, "12542314", DoWebHook, MntPosition, WhoMnt));
                        IndexCache = Index;
                    }
                    else if (Index > 250 && Index < 500)
                    {
                        MainTime.Foreground = new SolidColorBrush(Tcm);
                        if (HookPosition)
                            WebhookError(DataGet.DiscordWebHook(WebHook, Index.ToString(), IndexCache, "15453067", DoWebHook, MntPosition, WhoMnt));
                        IndexCache = Index;
                    }
                    else if (Index > 0 && Index < 250)
                    {
                        MainTime.Foreground = new SolidColorBrush(Tcl);
                        if (HookPosition)
                            WebhookError(DataGet.DiscordWebHook(WebHook, Index.ToString(), IndexCache, "10731148", DoWebHook, MntPosition, WhoMnt));
                        IndexCache = Index;
                    }
                    IndexCache = Full[0];
                    IsIn = true;
                    IsLogin = true;
                }
                else if (Full[1] != LastChatEvent && LastChatEvent > DataGet.NowTime() - Timeout)
                {
                    MainTime.Text = "Online!";
                    MainTime.Foreground = new SolidColorBrush(Tcl);
                    if (HookLogin && IsLogin)
                        WebhookError(DataGet.DiscordMessage(WebHook, "**Logged In!** :grin:", "9419928", DoWebHook, MntLogin, WhoMnt));
                    IsIn = true;
                    IsLogin = false;
                }
                else if (Index == IndexCache && Full[1] > DataGet.NowTime() - Timeout) { }
                else
                {
                    EqFr += TickDelay;
                    if (EqFr <= Timeout)
                        return;
                    MainTime.Text = "…";
                    MainTime.Foreground = new SolidColorBrush(Tcf);
                    if (HookLogout && IsIn)
                        WebhookError(DataGet.DiscordMessage(WebHook, "**Logged Out **", "12150125", DoWebHook, MntLogout, WhoMnt));
                    IsIn = false;
                    IsLogin = false;
                    EqFr = 0;
                }
            } catch
            {
                MainTime.Foreground = new SolidColorBrush(Tcf);
                MainTime.Text = "…";
            }
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            UpdateVars();
            MainTime.Text = "…";
            DispatcherTimer = new DispatcherTimer();
            DispatcherTimer.Tick += dispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, TickDelay);
            DispatcherTimer.Start();
        }

        #region TopBar

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Close();
        }

        private void Mini_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                WindowState = WindowState.Minimized;
        }

        private void Pin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (FullPin.Visibility == Visibility.Hidden)
            {
                FullPin.Visibility = Visibility.Visible;
                Topmost = true;
            }
            else
            {
                FullPin.Visibility = Visibility.Hidden;
                Topmost = false;
            }
        }

        #endregion TopBar
    }
}
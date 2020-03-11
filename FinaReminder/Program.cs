using System;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using Xml = System.Xml;
using Timer = System.Timers.Timer;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace FinaReminder
{
    internal class Program
    {
        private NotifyIcon _notifyIcon;
        private ContextMenu _contextMenu;
        private MenuItem _menuItem;
        private IContainer _components;
        private SpeechSynthesizer _synth;
        private Timer _timer;
        private int _lastHour;
        
        private const string APP_ID = @"{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}\WindowsPowerShell\v1.0\powershell.exe";
        public static void Main()
        {
            Program program = new Program();

            program.FinaReminder();
        }

        public Program()
        {
            _lastHour = DateTime.Now.Hour;

            _components = new Container();
            _contextMenu = new ContextMenu();
            _menuItem = new MenuItem();
            _contextMenu.MenuItems.AddRange(new [] {_menuItem});

            _menuItem.Index = 0;
            _menuItem.Text = "Stop";
            _menuItem.Click += menuItem1_Click;

            _notifyIcon = new NotifyIcon(_components);

            _notifyIcon.Icon = new Icon("clockwork.ico");
            _notifyIcon.ContextMenu = _contextMenu;

            _notifyIcon.Text = "Fina Reminder";
            _notifyIcon.Visible = true;
            
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, e) => reminder(sender, e, this);
            _timer.Start();
            
            _synth = new SpeechSynthesizer();
            _synth.SetOutputToDefaultAudioDevice();
            _synth.Rate = 0;
        }

        public void FinaReminder()
        {
            
            Application.Run();
        }

        private static void reminder(Object source, System.Timers.ElapsedEventArgs e, Program program)
        {
            Console.WriteLine(program._lastHour + " - " +DateTime.Now.Hour);
            if(program._lastHour < DateTime.Now.Hour || (program._lastHour == 23 && DateTime.Now.Hour == 0))
            {
                program._lastHour = DateTime.Now.Hour;
                string time;
                if (program._lastHour == 12)
                    time = "noon. Have you eaten yet?";
                else if (program._lastHour == 0)
                    time = "midnight. Maybe you should go to sleep";
                else if (program._lastHour > 12)
                    time = program._lastHour - 12 + "pm";
                else
                    time = program._lastHour + "am";
                SendToastNotification("Hey Sarah, it is " + time);
                program._synth.Speak("Hey Sarah, it is " + time);
            }
        }

        private static void SendToastNotification(string text)
        {
            ToastContent content = new ToastContent() {
                Visual = new ToastVisual() {
                    BindingGeneric = new ToastBindingGeneric() {
                        Children = {
                            new AdaptiveText() {
                                Text = "Fina Reminder",
                                HintMaxLines = 1
                            },
                            new AdaptiveText() {
                                Text = text
                            },
                        },
                    }
                },
                Duration = ToastDuration.Long
            };
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content.GetContent());
            var tstVisual = new ToastNotification(xmlDoc);
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(tstVisual);
        }
        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            Application.Exit();
        }
    }
}
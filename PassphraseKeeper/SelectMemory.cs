
// Defines a top-level window with border and title
using PassphraseKeeper;
using System;
using Terminal.Gui;
namespace PassphraseKeeper
{
    public class SelectMemory : Window
    {
        public TextField usernameText;

        public SelectMemory()
        {
            // Application.QuitKey = Key.Null; // Disallow quit by keyboard
            Title = "v.1.0";

            var passphraseKeeperLabel = new Label()
            {
                Text = @"██████╗  █████╗ ███████╗███████╗██████╗ ██╗  ██╗██████╗  █████╗ ███████╗███████╗
██╔══██╗██╔══██╗██╔════╝██╔════╝██╔══██╗██║  ██║██╔══██╗██╔══██╗██╔════╝██╔════╝
██████╔╝███████║███████╗███████╗██████╔╝███████║██████╔╝███████║███████╗█████╗  
██╔═══╝ ██╔══██║╚════██║╚════██║██╔═══╝ ██╔══██║██╔══██╗██╔══██║╚════██║██╔══╝  
██║     ██║  ██║███████║███████║██║     ██║  ██║██║  ██║██║  ██║███████║███████╗
╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝
                                                                                
██╗  ██╗███████╗███████╗██████╗ ███████╗██████╗                                 
██║ ██╔╝██╔════╝██╔════╝██╔══██╗██╔════╝██╔══██╗                                
█████╔╝ █████╗  █████╗  ██████╔╝█████╗  ██████╔╝                                
██╔═██╗ ██╔══╝  ██╔══╝  ██╔═══╝ ██╔══╝  ██╔══██╗                                
██║  ██╗███████╗███████╗██║     ███████╗██║  ██║                                
╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚══════╝╚═╝  ╚═╝                                

",
                X = 2,
                Y = 0,
            };
            Add(passphraseKeeperLabel);
            int id = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    var show = File.Exists(id + ".encrypted");
                    var button = new Button()
                    {
                        X = 1 + x * 14,
                        Y = 16 + y * 2,
                        Text = (show ? "Show " : "Memory ") + id,
                    };                  
                    void setClick(Button button, int id, bool show)
                    {
                        button.Clicked += () =>
                        {
                            Application.RequestStop(); // Quit (close the windows)
                            // Application.Shutdown();
                            Password.NoRepeat = show;
                            //Password.MemoryID = id;
                            if (show)
                            {
                                ShowPassphrase.MemoryID = id;
                                Application.Run<ShowPassphrase>();
                            }
                            else
                            {
                                InputPassphrase.MemoryID = id;
                                Application.Run<InputPassphrase>();
                            }
                            // Application.Run<Password>();
                            // Application.Run<Main>();
                        };
                    }
                    setClick(button, id, show);
                    button.TextFormatter.HotKeySpecifier = '\xffff';
                    Add(button);
                    id++;
                }
            }
            var info = new Label()
            {
                Text = "Make sure there are no hidden cameras, and that the monitor is connected directly to the computer",
                X = 1,
                Y = 22
            };
            Add(info);
        }
    }
}
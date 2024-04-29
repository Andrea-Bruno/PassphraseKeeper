
// Defines a top-level window with border and title
using PassphraseKeeper;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Terminal.Gui;
using static AntiBruteForce.Perform;

namespace PassphraseKeeper
{
    public class Password : Window
    {

        // static public int MemoryID = 0;
        static public bool NoRepeat;
        public Password()
        {
            while (InternetIsAvailable())
            {
                var n = MessageBox.Query(50, 7, "Security alert?", "Disconnect the device from the internet to continue!", "Ok", "Retry");
            }
            Start();
        }

        private static bool InternetIsAvailable()
        {
            if (Debugger.IsAttached)
                return false;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    using var ping = new Ping();
                    return ping.Send("duckduckgo.com").Status == IPStatus.Success;
                }
                catch (Exception)
                {
                    return false;
                }                
            }
            return false;
        }

        private void Start()
        {
            // Application.QuitKey = Key.Esc; // [esc] to quit

            // int memoryID = MemoryID;
            bool noRepeat = NoRepeat;
            Title = "Password";
            Hash = null;

            var fields = noRepeat ? new string[] { "Write the assigned seed:" } : new string[] { "Assign a seed:", "Repeat the seed:" };

            var wordFields = new InputFields(this, ElementsForRow: 1, labels: fields, widthField: 32, selectable: false);
            wordFields.OnCompletedAllFields += (x) =>
            {
                if (x[0].Text.Length < 8)
                {
                    var n = MessageBox.Query(50, 7, "Attention:", "Are Seed too short, must have at least 8 characters!", "ok");
                    return;
                }

                if (!noRepeat && !x[0].Text.Equals(x[1].Text))
                {
                    var n = MessageBox.Query(50, 7, "Attention:", "Seed and its repetition do not match!", "ok");
                    return;
                }

                Task.Run(() =>
                {
                    byte[]? entropy;
                    if (File.Exists(nameof(entropy)))
                        entropy = File.ReadAllBytes(nameof(entropy));
                    else
                    {
                        var rnd = new Random();
                        entropy = new Byte[32];
                        rnd.NextBytes(entropy);
                        File.WriteAllBytes(nameof(entropy), entropy);
                    }
                    var seed = x[0].Text.ToString();
                    using HashAlgorithm algorithm = SHA256.Create();
                    var startHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(seed)); // Generate a 32 bit seed
                    Hash = ParallelHash(startHash, 50000000, 50, default, RefreshProgressBarr, entropy: entropy);
                    if (Progress != null)
                        Progress.Visible = false;
                    OnHashCompleted?.Invoke(Hash);
                });
                // Application.Shutdown();
                Application.RequestStop(); // Quit (close the windows)
                Application.DoEvents();
                //Application.Run<SelectMemory>();
            };

            var keyboard = new OnScreenKeyboard(this, wordFields.Fields[0], OnScreenKeyboard.KeyboardType.Full);
            wordFields.Keyboard = keyboard;

            var infoSeed = new Label()
            {
                Text = noRepeat ? "Write the seed you used to save the passphrase, a wrong seed will return a wrong passphrase!" : "The seed is something similar to a password, it allows you to save and retrieve the passphrase (don't forget it).",
                X = 2,
                Y = wordFields.Y2 + 3
            };
            Add(infoSeed);

            var infoKey = new Label()
            {
                Text = "Anti keylogger protection: Use the mouse and ← ↑ ↓ → TAB SPACE keys to write the seed, ENTER to edit the field",
                X = 2,
                Y = wordFields.Y2 + 6
            };
            Add(infoKey);
        }
        public static byte[]? Hash { get; set; }
        public static Action<byte[]>? OnHashCompleted { get; set; }
        public static Action<float>? ProgressBarRefresh { get; set; }

        private static void RefreshProgressBarr(float progress)
        {
            ProgressBarRefresh?.Invoke(progress);
        }
        private static ProgressBar? Progress;
        public static void AddProgressBar(View view, bool center = true, int width = default)
        {
            if (Hash == null)
            {
                Progress = new ProgressBar()
                {
                    X = center ? Pos.Center() : 3,
                    Y = center ? Pos.Center() + 1 : 7,
                    Width = width == default ? view.Width - 4 : width,
                    Height = 4,
                    //          AutoSize = true,
                    //          ColorScheme = Colors.Error
                };
                view.Add(Progress);
                ProgressBarRefresh = (float progress) =>
                {
                    Progress.Fraction = progress;
                    Application.DoEvents();
                };
            }
        }

    }
}
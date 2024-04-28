using System;
using System.CodeDom.Compiler;
using Terminal.Gui;

namespace PassphraseKeeper
{
    internal class ShowPassphrase : Window
    {
        static public int MemoryID;
        public ShowPassphrase()
        {
            int memoryID = MemoryID;
            Title = "Decryption....     waiting!!";

            Alert = new Label()
            {
                Text = "Decryption requires a lot of computational effort to prevent brute force attacks: Please wait!",
                X = 9,
                Y = Pos.Center(),
            };
            Add(Alert);
            Password.AddProgressBar(this);

            CloseButton = new Button()
            {
                X = 1,
                Y = Pos.AnchorEnd(3),
                Text = "Close",
                Visible = false,
                // Border = new Border() { BorderStyle = BorderStyle.Single, BorderBrush = Color.BrightBlue },
            };
            CloseButton.Clicked += () => Application.RequestStop(); // Quit (close the windows)
            Add(CloseButton);


            if (Password.Hash == null)
            {
                //ProgressBar
                Password.OnHashCompleted = (hash) => ShowKey(memoryID, hash);
            }
            else
                ShowKey(memoryID, Password.Hash);              
        }
        private Button CloseButton;
        private Label Alert;

        private void ShowKey(int memoryID, byte[] password)
        {
            Title = "Passphrase";
            var privateKey = Util.LoadKey(memoryID, Password.Hash);
            var words = Util.BitesToWords(privateKey);
            ShowFields(words);
            Remove(Alert);
            CloseButton.Visible = true;
        }


        public void ShowFields(IEnumerable<string> words, int ElementsForRow = 6, int x1 = 2, int y1 = 1)
        {
            int X1, Y1, X2, Y2;
            X1 = x1;
            Y1 = y1;
            int nWords = words == null ? 0 : words.Count();
            var x = 0;
            var y = 0;
            for (int i = 0; i < nWords; i++)
            {
                var ySize = words != null ? words.Max(x => x.Length) : 2;
                X2 = x1 + x * 18;
                Y2 = y1 + y * 2;
                var label = new Label()
                {
                    Text = (i + 1).ToString() + " " + words.ToArray()[i],
                    X = X2,
                    Y = Y2,
                    Width = ySize
                };

                Add(label);
                x++;
                if (x == ElementsForRow)
                {
                    x = 0;
                    y++;
                }
            }
            Application.DoEvents();
        }
    }
}

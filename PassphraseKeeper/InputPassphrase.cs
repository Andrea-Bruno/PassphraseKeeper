
// Defines a top-level window with border and title
using PassphraseKeeper;
using System.Runtime.CompilerServices;
using System.Text;
using Terminal.Gui;
namespace PassphraseKeeper
{
    public class InputPassphrase : Window
    {
        static public int MemoryID;
        private Button SaveButton;
        // private ProgressBar Progress;
        public InputPassphrase()
        {
            Application.QuitKey = Key.Esc; // [esc] to quit

            int memoryId = MemoryID;
            Title = "Input Passphrase";
            var wordFields = new InputFields(this);
            SaveButton = new Button()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Text = "Save passphrase",
                Border = new Border() { BorderStyle = BorderStyle.Single, BorderBrush = Color.Red },
                Visible = false,
            };
            SaveButton.Leave += (x) => SaveButton.Visible = false;
            SaveButton.Clicked += () =>
            {
                SaveButton.Visible = false;
                // save
                var words = new List<string>();
                foreach (var field in wordFields.Fields)
                {
                    words.Add(field.Value.Text.ToString());
                }
                var privateKey = Util.WordsToBites(words);
                void save(byte[] hash)
                {

                    Util.SaveKey(memoryId, privateKey, hash);
                    Application.DoEvents();
                    Application.RequestStop(); // Quit (close the windows)
                }
                if (Password.Hash == null)
                {
                    Password.OnHashCompleted = (hash) =>
                    {                      
                        save(hash);
                    };
                    RemoveAll();
                    var label = "Encryption requires a lot of computational effort to prevent brute force attacks: Please wait!";
                    var alert = new Label()
                    {
                        Text = label,
                        X = 3,
                        Y = 5,
                        // Y = Pos.Center(),
                    };
                    Add(alert);
                    Password.AddProgressBar(this, false, label.Length);
                }
                else
                    save(Password.Hash);
            };
            Add(SaveButton);

            wordFields.OnUncompletedFields += () =>
            {
                SaveButton.Visible = false;
            };
            wordFields.OnCompletedAllFields += (x) =>
            {

                SaveButton.Visible = true;
                SaveButton.SetFocus();
                //saveButton.Focused = true;
                //var passphrase = new StringBuilder();
                //for (int i = 0; i < wordFields.Fields.Count; i++)
                //{
                //    passphrase.Append((i+1).ToString() + "." + wordFields.Fields[i].Text.ToString() + " ");
                //}
                //var n = MessageBox.Query(50, 7, "Save this passphrase?", passphrase.ToString() , "yes", "no");
                //if (n == 0)
                //{
                //}
            };

            var keyboard = new OnScreenKeyboard(this, wordFields.Fields[0], words: WordList.English.ToList());
            wordFields.Keyboard = keyboard;

            var infoKey = new Label()
            {
                Text = "Anti keylogger protection: Use the mouse and ← ↑ ↓ → TAB SPACE keys to set the passphrase, ENTER to edit the field",
                X = 2,
                Y = wordFields.Y2 + 3
            };
            Add(infoKey);
        }
    }
}
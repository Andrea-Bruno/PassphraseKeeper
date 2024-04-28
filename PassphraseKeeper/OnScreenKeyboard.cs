using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terminal.Gui;

namespace PassphraseKeeper
{
    internal class OnScreenKeyboard
    {
        public OnScreenKeyboard(Window window, TextField? inputField = null, KeyboardType type = KeyboardType.Passphrase, List<string>? words = null)
        {
            Words = words;
            Type = type;
            Window = window;
            AddKeyBoard();
            InputField ??= inputField;
        }

        bool _Shift { get; set; }

        bool Shift
        {
            get { return _Shift; }
            set
            {
                _Shift = value;
                foreach (var key in keys)
                {
                    if (char.IsLetter(key.Key))
                    {
                        if (value)
                            key.Value.Text = key.Value.Text.ToUpper();
                        else
                            key.Value.Text = key.Value.Text.ToLower();
                    }
                    // key.Value.TextFormatter.Text = key.Value.Text;
                }
                var shift = keys.First(x => x.Key == 128);
                shift.Value.Border.BorderBrush = value ? BorderActive : BorderNormal;
            }
        }

        private readonly KeyboardType Type;
        public enum KeyboardType
        {
            Passphrase,
            Full,
        }

        public Action<OnCompletedField>? OnCompleted;
        public Action<TextField>? OnFieldDeleted;
        private List<string>? Words;

        private Window Window;
        public class OnCompletedField
        {
            public TextField Current;
            public TextField? SetNext;
        }

        private TextField? _InputField;
        public TextField? InputField
        {
            get { return _InputField; }
            set
            {
                if (value != null && _InputField == value)
                    return;
                if (_InputField != null)
                {
                    _InputField.ColorScheme = InputFields.Unfocused;

                    if (Words != null && !Words.Contains((string)_InputField.Text))
                        _InputField.Text = "";
                }
                _InputField = value;
                if (_InputField != null)
                {
                    _InputField.ColorScheme = InputFields.Focused;
                    HideNotNecessaryKeys();
                }
            }
        }

        private Dictionary<char, Button> keys = new Dictionary<char, Button>();

        struct xy
        {
            public int x, y;
        }
        private Dictionary<char, xy> Position = new Dictionary<char, xy>();

        private Color BorderNormal = Color.BrightBlue;
        private Color BorderActive = Color.White;

        private void AddKeyBoard()
        {
            int btnForLine = 6;
            int marginBottom = 14;
            var x = 0;
            var y = 0;
            IEnumerable<int> allowed;

            if (Type == KeyboardType.Passphrase)
            {
                var start = 97;
                var end = 122;
                allowed = Enumerable.Range(start, end - start + 1);
            }
            else
            {
                var start = 0;
                var end = 255;
                var range = Enumerable.Range(start, end - start + 1);

                var allow = @"!""£$%^&*()-+={}[]:;@'~#\<>,.?/".ToCharArray();

                allowed = range.Distinct().Where(x => char.IsDigit((char)x));
                allowed = allowed.Concat(range.Distinct().Where(x => allow.Contains((char)x)));
                allowed = allowed.Concat(Enumerable.Range('a', 'z' - 'a' + 1));
                btnForLine = 14;
                //marginBottom = 18;
                allowed = allowed.Append(128); // SHIFT
            }
            allowed = allowed.Append(127); // DEL
            allowed = allowed.Append(13); // ENTER
            var rangeArray = allowed.ToArray();
            int r = 0;
            foreach (byte key in allowed)
            {
                char k = (char)key;
                string text;

                if (key == 127)
                    text = "DEL";
                else if (key == 128)
                    text = "SHIFT";
                else if (key == 13)
                    text = "ENTER";
                else
                    text = new string(k, 1);
                try
                {
                    var _x = 1 + (x * 8);
                    if (_x < r)
                        _x = r;
                    //var btnKey = new CustomButton(text, _x, Pos.AnchorEnd(marginBottom - (y * 3)), text.Length + 4, 3)
                    //{
                    //       Border = new Border() { BorderStyle = BorderStyle.Single, BorderBrush = BorderNormal },
                    //};
                    var btnKey = new Button()
                    {
                        Text = text,
                        X = _x,
                        Y = Pos.AnchorEnd(marginBottom - (y * 3)),
                        Border = new Border() { BorderStyle = BorderStyle.Single, BorderBrush = BorderNormal },
                    };
                    // btnKey.TextFormatter.Text = text;
                    btnKey.TextFormatter.HotKeySpecifier = '\xffff';
                    // btnKey.TextFormatter.HotKeyPos = -1;
                    r = _x + text.Length + 4 + 3;

                    keys.Add(k, btnKey);
                    btnKey.Clicked += () =>
                    {
                        if (btnKey.Visible == false)
                            return;
                        if (k == (char)13) // ENTER
                        {
                            Enter((string)InputField?.Text, OnCompleted);
                        }
                        else if (k == (char)128) // SHIFT
                        {
                            Shift = !Shift;
                        }
                        else if (k == (char)127) // DEL
                        {
                            var input = (string)InputField?.Text;
                            if (input.Length > 0)
                            {
                                if (Words != null && Words.Contains(input) && InputField != null)
                                {
                                    InputField.Text = "";
                                    OnFieldDeleted?.Invoke(InputField);
                                }
                                else
                                {
                                    input = input.Substring(0, input.Length - 1);
                                    if (InputField != null)
                                        InputField.Text = input;
                                }
                            }
                        }
                        else
                        {
                            if (InputField != null)
                                InputField.Text += Shift ? text.ToUpper() : text;
                        }
                        HideNotNecessaryKeys(true);
                    };
                    btnKey.KeyDown += (View.KeyEventEventArgs arg) =>
                    {
                        var xy = Position[k];
                        var keyName = arg.KeyEvent.Key;
                        void jumpTokey(int x, int y)
                        {
                            var toKey = Position.FirstOrDefault(KeyRows => KeyRows.Value.y == y && KeyRows.Value.x == x);
                            keys.TryGetValue(toKey.Key, out JumpToKey);
                            if (JumpToKey?.Visible == false)
                            {
                                foreach (var key in keys.Keys)
                                {
                                    var btn = keys[key];
                                    if (btn.Visible)
                                    {
                                        var xy = Position[key];
                                        if (xy.y == y)
                                        {
                                            JumpToKey = btn;
                                            if (xy.x > x && JumpToKey != null)
                                                break;
                                        }
                                    }
                                }

                            }
                        }
                        if (keyName == Key.CursorUp)
                        {
                            arg.Handled = false;
                            var y = xy.y - 1;
                            if (y >= 0)
                                jumpTokey(xy.x, y);
                        }
                        if (keyName == Key.CursorDown)
                        {
                            arg.Handled = false;
                            var y = xy.y + 1;
                            if (y <= KeyRows)
                                jumpTokey(xy.x, y);
                        }
                    };

                    btnKey.KeyUp += (View.KeyEventEventArgs arg) =>
                    {
                        arg.Handled = true;
                        JumpToKey?.SetFocus();
                        JumpToKey = null;
                    };

                    Position.Add(k, new xy { x = x, y = y });
                    Window.Add(btnKey);

                }
                catch (Exception)
                {

                }

                x++;
                if (x == btnForLine)
                {
                    r = 0;
                    x = 0;
                    y++;
                    KeyRows = y;
                }
            }
            //keys[(char)127].Visible = false;
            //keys[(char)13].Visible = false;
            //OnTextChange();
        }

        private Button? JumpToKey;
        private int KeyRows = 0;

        private void Enter(string word, Action<OnCompletedField>? onCompleted)
        {
            foreach (var kv in keys)
                kv.Value.Visible = true;
            InputField.Text = word;
            var completed = new OnCompletedField() { Current = InputField };
            onCompleted?.Invoke(completed);
            InputField = completed.SetNext;
            HideNotNecessaryKeys();
        }

        public void Focus()
        {
            HideNotNecessaryKeys();
        }

        private void HideNotNecessaryKeys(bool onTextChange = false)
        {
            bool ok = false;
            var t = (string)InputField.Text;
            do
            {
                try
                {
                    keys[(char)13].Visible = false; // ENTER
                    keys[(char)127].Visible = (t.Length > 0); // DEL
                    ok = true;
                }
                catch (Exception)
                {
                    Debugger.Break();
                }
            } while (ok == false);
            List<char> visible = new();

            void focusRandomKey()
            {
                if (visible.Count > 0)
                {
                    Random random = new();
                    int selectID = random.Next(0, visible.Count);
                    char preselect = visible[selectID];
                    keys[preselect].SetFocus();
                }
            }
            if (Words == null)
            {
                if (t.Length > 0)
                    keys[(char)13].Visible = true;
                foreach (var kv in keys)
                {
                    if (!char.IsControl(kv.Key))
                        visible.Add(kv.Key);
                }
                focusRandomKey();
                return;
            }
            try
            {
                var possibleNextWord = Words.FindAll(x => x.StartsWith(t));
                if (onTextChange && possibleNextWord.Count == 1)
                {
                    Enter(possibleNextWord[0], OnCompleted);
                }
                else
                {
                    var possibleNextKey = new List<char>();
                    bool enableEnter = false;
                    possibleNextWord.ForEach((word) =>
                    {
                        if (word.Length > t.Length)
                            possibleNextKey.Add(word[t.Length]);
                        else
                            enableEnter = true;
                    });
                    keys[(char)13].Visible = enableEnter;
                    int r = 0;

                    foreach (var kv in keys)
                    {
                        if (!char.IsControl(kv.Key))
                            if (possibleNextKey.Contains(kv.Key))
                            {
                                kv.Value.Visible = true;
                                var nt = t + kv.Key;
                                var nr = Words.FindAll(x => x.StartsWith(nt)).Count;
                                if (nr > r)
                                {
                                    r = nr;
                                    visible.Add(kv.Key);
                                    // preselect = kv.Key;
                                }
                            }
                            else
                                kv.Value.Visible = false;
                    }
                    focusRandomKey();
                    // if (preselect != default)
                    //if (visible.Count > 0)
                    //{
                    //    Random random = new();
                    //    int selectID = random.Next(0, visible.Count);
                    //    char preselect = visible[selectID];
                    //    keys[preselect].SetFocus();
                    //}
                }
            }
            catch (Exception)
            {


            }

        }


    }
}

using System.Diagnostics;
using Terminal.Gui;

namespace PassphraseKeeper
{
    internal class InputFields
    {
        public InputFields(Window window, int words = 24, int ElementsForRow = 6, int x1 = 2, int y1 = 1, int widthField = 8, IEnumerable<string>? labels = null, bool selectable = true)
        {
            X1 = x1;
            Y1 = y1;

            Window = window;

            if (labels != null)
                words = labels.Count();

            var x = 0;
            var y = 0;
            for (int i = 0; i < words; i++)
            {
                var ySize = labels != null ? labels.Max(x => x.Length) : 2;
                X2 = x1 + x * 18;
                Y2 = y1 + y * 2;
                var label = new Label()
                {
                    Text = labels != null ? labels.ToArray()[i] : ((i + 1).ToString() + " ")[..2] + ":",
                    X = X2,
                    Y = Y2,
                    Width = ySize
                };
                var field = new TextField("")
                {
                    X = Pos.Right(label) + 1,
                    Y = label.Y,
                    Width = widthField,
                    ReadOnly = true,
                    TabStop = selectable,
                };
                field.ColorScheme = Unfocused;

                field.Enter += (args) =>
                {
                    field.ColorScheme = Focused;
                    FieldOnFocus?.Invoke(field);
                };
                field.Leave += (args) =>
                {
                    if (Keyboard.InputField != field)
                        field.ColorScheme = Unfocused;
                };
                field.KeyUp += (args) =>
                {
                    var key = args.KeyEvent.Key;
                    if (key == Key.Enter || key == Key.Space)
                    {
                        RequestKeyboardFocus?.Invoke();
                        field.ColorScheme = Unfocused;
                    }
                };
                field.MouseClick += (args) =>
                {
                    if (args.MouseEvent.Flags.HasFlag(MouseFlags.Button1Clicked))
                    {
                        //RequestKeyboardFocus?.Invoke();
                        var timer = new Timer((o) => RequestKeyboardFocus?.Invoke(), null, 0, Timeout.Infinite);
                    }
                };


                Window.Add(label);
                Window.Add(field);
                Fields.Add(i, field);
                x++;
                if (x == ElementsForRow)
                {
                    x = 0;
                    y++;
                }
            }
        }

        public readonly int X1, Y1, X2, Y2;

        public OnScreenKeyboard _Keyboard;
        public OnScreenKeyboard Keyboard
        {
            get { return _Keyboard; }
            set
            {
                _Keyboard = value;
                FieldOnFocus += (textField) => _Keyboard.InputField = textField;
                RequestKeyboardFocus += () => _Keyboard.Focus();
                void OnCompleted(OnScreenKeyboard.OnCompletedField fields)
                {
                    var id = Fields.Values.ToList().IndexOf(fields.Current);
                    var nextId = id + 1;
                    if (nextId >= Fields.Count)
                        nextId = 0;
                    fields.SetNext = Fields[nextId];

                    var fieldsList = Fields.Values.ToList();
                    if (fieldsList.All(x => !string.IsNullOrEmpty((string)x.Text)))
                    {
                        OnCompletedAllFields?.Invoke(fieldsList);
                    }
                }
                _Keyboard.OnCompleted = OnCompleted;
                _Keyboard.OnFieldDeleted = (x) => OnUncompletedFields?.Invoke();
            }
        }

        public delegate void OnCompleted(List<TextField> fields);
        public event OnCompleted OnCompletedAllFields; // event
        public event Action OnUncompletedFields; // event


        public delegate void RequestFucus();
        public event RequestFucus RequestKeyboardFocus; // event


        public delegate void OnFocusNotify(TextField word);
        public event OnFocusNotify FieldOnFocus; // event

        public static readonly ColorScheme Focused = new ColorScheme()
        {
            Focus = new Terminal.Gui.Attribute(Color.Red, Color.BrightYellow),
            Normal = new Terminal.Gui.Attribute(Color.Red, Color.BrightYellow),
            Disabled = new Terminal.Gui.Attribute(Color.Red, Color.BrightYellow),

        };

        public static readonly ColorScheme Unfocused = new ColorScheme()
        {
            Focus = new Terminal.Gui.Attribute(Color.Black, Color.White),
            Normal = new Terminal.Gui.Attribute(Color.Blue, Color.White),
            Disabled = new Terminal.Gui.Attribute(Color.Blue, Color.White),

        };

        Window Window;
        public Dictionary<int, TextField> Fields = new Dictionary<int, TextField>();
    }
}

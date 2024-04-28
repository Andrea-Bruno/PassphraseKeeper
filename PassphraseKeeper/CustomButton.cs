using System.Diagnostics;
using Terminal.Gui;

namespace PassphraseKeeper
{
    public class CustomButton : Button
    {
        public string Description => $"Description of: {id}";

        public event Action<CustomButton> PointerEnter;

        private Label fill;
        public new FrameView Border;
        private string id;

        public CustomButton(string text, Pos x, Pos y, int width, int height) : base(text)
        {
            CustomInitialize("", text, x, y, width, height);
        }

        public CustomButton(string id, string text, Pos x, Pos y, int width, int height) : base(text)
        {
            CustomInitialize(id, text, x, y, width, height);
        }

        private void CustomInitialize(string id, string text, Pos x, Pos y, int width, int height)
        {
            this.id = id;
            X = x;
            Y = y;

            Frame = new Rect
            {
                Width = width,
                Height = height
            };

            Border = new FrameView()
            {
                Width = width,
                Height = height
            };

            AutoSize = false;

            var fillText = new System.Text.StringBuilder();
            for (int i = 0; i < Bounds.Height; i++)
            {
                if (i > 0)
                {
                    fillText.AppendLine("");
                }
                for (int j = 0; j < Bounds.Width; j++)
                {
                    fillText.Append("█");
                }
            }

            fill = new Label(fillText.ToString())
            {
                Visible = false,
                CanFocus = false
            };

            var title = new Label(text)
            {
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            Border.MouseClick += This_MouseClick;
            Border.Subviews[0].MouseClick += This_MouseClick;
            fill.MouseClick += This_MouseClick;
            title.MouseClick += This_MouseClick;

            Add(Border, fill, title);
        }

        private void This_MouseClick(MouseEventArgs obj)
        {
            OnMouseEvent(obj.MouseEvent);
        }

        public override bool OnMouseEvent(MouseEvent mouseEvent)
        {
            Debug.WriteLine($"{mouseEvent.Flags}");
            if (mouseEvent.Flags == MouseFlags.Button1Clicked)
            {
                if (!HasFocus && SuperView != null)
                {
                    if (!SuperView.HasFocus)
                    {
                        SuperView.SetFocus();
                    }
                    SetFocus();
                    SetNeedsDisplay();
                }

                OnClicked();
                return true;
            }
            return base.OnMouseEvent(mouseEvent);
        }

        public override bool OnEnter(View view)
        {
            Border.Visible = false;
            fill.Visible = true;
            PointerEnter.Invoke(this);
            view = this;
            return base.OnEnter(view);
        }

        public override bool OnLeave(View view)
        {
            Border.Visible = true;
            fill.Visible = false;
            if (view == null)
                view = this;
            return base.OnLeave(view);
        }
    }
}

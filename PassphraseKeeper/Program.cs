// See https://aka.ms/new-console-template for more information

using PassphraseKeeper;
using Terminal.Gui;

Application.QuitKey = Key.Esc;

// Application.Run<SelectMemory>();
// Before the application exits, reset Terminal.Gui for clean shutdown
// Application.Shutdown();
Password.NoRepeat = Directory.GetFiles(".", "*.encrypted").Count() > 0;
Application.Run<Password>();
do
{
    Application.Run<SelectMemory>();

} while (true);




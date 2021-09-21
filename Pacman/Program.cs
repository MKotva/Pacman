using System;
using System.Windows.Forms;

class Program
{
    [STAThreadAttribute] //Its needed for folderbrowser
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new SettingsForm());
    }
}

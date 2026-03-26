using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace RSAForge.Views;

// Меняем Window на FluentWindow
public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        WindowBackgroundManager.UpdateBackground(
            this,
            ApplicationThemeManager.GetAppTheme(),
            WindowBackdropType.Mica
        );
    }
}
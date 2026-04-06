using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using RSAForge.Services;
using RSAForge.ViewModels;

namespace RSAForge.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        // 1. Создаем экземпляры сервисов
        var rsaService = new RsaService();
        var fileService = new FileService();

        // 2. Устанавливаем DataContext
        DataContext = new MainViewModel(rsaService, fileService);

        // 3. Эффект Mica (из прошлого шага)
        WindowBackgroundManager.UpdateBackground(
            this, 
            ApplicationThemeManager.GetAppTheme(), 
            WindowBackdropType.Mica
        );
    }
}
using System.Windows;
using RSAForge.Infrastructure;
using RSAForge.Models;
using RSAForge.Services;

namespace RSAForge.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IRsaService _rsaService;
    private readonly IFileService _fileService;

    // Данные ключей
    private string _rawPublicKey = string.Empty;
    private string _rawPrivateKey = string.Empty;

    // Настройки генерации
    private int _selectedKeySize = 2048;
    private string _selectedFormat = "PKCS#8 (Modern)";

    // Настройки отображения Публичного ключа
    private bool _publicStripHeaders;
    private bool _publicSingleLine;

    // Настройки отображения Приватного ключа
    private bool _privateStripHeaders;
    private bool _privateSingleLine;

    // Валидация
    private string _testPhrase = string.Empty;
    private string _validationResult = "Ожидание генерации...";

    public MainViewModel(IRsaService rsaService, IFileService fileService)
    {
        _rsaService = rsaService;
        _fileService = fileService;

        // Инициализация команд
        GenerateCommand = new RelayCommand(_ => ExecuteGenerate());
        CopyCommand = new RelayCommand(param => ExecuteCopy(param));
        SaveCommand = new RelayCommand(_ => ExecuteSave(), _ => CanExecuteAction());
        ValidateCommand = new RelayCommand(_ => ExecuteValidate(), _ => CanExecuteAction());
    }

    #region Properties for Binding

    public string SelectedFormat
    {
        get => _selectedFormat;
        set => SetField(ref _selectedFormat, value);
    }

    public int SelectedKeySize
    {
        get => _selectedKeySize;
        set => SetField(ref _selectedKeySize, value);
    }

    // Свойства для ключей (то, что видит пользователь в TextBox)
    public string DisplayPublicKey => _rsaService.FormatKey(_rawPublicKey, _publicStripHeaders, _publicSingleLine);
    public string DisplayPrivateKey => _rsaService.FormatKey(_rawPrivateKey, _privateStripHeaders, _privateSingleLine);

    // Галочки для публичного ключа
    public bool PublicStripHeaders
    {
        get => _publicStripHeaders;
        set
        {
            if (SetField(ref _publicStripHeaders, value))
            {
                OnPropertyChanged(nameof(DisplayPublicKey));
            }
        }
    }

    public bool PublicSingleLine
    {
        get => _publicSingleLine;
        set
        {
            if (SetField(ref _publicSingleLine, value))
            {
                OnPropertyChanged(nameof(DisplayPublicKey));
            }
        }
    }

    // Галочки для приватного ключа
    public bool PrivateStripHeaders
    {
        get => _privateStripHeaders;
        set
        {
            if (SetField(ref _privateStripHeaders, value))
            {
                OnPropertyChanged(nameof(DisplayPrivateKey));
            }
        }
    }

    public bool PrivateSingleLine
    {
        get => _privateSingleLine;
        set
        {
            if (SetField(ref _privateSingleLine, value))
            {
                OnPropertyChanged(nameof(DisplayPrivateKey));
            }
        }
    }

    public string TestPhrase
    {
        get => _testPhrase;
        set => SetField(ref _testPhrase, value);
    }

    public string ValidationResult
    {
        get => _validationResult;
        set => SetField(ref _validationResult, value);
    }

    #endregion

    #region Commands

    public RelayCommand GenerateCommand { get; }
    public RelayCommand CopyCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand ValidateCommand { get; }

    #endregion

    #region Execution Methods

    private void ExecuteGenerate()
    {
        RsaKeyPair pair = _rsaService.GenerateKeyPair(_selectedKeySize, _selectedFormat);
        _rawPublicKey = pair.PublicKey;
        _rawPrivateKey = pair.PrivateKey;

        // Уведомляем UI, что отображаемые свойства изменились
        OnPropertyChanged(nameof(DisplayPublicKey));
        OnPropertyChanged(nameof(DisplayPrivateKey));
        ValidationResult = "Ключи сгенерированы. Готово к проверке.";
    }

    private void ExecuteCopy(object? parameter)
    {
        string textToCopy = parameter?.ToString() == "Public" ? DisplayPublicKey : DisplayPrivateKey;
        
        if (!string.IsNullOrEmpty(textToCopy))
        {
            Clipboard.SetText(textToCopy);
        }
    }

    private async void ExecuteSave()
    {
        // При сохранении учитываем текущие галочки форматирования
        await _fileService.SaveKeyPairAsync(DisplayPublicKey, DisplayPrivateKey);
    }

    private void ExecuteValidate()
    {
        if (string.IsNullOrWhiteSpace(_testPhrase))
        {
            ValidationResult = "Введите текст для проверки!";
            return;
        }

        bool isValid = _rsaService.ValidateKeyPair(_rawPublicKey, _rawPrivateKey, _testPhrase);
        ValidationResult = isValid ? "✅ Успех: Пара валидна!" : "❌ Ошибка: Ключи не совпадают.";
    }

    private bool CanExecuteAction()
    {
        return !string.IsNullOrEmpty(_rawPublicKey) && !string.IsNullOrEmpty(_rawPrivateKey);
    }

    #endregion
}
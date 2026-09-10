using System;
using System.Windows;
using System.Windows.Input;
using ValorantAutoLock.Models;
using ValorantAutoLock.Services;
using ValorantAutoLock.ViewModels;
using ValorantAutoLock.Views;

namespace ValorantAutoLock;

public partial class App : Application
{
    private readonly SettingsService _settingsService = new();
    private readonly LocalizationService _loc = new();
    private readonly DataService _dataService = new();
    private readonly RiotApiService _riotApi = new();
    private AutoLockService? _autoLock;
    private MainViewModel? _mainVm;
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var settings = _settingsService.Load();
        _loc.CurrentLanguage = settings.Language;

        _autoLock = new AutoLockService(_riotApi, _dataService, _settingsService, _loc);
        _mainVm = new MainViewModel(_autoLock, _settingsService, _loc, _dataService);

        _mainWindow = new MainWindow { DataContext = _mainVm };
        _mainWindow.Closing += (s, args) =>
        {
            if (settings.MinimizeToTray && !_mainVm.IsArmed)
            {
                args.Cancel = true;
                _mainWindow.Hide();
            }
            else
            {
                _mainVm.SaveSettings();
            }
        };

        if (settings.StartMinimized)
        {
            _mainWindow.Hide();
        }
        else
        {
            _mainWindow.Show();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mainVm?.SaveSettings();
        _mainVm?.Dispose();
        _autoLock?.Dispose();
        _riotApi.Dispose();
        base.OnExit(e);
    }
}
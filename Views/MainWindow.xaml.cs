using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using ValorantAutoLock.Models;
using ValorantAutoLock.Services;
using ValorantAutoLock.ViewModels;

namespace ValorantAutoLock.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = (MainViewModel)DataContext;
        _vm.SettingsRequested += (s, e) => ShowSettingsTab();
    }

    private void ShowSettingsTab()
    {
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Minimized && _vm.MinimizeToTray)
        {
            Hide();
        }
    }

    public void ShowWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
        Focus();
    }
}

// Static localization helper for XAML
public static class Loc
{
    public static LocalizationService Instance { get; } = new();
}

// Language name converter for ComboBox
public class LanguageNameConverter : IValueConverter
{
    public static LanguageNameConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Language.English => "English",
            Language.French => "Français",
            Language.Spanish => "Español",
            Language.German => "Deutsch",
            _ => value?.ToString() ?? ""
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
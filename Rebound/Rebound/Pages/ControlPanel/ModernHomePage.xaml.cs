using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Rebound.Rebound.Pages.ControlPanel;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ModernHomePage : Page
{
    public ModernHomePage()
    {
        this.InitializeComponent();
        if (App.cpanelWin != null) App.cpanelWin.SetWindowIcon("Assets\\AppIcons\\rcontrol.ico");
        if (App.cpanelWin != null) App.cpanelWin.Title = "Rebound Control Panel";
        LoadWallpaper();
        DisplaySystemInformation();
    }

    private void DisplaySystemInformation()
    {
        // Get Current User
        string currentUser = Environment.UserName;

        // Get PC Name
        string pcName = Environment.MachineName;

        // Get CPU Name
        string cpuName = GetCPUName();

        // Get RAM Capacity
        string ramCapacity = GetRAMCapacity();

        // Displaying the information (Assuming you have TextBlocks in your XAML)
        CurrentUser.Text = $"Logged in as {currentUser}";
        PCName.Text = $"{pcName}";
        CPU.Text = $"CPU: {cpuName}";
        Memory.Text = $"RAM: {ramCapacity}";
    }

    private string GetCPUName()
    {
        string cpuName = string.Empty;
        using (var searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
        {
            foreach (var item in searcher.Get())
            {
                cpuName = item["Name"].ToString();
                break;
            }
        }
        return cpuName;
    }

    private string GetRAMCapacity()
    {
        double ramCapacityGB = 0;
        using (var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory"))
        {
            foreach (var item in searcher.Get())
            {
                ramCapacityGB += Convert.ToDouble(item["Capacity"]) / (1024 * 1024 * 1024);
            }
        }
        return $"{ramCapacityGB} GB";
    }

    // Constants for SystemParametersInfo function
    private const int SPI_GETDESKWALLPAPER = 0x0073;
    private const int MAX_PATH = 260;

    // P/Invoke declaration for SystemParametersInfo function
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

    // Method to retrieve the current user's wallpaper path
    private string GetWallpaperPath()
    {
        StringBuilder wallpaperPath = new StringBuilder(MAX_PATH);
        SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaperPath, 0);
        return wallpaperPath.ToString();
    }

    public async void LoadWallpaper()
    {
        try
        {
            Wallpaper.Source = new BitmapImage(new Uri(GetWallpaperPath(), UriKind.RelativeOrAbsolute));
        }
        catch
        {
            await App.cpanelWin.ShowMessageDialogAsync("You need to run this app as administrator in order to retrieve the wallpaper.");
        }
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as ComboBox).SelectedIndex == 0 && (App.cpanelWin != null))
        {
            App.cpanelWin.AddressBox.Text = @"Control Panel";
            App.cpanelWin.NavigateToPath(true);
        }
        if ((sender as ComboBox).SelectedIndex == 1 && (App.cpanelWin != null))
        {
            App.cpanelWin.AddressBox.Text = @"Control Panel";
            App.cpanelWin.NavigateToPath();
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        try
        {
            if ((NavigationViewItem)sender.SelectedItem == AppearanceItem || (NavigationViewItem)sender.SelectedItem == Re11Item)
            {
                App.cpanelWin.AddressBox.Text = @"Control Panel\Appearance and Personalization";
                App.cpanelWin.NavigateToPath();
            }
            else if ((string)((NavigationViewItem)sender.SelectedItem).Tag is "SysAndSecurity")
            {
                App.cpanelWin.AddressBox.Text = @"Control Panel\System and Security";
                App.cpanelWin.NavigateToPath();
            }
        }
        catch (Exception ex)
        {
            if (App.cpanelWin != null) App.cpanelWin.Title = ex.Message;
        }
    }

    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        App.cpanelWin.RootFrame.Navigate(typeof(AppearanceAndPersonalization), null, new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
    }

    private void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        var info = new ProcessStartInfo()
        {
            FileName = "powershell.exe",
            Arguments = "Start-Process -FilePath \"C:\\Windows\\System32\\control.exe\"",
            Verb = "runas",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(info);

        App.cpanelWin.Close();
    }

    private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
    {
        App.cpanelWin.RootFrame.Navigate(typeof(SystemAndSecurity), null, new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
    }
}

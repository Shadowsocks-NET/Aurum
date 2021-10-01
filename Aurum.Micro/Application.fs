namespace Aurum.Micro

module Application =
    open Aurum.Micro
    open Avalonia
    open Avalonia.Controls.ApplicationLifetimes
    open Avalonia.FuncUI

    type App() =
        inherit Application()

        override this.Initialize() =
            this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
            this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"
            this.Styles.Load "avares://Aurum.Micro/Styles.xaml"

        override this.OnFrameworkInitializationCompleted() =
            match this.ApplicationLifetime with
            | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
                desktopLifetime.MainWindow <- Shell.MainWindow()
            | :? ISingleViewApplicationLifetime as singleViewLifetime ->
                singleViewLifetime.MainView <- ShellControl.Host()
            | _ -> ()

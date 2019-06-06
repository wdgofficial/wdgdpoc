// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using WDG.Utility;
using WDG.Wallet.Win.Biz.Monitor;
using WDG.Wallet.Win.Biz.Services;
using WDG.Wallet.Win.Common;
using WDG.Wallet.Win.Common.Detects;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace WDG.Wallet.Win
{
    /// <summary>
    /// App.xaml 
    /// </summary>
    public partial class App : Application
    {
        static Mutex CurrentAppMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!DetectEnviroment())
            {
                Environment.Exit(0);
            }

            DelVersionData();

            var netType = GetNetType(e.Args);

            FiiiCoinSetting.CurrentNetworkType = netType;
            DetectInRule.UpdateInRule();

            Formats.Init();

            bool isCreateNew;
            CurrentAppMutex = DetectSingleRun(out isCreateNew);

            if (!isCreateNew)
            {
                MessageBox.Show("Application is already running...");
                Thread.Sleep(1000);
                Environment.Exit(1);
                CloseMutex();
            }

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            LanguageService.Default.SetLanguage(AppSettingConfig.Default.AppConfig.LanguageType);
            base.OnStartup(e);
            var shell = BootStrapService.Default.Shell.GetWindow();
            if (shell != null)
                shell.ShowDialog();

            CloseMutex();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                Logger.Singleton.Error(((Exception)e.ExceptionObject).Message);
            }

            ServiceMonitor.StopAll();
            var result = EngineService.Default.AppClosed();
        }

        private NetworkType GetNetType(string[] s)
        {
            try
            {
                if (s[0].ToLower() == "-testnet")
                    return NetworkType.TestNet;
                return NetworkType.Mainnet;
            }
            catch
            {
                return NetworkType.Mainnet;
            }
        }


        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Singleton.Error(e.Exception.Message);
            ServiceMonitor.StopAll();
            var result = EngineService.Default.AppClosed();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceMonitor.StopAll();
            var result = EngineService.Default.AppClosed();
            base.OnExit(e);
        }

        private Mutex DetectSingleRun(out bool createNew)
        {
            var mutexStr = FiiiCoinSetting.CurrentNetworkType == NetworkType.Mainnet ? "WDG.Wallet" : "WDG.Wallet(testnet)";
            var mutex = new Mutex(true, mutexStr, out createNew);
            return mutex;
        }

        public static void CloseMutex()
        {
            if (CurrentAppMutex != null)
            {
                CurrentAppMutex.Close();
                CurrentAppMutex.Dispose();
            }
        }

        private static bool DetectVC2015()
        {
            DetectVC2015 detect = new DetectVC2015();
            var result = detect.Detect();
            if (!result)
            {
                var msgResult = MessageBox.Show("WDG wallet requires to install Visual C++ 2015 Redistributable\r\n download now??", 
                    "Enviroment Detect", MessageBoxButton.OK);
                if (msgResult == MessageBoxResult.OK)
                {
                    System.Diagnostics.Process.Start("https://www.microsoft.com/en-us/download/details.aspx?id=53587");
                }
            }
            return result;
        }

        private static bool DetectNetCore()
        {
            DetectNetCore detect = new DetectNetCore();
            var result = detect.DetectVersion("2.0.0");
            if (!result)
            {
                var msgResult = MessageBox.Show("New version available, download now?", 
                    "Enviroment Detect", MessageBoxButton.OK);
                if (msgResult == MessageBoxResult.OK)
                {
                    System.Diagnostics.Process.Start("https://aka.ms/dotnet-download");
                }
            }
            return result;
        }

        private static bool DetectEnviroment()
        {
            return DetectNetCore() && DetectVC2015();
        }


        //const string CurrentPackgeName = "FiiiCoin-0.4.6-Win.exe";
        //const string PackgeNameRegex = @"FiiiCoin-.\..\..-Win.exe";
        const string CurrentPackgeName = "WDG-1.0.2-Win.exe";
        const string PackgeNameRegex = @"WDG-.\..\..-Win.exe";


        private static void DelVersionData()
        {
            DelVersionExe();
            DelLogs();
        }

        private static void DelVersionExe()
        {
            try
            {
                var fileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory).Select(file => Path.GetFileName(file));
                var regex = new Regex(PackgeNameRegex);
                var deleteFiles = fileNames.Where(x => regex.Match(x).Length > 0).ToList();
                deleteFiles.RemoveAll(x => x.Equals(CurrentPackgeName));
                deleteFiles.ForEach(x => File.Delete(x));
            }
            catch
            {

            }
        }

        const string LogDir = "Logs";
        private static void DelLogs()
        {
            
            if (Directory.Exists(LogDir))
            {
                try
                {
                    Directory.Delete(LogDir, true);
                }
                catch (Exception ex)
                {
                    var s = ex;
                }
            }
        }

    }
}
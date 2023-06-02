﻿using VeraSoft.Wpf.Core;

namespace VeraSoft
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Bootstrapper
    {
        public override void ConfigurePluginNames()
        {
            MefDllNamesWhiteList = new System.Collections.Generic.List<string>();
            MefDllNamesWhiteList.Add("VeraSoft");
            MefDllNamesBlackList = new System.Collections.Generic.List<string>();
        }

        //protected override void LoadNonManagedDlls()
        //{
        //}
    }
}

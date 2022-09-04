﻿using FamilyFinance.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyFinance.ViewModel.MainVM
{
    public class MainVM : INotifyPropertyChanged
    {
        public MainVM()
        {
        }

        public Role UserRole { get { return DataManager.UserRole; } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

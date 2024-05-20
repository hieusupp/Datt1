using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Microsoft.WindowsAPICodePack.Dialogs;
using OfficeOpenXml;

namespace AlphaBIM
{
    public partial class ExportScheduleToExcelWindow
    {
        private ExportScheduleToExcelViewModel _viewModel;

        public ExportScheduleToExcelWindow(ExportScheduleToExcelViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;


          
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
           

            DialogResult = true;
            Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}

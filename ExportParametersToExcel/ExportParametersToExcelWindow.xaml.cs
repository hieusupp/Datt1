using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace AlphaBIM
{
    public partial class ExportParametersToExcelWindow
    {
        private ExportParametersToExcelViewModel _viewModel;

        public ExportParametersToExcelWindow(ExportParametersToExcelViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;

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

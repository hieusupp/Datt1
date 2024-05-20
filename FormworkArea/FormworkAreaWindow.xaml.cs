using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.IO;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;



namespace AlphaBIM
{
    public partial class FormworkAreaWindow
    {
        private FormworkAreaViewModel _viewModel;

        public FormworkAreaWindow(FormworkAreaViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                DialogResult = true;
                Close();
            }

            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClickOk();
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnEx_Click(object sender, RoutedEventArgs e)
        {

        }
    } 
}

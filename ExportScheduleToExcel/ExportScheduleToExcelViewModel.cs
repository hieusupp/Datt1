#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = Autodesk.Revit.DB.View;

#endregion

namespace AlphaBIM
{
    public class ExportScheduleToExcelViewModel : ViewModelBase
    {
        public ExportScheduleToExcelViewModel(UIDocument uiDoc)
        {
            // Khởi tạo sự kiện(nếu có) | Initialize event (if have)

            // Lưu trữ data từ Revit | Store data from Revit
            UiDoc = uiDoc;
            Doc = UiDoc.Document;

            // Khởi tạo data cho WPF | Initialize data for WPF
            Initialize();

            // Get setting(if have)

        }

        private void Initialize()
        {
            List<ViewSchedule> allViewSchedule =
                new FilteredElementCollector(Doc)
                    .OfCategory(BuiltInCategory.OST_Schedules)
                    .Cast<ViewSchedule>()
                    .Where(vs => vs.CropBox != null)
                    .Where(vs => vs.Definition.CategoryId.IntegerValue
                                 != (int)BuiltInCategory.OST_Revisions)
                    .ToList();

            foreach (ViewSchedule v in allViewSchedule)
            {
                ViewScheduleExtension viewExtension
                    = new ViewScheduleExtension(v);

                AllViewSchedules.Add(viewExtension);
            }

            AllViewSchedules.Sort((v1, v2)
                => string.CompareOrdinal(v1.ViewScheduleName, v2.ViewScheduleName));

            ExportExcelFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;


        #region Binding properties

        public string ExportExcelFolderPath
        {
            get => _exportExcelFolderPath;
            set
            {
                _exportExcelFolderPath = value;
                OnPropertyChanged();
            }
        }

        public List<ViewScheduleExtension> AllViewSchedules { get; set; } = new List<ViewScheduleExtension>();
        public List<ViewScheduleExtension> SelectedViewSchedules { get; set; } = new List<ViewScheduleExtension>();

        public bool IsMultiWorkbooks { get; set; }
        public bool IsOneWorkbook { get; set; } = true;

        #endregion Binding properties


        #endregion public property

        #region private variable

        private string _exportExcelFolderPath;


        #endregion private variable

        // Các method khác viết ở dưới đây | Other methods written below
    }
}
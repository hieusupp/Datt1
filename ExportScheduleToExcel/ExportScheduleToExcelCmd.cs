#region Namespaces

using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class ExportScheduleToExcelCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;



            // Khi ch?y b?ng Add-in Manager ho?c Debug thì commnent 2 dòng bên d??i ?? tránh l?i    
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);
         
        ExportScheduleToExcelViewModel viewModel =
                      new ExportScheduleToExcelViewModel(uidoc);

        ExportScheduleToExcelWindow window =
            new ExportScheduleToExcelWindow(viewModel);

            if (window.ShowDialog() == false) return Result.Cancelled;

            return Result.Succeeded;

        }
}
}



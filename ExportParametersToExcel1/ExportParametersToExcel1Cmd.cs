
#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using XtraCs;
using Application = Autodesk.Revit.ApplicationServices.Application;
using X = Microsoft.Office.Interop.Excel;
#endregion

namespace AlphaBIM
{
    [Transaction(TransactionMode.Manual)]
    public class ExportParametersToExcel1Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Khi chạy bằng Add-in Manager thì comment 2 dòng bên dưới để tránh lỗi
            // When running with Add-in Manager, comment the 2 lines below to avoid errors
            //string dllFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //AssemblyLoader.LoadAllRibbonAssemblies(dllFolder);


            // code here
            Dictionary<string, List<Element>> sortedElements = new Dictionary<string, List<Element>>();
            FilteredElementCollector collector= new FilteredElementCollector(doc).WhereElementIsNotElementType();

            foreach (Element e in collector)
            {
                Debug.Assert(!(e is ElementType),
                  "expected no ElementType elements");

                Category category = e.Category;

                if (null != category)
                {
                    string name = category.Name;

                    List<Element> elementSet;

                    // if we already have this key, get its value (set);
                    // otherwise, create the new key and value (set):

                    if (sortedElements.ContainsKey(name))
                    {
                        elementSet = sortedElements[name];
                    }
                    else
                    {
                        elementSet = new List<Element>();
                        sortedElements.Add(name, elementSet);
                    }
                    elementSet.Add(e);
                }
            }
            X.Application excel = new X.Application();
            if (null == excel)
            {
                LabUtils.ErrorMsg("Failed to get or start Excel.");
                return Result.Failed;
            }
            excel.Visible = true;
            X.Workbook workbook = excel.Workbooks.Add(Missing.Value);
            X.Worksheet worksheet;

            while (1 < workbook.Sheets.Count) // we cannot delete all work sheets, excel requires at least one
            {
                worksheet = workbook.Sheets.get_Item(1) as X.Worksheet;
                worksheet.Delete();
            }
            List< string > keys = new List<string>(sortedElements.Keys);
            keys.Sort();
            keys.Reverse(); // the worksheet added last shows up first in the excel tab
            bool first = true;
            foreach (string categoryName in keys)
            {
                List<Element> elementSet = sortedElements[categoryName];

                // create and name the worksheet

                if (first)
                {
                    worksheet = workbook.Sheets.get_Item(1) as X.Worksheet;
                    first = false;
                }
                else
                {
                    worksheet = excel.Worksheets.Add(Missing.Value, Missing.Value,
                      Missing.Value, Missing.Value) as X.Worksheet;
                }

                worksheet.Name = (31 < categoryName.Length)
                  ? categoryName.Substring(0, 31)
                  : categoryName;

                // we could find the list of Parameter names available for ALL the Elements
                // in this Set, but let's keep it simple and use all parameters encountered:

                List<string> allParamNamesEncountered = new List<string>();

                // loop through all the elements passed to the method

                foreach (Element e in elementSet)
                {
                    ParameterSet parameters = e.Parameters;

                    // an easier way to loop the parameters than ParameterSetIterator:

                    foreach (Parameter parameter in parameters)
                    {
                        string name = parameter.Definition.Name;
                        if (!allParamNamesEncountered.Contains(name))
                        {
                            allParamNamesEncountered.Add(name);
                        }
                    }
                }
                allParamNamesEncountered.Sort();

                // add the HEADER row in Bold

                worksheet.Cells[1, 1] = "ID";
                int column = 2;

                foreach (string paramName in allParamNamesEncountered)
                {
                    worksheet.Cells[1, column] = paramName;
                    ++column;
                }
                worksheet.get_Range("A1", "Z1").Font.Bold = true;
                worksheet.get_Range("A1", "Z1").EntireColumn.AutoFit();
                int row = 2;
                foreach (Element e in elementSet)
                {
                    // first column is the element id, which we display as an integer

                    worksheet.Cells[row, 1] = e.Id.IntegerValue;
                    column = 2;
                    foreach (string paramName in allParamNamesEncountered)
                    {
                        string paramValue;
                        try
                        {
                            paramValue = LabUtils.GetParameterValue(e.LookupParameter(paramName));
                        }
                        catch (Exception)
                        {
                            paramValue = "*NA*";
                        }
                        worksheet.Cells[row, column] = paramValue;
                        ++column;
                    }
                    ++row;
                } // row
            } // category = worksheet

            return Result.Succeeded;
        }
    }
}

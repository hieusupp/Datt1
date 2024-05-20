using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace AlphaBIM
{
    internal static class DocumentUtils
    {
        internal static List<ParameterFilterElement> GetAllRuleFilters(this Document doc)
        {
            List<ParameterFilterElement> allRuleFilters = new FilteredElementCollector(doc)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .ToList();

            return allRuleFilters;
        }

        internal static List<SelectionFilterElement> GetAllSelectionFilters(this Document doc)
        {
            List<SelectionFilterElement> allSelectionFilters = new FilteredElementCollector(doc)
                .OfClass(typeof(SelectionFilterElement))
                .Cast<SelectionFilterElement>().ToList();

            return allSelectionFilters;
        }
    }
}

using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace AlphaBIM
{
    public class FilterExtension : ViewModelBase
    {
        public string FilterName { get; set; }
        public ElementId FilterId { get; set; }

        private bool _isFilterSelected;

        public bool IsFilterSelected
        {
            get => _isFilterSelected;
            set { _isFilterSelected = value; OnPropertyChanged(); }
        }


        public FilterExtension(Document doc, ElementId elementId)
        {
            FilterId = elementId;
            FilterName = doc.GetElement(elementId).Name;
        }
    }
}
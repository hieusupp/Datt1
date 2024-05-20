using Autodesk.Revit.DB;

namespace AlphaBIM
{
    public class ViewExtension : ViewModelBase
    {
        private bool _isSelected;
        public View View { get; set; }
        public string Name { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public ViewExtension(View v)
        {
            View = v;
            Name = v.Name;
            IsSelected = true;
        }

    }
}
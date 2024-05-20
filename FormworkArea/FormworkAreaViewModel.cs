    #region Namespaces
using AlphaBIM.Models;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using TINHKHOILUONGVANKHUON;

#endregion

namespace AlphaBIM
{
    public class FormworkAreaViewModel : ViewModelBase
    {
        public FormworkAreaViewModel(UIDocument uiDoc)
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
            //Tạo share para cần thiết
            CreateShareParameterFormworkArea.CreateShareParameter(UiDoc);

            //Khởi tạo sự kiện

            IList<Reference> references = UiDoc.Selection.PickObjects(ObjectType.Element,   
                                          new BeamColumnFloorSelectionFilter(), "Chọn cột, dầm và sàn");

            // Các đối tượng được chọn được lưu vào biến BeamColumnsFloor
            BeamColumnsFloor = references.Select(x => Doc.GetElement(x)).ToList();
          
        }

        #region public property

        public UIDocument UiDoc;
        public Document Doc;

        #endregion public property

        #region private variable

        //private double _percent;
        //private IEnumerable<Element> intersectElements;

        #endregion private variable

         #region Binding properties
        public List<Element> BeamColumnsFloor { get; set; }
     


        //List<Element> intersectElements;
        public bool IsCurrentViewScope { get; set; }
        
        public bool IsCalBeamBottom { get; set; }
        public bool IsCalFloorBottom { get; set; }
        public bool IsCalFloorSubBeam { get; set; }

        #endregion Binding properties

        // Các method khác viết ở dưới đây | Other methods written below
        internal void ClickOk()
        {
           
            using (Transaction tran = new Transaction(Doc, "Formwork"))
            {
                tran.Start();
                foreach (Element element in BeamColumnsFloor)
                {
                    // Mỗi đối tượng sẽ lấy ra được solid 
                    Solid solid1 = element.GetSolid();// Lấy được đối tượng rồi 

                    #region // Lấy về tất cả đối tượng intersect
                    IList<ElementFilter> filters = new List<ElementFilter>();
                    filters.Add(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming));
                    filters.Add(new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns));
                    filters.Add(new ElementCategoryFilter(BuiltInCategory.OST_Floors));
                   
                    BoundingBoxXYZ box = element.get_BoundingBox(Doc.ActiveView);
                    Outline outline = new Outline(box.Min, box.Max);
                    BoundingBoxIntersectsFilter bbFilter
                        = new BoundingBoxIntersectsFilter(outline);// Làm giảm các đối tượng 

                    LogicalOrFilter logicalOrFilter
                        = new LogicalOrFilter(filters);
                    LogicalAndFilter logicalAndFilter
                        = new LogicalAndFilter(new List<ElementFilter>()
                        {
                            logicalOrFilter,bbFilter
                        });// Lấy ra được các đối tượng có cùng category và giới hạn trong  BoundingBox
                         
                    List<Element> intersectElements;
                    if (IsCurrentViewScope)
                    {
                        intersectElements
                           = new FilteredElementCollector(Doc)
                               .WherePasses(logicalAndFilter)
                               .ToList();
                    }
                    else
                    {
                        intersectElements
                            = new FilteredElementCollector(Doc, BeamColumnsFloor.Select(x => x.Id).ToList())
                                .WherePasses(logicalAndFilter)
                                .ToList();
                    }

                   
                    //Lấy được các đối tượng giao nhau 
                    intersectElements = intersectElements
                        .Where(x => x.Id.IntegerValue != element.Id.IntegerValue)
                        .ToList();// Loại bỏ đối tượng ở đâu có Id khác đối tượng formwork
                    #endregion
                    
                    //có đc các đối tượng dầm cột giao nhau 
                    List<Face> faces1 = solid1.GetFaces();// có diện tích của face
                    double solid1Area = 0;
                    double beamBottom = 0;
                    double floorBottom = 0;
                    double floorSides =0;

                    #region Cột , dầm ,san
                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                    {
                        solid1Area = faces1.CalAreaNotTopNotBottom();// không tính  trên và dưới 
                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {
                        if (!IsCalBeamBottom)
                        {
                            solid1Area = faces1.CalAreaNotTopNotBottom();  
                        }
                        else
                        {
                            solid1Area = faces1.CalAreaNotTop();
                            beamBottom = faces1.CalAreaOnlyBottom();// diện tích đáy dầm
                        }
                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                    {                        
                        if (IsCalFloorBottom)
                        {                         
                            floorBottom = faces1.CalAreaOnlyBottom();
                            floorSides = faces1.CalAreaNotTopNotBottom();
                            solid1Area = faces1.CalAreaNotTop();
                        }
                        if (IsCalFloorSubBeam)
                        {
                           // solid1Area = faces1.CalAreaNotTop();
                        }    
                       
                    }
                    #endregion

                    double totalArea = solid1Area;

                    double beamSubBeam = 0;
                    double beamSubCol = 0;
                    double beamSubFloor = 0;

                    double colSubCol = 0;
                    double colSubBeam = 0;
                    double colSubFloor = 0;

                    double floorSubCol = 0;
                    double floorSubBeam = 0;
                    double floorSubFloor = 0;

                    foreach (Element intersectElement in intersectElements)
                    {
                        Solid solid2 = intersectElement.GetSolid();
                        List<Face> faces2 = solid2.GetFaces();
                        double solid2Area = 0;
                        #region Cột dầm
                        if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                        {
                            solid2Area = faces2.CalAreaNotTopNotBottom();
                        }
                        else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                        {
                            if (!IsCalBeamBottom)
                            {
                                solid2Area = faces2.CalAreaNotTopNotBottom();
                            }
                            else
                            {
                                solid2Area = faces2.CalAreaNotTop();
                            }
                        }
                        else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                        {
                            //solid2Area = faces2.CalAreaNotTopNotBottom();
                            if (IsCalFloorBottom)
                            {                             
                                solid2Area = faces2.CalAreaNotTop();
                            }
                            if (IsCalFloorSubBeam)
                            {
                                //double totalArea2 = 0;
                                //double top = 0;
                                solid2Area = faces2.CalAreaNotTop();
                                //totalArea2 = faces2.CalAreaAll();
                                //top = faces2.CalAreaAll() - faces2.CalAreaNotTop();
                                //solid2Area = totalArea2 - top;
                            }

                        }

                        #endregion
                        try
                        {
                            Solid union = BooleanOperationsUtils.ExecuteBooleanOperation(
                                solid1,
                                solid2,
                                BooleanOperationsType.Union);
                            List<Face> facesUnion = union.GetFaces();
                            double uionArea = 0;
                            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                            {
                                uionArea = facesUnion.CalAreaNotTopNotBottom();
                            }
                            else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                            {
                                if (!IsCalBeamBottom)
                                {
                                    uionArea = facesUnion.CalAreaNotTopNotBottom();
                                }
                                else
                                {
                                    uionArea = facesUnion.CalAreaNotTop();
                                }
                            }
                            else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                            {
                                //uionArea = facesUnion.CalAreaNotTopNotBottom();
                                if (IsCalFloorBottom)
                                {                                   
                                    uionArea = facesUnion.CalAreaNotTop(); 
                                }
                                if (IsCalFloorSubBeam)
                                {
                                    uionArea = facesUnion.CalAreaNotTop();
                                }

                            }

                            double areaIntersect = (solid1Area + solid2Area - uionArea) / 2;

                            if (areaIntersect > 0)
                            {
                                if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        colSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        colSubBeam += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                                    {
                                        colSubFloor += areaIntersect;
                                    }
                                }
                                else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        beamSubCol += areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        beamSubBeam += areaIntersect;
                                    }
                                    if (IsCalBeamBottom)
                                    {
                                        beamBottom -= faces1.CalAreaOnlyBottom()
                                                    + faces2.CalAreaOnlyBottom()
                                                    - facesUnion.CalAreaOnlyBottom();
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                                    {
                                        beamSubFloor += areaIntersect;
                                    }
                                }
                                else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                                {
                                    if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                                    {
                                        floorSubCol = areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                                    {
                                        floorSubBeam = areaIntersect;
                                    }
                                    else if (intersectElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                                    {
                                        floorSubFloor += areaIntersect;
                                    }
                                    if (IsCalFloorBottom)
                                    {
                                        floorBottom -= faces1.CalAreaOnlyBottom()
                                                + faces2.CalAreaOnlyBottom()
                                                - facesUnion.CalAreaOnlyBottom(); 
                                    }
                                    if (IsCalFloorSubBeam)
                                    {
                                        floorSubBeam -= areaIntersect;
                                    }

                                }
                            }
                        }
                        catch (Exception  )
                        {
                            //break;
                        }
                    }
                    #region Cột dầm sàn
                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                    {
                        Parameter nameAlpFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlpFormworkArea);
                        Parameter nameFwColumnTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnTotal);
                        Parameter nameFwColumnSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubBeam);
                        Parameter nameFwColumnSubFloor = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubFloor);
                        Parameter nameFwColumnSubColumn = element.LookupParameter(CreateShareParameterFormworkArea.NameFwColumnSubColumn);
                        nameAlpFormworkArea.Set(0);
                        nameFwColumnTotal.Set(0);
                        nameFwColumnSubBeam.Set(0);
                        nameFwColumnSubColumn.Set(0);
                        nameFwColumnSubFloor.Set(0);    

                        nameAlpFormworkArea.Set(totalArea - colSubBeam - colSubCol- colSubFloor );
                        nameFwColumnTotal.Set(totalArea);
                        nameFwColumnSubBeam.Set(colSubBeam);
                        nameFwColumnSubFloor.Set(colSubFloor);
                        nameFwColumnSubColumn.Set(colSubCol);
                    }
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                    {
                        Parameter nameAlpFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlpFormworkArea);
                        Parameter nameFwBeamTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamTotal);
                        Parameter nameFwBeamBottom = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamBottom);
                        Parameter nameFwBeamSubCol =element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubCol);          
                        Parameter nameFwBeamSubFloor = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubFloor);
                        Parameter nameFwBeamSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwBeamSubBeam);
                        nameAlpFormworkArea.Set(0);
                        nameFwBeamTotal.Set(0);
                        nameFwBeamBottom.Set(0);
                        nameFwBeamSubCol.Set(0);    
                        nameFwBeamSubFloor.Set(0);
                        nameFwBeamSubBeam.Set(0);

                        nameAlpFormworkArea.Set(totalArea - beamSubCol - beamSubBeam - beamSubFloor);
                        nameFwBeamTotal.Set(totalArea);
                        if (IsCalBeamBottom)
                        {
                            nameFwBeamBottom.Set(beamBottom);
                        }
                        nameFwBeamSubCol.Set(beamSubCol);
                        nameFwBeamSubFloor.Set(beamSubFloor);
                        nameFwBeamSubBeam.Set(beamSubBeam);
                    }
                    

                    //Sàn 
                    else if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                    {
                        Parameter nameAlpFormworkArea = element.LookupParameter(CreateShareParameterFormworkArea.NameAlpFormworkArea);
                        Parameter nameFwFloorTotal = element.LookupParameter(CreateShareParameterFormworkArea.NameFwFloorTotal);
                        Parameter nameFwFloorSides = element.LookupParameter(CreateShareParameterFormworkArea.NameFwFloorSides);
                        Parameter nameFwFloorBottom = element.LookupParameter(CreateShareParameterFormworkArea.NameFwFloorBottom);
                        Parameter nameFwFloorSubCol = element.LookupParameter(CreateShareParameterFormworkArea.NameFwFloorSubCol);
                        Parameter nameFwFloorSubBeam = element.LookupParameter(CreateShareParameterFormworkArea.NameFwFloorSubBeam);
                        Parameter nameFwFloorSubFloor = element.LookupParameter(CreateShareParameterFormworkArea.NameFwFloorSubFloor);
                        nameAlpFormworkArea.Set(0);
                        nameFwFloorTotal.Set(0);
                        nameFwFloorSides.Set(0);
                        nameFwFloorBottom.Set(0);
                        nameFwFloorSubCol.Set(0);
                        nameFwFloorSubBeam.Set(0);
                        nameFwFloorSubFloor.Set(0);

                        nameFwFloorTotal.Set(totalArea);
                        nameFwFloorSides.Set(floorSides);
                        if (IsCalFloorBottom)
                        {                   
                            nameFwFloorBottom.Set(floorBottom);
                            nameAlpFormworkArea.Set(totalArea - floorSubCol - floorSubBeam - floorSubFloor);
                        }  
                        if(IsCalFloorSubBeam)
                        {
                            nameAlpFormworkArea.Set(floorBottom + floorSides - floorSubCol - floorSubBeam - floorSubFloor);
                        }    
                        nameFwFloorSubCol.Set(floorSubCol);
                        nameFwFloorSubBeam.Set(floorSubBeam);
                        nameFwFloorSubFloor.Set(floorSubFloor);
                    }


                    #endregion
                }

                tran.Commit();
            }

        }
    }
}


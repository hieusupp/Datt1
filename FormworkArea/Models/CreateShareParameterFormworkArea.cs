using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBIM;
using Autodesk.Revit.ApplicationServices;
using  Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TINHKHOILUONGVANKHUON
{
   internal class CreateShareParameterFormworkArea
    {
        internal static void CreateShareParameter(UIDocument uiDoc)
        {
            Document doc = uiDoc.Document;
            Application app = uiDoc.Application.Application;

            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path = System.IO.Path.Combine(pathDesktop, "ALB_ShareParameter.txt");
            string group = "STR";

            List<Category> categories = new List<Category>();
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming));
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns));
            categories.Add(Category.GetCategory(doc, BuiltInCategory.OST_Floors));

           
            Transaction t = new Transaction(doc, " Tạo ShareParameter");
            t.Start();

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameAlpFormworkArea,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionAlFormworkArea,
                categories,
                true);
            
            
            #region Para của dầm 
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamTotal,
                categories,
                true);

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamBottom,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamBottom,
                categories,
                true);
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubCol,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubCol,
                categories,
                true);
            ParameterUtils.CreateSharedParamater(
               doc,
               app,
               path,
               group,
               NameFwBeamSubFloor,
               ParameterType.Area,
               BuiltInParameterGroup.PG_STRUCTURAL,
               DescriptionFwBeamSubFloor,
               categories,
               true);
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwBeamSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwBeamSubBeam,
                categories,
                true);
            #endregion

            #region Para của cột 
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnTotal,
                categories,
                true);
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubBeam,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubBeam,
                categories,
                true); 
            ParameterUtils.CreateSharedParamater(
               doc,
               app,
               path,
               group,
               NameFwColumnSubFloor,
               ParameterType.Area,
               BuiltInParameterGroup.PG_STRUCTURAL,
               DescriptionFwColumnSubFloor,
               categories,
               true);
            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwColumnSubColumn,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwColumnSubColumn,
                categories,
                true);
            #endregion


            #region Para của sàn

            ParameterUtils.CreateSharedParamater(
                doc,
                app,
                path,
                group,
                NameFwFloorTotal,
                ParameterType.Area,
                BuiltInParameterGroup.PG_STRUCTURAL,
                DescriptionFwFloorTotal,
                categories,
                true);
            ParameterUtils.CreateSharedParamater(
               doc,
               app,
               path,
               group,
               NameFwFloorSides,
               ParameterType.Area,
               BuiltInParameterGroup.PG_STRUCTURAL,
               DescriptionFwFloorSides,
               categories,
               true);
            ParameterUtils.CreateSharedParamater(
              doc,
              app,
              path,
              group,
              NameFwFloorBottom,
              ParameterType.Area,
              BuiltInParameterGroup.PG_STRUCTURAL,
              DescriptionFwFloorBottom,
              categories,
              true);
            ParameterUtils.CreateSharedParamater(
              doc,
              app,
              path,
              group,
              NameFwFloorSubBeam,
              ParameterType.Area,
              BuiltInParameterGroup.PG_STRUCTURAL,
              DescriptionFwFloorSubBeam,
              categories,
              true);
            ParameterUtils.CreateSharedParamater(
             doc,
             app,
             path,
             group,
             NameFwFloorSubCol,
             ParameterType.Area,
             BuiltInParameterGroup.PG_STRUCTURAL,
             DescriptionFwFloorSubCol,
             categories,
             true);
            ParameterUtils.CreateSharedParamater(
            doc,
            app,
            path,
            group,
            NameFwFloorSubFloor,
            ParameterType.Area,
            BuiltInParameterGroup.PG_STRUCTURAL,
            DescriptionFwFloorSubFloor,
            categories,
            true);
            #endregion

            t.Commit();
        }



        internal static String NameAlpFormworkArea { get; set; } = "FW_FormworkArea";
        internal static String DescriptionAlFormworkArea { get; set; } = "Diện tích ván khuôn";

        #region Dầm 
        internal static string NameFwBeamTotal { get; set; } = "FW.Beam.Total";
        internal static string DescriptionFwBeamTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwBeamBottom { get; set; } = "FW.Beam.Bottom";
        internal static string DescriptionFwBeamBottom { get; set; } = "Diện tích ván khuôn đáy";
        internal static string NameFwBeamSubCol { get; set; } = "FW.Beam.SubCol";
        internal static string DescriptionFwBeamSubCol { get; set; } = "Diện tích tiếp xúc với cột";
        internal static string NameFwBeamSubFloor { get; set; } = "FW.Beam.SubFloor";
        internal static string DescriptionFwBeamSubFloor { get; set; } = "Diện tích tiếp xúc với sàn";
        internal static string NameFwBeamSubBeam { get; set; } = "FW.Beam.SubBeam";
        internal static string DescriptionFwBeamSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
       
        #endregion

        #region Cột
        internal static string NameFwColumnTotal { get; set; } = "FW.Colum.Total";
        internal static string DescriptionFwColumnTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwColumnSubBeam { get; set; } = "FW.Colum.SubBeam";
        internal static string DescriptionFwColumnSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
        internal static string NameFwColumnSubFloor { get; set; } = "FW.Colum.SubFloor";
        internal static string DescriptionFwColumnSubFloor { get; set; } = "Diện tích tiếp xúc với sàn";
        internal static string NameFwColumnSubColumn { get; set; } = "FW.Colum.SubColum";
        internal static string DescriptionFwColumnSubColumn { get; set; } = "Diện tích tiếp xúc với cột";
       
        #endregion

        #region Sàn
        internal static string NameFwFloorTotal { get; set; } = "FW.Floor.Total";
        internal static string DescriptionFwFloorTotal { get; set; } = "Tổng diện tích ván khuôn";
        internal static string NameFwFloorSides { get; set; } = "FW.Floor.Sides";
        internal static string DescriptionFwFloorSides { get; set; } = "Tổng diện tích xung quanh";
        internal static string NameFwFloorBottom { get; set; } = "FW.Floor.Bottom";
        internal static string DescriptionFwFloorBottom { get; set; } = "Diện tích ván khuôn đáy";
        internal static string NameFwFloorSubBeam { get; set; } = "FW.Floor.SubBeam";
        internal static string DescriptionFwFloorSubBeam { get; set; } = "Diện tích tiếp xúc với dầm";
        internal static string NameFwFloorSubCol { get; set; } = "FW.Floor.SubColum";
        internal static string DescriptionFwFloorSubCol { get; set; } = "Diện tích tiếp xúc với cột";
        internal static string NameFwFloorSubFloor { get; set; } = "FW.Floor.SubFloor";
        internal static string DescriptionFwFloorSubFloor { get; set; } = "Diện tích tiếp xúc với sàn";
        #endregion
    }
}

#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace AddSharedParamSample
{
    [Transaction(TransactionMode.Manual)]
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            SampleFamilyLoadOptions familyLoadOptions = new SampleFamilyLoadOptions();
            Family myFamily = null;
            string famPath = @"C:\Temp\EL-Wall Base.rfa";

            Transaction t = new Transaction(doc);
            t.Start("Load Family");
            doc.LoadFamily(famPath, familyLoadOptions, out myFamily);
            t.Commit();
            t.Dispose();
            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
    public class SampleFamilyLoadOptions : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            if (!familyInUse)
            {
                //TaskDialog.Show("SampleFamilyLoadOptions", "The family has not been in use and will keep loading.");

                overwriteParameterValues = true;
                return true;
            }
            else
            {
                //TaskDialog.Show("SampleFamilyLoadOptions", "The family has been in use but will still be loaded with existing parameters overwritten.");

                overwriteParameterValues = true;
                return true;
            }
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            if (!familyInUse)
            {
                //TaskDialog.Show("SampleFamilyLoadOptions", "The shared family has not been in use and will keep loading.");

                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
            else
            {
                //TaskDialog.Show("SampleFamilyLoadOptions", "The shared family has been in use but will still be loaded from the FamilySource with existing parameters overwritten.");

                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
        }
    }
}

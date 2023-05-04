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
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            string paramName = "Ceiling Height";

            // check if param exists
            if(Utils.DoesProjectParamExist(doc, paramName))
            {
                TaskDialog.Show("Error", "Parameter already exists.");
                return Result.Failed;
            }
            else
            {
                // add shared param to project
                Utils.CreateSharedParam(doc, "Rooms", paramName, BuiltInCategory.OST_Rooms);
            }

            // if not, the create from shared param file



            return Result.Succeeded;
        }

        

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}

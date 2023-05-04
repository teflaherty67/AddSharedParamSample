using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddSharedParamSample
{
    internal static class Utils
    {
        public struct ParameterData
        {
            public Definition def;
            public ElementBinding binding;
            public string name;
            public bool IsSharedStatusKnown;
            public bool IsShared;
            public string GUID;
            public ElementId id;
        }
        public static List<ParameterData> GetAllProjectParameters(Document curDoc)
        {
            if (curDoc.IsFamilyDocument)
            {
                TaskDialog.Show("Error", "Cannot be a family document.");
                return null;
            }

            List<ParameterData> paraList = new List<ParameterData>();

            BindingMap map = curDoc.ParameterBindings;
            DefinitionBindingMapIterator iter = map.ForwardIterator();
            iter.Reset();
            while (iter.MoveNext())
            {
                ParameterData pd = new ParameterData();
                pd.def = iter.Key;
                pd.name = iter.Key.Name;
                pd.binding = iter.Current as ElementBinding;
                paraList.Add(pd);
            }

            return paraList;
        }

        public static bool DoesProjectParamExist(Document curDoc, string pName)
        {
            List<ParameterData> pdList = GetAllProjectParameters(curDoc);
            foreach (ParameterData pd in pdList)
            {
                if (pd.name == pName)
                {
                    return true;
                }
            }
            return false;
        }
        public static void CreateSharedParam(Document curDoc, string groupName, string paramName, BuiltInCategory cat)
        {
            Definition curDef = null;

            //check if current file has shared param file - if not then exit
            DefinitionFile defFile = curDoc.Application.OpenSharedParameterFile();

            //check if file has shared parameter file
            if (defFile == null)
            {
                TaskDialog.Show("Error", "No shared parameter file.");
                //Throw New Exception("No Shared Parameter File!")
            }

            //check if shared parameter exists in shared param file - if not then create
            if (ParamExists(defFile.Groups, groupName, paramName) == false)
            {
                //create param
                curDef = AddParamToFile(defFile, groupName, paramName);
            }

            //check if param is added to views - if not then add
            if (ParamAddedToFile(curDoc, paramName) == false)
            {
                //add parameter to current Revitfile
                AddParamToDocument(curDoc, curDef, cat);
            }
        }

        //check if specified parameter is already added to Revit file
        public static bool ParamAddedToFile(Document curDoc, string paramName)
        {
            foreach (Parameter curParam in curDoc.ProjectInformation.Parameters)
            {
                if (curParam.Definition.Name.Equals(paramName))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AddParamToDocument(Document curDoc, Definition curDef, BuiltInCategory cat)
        {
            bool paramAdded = false;

            //define category for shared param
            Category myCat = curDoc.Settings.Categories.get_Item(cat);
            CategorySet myCatSet = curDoc.Application.Create.NewCategorySet();
            myCatSet.Insert(myCat);

            //create binding
            ElementBinding curBinding = curDoc.Application.Create.NewInstanceBinding(myCatSet);

            //insert definition into binding
            using (Transaction curTrans = new Transaction(curDoc, "Added Shared Parameter"))
            {
                if (curTrans.Start() == TransactionStatus.Started)
                {
                    //do something
                    paramAdded = curDoc.ParameterBindings.Insert(curDef, curBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                }

                //commit changes
                curTrans.Commit();
            }

            return paramAdded;
        }

        //check if specified parameter exists in shared parameter file
        public static bool ParamExists(DefinitionGroups groupList, string groupName, string paramName)
        {
            //loop through groups and look for match
            foreach (DefinitionGroup curGroup in groupList)
            {
                if (curGroup.Name.Equals(groupName) == true)
                {
                    //check if param exists
                    foreach (Definition curDef in curGroup.Definitions)
                    {
                        if (curDef.Name.Equals(paramName))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //add parameter to specified shared parameter file
        public static Definition AddParamToFile(DefinitionFile defFile, string groupName, string paramName)
        {
            //create new shared parameter in specified file
            DefinitionGroup defGroup = GetDefinitionGroup(defFile, groupName);

            //check if group exists - if not then create
            if (defGroup == null)
            {
                //create group
                defGroup = defFile.Groups.Create(groupName);
            }

            //create parameter in group
            ExternalDefinitionCreationOptions curOptions = new ExternalDefinitionCreationOptions(paramName, SpecTypeId.String.Text);
            curOptions.Visible = true;

            Definition newParam = defGroup.Definitions.Create(curOptions);

            return newParam;
        }
        public static DefinitionGroup GetDefinitionGroup(DefinitionFile defFile, string groupName)
        {
            //loop through groups and look for match
            foreach (DefinitionGroup curGroup in defFile.Groups)
            {
                if (curGroup.Name.Equals(groupName))
                {
                    return curGroup;
                }
            }

            return null;
        }
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }
    }
}

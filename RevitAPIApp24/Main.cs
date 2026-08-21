using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIApp24
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                FilteredElementCollector collector = new FilteredElementCollector(doc);
                ICollection<Element> walls = collector
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsNotElementType()
                    .ToElements();


                List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(l => l.Elevation)
                .ToList();

                Level firstLevel = levels.FirstOrDefault();
                Level secondLevel = levels.Count > 1 ? levels[1] : null;

                int firstFloorCount = 0;
                int secondFloorCount = 0;         

                foreach (Element wall in walls)
                {
                    Level level = doc.GetElement(wall.LevelId) as Level;

                    if (level != null)
                    {
                        if (firstLevel != null && level.Id == firstLevel.Id)
                        {
                            firstFloorCount++;
                        }
                        else if (secondLevel != null && level.Id == secondLevel.Id)
                        {
                            secondFloorCount++;
                        }
                       
                    }
                }

                TaskDialog.Show("Кол-во стен на этажах", $"Первый этаж: {firstFloorCount} стен \nВторой этаж: {secondFloorCount} стен \n ");
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

        }
    }
}

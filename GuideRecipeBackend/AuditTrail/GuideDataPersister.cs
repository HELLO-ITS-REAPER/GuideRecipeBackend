using GO;
using GO.Guide;
using GO.Guide.Actions;
using GO.Guide.Actions.Serialization;
using GO.Guide.DataLayer;
using GO.Mes.AuditTrail;
using GO.Windows.Extension;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuideRecipeBackend.Models;

namespace GuideRecipeBackend.AuditTrail
{
    [
        ValueContainerObjectType(new[] {
            typeof(NativeAction),
            typeof(Recipe)
        })
    ]
    public class GuideDataPersister : IObjectPersister
    {

        // We expect the auxData to contain information on the specific type of action
        public object CreateObject(Type objectType, string parentObjectIdPath, string auxData)
        {
            if (objectType == typeof(NativeAction))
            {
                return "{ name: \"new\" }";
            }
            if (objectType == typeof(Recipe))
            {
                var success = Guid.TryParse((string)parentObjectIdPath, out Guid actionId);
                var man = GO.AppManager.Current.ServiceManager.GetService<GuideManager>();

                var data = new SequenceAction()
                {
                    ActionId = actionId,
                    Name = "Guide2:NewRecipe",
                    Actions = { new StepAction() { Name = "StepAction", ActionId = Guid.NewGuid(), Action = new AndAction() } }
                };

                man.SaveAction(data, "create object");
                var action = man.LoadAction(data.ActionId);
                return action;
            }

            return null;
        }

        public void DeleteValueContainer(Type objectType, string objectId)
        {
            var man = GO.AppManager.Current.ServiceManager.GetService<GuideManager>();
            if (objectType == typeof(Recipe))
            {
                var success = Guid.TryParse((string)objectId, out Guid actionId);
                man.DeleteAction(actionId);
            }
            else if (objectType == typeof(NativeAction))
            {
                string actionId = objectId.Split(',').First();
                var recipe = (ICompositeAction)man.LoadAction(Guid.Parse(actionId));
                List<Models.ActionData> actionData = new List<Models.ActionData>();
                RebuildRecipeActions(recipe, actionData, objectId);
                var result = recipe.GetType().GetProperty("Actions").GetValue(recipe);
                //man.SaveAction(actionData, "Inline editing");

            }
        }

        private void RebuildRecipeActions(ICompositeAction compositeAction, List<Models.ActionData> actionData, string excludedActionId)
        {
            foreach (var action in compositeAction.Actions)
            {
                if (action.ActionId.ToString() == excludedActionId.Split(',').Last())
                {
                    continue;
                }
                var data = new Models.ActionData
                {
                    actionId = action.ActionId.ToString(),
                    Name = action.Name,
                    TypeName = action.GetInfo().TypeFullName,
                    AssemblyBaseName = action.GetInfo().AssemblyBaseName,
                };

                if (action is ICompositeAction nestedCompositeAction)
                {
                    RebuildRecipeActions(nestedCompositeAction, data.Actions, excludedActionId);
                }
                actionData.Add(data);
            }
        }

        public object GetObjectId(Type objectType, object data)
        {
            return -1;
        }

        public object UpdateValueContainer(Type objectType, string objectId, string changedPropertyName, object changedPropertyValue)
        {
            var man = GO.AppManager.Current.ServiceManager.GetService<GuideManager>();
            var action = man.LoadAction(Guid.Parse(objectId));
            var result = action.GetType().GetProperty(changedPropertyName).GetValue(action);
            action.GetType().GetProperty(changedPropertyName).SetValue(action, changedPropertyValue?.ToString(), null);
            man.SaveAction(action, "Inline editing");
            return result;
        }

        public void UpdateValueContainerCollection(Type parentType, string parentId, string collectionName, string childId, string updateType)
        {
            throw new NotImplementedException();
        }

        #region obsolete ValueContainer methods
        [Obsolete("Replaced by CreateObject")]
        public ValueContainer CreateValueContainer(Type objectType, string parentObjectId)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Replaced by GetObjectId")]
        public object GetId(Type objectType, ValueContainer container)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

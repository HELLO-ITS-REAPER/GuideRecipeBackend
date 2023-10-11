using GO;
using GO.Guide;
using GO.Guide.Actions;
using GO.Guide.Actions.Serialization;
using GO.Guide.DataLayer;
using GO.Owin;
using GO.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using GuideRecipeBackend.Models;

namespace GuideRecipeBackend
{
    public class GuideRecipeController : JsonRpcController
    {
        public GuideRecipeController()
        {
            FuncSet.Add("getAllRecipes", onGetAllRecipes);
            FuncSet.Add("getActionTypes", onGetActionTypes);
            FuncSet.Add("getRecipeActions", onGetRecipeActions);
        }

        [HttpPost]
        [Route("api/guide")]
        public object Main([FromBody] JsonRpcRequest request)
        {
            try
            {
                return FuncSet.Invoke(request, e => Error.WriteLine(e.Exception));
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred during call to {request.Method}: {ex.Message}", ex);
            }
        }

        private object onGetAllRecipes(dynamic arg)
        {
            var man = ServiceManager.GetService<GuideManager>();
            return man.Recipes;
        }

        private object onGetRecipeActions(dynamic arg)
        {
            var man = ServiceManager.GetService<GuideManager>();
            var success = Guid.TryParse((string)arg.actionId, out Guid actionId);

            var loadedAction = (ICompositeAction)man.LoadAction(actionId);
            List<Models.ActionData> actionsList = new List<Models.ActionData>();

            // Remember to make safety in case of no actions.
            CollectRecipeActionChilds(loadedAction, actionsList);

            return actionsList;
        }

        private void CollectRecipeActionChilds(ICompositeAction compositeAction, List<Models.ActionData> actionsList)
        {
            foreach (var action in compositeAction.Actions)
            {
                var actionData = new Models.ActionData
                {
                    actionId = action.ActionId.ToString(),
                    AssemblyBaseName = action.GetInfo().AssemblyBaseName,
                    Name = action.GetInfo().Name,
                    TypeName = action.GetInfo().TypeFullName,
                };

                foreach (var prop in action.GetProperties())
                {
                    if (typeof(InParameter).IsAssignableFrom(prop.PropertyType))
                    {
                        actionData.InputParameters.Add(new ActionParameterData() { Name = prop.Name, Description = prop.GetDescription(), TypeName = action.GetInfo().Properties?[0].Value.GetType().Name, Value = action.GetInfo().Properties?[0].Value });
                    }

                    if (typeof(OutParameter).IsAssignableFrom(prop.PropertyType))
                    {
                        actionData.OutputParameters.Add(new ActionParameterData() { Name = prop.Name, Description = prop.GetDescription(), TypeName = prop.PropertyType.GenericTypeArguments.ToString(), Value = null });
                    }
                }
                if (action is ICompositeAction nestedCompositeAction)
                {
                    CollectRecipeActionChilds(nestedCompositeAction, actionData.Actions);
                }

                actionsList.Add(actionData);
            }
        }


        private object onGetActionTypes(dynamic arg)
        {
            var man = ServiceManager.GetService<GuideManager>();
            var result = new List<ActionTypeData>();
            foreach (var type in man.ActionTypes)
            {
                var data = new ActionTypeData();
                data.TypeName = type.Name;
                data.AssemblyBaseName = GoExt.GoGetBaseName(type.Assembly);

                data.Description = ((ActionDescriptionAttribute)type.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).FirstOrDefault())?.Description;
                data.Category = ((CategoryAttribute)type.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault())?.Category;
                result.Add(data);
            }
            return result;
        }

        

        
    }
}

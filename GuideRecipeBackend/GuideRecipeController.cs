using GO;
using GO.Guide;
using GO.Guide.Actions;
using GO.Guide.Actions.Serialization;
using GO.Guide.DataLayer;
using GO.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GuideRecipeBackend
{
    public class GuideRecipeController : JsonRpcController
    {
        public GuideRecipeController()
        {
            FuncSet.Add("getAllRecipes", onGetAllRecipes);
            FuncSet.Add("getRecipe", onGetRecipe);
            FuncSet.Add("getActionTypes", onGetActionTypes);
            //FuncSet.Add("getActionData", onGetActionData);
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



        private object onGetRecipe(dynamic arg)
        {
            var man = ServiceManager.GetService<GuideManager>();
            var success = Guid.TryParse((string)arg.actionId, out Guid actionId);
            var recipe = man.LoadRecipe(actionId);
            return recipe;
        }

        private object onGetRecipeActions(dynamic arg)
        {
            var man = ServiceManager.GetService<GuideManager>();
            var success = Guid.TryParse((string)arg.actionId, out Guid actionId);

            var loadedAction = (ICompositeAction)man.LoadAction(actionId);

            var actions = loadedAction.Actions;

            var actionsList = new List<ActionData>();

            foreach (var action in actions)
            {
                var actionData = new ActionData
                {
                    AssemblyBaseName = action.Name,
                    Name = action.Name,
                };

                actionsList.Add(actionData);
            }


            return actionsList;
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
                //foreach(var prop in type.GetProperties())
                //{
                //    if (typeof(InParameter).IsAssignableFrom(prop.PropertyType))
                //    {
                //        data.InputParameters.Add(new ActionParameterData() { Name = prop.Name });
                //    }

                //    if (typeof(OutParameter).IsAssignableFrom(prop.PropertyType))
                //    {
                //        data.OutputParameters.Add(new ActionParameterData() { Name = prop.Name });
                //    }
                //}

                result.Add(data);
            }
            return result;
        }

        private class ActionTypeData
        {
            public string AssemblyBaseName { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public object TypeName { get; set; }

        }

        private class ActionData
        {
            public string AssemblyBaseName { get; set; }
            public string Name { get; set; }
            public string TypeName { get; set; }
            public List<ActionParameterData> InputParameters { get; set; } = new List<ActionParameterData>();
            public List<ActionParameterData> OutputParameters { get; set; } = new List<ActionParameterData>();

        }

        private class ActionParameterData
        {
            public string Description { get; set; }
            public string Name { get; set; }
            public string TypeName { get; set; }
            public object Value { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuideRecipeBackend.Models
{
    public class ActionData
    {
        public string actionId { get; set; }
        public string AssemblyBaseName { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public List<ActionParameterData> InputParameters { get; set; } = new List<ActionParameterData>();
        public List<ActionParameterData> OutputParameters { get; set; } = new List<ActionParameterData>();
        public List<ActionData> Actions { get; set; } = new List<ActionData>();
    }

    public class ActionParameterData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TypeName { get; set; }
        public object Value { get; set; }
    }
}

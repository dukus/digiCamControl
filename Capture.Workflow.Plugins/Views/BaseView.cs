using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Plugins.Views
{
    public class BaseView: IUiPlugin
    {
        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Steps.Helpers.Interfaces
{
    public interface ITSQLStepHelper : IStepHelper
    {
        bool Hello();

        string SendSQLCommand(string sqlCommand, string licence = "\"\"", string outputFile = "\"\"", bool json = false);

    }
}

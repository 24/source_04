using pb.Data;
using pb.Reflection;
using System.Collections.Generic;

namespace pb.Compiler
{
    partial class RunSource
    {
        public void SetOptions(string options)
        {
            NamedValues<ZValue> options2 = ParseNamedValues.ParseValues(options, useLowercaseKey: true);
            foreach (KeyValuePair<string, ZValue> option in options2)
            {
                SetOption(option.Key, option.Value);
            }
        }

        private void SetOption(string option, ZValue value)
        {
            switch (option)
            {
                case "traceinit":
                    if (!(value is ZBool))
                        throw new PBException($"wrong type for runsource option TraceInit {value.GetType().zGetTypeName()}");
                    TraceInit((bool)(ZBool)value);
                    break;
                case "traceduplicatesource":
                    if (!(value is ZBool))
                        throw new PBException($"wrong type for runsource option TraceDuplicateSource {value.GetType().zGetTypeName()}");
                    ProjectCompiler.TraceDuplicateSource = (bool)(ZBool)value;
                    break;
                default:
                    throw new PBException($"unknow runsource option \"{option}\"");
            }
        }

        private void TraceInit(bool trace)
        {
            _runSourceInitEndMethods.TraceRunOnce = trace;
            _runSourceInitEndMethods.TraceRunAlways = trace;
        }
    }
}

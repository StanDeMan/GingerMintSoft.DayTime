﻿using System.Text;
using CommandLine;
using DayTimeService.CmdLine;

namespace DayTimeService.Services
{
    public class ArgumentService(ParserResult<Options> args)
    {
        private readonly Arguments _arguments = new();

        public Arguments Read()
        {
            try
            {
                _arguments.WorkloadFile = args.Value.Filename;
            }
            catch (Exception)
            {
                _arguments.Errors = args.Errors;
                _arguments.Error = _arguments.Errors.Any();
            }

            return _arguments;
        }
    }

    public class Arguments
    {
        public string DefaultWorkloadFile { get; private set; } = 
            new StringBuilder().Append("DailyWorkload.json").ToString();

        public string? WorkloadFile { get; set; }

        public IEnumerable<Error>? Errors { get; set; } = new List<Error>();
        public bool Error { get; set; }
    }
}

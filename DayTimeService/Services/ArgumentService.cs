namespace DayTimeService.Services
{
    public class ArgumentService(IReadOnlyList<string?> args)
    {
        private readonly Arguments _arguments = new();

        public Arguments Read()
        {
            _arguments.WorkloadFile = args[0];

            return _arguments;
        }
    }

    public class Arguments
    {
        public string? WorkloadFile { get; set; }
    }
}

namespace Lab1_TaskScheduler.Contracts
{
	public class OperationContract
	{
		public string Name { get; set; } = string.Empty;
		public string PreCondition { get; set; } = string.Empty;
		public string PostCondition { get; set; } = string.Empty;
		public string Effects { get; set; } = string.Empty;
		public string ValidExample { get; set; } = string.Empty;
		public string InvalidExample { get; set; } = string.Empty;
	}
}
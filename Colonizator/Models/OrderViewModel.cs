namespace Colonizator.Models
{
	public class OrderViewModel
	{
		public string Token { get; set; }
		public int PlayerId { get; set; }
		public string PlayerName { get; set; }
		public OrderBatch Sell { get; set; }
		public OrderBatch Buy { get; set; }
	}

	public class OrderBatch
	{
		public int Corn { get; set; }
		public int Soil { get; set; }
		public int Minerals { get; set; }
		public int Wool { get; set; }
		public int Wood { get; set; }
	}
}
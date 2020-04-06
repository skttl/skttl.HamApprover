namespace skttl.HamApprover
{
	using RestSharp.Deserializers;

	public class HamResponse
	{
		[DeserializeAs(Name = "email_class")]
		public string EmailClass { get; set; }

		[DeserializeAs(Name = "email_text")]
		public string EmailText { get; set; }
	
		public int Status { get; set; }
	}
}

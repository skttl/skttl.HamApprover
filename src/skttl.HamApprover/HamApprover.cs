using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Umbraco.Core.Composing;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;

namespace skttl.HamApprover
{
	public class HamApprover : WorkflowType
	{
		[Setting("Submission settings", View = "../../../../../umbraco/views/propertyeditors/readonlyvalue/readonlyvalue")]
		public string SubmissionLabel { get; set; }

		[Setting("Comment fields", Description = "Add the alias(es) of the field(s) to test for spam, seperated by comma. If no aliases added, the test will use a concatenation of all fields.", View = "TextField")]
		public string CommentFieldsInput { get; set; }
		public string[] CommentFields => CommentFieldsInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //[Setting("Author name field", Description = "Optional - The alias of the field containg the authors name.", View = "TextField")]
		//public string AuthorField { get; set; }

		//[Setting("Email field", Description = "Optional - The alias of the field containg the authors email.", View = "TextField")]
		//public string EmailField { get; set; }

		//[Setting("Link field", Description = "Optional - The alias of the field containg the authors url.", View = "TextField")]
		//public string LinkField { get; set; }

		//[Setting("Subject field", Description = "Optional - The alias of the field containg the subject of the submission.", View = "TextField")]
		//public string SubjectField { get; set; }

		//[Setting("Approver settings", View = "../../../../../umbraco/views/propertyeditors/readonlyvalue/readonlyvalue")]
		//public string ApproverLabel { get; set; }

		//[Setting("Server", Description = "The server to test the submission against. Default is http://test.blogspam.net:9999/", View = "TextField")]
		//public string ServerInput { get; set; }
		//public string Server
		//{
		//	get
		//	{
		//		return string.IsNullOrEmpty(ServerInput) ? "https://plino.herokuapp.com/api/v1/classify/" : ServerInput;
		//	}
		//}

		//[Setting("IP Blacklist", Description = "Submissions from these IPs (seperated by comma) will always be denied", View = "TextField")]
		//public string BlacklistInput { get; set; }
		//public string[] Blacklist
		//{
		//	get
		//	{
		//		return BlacklistInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
		//	}
		//}

		//[Setting("IP Whitelist", Description = "Submissions from these IPs (seperated by comma) will always be approved", View = "TextField")]
		//public string WhitelistInput { get; set; }
		//public string[] Whitelist
		//{
		//	get
		//	{
		//		return WhitelistInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
		//	}
		//}

		//[Setting("Max links", Description = "The maximum number of links allowed in the submission. Default is 10", View = "TextField")]
		//public string MaxLinksInput { get; set; }
		//public int MaxLinks
		//{
		//	get
		//	{
		//		var result = 0;
		//		int.TryParse(MaxLinksInput, out result);
		//		return string.IsNullOrEmpty(MaxLinksInput) ? 10 : result;
		//	}
		//}

		//[Setting("Max length", Description = "The maximum number of characters allowed in the submission.", View = "TextField")]
		//public string MaxLengthInput { get; set; }
		//public int MaxLength
		//{
		//	get
		//	{
		//		var result = 0;
		//		int.TryParse(MaxLengthInput, out result);
		//		return result;
		//	}
		//}

		//[Setting("Min length", Description = "The minimum number of characters required in the submission.", View = "TextField")]
		//public string MinLengthInput { get; set; }
		//public int MinLength
		//{
		//	get
		//	{
		//		var result = 0;
		//		int.TryParse(MinLengthInput, out result);
		//		return result;
		//	}
		//}

		//[Setting("Min words", Description = "The minimum number of words required in the submission.", View = "TextField")]
		//public string MinWordsInput { get; set; }
		//public int MinWords
		//{
		//	get
		//	{
		//		var result = 0;
		//		int.TryParse(MinWordsInput, out result);
		//		return result;
		//	}
		//}


		public HamApprover()
		{
			this.Name = "Ham Approver";
			this.Id = new Guid("D90EE979-80C5-4AC8-BFEE-28F4DE47C857");
			this.Description = "Approves the submission, after testing if it is spam.";
			this.Icon = "icon-hamapprover";
		}

		public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
		{
			try
			{
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://plino.herokuapp.com/api/v1/classify/");
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";

				var request = new JObject();

				request["email_text"] = string.Join(Environment.NewLine, record.RecordFields.Where(x => CommentFields.Length == 0 || CommentFields.Contains(x.Value.Alias)).Select(x => x.Value.ValuesAsString()));
				//request["ip"] = record.IP;

				//if (!string.IsNullOrEmpty(EmailField))
				//{
				//	var field = record.RecordFields.Where(x => x.Value.Alias == EmailField);
				//	if (field.Any())
				//	{
				//		request["email"] = field.First().Value.ValuesAsString();
				//	}
				//}

				//if (!string.IsNullOrEmpty(LinkField))
				//{
				//	var field = record.RecordFields.Where(x => x.Value.Alias == LinkField);
				//	if (field.Any())
				//	{
				//		request["link"] = field.First().Value.ValuesAsString();
				//	}
				//}

				//if (!string.IsNullOrEmpty(AuthorField))
				//{
				//	var field = record.RecordFields.Where(x => x.Value.Alias == AuthorField);
				//	if (field.Any())
				//	{
				//		request["name"] = field.First().Value.ValuesAsString();
				//	}
				//}

				//if (!string.IsNullOrEmpty(SubjectField))
				//{
				//	var field = record.RecordFields.Where(x => x.Value.Alias == SubjectField);
				//	if (field.Any())
				//	{
				//		request["subject"] = field.First().Value.ValuesAsString();
				//	}
				//}
				
				//request["site"] = "http://test.dk";

				//var options = new List<string>();

				//foreach (var ip in Blacklist)
				//{
				//	options.Add("blacklist=" + ip);
				//}

				//foreach (var ip in Whitelist)
				//{
				//	options.Add("whitelist=" + ip);
				//}

				//options.Add("max-links=" + MaxLinks);
				//options.Add("min-words=" + MinWords);

				//if (MaxLength > 0)
				//{
				//	options.Add("max-size=" + MaxLength);
				//}
				//if (MinLength > 0)
				//{
				//	options.Add("min-size=" + MinLength);
				//}

				//if (options.Any())
				//{
				//	request["options"] = string.Join(",", options);
				//}

				using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					string json = JsonConvert.SerializeObject(request);

					streamWriter.Write(json);
					streamWriter.Flush();
					streamWriter.Close();
				}

				var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					var result = JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());

                    try
					{
						if (result.email_class == "ham")
                        {
                            record.State = FormState.Approved;
                            Current.Logger.Info(typeof(HamApprover), "Submission completed ham approval." + Environment.NewLine + "Request: " + JsonConvert.SerializeObject(request).Replace("{","{{").Replace("}","}}") + Environment.NewLine + "Response: " + JsonConvert.SerializeObject(result).Replace("{", "{{").Replace("}", "}}"));
							
						}
						else
						{
                            Current.Logger.Info(typeof(HamApprover), "Submission failed ham approval." + Environment.NewLine + "Request: " + JsonConvert.SerializeObject(request).Replace("{", "{{").Replace("}", "}}") + Environment.NewLine + "Response: " + JsonConvert.SerializeObject(result).Replace("{", "{{").Replace("}", "}}"));
						}
					}
					catch (Exception ex)
					{
                        Current.Logger.Error(typeof(HamApprover), "Failed testing submission." + Environment.NewLine + "Request: " + JsonConvert.SerializeObject(request).Replace("{", "{{").Replace("}", "}}"), ex);
					}
					

					return WorkflowExecutionStatus.Completed;
				}

			}

			catch (Exception ex)
			{
                Current.Logger.Error(typeof(HamApprover), "Failed testing submission.", ex);
				return WorkflowExecutionStatus.Completed;
			}

		}

        public override List<Exception> ValidateSettings()
		{
			return new List<Exception>();
		}
	}
}

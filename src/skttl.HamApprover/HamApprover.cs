using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Logging;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Data.Storage;
using Umbraco.Forms.Web.Services;
using Umbraco.Web;

namespace skttl.HamApprover
{
	public class HamApprover : WorkflowType
	{
		[Setting("Submission settings", view = "../../../../../umbraco/views/propertyeditors/readonlyvalue/readonlyvalue")]
		public string SubmissionLabel { get; set; }

		[Setting("Comment fields", description = "Add the alias(es) of the field(s) to test for spam, seperated by comma. If no aliases added, the test will use a concatenation of all fields.", view = "TextField")]
		public string CommentFieldsInput { get; set; }
		public string[] CommentFields
		{
			get
			{
				return CommentFieldsInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		[Setting("Author name field", description = "Optional - The alias of the field containg the authors name.", view = "TextField")]
		public string AuthorField { get; set; }

		[Setting("Email field", description = "Optional - The alias of the field containg the authors email.", view = "TextField")]
		public string EmailField { get; set; }

		[Setting("Link field", description = "Optional - The alias of the field containg the authors url.", view = "TextField")]
		public string LinkField { get; set; }

		[Setting("Subject field", description = "Optional - The alias of the field containg the subject of the submission.", view = "TextField")]
		public string SubjectField { get; set; }

		[Setting("Approver settings", view = "../../../../../umbraco/views/propertyeditors/readonlyvalue/readonlyvalue")]
		public string ApproverLabel { get; set; }

		[Setting("Server", description = "The server to test the submission against. Default is http://test.blogspam.net:9999/", view = "TextField")]
		public string ServerInput { get; set; }
		public string Server
		{
			get
			{
				return string.IsNullOrEmpty(ServerInput) ? "http://test.blogspam.net:9999" : ServerInput;
			}
		}

		[Setting("IP Blacklist", description = "Submissions from these IPs (seperated by comma) will always be denied", view = "TextField")]
		public string BlacklistInput { get; set; }
		public string[] Blacklist
		{
			get
			{
				return BlacklistInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		[Setting("IP Whitelist", description = "Submissions from these IPs (seperated by comma) will always be approved", view = "TextField")]
		public string WhitelistInput { get; set; }
		public string[] Whitelist
		{
			get
			{
				return WhitelistInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		[Setting("Max links", description = "The maximum number of links allowed in the submission. Default is 10", view = "TextField")]
		public string MaxLinksInput { get; set; }
		public int MaxLinks
		{
			get
			{
				var result = 0;
				int.TryParse(MaxLinksInput, out result);
				return string.IsNullOrEmpty(MaxLinksInput) ? 10 : result;
			}
		}

		[Setting("Max length", description = "The maximum number of characters allowed in the submission.", view = "TextField")]
		public string MaxLengthInput { get; set; }
		public int MaxLength
		{
			get
			{
				var result = 0;
				int.TryParse(MaxLengthInput, out result);
				return result;
			}
		}

		[Setting("Min length", description = "The minimum number of characters required in the submission.", view = "TextField")]
		public string MinLengthInput { get; set; }
		public int MinLength
		{
			get
			{
				var result = 0;
				int.TryParse(MinLengthInput, out result);
				return result;
			}
		}

		[Setting("Min words", description = "The minimum number of words required in the submission.", view = "TextField")]
		public string MinWordsInput { get; set; }
		public int MinWords
		{
			get
			{
				var result = 0;
				int.TryParse(MinWordsInput, out result);
				return result;
			}
		}


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

				var httpWebRequest = (HttpWebRequest)WebRequest.Create(Server);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";

				var request = new JObject();

				request["comment"] = string.Join(Environment.NewLine, record.RecordFields.Where(x => CommentFields.Length == 0 || CommentFields.Contains(x.Value.Alias)).Select(x => x.Value.ValuesAsString()));
				request["ip"] = record.IP;

				if (!string.IsNullOrEmpty(EmailField))
				{
					var field = record.RecordFields.Where(x => x.Value.Alias == EmailField);
					if (field.Any())
					{
						request["email"] = field.First().Value.ValuesAsString();
					}
				}

				if (!string.IsNullOrEmpty(LinkField))
				{
					var field = record.RecordFields.Where(x => x.Value.Alias == LinkField);
					if (field.Any())
					{
						request["link"] = field.First().Value.ValuesAsString();
					}
				}

				if (!string.IsNullOrEmpty(AuthorField))
				{
					var field = record.RecordFields.Where(x => x.Value.Alias == AuthorField);
					if (field.Any())
					{
						request["name"] = field.First().Value.ValuesAsString();
					}
				}

				if (!string.IsNullOrEmpty(SubjectField))
				{
					var field = record.RecordFields.Where(x => x.Value.Alias == SubjectField);
					if (field.Any())
					{
						request["subject"] = field.First().Value.ValuesAsString();
					}
				}
				
				request["site"] = "http://test.dk";

				var options = new List<string>();

				foreach (var ip in Blacklist)
				{
					options.Add("blacklist=" + ip);
				}

				foreach (var ip in Whitelist)
				{
					options.Add("whitelist=" + ip);
				}

				options.Add("max-links=" + MaxLinks);
				options.Add("min-words=" + MinWords);

				if (MaxLength > 0)
				{
					options.Add("max-size=" + MaxLength);
				}
				if (MinLength > 0)
				{
					options.Add("min-size=" + MinLength);
				}

				if (options.Any())
				{
					request["options"] = string.Join(",", options);
				}

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
						if (result.result == "OK")
						{
							RecordService.Instance.Approve(record, e.Form);
							
							LogHelper.Info(typeof(HamApprover), "Submission completed ham approval." + Environment.NewLine + "Request: " + JsonConvert.SerializeObject(request).Replace("{","{{").Replace("}","}}") + Environment.NewLine + "Response: " + JsonConvert.SerializeObject(result).Replace("{", "{{").Replace("}", "}}"));
							
						}
						else
						{
							LogHelper.Info(typeof(HamApprover), "Submission failed ham approval." + Environment.NewLine + "Request: " + JsonConvert.SerializeObject(request).Replace("{", "{{").Replace("}", "}}") + Environment.NewLine + "Response: " + JsonConvert.SerializeObject(result).Replace("{", "{{").Replace("}", "}}"));
						}
					}
					catch (Exception ex)
					{
						LogHelper.Error(typeof(HamApprover), "Failed testing submission." + Environment.NewLine + "Request: " + JsonConvert.SerializeObject(request).Replace("{", "{{").Replace("}", "}}"), ex);
					}
					

					return WorkflowExecutionStatus.Completed;
				}

			}

			catch (Exception ex)
			{
				LogHelper.Error(typeof(HamApprover), "Failed testing submission.", ex);
				return WorkflowExecutionStatus.Completed;
			}

		}

		public override List<Exception> ValidateSettings()
		{
			return new List<Exception>();
		}
	}
}

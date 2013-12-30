using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetAutoComplete
{
	[Serializable]
	public sealed class AutoFillEventArgs : EventArgs
	{
		public AutoFillEventArgs(String parameter)
		{
			this.Parameter = parameter;
			this.Results = new StringDictionary();
		}

		public String Parameter { get; private set; }

		public StringDictionary Results { get; private set; }
	}

	public class AutoFillDropDownList : DropDownList, ICallbackEventHandler
	{
		public event EventHandler<AutoFillEventArgs> AutoFill;

		protected override void OnInit(EventArgs e)
		{
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID + "onSuggestionCallback", String.Format("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {{ document.getElementById('{0}').onSuggestionCallback = function(result, context) {{ document.getElementById('{0}').options.length = 0; var r = result.split('\\n'); for (var i = 0; i < r.length; ++i) {{ var keyValue = r[i].split('='); if (keyValue.length == 1) {{ continue }}; var option = document.createElement('option'); option.value = keyValue[0]; option.text = keyValue[1]; document.getElementById('{0}').options.add(option);  }} }} }});", this.ClientID), true);
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID + "getSuggestions", String.Format("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {{ document.getElementById('{0}').getSuggestions = function(value) {{ document.getElementById('{0}_HiddenField').value = value; {1}; }} }});\n", this.ClientID, String.Format(this.Page.ClientScript.GetCallbackEventReference(this, "value", "document.getElementById('{0}').onSuggestionCallback", null, true), this.ClientID)), true);
			this.Page.ClientScript.RegisterHiddenField(String.Concat(this.ID, "_HiddenField"), this.Context.Request.Form[String.Concat(this.ID, "_HiddenField")]);

			this.Page.PreLoad += this.OnPreLoad;

			base.OnInit(e);
		}

		public override void Dispose()
		{
			this.Page.PreLoad -= this.OnPreLoad;

			base.Dispose();
		}

		protected void OnPreLoad(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == true)
			{
				var fillValue = this.Context.Request.Form[String.Concat(this.UniqueID, "_HiddenField")];

				if (String.IsNullOrWhiteSpace(fillValue) == false)
				{
					var args = new AutoFillEventArgs(fillValue);
					
					this.OnAutoFill(args);

					foreach (var key in args.Results.Keys.OfType<String>())
					{
						this.Items.Add(new ListItem(args.Results[key], key));
					}

					var selectedValue = this.Context.Request.Form[this.UniqueID];

					this.SelectedIndex = this.Items.IndexOf(this.Items.FindByValue(selectedValue));
				}
			}
		}
		
		protected virtual void OnAutoFill(AutoFillEventArgs e)
		{
			var handler = this.AutoFill;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		#region ICallbackEventHandler Members

		String ICallbackEventHandler.GetCallbackResult()
		{
			var output = this.Context.Items["Results"] as StringDictionary;

			return (String.Join(Environment.NewLine, output.Keys.OfType<String>().Select(x => String.Concat(x, "=", output[x]))));
		}

		void ICallbackEventHandler.RaiseCallbackEvent(String eventArgument)
		{
			var args = new AutoFillEventArgs(eventArgument);

			this.OnAutoFill(args);

			this.Context.Items["Results"] = args.Results;
		}

		#endregion
	}
}

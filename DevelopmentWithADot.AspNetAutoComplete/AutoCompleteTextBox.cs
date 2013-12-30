using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetAutoComplete
{
	[Serializable]
	public sealed class AutoCompleteEventArgs : EventArgs
	{
		public AutoCompleteEventArgs(String parameter)
		{
			this.Parameter = parameter;
			this.Results = Enumerable.Empty<String>();
		}

		public String Parameter { get; private set; }

		public IEnumerable<String> Results { get; set; }
	}

	public class AutoCompleteTextBox : TextBox, ICallbackEventHandler
	{
		private readonly Panel panel = new Panel();

		public AutoCompleteTextBox()
		{
			this.MinCharacters = 3;
		}

		[DefaultValue(3)]
		public Int32 MinCharacters { get; set; }

		public event EventHandler<AutoCompleteEventArgs> AutoComplete;

		[DefaultValue("")]
		public String OnClientItemSelected
		{
			get;
			set;
		}

		public CssStyleCollection PanelStyle
		{
			get
			{
				return (this.panel.Style);
			}
		}

		[DefaultValue("")]
		[CssClassProperty]
		public String PanelItemCssClass
		{
			get;
			set;
		}

		[DefaultValue("")]
		[CssClassProperty]
		public String PanelCssClass
		{
			get
			{
				return (this.panel.CssClass);
			}
			set
			{
				this.panel.CssClass = value;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			this.Page.LoadComplete += OnLoadComplete;

			base.OnInit(e);
		}

		public override void Dispose()
		{
			this.Page.LoadComplete -= OnLoadComplete;

			base.Dispose();
		}

		protected void OnLoadComplete(object sender, EventArgs e)
		{
			var index = this.Parent.Controls.IndexOf(this);

			this.Parent.Controls.AddAt(index + 1, this.panel);
		}

		protected override void CreateChildControls()
		{
			this.panel.ID = String.Concat(this.ID, "_Panel");
			this.panel.Style[HtmlTextWriterStyle.Display] = "none";
			this.panel.Style[HtmlTextWriterStyle.Position] = "absolute";

			var script = String.Empty;

			if (String.IsNullOrWhiteSpace(this.OnClientItemSelected) == false)
			{
				var index = this.OnClientItemSelected.IndexOf('(');

				if (index >= 0)
				{
					script = this.OnClientItemSelected.Replace("'", "\\'");
					script = script.Replace("\"", "\\\"");
				}
				else
				{
					script = String.Concat(this.OnClientItemSelected, "(document.getElementById(\\'", this.ClientID, "\\'), this.innerHTML)");
				}
			}

			this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID + "onSuggestionCallback", String.Format("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {{ document.getElementById('{0}').onSuggestionCallback = function(result, context) {{ var html = ''; var r = result.split('\\n'); for (var i = 0; i < r.length; ++i) {{ html += '<a href=\"#\" style=\"display:block\" class=\"{2}\" onclick=\"document.getElementById(\\'{0}\\').value = this.innerHTML; document.getElementById(\\'{1}\\').style.display = \\'none\\'; {3} \">' + r[i] + '</a>'; }}; document.getElementById('{1}').innerHTML = html; document.getElementById('{1}').style.display = ''; }} }});", this.ClientID, this.panel.ClientID, this.PanelItemCssClass ?? String.Empty, script), true);
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID + "getSuggestions", String.Format("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {{ document.getElementById('{0}').getSuggestions = function() {{ {1} }} }});\n", this.ClientID, String.Format(this.Page.ClientScript.GetCallbackEventReference(this, "document.getElementById('{0}').value", "document.getElementById('{0}').onSuggestionCallback", null, true), this.ClientID)), true);
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.UniqueID + "addEventListener", String.Format("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {{ document.getElementById('{0}').addEventListener('keyup', function(e) {{ if (e.target.value.length >= {1}) {{ document.getElementById('{0}').getSuggestions(); }} }}) }});\n", this.ClientID, this.MinCharacters), true);

			base.CreateChildControls();
		}

		protected virtual void OnAutoComplete(AutoCompleteEventArgs e)
		{
			var handler = this.AutoComplete;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		#region ICallbackEventHandler Members

		String ICallbackEventHandler.GetCallbackResult()
		{
			var output = this.Context.Items["Results"] as IEnumerable<String>;

			return (String.Join(Environment.NewLine, output));
		}

		void ICallbackEventHandler.RaiseCallbackEvent(String eventArgument)
		{
			var args = new AutoCompleteEventArgs(eventArgument);

			this.OnAutoComplete(args);

			this.Context.Items["Results"] = args.Results;
		}

		#endregion
	}
}
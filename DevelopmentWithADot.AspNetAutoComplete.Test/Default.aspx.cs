using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetAutoComplete.Test
{
	public partial class _Default : Page
	{
		protected void text_AutoComplete(object sender, AutoCompleteEventArgs e)
		{			
			e.Results = Enumerable.Range(0, 10).Select(x => e.Parameter + x.ToString());
		}

		protected void list_AutoFill(object sender, AutoFillEventArgs e)
		{
			for (var i = 0; i < 10; ++i)
			{
				e.Results.Add(i.ToString(), e.Parameter + i.ToString());
			}
		}

		protected void button_OnClick(object sender, EventArgs e)
		{

		}
	}
}
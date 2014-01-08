<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="DevelopmentWithADot.AspNetAutoComplete.Test._Default" %>
<%@ Register TagPrefix="test" Namespace="DevelopmentWithADot.AspNetAutoComplete" Assembly="DevelopmentWithADot.AspNetAutoComplete" %><!DOCTYPE html>
<html>
	<head runat="server">
		<title></title>
		<style type="text/css">

			.panel
			{
				background-color: cyan;
				width: 100px;
				border: solid 1px;
			}

			.panel-item
			{

			}

			.panel-item:hover
			{
				background-color: yellow;
				display: block;
			}
			
		</style>
		<script type="text/javascript">

			function fillList(sender, value)
			{
				document.getElementById('list').fill(value);
			}

		</script>
	</head>
	<body>
		<form runat="server">
			<asp:ScriptManager runat="server" />
			<asp:UpdatePanel runat="server">
				<ContentTemplate>
					<test:AutoCompleteTextBox runat="server" ID="text" PanelItemCssClass="panel-item" PanelCssClass="panel" MinCharacters="3" OnAutoComplete="text_AutoComplete" OnClientItemSelected="fillList(document.getElementById('text'), document.getElementById('text').value)" />
					<test:AutoFillDropDownList runat="server" ID="list" ClientIDMode="Static" OnAutoFill="list_AutoFill" />
					<asp:Button runat="server" ID="button" OnClick="button_OnClick" Text="Click Me" />
				</ContentTemplate>
			</asp:UpdatePanel>
		</form>
	</body>
</html>


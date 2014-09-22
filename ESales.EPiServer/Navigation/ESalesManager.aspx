<%@ Page Language="c#" Codebehind="ESalesManager.aspx.cs" AutoEventWireup="true" Inherits="Apptus.ESales.EPiServer.Navigation.ESalesManager" %>
<%@ Import Namespace="Apptus.ESales.EPiServer.Util" %>
<%@ Register TagPrefix="sc" Assembly="EPiServer.Shell" Namespace="EPiServer.Shell.Web.UI.WebControls" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="FullRegion">
    <div style="position: relative;" class="epi-globalNavigationContainer">
        <sc:ShellMenu ID="ESalesManagerShellMenu" runat="server" />
    </div>
    <div>
        <iframe src="<%= ESales.ManagerURL %>" class="episystemiframe" frameborder="0"></iframe>
    </div> 
</asp:Content>
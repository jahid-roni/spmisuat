<%@ Page Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="SPMS_Web.UI.PrmData.Test" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="Server">
        
        <asp:FileUpload runat="server" ID="fuTest" />
        <asp:Button runat="server" ID="btnSave" onclick="btnSave_Click" />
        
</asp:Content>
     

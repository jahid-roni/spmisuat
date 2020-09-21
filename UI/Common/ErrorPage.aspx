<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="ErrorPage.aspx.cs" Inherits="SBM_WebUI.mp.ErrorPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <table width="70%" align="center">
        <tr>
            <td>
                <asp:Image runat="server" ID="imgID" ImageUrl="~/Images/error.jpg" />
            </td>
            <td>
                <div style="text-transform: none">
                    <b>You are not authorized to view this page.</b>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnHomePage" runat="server" CssClass="ButtonAsh" Text="Go to home page"
                    OnClick="btnHomePage_Click" />
            </td>
        </tr>
    </table>
    <br />
    <br />
    <br />
    <br />
</asp:Content>

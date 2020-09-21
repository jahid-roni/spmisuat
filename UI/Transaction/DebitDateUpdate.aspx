<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="DebitDateUpdate.aspx.cs" Inherits="SBM_WebUI.mp.DebitDateUpdate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">


    <br />
    <fieldset>
        <table width="95%" align="center"  cellpadding="3" cellspacing="3" border="0">
            <tr>
                <td align="right" width="20%">Reg No.</td>
                <td width="30%"><asp:TextBox runat="server" ID="TextBox2" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox></td>
                <td align="right" width="20%">Debit Date</td>
                <td width="30%"><asp:TextBox runat="server" ID="TextBox1" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)" ></asp:TextBox></td>
            </tr>
        </table>
    </fieldset>


    <br />
    <fieldset>
        <table width="95%" align="center"  cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Update" ID="btnUpdate"  /> &nbsp;
                </td>
            </tr>
        </table>
    </fieldset>
    
    
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="MarkStockOut.aspx.cs" Inherits="SBM_WebUI.mp.MarkStockOut" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">

    <fieldset>
        <legend>SP Receive Details</legend>

    <table width="95%" align="center"  cellpadding="3" cellspacing="3" border="0">
    </table>    
    <table width="95%" align="center"  cellpadding="3" cellspacing="3" border="0">
        <tr>
            <td align="right">SP Type</td>
            <td colspan="4"><asp:DropDownList runat="server" ID="ddlSpType"><asp:ListItem Text="3MS : 3 Months Interest Based Sanchaya Patra" Value="3MS : 3 Months Interest Based Sanchaya Patra"></asp:ListItem></asp:DropDownList></td>
        </tr>
        <tr>
            <td align="right">Denomination</td>
            <td style="width: 119px"><asp:DropDownList runat="server" ID="ddlDenomination"><asp:ListItem Text="100000" Value="100000"></asp:ListItem></asp:DropDownList></td>
            <td width="30%" align="right">Quantity</td>
            <td width="20%"><asp:TextBox Width="150px" runat="server" CssClass="textInput" ID="txtQuantity"></asp:TextBox></td>
            <td width="20%"><asp:Button CssClass="ButtonAsh" runat="server" 
                    Text="Get Certificates" ID="Button1" onclick="Button1_Click"  /></td>
        </tr>
    </table>
    
    <br />Receive Details<br/>
    <div style="height: 200px; width: 100%; overflow: auto;">
        <asp:GridView Style="width: 98%" ID="gvSPType" runat="server" AutoGenerateColumns="False"
            SkinID="SBMLGridGreen" ShowHeader="true">
            <Columns>
                <asp:BoundField DataField="Val1" HeaderText="Denomination" />
                <asp:BoundField DataField="Val2" HeaderText="Series" />
                <asp:BoundField DataField="Val3" HeaderText="Srno" />
            </Columns>
        </asp:GridView>
    </div>
    </fieldset>
    <br />
    
    <br />
    <fieldset>
        <table width="95%" align="center"  cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave"  /> &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Close" ID="btnClose" /> &nbsp;
                </td>
            </tr>
        </table>
    </fieldset>
    
</asp:Content>

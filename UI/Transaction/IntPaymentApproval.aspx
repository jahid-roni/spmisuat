<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="IntPaymentApproval.aspx.cs" Inherits="SPMS_Web.mp.IntPaymentApproval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
 
 <br />Initiated Interest Payment Batch List
    <div style="height: 200px; width: 100%; overflow: auto;">
        <asp:GridView Style="width: 98%" ID="gvSPType" runat="server" AutoGenerateColumns="False"
            SkinID="SCBLGridGreen" ShowHeader="true">
            <Columns>
                <asp:BoundField DataField="Val1" HeaderText="Batch No" />
                <asp:BoundField DataField="Val2" HeaderText="Batch Date" />
                <asp:BoundField DataField="Val3" HeaderText="Maker" />
                <asp:BoundField DataField="Val4" HeaderText="SP Type" />
                <asp:BoundField DataField="Val5" HeaderText="Interest" />
                <asp:BoundField DataField="Val6" HeaderText="Approval Status" />
            </Columns>
        </asp:GridView>
    </div>
    <br />
    
    
    <br />
    <fieldset>
        <table width="95%" align="center"  cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Check" ID="btnCheck"  /> &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Close" ID="btnClose" /> &nbsp;
                </td>
            </tr>
        </table>
    </fieldset>
</asp:Content>

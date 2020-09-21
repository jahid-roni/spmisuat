<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="PrintCertificate.aspx.cs" Inherits="SBM_WebUI.mp.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
<fieldset>
    <table width="95%" align="center" cellpadding="3" cellspacing="3" border="0">
        <tr>
            <td align="right">SP Type</td>
            <td><asp:DropDownList runat="server" ID="ddlSpType"><asp:ListItem Text="3 MS 3 Mss.."></asp:ListItem></asp:DropDownList></td>
            <td align="right">Issue Date</td>
            <td><asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtIssueDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></td>
            <td><asp:Button CssClass="ButtonAsh" runat="server" Text="Show Data" ID="btnShowData"></asp:Button></td>
        </tr>
        <tr>
            <td colspan="5">Registration Details<br /><br />
            <div style="height: 200px; width: 100%; overflow: auto;">
                <asp:GridView Style="width: 94%" ID="gvCustomerType" runat="server" AutoGenerateColumns="False"
                    SkinID="SBMLGridGreen" ShowHeader="true">
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle Width="15px" />
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chkCusomerType" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Val1" HeaderText="Reg No" />
                        <asp:BoundField DataField="Val1" HeaderText="SP Type" />
                        <asp:BoundField DataField="Val1" HeaderText="Currency" />
                        <asp:BoundField DataField="Val1" HeaderText="Issue Date" />
                        <asp:BoundField DataField="Val1" HeaderText="Customer Name" />
                        <asp:BoundField DataField="Val1" HeaderText="Passport No" />
                        <asp:BoundField DataField="Val1" HeaderText="Nationality" />
                        <asp:BoundField DataField="Val1" HeaderText="Branch" />
                        <asp:BoundField DataField="Val1" HeaderText="Maker" />
                        </Columns>
                </asp:GridView>
            </div>
            </td>
        </tr>
    </table>
 </fieldset>
    <br />
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="left" width="50%">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Select All" ID="btnSelect" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Deselect All" ID="btnDeSelect" />
                    &nbsp;
                </td>
                <td align="right" width="50%">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Preview" ID="btnPrintPreview" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Print" ID="btnPrint" />
                    &nbsp;
                </td>
            </tr>
        </table>
    </fieldset>   
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="BBDocumentInquiry.aspx.cs" Inherits="SBM_WebUI.mp.BBDocumentInquiry" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <fieldset>
        <legend>Search Criteria</legend>
        <table align="left" class="tableBody" border="0" cellpadding="3">
            <tr>
                <td align="right">
                    Letter Ref No
                </td>
                <td align="left">
                    <asp:TextBox ID="txtLetterRefNo" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="300px" MaxLength="100" TabIndex="6"></asp:TextBox>
                </td>
                <td align="right">
                    Letter Type ID
                </td>
                <td align="left">
                    <asp:DropDownList runat="server" ID="ddlLetterType" SkinID="ddlLarge" Width="400px">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="Sales Statement Reference">Sales Statement Reference</asp:ListItem>
                        <asp:ListItem Value="Commission Claim Reference">Commission Claim Reference</asp:ListItem>
                        <asp:ListItem Value="Interest Claim Reference">Interest Claim Reference</asp:ListItem>
                        <asp:ListItem Value="Encashment Claim Reference">Encashment Claim Reference</asp:ListItem>
                        <asp:ListItem Value="Reinvestment Statement Reference">Reinvestment Statement Reference</asp:ListItem>
                        <asp:ListItem Value="Sales Statement Reconciliation Advice">Sales Statement Reconciliation Advice</asp:ListItem>
                        <asp:ListItem Value="Interest Claim Reconciliation Advice">Interest Claim Reconciliation Advice</asp:ListItem>
                        <asp:ListItem Value="Encashment Claim Reconciliation Advice">Encashment Claim Reconciliation Advice</asp:ListItem>
                        <asp:ListItem Value="Commission Claim Reconciliation Advice">Commission Claim Reconciliation Advice</asp:ListItem>
                        <asp:ListItem Value="General Letter">General Letter</asp:ListItem>
                        <asp:ListItem Value="BB Policy Circular">BB Policy Circular</asp:ListItem>
                        <asp:ListItem Value="Explanation Letter">Explanation Letter</asp:ListItem>
                        <asp:ListItem Value="Duplicate Issue Claim Settlement">Duplicate Issue Claim Settlement</asp:ListItem>
                        <asp:ListItem Value="Stop Payment Letter">Stop Payment Letter</asp:ListItem>
                        <asp:ListItem Value="Lien Letter">Lien Letter</asp:ListItem>
                        <asp:ListItem Value="Other Letter">Other Letter</asp:ListItem>
                    </asp:DropDownList>                
                </td>
                <td>
                    
                </td>
            </tr>
            <tr>
                <td>From Date</td>
                <td><asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtFromDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></td>
                <td>To Date</td>
                <td><asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtToDate" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox></td>
                <td><asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="ButtonAsh" OnClick="btnSearch_Click" /></td>
            </tr>
        </table>
    </fieldset>
<asp:UpdatePanel runat="server" ID="UpdatePanel1">
            </asp:UpdatePanel>
    <br />
    <fieldset>
        <legend runat="server" id="lgText">Issue Tracker Queue List</legend>
        <table width="100%" align="center" class="tableBody" border="0">
            <tr>
                <td align="center">
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging" PageSize="15">
                                <Columns>
                                    <asp:TemplateField HeaderText="View" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="View" runat="server" ID="btnView" Text="View" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                </td>
            </tr>
        </table>
    </fieldset>
    <uc1:Error ID="ucMessage" runat="server" />
        </table>
</asp:Content>

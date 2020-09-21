<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="CustomerPortfolio.aspx.cs" Inherits="SBM_WebUI.mp.CustomerPortfolio" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="UpdatePanel8" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Mandatory Field List</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset>
                <asp:UpdatePanel runat="server" ID="UpdatePanel8">
                    </asp:UpdatePanel>
                <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="left" width="10%" valign="top">Search Type</td>
                        <td align="left" width="20%" valign="top">
                            <asp:DropDownList ID="ddlTransType" Width="300px" runat="server" AutoPostBack="True" SkinID="ddlLarge"
                                OnSelectedIndexChanged="ddlTransType_SelectedIndexChanged">
                                <asp:ListItem Value="S">Select</asp:ListItem>
                                <asp:ListItem Value="R">Registration</asp:ListItem>
                                <asp:ListItem Value="T">ScriptNo</asp:ListItem>
                                <asp:ListItem Value="M">Master No</asp:ListItem>
                                <asp:ListItem Value="A">Account No</asp:ListItem>
                                <asp:ListItem Value="N">Name</asp:ListItem>
                                <asp:ListItem Value="D">NID</asp:ListItem>
                                <asp:ListItem Value="P">Passport</asp:ListItem>
                                <asp:ListItem Value="OI">Internal Reference Search</asp:ListItem>
                                <asp:ListItem Value="OC">Customer internal ID Search</asp:ListItem>
                                <asp:ListItem Value="ON">Nominee Internal ID Search</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right" width="15%" valign="top">Search Value</td>
                        <td width="40%" valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtSearchValue" Width="250px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="100">
                                </asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                            <asp:Button ID="btnSearch" runat="server" CssClass="ButtonAsh"
                                Text="Search" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" width="10%" valign="top">Issue Status</td>
                        <td colspan="3" align="left" width="75%" valign="top">
                            <asp:DropDownList ID="ddlIssueStatus" Width="300px" runat="server" AutoPostBack="True" SkinID="ddlLarge"
                                OnSelectedIndexChanged="ddlIssueStatus_SelectedIndexChanged">
                                <asp:ListItem Value="L">Live</asp:ListItem>
                                <asp:ListItem Value="E">Encashed</asp:ListItem>
                                <asp:ListItem Value="R">Reinvested</asp:ListItem>
                                <asp:ListItem Value="A">All</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
    </fieldset>
    <br />
    <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="3"
        border="0">
        <tr>
            <td>Registration Details
            </td>
            <td align="right">&nbsp;
            </td>
        </tr>
        <tr>
            <td width="100%" colspan="2">
                <div style="height: 200px; width: 100%; overflow: scroll;">
                    <asp:GridView ID="gvTransactionList" runat="server" Style="table-layout: fixed;" Width="98%"
                        OnRowDataBound="gvTransactionList_RowDataBound" HorizontalAlign="Center" EnableModelValidation="True" OnRowCommand="gvTransactionList_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="View" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="View" runat="server" ID="btnView" Text="View"  />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No record found
                        </EmptyDataTemplate>
                        <HeaderStyle BackColor="Red" ForeColor="White" Height="40px" Wrap="True" />
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <br />
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Customer Portfolio" ID="btnPortfolio" OnClick="btnPortfolio_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Present Value" ID="btnPresentValue"
                        OnClick="btnPresentValue_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <UpdatePanel8:Error ID="ucMessage" runat="server" />
</asp:Content>

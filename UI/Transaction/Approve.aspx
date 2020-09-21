<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="Approve.aspx.cs" Inherits="SBM_WebUI.mp.Approve" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <fieldset>
        <legend>Search Criteria</legend>
        <table width="50%" align="left" class="tableBody" border="0" cellpadding="3">
            <tr>
                <td align="right">
                    Maker ID
                </td>
                <td align="left">
                    <asp:DropDownList runat="server" ID="ddlUserName" SkinID="ddlSmall" OnSelectedIndexChanged="ddlUserName_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="ButtonAsh" OnClick="btnSearch_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend runat="server" id="lgText">Approval Queue List</legend>
        <table width="100%" align="center" class="tableBody" border="0">
            <tr>
                <td align="center">
                    <asp:UpdatePanel runat="server" ID="upGv">
                        <ContentTemplate>
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
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <uc1:Error ID="ucMessage" runat="server" />
</asp:Content>

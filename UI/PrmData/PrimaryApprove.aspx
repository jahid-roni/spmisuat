<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="PrimaryApprove.aspx.cs" Inherits="PrimaryApprove" %>

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
                    <asp:DropDownList runat="server" ID="ddlUserName" SkinID="ddlSmall"></asp:DropDownList>
                </td>
                <td>
                    <asp:Button runat="server" id="btnSearch" Text="Search" CssClass="ButtonAsh" OnClick="btnSearch_Click" />
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
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound"  AllowPaging="True" PageSize="2" OnPageIndexChanging="gvData_PageIndexChanging" >
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
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSearch" />
                        </Triggers>    
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    
    
</asp:Content>

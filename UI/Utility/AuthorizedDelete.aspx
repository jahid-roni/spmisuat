<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="AuthorizedDelete.aspx.cs" Inherits="SBM_WebUI.mp.AuthorizedDelete" %>

<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Hidden Field --%>
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
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                    <td align="left" width="10%" valign="top">Transaction Type</td>
                    <td align="left" width="15%" valign="top">
                        <asp:DropDownList ID="ddlTransType" SkinID="ddlLarge" Width="300px"
                                runat="server" AutoPostBack="True" 
                            onselectedindexchanged="ddlTransType_SelectedIndexChanged">
                            <asp:ListItem Value="S">Select</asp:ListItem>
                            <asp:ListItem Value="I">Issue</asp:ListItem>
                            <asp:ListItem Value="C">Interest</asp:ListItem>
                            <asp:ListItem Value="E">Encashment</asp:ListItem>
                            <asp:ListItem Value="Sale Document">Sale Document</asp:ListItem>
                            <asp:ListItem Value="Sales Statement Reconciliation Advice">Issue Document</asp:ListItem>
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
                        <td align="right" width="15%" valign="top">
                            Registration/Reference No</td>
                        <td width="20%" valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegNo" Width="160px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="25">
                                </asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                            <asp:Button ID="btnSearch" runat="server" CssClass="ButtonAsh" 
                                 Text="Search" onclick="btnSearch_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="3"
                border="0">
                <tr>
                    <td>
                        Transaction Details
                    </td>
                    <td align="right">
                        &nbsp;
                        </td>
                </tr>
                <tr>
                    <td width="100%" colspan="2">
                        <div style="height: 200px; width: 100%; overflow: auto;">
                            <asp:GridView ID="gvTransactionList" runat="server" AutoGenerateColumns="true" Width="98%"
                                SkinID="SBMLGridGreen">
                                <Columns>
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
    
    <fieldset>
        <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClick="btnDelete_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete All" ID="btnDeleteAll"
                        OnClick="btnDeleteAll_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:Error ID="ucMessage" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

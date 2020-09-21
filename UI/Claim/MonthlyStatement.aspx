<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true" CodeBehind="MonthlyStatement.aspx.cs" Inherits="SBM_WebUI.mp.MonthlyStatement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">

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
        <legend>Monthly Statement to Bangladesh Bank</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
        <table width="100%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
            border="0">
            <tr>
                <td align="right">
                    SP type
                </td>
                <td width="30%">
                    <div class="fieldLeft">
                        <asp:DropDownList Width="200px" SkinID="ddlLarge" ID="ddlSpType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged" >
                        </asp:DropDownList>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right">
                    Currency
                </td>
                <td >
                    <div class="fieldLeft">
                        <asp:DropDownList ID="ddlCurrency" SkinID="ddlMedium" runat="server" >
                        </asp:DropDownList>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    Date
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox ID="txtDate" Width="150px" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
                <td align="right">
                    Reference No
                </td>
                <td>
                    <div class="fieldLeft">
                        <asp:TextBox ID="txtConfirmPassword" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                            onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                    </div>
                    <div class="errorIcon">
                        *</div>
                </td>
            </tr>
        </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
        </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    
    <br />
    <fieldset>
        <table width="100%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
            border="0">
            <tr>
                <td align="center">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Preview" ID="btnPreview" OnClick="btnPreview_Click" />
                    &nbsp;
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save & Preview" ID="btnSaveAndPreview" OnClick="btnSaveAndPreview_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="AddJournalManual.aspx.cs" Inherits="SBM_WebUI.mp.AddJournalManual" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        };
        function AddValidation() {
            var sErrorList = "";

            //ddlConversionCurrency
            sErrorList += RequiredData('<%=ddlSPType.ClientID %>', 'DropDownList', "Please select SP Type");
            sErrorList += RequiredData('<%=txtTransDate.ClientID %>', 'TextBox', "Transaction date cant't empty");
            sErrorList += RequiredData('<%=ddlAccountNo.ClientID %>', 'DropDownList', "Please select Account No.");
            sErrorList += RequiredData('<%=txtNarration.ClientID %>', 'TextBox', "Narration can't empty");
            sErrorList += RequiredData('<%=ddlDrCr.ClientID %>', 'DropDownList', "Please select Dr/Cr");
            sErrorList += RequiredData('<%=ddlCurrency.ClientID %>', 'DropDownList', "Please select Currency");
            sErrorList += RequiredData('<%=txtAmount.ClientID %>', 'TextBox', "Amount can't empty");
            // show error divErrorList
            var divErrorPanel = document.getElementById('divErrorPanel');
            if (sErrorList.length > 0) {
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
                divErrorPanel.style.display = "block";

                window.scroll(0, 0);
                return false;
            }
            else {
                divErrorPanel.style.display = "none";
                //MsgPopupReturnTrue('Save');
                return true;
            }
            // end of show error divErroList
        }        
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
        <legend>Journal Adding</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel3">
            <ContentTemplate>
                <table width="95%" align="center" cellpadding="3" cellspacing="3" border="0">
                    <tr>
                        <td width="20%" align="right">
                            SP Type
                        </td>
                        <td width="20%">
                            <asp:DropDownList runat="server" ID="ddlSPType" Width="140px">
                            </asp:DropDownList>
                        </td>
                        <td width="30%" align="right">
                            Transaction Date
                        </td>
                        <td width="30%">
                            <asp:TextBox runat="server" ID="txtTransDate" CssClass="textInput" Width="140px"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Account No
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlAccountNo" Width="140px">
                            </asp:DropDownList>
                        </td>
                        <td colspan="2">
                            <%--<asp:TextBox runat="server" ID="txtAccName" CssClass="textInput" Width="300px" onblur="blurActiveInputWithObj(this)"
                        onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>--%>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="98%" align="center" cellpadding="2" cellspacing="2" border="0">
                    <tr>
                        <td>
                            Narration
                        </td>
                        <td>
                            Maker
                        </td>
                        <td>
                            DrCr
                        </td>
                        <td>
                            Currency
                        </td>
                        <td>
                            Amount
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox runat="server" ID="txtNarration" CssClass="textInput" Width="240px"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlMaker" Width="140px" />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlDrCr" Width="140px">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Text="Dr" Value="D"></asp:ListItem>
                                <asp:ListItem Text="Cr" Value="C"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCurrency" Width="140px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAmount" CssClass="textInput" Width="140px" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnAdd" Text="Add" CssClass="ButtonAsh" OnClick="btnAdd_Click"
                                OnClientClick="return AddValidation()" /><br />
                            <div align="right" style="padding-top: 6px">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Reset&nbsp;&nbsp;"
                                    ID="btnReset" OnClick="btnReset_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <div class="PageCaptionRight">
        Journal</div>
    <asp:UpdatePanel runat="server" ID="UpdatePanel5">
        <ContentTemplate>
            <div style="height: 200px; width: 100%; overflow: auto;">
                <asp:GridView Style="width: 98%" ID="gvAccRecon" runat="server" AutoGenerateColumns="False"
                    SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvAccRecon_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="SPTypeID" HeaderText="SP Type" />
                        <asp:BoundField DataField="AccountNo" HeaderText="Account No" />
                        <asp:BoundField DataField="Narration" HeaderText="Narration" />
                        <asp:BoundField DataField="CurrencyID" HeaderText="Currency" />
                        <asp:BoundField DataField="DrCr" HeaderText="DrCr" />
                        <asp:BoundField DataField="Amount" HeaderText="Amount" />
                        <asp:TemplateField HeaderText="Edit" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Button ID="btnEdit" CssClass="ButtonAsh" CommandName="Select" runat="server"
                                    Text="Edit" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remove" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Button ID="btnRemove" CssClass="ButtonAsh" CommandName="Remove" runat="server"
                                    Text="Remove" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <div align="right" style="padding-top: 6px">
                <%--<asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;&nbsp;Reset&nbsp;&nbsp;"
                    ID="btnReset" OnClick="btnReset_Click" />--%>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <fieldset>
        <legend>Reconciliation Specification</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel6">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="3" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right" width="30%">
                            Total Debit Amount
                        </td>
                        <td width="20%">
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtTotDrAmt" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right" width="30%">
                            Total Credit Amount
                        </td>
                        <td width="20%">
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtTotCrAmt" onblur="blurActiveInputWithObj(this)"
                                onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total No. of Debit Transaction
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtNoOfDrTrans"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total No. of Credit Transaction
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="textInput" Width="130px" ID="txtNoOfCrTrans"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <%--End of User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" cellpadding="10" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="center">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClick="btnSave_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <uc2:Error ID="ucMessage" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

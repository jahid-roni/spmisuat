<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="JournalApprove.aspx.cs" Inherits="SBM_WebUI.mp.JournalApprove" %>

<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }

        var selectedChqValue = true;
        function SetSelectedValue(obj) {
            var objID = obj.id;
            var iObjID = objID.lastIndexOf("_");
            var baseID = objID.substr(0, iObjID);
            var oChkIsReconciled = document.getElementById(baseID + '_chkIsReconciled');
            selectedChqValue = oChkIsReconciled.checked;

        }

        function RefChange(obj) {
            var objID = obj.id;
            var iObjID = objID.lastIndexOf("_");
            var baseID = objID.substr(0, iObjID);
            var oTxtReceiveDate = document.getElementById(baseID + '_txtReceiveDt');
            var oTxtReconBy = document.getElementById(baseID + '_txtReconBy');
            var oTxtAmount = document.getElementById(baseID + '_txtAmount');
            var oChkIsReconciled = document.getElementById(baseID + '_chkIsReconciled');
            var oHdDrCr = document.getElementById(baseID + '_hdDrCr');
            var oUserName = document.getElementById('<%= hdLoginUserName.ClientID %>');


            /*Detail section */
            var oTotalDebitAmount = document.getElementById('<%= txtTotalDebitAmount.ClientID %>');
            var oTotalCreditAmount = document.getElementById('<%= txtTotalCreditAmount.ClientID %>');
            var oTotalNoofDebitTransaction = document.getElementById('<%= txtTotalNoofDebitTransaction.ClientID %>');
            var oTotalNoofCreditTransaction = document.getElementById('<%= txtTotalNoofCreditTransaction.ClientID %>');
            var oTotalBalance = document.getElementById('<%= txtTotalBalance.ClientID %>');


            obj.value = obj.value.replace(/^\s+|\s+$/g, '');    // TRIM Method

            if (obj.value == "") {
                oTxtReceiveDate.value = "";
                oTxtReconBy.value = "";
                oChkIsReconciled.checked = false;

            }
            if (obj.value != "") {
                oTxtReconBy.value = oUserName.value;
                var currentDt = new Date();
                var mm = currentDt.getMonth();
                var dd = currentDt.getDate();
                dd = (dd < 10) ? '0' + dd : dd;
                var yyyy = currentDt.getFullYear();
                var mthNames = new Array();
                mthNames = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
                var date = dd + '-' + mthNames[mm] + '-' + yyyy;
                oTxtReceiveDate.value = date;
                oChkIsReconciled.checked = true;
            }

            var tValue = "";
            if (oChkIsReconciled.checked != selectedChqValue) {
                if (oChkIsReconciled.checked == true) { // ++
                    if (oHdDrCr.value == 'D') {
                        oTotalNoofDebitTransaction.value = parseFloat(oTotalNoofDebitTransaction.value) + parseFloat(1);
                        tValue = parseFloat(oTotalDebitAmount.value) + parseFloat(oTxtAmount.value);
                        oTotalDebitAmount.value = tValue.toFixed(2);
                    } else if (oHdDrCr.value == 'C') {
                        oTotalNoofCreditTransaction.value = parseFloat(oTotalNoofCreditTransaction.value) + parseFloat(1);
                        tValue = parseFloat(oTotalCreditAmount.value) + parseFloat(oTxtAmount.value);
                        oTotalCreditAmount.value = tValue.toFixed(2);
                    }
                    tValue = parseFloat(oTotalBalance.value) + parseFloat(oTxtAmount.value);
                    oTotalBalance.value = tValue.toFixed(2);
                } else {  // --
                    if (oHdDrCr.value == 'D') {
                        oTotalNoofDebitTransaction.value = parseFloat(oTotalNoofDebitTransaction.value) - parseFloat(1);
                        tValue = parseFloat(oTotalDebitAmount.value) - parseFloat(oTxtAmount.value);
                        oTotalDebitAmount.value = tValue.toFixed(2);
                    } else if (oHdDrCr.value == 'C') {
                        oTotalNoofCreditTransaction.value = parseFloat(oTotalNoofCreditTransaction.value) - parseFloat(1);
                        tValue = parseFloat(oTotalCreditAmount.value) - parseFloat(oTxtAmount.value);
                        oTotalCreditAmount.value = tValue.toFixed(2);
                    }
                    tValue = parseFloat(oTotalBalance.value) - parseFloat(oTxtAmount.value);
                    oTotalBalance.value = tValue.toFixed(2);
                }
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <asp:HiddenField ID="hdLoginUserName" runat="server" />
    <fieldset>
        <legend>Search Criteria</legend>
        <table width="100%" align="left" class="tableBody" border="0" cellpadding="3">
            <tr>
                <td align="right">
                    Account Number
                </td>
                <td align="left">
                    <asp:DropDownList runat="server" ID="ddlAccNumbe" SkinID="ddlSmall">
                    </asp:DropDownList>
                </td>
                <td align="right">
                    Reconcile Date From
                </td>
                <td>
                    <asp:TextBox ID="txtRecFromDate" Width="100px" CssClass="textInput" runat="server"
                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="lblRefNo" runat="server" Text="Reference Number" />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlReferenceNum" SkinID="ddlSmall">
                    </asp:DropDownList>
                </td>
                <td align="right">
                    Reconcile Date To
                </td>
                <td>
                    <asp:TextBox ID="txtRecToDate" Width="100px" CssClass="textInput" runat="server"
                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" align="center">
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
                            <asp:GridView AutoGenerateColumns="false" Width="98%" ID="gvData" runat="server"
                                SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="SPTypeID" ItemStyle-Width="99px" HeaderText="SPType." />
                                    <asp:BoundField DataField="AccountNo" ItemStyle-Width="78px" HeaderText="AccountNo." />
                                    <asp:BoundField DataField="Narration" ItemStyle-Width="60px" ItemStyle-Wrap="true"
                                        HeaderText="Narration." HtmlEncode="false" />
                                    <asp:BoundField DataField="CurrencyID" ItemStyle-Width="78px" HeaderText="Currency." />
                                    <asp:BoundField DataField="DrCr" ItemStyle-Width="78px" HeaderText="DrCr" />
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Width="90px" Text='<%# Eval("Amount") %>' ID="txtAmount"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                CssClass="textInput" Enabled="false"></asp:TextBox>
                                            <asp:HiddenField runat="server" ID="hdDrCr" Value='<%# Eval("DrCr") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ref No">
                                        <ItemStyle HorizontalAlign="Left" Width="50px" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtRefNo" onClick="SetSelectedValue(this)" OnChange="RefChange(this)"
                                                runat="server" Width="150px" Enabled="true" Text='<%# Eval("ReferenceNo") %>'
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                CssClass="textInput" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IsReconciled">
                                        <ItemStyle HorizontalAlign="Left" Width="25px" />
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("IsReconciled") %>'
                                                ID="chkIsReconciled" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ValueDate" ItemStyle-Width="78px" HeaderText="Trans Date."
                                        DataFormatString="{0: dd-MMM-yyyy}" />
                                    <asp:TemplateField HeaderText="Recon Date.">
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Width="90px" Text='<%# Eval("ReceiveDate") %>' ID="txtReceiveDt"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                CssClass="textInput" Enabled="true"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ReconBy.">
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Width="90px" Text='<%# Eval("ReconBy") %>' ID="txtReconBy"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                CssClass="textInput" Enabled="true"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DownLoadBy" ItemStyle-Width="78px" HeaderText="DownLoadBy." />
                                    <asp:BoundField DataField="MakerID" ItemStyle-Width="78px" HeaderText="Maker." />
                                </Columns>
                                <EmptyDataTemplate>
                                    No record found
                                </EmptyDataTemplate>
                                <AlternatingRowStyle CssClass="odd" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <%--<asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />--%>
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>
        <legend runat="server" id="Legend1">Reconciliation Specification</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table border="0" width="98%" align="center" cellpadding="2">
                    <tr>
                        <td align="right">
                            Total Debit Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalDebitAmount" Width="100px" CssClass="textInput" runat="server"
                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Credit Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalCreditAmount" Width="100px" CssClass="textInput" runat="server"
                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Balance
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalBalance" Width="100px" CssClass="textInput" runat="server"
                                Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Total No. of Debit Transaction
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalNoofDebitTransaction" Width="100px" CssClass="textInput"
                                runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total No. of Credit Transaction
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtTotalNoofCreditTransaction" Width="100px" CssClass="textInput"
                                runat="server" Enabled="false" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <%--<asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <table width="95%" align="center" cellpadding="10" cellspacing="0" border="0">
            <tr>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick="MsgPopupReturnTrue('Approve')" />
                    <br />
                </td>
            </tr>
        </table>
    </fieldset>
    <asp:UpdatePanel runat="server" ID="UpdatePanel2">
        <ContentTemplate>
            <uc3:Error ID="ucMessage" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

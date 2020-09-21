<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SPIssue.aspx.cs" Inherits="SBM_WebUI.mp.SPIssue" %>

<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCCustomerDetail_HSB.ascx" TagName="CustomerDetails" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<%@ Register Src="~/UI/UC/UCCustomerLimitInfo.ascx" TagName="CustLimitInfo" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtDDQuantity]').keypress(function(e) { return intNumber(e); });

            $('input[id*=txtNDShare]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtNDAmount]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtTotalAmount]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtPDConvRate]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtPDPaymentAmount]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtAppliedAmount]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtNWDQuantity]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtNWDAmount]').keypress(function(e) { return intNumber(e); });
        }

        function resetNomineeDetatil() {

            $("#<%=txtNDName.ClientID %>").val('');
            $("#<%=txtNDRelation.ClientID %>").val('');
            $("#<%=txtNDAddress.ClientID %>").val('');
            $("#<%=txtNDShare.ClientID %>").val('');
            $("#<%=txtNDAmount.ClientID %>").val('');
            $("#<%=hdNomSlno.ClientID %>").val('');

            $("#<%=txtNDParmanentAddress.ClientID %>").val('');
            $("#<%=txtNationalID.ClientID %>").val('');
            //$("#<%=ddlNationalIDCountry.ClientID %>").val('');
            $("#<%=txtNationalID_IssueAt.ClientID %>").val('');
            $("#<%=txtPassportNo.ClientID %>").val('');
            //$("#<%=ddlPassportNoCountry.ClientID %>").val('');
            $("#<%=txtPassportNo_IssueAt.ClientID %>").val('');
            $("#<%=txtBirthCertificateNo.ClientID %>").val('');
            //$("#<%=ddlBirthCertificateNoCountry.ClientID %>").val('');
            $("#<%=txtBirthCertificateNo_IssueAt.ClientID %>").val('');
            $("#<%=txtTIN.ClientID %>").val('');
            $("#<%=txtPhone.ClientID %>").val('');
            $("#<%=txtEmailAddress.ClientID %>").val('');
        }

        function ShowPolicyDetail() {
            var reg = document.getElementById("<%=ddlSpType.ClientID %>");
            if (reg != null) {
                if (reg.selectedIndex == 0) {
                    MsgPopupReturnTrue('Info');
                } else {
                    PolicyDetailPopupReturnTrue();
                }
            }
            return true
        }
        function CheckJointCustomer() {
            var sErrorList = "";

            var vAppliedAmount = $("#<%=txtAppliedAmount.ClientID %>").val();
            vAppliedAmount = vAppliedAmount.toString().replace(/,/g, '');
            if (vAppliedAmount == "")
            {
                vAppliedAmount = 0;
            }
            if (parseInt(vAppliedAmount, 10) <= 0) {
                sErrorList += "<li>Please input the applied amount, before proceed to customer adding.</li>";
                return OpenErrorPanel(sErrorList, 'Save');
            }
            var rowsGvCustmerDetail = $("#<%=gvCustmerDetail.ClientID %> tr").length;
            var customerType = document.getElementById('<%=ddlCustomerType.ClientID %>')
            var sptype = document.getElementById('<%=ddlSpType.ClientID %>')
            if (customerType.value == "01" && (sptype.value == "3MS" || sptype.value == "BSP")) {
                if (parseInt(vAppliedAmount, 10) > 3000000) {
                    sErrorList += "<li>Individual limit cannot be more then 3000000 (Thirty Lac).</li>";
                    return OpenErrorPanel(sErrorList, 'Save');
                }
            }
            else if (customerType.value == "02" && (sptype.value == "3MS" || sptype.value == "BSP")) {
                if (parseInt(vAppliedAmount, 10) > 6000000) {
                    sErrorList += "<li>Joint limit cannot be more then 6000000 (Sixty Lac).</li>";
                    return OpenErrorPanel(sErrorList, 'Save');
                }
            }
            CheckAndCustomerDetailPopup();
            //if (customerType.value == "01") {
            //}
            //else {
            //    CheckAndCustomerDetailPopup();
            //}

        }
        function SaveValidation() {
            var sErrorList = "";

            var rowsGvCustmerDetail = $("#<%=gvCustmerDetail.ClientID %> tr").length;
            var rowsGvDenomDetail = $("#<%=gvDenomDetail.ClientID %> tr").length;

            var vIssueName = $("#<%=txtIssueName.ClientID %>").val();

            if (vIssueName.toString().toUpperCase().indexOf("GRATUITY") != -1)
            {
                sErrorList += "<li>You cannot proceed issue with Gratuity fund</li>";
            }
            else if (vIssueName.toString().toUpperCase().indexOf("SERVICE BENEFIT") != -1)
            {
                sErrorList += "<li>You cannot proceed issue with End of Service Benefit Fund</li>";
            }
            else if (vIssueName.toString().toUpperCase().indexOf("SUPERANNATION") != -1)
            {
                sErrorList += "<li>You cannot proceed issue with Superannuation Fund</li>";
            }
            else if (vIssueName.toString().toUpperCase().indexOf("LONG TERM BENEFIT") != -1)
            {
                sErrorList += "<li>You cannot proceed issue with Long term benefit fund</li>";
            }
            else if (vIssueName.toString().toUpperCase().indexOf("RETIREMENT BENEFIT") != -1)
            {
                sErrorList += "<li>You cannot proceed issue with Retirement Benefit fund</li>";
            }

            var vAppliedAmount = $("#<%=txtAppliedAmount.ClientID %>").val();
            vAppliedAmount = vAppliedAmount.toString().replace(/,/g, '');
            var vTotalAmount = $("#<%=txtTotalAmount.ClientID %>").val();
            vTotalAmount = vTotalAmount.toString().replace(/,/g, '');

            if (parseInt(vAppliedAmount, 10) < parseInt(vTotalAmount, 10)) {
                sErrorList += "<li>You cannot exceed total amount</li>";
            }
            if (parseInt(vAppliedAmount, 10) != parseInt(vTotalAmount, 10)) {
                sErrorList += "<li>Total Amount & Total Certificate Amount must be equal</li>";
            }

            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=ddlBranch.ClientID %>', 'DropDownList', "Branch Code cannot be empty!"); 
            sErrorList += RequiredData('<%=ddlCustomerType.ClientID %>', 'DropDownList', "Customer Type cannot be empty!");
            sErrorList += RequiredData('<%=txtTotalAmount.ClientID %>', 'TextBox', "Total Amount cannot be empty!");
            sErrorList += RequiredData('<%=txtIssueDate.ClientID %>', 'TextBox', "Issue Date cannot be empty!");

            sErrorList += RequiredData('<%=txtIssueName.ClientID %>', 'TextBox', "Issue Name cannot be empty!");
            sErrorList += RequiredData('<%=ddlCollectionBranch.ClientID %>', 'DropDownList', "Collection Branch Code cannot be empty!"); 
            if (rowsGvCustmerDetail == 1 || rowsGvCustmerDetail == 0) {
                sErrorList += "<li>Customer Detail cannot be null</li>";
            }

            if (rowsGvDenomDetail == 1 || rowsGvDenomDetail == 0) {
                sErrorList += "<li>Denomination cannot be null</li>";
            }

            var hdIsBondHolderRequired = document.getElementById('<%=hdIsBondHolderRequired.ClientID %>');

            if (hdIsBondHolderRequired != null) {

                if (hdIsBondHolderRequired.value == "True") {
                    sErrorList += RequiredData('<%=txtBHDHolderName.ClientID %>', 'TextBox', "Holder Name cannot be empty!");
                }
            }

            sErrorList += RequiredData('<%=txtTotalAmount.ClientID %>', 'TextBox', "Total Denomination Amount cannot be empty!");
            sErrorList += RequiredData('<%=ddlPDPaymentMode.ClientID %>', 'DropDownList', "Payment Mode cannot be empty!");
            sErrorList += RequiredData('<%=ddlPDCurrencyCode.ClientID %>', 'DropDownList', "Currency Code cannot be empty!");
            var paymentMode = $("#<%=ddlPDPaymentMode.ClientID %> option:selected").val();
            if (paymentMode == 4 || paymentMode == 5 || paymentMode == 6) {
                sErrorList += RequiredData('<%=txtPDAccDraftNo.ClientID %>', 'TextBox', "Account/Draft No. cannot be empty!");
                sErrorList += RequiredData('<%=txtPDAccName.ClientID %>', 'TextBox', "Account Name cannot be empty!");
            }
            if (paymentMode == 2) {
                sErrorList += RequiredData('<%=txtPDDraftNo.ClientID %>', 'TextBox', "Cheque No. cannot be empty!");
            }
            if (paymentMode == 3) {
                sErrorList += RequiredData('<%=txtPDDraftNo.ClientID %>', 'TextBox', "Draft No. cannot be empty!");
            }
            sErrorList += RequiredData('<%=txtPDConvRate.ClientID %>', 'TextBox', "Conv. Rate cannot be empty!");
            sErrorList += RequiredData('<%=txtPDPaymentAmount.ClientID %>', 'TextBox', "Payment Amount cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
        }

        function SaveDenomValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtAppliedAmount.ClientID %>', 'TextBox', "Total Amount cannot be empty!");
            sErrorList += RequiredData('<%=ddlDDDenom.ClientID %>', 'DropDownList', "Denomination cannot be empty!");
            sErrorList += RequiredData('<%=txtDDQuantity.ClientID %>', 'TextBox', "Quantity cannot be empty!");

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
                var ddlDDDenom = $("#<%=ddlDDDenom.ClientID %>").val();
                var vAppliedAmount = $("#<%=txtAppliedAmount.ClientID %>").val().replace(/,/g, '');
                var oTotalAmount = document.getElementById('<%=txtTotalAmount.ClientID %>');
                var vTotalAmount = "";
                if (oTotalAmount.value != "") {
                    vTotalAmount = oTotalAmount.value.replace(/,/g, '');
                }
                else {
                    vTotalAmount = 0;
                }
                var vDDQuantity = $("#<%=txtDDQuantity.ClientID %>").val();
                var cValue = parseInt(ddlDDDenom, 10) * parseInt(vDDQuantity, 10);
                var cTotalAmount = parseFloat(vTotalAmount) + parseFloat(cValue);
                if (parseFloat(vAppliedAmount) < cTotalAmount) {
                    //MsgPopupReturnTrue('Info');
                } else {
                    //MsgPopupReturnTrue('Save');
                    return true;
                }
            }
            // end of show error divErroList
        }

        function CalculateAmount(vType) {
            var oAmount = document.getElementById('<%=txtAppliedAmount.ClientID %>');
            var oPaymentAmount = document.getElementById('<%=txtPDPaymentAmount.ClientID %>');
            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');

            var vTotalAmount = "";
            if (oAmount.value != "") {
                vTotalAmount = oAmount.value.replace(/,/g, '');
                oPaymentAmount.value = oAmount.value;
                CloseErrorPanel();
            }
            else {
                OpenErrorPanel("<li>Total Amount cannot be empty!</li>", "");
                oPaymentAmount.value = "";
                oNDShare.value = "";
                oNDAmount.value = "";
                return false;
            }
            vTotalAmount = parseFloat(vTotalAmount);


            // this is by total amount
            if (vType == "Amount") {
                if (oNDAmount.value != "") {
                    if (parseFloat(oNDAmount.value) > parseFloat(vTotalAmount)) {
                        OpenErrorPanel("<li>Total Nominee amount cannot be exceeded from Total Amount</li>", "");
                        oNDShare.value = "";
                        return false;
                    }
                    var vNDAmount = parseFloat(oNDAmount.value);
                    var vFinalVal = (parseFloat(vNDAmount) * 100) / vTotalAmount;
                    vFinalVal = vFinalVal.toFixed(0);
                    oNDShare.value = vFinalVal;
                    CloseErrorPanel();
                    return true;
                }
                else {
                    oNDShare.value = "";
                    return false;
                }
            }

            // this is by share amount
            else if (vType == "Share") {
                if (oNDShare.value != "") {
                    if (parseFloat(oNDShare.value) > 100) {
                        OpenErrorPanel("<li>Total amount of share cannot be exceeded 100!</li>", "");
                        oNDAmount.value = "";
                        return false;
                    }
                    var vShare = parseFloat(oNDShare.value);
                    var vFinalVal = (parseFloat(vShare) * vTotalAmount) / 100;
                    vFinalVal = vFinalVal.toFixed(2);

                    oNDAmount.value = vFinalVal;
                    CloseErrorPanel();
                    return true;
                }
                else {
                    oNDAmount.value = "";
                    return false;
                }
            }
        }

        function SaveNomineeValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtAppliedAmount.ClientID %>', 'TextBox', "Total Amount cannot be empty!");
            sErrorList += RequiredData('<%=txtNDName.ClientID %>', 'TextBox', "Nominee Name cannot be empty!");
            sErrorList += RequiredData('<%=txtNDShare.ClientID %>', 'TextBox', "Share cannot be empty!");
            sErrorList += RequiredData('<%=txtNDRelation.ClientID %>', 'TextBox', "Relation cannot be empty!");
            sErrorList += RequiredData('<%=txtNDateofBirth.ClientID %>', 'TextBox', "Nominee date of birth be empty!");
            sErrorList += RequiredData('<%=ddlResidentCountry.ClientID %>', 'DropDownList', "Residence country cannot be empty!");
            sErrorList += RequiredData('<%=txtNDAddress.ClientID %>', 'TextBox', " Present address cannot be empty!");
            sErrorList += RequiredData('<%=txtNDAmount.ClientID %>', 'TextBox', "Amount cannot be empty!");

            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');
            if (oNDShare.value != "") {
                if (parseInt(oNDShare.value, 10) > 100) {
                    sErrorList += "<li>Total amount of share cannot be exceeded 100!</li>";
                    oNDAmount.value = "";
                }
            }
            return OpenErrorPanel(sErrorList, '');
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }

        function ChequeAndDraft(obj) {
            var lbl = document.getElementById('ctl00_cphDet_lblPDDraftNo');
            var txtDraftNo = document.getElementById('ctl00_cphDet_txtPDDraftNo');
            var txtAccName = document.getElementById('ctl00_cphDet_txtPDAccName');
            var oAccDraftNo = document.getElementById('ctl00_cphDet_txtPDAccDraftNo');
            oAccDraftNo.value = "";
            txtAccName.value = "";
            txtDraftNo.value = "";

            if (obj.value == "2") {         // Cheque
                lbl.innerHTML = "Cheque"
                txtDraftNo.disabled = false;
                oAccDraftNo.disabled = true;

            } else if (obj.value == "3") {  // Draft
                // to active draft TextBox
                // to change label
                lbl.innerHTML = "Draft";
                txtDraftNo.disabled = false;
                oAccDraftNo.disabled = true;

            } else if (obj.value == "4" || obj.value == "5" || obj.value == "6") {  // Account No
                lbl.innerHTML = "Draft";
                txtDraftNo.disabled = true;
                oAccDraftNo.disabled = false;

            } else {
                lbl.innerHTML = "Draft";
                txtDraftNo.disabled = true;
                oAccDraftNo.disabled = true;
            }
        }

        function Delete() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdReceiveTransNo = document.getElementById('<%=hdTransNo.ClientID %>');
            if (hdReceiveTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Issue Data')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Issue transaction has not been selected</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function CheckAndCustomerDetailPopup() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var cType = document.getElementById("<%=ddlCustomerType.ClientID %>");
            if (cType != null) {
                if (cType.selectedIndex == 0 || cType.selectedIndex == -1) {
                    // show error divErrorList
                    var errorList = document.getElementById('divErrorList');
                    errorList.innerHTML = "<ul><li>Customer Type has not been selected yet</li></ul>";
                    divErrorPanel.style.display = "block";
                    window.scroll(0, 0);
                    return false;
                    // end of show error divErroList
                } else {
                    divErrorPanel.style.display = "none";
                    CloseErrorPanel();
                    CustomerDetailPopup();
                    return true;
                }
            }
            return false;
        }

        function ViewJournalValidation() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdIssueTransNo = document.getElementById('<%=hdTransNo.ClientID %>');
            if (hdIssueTransNo.value != "") {
                return true;
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Issue transaction has not been selected/loaded</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }
        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%-- Error --%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%-- Error --%>
    <fieldset runat="server" id="fsList">
        <legend runat="server" id="lgText">Unapproved Issue List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdReg" />
                <asp:HiddenField runat="server" ID="hdTransNo" />
                <asp:HiddenField runat="server" ID="hdDenom" />
                <asp:HiddenField runat="server" ID="hdIsBondHolderRequired" />
                <asp:HiddenField runat="server" ID="hdSupportdGndr" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="910px"
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" OnClientClick="CloseErrorPanel()"
                                                runat="server" ID="btnSelect" Text="Select" />
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
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnSave" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReject" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDelete" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnApprove" />
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnReset" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0" cellpadding="0" cellspacing="1">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSpType" SkinID="ddlLarge" runat="server" OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged"
                                    AutoPostBack="True" TabIndex="1">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlBranch" SkinID="ddlMedium" runat="server" TabIndex="2">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td align="left">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                    ReadOnly="true" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Registration No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegistrationNo" Width="120px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    MaxLength="25"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" OnClick="btnRegSearch_Click" Text="Search"
                                    ID="btnRegSeach" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo'),CloseErrorPanel()" /></div>
                        </td>
                        <td align="right">
                            Customer Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCustomerType" SkinID="ddlMedium" runat="server" TabIndex="3">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Total Amount
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAppliedAmount" AutoPostBack="true" Width="100px" CssClass="textInput"
                                    runat="server" MaxLength="19" OnTextChanged="txtAppliedAmount_TextChanged" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" TabIndex="4"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="txtAppliedAmount" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
        <br />
    <!-- start to 3 Tab panel  -->
    <fieldset>
        <legend>Customer(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                <ContentTemplate>
                    <div style="height: 26%; width: 100%; overflow: auto;">
                        <table border="0">
                            <tr>
                                <td>
                                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Add" ID="btnCDAdd" Width="80px"
                                        OnClientClick="CheckJointCustomer()" TabIndex="5" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="height: 70%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvCustmerDetail" runat="server"
                            AutoGenerateColumns="false" SkinID="SBMLGridGreen" OnRowCommand="gvCustmerDetail_RowCommand"
                            OnRowDeleting="gvCustmerDetail_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnCusDetailSelect"
                                            Text="Select" OnClientClick="return CustomerDetailPopupReturnTrue()" />
                                        <asp:HiddenField runat="server" ID="hdTmpCustomerID" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnCustDetailDelete"
                                            Text="Delete" OnClientClick="return CheckForDelete('this customer')" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="CustomerID" HeaderText="Customer ID" />
                                <asp:BoundField DataField="CustomerName" HeaderText="Name" />
                                <asp:BoundField DataField="DateOfBirth" HeaderText="Date Of Birth" />
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Customer(s) Details Tab  --%>
        <%-- Nominee(s) Details Tab  --%>
        <br />
        <fieldset>
        <legend style="background-color: ButtonFace">Nominee(s) Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <div style="height: 40%; width: 100%; overflow: auto;">
                    <table>
                        <tr>
                            <td>Name
                            </td>
                            <td>Relation
                            </td>
                            <td>TIN
                            </td>
                            <td>Share
                            </td>
                            <td>Amount
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtNDName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="100"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNDRelation" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="20"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtTIN" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="150"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNDShare" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Share')"
                                    onfocus="highlightActiveInputWithObj(this)" Width="150px" MaxLength="3"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNDAmount" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Amount')"
                                    onfocus="highlightActiveInputWithObj(this)" Width="150px" MaxLength="9"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>NationalID</td>
                            <td>PassportNo</td>
                            <td>BirthCertificateNo</td>
                            <td>ResidentStatus</td>
                            <td>Phone</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtNationalID" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="100"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPassportNo" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="20"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBirthCertificateNo" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="150"></asp:TextBox>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlResidenceStatus" runat="server" SkinID="ddlSmall" TabIndex="40" Width="150px">
                                    <asp:ListItem Text="Resident" Value="RS"></asp:ListItem>
                                    <asp:ListItem Text="Non Resident" Value="NRS"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Amount')"
                                    onfocus="highlightActiveInputWithObj(this)" Width="150px" MaxLength="20"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>NationalID Country</td>
                            <td>PassportNo Country</td>
                            <td>BirthCertificateNo Country</td>
                            <td>Resident Country</td>
                            <td>Email Address</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlNationalIDCountry" runat="server" AutoPostBack="true" SkinID="ddlSmall" TabIndex="39" Width="190px">
                                    <asp:ListItem Text="BANGLADESH" Value="BANGLADESH"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlPassportNoCountry" runat="server" AutoPostBack="true" SkinID="ddlSmall" TabIndex="39" Width="190px">
                                    <asp:ListItem Text="BANGLADESH" Value="BANGLADESH"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlBirthCertificateNoCountry" runat="server" AutoPostBack="true" SkinID="ddlSmall" TabIndex="39" Width="190px">
                                    <asp:ListItem Text="BANGLADESH" Value="BANGLADESH"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlResidentCountry" runat="server" AutoPostBack="true" SkinID="ddlSmall" TabIndex="39" Width="150px">
                                    <asp:ListItem Text="BANGLADESH" Value="BANGLADESH"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this), CalculateAmount('Amount')"
                                    onfocus="highlightActiveInputWithObj(this)" Width="150px" MaxLength="9"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>NationalID IssueAt</td>
                            <td>PassportNo IssueAt</td>
                            <td>BirthCertificateNo IssueAt</td>
                            <td>Date of Birth
                                <td>
                            Gender
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtNationalID_IssueAt" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="100"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPassportNo_IssueAt" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="20"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBirthCertificateNo_IssueAt" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="190px" MaxLength="150"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNDateofBirth" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="150px" MaxLength="150"></asp:TextBox>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlGender" runat="server" SkinID="ddlSmall" TabIndex="40" Width="150px">
                                    <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                    <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">Present Address</td>
                            <td colspan="2">Permanent Address</td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="txtNDAddress" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="300px" Height="35px" MaxLength="100"></asp:TextBox>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtNDParmanentAddress" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" Width="300px" Height="35px" MaxLength="100"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnNDAdd" runat="server" CssClass="ButtonAsh" Text="Add" Width="80px"
                                    OnClick="btnNDAdd_Click" OnClientClick="return SaveNomineeValidation()" />
                                &nbsp
                                    <asp:Button ID="btnNDReset" runat="server" CssClass="ButtonAsh" Text="Reset" Width="80px"
                                        OnClientClick="resetNomineeDetatil(), CloseErrorPanel()"/>

                            </td>
                        </tr>
                    </table>
                    <asp:HiddenField ID="hdNomSlno" Value="" runat="server" />
                </div>
                <div style="height: 60%; width: 100%; overflow: auto;">
                    <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDetail" OnRowCommand="gvNomDetail_RowCommand"
                        runat="server" AutoGenerateColumns="False" SkinID="SBMLGridGreen" OnRowDeleting="gvNomDetail_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="Select">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnNDSelect"
                                        Text="Select" />
                                    <asp:HiddenField ID="hdNomineeSlno" Value='<%# Eval("SlNo") %>' runat="server" />
                                </ItemTemplate>
                                <HeaderStyle Width="5%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnNDDelete"
                                        Text="Delete" OnClientClick="return CheckForDelete('this Nominee info?')" />
                                </ItemTemplate>
                                <HeaderStyle Width="5%" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="NomineeName" HeaderText="Nominee Name" />
                            <asp:BoundField DataField="Relation" HeaderText="Relation" />
                            <asp:BoundField DataField="Address" HeaderText="Address" />
                            <asp:BoundField DataField="NomineeShare" HeaderText="Share" />
                            <asp:BoundField DataField="Amount" HeaderText="Amount" />
                        </Columns>
                        <EmptyDataTemplate>
                            No record found
                        </EmptyDataTemplate>
                        <AlternatingRowStyle CssClass="odd" />
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
        <%-- End of Nominee(s) Details Tab  --%>
        <%-- Nominee(s) Wise Denomination Tab  --%>
        <%--<div class="tabbertab" runat="server">
            <h2>
                Nominee(s) Wise Denomination</h2>
            <div style="width: 100%; overflow: auto;" id="divNomineeWiseDenomTab" runat="server">
                <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td>
                                    Name
                                </td>
                                <td>
                                    Denomination
                                </td>
                                <td>
                                    Quantity
                                </td>
                                <td colspan="2">
                                    Amount
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlNWDName" SkinID="ddlLarge" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlNWDDenom" SkinID="ddlMedium" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNWDQuantity" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNWDAmount" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="100px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNWDAdd" runat="server" CssClass="ButtonAsh" Text="Add" Width="80px" />
                                    <asp:Button ID="btnNWDReset" runat="server" CssClass="ButtonAsh" Text="Reset" Width="80px" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div style="width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDemon" runat="server" AutoGenerateColumns="False"
                            SkinID="SBMLGridGreen">
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnSelect"
                                            Text="Select" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="5%" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Val1" HeaderText="Nominee Name" />
                                <asp:BoundField DataField="Val1" HeaderText="Denomination" />
                                <asp:BoundField DataField="Val1" HeaderText="Quantity" />
                                <asp:BoundField DataField="Val1" HeaderText="Amount" />
                            </Columns>
                            <EmptyDataTemplate>
                                No record found
                            </EmptyDataTemplate>
                            <AlternatingRowStyle CssClass="odd" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>--%>
        <%-- End of Nominee(s) Wise Denomination Tab  --%>
    <!-- </div> -->
    <!-- End of 3 Tab panel  -->
    <br />
    <!-- Issue name panel -->
    <fieldset>
        <legend>Policy Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" border="0">
                    <tr>
                        <td colspan="4" align="left">
                            <asp:Button CssClass="ButtonAsh" OnClick="btnShowPolicy_Click" runat="server" OnClientClick="return ShowPolicyDetail()"
                                Text="Show Policy" ID="btnShowPolicy" />
                            <%--<asp:Button CssClass="ButtonAsh" runat="server" OnClientClick="return NotDoneYet()"
                                Text="Print Limit" ID="bnnPrintLimit" />--%>
                            <asp:Button CssClass="ButtonAsh" runat="server" OnClick="btnLimitStatus_Click" Text="Limit Status"
                                ID="btnLimitStatus" />
                            <%--<asp:CheckBox runat="server" ID="chkFiscalYear" Text="Include Fiscal Year" />--%>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Issue Name
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueName" Width="500px" TextMode="MultiLine" Height="37" CssClass="textInput"
                                    TabIndex="13" runat="server" onblur="blurActiveInputWithObj(this)" MaxLength="100"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right" valign="top">
                            Master No
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtMasterNo" Width="100px" MaxLength="7" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" OnTextChanged="txtMasterNo_TextChanged"
                                AutoPostBack="true" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox><br />
                            <asp:Label runat="server" ID="lblMasterVarified" Text="Not verified yet"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnLimitStatus" />
            </Triggers>
        </asp:UpdatePanel>
    </fieldset>
    <!-- end of Issue name panel -->
    <!-- Start to Bond Holder Details panel-->
    <br />
    <fieldset>
        <legend>Bond holder detailsholder details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">
                            Holder Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDHolderName" MaxLength="50" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Relation
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDRelation" MaxLength="50" Width="100px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Address
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDAddress" MaxLength="315" Width="275px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Height="19px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <!-- End of Bond Holder Details panel-->
    <!-- start to Denomination(s) details   &   Payment Details -->
    <br />
    <table width="100%" align="center" class="tableBody" border="0">
        <tr>
            <td valign="top" width="60%">
                <fieldset>
                    <legend>Denomination(s) details</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel6">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="1" cellspacing="1"
                                border="0">
                                <tr>
                                    <td width="50%" valign="top">
                                        <div style="height: 168px; width: 98%; overflow: auto; border: solid 1px white">
                                            <asp:GridView Style="width: 94%" ID="gvDenomDetail" runat="server" AutoGenerateColumns="false"
                                                SkinID="SBMLGridGreen" ShowHeader="true" OnRowCommand="gvDenomDetail_RowCommand"
                                                OnRowDataBound="gvDenomDetail_RowDataBound" OnRowDeleting="gvDenomDetail_RowDeleting">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Select">
                                                        <ItemStyle Width="15px" />
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" CommandName="Delete"
                                                                ID="btnDenomRemove" OnClientClick="return CheckForDelete('this denomination')" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="DenominationID" HeaderText="Denomination" />
                                                    <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No Record Found.
                                                </EmptyDataTemplate>
                                                <HeaderStyle CssClass="ssHeader" />
                                                <AlternatingRowStyle CssClass="odd" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td width="50%" valign="top">
                                        <div>
                                            &nbsp;</div>
                                        <table width="100%" align="center" class="tableBody" border="0">
                                            <tr>
                                                <td align="right">
                                                    Denomination
                                                </td>
                                                <td>
                                                    <div class="fieldLeft">
                                                        <asp:DropDownList ID="ddlDDDenom" SkinID="ddlCommon" Width="125px" runat="server"
                                                            TabIndex="14">
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="errorIcon">
                                                        *</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    Quantity
                                                </td>
                                                <td>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox MaxLength="4" Width="120px" ID="txtDDQuantity" CssClass="textInput"
                                                            TabIndex="15" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                                    </div>
                                                    <div class="errorIcon">
                                                        *</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td>
                                                    <asp:Button CssClass="ButtonAsh" Width="80px" runat="server" Text="Add" ID="btnAddDenomination"
                                                        TabIndex="16" OnClick="btnAddDenomination_Click" OnClientClick="return SaveDenomValidation()" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    Total Amount
                                                </td>
                                                <td>
                                                    <asp:TextBox Width="120px" ID="txtTotalAmount" CssClass="textInput" runat="server"
                                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                        MaxLength="9" ReadOnly="true"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <triggers>
                                <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnDenomRemove" />
                            </triggers>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
                <%-- <br />--%>
            </td>
            <td valign="top" width="40%">
                <fieldset>
                    <legend>Payment Details</legend>
                    <asp:UpdatePanel runat="server" ID="UpdatePanel9">
                        <ContentTemplate>
                            <table width="100%" align="center" class="tableBody" cellpadding="1" cellspacing="0"
                                border="0">
                                <tr>
                                    <td align="right">
                                        Payment Mode
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:DropDownList SkinID="ddlMedium" ID="ddlPDPaymentMode" Width="120px" runat="server"
                                                onChange="ChequeAndDraft(this)">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Currency Code
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:DropDownList SkinID="ddlMedium" ID="ddlPDCurrencyCode" Width="180px" runat="server"
                                                AutoPostBack="True" OnSelectedIndexChanged="ddlPDCurrencyCode_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="errorIcon">
                                            *</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Account/Draft No
                                    </td>
                                    <td>
                                        <asp:TextBox MaxLength="12" Width="180px" ID="txtPDAccDraftNo" CssClass="textInput"
                                            TabIndex="17" AutoPostBack="true" runat="server" OnTextChanged="txtPDAccDraftNo_TextChanged"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Account Name
                                    </td>
                                    <td>
                                        <asp:TextBox Width="180px" ID="txtPDAccName" ReadOnly="true" MaxLength="75" CssClass="textInput"
                                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label runat="server" ID="lblPDDraftNo" Text="Draft No"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox Width="180px" ID="txtPDDraftNo" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Conv. Rate
                                    </td>
                                    <td>
                                        <asp:TextBox Width="180px" MaxLength="8" ID="txtPDConvRate" CssClass="textInput"
                                            AutoPostBack="true" runat="server" OnTextChanged="txtPDConvRate_TextChanged"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        Payment Amount
                                    </td>
                                    <td>
                                        <asp:TextBox Width="180px" MaxLength="15" Enabled="false" ID="txtPDPaymentAmount"
                                            CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">Collection Branch
                                    </td>
                                    <td>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlCollectionBranch" SkinID="ddlMedium" runat="server" TabIndex="2">
                                            </asp:DropDownList>
                                        </div>

                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
            </td>
        </tr>
    </table>
    <!-- end of Denomination(s) details   &   Payment Details -->
    <%-- <br />--%>
    <%--Start to User Comments--%>
    <asp:UpdatePanel runat="server" ID="upUserDet">
        <ContentTemplate>
            <uc1:UserDetails ID="ucUserDet" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--End of User Comments--%>
    <br />
    <fieldset>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="left">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="View Journals" ID="btnViewJournals"
                        OnClientClick="return ViewJournalValidation()" OnClick="btnViewJournals_Click"
                        Visible="false" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve')" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClientClick="CloseErrorPanel()"
                        OnClick="btnReset_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        TabIndex="18" OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return Delete()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc2:CustomerDetails ID="CustomerDetail" runat="server" />
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" type="1" />
    <uc3:Error ID="ucMessage" runat="server" />
    <uc5:CustLimitInfo ID="CustLimitInfo" runat="server" />
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="IntPayment.aspx.cs" Inherits="SBM_WebUI.mp.IntPayment" %>

<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCViewGridData.ascx" TagName="VGData" TagPrefix="uc4" %>
<%@ Register Src="../UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="../UC/UCSearchIPE.ascx" TagName="SearchIPE" TagPrefix="uc5" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
            $('input[id*=txtTotalAmount]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtPDConvRate]').keypress(function(e) { return floatNumber(e); });
            $('input[id*=txtFromScrip]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtToScrip]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtFromCoupon]').keypress(function(e) { return intNumber(e); });
            $('input[id*=txtToCoupon]').keypress(function(e) { return intNumber(e); });
        }

        function CheckTextBox() {
            var TargetBaseControl = document.getElementById('<%= this.gvPaymentDetails.ClientID %>');
            var TargetChildControl = "chkSelected";
            var Inputs = TargetBaseControl.getElementsByTagName("input");

            var iCouponCount = 0;
            var dCalInterest = 0;
            var dCalIncomeTax = 0;
            var dSSAmount = 0;

            for (var n = 0; n < Inputs.length; ++n) {
                if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf(TargetChildControl, 0) >= 0) {
                    var chkBoxID = Inputs[n].id;
                    var iIndex = chkBoxID.lastIndexOf("_")
                    var oText = document.getElementById(chkBoxID.substr(0, iIndex) + '_txtInterestAmount');
                    var oIncomeTax = document.getElementById(chkBoxID.substr(0, iIndex) + '_lblIncomeTax');
                    var temp = "";
                    if (oText != null) {
                        if (Inputs[n].checked == true) {
                            iCouponCount++;
                            temp = oText.value.toString().replace(/,/g, '');
                            dCalInterest = parseFloat(dCalInterest) + parseFloat(temp);
                            dCalInterest = dCalInterest.toFixed(2);

                            var tmpIncomeTax = oIncomeTax.innerHTML.toString().replace(/,/g, '');
                            dCalIncomeTax = parseFloat(dCalIncomeTax) + parseFloat(tmpIncomeTax);
                            dCalIncomeTax = dCalIncomeTax.toFixed(2);

                            //oText.disabled = false;

                            var vIntTemp = parseFloat(temp);
                            vIntTemp = vIntTemp.toFixed(2);
                            vIntTemp = AddingCommas(vIntTemp);
                            oText.value = vIntTemp;
                        } else {
                            oText.disabled = true;
                        }
                    }
                }
            }
            dSSAmount = dSSAmount.toFixed(2);

            //#region Assign Data in calculation field set
            var txtPDCouponInsSelected = document.getElementById('<%= txtPDCouponInsSelected.ClientID %>');
            txtPDCouponInsSelected.value = iCouponCount;

            var txtPDSocialSecurityAmount = document.getElementById('<%= txtPDSocialSecurityAmount.ClientID %>');
            txtPDSocialSecurityAmount.value = AddingCommas(dSSAmount);

            var txtPDIncomeTax = document.getElementById('<%= txtPDIncomeTax.ClientID %>');
            txtPDIncomeTax.value = AddingCommas(dCalIncomeTax);

            var txtPDCalcInterest = document.getElementById('<%= txtPDCalcInterest.ClientID %>');
            txtPDCalcInterest.value = AddingCommas(dCalInterest);

            var txtPDIntPayable = document.getElementById('<%= txtPDIntPayable.ClientID %>');
            txtPDIntPayable.value = AddingCommas(dCalInterest);

            var dCalPayAmount = parseFloat(dCalInterest) - parseFloat(dCalIncomeTax) - parseFloat(dSSAmount);
            var txtPDPaymentAmount = document.getElementById('<%= txtPDPaymentAmount.ClientID %>');
            dCalPayAmount = dCalPayAmount.toFixed(2);
            txtPDPaymentAmount.value = AddingCommas(dCalPayAmount);

            var txtPDConvertedAmount = document.getElementById('<%= txtPDConvertedAmount.ClientID %>');
            var txtPDConvRate = document.getElementById('<%= txtPDConvRate.ClientID %>');
            var ConvertedAmount = parseFloat(dCalPayAmount) * parseFloat(txtPDConvRate.value);
            ConvertedAmount = ConvertedAmount.toFixed(2);
            txtPDConvertedAmount.value = AddingCommas(ConvertedAmount);
        }

        function RejectValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");

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
                MsgPopupReturnTrue('Reject');
                return true;
            }
            // end of show error divErroList
        }


        function SaveValidation() {
            var sErrorList = "";

            var TargetBaseControl = document.getElementById('<%= this.gvPaymentDetails.ClientID %>');
            var TargetChildControl = "chkSelected";

            var bCount = false;
            var Inputs = TargetBaseControl.getElementsByTagName("input");
            for (var n = 0; n < Inputs.length; ++n) {
                if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf(TargetChildControl, 0) >= 0) {
                    if (Inputs[n].checked == true) {
                        bCount = true;
                    }
                }
            }

            sErrorList += RequiredData('<%=hdRegNo.ClientID %>', 'TextBox', "You must load Issue first before viewing detail");
            if (bCount == false) {
                sErrorList += "<li>Please select at least one coupon\Installment to make interest payment.</li>";
            }
            sErrorList += RequiredData('<%=ddlPDPaymentMode.ClientID %>', 'DropDownList', "Payment Mode  cannot be empty!");
            sErrorList += RequiredData('<%=ddlPDCurrency.ClientID %>', 'DropDownList', "Currency  cannot be empty!");
            sErrorList += RequiredData('<%=txtPaymentDate.ClientID %>', 'TextBox', "Payment date cannot be empty!");
            sErrorList += RequiredData('<%=txtPDConvertedAmount.ClientID %>', 'TextBox', "Payment date cannot be empty!");
            var objPayment = document.getElementById('<%= ddlPDPaymentMode.ClientID %>');
            if (objPayment.value == "4" || objPayment.value == "5" || objPayment.value == "6") {
                sErrorList += RequiredData('<%=txtPDAccountNo.ClientID %>', 'TextBox', "Account No cannot be empty!");
                sErrorList += RequiredData('<%=txtPDAccountName.ClientID %>', 'TextBox', "Account Name cannot be empty!");
            }
            else if (objPayment.value == "3") {
                sErrorList += RequiredData('<%=txtPDDraftNo.ClientID %>', 'TextBox', "Draft No cannot be empty!");
            }
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
                MsgPopupReturnTrue('Save');
                return true;
            }
            // end of show error divErroList
        }

        function IntPayTransNoSearch() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtIntPayTransNo.ClientID %>', 'TextBox', "Int Payment Transaction No cannot be empty!");

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
                return true;
            }
            // end of show error divErroList
        }

        function ShowVGDataShow() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=hdIssueTransNo.ClientID %>', 'TextBox', "You must load Issue Payment first before viewing detail");

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
                CloseErrorPanel();
                VGDataShow();
                return true;
            }
            // end of show error divErroList
        }

        function ShowPolicyDetail() {
            var reg = document.getElementById("<%=ddlSpType.ClientID %>");
            if (reg != null) {
                if (reg.selectedIndex == 0) {
                    var divErrorPanel = document.getElementById('divErrorPanel');
                    var errorList = document.getElementById('divErrorList');
                    errorList.innerHTML = "<ul><li>You must select SP Type first before viewing policy Detail!</li></ul>";
                    divErrorPanel.style.display = "block";
                    window.scroll(0, 0);
                    return false;
                } else {
                    CloseErrorPanel();
                    PolicyDetailPopupReturnTrue();
                }
            }
            else {
                return false;
            }
        }


        function DeleteIntPayment() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdIntTransNo = document.getElementById('<%=hdIntTransNo.ClientID %>');
            if (hdIntTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Interest Payment')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Interest Payment has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function ChequeAndDraft(obj) {
            var oDraftNo = document.getElementById('<%=txtPDDraftNo.ClientID %>');
            var oAccNo = document.getElementById('<%=txtPDAccountNo.ClientID %>');
            var oAccName = document.getElementById('<%=txtPDAccountName.ClientID %>');

            oDraftNo.value = "";
            oAccNo.value = "";
            oAccName.value = "";

            if (obj.value == "4" || obj.value == "5" || obj.value == "6") { // Account
                oDraftNo.disabled = true;
                oDraftNo.className = 'textInputDisabled';

                oAccNo.disabled = false;
                oAccNo.className = 'textInput';
            } else if (obj.value == "3") {  // Draft
                oDraftNo.disabled = false;
                oDraftNo.className = 'textInput';

                oAccNo.disabled = true;
                oAccNo.className = 'textInputDisabled';
            } else if (obj.value == "1" || obj.value == "2") {  // all r close
                oDraftNo.disabled = true;
                oDraftNo.className = 'textInputDisabled';

                oAccNo.disabled = true;
                oAccNo.className = 'textInputDisabled';
            }
        }


        function openIssueSearchPopup(objName) {
            var oTxtRegNo = document.getElementById(objName);
            if (oTxtRegNo != null) {
                if (oTxtRegNo.value != null && oTxtRegNo.value != "") {
                    return true;
                }
                else {
                    IssueSearchPopupReturnTrue();
                    CloseErrorPanel();
                    return false;
                }
            } else {
                IssueSearchPopupReturnTrue();
                CloseErrorPanel();
                return false;
            }
        }

        function ViewJournalValidation() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdIntTransNo = document.getElementById('<%=hdIntTransNo.ClientID %>');
            if (hdIntTransNo.value != "") {
                return true;
            }
            else {
                // show error divErrorList                
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Interest Payment has not been selected/loaded</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }

        function ConvertAmount(obj) {
        
            var convertedAmount = 0
            var paymentAmount = $("#<%=txtPDPaymentAmount.ClientID %>").val()
            paymentAmount = paymentAmount.toString().replace(/,/g, '');            
            var currencyCode = $("#<%=ddlPDCurrency.ClientID %> option:selected").val()

            if (currencyCode == "00")//BDT
            {
                convertedAmount = parseFloat(paymentAmount) * parseFloat(obj.value)
            }
            else if (currencyCode == "01" || currencyCode == "16" //USD || EURO || GBP 
            || currencyCode == "80"){                
                convertedAmount = parseFloat(paymentAmount) / parseFloat(obj.value)
            }
            else {
                convertedAmount = 0
            }
            convertedAmount = convertedAmount.toFixed(2)
            $("#<%=txtPDConvertedAmount.ClientID %>").val(AddingCommas(convertedAmount))
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
        <legend runat="server" id="lgText">Unapproved Interest Payment List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdIntTransNo" />
                <asp:HiddenField runat="server" ID="hdRegNo" />
                <asp:HiddenField runat="server" ID="hdDataType" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                OnRowDataBound="gvData_RowDataBound" ID="gvData" runat="server" SkinID="SBMLGridGreen"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                OnClientClick="CloseErrorPanel()" Text="Select" />
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
        <legend>Interest Payment Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            Int Payment Trans No.
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIntPayTransNo" Width="150px" CssClass="textInput" runat="server"
                                    ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnIntPayTransSearch"
                                    OnClientClick="return SearchIPEPopupReturnTrue(),CloseErrorPanel()" />
                            </div>
                        </td>
                        <td>
                            Payment Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtPaymentDate" Width="100px" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:CheckBox Text="Mark as Premature Encashment" runat="server" ID="chkMarkAsPremature" />
                            &nbsp;
                            <asp:CheckBox Text="Individual Yes/No" runat="server" ID="chkIndiYesNo" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Issue Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <table width="100%" align="center" class="tableBody" border="0" cellpadding="1" cellspacing="1">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlCommon" Width="120px" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td align="left">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                    ReadOnly="true"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">
                            Registration No
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegistrationNo" Width="160px" CssClass="textInput" runat="server"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    MaxLength="25" AutoPostBack="True" OnTextChanged="txtRegistrationNo_TextChanged"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" OnClick="btnRegSearch_Click" runat="server" Text="Search"
                                    ID="btnRegSearch" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo')" /></div>
                        </td>
                        <td align="right" valign="top">
                            Issue Name
                        </td>
                        <td rowspan="2" valign="top">
                            <asp:TextBox ID="txtIssueName" Width="200px" CssClass="textInput" runat="server"
                                TextMode="MultiLine" Height="35px" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right" valign="top">
                            Total Amount
                            <td valign="top">
                                <asp:TextBox ID="txtTotalAmount" Width="100px" CssClass="textInput" runat="server"
                                    ReadOnly="true"></asp:TextBox>
                            </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="3">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Show Details" ID="btnShowDetails"
                                OnClick="btnShowDetails_Click" OnClientClick="return ShowVGDataShow()" />
                            &nbsp;
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Show Policy" ID="btnShowPolicy"
                                OnClientClick="return ShowPolicyDetail()" OnClick="btnShowPolicy_Click" />
                            &nbsp;
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Stop Pmt" ID="btnStopPmt" OnClick="btnStopPmt_Click"
                                OnClientClick="return ShowVGDataShow()" />
                            &nbsp;
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="&nbsp;Lien Status&nbsp;" ID="btnLienStatus"
                                OnClick="btnLienStatus_Click" OnClientClick="return ShowVGDataShow()" />
                        </td>
                        <td align="right">
                            Master ID
                            <td>
                                <asp:TextBox ID="txtMasterID" Width="100px" CssClass="textInput" runat="server" ReadOnly="true"></asp:TextBox>
                            </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
        <br />
    <!-- start to 2Tab panel  -->
    <fieldset>
        <legend>Customer(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                <ContentTemplate>
                    <div style="height: 100%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvCustomerDetail" runat="server"
                            AutoGenerateColumns="true" SkinID="SBMLGridGreen">
                            <Columns>
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
        <legend>Nominee(s) Details</legend>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div style="height: 100%; width: 100%; overflow: auto;">
                        <asp:GridView Style="width: 98%; height: 100%" ID="gvNomDetail" runat="server" AutoGenerateColumns="true"
                            SkinID="SBMLGridGreen">
                            <Columns>
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
        </fieldset>
        <%-- End of Nominee(s) Details Tab  --%>
    <br />
    <fieldset>
        <legend>Coupon \ Installments Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="3"
                    border="0">
                    <tr>
                        <td align="right">
                            From Scrip
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtFromScrip" CssClass="textInput" Width="120px" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="7" runat="server"></asp:TextBox>
                            </div>
                        </td>
                        <td align="right">
                            To Scrip
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtToScrip" CssClass="textInput" Width="120px" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="7" runat="server"></asp:TextBox>
                            </div>
                        </td>
                        <td align="right">
                            From Coupon
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtFromCoupon" CssClass="textInput" Width="50px" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="3" runat="server"></asp:TextBox>
                            </div>
                        </td>
                        <td align="right">
                            To Coupon
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtToCoupon" CssClass="textInput" Width="50px" onblur="blurActiveInputWithObj(this)"
                                    onfocus="highlightActiveInputWithObj(this)" MaxLength="3" runat="server"></asp:TextBox>
                            </div>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Scrips
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlScrips" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Coupon / Installment No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCouponInstalNo" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Select" ID="btnSelect" OnClick="btnSelect_Click" />
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Deselect" ID="btnDeSelect"
                                OnClick="btnDeSelect_Click" />
                        </td>
                        <td>
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Calculate" ID="btnCalculate"
                                OnClick="btnCalculate_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="8">
                            <table width="100%" cellspacing="0" cellpadding="0" style="color: Black; background-color: #E9F0D5;
                                border-color: #C1D586; border-width: 1px; border-style: Solid; width: 98%; border-collapse: collapse;">
                                <tr style="color: Black; background-color: #D1D3CF; font-size: 12px; font-weight: normal;
                                    white-space: nowrap;">
                                    <th style="width: 50px" scope="col">
                                        Paid
                                    </th>
                                    <th style="width: 50px" scope="col">
                                        Lien
                                    </th>
                                    <th style="width: 50px" scope="col">
                                        Select
                                    </th>
                                    <th style="width: 90px" scope="col">
                                        Stop<br />
                                        Payment
                                    </th>
                                    <th style="width: 110px" scope="col">
                                        Certificate<br />
                                        No.</span>
                                    </th>
                                    <th style="width: 90px" scope="col">
                                        Denom<br />
                                        <span style="text-transform: lowercase;">ination</span>
                                    </th>
                                    <th style="width: 75px" scope="col">
                                        Insta<br />
                                        <span style="text-transform: lowercase;">lment</span><br />
                                        No.
                                    </th>
                                    <th style="width: 110px" scope="col">
                                        Maturity<br />
                                        Date
                                    </th>
                                    <th style="width: 110px" scope="col">
                                        Interest<br />
                                        Amount
                                    </th>
                                    <th style="width: 90px" scope="col">
                                        Income<br />
                                        Tax
                                    </th>
                                    <th style="width: 75px" scope="col">
                                        Cal.<br />
                                        Amount
                                    </th>
                                </tr>
                            </table>
                            <div style="height: 180px; width: 100%; overflow: auto;">
                                <asp:GridView ID="gvPaymentDetails" runat="server" Width="98%" BorderWidth="1px"
                                    AutoGenerateColumns="false" SkinID="SBMLGridV2" ShowHeader="false" 
                                    PageSize="200">
                                    <%--SBMLGridV2--%>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Paid">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("Paid") %>' ID="chkPaid" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Lien">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("Lien") %>' ID="chkLien" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Selected">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" Enabled='<%# Eval("FlgPaid") %>' Checked='<%# Eval("Selected") %>'
                                                    ID="chkSelected" />
                                                <asp:HiddenField ID="hdnSPScripID" Value='<%# Eval("SPScripID") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Stop Payment">
                                            <ItemStyle HorizontalAlign="Left" Width="90px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" Checked='<%# Eval("StopPayment") %>' ID="chkStopPayment"
                                                    Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CertificatNo" ItemStyle-HorizontalAlign="Left" 
                                            ItemStyle-Width="110px" >
                                            <ItemStyle HorizontalAlign="Left" Width="110px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Denomination" ItemStyle-HorizontalAlign="Left" 
                                            ItemStyle-Width="90px" >
                                            <ItemStyle HorizontalAlign="Left" Width="90px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CouponNo" ItemStyle-HorizontalAlign="Left" 
                                            ItemStyle-Width="75px" >
                                            <ItemStyle HorizontalAlign="Left" Width="75px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="MaturityDate" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="110px"
                                            DataFormatString="{0: dd-MMM-yyyy}" HeaderText="Maturity Date" >
                                            <ItemStyle HorizontalAlign="Left" Width="110px" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Interest Amount">
                                            <ItemStyle HorizontalAlign="Left" Width="110px" />
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" Text='<%# Eval("InterestAmount")%>' onblur="blurActiveInputWithObj(this)"
                                                    Enabled="false" Width="80px" CssClass="textInput" onfocus="highlightActiveInputWithObj(this)"
                                                    ID="txtInterestAmount" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Income Tax">
                                            <ItemStyle HorizontalAlign="Left" Width="90px" />
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("IncomeTax")%>' ID="lblIncomeTax" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PaymentAmount" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="75px"
                                            HeaderText="Payment Amount" >
                                            <ItemStyle HorizontalAlign="Left" Width="75px" />
                                        </asp:BoundField>
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
    </fieldset>
    <br />
    <fieldset>
        <legend>Payment Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" border="0">
                    <tr>
                        <td colspan="6" align="right">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="TAX Adjustment 5% or 10% Vise versa" ID="btnTaxAdjustment" OnClick="btnTaxAdjustment_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Coupon/Installments Selected
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDCouponInsSelected" Width="150px" CssClass="textInputDisabled"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Calc. Interest
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDCalcInterest" Width="150px" CssClass="textInputDisabled" runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Interest Payable
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDIntPayable" Width="150px" CssClass="textInputDisabled" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Social Security Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDSocialSecurityAmount" Width="150px" CssClass="textInputDisabled"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Income Tax
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDIncomeTax" Width="150px" CssClass="textInputDisabled" 
                                runat="server" AutoPostBack="True" ontextchanged="txtPDIncomeTax_TextChanged"></asp:TextBox>
                        </td>
                        <td align="right">
                            Payment Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDPaymentAmount" Width="150px" CssClass="textInputDisabled" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Payment Mode
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlPDPaymentMode" SkinID="ddlSmall" Width="150px" runat="server"
                                    onChange="ChequeAndDraft(this)">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Currency
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlPDCurrency" Width="150px" SkinID="ddlSmall" OnSelectedIndexChanged="ddlPDCurrency_SelectedIndexChanged"
                                    AutoPostBack="true" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Conv. Rate
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDConvRate" MaxLength="8" Width="150px" CssClass="textInputDisabled" onChange="ConvertAmount(this)"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Account No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPDAccountNo" Width="150px" CssClass="textInputDisabled" runat="server" MaxLength="12"
                                    onblur="blurActiveInputWithObj(this)" OnTextChanged="txtPDAccountNo_TextChanged"
                                    AutoPostBack="true" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Draft No
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDDraftNo" Width="150px" CssClass="textInputDisabled" runat="server"
                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td align="right">
                            Payment Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDConvertedAmount" Width="150px" CssClass="textInputDisabled"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Account Name
                        </td>
                        <td colspan="3">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPDAccountName" Width="400px" CssClass="textInputDisabledForTxt" 
                                    runat="server"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Interest Rate
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDInterestRate" Width="150px" CssClass="textInputDisabled" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
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
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Peview Advice" ID="btnViewJournals"
                        OnClientClick="return ViewJournalValidation()" OnClick="btnViewJournals_Click"  Visible="true" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click" OnClientClick=" MsgPopupReturnTrue('Approve')"/>
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteIntPayment()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc4:VGData ID="VGD" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" type="8" />
    <uc5:SearchIPE ID="SearchIPE" runat="server" type="19" label1="Payment Trans No"
        pagetitle="Interest Payment Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>

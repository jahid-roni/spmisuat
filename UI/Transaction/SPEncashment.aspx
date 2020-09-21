<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="SPEncashment.aspx.cs" Inherits="SBM_WebUI.mp.SPEncashment" %>

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
                    return true;
                }
            } else {
                return false;
            }
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

        function SaveValidation() {
            var sErrorList = "";

            var TargetBaseControl = document.getElementById('<%= this.gvEncashmentDetails.ClientID %>');
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
                sErrorList += "<li>Please select at least one scrip to encash.</li>";
            }
            sErrorList += RequiredData('<%=ddlPDPaymentMode.ClientID %>', 'DropDownList', "Payment Mode  cannot be empty!");
            sErrorList += RequiredData('<%=ddlPDCurrency.ClientID %>', 'DropDownList', "Currency  cannot be empty!");
            sErrorList += RequiredData('<%=txtPaymentDate.ClientID %>', 'TextBox', "Payment Date  cannot be empty!");
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

        function DeleteEnchPayment() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var hdEncashTransNo = document.getElementById('<%=hdEncashTransNo.ClientID %>');
            if (hdEncashTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Encashment')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Encashment has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                window.scroll(0, 0);
                return false;
                // end of show error divErroList
            }
        }


        function CheckTextBox() {
            var TargetBaseControl = document.getElementById('<%= this.gvEncashmentDetails.ClientID %>');
            var TargetChildControl = "chkSelected";

            var dCalInt = 0;    // A
            var dPaidInt = 0;   // B
            var dPrincAmnt = 0; // P 
            var dLevi = 0;
            var dIncomeTax = 0;

            var Inputs = TargetBaseControl.getElementsByTagName("input");
            for (var n = 0; n < Inputs.length; ++n) {
                if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf(TargetChildControl, 0) >= 0) {

                    var chkBoxID = Inputs[n].id;
                    var iIndex = chkBoxID.lastIndexOf("_")
                    var oText = document.getElementById(chkBoxID.substr(0, iIndex) + '_txtTotalInterest');
                    var oDenomination = document.getElementById(chkBoxID.substr(0, iIndex) + '_lblDenomination');
                    var oPaidInterest = document.getElementById(chkBoxID.substr(0, iIndex) + '_lblPaidInterest');

                    var oIncomeTax = document.getElementById(chkBoxID.substr(0, iIndex) + '_lblIncomeTax');

                    if (oText != null) {
                        if (Inputs[n].checked == true) {
                            //oText.disabled = false;

                            var temp = oText.value.toString().replace(/,/g, '');
                            dCalInt = parseFloat(dCalInt) + parseFloat(temp);
                            dCalInt = dCalInt.toFixed(2);

                            var tempPaid = oPaidInterest.innerHTML.toString().replace(/,/g, '');
                            dPaidInt = parseFloat(dPaidInt) + parseFloat(tempPaid);
                            dPaidInt = dPaidInt.toFixed(2);

                            var tempDenomination = oDenomination.innerHTML.toString().replace(/,/g, '');
                            dPrincAmnt = parseFloat(dPrincAmnt) + parseFloat(tempDenomination);
                            dPrincAmnt = dPrincAmnt.toFixed(2);


                            var tempIncomeTax = oIncomeTax.innerHTML.toString().replace(/,/g, '');
                            dIncomeTax = parseFloat(dIncomeTax) + parseFloat(tempIncomeTax);
                            dIncomeTax = dIncomeTax.toFixed(2);

                        } else {
                            oText.disabled = true;
                        }
                    }
                }
            }

            // #region Assign Data in calculation field set
            var txtPDCalcInterest = document.getElementById('<%= txtPDCalcInterest.ClientID %>');
            txtPDCalcInterest.value = AddingCommas(dCalInt)

            var txtPDPaidInterest = document.getElementById('<%= txtPDPaidInterest.ClientID %>');
            txtPDPaidInterest.value = AddingCommas(dPaidInt);

            var tmpIntAmount = (parseFloat(dCalInt) - parseFloat(dPaidInt));
            tmpIntAmount = tmpIntAmount.toFixed(2);
            var txtPDInterestAmount = document.getElementById('<%= txtPDInterestAmount.ClientID %>');
            txtPDInterestAmount.value = AddingCommas(tmpIntAmount);

            var txtPDLevi = document.getElementById('<%= txtPDLevi.ClientID %>');
            txtPDLevi.value = AddingCommas(dLevi);

            var txtPDIncomeTax = document.getElementById('<%= txtPDIncomeTax.ClientID %>');
            txtPDIncomeTax.value = AddingCommas(dIncomeTax);

            var txtPDPrincipleAmount = document.getElementById('<%= txtPDPrincipleAmount.ClientID %>');
            txtPDPrincipleAmount.value = AddingCommas(dPrincAmnt);

            var tmpIntPay = (parseFloat(dCalInt) - parseFloat(dPaidInt) - parseFloat(dLevi) - parseFloat(dIncomeTax));
            var txtPDInterestPayable = document.getElementById('<%= txtPDInterestPayable.ClientID %>');
            tmpIntPay = tmpIntPay.toFixed(2);
            txtPDInterestPayable.value = AddingCommas(tmpIntPay);

            var txtPDConvertedAmount = document.getElementById('<%= txtPDConvertedAmount.ClientID %>');
            var txtPDConvRate = document.getElementById('<%= txtPDConvRate.ClientID %>');
            var tmpConAmnt = parseFloat(txtPDConvRate.value) * parseFloat(parseFloat(dPrincAmnt) + parseFloat(tmpIntPay));
            tmpConAmnt = tmpConAmnt.toFixed(2);
            txtPDConvertedAmount.value = AddingCommas(tmpConAmnt);
        }


        function ChequeAndDraft(obj) {
            var oDraftNo = document.getElementById('<%=txtPDDraftNo.ClientID %>');
            var oAccNo = document.getElementById('<%=txtPDAccountNo.ClientID %>');
            var oAccName = document.getElementById('<%=txtPDAccountName.ClientID %>');

            oDraftNo.value = "";
            oAccNo.value = "";
            oAccName.value = "";

            if (obj.value == "4" || obj.value == "5" || obj.value == "6") { // Account No will be active
                oDraftNo.disabled = true;
                oDraftNo.className = 'textInputDisabled';

                oAccNo.disabled = false;
                oAccNo.className = 'textInput';
            } else if (obj.value == "3") {  // Draft will be active
                oDraftNo.disabled = false;
                oDraftNo.className = 'textInput';

                oAccNo.disabled = true;
                oAccNo.className = 'textInputDisabled';
            } else if (obj.value == "1" || obj.value == "2") {  // both are disable
                oDraftNo.disabled = true;
                oDraftNo.className = 'textInputDisabled';

                oAccNo.disabled = true;
                oAccNo.className = 'textInputDisabled';
            }
        }

        function ConvertAmount(obj) {
            var convertedAmount = 0

            var principalAmount = $("#<%=txtPDPrincipleAmount.ClientID %>").val()
            principalAmount = principalAmount.toString().replace(/,/g, '');

            var intPayableAmnt = $("#<%=txtPDInterestPayable.ClientID %>").val()
            intPayableAmnt = intPayableAmnt.toString().replace(/,/g, '');

            var currencyCode = $("#<%=ddlPDCurrency.ClientID %> option:selected").val()
            //ddlSpType
            var sptype = $("#<%=ddlSpType.ClientID %> option:selected").val()

            if (currencyCode == "00")//BDT
            {
                convertedAmount = (parseFloat(principalAmount) + parseFloat(intPayableAmnt)) * parseFloat(obj.value)
            }
            else if (currencyCode == "01" || currencyCode == "16" //USD || EURO || GBP 
            || currencyCode == "80") {
                convertedAmount = (parseFloat(principalAmount) + parseFloat(intPayableAmnt)) / parseFloat(obj.value)
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
        <legend runat="server" id="lgText">Unapproved Encashment List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdEncashTransNo" />
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
        <legend>Encashment Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="95%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            Trans No.
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtEncashTransNo" Width="150px" CssClass="textInputDisabled" runat="server"
                                    ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Search" ID="btnEncashTransSearch"
                                    OnClientClick="return SearchIPEPopupReturnTrue(),  CloseErrorPanel()" />
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
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdIssueTransNo" />
                <table width="100%" align="center" class="tableBody" border="0" cellpadding="1" cellspacing="1">
                    <tr>
                        <td align="right">
                            SP Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Branch Code
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBranch" SkinID="ddlCommon" Width="120px" runat="server"
                                Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            Issue Date
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInputDisabled" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Registration No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegistrationNo" Width="160px" CssClass="textInput" runat="server" OnTextChanged="txtRegistrationNo_TextChanged"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" AutoPostBack="true"
                                    MaxLength="25"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                <asp:Button CssClass="ButtonAsh" runat="server" OnClick="btnRegSearch_Click" Text="Search"
                                    ID="btnRegSearch" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo')" />
                            </div>
                        </td>
                        <td align="right">
                            Issue Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtIssueName" Width="250px" CssClass="textInputDisabledForTxt" runat="server"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td align="right">
                            Total Amount
                            <td>
                                <asp:TextBox ID="txtTotalAmount" Width="100px" CssClass="textInputDisabled" runat="server"
                                    ReadOnly="true"></asp:TextBox>
                            </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="4">
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
                                <asp:TextBox ID="txtMasterID" Width="100px" CssClass="textInputDisabledForTxt" ReadOnly="true"
                                    runat="server"></asp:TextBox>
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
            </asp:UpdatePanel>
        </fieldset>
        <%-- End of Nominee(s) Details Tab  --%>
    <br />
    <fieldset>
        <legend>Coupon \ Installments Details</legend>
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" cellpadding="0" cellspacing="0"
                    border="0">
                    <tr>
                        <td align="left" colspan="2">
                            <table width="99%" cellspacing="0" cellpadding="0" style="color: Black; background-color: #E9F0D5;
                                border-color: #C1D586; border-width: 1px; border-style: Solid; border-collapse: collapse;">
                                <tr style="color: Black; background-color: #D1D3CF; font-size: 12px; font-weight: normal;">
                                    <th style="width: 50px;" scope="col">
                                        Enca<br />
                                        <span style="text-transform: lowercase;">shed</span>
                                    </th>
                                    <th style="width: 50px" scope="col">
                                        Stop<br />
                                        <span style="text-transform: lowercase;">ped</span>
                                    </th>
                                    <th style="width: 50px" scope="col">
                                        Liend
                                    </th>
                                    <th style="width: 50px" scope="col">
                                        Select
                                    </th>
                                    <th style="width: 130px" scope="col">
                                        Certificate<br />
                                        No.
                                    </th>
                                    <th style="width: 90px" scope="col">
                                        Denom<br />
                                        <span style="text-transform: lowercase;">ination</span>
                                    </th>
                                    <th style="width: 110px" scope="col">
                                        Total<br />
                                        Interest
                                    </th>
                                    <th style="width: 75px" scope="col">
                                        Paid<br />
                                        Interest
                                    </th>
                                    <th style="width: 95px" scope="col">
                                        Interest<br />
                                        Payable
                                    </th>
                                    <th style="width: 70px" scope="col">
                                        Income<br />
                                        Tax
                                    </th>
                                    <th style="width: 95px" scope="col">
                                        Payment<br />
                                        Amount
                                    </th>
                                    <th id="thInstallNo" style="width: 65px" scope="col" runat="server">
                                        Last<br />
                                        Instal. No.
                                    </th>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <div style="height: 180px; width: 100%; overflow: auto;">
                                <asp:GridView Style="width: 99%; border: 1px" ID="gvEncashmentDetails" runat="server"
                                    AutoGenerateColumns="false" SkinID="SBMLGridV2" ShowHeader="false">
                                    <%--SBMLGridGreen--%>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Encashed">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkEncashed" Enabled="false" Checked='<%# Eval("Encashed") %>' />
                                                <asp:HiddenField ID="hdnSPScripID" Value='<%# Eval("SPScripID") %>' runat="server" />
                                                <asp:HiddenField ID="hdnAlreadyEnchsdCoupons" Value='<%# Eval("AlreadyEncashedCoupons") %>'
                                                    runat="server" />
                                                <asp:HiddenField ID="hdnCouponsToBeEnchsd" Value='<%# Eval("CouponsToBeEncashed") %>'
                                                    runat="server" />
                                                <asp:HiddenField ID="hdnPaidIncomeTax" Value='<%# Eval("AlreadyPaidIncomeTax") %>'
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Stop Payment">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" Checked='<%# Eval("StopPayment") %>' ID="chkStopPayment"
                                                    Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Liend">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkLien" Enabled="false" Checked='<%# Eval("Lien") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Select">
                                            <ItemStyle HorizontalAlign="Left" Width="50px" />
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" Enabled='<%# Eval("FlgEncashed") %>' ID="chkSelected"
                                                    Checked='<%# Eval("Selected") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CertificatNo" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="130px"
                                            HeaderText="Certificate No" />
                                        <asp:TemplateField HeaderText="Denomination">
                                            <ItemStyle HorizontalAlign="Left" Width="90px" />
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("Denomination")%>' ID="lblDenomination" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total Interest">
                                            <ItemStyle HorizontalAlign="Left" Width="110px" />
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" Text='<%# Eval("TotalInterest")%>' onblur="blurActiveInputWithObj(this)"
                                                    Enabled="false" Width="90px" CssClass="textInput" onfocus="highlightActiveInputWithObj(this)"
                                                    ID="txtTotalInterest" />
                                            </ItemTemplate>
                                            <%--< % # Eval ("Selected") % >--%>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Paid Interest">
                                            <ItemStyle HorizontalAlign="Left" Width="75px" />
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("PaidInterest")%>' ID="lblPaidInterest" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="InterestToPay" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="95px"
                                            HeaderText="Interest To Pay" />
                                        <asp:TemplateField HeaderText="Income Tax">
                                            <ItemStyle HorizontalAlign="Left" Width="75px" />
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("IncomeTax")%>' ID="lblIncomeTax" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PaymentAmount" ItemStyle-Width="95px" HeaderText="Payment Amount" />
                                        <asp:BoundField DataField="InstallmentNumber" ItemStyle-Width="60px" HeaderText="Installment Number" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No record found
                                    </EmptyDataTemplate>
                                    <AlternatingRowStyle CssClass="odd" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Select All" ID="btnSelect"
                                OnClick="btnSelect_Click" />
                            &nbsp;
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Deselect All" ID="btnDeSelect"
                                OnClick="btnDeSelect_Click" />
                        </td>
                        <td align="right">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="Calculate" ID="btnCalculate"
                                OnClick="btnCalculate_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Payment Details</legend>
        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td colspan="6" align="right">
                            <asp:Button CssClass="ButtonAsh" runat="server" Text="TAX Adjustment 5% or 10% Vise versa" ID="btnTaxAdjustment" OnClick="btnTaxAdjustment_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="3">
                            Maturity Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDMaturityDate" Width="150px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Principle Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDPrincipleAmount" Width="150px" CssClass="textInputDisabled"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Calc. Interest
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDCalcInterest" Width="150px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Paid Interest
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDPaidInterest" Width="150px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Interest Amount
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDInterestAmount" Width="150px" CssClass="textInputDisabled"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Levi
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDLevi" Width="150px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
                        </td>
                        <td align="right">
                            Income Tax
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDIncomeTax" Width="150px" CssClass="textInputDisabled" runat="server"
                                AutoPostBack="True" OnTextChanged="txtPDIncomeTax_TextChanged"></asp:TextBox>
                        </td>
                        <td align="right">
                            Interest Payable
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDInterestPayable" Width="150px" CssClass="textInputDisabled"
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Payment Mode
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlPDPaymentMode" SkinID="ddlCommon" Width="155px" runat="server"
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
                                <asp:DropDownList ID="ddlPDCurrency" Width="155px" SkinID="ddlCommon" runat="server"
                                    OnSelectedIndexChanged="ddlPDCurrency_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Conv. Rate
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDConvRate" Width="150px" CssClass="textInputDisabled" onChange="ConvertAmount(this)"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Account No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPDAccountNo" Width="150px" CssClass="textInputDisabled" runat="server"
                                    MaxLength="12" onblur="blurActiveInputWithObj(this)" OnTextChanged="txtPDAccountNo_TextChanged"
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
                                ReadOnly="true" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Account Name
                        </td>
                        <td colspan="3">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtPDAccountName" Width="466px" CssClass="textInputDisabledForTxt"
                                    ReadOnly="true" runat="server"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *</div>
                        </td>
                        <td align="right">
                            Interest Rate
                        </td>
                        <td>
                            <asp:TextBox ID="txtPDInterestRate" Width="150px" CssClass="textInputDisabled" ReadOnly="true"
                                runat="server"></asp:TextBox>
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
                    <asp:Button CssClass="ButtonAsh" runat="server"
                        Text="Preview Advice" ID="btnViewJournals" Visible="true" OnClick="btnViewJournals_Click" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClick="btnReject_Click"
                        OnClientClick="return RejectValidation()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteEnchPayment()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
    </fieldset>
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc4:VGData ID="VGD" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" Type="11" />
    <uc5:SearchIPE ID="SearchIPE" runat="server" Type="20" Label1="Encashment Trans No"
        PageTitle="Encashment Search" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="OnlineSPIssue.aspx.cs" Inherits="SBM_WebUI.mp.OnlineSPIssue" %>

<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCCustomerDetail_HSB.ascx" TagName="CustomerDetails" TagPrefix="uc2" %>
<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function () { $(this).mask("99/99/9999"); });
            $('input[id*=txtAppliedAmount]').keypress(function (e) { return intNumber(e); });
            $('input[id*=txtNDShare]').keypress(function (e) { return intNumber(e); });
            $('input[id*=txtNDAmount]').keypress(function (e) { return intNumber(e); });
            $('input[id*=txtTotalAmount]').keypress(function (e) { return floatNumber(e); });
            $('input[id*=txtCDSeriesFrom]').keypress(function (e) { return intNumber(e); });
            $('input[id*=txtCDSeriesTo]').keypress(function (e) { return intNumber(e); });
            $('input[id*=txtMasterNo]').keypress(function (e) { return intNumber(e); });
            $('input[id*=txtCDQuantity]').keypress(function (e) { return intNumber(e); });
        }



        function SaveNomineeValidation() {
            var sErrorList = "";

            sErrorList += RequiredData('<%=txtAppliedAmount.ClientID %>', 'TextBox', "Total Amount cannot be empty!");
            sErrorList += RequiredData('<%=txtNDName.ClientID %>', 'TextBox', "Nominee Name cannot be empty!");
            sErrorList += RequiredData('<%=txtNDShare.ClientID %>', 'TextBox', "Share cannot be empty!");
            sErrorList += RequiredData('<%=txtNDRelation.ClientID %>', 'TextBox', "Relation cannot be empty!");
            sErrorList += RequiredData('<%=txtNationalID.ClientID %>', 'TextBox', "NID cannot be empty!");
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

        function IssueDateCheckData() {
            var sErrorList = "";
            var txtIssueDate = $("#<%=txtIssueDate.ClientID %>").val();;
            if (txtIssueDate == "") {
                sErrorList += "<li>Issue Date cannot be empty!</li>";
            }
            return true;
        }




        function addPadding(txtObj) {
            if (txtObj.value == "") {
                txtObj.value = "";
            } else {
                txtObj.value = leftPad(txtObj.value, 7);
            }
            return false;
        }

        function leftPad(n, len) {
            return (new Array(len - String(n).length + 1)).join("0").concat(n);
        }


        function RejectValidation() {
            var sErrorList = "";
            sErrorList += RequiredData('ctl00_cphDet_ucUserDet_txtCheckerComments', 'TextBox', "Checker Comments cannot be empty!");
            return OpenErrorPanel(sErrorList, 'Reject');
        }


        function DeleteOldCustomerIssue() {
            var divErrorPanel = document.getElementById('divErrorPanel');

            var hdTransNo = document.getElementById('<%=hdTransNo.ClientID %>');
            if (hdTransNo.value != "") {
                divErrorPanel.style.display = "none";
                if (CheckForDelete('this Old Customer Issue')) {
                    MsgPopupReturnTrue('Delete');
                    return true;
                } else {
                    return false;
                }
            }
            else {
                // show error divErrorList
                var errorList = document.getElementById('divErrorList');
                errorList.innerHTML = "<ul><li>Old Customer Issue has not selected yet</li></ul>";
                divErrorPanel.style.display = "block";
                return false;
                // end of show error divErroList
            }
        }
        function CheckJointCustomer() {
            var sErrorList = "";

            var vAppliedAmount = $("#<%=txtAppliedAmount.ClientID %>").val();
            vAppliedAmount = vAppliedAmount.toString().replace(/,/g, '');
            if (vAppliedAmount == "") {
                vAppliedAmount = 0;
            }
            if (parseInt(vAppliedAmount, 10) <= 0) {
                sErrorList += "<li>Please input the applied amount, before proceed to customer adding.</li>";
                return OpenErrorPanel(sErrorList, 'Save');
            }
            var rowsGvCustmerDetail = $("#<%=gvCustomerDetail.ClientID %> tr").length;
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

            var gvCustomerDetail = $("#<%=gvCustomerDetail.ClientID %> tr").length;

            var vAppliedAmount = $("#<%=txtAppliedAmount.ClientID %>").val().replace(/,/g, '');
            var vTotalAmount = $("#<%=txtTotalAmount.ClientID %>").val().replace(/,/g, '');

            if (parseInt(vAppliedAmount, 10) < parseInt(vTotalAmount, 10)) {
                sErrorList += "<li>You cannot exceed total amount</li>";
            }
            if (parseInt(vAppliedAmount, 10) != parseInt(vTotalAmount, 10)) {
                sErrorList += "<li>Total Amount & Total Payment Amount must be equal</li>";
            }
            sErrorList += RequiredData('<%=ddlSpType.ClientID %>', 'DropDownList', "SP Type cannot be empty!");
            sErrorList += RequiredData('<%=ddlBranch.ClientID %>', 'DropDownList', "Branch Code cannot be empty!");
            sErrorList += RequiredData('<%=ddlCustomerType.ClientID %>', 'DropDownList', "Customer Type cannot be empty!");
            sErrorList += RequiredData('<%=txtAppliedAmount.ClientID %>', 'TextBox', "Total Amount cannot be empty!");
            sErrorList += RequiredData('<%=txtIssueDate.ClientID %>', 'TextBox', "Issue Date cannot be empty!");
            sErrorList += RequiredData('<%=txtRegistrationNo.ClientID %>', 'TextBox', "Registration No cannot be empty!");

            if (gvCustomerDetail == 1 || gvCustomerDetail == 0) {
                sErrorList += "<li>Customer Detail list cannot be empty</li>";
            }

            var oSPType = document.getElementById("ctl00_cphDet_ddlSpType");
            if (oSPType != null) {
                if (oSPType.value == "WDB") {
                    sErrorList += RequiredData('<%=txtBHDHolderName.ClientID %>', 'TextBox', "Holder Name cannot be empty!");
                }
            }

            sErrorList += RequiredData('<%=txtIssueName.ClientID %>', 'TextBox', "Issue Name cannot be empty!");
            sErrorList += RequiredData('<%=txtTotalAmount.ClientID %>', 'TextBox', "Total Amount cannot be empty!");

            return OpenErrorPanel(sErrorList, 'Save');
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

        function CheckAndCustomerDetailPopup() {
            var divErrorPanel = document.getElementById('divErrorPanel');
            var cType = document.getElementById("<%=ddlCustomerType.ClientID %>");
            if (cType != null) {
                if (cType.selectedIndex == 0 || cType.selectedIndex == -1) {
                    // show error divErrorList
                    divErrorPanel.style.display = "block";
                    var errorList = document.getElementById('divErrorList');
                    errorList.innerHTML = "<ul><li>Customer Type has not been selected yet</li></ul>";
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


        function CalculateAmount(vType) {
            var oAmount = document.getElementById('<%=txtAppliedAmount.ClientID %>');
            var oNDShare = document.getElementById('<%=txtNDShare.ClientID %>');
            var oNDAmount = document.getElementById('<%=txtNDAmount.ClientID %>');

            var vTotalAmount = "";
            if (oAmount.value != "") {
                vTotalAmount = oAmount.value.replace(/,/g, '');
                CloseErrorPanel();
            }
            else {
                OpenErrorPanel("<li>Total Amount cannot be empty!</li>", "");
                oNDShare.value = "";
                oNDAmount.value = "";
                return false;
            }
            vTotalAmount = parseInt(vTotalAmount, 10);


            // this is by total amount
            if (vType == "Amount") {
                if (oNDAmount.value != "") {
                    if (parseInt(oNDAmount.value, 10) > parseInt(oAmount.value, 10)) {
                        OpenErrorPanel("<li>Total Nominee amount cannot be exceeded from Total Amount</li>", "");
                        oNDShare.value = "";
                        return false;
                    }
                    var vNDAmount = parseInt(oNDAmount.value, 10);
                    var vFinalVal = (parseFloat(vNDAmount) * 100) / parseFloat(vTotalAmount);
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
                    if (parseInt(oNDShare.value, 10) > 100) {
                        OpenErrorPanel("<li>Total amount of share cannot be exceeded 100!</li>", "");
                        oNDAmount.value = "";
                        return false;
                    }
                    var vShare = parseInt(oNDShare.value, 10);
                    var vFinalVal = (parseFloat(vShare) * parseFloat(vTotalAmount)) / 100;
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

        function checkIssueDate(txtObj) {
            if (txtObj.value == "__/__/____") {
                alert("Issue Date cannot be empty");
                return false;
            }
            else {
                return true;
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <%--End of User Comments--%>
    <div id="divErrorPanel" style="display: none">
        <fieldset class="errorFieldset">
            <legend class="errorLegend">Sorry, there were problems with your input</legend>
            <div class="errorBox" id="divErrorList">
            </div>
        </fieldset>
        <br />
    </div>
    <%--Start to User Comments--%>
    <fieldset runat="server" id="fsList">
        <legend runat="server" id="lgText">Unapproved Issue List</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel7">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="hdTransNo" />
                <asp:HiddenField runat="server" ID="hdDenom" />
                <asp:HiddenField runat="server" ID="hdSupportdGndr" />
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>
                            <asp:GridView OnRowCommand="gvData_RowCommand" AutoGenerateColumns="true" Width="98%"
                                ID="gvData" runat="server" SkinID="SBMLGridGreen" OnRowDataBound="gvData_RowDataBound"
                                AllowPaging="True" OnPageIndexChanging="gvData_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnSelect"
                                                Text="Select" />
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
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right">SP Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlSpType" SkinID="ddlMedium" runat="server" OnSelectedIndexChanged="ddlSpType_SelectedIndexChanged" AutoPostBack="true">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                        </td>
                        <td align="right">Branch Code
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlBranch" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                        </td>
                        <td align="right">Issue Date
                        </td>
                        <td align="left">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueDate" Width="100px" CssClass="textInput" runat="server"
                                    AccessKey="A" onblur="checkIssueDate(this),blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    AutoPostBack="True" OnTextChanged="txtIssueDate_TextChanged"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">Registration No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtRegistrationNo" Width="130px" CssClass="textInput" runat="server"
                                    MaxLength="25" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                            <asp:Button CssClass="ButtonAsh" OnClick="btnRegSearch_Click" runat="server" Text="Search"
                                ID="btnRegSeach" OnClientClick="return openIssueSearchPopup('ctl00_cphDet_txtRegistrationNo')" /></div>
                        </td>
                        <td align="right">Customer Type
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlCustomerType" SkinID="ddlMedium" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                        </td>
                        <td align="right">Total Amount
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtAppliedAmount" Width="100px" CssClass="textInput" runat="server"
                                    MaxLength="19" AutoPostBack="true" OnTextChanged="txtAppliedAmount_TextChanged"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Customer(s) Details</legend>
        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
            <ContentTemplate>
                <div style="height: 26%; width: 100%; overflow: auto;">
                    <table border="0">
                        <tr>
                            <td>
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Add" ID="btnCDAdd" Width="80px"
                                    OnClientClick="CheckJointCustomer()" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="height: 70%; width: 100%; overflow: auto;">
                    <asp:GridView Style="width: 98%; height: 100%" ID="gvCustomerDetail" runat="server"
                        AutoGenerateColumns="False" SkinID="SBMLGridGreen" OnRowCommand="gvCustomerDetail_RowCommand"
                        OnRowDeleting="gvCustomerDetail_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="Select" runat="server" ID="btnCusDetailSelect"
                                        Text="Select" OnClientClick="return CustomerDetailPopupReturnTrue(), CloseErrorPanel()" />
                                    <asp:HiddenField runat="server" ID="hdTmpCustomerID" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Button CssClass="ButtonAsh" CommandName="Delete" runat="server" ID="btnCustDetailDelete"
                                        Text="Delete" OnClientClick="return CloseErrorPanel(), CheckForDelete('this customer info?')" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="bfCustomerID" HeaderText="Customer ID" />
                            <asp:BoundField DataField="bfCustomerName" HeaderText="Name" />
                            <asp:BoundField DataField="bfDateOfBirth" DataFormatString="{0:dd-MMM-yyyy}" HeaderText="Date Of Birth 1" />
                            <asp:BoundField DataField="bfAddress" HeaderText="Address" />
                            <asp:BoundField DataField="bfPhone" HeaderText="Phone" />
                            <asp:BoundField DataField="bfNationality" HeaderText="Nationality" />
                            <asp:BoundField DataField="bfPassportNo" HeaderText="Passport No" />
<%--                            <asp:BoundField DataField="bfDateOfBirth2" DataFormatString="{0:dd-MMM-yyyy}" HeaderText="Date Of Birth 2" />
                            <asp:BoundField DataField="bfForeignAddress" HeaderText="Foreign Address" />--%>
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
    <%--End of User Comments--%><%--Start to User Comments--%>
    <br />
    <fieldset>
        <legend style="background-color: ButtonFace">Nominee(s) Details</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2">
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
                                <asp:DropDownList ID="ddlResidentCountry" runat="server" AutoPostBack="true" SkinID="ddlSmall" TabIndex="39" Width="190px">
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
    <%--End of User Comments--%>
    <br />
    <!-- Issue name panel -->
    <fieldset>
        <asp:UpdatePanel runat="server" ID="UpdatePanel4">
            <ContentTemplate>
                <table width="98%" align="center" class="tableBody" border="0">
                    <tr>
                        <td colspan="4" align="left">
                            <asp:Button CssClass="ButtonAsh" OnClick="btnShowPolicy_Click" runat="server" OnClientClick="return ShowPolicyDetail(), CloseErrorPanel()"
                                Text="Show Policy" ID="btnShowPolicy" />
                            <asp:Button CssClass="ButtonAsh" runat="server" OnClientClick="return NotDoneYet(), CloseErrorPanel()"
                                Text="Print Limit" ID="bnnPrintLimit" />
                            <asp:CheckBox runat="server" ID="chkFiscalYear" Text="Include Fiscal Year" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" valign="top">Issue Name
                        </td>
                        <td valign="top">
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtIssueName" Width="500px" CssClass="textInput" runat="server"
                                    TextMode="MultiLine" Height="35px" onblur="blurActiveInputWithObj(this)" MaxLength="100"
                                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            </div>
                            <div class="errorIcon">
                                *
                            </div>
                        </td>
                        <td align="right" valign="top">Master No
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="txtMasterNo" Width="100px" MaxLength="7" CssClass="textInput" runat="server"
                                onblur="blurActiveInputWithObj(this)" OnTextChanged="txtMasterNo_TextChanged"
                                AutoPostBack="true" onfocus="highlightActiveInputWithObj(this)">
                            </asp:TextBox><br />
                            <asp:Label runat="server" ID="lblMasterVarified" Text="Not Varified Yet"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <!-- end of Issue name panel -->
    <!-- Start to Bond Holder Details panel-->
    <br />
    <fieldset>
        <legend>Account Holder Details  </legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td>Account Holder(s) Name
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDHolderName" MaxLength="50" Width="480px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                        </td>
                        <td>Relation
                        </td>
                        <td>
                            <asp:TextBox ID="txtBHDRelation" MaxLength="50" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">Address
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtBHDAddress" MaxLength="255" Width="500px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                TextMode="MultiLine" Height="60px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <!-- End of Bond Holder Details panel-->

    <br />
    <!-- Start of Payment(s) Detail -->
    <fieldset>
        <legend style="background-color: ButtonFace">Payment Detail</legend>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
            <ContentTemplate>
                <table width="100%" align="center" class="tableBody" border="0">
                    <tr>
                        <td valign="top">Payment Mode
                        </td>
                        <td valign="top">Payment Amount
                        </td>
                        <td valign="top">Pay Account No
                        </td>
                        <td valign="top">Pay Account Name
                        </td>
                         <td>Collection Branch</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList SkinID="ddlMedium" ID="ddlPDPaymentMode" Width="150px" runat="server"
                                OnSelectedIndexChanged="ddlPDPaymentMode_SelectedIndexChanged" AutoPostBack="True">
                                <asp:ListItem Value="3">Debit Inst</asp:ListItem>
                                <asp:ListItem Value="2">Cheque</asp:ListItem>
                                <asp:ListItem Value="1">Cash</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalAmount" MaxLength="150" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Height="20px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAccountNo" MaxLength="150" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Height="20px" OnTextChanged="txtAccountNo_TextChanged" AutoPostBack="True"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAccountName" MaxLength="250" Width="220px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Height="20px"></asp:TextBox>
                        </td>
                                                <td><asp:DropDownList ID="ddlCollectionBranch" SkinID="ddlMedium" runat="server" TabIndex="2">
                                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td valign="top">Account Type
                        </td>
                        <td valign="top">CHQ No
                        </td>
                        <td valign="top">Branch Name
                        </td>
                        <td valign="top">Routing No
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList SkinID="ddlMedium" ID="ddlAccountType" Width="150px" runat="server">
                                <asp:ListItem Value="Savings">Saving</asp:ListItem>
                                <asp:ListItem Value="Current">Current</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCHQNo" MaxLength="150" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Height="20px"></asp:TextBox>
                        </td>
                        <td>
                             <asp:DropDownList ID="ddlAccountBranch" SkinID="ddlMedium" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlAccountBranch_SelectedIndexChanged">
                            </asp:DropDownList>

                        </td>
                        <td>
                                <asp:TextBox ID="txtRoutingNo" MaxLength="150" Width="150px" CssClass="textInput"
                                runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                Height="20px"></asp:TextBox>

                        </td>
                        <td></td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </fieldset>
    <!-- End of Payment(s) Detail -->

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
        <asp:UpdatePanel runat="server" ID="UpdatePanel6">
        <ContentTemplate>
        <table width="95%" align="center" cellpadding="10" cellspacing="3" border="0">
            <tr>
                <td align="left">
                    <asp:Button CssClass="ButtonAsh" OnClientClick="return NotDoneYet()" runat="server"
                        Text="View Journals" ID="Button1" Visible="False" />
                </td>
                <td align="right">
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Back" ID="btnBack" OnClick="btnBack_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reject" ID="btnReject" OnClientClick="return RejectValidation()"
                        OnClick="btnReject_Click" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Approve" ID="btnApprove" OnClick="btnApprove_Click"
                        OnClientClick=" MsgPopupReturnTrue('Approve') " />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Reset" ID="btnReset" OnClick="btnReset_Click"
                        OnClientClick="CloseErrorPanel()" />
                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Save" ID="btnSave" OnClientClick="return SaveValidation()"
                        OnClick="btnSave_Click" />

                    <asp:Button CssClass="ButtonAsh" runat="server" Text="Delete" ID="btnDelete" OnClientClick="return DeleteOldCustomerIssue()"
                        OnClick="btnDelete_Click" />
                </td>
            </tr>
        </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </fieldset>
    <uc3:PolicyDetails ID="PD" runat="server" />
    <uc2:CustomerDetails ID="CustomerDetail" runat="server" />
    <uc4:SIssue ID="SIssue" runat="server" Type="3" />
    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>

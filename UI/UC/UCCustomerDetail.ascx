<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCCustomerDetail.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCCustomerDetail" %>
er">
    <table width="900" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Customer Detail
                    </div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideErroPanel(),hideModal('ctl00_cphDet_CustomerDetail_MDCustomerDetail');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 620px; overflow: auto; width: 900px;">
                    <div id="divPopUpCustomerErrorPanel" style="display: none">
                        <fieldset class="errorFieldset">
                            <legend class="errorLegend">Sorry, there were problems with your input</legend>
                            <div class="errorBox" id="divPopUpCustomerErrorList">
                            </div>
                        </fieldset>
                        <br />
                    </div>
                    <asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>
                            <asp:HiddenField runat="server" ID="hdnIsReinvested" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnCustomerType" Value="" />
                            <fieldset>
                                <legend>Master Details</legend>
                                <table width="850" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="right">Master No
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtMasterNo" TabIndex="31" MaxLength="7" runat="server" CssClass="textInput"
                                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                    Width="120px"></asp:TextBox>
                                                <div class="fieldLeft">
                                                    <asp:TextBox ID="txtCustomerID" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                        onfocus="highlightActiveInputWithObj(this)" Width="120px" MaxLength="9" TabIndex="33" Visible="false"></asp:TextBox>
                                                    <asp:HiddenField runat="server" ID="hdTmpCustomerID" />
                                                </div>
                                            </div>
                                            <div class="searchButton">
                                                <asp:Button ID="btnMasterLoad" OnClick="btnMasterLoad_Click" runat="server" CssClass="ButtonAsh"
                                                    Text="Search" TabIndex="32" />
                                            </div>
                                        </td>
                                        <td align="right">Customer Name
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCustomerName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                    onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="100" TabIndex="34"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                *
                                            </div>
                                        </td>
                                    </tr>
                                    <%--                                <tr>
                                    <td align="right">
                                        Customer ID
                                    </td>
                                    <td colspan="3">
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtCustomerID" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                onfocus="highlightActiveInputWithObj(this)" Width="120px" MaxLength="9" TabIndex="33"></asp:TextBox>
                                            <asp:HiddenField runat="server" ID="hdTmpCustomerID" />
                                        </div>
                                    </td>
                                </tr>--%>
                                    <tr>
                                        <td align="right" valign="top">Is HUB Customer</td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="IsHUBCust" runat="server" />
                                            </div>

                                        </td>
                                        <td align="right" valign="top">SCC Flags
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlSCCFlags" runat="server" SkinID="ddlSmall" TabIndex="39">
                                                    <asp:ListItem Text="Non SCC" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Non PEP SCC" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Regular PEP" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Regular PEP" Value="3"></asp:ListItem>
                                                    <asp:ListItem Text="Unknown" Value="4"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">Present Address
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtAddress" CssClass="textInput"
                                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                    MaxLength="150" TabIndex="35"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                *
                                            </div>
                                        </td>
                                        <td align="right" valign="top"></td>
                                        <td>
                                            <div class="fieldLeft"></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="top">Permanent Address</td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtPermanentAddress" CssClass="textInput"
                                                    runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                    MaxLength="150" TabIndex="35"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                *
                                            </div>

                                        </td>
                                        <td align="right" valign="top">Forign Address
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtForignAddress"
                                                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                    MaxLength="150" TabIndex="36"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">Gender
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlSex" runat="server" SkinID="ddlSmall" TabIndex="39">
                                                    <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                                    <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                        <td align="right">Residence Status
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlResidenceStatus" runat="server" SkinID="ddlSmall" TabIndex="40" Width="240px">
                                                    <asp:ListItem Text="Resident" Value="RS"></asp:ListItem>
                                                    <asp:ListItem Text="Non Resident" Value="NRS"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">Phone
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPhone" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                    onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="50" TabIndex="37"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td align="right">Email ID
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtEmail" TabIndex="46" MaxLength="45" runat="server" CssClass="textInput"
                                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                    Width="240px"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset>
                                <legend>ID/No Details</legend>
                                <table width="850" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td align="right">Country Details</td>
                                        <td>Nationality</td>
                                        <td align="top">Nationality Country</td>
                                        <td>Resident Country</td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtNationality" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                                    Text="Bangladeshi" onfocus="highlightActiveInputWithObj(this)" Width="240px"
                                                    MaxLength="20" TabIndex="41"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                *
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlNationalityCountry" runat="server" SkinID="ddlSmall" TabIndex="39" Height="18px" Width="240px">
                                                    <asp:ListItem Text="Bangladesh" Value="M"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlResidentCountry" runat="server" SkinID="ddlSmall" TabIndex="39" Width="240px">
                                                    <asp:ListItem Text="Dhaka" Value="M"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">Birth Details</td>
                                        <td>Date</td>
                                        <td align="top">Country</td>
                                        <td>Place</td>
                                    </tr>
                                    <tr>
                                        <td align="right">&nbsp; 
                                        </td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox Width="80Px" ID="txtDateofBirth" CssClass="textInput" runat="server"
                                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                                    AccessKey="A" TabIndex="38"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                *
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlDateofBirthCountry" runat="server" SkinID="ddlSmall" TabIndex="39" Height="18px" Width="240px">
                                                    <asp:ListItem Text="Bangladesh" Value="M"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDateofBirthPlace" runat="server" CssClass="textInput" MaxLength="50" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="42" Width="240px"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">NID Details</td>
                                        <td>ID/No</td>
                                        <td align="top">Country</td>
                                        <td>Issue At</td>
                                    </tr>
                                    <tr>
                                        <td align="right">&nbsp;</td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtNationalID" runat="server" CssClass="textInput" MaxLength="30" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="42" Width="240px"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                [*]
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlNationalIDCountry" runat="server" SkinID="ddlSmall" TabIndex="39" Height="18px" Width="240px">
                                                    <asp:ListItem Text="Bangladesh" Value="M"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtNationalIDIssueAt" runat="server" CssClass="textInput" MaxLength="50" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="42" Width="240px"></asp:TextBox>
                                            </div>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">Passport Details
                                        </td>
                                        <td valign="top">ID/No</td>
                                        <td align="Left">Country</td>
                                        <td valign="top">Issue At</td>
                                    </tr>
                                    <tr>
                                        <td align="right">&nbsp;</td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPassportNo" runat="server" CssClass="textInput" MaxLength="12" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="43" Width="240px"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                [*]
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlPassportCountry" runat="server" SkinID="ddlSmall" TabIndex="39" Height="18px" Width="240px">
                                                    <asp:ListItem Text="Bangladesh" Value="M"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPassportIssueAt" runat="server" CssClass="textInput" MaxLength="50" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="42" Width="240px"></asp:TextBox>
                                            </div>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">Birth certificate
                                        </td>
                                        <td>ID/No</td>
                                        <td valign="top">Country</td>
                                        <td valign="top">Issue At</td>
                                    </tr>
                                    <tr>
                                        <td align="right">&nbsp;</td>
                                        <td>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtBirthCertNo" runat="server" CssClass="textInput" MaxLength="30" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="45" Width="240px"></asp:TextBox>
                                            </div>
                                            <div class="errorIcon">
                                                [*]
                                            </div>
                                        </td>
                                        <td valign="top" align="midle">
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlBirthCertNoCountry" runat="server" SkinID="ddlSmall" TabIndex="39" Height="18px" Width="240px">
                                                    <asp:ListItem Text="Bangladesh" Value="M"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtBirthCertNoIssueAt" runat="server" CssClass="textInput" MaxLength="50" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" TabIndex="42" Width="240px"></asp:TextBox>
                                            </div>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right">
                                            <br />
                                            <asp:Button ID="btnSaveAndLoad" TabIndex="60" runat="server" CssClass="ButtonAsh"
                                                Text="Save & Load" OnClick="btnSaveAndLoad_Click" OnClientClick="return SaveAndLoadValidation()" />
                                            <asp:Button ID="btnCustomerDetailPopup" TabIndex="61" runat="server" CssClass="ButtonAsh"
                                                Text="Close" OnClientClick="return CustomerDetailPopupHide()" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger EventName="Click" ControlID="btnMasterLoad" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">

    function CustomerDetailPopupHide() {
        hideErroPanel();
        hideModal('ctl00_cphDet_CustomerDetail_MDCustomerDetail');
        return false;
    }
    function CustomerDetailPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_CustomerDetail_MDCustomerDetail');
        return true;
    }
    function CustomerDetailPopup() {
        $("#<%=txtEmail.ClientID %>").val('');
        $("#<%=txtCustomerID.ClientID %>").val('');
        $("#<%=hdTmpCustomerID.ClientID %>").val('');
        $("#<%=txtMasterNo.ClientID %>").val('');
        $("#<%=txtCustomerName.ClientID %>").val('');
        $("#<%=txtAddress.ClientID %>").val('');
        $("#<%=txtForignAddress.ClientID %>").val('');
        $("#<%=txtPhone.ClientID %>").val('');
        $("#<%=txtDateofBirth.ClientID %>").val('');
        $("#<%=txtNationality.ClientID %>").val('BANGLADESHI');
        $("#<%=txtPassportNo.ClientID %>").val('');
        $("#<%=txtNationalID.ClientID %>").val('');
        $("#<%=txtBirthCertNo.ClientID %>").val('');

        var cType = document.getElementById("ctl00_cphDet_ddlCustomerType");
        if (cType != null) {
            if (cType.options[cType.selectedIndex].text.search("Joint") == -1) {
                $("#<%=hdnCustomerType.ClientID %>").val('');
            }
            else {
                $("#<%=hdnCustomerType.ClientID %>").val('2');
            }
        }

        showModal("ctl00_cphDet_CustomerDetail_MDCustomerDetail");

        return false;
    }

    function CustomerDetailPopupReturnTrue() {
        showModal("ctl00_cphDet_CustomerDetail_MDCustomerDetail");
        return true;
    }

    function RestCustomerDetail() {
        //$("#<%=ddlSex.ClientID %>")[0].selectedIndex = 0;
        $("#<%=txtMasterNo.ClientID %>").val('');
        $("#<%=txtCustomerID.ClientID %>").val('');
        $("#<%=txtCustomerName.ClientID %>").val('');
        $("#<%=txtAddress.ClientID %>").val('');
        $("#<%=txtForignAddress.ClientID %>").val('');
        $("#<%=txtDateofBirth.ClientID %>").val('');
        $("#<%=txtNationality.ClientID %>").val('Bangladeshi');
        $("#<%=txtPassportNo.ClientID %>").val('');
        $("#<%=txtNationalID.ClientID %>").val('');
        $("#<%=txtBirthCertNo.ClientID %>").val('');
        $("#<%=txtEmail.ClientID %>").val('');
        $("#<%=hdTmpCustomerID.ClientID %>").val('');
    }


    function SaveAndLoadValidation() {

        var sErrorList = "";
        var hdnReinvested = document.getElementById('<%=hdnIsReinvested.ClientID %>');
        if (hdnReinvested.value == "1") {
            var oSPType = document.getElementById("ctl00_cphDet_ddlSpType");
            if (oSPType != null) {
                if (oSPType.value == "DIB" || oSPType.value == "DPB") {
                    sErrorList += RequiredData('<%=txtPassportNo.ClientID %>', 'TextBox', "Passport No. cannot be empty!");
                }
                else {
                    if ($("#<%=txtPassportNo.ClientID %>").val() != "") {

                    }
                }
            }
            sErrorList += RequiredData('<%=txtAddress.ClientID %>', 'TextBox', "Address cannot be empty!");

        }
        else {
            //distinguishable feature between New SPIssue & Old Customer SPIssue
            var oOldCustSeries = document.getElementById("ctl00_cphDet_txtCDSeries");

            var cType = document.getElementById("ctl00_cphDet_ddlCustomerType");
            if (oOldCustSeries != null) { //Only for OldCustomer     
                sErrorList += RequiredData('<%=txtCustomerName.ClientID %>', 'TextBox', "Customer Name cannot be empty!");
            } else {

                sErrorList += RequiredData('<%=txtCustomerName.ClientID %>', 'TextBox', "Customer Name cannot be empty!");
                sErrorList += RequiredData('<%=txtAddress.ClientID %>', 'TextBox', "Address cannot be empty!");
                if (cType.options[cType.selectedIndex].text.search("Institution") == -1) {
                    sErrorList += RequiredData('<%=txtDateofBirth.ClientID %>', 'TextBox', "Date of Birth cannot be empty!");
                    sErrorList += RequiredData('<%=txtNationality.ClientID %>', 'TextBox', "Nationality cannot be empty!");
                }
            }
            // checking Gender for SP Type
            var oSPType = document.getElementById("ctl00_cphDet_ddlSpType");
            if (oSPType != null) {
                //Foreging Adress is mandatory for Bonds
                if (oOldCustSeries == null) {//Only for New SPIssue
                    if (oSPType.value == "WDB" || oSPType.value == "DIB" || oSPType.value == "DPB") {
                        sErrorList += RequiredData('<%=txtForignAddress.ClientID %>', 'TextBox', "Foreign Address cannot be empty!");
                    }
                }

                var oSupprtdSex = document.getElementById("ctl00_cphDet_hdSupportdGndr");

                var oddlSex = document.getElementById('<%=ddlSex.ClientID %>');
                if (oSupprtdSex.value == "1" && oddlSex.value == "F") {
                    sErrorList += "<li>You must select Male gender for Customer</li>";
                }
                else if (oSupprtdSex.value == "2" && oddlSex.value == "M") {
                    sErrorList += "<li>You must select Female gender for Customer</li>";
                }
            }
            if (oOldCustSeries == null) { //Only for SPIssue
                if (cType.options[cType.selectedIndex].text.search("Institution") == -1) {
                    var oPassportNo = document.getElementById('<%=txtPassportNo.ClientID %>');
                    var oBirthCertNo = document.getElementById('<%=txtBirthCertNo.ClientID %>');
                    var oNationalID = document.getElementById('<%=txtNationalID.ClientID %>');
                }
                if (oSPType != null) {
                    if (oSPType.value == "WDB" || oSPType.value == "DIB" || oSPType.value == "DPB") {
                        if (oPassportNo != null ) {
                            if (oPassportNo.value == "" ) {
                                sErrorList += RequiredData('<%=txtPassportNo.ClientID %>', 'TextBox', "Passport No. cannot be empty!");
                            }
                        }
                    }
                    else {
                        if (oPassportNo != null && oBirthCertNo != null && oNationalID != null) {
                            if (oPassportNo.value == "" && oBirthCertNo.value == "" && oNationalID.value == "") {
                                sErrorList += "<li>Passport No. / Birth certificate / National ID cannot be empty!</li>";
                            }
                        }
                    }
                }
                //if (oPassportNo != null && oIssueAt != null) {
                //    if (oPassportNo.value != "" && oIssueAt.value == "") {

                //    }
                //}
                //&& oIssueAt.value != null
                //|| oIssueAt.value == ""

            }
        }

        var divErrorPanel = document.getElementById('divPopUpCustomerErrorPanel');
        if (sErrorList.length > 0) {
            var errorList = document.getElementById('divPopUpCustomerErrorList');
            errorList.innerHTML = "<ul>" + sErrorList + "</ul>";
            divErrorPanel.style.display = "block";

            return false;
        }
        else {
            divErrorPanel.style.display = "none";
            CustomerDetailPopupHideReturnTrue();
            return true;
        }
    }

    function hideErroPanel() {
        var divErrorPanel = document.getElementById('divPopUpCustomerErrorPanel');
        if (divErrorPanel != null) {
            divErrorPanel.style.display = "none";
        }
    }
</script>


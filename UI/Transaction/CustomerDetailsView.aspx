<%@ Page Title="" Language="C#" MasterPageFile="~/mp/site.master" AutoEventWireup="true"
    CodeBehind="CustomerDetailsView.aspx.cs" Inherits="SBM_WebUI.mp.CustomerDetailsView" %>

<%@ Register Src="~/UI/UC/UCUserDetails.ascx" TagName="UserDetails" TagPrefix="uc1" %>
<%@ Register Src="../UC/UCCustomerDetail.ascx" TagName="CustomerDetails" TagPrefix="uc2" %>
<%@ Register Src="../UC/UCPolicy.ascx" TagName="PolicyDetails" TagPrefix="uc3" %>
<%@ Register Src="../UC/UCSearchIssue.ascx" TagName="SIssue" TagPrefix="uc4" %>
<%@ Register Src="~/UI/UC/UCMessage.ascx" TagName="Error" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/tabber.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function pageLoad(sender, args) {
            $('input[id*=Date]').each(function() { $(this).mask("99/99/9999"); });
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphDet" runat="server">
    <table width="850" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <table width="850" align="center" class="tableBody" border="0">
                    <tr>
                        <td align="right" style="width: 120px">
                            Master No
                        </td>
                        <td>
                            <div class="fieldLeft">
                                <asp:TextBox ID="txtMasterNo" TabIndex="1" MaxLength="7" runat="server" CssClass="textInput"
                                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                    Width="120px"></asp:TextBox>
                            </div>
                            <div class="searchButton">
                                <asp:Button ID="btnMasterLoad" OnClick="btnMasterLoad_Click" runat="server" CssClass="ButtonAsh"
                                    Text="Search" TabIndex="2" />
                                <asp:Button CssClass="ButtonAsh" runat="server" Text="Print Limit" ID="btnPrintLimit"
                                    OnClick="btnPrintLimit_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel runat="server" ID="upSuccess">
                    <ContentTemplate>
                        <table width="850" align="center" class="tableBody" border="0">
                            <tr>
                                <td align="right">
                                    Customer ID
                                </td>
                                <td colspan="3">
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtCustomerID" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            onfocus="highlightActiveInputWithObj(this)" Width="120px" MaxLength="9" TabIndex="3"
                                            ReadOnly="True"></asp:TextBox>
                                        <asp:HiddenField runat="server" ID="hdTmpCustomerID" />
                                    </div>
                                    <%--<div class="errorIcon">
                                            *</div>
                                        <div class="searchButton">
                                            <asp:Button ID="btnCustomerLoad" OnClick="btnCustomerLoad_Click" runat="server" CssClass="ButtonAsh"
                                                Text="Search" />
                                        </div>--%>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Customer Name
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtCustomerName" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="100" TabIndex="4"
                                            ReadOnly="True"></asp:TextBox>
                                    </div>
                                    <div class="errorIcon">
                                        *</div>
                                </td>
                                <td align="right">
                                    Customer Name 2
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCustomerName2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="100" TabIndex="17"
                                        ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    Address
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtAddress" CssClass="textInput"
                                            runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            MaxLength="150" TabIndex="5" ReadOnly="True"></asp:TextBox></div>
                                    <div class="errorIcon">
                                        *</div>
                                </td>
                                <td align="right" valign="top">
                                    Address 2
                                </td>
                                <td>
                                    <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtAddress2" CssClass="textInput"
                                        runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        MaxLength="150" TabIndex="18" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    Forign Address
                                </td>
                                <td valign="top">
                                    <div class="fieldLeft">
                                        <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtForignAddress"
                                            CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            MaxLength="150" TabIndex="6" ReadOnly="True"></asp:TextBox></div>
                                </td>
                                <td align="right" valign="top">
                                    Forign Address2
                                </td>
                                <td valign="top">
                                    <div class="fieldLeft">
                                        <asp:TextBox TextMode="MultiLine" Height="70px" Width="240px" ID="txtForignAddress2"
                                            CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            MaxLength="150" TabIndex="19" ReadOnly="True"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Phone
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPhone" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="50" TabIndex="7"
                                            ReadOnly="True"></asp:TextBox>
                                    </div>
                                </td>
                                <td align="right">
                                    Phone2
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPhone2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="50" TabIndex="20"
                                        ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Date of Birth
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox Width="80Px" ID="txtDateofBirth" CssClass="textInput" runat="server"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            AccessKey="A" TabIndex="8"></asp:TextBox>
                                    </div>
                                    <div class="errorIcon">
                                        *</div>
                                </td>
                                <td align="right">
                                    Date of Birth 2
                                </td>
                                <td>
                                    <asp:TextBox Width="80Px" ID="txtDateofBirth2" CssClass="textInput" runat="server"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        AccessKey="A" TabIndex="21" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Gender
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                    </div>
                                    <asp:TextBox ID="txtSex" runat="server" CssClass="textInput" MaxLength="20" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" ReadOnly="True" TabIndex="11" Width="100px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    Gender 2
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSex2" runat="server" CssClass="textInput" MaxLength="20" onblur="blurActiveInputWithObj(this)"
                                        onfocus="highlightActiveInputWithObj(this)" ReadOnly="True" TabIndex="11" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Residence Status
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                    </div>
                                    <asp:TextBox ID="txtResStatus" runat="server" CssClass="textInput" MaxLength="20"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        ReadOnly="True" TabIndex="11" Width="150px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    Residence Status2
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                    </div>
                                    <asp:TextBox ID="txtResStatus2" runat="server" CssClass="textInput" MaxLength="20"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        ReadOnly="True" TabIndex="11" Width="150px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Nationality
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtNationality" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            Text="Bangladeshi" onfocus="highlightActiveInputWithObj(this)" Width="240px"
                                            MaxLength="20" TabIndex="11" ReadOnly="True"></asp:TextBox>
                                    </div>
                                    <div class="errorIcon">
                                        *</div>
                                </td>
                                <td align="right">
                                    Nationality 2
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtNationality2" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            Text="Bangladeshi" onfocus="highlightActiveInputWithObj(this)" Width="240px"
                                            MaxLength="24" TabIndex="24" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Passport No
                                </td>
                                <td valign="top">
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPassportNo" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            onfocus="highlightActiveInputWithObj(this)" Width="240px" MaxLength="12" TabIndex="12"
                                            ReadOnly="True"></asp:TextBox>
                                    </div>
                                    <div class="errorIcon">
                                        [*]</div>
                                </td>
                                <td align="right">
                                    Passport No 2
                                </td>
                                <td valign="top">
                                    <div class="fieldLeft">
                                    </div>
                                    <asp:TextBox ID="txtPassportNo2" runat="server" CssClass="textInput" MaxLength="12"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        ReadOnly="True" TabIndex="25" Width="240px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Issue At
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtIssueAt" MaxLength="20" runat="server" CssClass="textInput" onblur="blurActiveInputWithObj(this)"
                                            onfocus="highlightActiveInputWithObj(this)" Width="240px" TabIndex="13" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </td>
                                <td align="right">
                                    Issue At 2
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtIssueAt2" MaxLength="20" runat="server" CssClass="textInput"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            Width="240px" TabIndex="26" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    National ID
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtNationalID" TabIndex="14" runat="server" CssClass="textInput"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            Width="240px" MaxLength="30" ReadOnly="True"></asp:TextBox>
                                    </div>
                                    <div class="errorIcon">
                                        [*]</div>
                                </td>
                                <td align="right">
                                    National ID 2
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNationalID2" TabIndex="27" runat="server" CssClass="textInput"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        Width="240px" MaxLength="30" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Birth certificate No
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtBirthCertNo" TabIndex="15" MaxLength="30" runat="server" CssClass="textInput"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            Width="240px" ReadOnly="True"></asp:TextBox>
                                    </div>
                                    <div class="errorIcon">
                                        [*]</div>
                                </td>
                                <td align="right">
                                    Birth certificate No 2
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBirthCertNo2" TabIndex="28" MaxLength="30" runat="server" CssClass="textInput"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        Width="240px" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Email ID
                                </td>
                                <td>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtEmail" TabIndex="16" MaxLength="45" runat="server" CssClass="textInput"
                                            onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                            Width="240px" ReadOnly="True"></asp:TextBox>
                                    </div>
                                </td>
                                <td align="right">
                                    Email ID 2
                                </td>
                                <td>
                                    <asp:TextBox ID="txtEmail2" TabIndex="29" MaxLength="45" runat="server" CssClass="textInput"
                                        onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"
                                        Width="240px" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnMasterLoad" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
    </table>

    <script language="javascript" type="text/javascript">
        function RestCustomerDetail() {
            $("#<%=txtMasterNo.ClientID %>").val('');
            $("#<%=txtCustomerID.ClientID %>").val('');
            $("#<%=txtCustomerName.ClientID %>").val('');
            $("#<%=txtCustomerName2.ClientID %>").val('');
            $("#<%=txtAddress.ClientID %>").val('');
            $("#<%=txtAddress2.ClientID %>").val('');
            $("#<%=txtForignAddress.ClientID %>").val('');
            $("#<%=txtForignAddress2.ClientID %>").val('');
            $("#<%=txtPhone.ClientID %>").val('');
            $("#<%=txtPhone2.ClientID %>").val('');
            $("#<%=txtDateofBirth.ClientID %>").val('');
            $("#<%=txtDateofBirth2.ClientID %>").val('');
            $("#<%=txtNationality.ClientID %>").val('Bangladeshi');
            $("#<%=txtNationality2.ClientID %>").val('Bangladeshi');
            $("#<%=txtPassportNo.ClientID %>").val('');
            $("#<%=txtPassportNo2.ClientID %>").val('');
            $("#<%=txtIssueAt.ClientID %>").val('');
            $("#<%=txtIssueAt2.ClientID %>").val('');
            $("#<%=txtNationalID.ClientID %>").val('');
            $("#<%=txtNationalID2.ClientID %>").val('');
            $("#<%=txtBirthCertNo.ClientID %>").val('');
            $("#<%=txtBirthCertNo2.ClientID %>").val('');
            $("#<%=txtEmail.ClientID %>").val('');
            $("#<%=txtEmail2.ClientID %>").val('');
            $("#<%=hdTmpCustomerID.ClientID %>").val('');
        }
        function hideErroPanel() {
            var divErrorPanel = document.getElementById('divPopUpCustomerErrorPanel');
            if (divErrorPanel != null) {
                divErrorPanel.style.display = "none";
            }
        }
    </script>

    <uc3:Error ID="ucMessage" runat="server" />
</asp:Content>

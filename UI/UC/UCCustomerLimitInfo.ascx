<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCCustomerLimitInfo.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCCustomerLimitInfo" %>
<div id="MDCustomerLimitInfo" class="MDClass" runat="server">
    <table width="600" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <div class="MDHeader" style="width: 100%">
                    <div class="MDTitle">
                        Customer Limit Information</div>
                    <div class="MDClose">
                        <a href="javascript:void(0);">
                            <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_CustLimitInfo_MDCustomerLimitInfo');" />
                        </a>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="MDBody" style="height: 450px; overflow: auto;">
                    <asp:UpdatePanel runat="server" ID="upSuccess">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Search Criteria</legend>
                                <table width="550px" align="center" class="tableBody" border="0" cellpadding="3">
                                    <tr>
                                        <td align="right">
                                            Customer Type
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCustomerType" MaxLength="15" CssClass="textInput"
                                                Width="120px" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Maximum Limit
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtMaximumLimit" CssClass="textInput" Width="120px"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            SP Type
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtSPType" MaxLength="15" CssClass="textInput" Width="120px"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                        <td align="right">
                                            Minimum Limit
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtMinimumLimit" CssClass="textInput" Width="120px"
                                                onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <br />
                            <fieldset runat="server" id="fsList">
                                <legend>Limit Detail List</legend>
                                <table width="100%" align="center" class="tableBody" border="0">
                                    <tr>
                                        <td colspan="2">
                                            <asp:UpdatePanel runat="server" ID="upGv">
                                                <ContentTemplate>
                                                    <asp:GridView Width="98%" AutoGenerateColumns="true" ID="gvSearchList" runat="server"
                                                        SkinID="SBMLGridGreen" ShowHeader="true" AllowPaging="true">
                                                        <EmptyDataTemplate>
                                                            No record found
                                                        </EmptyDataTemplate>
                                                        <AlternatingRowStyle CssClass="odd" />
                                                    </asp:GridView>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" width="66%">Total Amount:&nbsp;</td>
                                        <td align="left" width="34%"><asp:TextBox runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)" ReadOnly="true" CssClass="textInput" id="txtTotalAmount" Width="120px" ></asp:TextBox></td>
                                    </tr>
                                </table>
                            </fieldset>
                            <table width="100%">
                            <tr>
                                <td align="center" style="padding-bottom:10px">
                                    <asp:Button ID="btnClose" runat="server" CssClass="ButtonAsh" Text="&nbsp;&nbsp;&nbsp;&nbsp;Close&nbsp;&nbsp;&nbsp;&nbsp;"
                                       onClick="btnClose_Click"  OnClientClick="return CustLimitInfoPopupHideReturnTrue()" />
                                </td>
                            </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</div>

<script language="javascript" type="text/javascript">

    function CustLimitInfoPopupHide() {
        hideModal('ctl00_cphDet_CustLimitInfo_MDCustomerLimitInfo');
        return false;
    }
    function CustLimitInfoPopupHideReturnTrue() {
        hideModal('ctl00_cphDet_CustLimitInfo_MDCustomerLimitInfo');
        return true;
    }
    function CustLimitInfoPopupReturnTrue() {
        showModal("ctl00_cphDet_CustLimitInfo_MDCustomerLimitInfo");
        return true;
    }
    
</script>


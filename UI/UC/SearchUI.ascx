<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchUI.ascx.cs" Inherits="SBM_WebUI.UI.UC.SearchUI" %>


<div id="MDLoanAc" class="MDClass" runat="server">
        <table width="300" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td>
                    <div class="MDHeader" style="width: 100%">
                        <div class="MDTitle">
                            Test Search Panel</div>
                        <div class="MDClose">
                            <a href="javascript:void(0);">
                                <img border="0" alt="" src="../../Images/popupclose.gif" onclick="javascript:hideModal('ctl00_cphDet_SearchUI1_MDLoanAc');" />
                            </a>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="MDBody">
                        <table width="400px">
                            <tr>
                                <td>
                                    <div class="row">
                                        <span class="labelLeft">Name:</span> <span class="valueField">
                                            <asp:TextBox MaxLength="13" CssClass="textInput" runat="server" ID="txtName"></asp:TextBox>
                                        </span>
                                    </div>
                                    <div id="divButton" class="row centerAlign">
                                        <asp:Button ID="btnAction" OnClick="btnAction_OnClick" runat="server" Text="Action Partial" CssClass="ButtonGreen" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    
    
    <script language="javascript" type="text/javascript">
        function SearchPopup() {
            showModal("ctl00_cphDet_SearchUI1_MDLoanAc");
            return false;
        }
    </script>


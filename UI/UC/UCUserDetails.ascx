<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCUserDetails.ascx.cs"
    Inherits="SBM_WebUI.UI.UC.UCUserDetails" %>
<fieldset>
    <legend>User Details</legend>
    <table width="100%" align="center" class="tableBody" border="0">
        <tr>
            <td valign="top" align="right">
                Maker ID
            </td>
            <td valign="top" style="width: 120px">
                <asp:TextBox Enabled="false" Width="120px" ID="txtMakerId" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                    onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
            </td>
            <td valign="top" align="right">
                Checker ID
            </td>
            <td valign="top" style="width: 120px">
                <asp:TextBox  Enabled="false"  Width="120px" ID="txtCheckerId" CssClass="textInput" runat="server"
                    onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
            </td>
            <td rowspan="2" valign="top" align="right">
                Checker Comments
            </td>
            <td rowspan="2">
                <asp:TextBox  Enabled="false"  Width="330px" Height="40px" TextMode="MultiLine" ID="txtCheckerComments"
                    CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)" onfocus="highlightActiveInputWithObj(this)"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td valign="top" align="right">
                Make Date
            </td>
            <td valign="top" style="width: 120px">
                <asp:TextBox Enabled="false" Width="80Px" ID="txtMakeDate" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                    onfocus="highlightActiveInputWithObj(this)">
                </asp:TextBox>
            </td>
            <td valign="top" align="right">
                Check Date
            </td>
            <td valign="top" style="width: 120px">
                <asp:TextBox  Enabled="false" Width="80Px" ID="txtCheckDate" CssClass="textInput" runat="server" onblur="blurActiveInputWithObj(this)"
                    onfocus="highlightActiveInputWithObj(this)">
                </asp:TextBox>
            </td>
        </tr>
    </table>
</fieldset>

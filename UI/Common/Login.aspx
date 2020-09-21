<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="SBM_WebUI.UI.Common.Login" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<html>
<head>
    <title>Sanchaya Patra Management System</title>
    
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.autocomplete.js" type="text/javascript"></script>
    
    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            var sNameList = $("#<%=hdUserList.ClientID %>").val();
            var data = sNameList.split("~");
            $("#txtUserName").autocomplete(data);
            $("#txtUserName").focus();
        });
    </script>
    
    <style type="text/css">
        
        
      /* new auto complete*/
    .ac_results { FONT-SIZE: 15px; BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 0px; BORDER-TOP: black 1px solid; 
                  PADDING-LEFT: 0px; Z-INDEX: 99999; PADDING-BOTTOM: 0px; OVERFLOW: hidden; 
                  BORDER-LEFT: black 1px solid; PADDING-TOP: 0px; 
                  BORDER-BOTTOM: black 1px solid; BACKGROUND-COLOR: white }
    .ac_results UL {PADDING-RIGHT: 0px; PADDING-LEFT: 0px; LIST-STYLE-POSITION: outside; 
                    PADDING-BOTTOM: 0px; MARGIN: 0px; WIDTH: 100%; PADDING-TOP: 0px; 
                    LIST-STYLE-TYPE: none; FONT-SIZE: 15px;  }
    .ac_results LI {PADDING-RIGHT: 5px; DISPLAY: block; PADDING-LEFT: 5px; FONT-SIZE: 15px; 
                    PADDING-BOTTOM: 2px; MARGIN: 0px; FONT: menu; OVERFLOW: hidden; 
                    CURSOR: default; LINE-HEIGHT: 16px; PADDING-TOP: 2px; }
    .ac_loading { BACKGROUND: url(indicator.gif) white no-repeat right center; FONT-SIZE: 15px;  }
    .ac_odd { BACKGROUND-COLOR: #EAFBE1; FONT-SIZE: 15px;  }
    .ac_over { COLOR: white; BACKGROUND-COLOR: #3192C4; FONT-SIZE: 15px; }
    /* end of auto complete*/

        
        
    .ButtonGreen {
	    background: url(../../Images/btn_bg.jpg);	border: #D2D0D0 solid 1px;	
	    border-bottom-color:#B2B2B3;	font-weight: bold;	padding-left:2px;	
	    padding-right:2px;	color: #5D5F60; 	
	    font-family: Verdana;	font-size: 12px;	
	    cursor: hand;	height: 24px;	white-space:nowrap;	
	    -moz-background-clip:border;    -moz-background-origin:padding;    
	    -moz-background-size:auto auto;    
	    -moz-box-shadow:0 1px 0 rgba(0, 0, 0, 0.1);    background-position:0 0;    
	    padding-bottom:2px;    padding-top:0px;
	
    }

#loginh {  clear:both; width:100%; height:40px; padding:0px 0px 5px 0px; border-bottom:solid 2px green; }
.loginfl { float:left; }
.loginfr { font-family:Tahoma; font-size:24px; text-align:right; float:right; vertical-align:middle;}        
a.text { font-family:Verdana; font-size:12px; background-color: #FFFFFF; }
a.errorText {   font-family:Verdana;   font-size:12px;   background-color: #FFFFFF;}
.InputUser  {
  width: 170px;  font-family:Verdana; font-size :14px; padding-left:2px; 
  background-color : #FFFFFF; border : 1px solid #CDCECF;
  background-image:url(../../Images/InactiveUser.gif); 
  background-position:right; background-repeat:no-repeat;
}
.inputHighlightedUser {
  width: 170px; font-family:Verdana; font-size:14px; padding-left:2px;
  background-color: #F9FDEE; border: 1px solid #000; 
  background-image:url(../../Images/ActiveUser.gif); 
  background-position:right; background-repeat:no-repeat;
}
.inputHighlightedPass {
  width: 170px; font-family:Verdana; font-size:14px; padding-left:2px;
  background-color: #F9FDEE; border: 1px solid #000; 
  background-image:url(../../Images/ActivePass.gif);  
  background-position:right; background-repeat:no-repeat;
}
.InputPass{
  width: 170px; font-family:Verdana;  font-size:14px;
  padding-left:2px; background-color: #FFFFFF;  border: 1px solid #CDCECF;
  background-image:url(../../Images/InactivePass.gif); 
  background-position:right; background-repeat:no-repeat;
}

    </style>
</head>

<body onload="window.history.forward(1)">
<form id="LoginUI" runat="server">
    <asp:HiddenField id="hdUserList" runat="server" />
    <div id="loginh">
        <div class="loginfl">
            <img src="../../Images/logoright.PNG" id="logoLeft" alt="logoLeft" /></div>
        <div class="loginfr">
                Sanchaya Patra Management System
        </div>
    </div>
            

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    
    
    <table style="width: 100%; height: 88%" border="0" >
        <tr>
            <td style="width: 100%; height: 33%">
                &nbsp;</td>
        </tr>
        <tr>
            <td valign="top" style="width: 100%; height: 33%">
            <asp:UpdatePanel runat="server" ID="upData">
        <ContentTemplate>
                <table border="0" align="center" cellpadding="2">
                    <tr>
                        <td align="right">
                            <a class="text">User Name:</a>
                        </td>
                        <td>
                            <asp:TextBox MaxLength="20" onblur="blurActiveInput(this)" onfocus="highlightActiveInput(this, 'inputHighlightedUser')" CssClass="InputUser" ID="txtUserName" runat="server" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="txtUserName"
                                ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <a class="text">Password:</a>
                        </td>
                        <td>
                            <asp:TextBox MaxLength="28" onblur="blurActiveInput(this)" onfocus="highlightActiveInput(this, 'inputHighlightedPass')" CssClass="InputPass" TextMode="Password" ID="txtPassword" runat="server" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="txtPassword"
                                ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
<%--                    <tr>
                        <td align="right">
                            <a class="text">Division:</a>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlDivision" runat="server" Height="22px" Width="173px" >
                                </asp:DropDownList>
                        </td>
                    </tr>--%>
                    <tr>
                        <td>&nbsp;</td>
                        <td >
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" CssClass="ButtonGreen marginLeft"
                                ValidationGroup="Login1" onclick="LoginButton_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2" style="color: Red;">
                            <div>
                                <asp:ValidationSummary ID="vsApplicant" runat="server" ValidationGroup="Login1" />
                                <a class="errorText"><asp:Label ID="lblMessage" runat="server" ></asp:Label></a>
                             </div>
                       </td>
                    </tr>
                </table>
                </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger EventName="Click" ControlID="LoginButton" />
        </Triggers>
    </asp:UpdatePanel>
                .
            </td>
        </tr>
        <tr>
            <td style="width: 100%; height: 34%">
                &nbsp;</td>
        </tr>
    </table>
</form>    
</body>
</html>



<script language="javascript" type="text/javascript">
    <!--
    /* active input control */
    
    var currentlyActiveInputRef = false;
    var currentlyActiveInputClassName = false;

    function highlightActiveInput(obj , vClass) {
        if (currentlyActiveInputRef) {
            currentlyActiveInputRef.className = currentlyActiveInputClassName;
        }
        currentlyActiveInputClassName = obj.className;
        obj.className = vClass; 
        currentlyActiveInputRef = obj;
    }
    function blurActiveInput(obj) {
        obj.className = currentlyActiveInputClassName;
    }
    //-->
</script>

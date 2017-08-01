<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CasinoSlotMachine.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Image ID="leftReelImage" runat="server" Height="225px" />
            <asp:Image ID="middleReelImage" runat="server" Height="225px" />
            <asp:Image ID="rightReelImage" runat="server" Height="225px" />
            <br />
            <br />
            <br />
            Your Bet:
            <asp:TextBox ID="betTextBox" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="LeverButton" runat="server" OnClick="LeverButton_Click" Text="Pull The Lever!" />
            <br />
            <br />
            <asp:Label ID="resultLabel" runat="server"></asp:Label>
            <br />
            <br />
            Player&#39;s Money: <asp:Label ID="playersMoneyLabel" runat="server"></asp:Label>
            <br />
            <br />
            1 Cherry - x2 Your Bet<br />
            2 Cherrys - x3 Your Bet<br />
            3 Cherrys - x4 Your Bet<br />
            3 7&#39;s - Jackpot - x100 Your Bet<br />
            <br />
            However ... If there&#39;s one BAR you win nothing</div>
    </form>
</body>
</html>

<%@ Page Title="Horario de trabajo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TurnoTrabajoAlta.aspx.cs" Inherits="ClinicaTurnos.Web.TurnoTrabajoAlta" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <a href="TurnosTrabajo.aspx">Volver a horarios de trabajo</a>
    <h1><asp:Literal ID="litTitulo" runat="server" Text="Agregar horario" /></h1>
    <p>Marcá los días de trabajo y completá el horario de cada uno.</p>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="alert" />
    <asp:Panel ID="pnlFormulario" runat="server" DefaultButton="btnGuardar">
        <asp:ValidationSummary ID="resumen" runat="server" CssClass="resumen" HeaderText="Revisá los datos del formulario:" />
        <div class="campo">
            <asp:Label ID="lblNombre" runat="server" AssociatedControlID="txtNombre" Text="Nombre del horario *" />
            <asp:TextBox ID="txtNombre" runat="server" MaxLength="80" />
            <asp:RequiredFieldValidator ID="reqNombre" runat="server" ControlToValidate="txtNombre" ErrorMessage="Completá el nombre del horario." Text="Campo obligatorio." Display="Dynamic" CssClass="validacion" />
        </div>
        <div class="campo">
            <table class="tabla">
                <thead>
                    <tr><th scope="col">Día</th><th scope="col">Trabaja</th><th scope="col">Entrada</th><th scope="col">Salida</th></tr>
                </thead>
                <tbody>
                    <tr><th scope="row">Lunes</th><td><asp:CheckBox ID="chkLunes" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtLunesEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtLunesSalida" runat="server" TextMode="Time" /></td></tr>
                    <tr><th scope="row">Martes</th><td><asp:CheckBox ID="chkMartes" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtMartesEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtMartesSalida" runat="server" TextMode="Time" /></td></tr>
                    <tr><th scope="row">Miércoles</th><td><asp:CheckBox ID="chkMiercoles" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtMiercolesEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtMiercolesSalida" runat="server" TextMode="Time" /></td></tr>
                    <tr><th scope="row">Jueves</th><td><asp:CheckBox ID="chkJueves" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtJuevesEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtJuevesSalida" runat="server" TextMode="Time" /></td></tr>
                    <tr><th scope="row">Viernes</th><td><asp:CheckBox ID="chkViernes" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtViernesEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtViernesSalida" runat="server" TextMode="Time" /></td></tr>
                    <tr><th scope="row">Sábado</th><td><asp:CheckBox ID="chkSabado" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtSabadoEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtSabadoSalida" runat="server" TextMode="Time" /></td></tr>
                    <tr><th scope="row">Domingo</th><td><asp:CheckBox ID="chkDomingo" runat="server" Text="Sí" /></td><td><asp:TextBox ID="txtDomingoEntrada" runat="server" TextMode="Time" /></td><td><asp:TextBox ID="txtDomingoSalida" runat="server" TextMode="Time" /></td></tr>
                </tbody>
            </table>
        </div>
        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
            <a href="TurnosTrabajo.aspx">Cancelar</a>
        </div>
    </asp:Panel>
</asp:Content>

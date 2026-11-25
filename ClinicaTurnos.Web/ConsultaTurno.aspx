<%@ Page Title="Consulta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaTurno.aspx.cs" Inherits="ClinicaTurnos.Web.ConsultaTurno" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <a href="MiAgenda.aspx">Volver a mi agenda</a>
    <h1>Consulta</h1>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <asp:Panel ID="pnlConsulta" runat="server" DefaultButton="btnGuardar">
        <p><asp:Label ID="lblPaciente" runat="server" /></p>
        <p><asp:Label ID="lblTurno" runat="server" /></p>
        <p><asp:Label ID="lblMotivo" runat="server" /></p>
        <p><asp:HyperLink ID="lnkHistorial" runat="server" Text="Historial del paciente" /></p>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtObservacion" Text="Observación de diagnóstico" />
            <asp:TextBox ID="txtObservacion" runat="server" TextMode="MultiLine" Rows="5" maxlength="1000" />
        </div>
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar observación" OnClick="btnGuardar_Click" />
        <asp:Button ID="btnCerrar" runat="server" Text="Guardar y cerrar" OnClick="btnCerrar_Click"
            OnClientClick="return confirm('¿Cerrar esta consulta?');" />
        <asp:Button ID="btnAusente" runat="server" Text="Marcar No asistió" OnClick="btnAusente_Click"
            OnClientClick="return confirm('¿Marcar que el paciente no asistió?');" />
    </asp:Panel>
</asp:Content>

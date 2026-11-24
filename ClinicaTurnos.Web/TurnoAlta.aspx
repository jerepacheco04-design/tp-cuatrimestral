<%@ Page Title="Agregar turno" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TurnoAlta.aspx.cs" Inherits="ClinicaTurnos.Web.TurnoAlta" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <a href="Turnos.aspx">Volver al listado</a>
    <h1><asp:Literal ID="litTitulo" runat="server" Text="Agregar turno" /></h1>
    <asp:Label ID="lblTurnoActual" runat="server" />
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="alert" />
    <asp:Panel ID="pnlFormulario" runat="server" DefaultButton="btnGuardar">
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="ddlPaciente" Text="Paciente" />
            <asp:DropDownList ID="ddlPaciente" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPaciente_SelectedIndexChanged" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="ddlEspecialidad" Text="Especialidad" />
            <asp:DropDownList ID="ddlEspecialidad" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEspecialidad_SelectedIndexChanged" />
        </div>
        <asp:Panel ID="pnlSugerencias" runat="server" CssClass="campo" Visible="false">
            <asp:Label runat="server" AssociatedControlID="ddlSugerencia" Text="Horarios sugeridos" />
            <asp:DropDownList ID="ddlSugerencia" runat="server" />
            <asp:Button ID="btnUsarSugerencia" runat="server" Text="Usar sugerencia" CausesValidation="false" OnClick="btnUsarSugerencia_Click" />
        </asp:Panel>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="ddlMedico" Text="Médico" />
            <asp:DropDownList ID="ddlMedico" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ActualizarHorarios" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtFecha" Text="Fecha" />
            <asp:TextBox ID="txtFecha" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="ActualizarHorarios" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="ddlHora" Text="Hora de inicio" />
            <asp:DropDownList ID="ddlHora" runat="server" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtMotivo" Text="Motivo de consulta" />
            <asp:TextBox ID="txtMotivo" runat="server" TextMode="MultiLine" Rows="3" maxlength="500" />
        </div>
        <div class="acciones">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
            <a href="Turnos.aspx">Cancelar</a>
        </div>
    </asp:Panel>
</asp:Content>

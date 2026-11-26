<%@ Page Title="Turnos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Turnos.aspx.cs" Inherits="ClinicaTurnos.Web.Turnos" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Turnos de pacientes</h1>
    <p><a href="TurnoAlta.aspx">Agregar turno</a></p>
    <asp:Panel ID="pnlFiltro" runat="server" DefaultButton="btnBuscar">
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="txtFecha" Text="Fecha" />
            <asp:TextBox ID="txtFecha" runat="server" TextMode="Date" />
        </div>
        <div class="campo">
            <asp:Label runat="server" AssociatedControlID="ddlEstado" Text="Estado" />
            <asp:DropDownList ID="ddlEstado" runat="server">
                <asp:ListItem Text="Todos" Value="" />
                <asp:ListItem Text="Nuevo" Value="Nuevo" />
                <asp:ListItem Text="Reprogramado" Value="Reprogramado" />
                <asp:ListItem Text="Cancelado" Value="Cancelado" />
                <asp:ListItem Text="No asistio" Value="NoAsistio" />
                <asp:ListItem Text="Cerrado" Value="Cerrado" />
            </asp:DropDownList>
        </div>
        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="Filtrar_Click" />
        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" OnClick="Filtrar_Click" />
    </asp:Panel>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="Todavía no hay turnos cargados." OnRowCommand="grilla_RowCommand" OnRowDataBound="grilla_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Número" />
                <asp:BoundField DataField="FechaHora" HeaderText="Fecha y hora" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Paciente">
                    <ItemTemplate><%#: Eval("Paciente.Apellido") %>, <%#: Eval("Paciente.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Médico">
                    <ItemTemplate><%#: Eval("Medico.Apellido") %>, <%#: Eval("Medico.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Especialidad">
                    <ItemTemplate><%#: Eval("Especialidad.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EstadoVisible" HeaderText="Estado" />
                <asp:BoundField DataField="MotivoConsulta" HeaderText="Motivo" />
                <asp:BoundField DataField="CargadoPor" HeaderText="Cargado por" NullDisplayText="—" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:HyperLink ID="linkReprogramar" runat="server" Text="Reprogramar" NavigateUrl='<%# "TurnoAlta.aspx?id=" + Eval("Id") %>' />
                        <asp:LinkButton ID="btnCancelar" runat="server" Text="Cancelar" CommandName="CancelarTurno" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('¿Confirmás la cancelación de este turno?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

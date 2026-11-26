<%@ Page Title="Turnos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Turnos.aspx.cs" Inherits="ClinicaTurnos.Web.Turnos" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1><asp:Label ID="lblTitulo" runat="server" Text="Turnos de pacientes" /></h1>
    <p><asp:HyperLink ID="linkAgregar" runat="server" NavigateUrl="~/TurnoAlta.aspx" Text="Agregar turno" /></p>
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
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate><%#: NombreEstado(Eval("Estado")) %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="MotivoConsulta" HeaderText="Motivo" />
                <asp:TemplateField HeaderText="Diagnóstico">
                    <ItemTemplate>
                        <asp:Label ID="lblDiagnostico" runat="server" Text='<%#: Eval("ObservacionesDiagnostico") %>' />
                        <asp:TextBox ID="txtDiagnostico" runat="server" Text='<%# Eval("ObservacionesDiagnostico") %>' TextMode="MultiLine" Rows="2" MaxLength="1000" />
                        <asp:LinkButton ID="btnGuardarDiagnostico" runat="server" Text="Guardar diagnóstico" CommandName="GuardarDiagnostico" CommandArgument='<%# Eval("Id") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
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

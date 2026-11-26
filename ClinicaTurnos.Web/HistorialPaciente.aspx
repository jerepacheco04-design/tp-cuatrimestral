<%@ Page Title="Historial del paciente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistorialPaciente.aspx.cs" Inherits="ClinicaTurnos.Web.HistorialPaciente" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <asp:HyperLink ID="lnkVolver" runat="server" />
    <h1>Historial del paciente</h1>
    <p><asp:Label ID="lblPaciente" runat="server" /></p>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="No hay turnos anteriores.">
            <Columns>
                <asp:BoundField DataField="FechaHora" HeaderText="Fecha y hora" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Médico">
                    <ItemTemplate><%#: Eval("Medico.Apellido") %>, <%#: Eval("Medico.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Especialidad">
                    <ItemTemplate><%#: Eval("Especialidad.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EstadoVisible" HeaderText="Estado" />
                <asp:BoundField DataField="ObservacionesDiagnostico" HeaderText="Observación de diagnóstico" ItemStyle-CssClass="observacion" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

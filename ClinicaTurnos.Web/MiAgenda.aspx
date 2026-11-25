<%@ Page Title="Mi agenda" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MiAgenda.aspx.cs" Inherits="ClinicaTurnos.Web.MiAgenda" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Mi agenda</h1>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" CssClass="tabla" AutoGenerateColumns="false" EmptyDataText="No tenés turnos asignados.">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Número" />
                <asp:BoundField DataField="FechaHora" HeaderText="Fecha y hora" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Paciente">
                    <ItemTemplate><%#: Eval("Paciente.Apellido") %>, <%#: Eval("Paciente.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Especialidad">
                    <ItemTemplate><%#: Eval("Especialidad.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EstadoVisible" HeaderText="Estado" />
                <asp:BoundField DataField="MotivoConsulta" HeaderText="Motivo" />
                <asp:TemplateField HeaderText="Consulta">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" Text="Ver consulta" NavigateUrl='<%# "ConsultaTurno.aspx?id=" + Eval("Id") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

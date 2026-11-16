<%@ Page Title="Especialidades" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Especialidades.aspx.cs" Inherits="ClinicaTurnos.Web.Especialidades" %>
<asp:Content ID="contenido" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h1>Especialidades</h1>
    <p><a href="EspecialidadAlta.aspx">Agregar especialidad</a></p>
    <asp:Label ID="lblMensaje" runat="server" CssClass="aviso" role="status" />
    <div class="tabla-contenedor">
        <asp:GridView ID="grilla" runat="server" AutoGenerateColumns="false" CssClass="tabla" EmptyDataText="No hay registros cargados.">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Especialidad" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

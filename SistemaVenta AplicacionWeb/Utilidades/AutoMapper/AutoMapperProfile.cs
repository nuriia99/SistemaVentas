using SistemaVenta_AplicacionWeb.Models.ViewModel;
using SistemaVenta.Entity;
using System.Globalization;
using AutoMapper;

namespace SistemaVenta_AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Rol, VMRol>().ReverseMap();

            CreateMap<Usuario, VMUsuario>()
                .ForMember(destino => destino.EsActivo, opciones => opciones.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                .ForMember(destino => destino.NombreRol, opciones => opciones.MapFrom(origen => origen.IdRolNavigation.Descripcion));
            CreateMap<VMUsuario, Usuario>()
                .ForMember(destino => destino.EsActivo, opciones => opciones.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdRolNavigation, opciones => opciones.Ignore());

            CreateMap<Negocio, VMNegocio>()
                .ForMember(destino => destino.PorcentajeImpuesto, opciones => opciones.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-ES"))));
            CreateMap<VMNegocio, Negocio>()
                .ForMember(destino => destino.PorcentajeImpuesto, opciones => opciones.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-ES"))));

            CreateMap<Categoria, VMCategoria>()
                .ForMember(destino => destino.EsActivo, opciones => opciones.MapFrom(origen => origen.EsActivo == true ? 1 : 0));
            CreateMap<VMCategoria, Categoria>()
                .ForMember(destino => destino.EsActivo, opciones => opciones.MapFrom(origen => origen.EsActivo == 1 ? true : false));

            CreateMap<Producto, VMProducto>()
                .ForMember(destino => destino.EsActivo, opciones => opciones.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                .ForMember(destino => destino.NombreCategoria, opciones => opciones.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion))
                .ForMember(destino => destino.Precio, opciones => opciones.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-ES"))));
            CreateMap<VMProducto, Producto>()
                .ForMember(destino => destino.EsActivo, opciones => opciones.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdCategoriaNavigation, opciones => opciones.Ignore())
                .ForMember(destino => destino.Precio, opciones => opciones.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-ES"))));


            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();

            CreateMap<Venta, VMVenta>()
                .ForMember(destino => destino.TipoDocumentoVenta, opciones => opciones.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion))
                .ForMember(destino => destino.Usuario, opciones => opciones.MapFrom(origen => origen.IdUsuarioNavigation.Nombre))
                .ForMember(destino => destino.SubTotal, opciones => opciones.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.ImpuestoTotal, opciones => opciones.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.Total, opciones => opciones.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.FechaRegistro, opciones => opciones.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy")));
            CreateMap<VMVenta, Venta>()
                .ForMember(destino => destino.SubTotal, opciones => opciones.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.ImpuestoTotal, opciones => opciones.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.Total, opciones => opciones.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-ES"))));

            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(destino => destino.Precio, opciones => opciones.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.Total, opciones => opciones.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-ES"))));
            CreateMap<VMDetalleVenta, DetalleVenta>()
                .ForMember(destino => destino.Precio, opciones => opciones.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-ES"))));
                

            CreateMap<DetalleVenta, VMReporteVenta>()
                .ForMember(destino => destino.FechaRegistro, opciones => opciones.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))
                .ForMember(destino => destino.NumeroVenta, opciones => opciones.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta))
                .ForMember(destino => destino.TipoDocumento, opciones => opciones.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion))
                .ForMember(destino => destino.DocumentoCliente, opciones => opciones.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente))
                .ForMember(destino => destino.NombreCliente, opciones => opciones.MapFrom(origen => origen.IdVentaNavigation.NombreCliente))
                .ForMember(destino => destino.SubTotalVenta, opciones => opciones.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.ImpuestoTotalVenta, opciones => opciones.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.TotalVenta, opciones => opciones.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.Producto, opciones => opciones.MapFrom(origen => origen.DescripcionProducto))
                .ForMember(destino => destino.Precio, opciones => opciones.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-ES"))))
                .ForMember(destino => destino.Total, opciones => opciones.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-ES"))));
            
            
            CreateMap<Menu, VMMenu>()
                .ForMember(destino => destino.SubMenus, opciones => opciones.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
        }
    }
}

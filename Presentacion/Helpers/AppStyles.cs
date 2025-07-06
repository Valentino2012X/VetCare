using System.Drawing;

namespace Presentacion.Helpers
{
    public static class AppStyles
    {
        // =============================================
        // PALETA DE COLORES
        // =============================================
        public static class Colores
        {
            // Paleta principal
            public static readonly Color Fondo = Color.FromArgb(18, 187, 190);
            public static readonly Color Borde = Color.FromArgb(0, 150, 153);
            public static readonly Color Naranja = Color.FromArgb(249, 153, 53);

            // Colores secundarios
            public static readonly Color Exito = Color.FromArgb(76, 175, 80);
            public static readonly Color Error = Color.FromArgb(244, 67, 54);
            public static readonly Color Advertencia = Color.FromArgb(255, 193, 7);

            // Textos
            public static readonly Color TextoPrimario = Color.White;
            public static readonly Color TextoSecundario = Color.FromArgb(64, 64, 64);
        }

        // =============================================
        // TIPOGRAFÍAS
        // =============================================
        public static class Fuentes
        {
            public static readonly Font Titulo = new Font("Segoe UI", 14, FontStyle.Bold);
            public static readonly Font Subtitulo = new Font("Segoe UI", 12, FontStyle.Bold); // Cambiado de SemiBold a Bold
            public static readonly Font Normal = new Font("Segoe UI", 10, FontStyle.Bold);
            public static readonly Font Pequeña = new Font("Segoe UI", 8);
        }

        // =============================================
        // TAMAÑOS ESTÁNDAR
        // =============================================
        public static class Tamanos
        {
            public static readonly Size BotonNormal = new Size(120, 40);
            public static readonly Size BotonGrande = new Size(180, 50);
            public static readonly int RadioBordes = 15;
        }
    }
}
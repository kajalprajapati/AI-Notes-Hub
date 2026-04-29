using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AINotesHub.WPF.Helpers
{
    public static class NoteColorPalette
    {
        public static IReadOnlyList<Brush> Default { get; } =
        new List<Brush>
        {
            //// Whites & Neutrals
            //BrushFrom("#FFFFFF"), // White
            //BrushFrom("#FAFAFA"), // Off White
            //BrushFrom("#F5F5F5"), // Light Gray
            //BrushFrom("#ECEFF1"), // Cool Gray

            //// Yellows & Creams (Best for notes)
            //BrushFrom("#FFFDE7"), // Paper Yellow
            //BrushFrom("#FFF9C4"), // Soft Yellow
            //BrushFrom("#FFF8E1"), // Cream
            //BrushFrom("#FFF3E0"), // Light Peach

            //// Blues (Calm & Focus)
            //BrushFrom("#E3F2FD"), // Soft Blue
            //BrushFrom("#E1F5FE"), // Sky Blue
            //BrushFrom("#E0F7FA"), // Cyan Light
            //BrushFrom("#E8F0FE"), // Google Blue Light

            //// Greens (Relaxing)
            //BrushFrom("#E8F5E9"), // Soft Green
            //BrushFrom("#F1F8E9"), // Mint Green
            //BrushFrom("#E6F4EA"), // Success Light
            //BrushFrom("#DCEDC8"), // Pistachio

            //// Pink / Rose (Soft only)
            //BrushFrom("#FCE4EC"), // Blush Pink
            //BrushFrom("#FFF1F1"), // Baby Pink
            //BrushFrom("#FADADD"), // Rose Light

            //// Purple / Lavender
            //BrushFrom("#F3E5F5"), // Lavender
            //BrushFrom("#EDE7F6"), // Indigo Light
            //BrushFrom("#E1BEE7"), // Lilac

            //// Orange / Peach
            //BrushFrom("#FFE0B2"), // Soft Orange
            //BrushFrom("#FFCCBC"), // Peach
            ////////////////////////////////////////////////////////
            ///


            // =======================
    //// Whites & Neutrals
    //// =======================
    //BrushFrom("#FFFFFF"), // White
    //BrushFrom("#FAFAFA"), // Off White
    //BrushFrom("#F5F5F5"), // Light Gray
    //BrushFrom("#ECEFF1"), // Cool Gray
    //BrushFrom("#E0E0E0"), // Soft Gray
    //BrushFrom("#CFD8DC"), // Blue Gray Light
    //BrushFrom("#B0BEC5"), // Blue Gray Medium
    //BrushFrom("#90A4AE"), // Blue Gray Dark (still soft)
    //// =======================


//PearlescentShimmerFrom("#FFFFFF"), // White highlighter shimmer
//PearlescentShimmerFrom("#F5F5F5"), // Soft pearl
//PearlescentShimmerFrom("#E3F2FD"), // Blue shift shimmer
//PearlescentShimmerFrom("#FCE4EC"), // Pink pearl shimmer
//PearlescentShimmerFrom("#E1BEE7"), // Lavender shimmer

//            PearlescentFrom("#FFFFFF"),
//PearlescentFrom("#FFF9C4"),
//PearlescentFrom("#E3F2FD"),
//PearlescentFrom("#E8F5E9"),
//PearlescentFrom("#FCE4EC"),
//PearlescentFrom("#E1BEE7"),
//PearlescentFrom("#FFE0B2"),

   
    //// =======================
    //// Yellows & Creams
    //// =======================
    //BrushFrom("#FFFDE7"), // Paper Yellow
    //BrushFrom("#FFF9C4"), // Soft Yellow
    //BrushFrom("#FFF8E1"), // Cream
    //BrushFrom("#FFF3E0"), // Light Peach
    //BrushFrom("#FFE082"), // Warm Yellow
    //BrushFrom("#FFD54F"), // Golden Soft
    //BrushFrom("#FFCA28"), // Yellow Medium
    //BrushFrom("#FFB300"), // Yellow Dark Soft

    //// =======================
    //// Blues (Calm & Focus)
    //// =======================
    //BrushFrom("#E3F2FD"), // Soft Blue
    //BrushFrom("#E1F5FE"), // Sky Blue
    //BrushFrom("#E0F7FA"), // Cyan Light
    //BrushFrom("#E8F0FE"), // Google Blue Light
    //BrushFrom("#BBDEFB"), // Blue Light
    //BrushFrom("#90CAF9"), // Blue Medium
    //BrushFrom("#64B5F6"), // Blue Soft Dark
    //BrushFrom("#42A5F5"), // Blue Dark (UI friendly)

    //// =======================
    //// Greens (Relaxing)
    //// =======================
    //BrushFrom("#E8F5E9"), // Soft Green
    //BrushFrom("#F1F8E9"), // Mint Green
    //BrushFrom("#E6F4EA"), // Success Light
    //BrushFrom("#DCEDC8"), // Pistachio
    //BrushFrom("#C8E6C9"), // Green Light
    //BrushFrom("#A5D6A7"), // Green Medium
    //BrushFrom("#81C784"), // Green Soft Dark
    //BrushFrom("#66BB6A"), // Green Dark Soft

    //// =======================
    //// Pink / Rose
    //// =======================
    //BrushFrom("#FCE4EC"), // Blush Pink
    //BrushFrom("#FFF1F1"), // Baby Pink
    //BrushFrom("#FADADD"), // Rose Light
    //BrushFrom("#F8BBD0"), // Pink Light
    //BrushFrom("#F48FB1"), // Pink Medium
    //BrushFrom("#EC407A"), // Rose Dark Soft
    //BrushFrom("#D81B60"), // Pink Dark Soft

    //// =======================
    //// Purple / Lavender
    //// =======================
    //BrushFrom("#F3E5F5"), // Lavender
    //BrushFrom("#EDE7F6"), // Indigo Light
    //BrushFrom("#E1BEE7"), // Lilac
    //BrushFrom("#CE93D8"), // Purple Light
    //BrushFrom("#BA68C8"), // Purple Medium
    //BrushFrom("#9575CD"), // Indigo Medium
    //BrushFrom("#7E57C2"), // Purple Dark Soft

    //// =======================
    //// Orange / Peach
    //// =======================
    //BrushFrom("#FFE0B2"), // Soft Orange
    //BrushFrom("#FFCCBC"), // Peach
    //BrushFrom("#FFECB3"), // Light Amber
    //BrushFrom("#FFB74D"), // Orange Light
    //BrushFrom("#FFA726"), // Orange Medium
    //BrushFrom("#FB8C00"), // Orange Dark Soft
    //BrushFrom("#F57C00"), // Deep Orange Soft

    //// =======================
    //// Teal / Aqua (New Family)
    //// =======================
    //BrushFrom("#E0F2F1"), // Teal Light
    //BrushFrom("#B2DFDB"), // Teal Soft
    //BrushFrom("#80CBC4"), // Teal Medium
    //BrushFrom("#4DB6AC"), // Teal Dark Soft

    //// =======================
    //// Brown / Sand (New Family)
    //// =======================
    //BrushFrom("#EFEBE9"), // Sand Light
    //BrushFrom("#D7CCC8"), // Beige
    //BrushFrom("#BCAAA4"), // Brown Light
    //BrushFrom("#A1887F"), // Brown Soft Dark

// =======================
    // Whites & Neutrals
    // =======================
    BrushFrom("#FFFFFF"), // Pure White
    BrushFrom("#FAFAFA"), // Off White
    BrushFrom("#F5F5F5"), // Light Gray
    BrushFrom("#ECEFF1"), // Cool Gray
    BrushFrom("#E0E0E0"), // Soft Gray
    BrushFrom("#CFD8DC"), // Blue Gray Light
    BrushFrom("#B0BEC5"), // Blue Gray Medium
    BrushFrom("#90A4AE"), // Blue Gray Dark
    BrushFrom("#BDBDBD"), // Silver Gray
    BrushFrom("#424242"), // Dark Gray
    BrushFrom("#212121"), // Almost Black

    // =======================
    // Yellows & Creams (Best for notes)
    // =======================
    BrushFrom("#FFFDE7"), // Paper Yellow
    BrushFrom("#FFF9C4"), // Soft Yellow
    BrushFrom("#FFF8E1"), // Cream
    BrushFrom("#FFF3E0"), // Light Peach
    BrushFrom("#FFE082"), // Warm Yellow
    BrushFrom("#FFD54F"), // Golden Soft
    BrushFrom("#FFCA28"), // Yellow Medium

    // =======================
    // Blues (Calm & Focus)
    // =======================
    BrushFrom("#E3F2FD"), // Soft Blue
    BrushFrom("#E1F5FE"), // Sky Blue
    BrushFrom("#E8F0FE"), // Google Blue Light
    BrushFrom("#BBDEFB"), // Blue Light
    BrushFrom("#90CAF9"), // Blue Medium
    BrushFrom("#64B5F6"), // Blue Soft Dark
    BrushFrom("#42A5F5"), // Blue Dark
    BrushFrom("#1976D2"), // Deep Blue

    // =======================
    // Greens (Relaxing / Success)
    // =======================
    BrushFrom("#E8F5E9"), // Soft Green
    BrushFrom("#F1F8E9"), // Mint Green
    BrushFrom("#E6F4EA"), // Success Light
    BrushFrom("#DCEDC8"), // Pistachio
    BrushFrom("#C8E6C9"), // Green Light
    BrushFrom("#A5D6A7"), // Green Medium
    BrushFrom("#81C784"), // Green Soft Dark
    BrushFrom("#66BB6A"), // Green Dark Soft

    // =======================
    // Teal / Aqua (Fresh)
    // =======================
    BrushFrom("#E0F2F1"), // Teal Very Light
    BrushFrom("#B2DFDB"), // Teal Light
    BrushFrom("#80CBC4"), // Teal Medium
    BrushFrom("#4DB6AC"), // Teal Dark

    // =======================
    // Pink / Rose (Soft)
    // =======================
    BrushFrom("#FFF1F1"), // Baby Pink
    BrushFrom("#FCE4EC"), // Blush Pink
    BrushFrom("#FADADD"), // Rose Light
    BrushFrom("#F8BBD0"), // Pink Light
    BrushFrom("#F48FB1"), // Pink Medium
    BrushFrom("#EC407A"), // Rose Dark
    BrushFrom("#D81B60"), // Magenta Dark

    // =======================
    // Purple / Lavender
    // =======================
    BrushFrom("#F3E5F5"), // Lavender
    BrushFrom("#EDE7F6"), // Indigo Light
    BrushFrom("#E1BEE7"), // Lilac
    BrushFrom("#CE93D8"), // Purple Light
    BrushFrom("#BA68C8"), // Purple Medium
    BrushFrom("#9575CD"), // Indigo Medium
    BrushFrom("#7E57C2"), // Purple Dark
    BrushFrom("#8E24AA"), // Deep Violet

    // =======================
    // Orange / Peach
    // =======================
    BrushFrom("#FFE0B2"), // Soft Orange
    BrushFrom("#FFCCBC"), // Peach
    BrushFrom("#FFECB3"), // Light Amber
    BrushFrom("#FFB74D"), // Orange Light
    BrushFrom("#FFA726"), // Orange Medium
    BrushFrom("#FB8C00"), // Orange Dark Soft
    BrushFrom("#F57C00"), // Deep Orange

    // =======================
    // Accent / Pop Colors
    // =======================
    BrushFrom("#EF5350"), // Red Accent
    BrushFrom("#FF7043"), // Sunset Orange
    BrushFrom("#FF80AB"), // Pink Accent
    BrushFrom("#7E57C2"), // Purple Accent
        };

        private static Brush BrushFrom(string hex)
        {
            var brush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));

            brush.Freeze(); // performance + safety
            return brush;
        }

        // ✨ ADD THIS HERE
        private static Brush ShimmerFrom(string hex)
        {
            var baseColor = (Color)ColorConverter.ConvertFromString(hex);

            return new LinearGradientBrush(
                new GradientStopCollection
                {
                new GradientStop(Colors.White, 0.0),
                new GradientStop(baseColor, 0.3),
                new GradientStop(Colors.WhiteSmoke, 0.5),
                new GradientStop(baseColor, 0.7),
                new GradientStop(Colors.White, 1.0),
                },
                new Point(0, 0),
                new Point(1, 1)
            );
        }

        // ✨ ADD THIS TOO
        private static Brush SoftGlitterFrom(string hex)
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);

            return new RadialGradientBrush(
                new GradientStopCollection
                {
                new GradientStop(Colors.White, 0.0),
                new GradientStop(color, 0.4),
                new GradientStop(Colors.WhiteSmoke, 0.6),
                new GradientStop(color, 1.0)
                }
            )
            {
                RadiusX = 0.9,
                RadiusY = 0.9
            };
        }


        private static Brush PearlescentFrom(string hex)
        {
            var baseColor = (Color)ColorConverter.ConvertFromString(hex);

            var baseBrush = new SolidColorBrush(baseColor);

            var pearlOverlay = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops = new GradientStopCollection
        {
            new GradientStop(Color.FromArgb(180, 255, 255, 255), 0.0),
            new GradientStop(Color.FromArgb(120, 200, 230, 255), 0.25), // blue sheen
            new GradientStop(Color.FromArgb(120, 235, 200, 255), 0.45), // violet sheen
            new GradientStop(Color.FromArgb(120, 255, 230, 200), 0.65), // warm sheen
            new GradientStop(Color.FromArgb(180, 255, 255, 255), 1.0),
        }
            };

            var drawing = new DrawingGroup();
            drawing.Children.Add(
                new GeometryDrawing(baseBrush, null, new RectangleGeometry(new Rect(0, 0, 1, 1))));
            drawing.Children.Add(
                new GeometryDrawing(pearlOverlay, null, new RectangleGeometry(new Rect(0, 0, 1, 1))));

            return new DrawingBrush
            {
                Drawing = drawing,
                Stretch = Stretch.Fill
            };
        }


    }
}

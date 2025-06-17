namespace EGFramework.Code{
    public class EGSvgGenerator : IGenerateToolsInterface
    {
        public string SvgHeader { get ; private set;}
        public const string SvgFooter = "</svg>";

        public int Width { get; set; }
        public int Height { get; set; }
        public EGSvgViewBox ViewBox { get; set; }

        public EGSvgGenerator(int width, int height, EGSvgViewBox viewBox)
        {
            Width = width;
            Height = height;
            ViewBox = viewBox;
            SvgHeader = $"<svg width=\"{width}\" height=\"{height}\" viewBox=\"{viewBox.X} {viewBox.Y} {viewBox.Width} {viewBox.Height}\" xmlns=\"http://www.w3.org/2000/svg\">";
        }

        public void DrawCircle(float x, float y, float radius, string color)
        {
            // Implementation for drawing a circle in SVG format
            string svgCircle = $"<circle cx=\"{x}\" cy=\"{y}\" r=\"{radius}\" fill=\"{color}\" />";
        }
        public void DrawEllipse(float cx, float cy, float rx, float ry, string color)
        {
            // Implementation for drawing an ellipse in SVG format
            string svgEllipse = $"<ellipse cx=\"{cx}\" cy=\"{cy}\" rx=\"{rx}\" ry=\"{ry}\" fill=\"{color}\" />";
        }
        public void DrawPolygon(float[] points, string color)
        {
            // Implementation for drawing a polygon in SVG format
            string svgPolygon = "<polygon points=\"";
            for (int i = 0; i < points.Length; i += 2)
            {
                svgPolygon += $"{points[i]},{points[i + 1]} ";
            }
            svgPolygon += $"\" fill=\"{color}\" />";
        }
        public void DrawPolyline(float[] points, string color)
        {
            // Implementation for drawing a polyline in SVG format
            string svgPolyline = "<polyline points=\"";
            for (int i = 0; i < points.Length; i += 2)
            {
                svgPolyline += $"{points[i]},{points[i + 1]} ";
            }
            svgPolyline += $"\" stroke=\"{color}\" fill=\"none\" />";
        }
        public void DrawPath(string d, string color)
        {
            // Implementation for drawing a path in SVG format
            string svgPath = $"<path d=\"{d}\" fill=\"{color}\" />";
        }
        public void DrawRectangle(float x, float y, float width, float height, string color)
        {
            // Implementation for drawing a rectangle in SVG format
            string svgRectangle = $"<rect x=\"{x}\" y=\"{y}\" width=\"{width}\" height=\"{height}\" fill=\"{color}\" />";
        }
        public void DrawLine(float x1, float y1, float x2, float y2, string color)
        {
            // Implementation for drawing a line in SVG format
            string svgLine = $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" stroke=\"{color}\" />";
        }
        public void DrawText(float x, float y, string text, string color)
        {
            // Implementation for drawing text in SVG format
            string svgText = $"<text x=\"{x}\" y=\"{y}\" fill=\"{color}\">{text}</text>";
        }


        public string GenerateCode<T>()
        {
            return typeof(T).Name;
        }
    }
    public struct EGSvgViewBox {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public EGSvgViewBox(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
namespace Uxcheckmate_Main.Models
{
    public class HtmlElement
    {
        public string Tag { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string? Text { get; set; }
        public double Area => Width * Height;
        public double Density => string.IsNullOrWhiteSpace(Text) || Area == 0 ? 0 : Text.Length / Area;
    }
}
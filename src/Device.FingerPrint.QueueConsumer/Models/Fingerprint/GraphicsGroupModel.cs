using System.Collections.Generic;

namespace Device.FingerPrint.QueueConsumer.Models.Fingerprint
{
    public class GraphicsGroupModel
    {
        public CanvasModel? Canvas { get; set; }
        public WebGLBasicsModel? WebglBasics { get; set; }
        public WebGLExtensionsModel? WebglExtensions { get; set; }
        public FontsModel? Fonts { get; set; }
        public FontPreferencesModel? FontPreferences { get; set; }
        public ScreenResolutionModel? ScreenResolution { get; set; }
        public ScreenFrameModel? ScreenFrame { get; set; }
        public ColorDepthModel? ColorDepth { get; set; }
        public ColorGamutModel? ColorGamut { get; set; }
    }

    public class CanvasModel
    {
        public string? Fingerprint { get; set; }
        public string? Status { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? TextBaseline { get; set; }
        public string? Font { get; set; }
        public string? FillStyle { get; set; }
        public string? StrokeStyle { get; set; }
        public int LineWidth { get; set; }
        public string? LineCap { get; set; }
        public string? LineJoin { get; set; }
        public int MiterLimit { get; set; }
        public int ShadowOffsetX { get; set; }
        public int ShadowOffsetY { get; set; }
        public int ShadowBlur { get; set; }
        public string? ShadowColor { get; set; }
        public int GlobalAlpha { get; set; }
        public string? GlobalCompositeOperation { get; set; }
    }

    public class WebGLBasicsModel
    {
        public string? Vendor { get; set; }
        public string? Renderer { get; set; }
        public string? Version { get; set; }
        public string? ShadingLanguageVersion { get; set; }
        public string? UnmaskedVendor { get; set; }
        public string? UnmaskedRenderer { get; set; }
        public int MaxTextureSize { get; set; }
        public int[] MaxViewportDims { get; set; } = new int[2];
        public int MaxRenderbufferSize { get; set; }
        public int[] AliasedLineWidthRange { get; set; } = new int[2];
        public int[] AliasedPointSizeRange { get; set; } = new int[2];
        public int MaxAnisotropy { get; set; }
    }

    public class WebGLExtensionsModel
    {
        public List<string> Extensions { get; set; } = new List<string>();
        public List<string> SupportedExtensions { get; set; } = new List<string>();
        public List<string> CompressedTextureFormats { get; set; } = new List<string>();
        public int MaxDrawBuffers { get; set; }
        public int MaxColorAttachments { get; set; }
        public int MaxVertexAttribs { get; set; }
        public int MaxVertexUniformVectors { get; set; }
        public int MaxFragmentUniformVectors { get; set; }
        public int MaxVaryingVectors { get; set; }
    }

    public class FontsModel
    {
        public List<string> AvailableFonts { get; set; } = new List<string>();
        public List<string> FontList { get; set; } = new List<string>();
        public Dictionary<string, FontMetricsModel> FontMetrics { get; set; } = new Dictionary<string, FontMetricsModel>();
        public FontDetectionModel? FontDetection { get; set; }
        public string? NoiseType { get; set; } // "ok" | "blocked" | "spoofed" | "randomized"
        public int Entropy { get; set; }
    }

    public class FontMetricsModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Baseline { get; set; }
        public int Ascent { get; set; }
        public int Descent { get; set; }
    }

    public class FontDetectionModel
    {
        public List<string> DetectedFonts { get; set; } = new List<string>();
        public List<string> FallbackFonts { get; set; } = new List<string>();
        public List<string> SystemFonts { get; set; } = new List<string>();
    }

    public class FontPreferencesModel
    {
        public int Default { get; set; }
        public int Apple { get; set; }
        public int Serif { get; set; }
        public int Sans { get; set; }
        public int Mono { get; set; }
        public int Min { get; set; }
        public int System { get; set; }
    }

    public class ScreenResolutionModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int AvailWidth { get; set; }
        public int AvailHeight { get; set; }
        public decimal PixelRatio { get; set; }
        public decimal DevicePixelRatio { get; set; }
        public decimal LogicalPixelRatio { get; set; }
    }

    public class ScreenFrameModel
    {
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameDepth { get; set; }
        public int FrameColorDepth { get; set; }
        public int FramePixelDepth { get; set; }
        public string? Orientation { get; set; }
        public int OrientationAngle { get; set; }
    }

    public class ColorDepthModel
    {
        public int ColorDepth { get; set; }
        public int PixelDepth { get; set; }
        public int BitsPerChannel { get; set; }
        public int TotalBits { get; set; }
        public bool HasAlpha { get; set; }
        public bool HasTransparency { get; set; }
    }

    public class ColorGamutModel
    {
        public string? ColorGamut { get; set; }
        public List<string> SupportedColorSpaces { get; set; } = new List<string>();
        public bool P3Support { get; set; }
        public bool Rec2020Support { get; set; }
        public bool SrgbSupport { get; set; }
        public bool DisplayP3Support { get; set; }
    }
}

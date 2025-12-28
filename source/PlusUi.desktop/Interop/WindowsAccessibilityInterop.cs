using System.Runtime.InteropServices;

namespace PlusUi.desktop.Interop;

internal static class WindowsAccessibilityInterop
{
    internal const uint SPI_GETHIGHCONTRAST = 0x0042;
    internal const uint SPI_GETCLIENTAREAANIMATION = 0x1042;
    internal const uint HCF_HIGHCONTRASTON = 0x00000001;

    [StructLayout(LayoutKind.Sequential)]
    internal struct HIGHCONTRAST
    {
        public uint cbSize;
        public uint dwFlags;
        public IntPtr lpszDefaultScheme;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref HIGHCONTRAST pvParam, uint fWinIni);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref bool pvParam, uint fWinIni);
}

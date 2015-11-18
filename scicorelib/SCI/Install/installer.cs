using System.Runtime.InteropServices;

//using System.ComponentModel;

namespace SCI.Install
{
    internal class Installer
    {
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        public static extern int RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        public void InstallFont()
        {
            //			// Try install the font.
            //			int result = AddFontResource(@"C:\MY_FONT_LOCATION\MY_NEW_FONT.TTF");
            //			int error = Marshal.GetLastWin32Error();
            //			if ( error != 0 )
            //			{
            //				//Console.WriteLine(new Win32Exception(error).Message);
            //			}
        }
    }
}

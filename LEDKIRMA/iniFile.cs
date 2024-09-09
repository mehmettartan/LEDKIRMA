using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LEDKIRMA
{
    class iniFile
    {
        public string Path;
        public string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public iniFile(string iniPath = null)
        {
            Path = new FileInfo(iniPath ?? EXE + ".ini").FullName.ToString();
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? EXE, key, value, Path);
        }

        public string Read(string key, string section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? EXE, key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? EXE);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? EXE);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}

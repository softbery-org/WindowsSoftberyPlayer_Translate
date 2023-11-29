using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    public static class Language
    {
        private static IList<ILanguage> _langs = new List<ILanguage>();

        private static string _path = "languages/";

        public static string LanguageDir { get => _path; }

        public static IList<ILanguage> LoadLanguage(string dir_path = null)
        {
            if (dir_path != null)
            {
                setLanguagesDir(dir_path);
            }

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            var files = Directory.GetFiles(_path, "*.dll");

            if (files != null)
            {
                foreach (var file in files)
                {
                    var assembly = Assembly.LoadFrom(file);
                    var types = assembly.GetExportedTypes();

                    foreach (Type type in types)
                    {
                        if (type.GetInterfaces().Contains(typeof(ILanguage)))
                        {
                            var obj = Activator.CreateInstance(type);
                            _langs.Add(obj as ILanguage);
                        }
                    }
                }
            }
            return _langs;
        }

        private static void setLanguagesDir(string dir)
        {
            var dir_path = new DirectoryInfo(dir);
            if (dir_path.Exists)
            {
                _path = dir_path.FullName;
            }
        }

        public static object UseLanguage<T>(this T obj)
        {
            if (_langs.Contains((ILanguage)obj))
                return (T)Activator.CreateInstance(obj.GetType());
            else
                return null;
        }
    }
}

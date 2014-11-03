using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace WpfMdiApp1.CIL
{
    public class IoC
    {
        #region | Fields |

        private static IoC _instance;

        private Container _container;

        private bool _isVerified;

        #endregion

        #region | Properties |

        public static IoC Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IoC();
                }

                return _instance;
            }
        }

        public bool IsVerified { get { return _isVerified; } }

        public Dictionary<string, string> Params { get; private set; }

        public List<string> PackageList { get; private set; }

        #endregion

        #region | Constructor |

        private IoC()
        {
            Params = new Dictionary<string, string>();

            PackageList = new List<string>();

            _container = new Container();

            SetupContainer();
        }

        #endregion

        #region | Methods |

        //  Создание ноовго экземпляра контейнера
        [DebuggerStepThrough]
        public void CreateContainer()
        {
            _container = new Container();

            SetupContainer();
        }

        //  Подготовка контейнера
        [DebuggerStepThrough]
        private void SetupContainer()
        {
            _isVerified = false;

            _container.RegisterSingle<ILogger, Logger>();
        }

        //  Получение экземпляра сервиса из контейнера
        [DebuggerStepThrough]
        public TService GetInstance<TService>()
            where TService : class
        {
            if (!_isVerified)
            {
                _container.Verify();
                _isVerified = true;
            }

            return _container.GetInstance<TService>();
        }

        //  Получение коллекции экземпляров одного сервиса из контейнера
        [DebuggerStepThrough]
        public IEnumerable<TService> GetAllInstances<TService>()
            where TService : class
        {
            if (!_isVerified)
            {
                _container.Verify();
                _isVerified = true;
            }

            return _container.GetAllInstances<TService>();
        }

        //  Получение коллекции экземпляров одного сервиса из контейнера
        //[DebuggerStepThrough]
        public object GetInstance(Type type)
        {
            //if (!_isVerified) return null;

            //  weakly typed version
            return _container.GetInstance(type);
        }

        //  Регистрация единственного экземпляра сервиса (синглтон)
        [DebuggerStepThrough]
        public void RegisterSingle<TService, Service>()
            where TService : class
            where Service : class, TService
        {
            _container.RegisterSingle<TService, Service>();
        }

        //  Регистрация единственного экземпляра сервиса (синглтон) с параметризованным конструктором через делегат
        [DebuggerStepThrough]
        public void RegisterSingle<TService>(Func<TService> fun)
            where TService : class
        {
            _container.RegisterSingle<TService>(fun);
        }

        //  Регистрация экземпляра сервиса (новый инстанс на каждый вызов)
        [DebuggerStepThrough]
        public void Register<TService, Service>()
            where TService : class
            where Service : class, TService
        {
            _container.Register<TService, Service>();
        }

        //  Регистрация экземпляра сервиса (синглтон) с параметризованным конструктором через делегат
        [DebuggerStepThrough]
        public void Register<TService>(Func<TService> fun)
            where TService : class
        {
            _container.Register<TService>(fun);
        }

        //  Регистрация обощенного сингл экземпляра по обобщенному интерфейсу
        [DebuggerStepThrough]
        public void RegisterSingleOpenGeneric(Type T, Type U)
        {
            _container.RegisterSingleOpenGeneric(T, U);
        }

        //  Регистрация фиксированного списка экземпляров по общему интерфейсу
        [DebuggerStepThrough]
        public void RegisterAll<TService>(params Type[] types)
            where TService : class
        {
            _container.RegisterAll<TService>(types);
        }

        [DebuggerStepThrough]
        public void RegisterPackages(string projectPrefix)
        {
            string pluginDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var assemblies =
                from file in new DirectoryInfo(pluginDirectory).GetFiles()
                where file.Extension == ".dll" && file.Name.StartsWith(projectPrefix)
                select Assembly.LoadFile(file.FullName);

            foreach (var assembly in assemblies)
                PackageList.Add(string.Format("Assembly: {0}  Date: {1}",
                    assembly.FullName.Replace(", Culture=neutral, PublicKeyToken=null", ""),
                    RetrieveLinkerTimestamp(assembly)));

            RegisterPackages(assemblies);
        }

        [DebuggerStepThrough]
        public DateTime RetrieveLinkerTimestamp(Assembly assembly)
        {
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            string filePath = assembly.Location;

            byte[] b = new byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0);
            date = date.AddSeconds(secondsSince1970);
            date = date.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(date).Hours);
            return date;
        }

        [DebuggerStepThrough]
        private void RegisterPackages(IEnumerable<Assembly> assemblies)
        {
            if (_container == null)
                throw new ArgumentNullException("container");

            if (assemblies == null)
                throw new ArgumentNullException("assemblies");

            Type[] packageTypes =
                assemblies.SelectMany(GetExportedTypesFrom, (assembly, type) => new
                {
                    assembly,
                    type
                })
                    .Where(param => typeof(IPackage).IsAssignableFrom(param.type))
                    .Where(param => !param.type.IsAbstract)
                    .Where(param => !param.type.IsGenericTypeDefinition)
                    .Select(param => param.type)
                    .ToArray();

            RequiresPackageTypesHaveDefaultConstructor(packageTypes);

            foreach (IPackage package in packageTypes.Select(CreatePackage).ToArray())
                package.RegisterServices();
        }

        [DebuggerStepThrough]
        private IEnumerable<Type> GetExportedTypesFrom(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException ex)
            {
                return Enumerable.Empty<Type>();
            }
        }

        [DebuggerStepThrough]
        private void RequiresPackageTypesHaveDefaultConstructor(Type[] packageTypes)
        {
            Type type;

            type = packageTypes.FirstOrDefault(t => t.GetConstructor(Type.EmptyTypes) == (ConstructorInfo)null);

            if (!(type != null))
                return;

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                "The type {0} does not contain a default (public parameterless) constructor. Packages must have a default constructor.",
                new object[1] { type.FullName }));
        }

        [DebuggerStepThrough]
        private IPackage CreatePackage(Type packageType)
        {
            try
            {
                return (IPackage)Activator.CreateInstance(packageType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "The creation of package type {0} failed. " + ex.Message,
                        new object[0]), ex);
            }
        }

        #endregion
    }
}

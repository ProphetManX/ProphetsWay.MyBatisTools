using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using ProphetsWay.Utilities;

namespace ProphetsWay.MyBatisTools
{
	public static class MapperFactory
	{
		public static ISqlMapper GenerateMapper(this Assembly callingAssembly)
		{
			var assemblyName = callingAssembly.ManifestModule.Name;
			
			Logger.Debug(string.Format("Generating an ISqlMapper for {0}", assemblyName));

			var builderProps = new NameValueCollection();

			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MyBatisDBUserName"]))
				builderProps.Add("username", ConfigurationManager.AppSettings["MyBatisDBUserName"]);

			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MyBatisDBPassword"]))
				builderProps.Add("password", ConfigurationManager.AppSettings["MyBatisDBPassword"]);

			if (ConfigurationManager.ConnectionStrings["MyBatisDBConnectionString"] != null)
			{
				builderProps.Add("connectionString", ConfigurationManager.ConnectionStrings["MyBatisDBConnectionString"].ConnectionString);
				builderProps.Add("provider", ConfigurationManager.ConnectionStrings["MyBatisDBConnectionString"].ProviderName);
			}

			var builder = new DomSqlMapBuilder { ValidateSqlMapConfig = true, Properties = builderProps };
			var resources = callingAssembly.GetManifestResourceNames();

			try
			{
				var cfgResourceName = resources.Single(x => x.StartsWith(string.Format("{0}.SqlMap.", callingAssembly.GetName().Name)) && x.EndsWith(".config"));
				var mapper = builder.Configure(callingAssembly.GetManifestResourceStream(cfgResourceName));

				return mapper;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, string.Format("There was a problem when generating the SqlMapper for {0}", assemblyName));

				throw;
			}
		}
	}
}
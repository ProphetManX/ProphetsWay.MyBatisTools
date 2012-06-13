using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
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

			var assemblyUri = new Uri(callingAssembly.CodeBase);
			var assemblyFile = new FileInfo(assemblyUri.LocalPath);
			var assemblyPath = assemblyFile.Directory.FullName;
			var assemblyParts = assemblyName.Split('.');
			var assemblyLength = assemblyParts.Length;


			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MyBatisDBUserName"]))
				builderProps.Add("username", ConfigurationManager.AppSettings["MyBatisDBUserName"]);

			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MyBatisDBPassword"]))
				builderProps.Add("password", ConfigurationManager.AppSettings["MyBatisDBPassword"]);

			if (ConfigurationManager.ConnectionStrings["MyBatisDBConnection"] != null)
				builderProps.Add("connectionString", ConfigurationManager.ConnectionStrings["MyBatisDBConnection"].ConnectionString);
			

			var builder = new DomSqlMapBuilder { ValidateSqlMapConfig = true, Properties = builderProps };
			
			var mapper = builder.Configure(
				string.Format("{0}{1}SqlMap.{2}.config",
				              assemblyPath,
				              Path.DirectorySeparatorChar,
				              assemblyParts[assemblyLength - 2]));
			
			return mapper;
		}
	}
}

﻿using System.Collections.Generic;

namespace ProphetsWay.MyBatisTools
{
	/// <summary>
	/// Requires that all IDaos require the basic CRUD calls
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IBaseDao<T>
	{
		/// <summary>
		/// This will only require that the "ID" field be set, it will then load all properties from the DB and return a new Object.
		/// </summary>
		T Get(T item);

		void Insert(T item);

		int Update(T item);

		int Delete(T item);
	}

	/// <summary>
	/// Requires that all IDaos for Drop Down List items have a "GetAll" method
	/// </summary>
	/// <typeparam name="TGenericDDLItem"></typeparam>
	public interface IBaseDDLDao<TGenericDDLItem>  : IBaseDao<TGenericDDLItem> where TGenericDDLItem : BaseDDLItemClass
	{
		IList<TGenericDDLItem> GetAll(TGenericDDLItem item);
	}
}
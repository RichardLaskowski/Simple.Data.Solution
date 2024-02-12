using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using MySql.Data.Types;

namespace Simple.Data.Common.Extensions;
public static class DbDataReaderExtensions
{
    #region Public Methods

    public static T First<T>(this DbDataReader dbDataReader) where T : new()
    {
        if (!dbDataReader.HasRows)
        {
            throw new InvalidOperationException("The source DbDataReader has no rows.");
        }

        dbDataReader.Read();

        T firstItem = CreateItem<T>(dbDataReader, new T());

        return firstItem;
    }


    public async static Task<T> FirstAsync<T>(this DbDataReader dbDataReader, CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!dbDataReader.HasRows)
        {
            throw new InvalidOperationException("The source DbDataReader has no rows.");
        }

        await dbDataReader.ReadAsync(cancellationToken);

        T firstItem = CreateItem<T>(dbDataReader, new T());

        return firstItem;
    }


    public static T? FirstOrDefault<T>(this DbDataReader dbDataReader) where T : new()
    {
        T? firstItem = default;

        if (dbDataReader.HasRows)
        {
            dbDataReader.Read();
            firstItem = CreateItemOrDefault<T>(dbDataReader, new T());
        }

        return firstItem;
    }


    public async static Task<T?> FirstOrDefaultAsync<T>(this DbDataReader dbDataReader, CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        T? firstItem = default;

        if (dbDataReader.HasRows)
        {
            await dbDataReader.ReadAsync(cancellationToken);
            firstItem = CreateItemOrDefault<T>(dbDataReader, new T());
        }

        return firstItem;
    }


    public static List<T> ToList<T>(this DbDataReader dbDataReader) where T : new()
    {
        List<T> items = new List<T>();

        if (dbDataReader.HasRows)
        {
            while (dbDataReader.Read())
            {
                items.Add(CreateItem<T>(dbDataReader, new T()));
            }
        }

        return items;
    }


    public async static Task<List<T>> ToListAsync<T>(this DbDataReader dbDataReader, CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<T> items = new List<T>();

        if (dbDataReader.HasRows)
        {
            while (await dbDataReader.ReadAsync(cancellationToken))
            {
                items.Add(CreateItem<T>(dbDataReader, new T()));
            }
        }

        return items;
    }


    #endregion Public Methods


    #region Private Methods

    private static T CreateItem<T>(DbDataReader dbDataReader, T item) where T : new()
    {
        if (item is not null)
        {
            foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
            {
                bool isNullValue;

                try
                {
                    isNullValue = dbDataReader.IsDBNull(dbDataReader.GetOrdinal(propertyInfo.Name));
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }

                if (isNullValue)
                {
                    propertyInfo.SetValue(item, null);
                }
                else
                {
                    Type propertyType = IsNullable(propertyInfo.PropertyType)
                        ? Nullable.GetUnderlyingType(propertyInfo.PropertyType)!
                        : propertyInfo.PropertyType;

                    try
                    {
                        switch (propertyType.Name)
                        {
                            case "Bool": SetBoolProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Byte": SetByteProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Char": SetCharProperty(ref item, dbDataReader, propertyInfo); break;
                            case "DateTime": SetDateTimeProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Decimal": SetDecimalProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Double": SetDoubleProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Float": SetFloatProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Guid": SetGuidProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Int32": SetIntProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Long": SetLongProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Short": SetShortProperty(ref item, dbDataReader, propertyInfo); break;
                            case "String": SetStringProperty(ref item, dbDataReader, propertyInfo); break;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                    catch (InvalidCastException)
                    {
                        continue;
                    }
                }
            }
        }

        return item;
    }


    private static T? CreateItemOrDefault<T>(DbDataReader dbDataReader, T? item = default(T)) where T : new()
    {
        if (item is not null)
        {
            if (dbDataReader.HasRows)
            {
                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties(BindingFlags.Instance))
                {
                    bool isNullValue;

                    try
                    {
                        isNullValue = dbDataReader.IsDBNull(dbDataReader.GetOrdinal(propertyInfo.Name));
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }

                    if (isNullValue)
                    {
                        propertyInfo.SetValue(item, null);
                    }
                    else
                    {
                        Type propertyType = IsNullable(propertyInfo.PropertyType)
                            ? Nullable.GetUnderlyingType(propertyInfo.PropertyType)!
                            : propertyInfo.PropertyType;

                        switch (propertyType.Name)
                        {
                            case "Bool": SetBoolProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Byte": SetByteProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Char": SetCharProperty(ref item, dbDataReader, propertyInfo); break;
                            case "DateTime": SetDateTimeProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Decimal": SetDecimalProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Double": SetDoubleProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Float": SetFloatProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Guid": SetGuidProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Int32": SetIntProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Long": SetLongProperty(ref item, dbDataReader, propertyInfo); break;
                            case "Short": SetShortProperty(ref item, dbDataReader, propertyInfo); break;
                            case "String": SetStringProperty(ref item, dbDataReader, propertyInfo); break;
                        }
                    }
                }
            }
        }

        return item;
    }


    private static void SetBoolProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        bool? value = dbDataReader.GetBoolean(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetByteProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        byte? value = dbDataReader.GetByte(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetCharProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        char? value = dbDataReader.GetChar(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetDateTimeProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        DateTime? value;

        try
        {
            value = dbDataReader.GetDateTime(dbDataReader.GetOrdinal(propertyInfo.Name));
        }
        catch (MySqlConversionException)
        {
            MySqlDateTime mySqlDateTime = (MySqlDateTime)dbDataReader.GetValue(dbDataReader.GetOrdinal(propertyInfo.Name));

            value = mySqlDateTime.IsValidDateTime
                ? mySqlDateTime.GetDateTime()
                : DateTime.MinValue;
        }

        propertyInfo.SetValue(item, value);
    }


    private static void SetDecimalProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        decimal? value = dbDataReader.GetDecimal(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetDoubleProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        double? value = dbDataReader.GetDouble(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);

    }


    private static void SetFloatProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        float? value = dbDataReader.GetFloat(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetGuidProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        Guid? value = dbDataReader.GetGuid(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetIntProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        int? value = dbDataReader.GetInt32(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetLongProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        long? value = dbDataReader.GetInt64(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetShortProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        short? value = dbDataReader.GetInt16(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }


    private static void SetStringProperty<T>(ref T item, DbDataReader dbDataReader, PropertyInfo propertyInfo) where T : new()
    {
        string? value = dbDataReader.GetString(dbDataReader.GetOrdinal(propertyInfo.Name));

        propertyInfo.SetValue(item, value);
    }

    private static bool IsNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    #endregion Private Methods
}

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;

namespace EfCore.Attempt2.EnumHelpers
{
    public class Seeder
    {
        public static void SeedEnumData<TData, TEnum>(DbSet<TData> items)
            where TData : EnumBase<TEnum>
            where TEnum : struct
        {
            var etype = typeof(TEnum);

            if (!etype.IsEnum)
                throw new Exception($"Type '{etype.AssemblyQualifiedName}' must be enum");

            var ntype = Enum.GetUnderlyingType(etype);

            if (ntype == typeof(long) || ntype == typeof(ulong) || ntype == typeof(uint))
                throw new Exception();

            foreach (TEnum evalue in Enum.GetValues(etype))
            {
                var id = (int)Convert.ChangeType(evalue, typeof(int));
                if (id <= 0)
                    throw new Exception("Enum underlying value must be positive");

                if (!items.Any(a => a.Id ==id))
                {
                    var item = Activator.CreateInstance<TData>();
                    item.Id = id;
                    item.Name = Enum.GetName(etype, evalue);
                    item.Description = GetEnumDescription(evalue);
                    items.Add(item);
                }       
            }
        }

        static string GetEnumDescription<TEnum>(TEnum item)
        {
            Type type = item.GetType();

            var attribute = type.GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
            return attribute == null ? string.Empty : attribute.Description;
        }
    }

}

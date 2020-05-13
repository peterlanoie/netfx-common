using System;
using System.Collections.Generic;
using System.Text;

namespace System.Reflection
{
	public static class ReflectionAssemblyExt
	{

		/// <summary>
		/// Gets all members of <paramref name="assembly"/> that are decorated with the specified <paramref name="attributeType"/>.
		/// </summary>
		/// <param name="assembly">Source assembly to search.</param>
		/// <param name="attributeType">Type of the attribute that decorates the desired types.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetTypesWithAttribute(this System.Reflection.Assembly assembly, Type attributeType)
		{
			foreach(Type type in assembly.GetTypes())
			{
				if(type.GetCustomAttributes(attributeType, true).Length > 0)
				{
					yield return type;
				}
			}
		}
		
		/// <summary>
		/// Gets all members of <paramref name="assembly"/> that are derived from <paramref name="type"/>.
		/// </summary>
		/// <param name="assembly">Source assembly to search.</param>
		/// <param name="type">The super type of which the returned types must be a subclass.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetMembersDerivedFrom(this Assembly assembly, Type type)
		{
			foreach(var member in assembly.GetTypes())
			{
				if(member.IsSubclassOf(type))
				{
					yield return member;
				}
			}
		}

	}
}

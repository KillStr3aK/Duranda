using System.Reflection;

namespace Duranda
{
    class Reflection
    {
		private static readonly BindingFlags FieldProperty = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;
		private static readonly BindingFlags Method = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		public static Return GetPrivateFieldValue<Return, Class>(Class pThis, string field)
		{
			var info = pThis.GetType().GetField(field, FieldProperty);
			return (Return)info.GetValue(pThis);
		}

		public static void SetPrivateFieldValue<Class, Value>(Class pThis, string field, Value value)
		{
			var info = pThis.GetType().GetField(field, FieldProperty);
			info.SetValue(pThis, value);
		}

		public static void CallPrivateVoidMethod<Class>(Class pThis, string method, params object[] args)
		{
			MethodInfo targetMethod = pThis.GetType().GetMethod(method, Method);
			targetMethod.Invoke(pThis, args);
		}
	}
}
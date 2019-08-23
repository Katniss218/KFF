
namespace KFF
{
	internal static class MathHelper
	{
		internal static bool HasDecimalDigits( double d )
		{
			return d - (long)d != 0d;
		}
	}
}
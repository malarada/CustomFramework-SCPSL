namespace CustomFramework.CustomItems
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class CustomItemAttribute : System.Attribute
	{
		public ItemType Item;

		public CustomItemAttribute(ItemType item)
		{
			Item = item;
		}
	}
}

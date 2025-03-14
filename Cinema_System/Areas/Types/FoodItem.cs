namespace Cinema_System.Areas.Types
{
    public class FoodItem
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }

        public FoodItem() { }

        public FoodItem(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}

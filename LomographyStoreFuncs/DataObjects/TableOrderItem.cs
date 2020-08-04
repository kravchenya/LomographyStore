namespace LomographyStoreFuncs.DataObjects
{
    public class TableOrderItem : OrderItem
    {
        public TableOrderItem()
        {}
        public TableOrderItem(OrderItem item)
        {
            base.Id = item.Id;
            base.Name = item.Name;
            base.Description = item.Description;
            base.Camera = item.Camera;
        }
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }
    }
}
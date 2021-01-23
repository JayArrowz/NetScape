namespace NetScape.Abstractions.Model.Area
{
    public class ObjectType
    {
        public static readonly ObjectType Lengthwise_Wall = new(0, ObjectGroup.Wall);
        public static readonly ObjectType Triangular_Corner = new(1, ObjectGroup.Wall);
        public static readonly ObjectType Wall_Corner = new(2, ObjectGroup.Wall);
        public static readonly ObjectType Rectangular_Corner = new(3, ObjectGroup.Wall);
        public static readonly ObjectType Diagonal_Wall = new(9, ObjectGroup.Interactable_Object);
        public static readonly ObjectType Interactable = new(10, ObjectGroup.Interactable_Object);
        public static readonly ObjectType Diagonal_Interactable = new(11, ObjectGroup.Interactable_Object);
        public static readonly ObjectType Floor_Decoration = new(22, ObjectGroup.Ground_Decoration);

        public ObjectType(int value, ObjectGroup group)
        {
            Value = value;
            Group = group;
        }

        public int Value { get; }
        public ObjectGroup Group { get; }
    }
}

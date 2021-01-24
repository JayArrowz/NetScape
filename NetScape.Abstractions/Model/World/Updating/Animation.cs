namespace NetScape.Abstractions.Model.World.Updating
{
    public record Animation
    {
        public static readonly Animation StopAnimation = new Animation(-1);
        public int Delay { get; }
        public int Id { get; }

        public Animation(int id) : this(id, 0) { }

        public Animation(int id, int delay)
        {
            Id = id;
            Delay = delay;
        }
    }
}

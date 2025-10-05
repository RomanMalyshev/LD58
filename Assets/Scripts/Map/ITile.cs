namespace Model.Map
{
    public interface ITile
    {
        public bool IsOccupied { get; }
        public bool IsReadyToOccupy { get; }
        public void SetOccupied(bool occupied);
        public void SetReadyToOccupy(bool ready);
    }
}